using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.Static;
using GM.Data.Result.Static;
using GM.Data.View.Static;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
   [Authorize]  
   [Audit]  
   public class CurrencyController : BaseController  
   {

      StaticEntities api_static = new StaticEntities();   

      // GET: Role
      [RoleScreen(RoleScreen.VIEW)]   
      public ActionResult Index()  
      {
            return View();  
      }

      [HttpPost]  
      public ActionResult Search(DataTableAjaxPostModel model)  
      {
            string searchvalue = Request["search[value]"];  
            ResultWithModel<CurrencyResult> result = new ResultWithModel<CurrencyResult>();   
            CurrencyModel CurrencyModel = new CurrencyModel();   
            //Add Paging  
            PagingModel paging = new PagingModel();
            paging.PageNumber = model.pageno;   
            paging.RecordPerPage = model.length;  
            CurrencyModel.paging = paging;  

            //Add Orderby
            var orders = new List<OrderByModel>();


            if (model.order != null)
            {
                  model.order.ForEach(o =>  
                  {
                        var col = model.columns[o.column];   
                        orders.Add(new OrderByModel { Name = col.data, SortDirection = (o.dir.Equals("desc") ? SortDirection.Descending : SortDirection.Ascending) });      
                  });    
            }       

            CurrencyModel.ordersby = orders;  

            var columns = model.columns.Where(o => o.search.value != null).ToList();   
            columns.ForEach(column => 
            {
                  switch (column.data)
                  {
                        case "cur":
                              CurrencyModel.cur = column.search.value;  
                              break;  
                        case "cur_code": 
                              CurrencyModel.cur_code = column.search.value;  
                              break;  
                        case "cur_desc":  
                              CurrencyModel.cur_desc = column.search.value;   
                              break;  
                        case "active_flag":  
                              CurrencyModel.active_flag = Boolean.Parse(column.search.value == "true" ? "true" : "false");   
                              break;  
                  }
            });  

            api_static.Currency.GetCurrencyList(CurrencyModel, p => {
                  result = p;  
            });  

            return Json(new   
            {
                  draw = model.draw,
                  recordsTotal = result.HowManyRecord,  
                  recordsFiltered = result.HowManyRecord,  
                  data = result.Data != null ? result.Data.CurrencyResultModel:new List<CurrencyModel>()
            });   

            // return Json(new { data = result.Data.UserModels }, JsonRequestBehavior.AllowGet);   
      }

      [HttpPost]
      [RoleScreen(RoleScreen.CREATE)]  
      public ActionResult Create(CurrencyViewModel view)   
      {
            var rwm = new ResultWithModel<CurrencyResult>();    
            view.FormAction.create_by = HttpContext.User.Identity.Name; 
            try
            {
                  if (ModelState.IsValid)   
                  {
                        api_static.Currency.CreateCurrency(view.FormAction, p => {
                              rwm = p;   
                        });   

                        if (!rwm.Success)  
                        {
                              ModelState.AddModelError("", rwm.Message);  
                        }  
                  }
                  else 
                  {
                        var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();   
                        Models.ForEach(field => 
                        {
                              field.Errors.ToList().ForEach(error =>  
                              {
                                    rwm.Message += error.ErrorMessage;  
                              });  
                        });  
                  }
            }
            catch (Exception ex)    
            {
                  rwm.Message = ex.Message; 
            }

            return Json(rwm. JsonRequestBehavior.AllowGet);    
      }

      //Action Update   
      [RoleScreen(RoleScreen.VIEW)]  
      public ActionResult Edit(string id)  
      {
            var result = new ResultWithModel<CurrencyResult>();   

            CurrencyModel model = new CurrencyModel();  
            PagingModel paging = new PagingModel();  
            paging.PageNumber = 0;   
            paging.RecordPerPage = 0;   
            model.paging = paging;   
            model.cur = id;   
            model.create_by = HttpContext.User.Identity.Name;  

            api_static.Currency.GetCurrencyEdit(model, p => 
            {
                  result = p;   
            });  
            return Json((result.Data.CurrencyResultModel.Count > 0 ? result.Data.CurrencyResultModel[0] : new CurrencyModel()), JsonRequestBehavior.AllowGet); 
      }  

      [HttpPost]
      [RoleScreen(RoleScreen.EDIT)] 
      public ActionResult Edit(CurrencyViewModel view)   
      {
            var rwm = new ResultWithModel<CurrencyResult>();   

            try  
            {
                  if (ModelState.IsValid)  
                  {
                        api_static.Currency.UpdateCurrency(view.FormAction, p => {
                              rwm = p;  
                        });    
                        
                        if (!rwm.Success)  
                        {
                              ModelState.AddModelError("", rwm.Message);   
                        }  
                  }
                  else
                  {
                        var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();   

                        Models.ForEach(field =>  
                        {
                              field.Errors.ToList().ForEach(error => 
                              {
                                    rwm.Message += error.ErrorMessage;  
                              });  
                        });
                  }
            }
            catch (Exception ex)   
            {
                  rwm.Message = ex.Message;  
            }  

            return Json(rwm, JsonRequestBehavior.AllowGet);
      }

      // Action : Delete  
      public class Data  
      {
            public string cur { get; set; }  
      }  

      [HttpPost]
      [RoleScreen(RoleScreen.DELETE)]   
      public JsonResult Deletes(Data data)  
      {
            var rwm = new ResultWithModel<CurrencyResult>();  
            CurrencyModel view = new CurrencyModel();  
            view.create_by = HttpContext.User.Identity.Name;   
            view.cur = data.cur;   
            try
            {
                  api_static.Current.DeleteCurrency(view, p => 
                  {
                        rwm = p;  
                  });   

                  if (rwm.Success)   
                  {
                        return Json(new { success = true, responseText = "Your message successfully sent!" }, JsonRequestBehavior.AllowGet); 
                  } 

                  else
                  {
                        ModelState.AddModelError("", rwm.Message);
                        return Json(new { success = false, responseText = rwm.Message }, JsonRequestBehavior. AllowGet);      
                  } 
            }

            catch (Exception ex)  
            {
                  return Json(new { success = false, responseText = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
      }
   }
}


