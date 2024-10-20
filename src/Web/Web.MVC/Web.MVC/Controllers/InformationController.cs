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
    }
}
