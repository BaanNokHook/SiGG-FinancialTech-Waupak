using GM.CommonLibs.Constants;
using GM.Filters;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class ReportUserController : BaseController
    {
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            return View();
        }
    }
}