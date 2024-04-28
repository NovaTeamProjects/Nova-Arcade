using AspNetCoreRateLimit;
using Game_Store.Domain.Entities.Auth;
using Game_Store.Infrastructure;
using Game_Store.Infrastructure.Persistance;
using Game_Store.Middlewares;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Reflection;

namespace Game_Store
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure logging
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
            //builder.Logging.ClearProviders(); // write to console without it
            builder.Logging.AddSerilog(logger);

            // Add services
            builder.Services.AddControllersWithViews();

            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<NovaStoreDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["Auth:Google:ClientId"]!;
                    options.ClientSecret = builder.Configuration["Auth:Google:ClientSecret"]!;
                })
                .AddFacebook(options =>
                {
                    options.AppId = builder.Configuration["Auth:Facebook:AppId"]!;
                    options.AppSecret = builder.Configuration["Auth:Facebook:AppSecret"]!;
                    options.AccessDeniedPath = "/AccessDeniedPathInfo";
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Creator", policy => policy.RequireRole("Creator"));
                options.AddPolicy("Developer", policy => policy.RequireRole("Developer"));
                options.AddPolicy("User", policy => policy.RequireRole("User"));
            });


            builder.Services.AddRazorPages();
            builder.Services.AddMemoryCache();
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
            builder.Services.AddInMemoryRateLimiting();
            builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseMiddleware<GlobalExceptionHandling>();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseIpRateLimiting();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            using (var scope = app.Services.CreateScope())
            {
                var roleManager =
                    scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                var roles = new[] { "Creator", "Developer", "User" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            app.Run();
        }
    }
}
