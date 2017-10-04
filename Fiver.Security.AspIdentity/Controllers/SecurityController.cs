using Fiver.Security.AspIdentity.Lib;
using Fiver.Security.AspIdentity.Models.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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

        #region " Login "

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel inputModel)
        {
            if (!ModelState.IsValid)
                return View(inputModel);

            var user = await this.userManager.FindByNameAsync(inputModel.Username);
            if (user != null)
            {
                if (!await this.userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty,
                              "Confirm your email please");
                    return View(inputModel);
                }
            }

            var result = await this.signInManager.PasswordSignInAsync(
                inputModel.Username, inputModel.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError(string.Empty, "Login Failed");
            return View(inputModel);
        }

        #endregion

        #region " Logout "

        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region " Register "

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterInputModel inputModel)
        {
            if (!ModelState.IsValid)
                return View(inputModel);

            var user = new AppIdentityUser
            {
                UserName = inputModel.UserName,
                Email = inputModel.Email,
                Age = inputModel.Age
            };

            var result = await this.userManager.CreateAsync(user, inputModel.Password);
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

            return View(inputModel);
        }

        #endregion

        #region " Access "

        public IActionResult Access()
        {
            return View();
        }

        #endregion

        #region " Users "

        [Authorize]
        public IActionResult ListUsers()
        {
            var viewModel = this.userManager.Users.ToList();
            return View(viewModel);
        }

        #endregion

        #region " Email Confirmation (after Registration) "

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
    }
}
