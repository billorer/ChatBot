using System.Web.Mvc;

namespace Backend.Controllers
{
    public class ContactController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Anonym";
            return View();
        }
    }
}
