using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Fiver.Security.AspIdentity.Services.Identity;

namespace Fiver.Security.AspIdentity.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<AppIdentityUser> userManager;

        public UsersController(
            UserManager<AppIdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            var viewModel = this.userManager.Users.ToList();
            return View(viewModel);
        }
    }
}
