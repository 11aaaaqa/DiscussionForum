using Microsoft.AspNetCore.Mvc;

namespace Web.MVC.Controllers
{
    public class InformationController : Controller
    {
        [Route("info/forbidden")]
        public IActionResult AccessIsForbidden()
        {
            return View();
        }

        [Route("not-found")]
        public IActionResult RouteIsNotFound()
        {
            return View();
        }

        [Route("error")]
        public IActionResult ActionError()
        {
            return View();
        }
    }
}
