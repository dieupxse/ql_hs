using Microsoft.AspNetCore.Mvc;

namespace QL_HS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("/index.html");
        }
    }
}
