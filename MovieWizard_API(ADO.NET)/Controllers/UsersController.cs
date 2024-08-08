using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace MovieWizard_API_ADO.NET_.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
