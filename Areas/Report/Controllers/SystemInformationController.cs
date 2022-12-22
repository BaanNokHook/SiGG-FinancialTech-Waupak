using System;
using System.Web.Mvc;
using GM.Application.Web.Areas.Report.Models;

namespace GM.Application.Web.Areas.Report.Controllers
{

    public class SystemInformationController : Controller  
    {
        private readonly ReportEntitiesModel reportentity = new ReportEntitiesModel();   

        [HttpGet]  
        public JsonResult GetBusinessDate(string datastr)    
        {

            var res = new ResultJsonModel();   
            try
            {
                  DateTime? Date = new DateTime();  
                  Date = reportentity.Getbusinessdate();
                  if (Date != null) res.Data = Date.Value.ToString("MM/dd/yyyy");    
                  return Json(res, JsonRequestBehavior.AllowGet);   
            } 
            catch (Exception ex)  
            {
                  res.Error = true;  
                  res.Error_detail = ex.ToString();  
                  return Json(res, JsonRequestBehavior.AllowGet);   
            }  
        }
    }
}