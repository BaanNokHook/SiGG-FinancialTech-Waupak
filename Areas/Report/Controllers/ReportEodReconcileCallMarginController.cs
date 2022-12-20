using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GM.Application.Web.Controllers;
using GM.Data.Model.Report;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GM.Data.Entity;
using GM.Data.Helper;

namespace GM.Application.Web.Areas.Report.Controllers
{
    public class ReportEodReconcileCallMarginController : Controller
    {
        private readonly ReportEntities apiReport = new ReportEntities();
        private readonly Utility utility = new Utility();

        [HttpGet]
        public ActionResult Index(string asofDate, string type, string access_token)
        {
            if (string.IsNullOrEmpty(access_token) || !access_token.Equals("repo2022"))
            {
                return RedirectToAction("NoAccess");
            }

            try
            {
                ReportCriteriaModel model = new ReportCriteriaModel();
                model.asofdate = utility.ConvertStringToDatetimeFormatDDMMYYYY(asofDate);

                List<EodReconcileCallMarginReportModel> resList = new List<EodReconcileCallMarginReportModel>();
                apiReport.ReportData.EodReconcileCallMarginReport(model, type, p =>
                {
                    resList = !p.Success ? throw new Exception(p.Message) : p.Data.EodReconcileCallMarginReportResultModel;
                });

                DataTable dt = resList.ToDataTable();

                var rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),
                        "EodReconcileCallMarginReport.rpt"));
                rd.SetDataSource(dt);
                rd.SetParameterValue("as_of_date", model.asofdate);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                var stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return new FileStreamResult(stream, "application/pdf");

            }
            catch(Exception ex)
            {
                return RedirectToAction("Error", new { error = ex.Message });
            }
        }

        public ActionResult NoAccess()
        {
            return Content("No access", "text/html");
        }

        public ActionResult Error(string error)
        {
            return Content(error, "text/html");
        }
    }
}