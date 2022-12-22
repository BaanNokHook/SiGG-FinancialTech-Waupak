using System;
using System.Data;
using System.IO;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.PaymentProcess;
using GM.Data.Model.Report;
using GM.Data.Result.PaymentProcess;

namespace GM.Application.Web.Areas.Report.Controllers
{

    public class RPConfirmationReportController : Controller 
    {
        // GET: RPConfirmation  
        public ActionResult Index(string trans_no)  
        {
            var dt = new DataTable();  
            dt = GetConfirmation(trans_no);  

            var rd = new ReportDocument();   
            if (dt.Rows[0]["cur"].ToString() == "THB")   
            {
                  rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),   
                    "RPConfirmation.rpt"));   
                  rd.SetDataSource(dt);
            }
            else
            {
                  rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),   
                     "RPConfirmationFCY.rpt"));  
                  rd.SetDataSource(dt);   
            }  

            Response.Buffer = false;  
            Response.ClearContent();   
            Response.ClearHeaders();   
            var stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);   
            stream.Seek(0, SeekOrigin.Begin);  
            return new FileStreamResult(stream, "application/pdf"); 
            //return File(stream, "application/pdf", "ConfirmationReport.pdf");  
        }

        [HttpGet]  
        public ActionResult DownloadPDF(string trans_no, string print_confirm_bo1_by, string print_confirm_bo2_by, string type_code, string access_token)    
        {
            if (string.IsNullOrEmpty(access_token) || !access_token.Equals("repo2022"))   
            {
                return RedirectToAction("NoAccess");   
            }

            //update sign name 
            if (!string.IsNullOrEmpty(print_confirm_bo1_by) || !string.IsNullOrEmpty(print_confirm_bo2_by))   
            {
                  var api = new PaymentProcessEntities();   
                  var arrTransNo = trans_no.Split(',');   
                  foreach (var transNo in arrTransNo)   
                  {
                        var rwm = new ResultWithModel<RPConfirmationResult>();   
                        var updateModel = new RPConfirmationModel();   
                        updateModel.update_by = "REPORT_CONFIRMATION";  
                        updateModel.trans_no = transNo;  
                        updateModel.print_confirm_bo1_by = print_confirm_bo1_by;  
                        updateModel.print_confirm_bo2_by = print_confirm_bo2_by;   
                        
                        api.RPConfirmation.UpdateSignName(updateModel, p => { rwm = p; });     
                  }   
            }   

            var rd = new ReportDocument();  
            var dtResult = GetConfirmation(trans_no);  

            if (dtResult.Rows.Count > 0)   
            {
                  if (dtResult.Rows[0]["cur"].ToString() == "THB")    
                  {
                        var rwm = new ResultWithModel<RPConfirmationResult>();   
                        var updateModel = new RPConfirmation();   
                        updateModel.update_by = "REPORT_CONFIRMATION";  
                        updateModel.trans_no = transNo;   
                        updateModel.print_confirm_bo1_by = print_confirm_bo1_by;  
                        updateModel.print_confirm_bo2_by = print_confirm_bo2_by;   

                        api.RPConfirmation.UpdateSignName(updateModel, p => { rwm = p; });   
                  }
            }

            var rd = new ReportDocument();   
            var dtResult = GetConfirmation(trans_no);  

            if (dtResult.Rows.Count > 0)   
            {
                  if (dtResult.Rows[0]["cur"].ToString() == "THB")   
                  {
                     
                      rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),   
                         "RPConfirmation.rpt"));   
                      rd.SetDataSource(dtResult);  
                  }  
                  else  
                  {
                        rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),    
                          "RPConfirmationFCY.rpt"));   
                        rd.SetDataSource(dtResult);  
                  }  

                  if (string.Equals(type_code, "A", StringComparison.CurrentCultureIgnoreCase))   
                  {
                     rd.SetParameterValue("type_code", "AMENN");  
                  }
                  else if (string.Equals(type_code, "C", StringComparison.CurrentCultureIgnoreCase))   
                  {
                        rd.SetParameterValue("type_code", "");   
                  }  

                  Response.Buffer = false;   
                  Response.ClearContent();  
                  Response.ClearHeaders();   
                  Response.AppendHeader("Content-disposition", "attachment; filename=RPConfirmation.pdf");   
                  var stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);  
                  stream.Seek(0, SeekOrigin.Begin);   
                  return new FileStreamResult(stream, "application/pdf");    
                  //return File(stream, "application/pdf", "ConfirmationReport.pdf");
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

        public DataTable GetConfirmation(string transno)   
        {
            var apiReport = new ReportEntities();   
            var dt = new DataTable();  
            //SqlParameter outRefCode = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //SqlParameter outMessage = new SqlParameter("@Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };
            //SqlParameter outServerity = new SqlParameter("@Serverity", SqlDbType.VarChar, 15) { Direction = ParameterDirection.Output };
            //SqlParameter outHowManyRecord = new SqlParameter("@HowManyRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

            //dt = (new DbHelper()).GetTableRow("RP_Report_Confirmation_Proc", new[] { outRefCode, outMessage, outServerity, outHowManyRecord,
            //     new SqlParameter("@trans_no", transno) });   
            var data = new ReportCriteriaModel();   
            data.trans_no = transno;    
            apiReport.ReportData.RPConfirmationReport(data, p =>  
            {
                  if (p.Success) dt = p.Data.RPConfirmationResultModel.ToDataTable();    
            });      

            return dt;     
        }

    }

}