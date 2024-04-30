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

        public async Task<IActionResult> LogIn(LoginVM data)
        {
            var user = await _userManager.FindByEmailAsync(data.Email.ToLower());

            if (user == null)
            {
                return RedirectToAction("Index");
            }

            var result = await _signInManager.PasswordSignInAsync(user, data.Password, false, false);

            if (!result.Succeeded)
                throw new Exception("There is an issue with your Sign In!");

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> SignUp(LoginVM data)
        {
            var user = await _userManager.FindByEmailAsync(data.Email.ToLower());

            if (user != null)
                return RedirectToAction("Index");

            user = new User()
            {
                Id = Guid.NewGuid(),
                UserName = data.Name,
                Email = data.Email.ToLower(),
                EmailConfirmed = true,
            };

            await _userManager.CreateAsync(user, data.Password);

            if (user.Email.EndsWith("@gamedevs.com"))
                await _userManager.AddToRoleAsync(user, "Developer");
            else
                await _userManager.AddToRoleAsync(user, "User");

            var result = await _signInManager.PasswordSignInAsync(user, data.Password, false, false);

            if (!result.Succeeded)
                throw new Exception("There is an issue with your Sign Up!");

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Login", new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

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

            else
            {
                var userEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (userEmail != null)
                {
                    var user = await _userManager.FindByEmailAsync(userEmail);

                    if (user == null)
                    {
                        var name = info.Principal.FindFirstValue(ClaimTypes.GivenName);

                        user = new User()
                        {
                            Id = Guid.NewGuid(),
                            UserName = name,
                            Email = userEmail,
                            EmailConfirmed = true,
                        };

                        await _userManager.CreateAsync(user);

                        await _userManager.AddToRoleAsync(user, "User");
                    }

                    await _userManager.AddLoginAsync(user, info);

                    await _signInManager.SignInAsync(user, false);

                    return RedirectToAction("Index", "Home");
                }

            }

            ModelState.AddModelError("", $"Something went wrong");
            return View("Login", model);
        }
    }
}
