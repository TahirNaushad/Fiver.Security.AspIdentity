using Fiver.Security.AspIdentity.Models.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fiver.Security.AspIdentity.Controllers
{
    public class SecurityController : Controller
    {
        private readonly UserManager<AppIdentityUser> userManager;
        private readonly SignInManager<AppIdentityUser> signInManager;

        public SecurityController(
            UserManager<AppIdentityUser> userManager,
            SignInManager<AppIdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

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
                await this.signInManager.SignInAsync(user, isPersistent: false);
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
    }
}
