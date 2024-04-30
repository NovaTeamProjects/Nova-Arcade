using Microsoft.AspNetCore.Mvc;

namespace Game_Store.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SystemRequirement()
        {
            return View();
        }
    }
}
