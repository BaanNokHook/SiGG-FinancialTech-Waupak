using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Report;
using System.Data;
using System.IO;
using System.Web.Mvc;

namespace GM.Application.Web.Areas.Report.Controllers
{

    public class ReportMarginNoticeController : Controller  
    {

        private readonly Utility = new Utility();   

        // GET: RPConfirmation  
        public ActionResult Index(string asofdate, string counter_party_id)  
        {
            var rd = new ReportDocument();   
            var dtResult = GetDataReport(asofdate, counter_party_id);    

            if (dtResult.Rows.Count > 0)   

                  rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),   
                      "MarginNoticeReport.rpt"));  
                  rd.SetDataSource(dtResult);   

                  Response.Buffer = false;  
                  Response.ClearContent();  
                  Response.ClearHeaders();   
                  Response.AppendHeader("Content-disposition", "attachment;  filename=MarginNoticeReport.doc");   
                  var stream = rd.ExportToStream(ExportFormatType.WordForWindows);  
                  stream.Seek(0, SeekOrigin.Begin);   
                  return new FileStreamResult(stream. "application/vnd.ms-word.document");  
            }

            return RedirectToAction("NoData");  
        }

        [HttpGet]  
        public ActionResult DownloadPDF(string asofdate, string counter_party_id, string access_token)  
        {
            if (string.IsNullOrEmpty(access_token) || !access_token.Equals("repo2022"))   
            {
                  return RedirectToAction("NoAccess");  
            } 

            var rd = new ReportDocument();  
            var dtResult = GetDataReport(asofdate, counter_party_id);  

            if (dtResult.Rows.Count > 0) 
            {

                rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),
                    "MarginNoticeReport.rpt"));
                rd.SetDataSource(dtResult);
                //rd.FileName = "MarginNoticeReport.doc";

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Response.AppendHeader("Content-disposition", "attachment; filename=MarginNoticeReport.docx");
                var stream = rd.ExportToStream(ExportFormatType.WordForWindows);
                stream.Seek(0, SeekOrigin.Begin);
                return new FileStreamResult(stream, "application/vnd.ms-word.document");
            }

            return RedirectToAction("NoData");
        }

        public ActionResult NoAccess()
        {
            return Content("No access", "text/html");
        }

        public ActionResult NoData()
        {
            return Content("No data", "text/html");
        }

        public DataTable GetDataReport(string asofdate, string counter_party_id)
        {
            var apiReport = new ReportEntities();
            var dt = new DataTable();
            var data = new ReportCriteriaModel();
            data.asofdate = utility.ConvertStringToDatetimeFormatDDMMYYYY(asofdate);
            data.counterparty_id = counter_party_id;
            apiReport.ReportData.MarginNotiecReport(data, p =>
            {
                if (p.Success) dt = p.Data.MarginNoticeReportResultModel.ToDataTable();
            });

            return dt;
        }
}