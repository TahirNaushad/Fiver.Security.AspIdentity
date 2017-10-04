using Fiver.Security.AspIdentity.Models.Security;
using Fiver.Security.AspIdentity.Services.Email;
using Fiver.Security.AspIdentity.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Fiver.Security.AspIdentity.Controllers
{
    public class SecurityController : Controller
    {
        #region " Fields & Constructor "

        private readonly UserManager<AppIdentityUser> userManager;
        private readonly SignInManager<AppIdentityUser> signInManager;
        private readonly IEmailSender emailSender;

        public SecurityController(
            UserManager<AppIdentityUser> userManager,
            SignInManager<AppIdentityUser> signInManager,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
        }

        #endregion

        #region " Login / Logout / Access Denied "

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await this.userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                if (!await this.userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty,
                              "Confirm your email please");
                    return View(model);
                }
            }

            var result = await this.signInManager.PasswordSignInAsync(
                model.Username, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError(string.Empty, "Login Failed");
            return View(model);
        }
        
        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion

        #region " Register "

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new AppIdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
                Age = model.Age
            };

            var result = await this.userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                //await this.signInManager.SignInAsync(user, isPersistent: false);

                var confrimationCode = 
                    await this.userManager.GenerateEmailConfirmationTokenAsync(user);

                var callbackurl = Url.Action(
                    controller: "Security",
                    action: "ConfirmEmail",
                    values: new { userId = user.Id, code = confrimationCode },
                    protocol: Request.Scheme);

                await this.emailSender.SendEmailAsync(
                    email: user.Email,
                    subject: "Confirm Email",
                    message: callbackurl);

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
        
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return RedirectToAction("Index", "Home");

            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");

            var result = await this.userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return View("ConfirmEmail");

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region " Forgot Password "

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return View();

            var user = await this.userManager.FindByEmailAsync(email);
            if (user == null)
                return RedirectToAction("ForgotPasswordEmailSent");

            if (!await this.userManager.IsEmailConfirmedAsync(user))
                return RedirectToAction("ForgotPasswordEmailSent");

            var confrimationCode =
                    await this.userManager.GeneratePasswordResetTokenAsync(user);

            var callbackurl = Url.Action(
                controller: "Security",
                action: "ResetPassword",
                values: new { userId = user.Id, code = confrimationCode },
                protocol: Request.Scheme);

            await this.emailSender.SendEmailAsync(
                email: user.Email,
                subject: "Reset Password",
                message: callbackurl);

            return RedirectToAction("ForgotPasswordEmailSent");
        }

        public IActionResult ForgotPasswordEmailSent()
        {
            return View();
        }

        #endregion

        #region " Reset Password "

        public IActionResult ResetPassword(string userId, string code)
        {
            if (userId == null || code == null)
                throw new ApplicationException("Code must be supplied for password reset.");

            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await this.userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("ResetPasswordConfirm");

            var result = await this.userManager.ResetPasswordAsync(
                                        user, model.Code, model.Password);
            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirm");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
           
            return View(model);
        }

        public IActionResult ResetPasswordConfirm()
        {
            return View();
        }

        #endregion
    }
}
