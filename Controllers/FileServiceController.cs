using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using GM.CommonLibs.ClassLibs;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Result.Common;
using GM.Filters;

namespace GM.Application.Web.Controllers
{
   [Authorize]   
   [Audit]  
   public class FileServiceController : BaseController
   {
      // GET: FileService
      public ActionResult Index()   
      {
            if (IsCheckPermission() && User.WizardPage == "Y")   
            {
                  return View();  
            }  

            return RedirectToAction("Index", "Home");   
      }  

      [HttpGet]   
      public JsonResult GetHeadList(string service)    
      {
            List<FileServiceModel> fileModel = new List<FileServiceModel>();  
            ResultWithModel<FileServiceResult> result = new ResultWithModel<FileServiceResult>();   

            switch (service)  
            {
                  case "CounterParty": 
                        new CounterPartyEntities().FileService.GetHeadList(p => { result = p; });   
                        break;   
                  case "ExternalInterface":  
                        new ExternalInterfaceEntities().FileService.GetHeadList(p => { result = p; }); 
                        break;  
                  case "GLProcess":  
                        new GLProcessEntities().FileService.GetHeadList(p => { result = p; });   
                        break;  
                  case "MarketRate":  
                        new MarketRateEntities().FileService.GetHeadList(p => { result = p; });  
                        break;  
                  case "PaymentProcess":  
                        new PaymentProcessEntities().FileService.GetHeadList(p => { result = p; });   
                        break;   
                  case "Report":  
                        new ReportEntities().FileService.GetHeadList(p => { result = p: });   
                        break;    
                  case "Static":
                        new StaticEntities().FileService.GetHeadList(p => { result = p; });   
                        break;   
                  case "RPTrans":   
                        new RPTransEntity().FileService.GetHeadList(p => { result = p; });   
                        break;   
                  case "UserAndScreen":   
                        new UserAndScreenEntities().FileService.GetHeadList(p => {});       
                        break;  
                  case "Gateway":  
                        new GatewayEntity().FileService.GetHeadList(p => { result = p; });   
                        break;   
                  case "Provider":  
                        new ProviderEntity().FileService.GetHeadList(p => { result = p; });   
                        break;  
                  default:  
                        fileModel.Add(new FileServiceModel() { key = "", title = "Service not found" });  
                        return Json(fileModel, JsonRequestBehavior.AllowGet);  
            } 

            if (result.Success)  
            {
                  if (!(result.Data is null))  
                  {
                        fileModel = result.Data.FileServiceResultModel.OrderBy(x=>x.title).ToList();  
                  }
                  else 
                  {
                        fileModel.Add(new FileServiceModel{ key = "", title = "Folder not found"});  
                  }  
            }
            else
            {
                  fileModel.Add(new FileServiceModel { key = "", title = result.Message });   
            }  

            return Json(fileModel, JsonRequestBehavior.AllowGet);  
      }

      [HttpGet]  
      public JsonResult GetNodeList(string service, string path)   
      {
            List<FileServiceModel> fileModel = new List<FileServiceModel>();   
            ResultWithModel<FileServiceResult> result = new ResultWithModel<FileServiceResult>();   

            switch (service)  
            {
                  case "CounterParty":
                        new CounterPartyEntities().FileService.GetNodeList(path, p => { result = p; });  
                        break; 
                  case "ExternalInterface":  
                        new ExternalInterfaceEntities().FileService.GetNodeList(path, p => { result = p; });   
                        break;   
                  case "GLProcess":  
                        new GLProcessEntities().FileService.GetNodeList(path, p => ( result = p; ));   
                        break;   
                  case "MarketRate":  
                        new MarketRateEntities().FileService.GetNodeList(path, p => { result = p; });    
                        break;   
                  case "PaymentProcess":
                        new PaymentProcessEntities().FileService.GetNodeList(path, p => { result = p; });
                        break;
                  case "Report":
                        new ReportEntities().FileService.GetNodeList(path, p => { result = p; });
                        break;
                  case "Static":
                        new StaticEntities().FileService.GetNodeList(path, p => { result = p; });
                        break;
                  case "RPTrans":
                        new RPTransEntity().FileService.GetNodeList(path, p => { result = p; });
                        break;
                  case "UserAndScreen":
                        new UserAndScreenEntities().FileService.GetNodeList(path, p => { result = p; });
                        break;
                  case "Gateway":
                        new GatewayEntity().FileService.GetNodeList(path, p => { result = p; });
                        break;
                  case "Provider":
                        new ProviderEntity().FileService.GetNodeList(path, p => { result = p; });
                        break;
                  default:
                        fileModel.Add(new FileServiceModel{ key = "", title = "Service not found" });
                        return Json(fileModel, JsonRequestBehavior.AllowGet);
            }

            if (result.Success)
            {
                if (!(result.Data is null))
                {
                    fileModel = result.Data.FileServiceResultModel.OrderByDescending(x => x.title).ToList(); ;
                }
                else
                {
                    fileModel.Add(new FileServiceModel() { key = "", title = "Folder/file not found" });
                }
            }
            else
            {
                fileModel.Add(new FileServiceModel { key = "", title = result.Message });
            }

            return Json(fileModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DownloadFile(string service, string path)
        {

            string url = DataMappingService().FirstOrDefault(a => a.Key == service).Value;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);

                string token = PublicConfig.GetValue("Token");

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("x-access-token", token);
                }

                HttpResponseMessage response = client.GetAsync("FileService/DownloadFile?path=" + path).Result;

                if (response.IsSuccessStatusCode)
                {
                    var file = new FileStreamResult(response.Content.ReadAsStreamAsync().Result,
                        "application/octet-stream") {FileDownloadName = path.Substring(path.LastIndexOf('/') + 1)};
                    return file;
                }
            }
            return Content("no data", "text/html");
        }

        [HttpGet]
        public ActionResult FillService()
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            foreach (var data in DataMappingService())
            {
                DDLItemModel ddlItem = new DDLItemModel {Value = data.Key, Text = data.Key};
                res.Add(ddlItem);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        private Dictionary<string, string> DataMappingService()
        {
            return new Dictionary<string, string>()
            {
                { "CounterParty", PublicConfig.GetValue("CounterParty")},
                { "MarketRate", PublicConfig.GetValue("MarketRate")},
                { "PaymentProcess", PublicConfig.GetValue("PaymentProcess")},
                { "RPTrans", PublicConfig.GetValue("RPTrans")},
                { "Static", PublicConfig.GetValue("Static")},
                { "UserAndScreen", PublicConfig.GetValue("UserAndScreen")},
                { "ExternalInterface", PublicConfig.GetValue("ExternalInterface")},
                { "GLProcess", PublicConfig.GetValue("GLProcess")},
                { "Report", PublicConfig.GetValue("Report")},
                { "Gateway", PublicConfig.GetValue("Gateway")},
                { "Provider", PublicConfig.GetValue("Provider")}
            };
        }

        private bool IsCheckPermission()
        {
            if (User.RoleName != "Administrator")
            {
                return false;
            }

            return true;
        }
    }
}