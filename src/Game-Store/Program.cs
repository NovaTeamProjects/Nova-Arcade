using Game_Store.Domain.Entities.Auth;
using Game_Store.Infrastructure;
using Game_Store.Infrastructure.Persistance;
using Game_Store.Middlewares;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Game_Store
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<NovaStoreDbContext>()
                .AddDefaultTokenProviders();

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
            //builder.Logging.ClearProviders(); // write to console without it
            builder.Logging.AddSerilog(logger);

            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["Auth:Google:ClientId"];
                    options.ClientSecret = builder.Configuration["Auth:Google:ClientSecret"];
                });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseMiddleware<GlobalExceptionHandling>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //using (var scope = app.Services.CreateScope())
            //{
            //    var roleManager =
            //        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //    var roles = new[] { "Admin", "User" };
                
            //    foreach (var role in roles)
            //    {
            //        if (!await roleManager.RoleExistsAsync(role))
            //            await roleManager.CreateAsync(new IdentityRole(role));
            //    }
            //}

            app.Run();
        }
    }
}
