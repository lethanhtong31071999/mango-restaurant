using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    public class DashboardController : Controller
    {
        // In the future change sidebar into Layout of Admin
        public IActionResult Index()
        {
            return View();
        }
    }
}
