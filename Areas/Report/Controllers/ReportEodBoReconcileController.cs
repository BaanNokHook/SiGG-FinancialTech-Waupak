using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GM.Application.Web.Controllers;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.Report;
using GM.Data.Result.ExternalInterface;
using GM.Filters;

namespace GM.Application.Web.Areas.Report.Controllers
{
   
    public class ReportEodBoReconcileController : Controller 
    {
        private readonly Utility utility = new Utility();   

        [HttpGet]  
        public ActionResult Index(string asofDate, string access_token)   
        {
            if (string.IsNullOrEmpty(access_token) || !access_token.Equals("repo2022"))  
            {
                  return RedirectToAction("NoAccess");   
            }

            try 
            {
                  DateTime asOfDateTime = utility.ConvertStringToDatetimeFormatDDMMYYYY(asofDate);

                  ResultWithModel<RPEodReconcileResult> result = new ResultWithModel<RPEodReconcileResult>();  
                  ExternalInterfaceEntities api_ext = new ExternalInterfaceEntities();   
                  api_ext.InterfaceEodReconcile.GetEODReconcile(asOfDateTime.ToString("yyyyMMdd"), p =>  
                  {
                        result = p;  
                  });  

                  if (!result.Success)   
                  {
                        return Content(result.Message, "text/html");  
                  }  

                  DataTable dt = result.Data.RPEodReconcileResultModel.ToDataTable();   

                  var rd = new ReportDocument();  
                  rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),   
                      "EodBoReconcile.rpt"));   
                  rd.SetDataSource(dt);   
                  rd.SetParameterValue("as_of_date", asOfDateTime);   
                  Response.Buffer = false;
                  Response.ClearContent();
                  Response.ClearHeaders();
                  var stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                  stream.Seek(0, SeekOrigin.Begin);
                  return new FileStreamResult(stream, "application/pdf");

            }
            catch (Exception ex)
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