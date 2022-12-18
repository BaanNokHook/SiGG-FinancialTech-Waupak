using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.ExchangeRate;
using GM.Data.Result.ExchangeRate;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers  
{
    [Authorize]  
    [Audit]  
    public class ExchangeRateController : BaseController  
    {
        ExchangeRateEntities api_exchangeRate = new ExchangeRateEntities();   
        Utitlity utility = new Utitlity();   

        [RoleScreen(RoleScreen.VIEW)]   
        public ActionResult Index()   
        {
                return View();  
        }  

        [RoleScreen(RoleScreen.CREATE)]   
        public ActionResult Index()   
        {
                return View();   
        }  

        [HttpPost]    
        [RoleScreen(RoleScreen.Create)]  
        public JsonResult Add(ExchangeRateModel model)   
        {
                ResultWithModel<ExchangeRateResult> result = new ResultWithModel<ExchangeRateResult>();   
                if (ModelState.IsValid)   
                {
                    model.create_by = User.UserId;  
                    api_exchangeRate,ExchangeRate.Create(model, p =>  
                    {
                            result = p;   
                    });  

                    if (result.Success)   
                    {
                            return Json(result, JsonRequestBehavior.AllowGet);  
                    }  
                    ModelState.AddModelError("Exception", result.Message);      
                }  
                return Json(result, JsonRequestBehavior.AllowGet);  
        }  

        [RoleScreen(RoleScreen.VIEW)]   
        public ActionResult Edit(string exchange_type, string source_type, string asof_date, string cur1, string cur2)   
        {
                ResultWithModel<ExchangeRateResult> result = new ResultWithModel<ExchangeRateResult>();    
                PagingModel pagingModel = new PagingModel();   
                ExchangeRateModel exchangeModel = new ExchangeRateModel();   
                List<OrderByModel> orderByModels = new List<OrderByModel>();  
                exchangeModel.paging = pagingModel;    
                exchangeModel.ordersby = orderByModels;  
                exchangeModel.exchange_type = exchange_type;   
                exchangeModel.source_type = source_type;   
                exchangeModel.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(asof_date);  
                exchangeModel.cur1 = cur1;  
                exchangeModel.cur2 = cur2;  

                api_exchangeRate.ExchangeRate.GetEdit(exchangeModel, p =>  
                {
                    result = p;   
                });   

                exchangeModel = result.Data.ExchangeRateResultModel[0];   

                return View(exchangeModel);   
        }  

        [HttpPost]   
        [RoleScreen(RoleScreen.EDIT)]  
        public JsonResult Edit(ExchangeRateModel model)   
        {
                ResultWithModel<ExchangeRateResult> result = new ResultWithModel<ExchangeRateResult>();   
                if (ModelState.IsValid)   
                {
                    model.create_by = User.UserId;  

                    api_exchangeRate.ExchangeRate.Update(model, p =>  
                    {
                            result = p;  

                    });  

                    if (result.Success)  
                    {
                            return Json(result, JsonRequestBehavior.AllowGet);   
                    }
                    ModelState.AddModelError("Exception", result.Message);  
                }  
                return Json(result, JsonRequestBehavior.AllowGet);  
        }  

        [HttpPost]  
        [RoleScreen(RoleScreen.DELETE)]   
        public JsonResult Delete(string exchange_type, string source_type, string asof_date, string cur1, string cur2)    
        {
                ResultWithModel<ExchangeRateResult> result - new ResultWithModel<ExchangeRateResult>();  
                if (ModelState.IsValid)  
                {
                    model.create_by = User.UserId;  
                    
                    api_exchangeRate.ExchangeRate.Update(model, p => 
                    {
                            result = p;   
                    });  

                    if (result.Success)   
                    {
                            return Json(result, JsonRequestBehavior.AllowGet);   
                    }  
                    ModelState.AddModelError("Exception", result.Message);  
                }
                return Json(result, JsonRequestBehavior.AllowGet);   
        }   

        [HttpPost]   
        [RoleScreen(RoleScreen.DELETE)]   
        public JsonResult Delete(string exchange_type, string source_type, string asof_date, string cur1, string cur2)
        {
                ResultWithModel<ExchangeRateResult> result = new ResultWithModel<ExchangeRateModel>();   
                ExchangeRateModel exchangeModel = new ExchangeRateModel();   
                exchangeModel.create_by = HttpContext.User.Identity.Name;   
                exchangeModel.exchange_type = exchange_type;   
                exchangeModel.source_type = source_type;  
                exchangeModel.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(asof_date);  
                exchangeModel.cur1 = cur1;  
                exchangeModel.cur2 = cur2;  

                try
                {
                    api_exchangeRate.ExchangeRate.Delete(exchangeModel, p => 
                    {
                        result = p;  
                    });  

                    if (result.Success)  
                    {
                        return Json(new { success = true, responseText = "Your message successfully sent!" }, JsonRequestBehavior.AllowGet);  
                    }  

                    else 
                    {
                        ModelState.AddModelError("", result.Message);   
                        return Json(new { success = false, responseText = result.Message }, JsonRequestBehavior.AllowGet);   
                    }
                } 

                catch (Exception ex)   
                {
                    return Json(new { success = false, responseText = ex.ToString() }, JsonRequestBehavior.AllowGet);   
                    // return View();  
                }
                // return Json(rwm, JsonRequestBehavior.AllowGet);  
        }  

        [HttpPost]  
        public ActionResult Search(DataTableAjaxPostModel model)    
        {
                ResultWithModel<ExchangeRateResult> result = new ResultWithModel<ExchangeRateResult>();  
                ExchangeRateModel exchangeRateModel = new ExchangeRateModel();  
                try  
                {
                    //Add Paging  
                    PagingModel paging = new PagingModel();  
                    paging.PageNumber = model.pageno;    
                    paging.RecordPerPage = model.length;  
                    exchangeRateModel.paging = paging; 

                    //Add Orderby  
                    var orders = new List<OrderByModel>();   

                    if (model.order != null)  
                    {
                        model.order.ForEach(o => 
                        {
                            var col = model.columns[o.column];   
                            orders.Add(new OrderByModel { Name = col.data, SortDirection = (o.dir.Equals("desc" ? SortDirection.Descending : SortDirection.Ascending) });   
                        });   
                    }

                    exchangeRateModel.ordersby = orders;  

                    var columns = model.columns.Where(o => o.search.value != null).ToList();   
                    columns.ForEach(column =>  
                    {
                        switch (column.data)  
                        {
                            case "source_type":  
                                exchangeRateModel.source_type = column.search.value;  
                                break;  
                            case "exchange_type":
                                exchangeRateModel.exchange_type = column.search.value;   
                                break;  
                            case "asof_date":   
                                exchangeRateModel.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);  
                                break;   
                        } 
                    });  

                    exchangeRateModel.create_by = User.UserId

                    api_exchangeRate.ExchangeRate.GetExchangeRateList(exchangeRateModel, p =>
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
                data = result.Data != null ? result.Data.ExchangeRateResultModel : new List<ExchangeRateModel>()   
            });  
        }

        public ActionResult FillExchangeRateSource(string datastr)   
        {
                List<DDLItemModel> res = new List<DDLItemModel>();   
                api_exchangeRate.ExchangeRate.GetDDLExchangeRateSource(datastr, p =>  
                {
                    if (p.Success)   
                    {
                        res = p.Data.DDLItems;  
                    }  
                });  
                return Json(res, JsonRequestBehavior.AllowGet);  
        }  

        public ActionResult FillExchangeRateType(string datastr)
        {
                List<DDLItemModel> res = new List<DDLItemModel>(); 
                api_exchangeRate.ExchangeRate.GetDDLExchangeRateType(datastr, p =>
                    {
                        if (p.Success)
                        {
                            res = p.Data.DDLItems;
                        }
                    });
                return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCurrency(string datastr)
        {
                List<DDLItemModel> res = new List<DDLItemModel>();  
                StaticEntities api_static = new StaticEntities();  
                api_static.Currency.GetDDLCurrency(datastr, p =>  
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

