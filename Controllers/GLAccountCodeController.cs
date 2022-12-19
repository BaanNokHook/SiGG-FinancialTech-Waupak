using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.GLProcess;
using GM.Data.Result.GLProcess;
using GM.Data.View.GLProcess;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
   [Authorize]  
   [Audit] 
   public class GLAccountCodeController : BaseController
   {
      GLProcessEntities api_glprocess =new GLProcessEntities();  
      GM.Data.Helper.Utility utility = new GM.Data.Helper.Utility();   
      // GET: GLAccountCode  
      [RoleScreen(RoleScreen.VIEW)]  
      public ActionResult Index()  
      {
            return View();   
      }

      [HttpPost]
      public ActionResult Search(DataTableAjaxPostModel model)
      {
            ResultWithModel<GLAccountCodeResult> Result = new ResultWithModel<GLAccountCodeResult>();   
            GLAccountCodeModel gLAccountModel = new GLAccountCodeModel();      

            //Add Paging    
            PagingModel paging = new PagingModel();   
            paging.PageNumber = model.pageno;   
            paging.RecordPerPage = model.length;   
            gLAccountModel.paging = paging;  

            //Add Ordersby  
            var orders = new List<OrderByModel>();  

            if (model.order != null)   
            {
                  model.order.ForEach(o =>
                  {
                        var col = model.columns[o.column];  
                        orders.Add(new OrderByModel { Name = col.data, SortDirection = (o.dir.Equals("desc") ? SortDirection.Descending : SortDirection.Ascending) });   
                  });  
            }

            gLAccountModel.ordersby = orders;    

            var columns = model.columns.Where(o => o.search.value != null).ToList();
            columns.ForEach(colum => 
            {
                  switch (column.data)  
                  {
                        case "account_num":   
                              gLAccountModel.account_num = column.search.value;  
                              break;  
                        case "account_name":  
                              gLAccountModel.account_name = column.search.value;   
                              break;  
                        case "acct_port":  
                              gLAccountModel.acct_port = column.search.value;      
                              break;  
                  }
            });  

            api_glprocess.GLAccountCode.GetGLAccountCodeList(gLAccountModel, p => {
                  Result = p;  
            });  

            return Json(new
            {
                  draw = model.draw,  
                  recordsTotal = Result.HowManyRecord,  
                  recordsFiltered = Result.HowManyRecord,  
                  data = Result.Data != null ? Result.Data.GLAccountCodeResultModel : new List<GLAccountCodeModel>()  
            });  
      }

      [HttpPost]
      [RoleScreen(RoleScreen.CREATE)]   
      public ActionResult Create(GLAccountCodeViewModel model)  
      {
            var Result = new ResultWithModel<GLAccountCodeResult>();  
            try  
            {
                  model.FormAction.create_by = HttpContext.User.Identity.Name;  
                  if (ModelState.IsValid)  
                  {
                        api_glprocess.GLAccountCode.CreateGLAccountCode(model.FormAction, p => {
                              Result = p;  
                        }); 

                        if (!Result.Success)  
                        {
                              ModelState.AddModelError("", Result.Message);  
                        }  
                  }
                  else
                  {
                        var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();   

                        Models.ForEach(field => 
                        {
                              field.Errors.ToList().ForEach(error => 
                              {
                                    Result.Message += error.ErrorMessage;
                              });  
                        });  

                  }
            }
            catch (Exception ex)  
            {
                  Result.Message = ex.Message; 
            }  

            return Json(Result, JsonRequestBehavior.AllowGet);  
      }

      [RoleScreen(RoleScreen.VIEW)]  
      public ActionResult Edit(string account_num)
      {
            GLAccountCodeModel Model = new GLAccountCodeModel();   
            ResultWithModel<GLAccountCodeResult> result = new ResultWithModel<GLAccountCodeResult>();   
            PagingModel paging = new PagingModel();   
            paging.PageNumber = 0;  
            paging.RecordPerPage = 0;
            Model.paging = paging;  
            Model.account_num = account_num;  
            Model.update_by = HttpContext.User.Identity.Name;      

            api_glprocess.GLAccountCode.GetGLAccountCodeList(Model, p => {  
                  result = p;  
            }); 
            // return View(model);   
            return Json((result.Data.GLAccountCodeResultModel.Count > 0 ? result.Data.GLAccountCodeResultModel[0] : new GLAccountCodeModel()), JsonRequestBehavior.AllowGet);   
      }  

      [HttpPost]  
      [RoleScreen(RoleScreen.EDIT)]  
      public ActionResult Edit(GLAccountCodeViewModel model)  
      {
            var Result = new ResultWithModel<GLAccountCodeResult>();  
            try
            {
                  if (ModelState.IsValid)
                  {
                      api_glprocess.GLAccountCode.UpdateGLAccountCode(model.FormAction, p => {
                              Result = p; 
                      });  

                      if (!Result.Success)
                      {
                         ModelState.AddModelError("", Result.Message);
                      }
                  }
                  else 
                  {
                        var Models = ModelState.Values.Where(o => o.Error.Count > 0).ToList();   
                        Models.ForEach(field => 
                        {
                              field.Errors.ToList().ForEach(error =>  
                              {
                                    Result.Message += error.ErrorMessage;  
                              });  
                        });  
                  }
            }
            catch (Exception ex)   
            {
                  Result.Message = ex.Message;  
            }

            return Json(Result, JsonRequestBehavior.AllowGet);  
      }  

      public class Data 
      {
            public string account_num { get; set; }  
      }  

      [HttpPost]
      [RoleScreen(RoleScreen.DELETE)]   
      public JsonResult Delete(Data data)
      {
            var Result = new ResultWithModel<GLAccountCodeResult>();  
            GLAccountCodeModel Model = new GLAccountCodeModel();  
            try 
            {
                  Model.account_num = data.account_num;  
                  //Model.cur = data.cur; 
                  Model.create_by = HttpContext.User.Identity.Name;  

                  api_glprocess.GLAccountCode.DeleteGLAccountCode(Model, p =>  
                  {
                        Result = p;  
                  });  

                  if (Result.Success == false)
                  {
                        return Json(new { success = false, responseText = Result.Message, refcode = Result.RefCode }, JsonRequestBehavior.AllowGet);  
                  }  

                  return Json(new { success = true, responseText = "Your message successfully sent!", refcode = Result.RefCode }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)   
            {
                  return Json(new { success = false, responseText = ex.ToString(), refcode = Result.RefCode }, JsonRequestBehavior.AllowGet);   
            }  
      }

      public ActionResult FillAccountPort(string port_gl)
      {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_glprocess.GLAccountCode.GetDDLAccountPort(port_gl, p =>  
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