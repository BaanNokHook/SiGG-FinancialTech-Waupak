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
    public class ScreenController : BaseController
    {
        UserAndScreenEntities api_screen = new UserAndScreenEntities();

        // GET: Role
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            ScreenViewModel model = new ScreenViewModel();
            model.FormAction = new ScreenModel();
            model.FormSearch = new ScreenModel();
            return View(model);
        }

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Edit(string id)
        {
            ScreenModel screenmodel = new ScreenModel();
            ResultWithModel<ScreenResult> result = new ResultWithModel<ScreenResult>();
            PagingModel paging = new PagingModel();
            paging.PageNumber = 0;
            paging.RecordPerPage = 0;
            screenmodel.paging = paging;
            screenmodel.screen_id = int.Parse(id);
            screenmodel.create_by = HttpContext.User.Identity.Name;

            api_screen.Screen.GetScreensEdit(screenmodel, p => {
                result = p;
            });
            ScreenViewModel model = new ScreenViewModel();
            model.FormAction = result.Data.ScreenResultModel[0];
            //return View(model);
            return Json((result.Data.ScreenResultModel.Count > 0 ? result.Data.ScreenResultModel[0] : new ScreenModel()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            string searchvalue = Request["search[value]"];
            ResultWithModel<RolesScreenMappingResult> result = new ResultWithModel<RolesScreenMappingResult>();
            ScreenModel screenmodel = new ScreenModel();
            //Add Paging
            PagingModel paging = new PagingModel();
            paging.PageNumber = model.pageno;
            paging.RecordPerPage = model.length;
            screenmodel.paging = paging;

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

            screenmodel.ordersby = orders;

            var columns = model.columns.Where(o => o.search.value != null).ToList();           
            columns.ForEach(column =>
            {
                switch (column.data)
                {
                    case "screen_id":
                        screenmodel.screen_id = int.Parse(column.search.value) ;
                        break;
                    case "screen_name":
                        screenmodel.screen_name = column.search.value;
                        break;
                    case "controller":
                        screenmodel.controller = column.search.value;
                        break;
                    case "action":
                        screenmodel.action = column.search.value;
                        break;
                    case "operation_id":
                        screenmodel.operation_id = int.Parse(column.search.value);
                        break;
                    case "parent_screen_id":
                        screenmodel.parent_screen_id = int.Parse(column.search.value);
                        break;
                    case "active_flag":                       
                        screenmodel.active_flag = Boolean.Parse(column.search.value == "true" ? "true" : "false");
                        break;
                    case "icon":
                        screenmodel.icon = column.search.value;
                        break;
                }
            });

            api_screen.Screen.GetScreensList(screenmodel, p => {
                result = p;
            });

            return Json(new
            {
                draw = model.draw,
                recordsTotal = result.HowManyRecord,
                recordsFiltered = result.HowManyRecord,
                data = result.Data != null ? result.Data.RolesScreenMappingResultModels : new List<RolesScreenMappingResultModel>()
            });
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Create(ScreenViewModel view)
        {
            var rwm = new ResultWithModel<ScreenResult>();
            view.FormAction.create_by = HttpContext.User.Identity.Name;
            try
            {
                if (ModelState.ContainsKey("FormAction.screen_id"))
                {
                    ModelState["FormAction.screen_id"].Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    api_screen.Screen.CreateScreen(view.FormAction, p => {
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

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Edit(ScreenViewModel view)
        {
            var rwm = new ResultWithModel<ScreenResult>();

            try
            {
                if (ModelState.IsValid)
                {
                    
                    api_screen.Screen.UpdateScreen(view.FormAction, p => {
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
            public int screenid { get; set; }
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult Deletes(Data data)
        {
            var rwm = new ResultWithModel<ScreenResult>();
            ScreenModel view = new ScreenModel();
            view.create_by = HttpContext.User.Identity.Name;
            view.screen_id = data.screenid;
            try
            {
                api_screen.Screen.DeleteScreenID(view, p =>
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

        public ActionResult FillOperation(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_screen.Screen.GetDDLOperation(datastr, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillParentScreen(string datastr,string operation_id)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_screen.Screen.GetDDLParentScreen(operation_id, datastr, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}