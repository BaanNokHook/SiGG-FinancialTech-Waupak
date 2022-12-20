using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.ExternalInterface;
using GM.Data.Result.ExternalInterface;
using GM.Filters;
using System.Web.Mvc;
using System.Web.Routing;
using GM.CommonLibs.ClassLibs;
using GM.Data.Model.Static;
using GM.Data.Result.Static;

namespace GM.Application.Web.Controllers
{
   [Authorize]
   [Audit]  
   public class EODReconcileController : BaseController
   {
      // GET: EODReconcile
      [RoleScreen(RoleScreen.VIEW)]   
      public ActionResult Index()  
      {
            return View();  
      }  

      [RoleScreen(RoleScreen.VIEW)]  
      public ActionResult Get(string AsofDate)   
      {
            ResultWithModel<RPEodReconcileResult> result = new ResultWithModel<RPEodReconcileResult>();   
            ExternalInterfaceEntities api_ext = new ExternalInterfaceEntities();   
            api_ext.InterfaceEodReconcile.GetEODReconcile(AsofDate, p => 
            {
                  result = p;   
            });  

            return Json(result,JsonRequestBehavior.AllowGet);   
      }

      [RoleScreen(RoleScreen.EDIT)]   
      [HttpPost]  
      public ActionResult Save(RPEodReconcileModel model)  
      {
            ResultWithModel<RPEodReconcileResult> result = new ResultWithModel<RPEodReconcileResult>();   
            try
            {
                  model.CREATE_BY = HttpContext.User.Identity.Name;  

                  ExternalInterfaceEntities api_ext = new ExternalInterfaceEntities();   
                  api_ext.InterfaceEodReconcile.SaveEODReconcile(model, p =>   
                  {
                        result = p;  

                  });

                  if (!result.Success)  
                  {
                        throw new Exception("SaveEOReconcile() => [" + result.RefCode + "]" + result.Message);     
                  }    

                  #region Send Email
                  ResultWithModel<RpConfigResult> resultRpconfig = new ResultWithModel<RpConfigResult>();   
                  StaticEntities staticEnt = new StaticEntities();    
                  List<RpConfigModel> rpConfigModelList = new List<RpConfigModel>();   

                  staticEnt.RpConfig.GetRpConfig("RP_EOD_BO_RECONCILE_MAIL", string.Empty, p => 
                  {
                        resultRpconfig = p;  
                  });  

                  if (resultRpconfig.RefCode != 0)  
                  {
                        throw new Exception("GetRpCOnfig() => [" + resultRpconfig.RefCode + "]" + resultRpconfig.Message);   
                  }  

                  rpConfigModelList = resultRpconfig.Data.RpConfigResultModel;   
                  var rpConfig = rpConfigModelList.FirstOrDefault(a => a.item_code == "ENABLE_MAIL");   
                  if (rpConfig != null && rpConfig.item_value == "Y")   
                  {
                        model.RpConfigModel = rpConfigModelList;   
                        api_ext.InterfaceEodReconcile.SendEmailEODReconcile(model, p =>  
                        {
                              result = p;  
                        });  
                  }  

                  #endregion  
            }
            catch (Exception Ex)   
            {
                  result.Message = Ex.Message;  
                  result.Success = false;   
            }

            return Json(result, JsonRequestBehavior.AllowGet);   
      }

      [HttpGet]  
      [RoleScreen(RoleScreen.VIEW)]
      public ActionResult DownloadEodBoReconcile(string asofDate)   
      {
            try 
            {
                  if (string.IsNullOrEmpty(asofDate))   
                  {
                        return Content("Not AsofDate!!!");   
                  }

                  using (WebClient client = new WebClient())   
                  {
                        
                    //string url = this.Url.Action("Index", "ReportEodBoReconcile",
                    //    new
                    //    {
                    //        asofDate = asofDate,
                    //        access_token = "repo2022"
                    //    }, this.Request.Url.Scheme);
                  }  

                        string urlReport = string.Format(PublicConfig.GetValue("Reportpath") + "/ReportEodBoReconcile/index?asofDate={0}&access_token=repo2022",   
                              asofDate);   
                        
                        if (urlReport.StartsWith("https://"))     
                              ServicePointManager.ServiceCertificatwValidationCallback += (sender, cert, chain, errors) => true;   

                        byte[] byteFile = client.DownloadData(urlReport);     
                        return new FileContentResult(byteFile, "application/pdf");   
                  }
            }
            catch (Exception ex)   
            {
                  return Content(ex.Message, "text/html");    
            } 
      }

      [HttpGet]
      public ActionResult DownloadEodBoReconcile(string asofDate, string type)   
      {
            try
            {
                  if (string.IsNullOrEmpty(asofDate))   
                  {
                        return Content("Not AsofDate!!!");  
                  }  
                  using (WebClient client = new WebClient())
                  {
                    //string url = this.Url.Action("Index", "ReportEodReconcile",
                    //    new
                    //    {
                    //        asofDate = asofDate,
                    //        type = type,
                    //        access_token = "repo2022"
                    //    }, this.Request.Url.Scheme);

                    string urlReport = string.Format(PublicConfig.GetValue("Reportpath") + "/ReportEodBoReconcile/Index?asofDate={0}&type={1}&access_token=repo2022",   
                        asofDate, type);   

                    if (urlReport.StartsWith("https://"))   
                        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, errors) => true;   

                    byte[] byteFile = client.DownloadData(urlReport);   
                    return new FileContentResult(byteFile, "application/pdf");      
                  }  
            }     
            catch (Exception ex)   
            {
                  return Content(ex.Message, "text/html");  
            }
      }

      [HttpGet]  
      [RoleScreen(RoleScreen.VIEW)]   
      public ActionResult DownloadEodBoReconcileCallMargin(string asofDate, string type)   
      {
            try
            {
                  if (string.IsNullOrEmpty(asofDate))   
                  {
                        return Content("Not AsoffDate!!!");     
                  }   

                  using (WebClient client = new WebClient())   
                  {
                    //string url = this.Url.Action("Index", "ReportEodReconcileCallMargin",
                    //    new
                    //    {
                    //        asofDate = asofDate,
                    //        type = type,
                    //        access_token = "repo2022"
                    //    }, this.Request.Url.Scheme);

                    string urlReport = string.Format(PublicConfig.GetValue("Reportpath") + "/ReportEodReconcileCallMargin/Index?asofDate={0}&type={1}&access_token=repo2022",   
                        asofDate, type);    

                        if (urlReport.StartsWith("https://"))   
                            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, erros) => true;  

                        byte[] byteFile = client.DownloadData(urlReport);   
                        return new FileContentResult(byteFile, "application/pdf");            
                  }
            }
            catch (Exception ex)     
            {
                  return Content(ex.Message, "text/html");   
            }
      }
}