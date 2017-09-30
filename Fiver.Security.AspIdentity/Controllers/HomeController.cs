using Microsoft.AspNetCore.Mvc;

namespace Fiver.Security.AspIdentity.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
