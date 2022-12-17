using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.UserAndScreen;
using GM.Data.Result.UserAndScreen;
using GM.Data.View.UserAndScreen;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]  
    [Audit]  
    public class DeskGroupController : BaseController
    {
      UserAndScreenEntities api_userandscreen = new UserAndScreenEntities();    

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
            ResultWithModel<DeskGroupResult> result = new ResultWithModel<DeskGroupResult>();  
            DeskGroupModel deskgroupmodel = new DeskGroupModel();  
            //Add paging  
            PagingModel paging = new PagingModel();  
            paging.PageNumber = model.pageno;   
            paging.RecordPerPage = model.length;  
            deskgroupmodel.paging = paging;   

            //Add Orderby
            var orders = new List<OrderByModel>();

            if (model.order != null)  
            {
                  model.order.ForEach(o => 
                  {
                        var col = model.columns[o.column];  
                        orders.Add(new OrderbyModel { Name = col.data, SortDirection = (o.dir.Equals("desc") ? SortDirection.Descending : SortDirection.Ascending) });  
                  });
            }

            deskgroupmodel.ordersby = orders;  

            var columns = model.columns.Where(o = > o.search.value != null).ToList();  
            columns.ForEach(column => 
            {
                  switch (column.data)  
                  {
                        case "desk_group_id":
                              deskgroupmodel.desk_group_id = int.Parse(column.search.value) ;
                              break;  
                        case "desk_group_name":   
                              deskgroupmodel.desk_group_name = column.search.value;      
                              break; 
                        case "active_flag":  
                              deskgroupmodel.active_flag = Boolean.Parse(column.search.value == "true" ? "true" : "false");
                              break;   
                  }  
            });  

            api_userandscreen.DeskGroup.GetDeskGroupList(deskgroupmodel, p => {  
                result = p;  
            });  
            return Json(new
            {
                draw = model.draw,  
                recordsTotal = result.HowManyRecord,    
                recordsFiltered = result.HowManyRecord,  
                data = result.Data != null ? result.Data.DeskGroupResultModel:new list<DeskGroupModel>()  
            });  
      } 

      [HttpPost]  
      [RoleScreen(RoleScreen.CREATE)]  
      public ActionResult Create(DeskGroupViewModel view)   
      {
          var rwm = new ResultWithModel<DeskGroupResult>();  
          view.FormAction.create_by = HttpContext.User.identity.Name;   
          try
          {
                //if (ModelState.ContainsKey("FormAction.desk_group_id"))
                //{
                //    ModelState["FormAction.desk_group_id"].Errors.Clear();
                //}
                if (ModelState.IsValid)
                {
                    api_userandscreen.DeskGroup.CreateDeskGroup(view.FormAction, p => { 
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

       [RoleScreen(RoleScreen.VIEW)]  
       public ActionResult Edit(string id)  
       {
        
            DeskGroupModel model = new DeskGroupModel();
            ResultWithModel<DeskGroupResult> result = new ResultWithModel<DeskGroupResult>();
            PagingModel paging = new PagingModel();
            paging.PageNumber = 0;
            paging.RecordPerPage = 0;
            model.paging = paging;
            model.desk_group_id = int.Parse(id);
            model.create_by = HttpContext.User.Identity.Name;

            api_userandscreen.DeskGroup.GetDeskGroupEdit(model, p => {
                result = p;
            });
            //return View(model);
            return Json((result.Data.DeskGroupResultModel.Count > 0 ? result.Data.DeskGroupResultModel[0] : new DeskGroupModel()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Edit(DeskGroupViewModel view)
        {
            var rwm = new ResultWithModel<DeskGroupResult>();

            try
            {
                if (ModelState.IsValid)
                {

                    api_userandscreen.DeskGroup.UpdateDeskGroup(view.FormAction, p => {
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

        public class Data
        {
            public int deskgroupid { get; set; }
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult Deletes(Data data)
        {
            var rwm = new ResultWithModel<DeskGroupResult>();
            DeskGroupModel view = new DeskGroupModel();
            view.create_by = HttpContext.User.Identity.Name;
            view.desk_group_id = data.deskgroupid;
            try
            {
                api_userandscreen.DeskGroup.DeleteDeskGroup(view, p =>
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
                // return View();
            }
            // return Json(rwm, JsonRequestBehavior.AllowGet);
        }
    }
}