using Microsoft.AspNetCore.Mvc;
using Prometheus;

namespace Web.MVC.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Counter HomePageVisitingCounter = Metrics.CreateCounter
            ("home_page_visiting", "Number of times visited home page.");
        
        public IActionResult Index()
        {
            HomePageVisitingCounter.Inc();

            return View();
        }
    }
}
