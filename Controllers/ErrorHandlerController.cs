using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    public class ErrorHandlerController : Controller
    {
        public ActionResult NotFound()
        {
            return View();
        }
    }
}