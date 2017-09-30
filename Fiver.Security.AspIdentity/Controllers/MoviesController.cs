using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Fiver.Security.AspIdentity.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
