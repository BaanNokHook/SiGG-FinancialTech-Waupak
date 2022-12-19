using System.Web.Mvc;

namespace GM.Application.Web.Areas.Report
{
    public class ReportAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Report";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Report_default",
                "Report/{controller}/{action}/{id}",
                new {action = "Index", id = UrlParameter.Optional}
            );
        }
    }
}