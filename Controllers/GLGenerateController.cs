using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.GLProcess;
using GM.Data.Result.GLProcess;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
   [Authorize]  
   [Audit]
   public class GLGenerateController : BaseController
   {
      GLProcessEntities api = new GLProcessEntities();
      // GET: GLGenerate  
      [RoleScreen(RoleScreen.VIEW)]  
      public ActionResult Index()
      {
            return View();   
      }  

      // POST: GLGenerate/Generate  
      [HttpPost]  
      [RoleScreen(RoleScreen.CREATE)]    
      public ActionResult Generate(GLGenerateModel model)
      {
            bool isSuccess = true;  
            string message = string.Empty;
            ResultWithModel<GLGenerateResult> result = new ResultWithModel<GLGenerateResult>();  
            try
            {
                  // TODO: Generate insert logic here  
                  api.GLGenerate.GLGenerateRunBatch(model, p =>  
                  {
                        result = p;  
                  });  

                  if (result.RefCode != 0)
                  {
                        isSuccess = false; 
                        message = result.Message; 
                  }  
            }
            catch(Exception ex)  
            {
                  message = ex.Message;  
                  isSuccess = false;  
            }  

            return Json(new 
            {
                  Success = isSuccess,  
                  Message = message
            });  
      } 

      public ActionResult FillCur(string datastr)   
      {
            List<DDLItemModel> res = new List<DDLItemModel>();  
            api.GLGenerate.DDLCurrency(datastr, p =>  
            {
                  if (p.Success)  
                  {
                        res = p.Data.DDLItems;
                  }
            });
            return Json(res, JsonRequestBehavior.AllowGet);  
      }  

      public ActionResult FillEvent(string datastr)   
      {
            List<DDLItemModel> res = new List<DDLItemModel>();   
            api.GLGenerate.DDLEvent(datastr, p => 
            {
                  if (p.Success)  
                  {
                        res = p.Data.DDLItems;  
                  }
            });  
            return Json(res, JsonRequestBehavior.AllowGet);    
      }
   }
}