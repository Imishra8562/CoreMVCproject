using Microsoft.AspNetCore.Mvc;

namespace CoreMVCproject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
