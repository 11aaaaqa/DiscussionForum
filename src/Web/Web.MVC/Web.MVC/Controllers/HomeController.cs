using Microsoft.AspNetCore.Mvc;

namespace Web.MVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
