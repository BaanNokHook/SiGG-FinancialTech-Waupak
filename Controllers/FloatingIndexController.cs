using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.MarketRate;
using GM.Data.Result.MarketRate;
using GM.Data.View.MarketRate;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
     [Authorize]  
     [Audit]
     public class FloatingIndexController : Controller  
     {
         // GET: FloatingIndex  
         MarketRateEntities api_MarketRate = new MarketRateEntities();   
         StaticEntities api_static = new StaticEntities();  
         Utility utility = new Utility();   

         // Action : Index
         [RoleScreen(RoleScreen.VIEW)]  
         public ActionResult Index()  
         {
            var FloatingIndexView = new FloatingIndexViewModel();  
            FloatingIndexModel FloatingIndex = new FloatingIndexModel();   

            FloatingIndexView.FormSearch = FloatingIndex;  
            return View(FloatingIndexView);  
         }   

         [HttpPost]
         public ActionResult SearchFloatingIndex(DataTableAjaxPostModel model)   
         {
            ResultingWithModel<FloatingIndexResult> result = new ResultingWithModel<FloatingIndexResult>();   
            FloatingIndexModel FloatingIndexModel = new FloatingIndexModel();   
            try   
            {
                  //Add Paging   
                  PagingModel paging = new PagingModel();   
                  paging.PageNumber = model.pageno;   
                  paging.RecordPerPage = model.length;   
                  FloatingIndexModel.paging = paging;   

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

                  FloatingIndexModel.ordersby = orders;   

                  var columns = model.columns.Where(o => o.search.value != null).ToList();   
                  columns.ForEach(column =>  
                  {
                        switch (column.data)  
                        {
                              case "floating_index_date":  
                                    FloatingIndexModel.floating_index_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);   
                                    break;   
                              case "floating_index_code":  
                                    FloatingIndexModel.floating_index_code = column.search.value;  
                                    break;  
                              case "cur":
                                    FloatingIndexModel.cur = column.search.value;  
                                    break;    

                        }           
                  });   

                  api_MarketRate.FloatingIndex.GetFloatingIndexList(FloatingIndexModel, p =>  
                  {
                        result = p;  
                  });  
            }    
            catch (Exception ex)
            {
                result.Message = ex.Message;  
            }  
            return Json(new  
            {
                draw = model.draw,  
                recordsTotal = result.HowManyRecord,  
                recordsFiltered = result.HowManyRecord,  
                Message = result.Message,  
                data = result.Data != null ? result.Data.FloatingIndexResultModel : new List<FloatingIndexModel>()  
            }); 
        }

        //Create, Update Floating Index History 
        public ActionResult Add(string floating_index_date, string floating_index_code, string cur)  
        {
            if (!string.IsNullOrEmpty(floating_index_code))  
            {
                ResultWithModel<FloatingIndexResult> result = new ResultingWithModel<FloatingIndexResult>();   
                FloatingIndexModel FloatingIndex = new FloatingIndexModel();  

                try
                {
                    //Add Paging
                    PagingModel paging = new PagingModel();  
                    FloatingIndex.paging = paging;  
                    //Add Orderby  
                    var orders = new List<OrderByModel>();  
                    FloatingIndex.ordersby = orders;  
                    //Add id  
                    FloatingIndex.floating_index_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(floating_index_date);    
                    FloatingIndex.floating_index_code = floating_index_code;   
                    FloatingIndex.cur = cur;  
                    FloatingIndex.page_name = "editpage";   

                    api_MarketRate.FloatingIndex.GetFloatingIndexList(FloatingIndex, p =>  
                    {
                        result = p;  
                    });  

                    FloatingIndex = result.Data.FloatingIndexResultModel[0];  
                
                }
                catch (Exception)
                {
                    throw;   
                }
                ViewBag.page_name = "editpage"

                return View(FloatingIndex);   
            }
            else
            {
                FloatingIndexModel FloatingIndex = new FloatingIndexModel();  
                FloatingIndex.page_name = "addpage";   

                return View(FloatingIndex); 
            }
        }

        [HttpPost]  
        public ActionResult Add(FloatingIndexModel FloatingIndex)    
        {
            if (FloatingIndex.page_name == "addpage")  
            {
                new RoleScreenAttribute(RoleScreen.CREATE);   
                ResultWithModel<FloatingIndexResult> rwm = new ResultingWithModel<FloatingIndexResult>();     

                FloatingIndex.create_by = HttpContext.User.Identity.Name;   

                if (ModelState.IsValid)  
                {
                    api_MarketRate.FloatingIndex.CreateFloatingIndexHistory(FloatingIndex , p =>  
                    {
                        rwm = p;   
                    });

                    if (rwm.Success)
                    {
                        return RedirectToAction("Index");   
                    }
                    else  
                    {
                        ModelState.AddModelError("Exception", rwm.Message);  
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
            else 
            {
                new RoleScreenAttribute(RoleScreen.EDIT);  
                ResultWithModel<FloatingIndexResult> res = new ResultWithModel<FloatingIndexResult>();   
                FloatingIndex.create_by = HttpContext.User.Identity.Name;  

                if (ModelState.IsValid)  
                {
                    api_MarketRate.FloatingIndex.UpdateFloatingIndexHistory(FloatingIndex, p =>  
                    {
                        res = p;  
                    });

                    if (res.Success)  
                    {
                        return RedirectToAction("Index");   
                    }
                    else
                    {
                        ModelState.AddModelError("Exception", res.Message);  
                    }
                }
                else
                {
                    var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();   
                    Models.ForEach(field =>  
                    {
                        fiels.Errors.ToList().ForEach(error => 
                        {
                            res.Message += error.ErrorMessage; 
                        });  
                    }); 
                }
            }
            ModelState.Clear();  
            return View(FloatingIndex);  
        }  

        [RoleScreen(RoleScreen.VIEW)]  
        public ActionResult Edit(string floating_index_date, string floating_index_code, string cur)   
        {
            return RedirectToAction("Add", new { floating_index_date = floating_index_date, floating_index_code = floating_index_code, cur = cur });  
        }

        public class Data
        {
            public string floating_index_date { get; set; }
            public string floating_index_code { get; set; }
            public string cur { get; set; }
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult Deletes(Data data)
        {
            var rwm = new ResultWithModel<FloatingIndexResult>();
            FloatingIndexModel view = new FloatingIndexModel();
            view.create_by = HttpContext.User.Identity.Name;

            view.floating_index_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(data.floating_index_date);
            view.floating_index_code = data.floating_index_code;
            view.cur = data.cur;
            try
            {
                api_MarketRate.FloatingIndex.DeleteFloatingIndexHistory(view, p =>
                {
                    rwm = p;
                });

                if (rwm.Success)
                {
                    return Json(new { success = true, responseText = "Your message successfuly sent!" }, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    ModelState.AddModelError("", rwm.Message);
                    return Json(new { success = false, responseText = rwm.Message }, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FillCurrency(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.Currency.GetDDLCurrency(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillFloatingIndex(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_MarketRate.FloatingIndex.GetDDLFloatingIndex(datastr, p =>
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