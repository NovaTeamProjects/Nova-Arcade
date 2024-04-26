using Game_Store.Domain.Entities.Auth;
using Game_Store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Game_Store.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public LoginController(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index(string? ReturnUrl = null)
        {
            var model = new LoginVM
            {
                ReturnUrl = ReturnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            //This call will generate a URL that directs to the ExternalLoginCallback action method in the Account controller
            //with a route parameter of ReturnUrl set to the value of returnUrl.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Login", new { ReturnUrl = returnUrl });

            // Configure the redirect URL, provider and other properties
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            //This will redirect the user to the external provider's login page
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl, string remoteError)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var model = new LoginVM()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", model);
            }

            // Get the login information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError("", "Error loading external login information.");
                return View("Login", model);
            }

            // If the user already has a login (i.e., if there is a record in AspNetUserLogins table)
            // then sign-in the user with this external login provider
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
                return RedirectToAction("Index", "Home");

            // If there is no record in AspNetUserLogins table, the user may not have a local account
            else
            {
                var userEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (userEmail != null)
                {
                    var user = await _userManager.FindByEmailAsync(userEmail);

                    if (user == null)
                    {
                        var name = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                        var surname = info.Principal.FindFirstValue(ClaimTypes.Surname);

                        user = new User()
                        {
                            Id = Guid.NewGuid(),
                            Name = name!,
                            Surname = surname!,
                            Email = userEmail,
                            EmailConfirmed = true,
                        };

                        await _userManager.CreateAsync(user);

                        await _userManager.AddToRoleAsync(user, "User");
                    }

                    // Add a login (i.e., insert a row for the user in AspNetUserLogins table)
                    await _userManager.AddLoginAsync(user, info);

                    await _signInManager.SignInAsync(user, false);

                    return RedirectToAction("Index", "Home");
                }

            }

            ModelState.AddModelError("", $"Something went wrong");
            return View("Login", model);
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
