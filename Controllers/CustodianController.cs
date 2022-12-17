using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.Static;
using GM.Data.Result.Static;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]  
    [Audit]  
    public class CustodianController : BaseController 
    {

      StaticEntities api_static = new StaticEntities();  

      // GET: Custodian  
      [RoleScreen(RoleScreen.VIEW)]    
      public ActionResult Index()
      {
            return View();  
      }

      [RoleScreen(RoleScreen.CREATE)] 
      public ActionResult Add()   
      {
            return View();  
      }  

      [HttpPost]
      [RoleScreen(RoleScreen.CREATE)]   
      public JsonResult Add(CustodianModel model)   
      {
            Result<CustodianResult> result = new ResultWithModel<CustodianResult>();     
            if (ModelState.IsValid)   
            {
                  model.create_by = User.UserId;  
                  api_static.Custodian.Create(model, p =>  
                  {
                        result = p;
                  });

                  if (result.Success)  
                  {
                        returm Json(result, JsonRequestBehavior.AllowGet);   
                  }     
                  ModelState.AddModelError("Exception", result.Message);   
            }
            return Json(result, JsonRequestBehavior.AllowGet);  
      }

      [RoleScreen(RoleScreen.VIEW)]
      public ActionResult Edit(int custodian_id)  
      {
            if (custodian_id != 0)  
            {
                  ResultWithModel<CustodianResult> result = new ResultWithModel<CustodianResult>();   
                  PagingModel pagingModel = new PagingModel();   
                  CustodianModel custodianModel = new CustodianModel();   
                  List<OrderByModel> orderByModels = new List<OrderByModel>();   
                  custodianModel.paging = pagingModel;   
                  custodianModel.ordersby = orderByModels;  
                  custodianModel.custodian_id = custodian_id;   

                  api_static.Custodian.GetEdit(custodianModel, p => 
                  {
                        result = p;  
                  });   

                  custodianModel = result.Data.CustodianResultModel[0];   

                  return View(custodianModel);    
            }
            else 
            {
                  return View("Index");   
            }  
      }   

      [HttpPost]  
      [RoleScreen(RoleScreen.EDIT)]   
      public JsonResult Edit(CustodianModel model)   
      {
            ResultWithModel<CustodianResult> result = new ResultWithModel<CustodianResult>();    
            if (ModelState.IsValid)   
            {
                  model.create_by = User.UserId;   

                  api_static.Custodian.Update(model, p =>  
                  {
                        result = p;  
                  }); 

                  if (result.Success)   
                  {
                        return Json(result, JsonRequestBehavior.AllowGet);    
                  }  
                  ModelState.AddModelError("Exception", result.Message);     
            }  
            return Json(result, JsonRequestBehavior, AllowGet);  
      }  

      [HttpPost]  
      [RoleScreen(RoleScreen.DELETE)]
      public JsonResult Delete(int custodian_id)   
      {
            ResultWithModel<CustodianResult> result = new ResultWithModel<CustodianResult>();  
            CustodianModel custodianModel = new CustodianModel();  
            CustodianModel.create_by = HttpContext.User.Identity.Name;
            custodianModel.custodian_id = custodian_id;    
            try
            {
                  api_static.Custodian.Delete(custodianModel, p => 
                  {
                        result = p;   
                  }); 

                  if (result.Success)  
                  {
                    return Json(new { success = true, responseText = "Your message successfully sent!"  }, JsonRequestBehavior.AllowGet);   
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
            ResultWithModel<CustodianResult> result = new ResultWithModel<CustodianResult>();    
            CustodianModel custodianModel = new CustodianModel();    
            try 
            {
                //Add Paging  
                PagingModel paging = new PagingModel();  
                paging.PageNumber = model.pageno;  
                paging.RecordPerPage = model.length;  
                custodianModel.paging = paging;   

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

                custodianModel.ordersby = orders;  

                var columns = model.columns.Where(o => o.search.value != null).ToList();   
                columns.ForEach(column =>  
                {
                    switch (column.data)  
                    {
                        case "custodian_code":  
                            custodianModel.custodian_code = column.search.value;  
                            break;   
                        case "custodian_shortname":
                            custodianModel.custodian_shortname = column.search.value;
                            break;   
                    }
                });  

                custodianModel.create_by = User.UserId;   

                api_static.Custodian.GetList(custodianModel, p => 
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
                data = result.Data != null ? result.Data.CustodianResultModel : new List<CustodianModel>()   
            });  
        }  

        public ActionResult FillProvince(string datastr)   
        {
            List<DDLItemModel> res = new List<DDLItemModel>();   
            api-static.province.GetDDLProvince(datastr, p => {  
                if (p.Success)  
                {
                    res = p.Data.DDLItems;   
                }
            });   

            return Json(res, JsonRequestBehavior.AllowGet);  
        }  

        public ActionResult FillDistrict(string dataint, string datastr)   
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.District.GetDDLDistrict(dataint, datastr, p => {  
                if (p.Success)   
                {
                    res = p.Data.DDLItems;   
                } 
            });   

            return Json(res, JsonRequestBehavior.AllowGet); 
        }  

        public ActionResult FillSubDistrict(string dataint, string datastr)   
        {
            list<DDLItemModel> res = new List<DDLItemModel>();
            api_static.SubDistrict.GetDDLSubDistrict(dataint, datastr, p => {
                if (p.Success)  
                {
                    res = p.Data.DDLItems;   
                }  
            });  

            return Json(res, JsonRequestBehavior.AllowGet);  
        }
    }
}

