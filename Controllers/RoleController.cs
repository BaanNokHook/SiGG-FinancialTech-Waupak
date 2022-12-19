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
    public class RoleController : BaseController
    {
       UserAndScreenEntities api_userandscreen = new UserAndScreenEntities();

        // GET: Role
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult FillRoleAndScreen(string datastr, string dataint)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_userandscreen.RoleAndScreen.GetDDLScreen(datastr, dataint, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
           ResultWithModel<RoleResult> result = new ResultWithModel<RoleResult>();
           RoleModel RoleModel = new RoleModel();
            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                RoleModel.paging = paging;

                //Add Orderby
                var orders = new List<OrderByModel>();

                if (model.order != null)
                {
                    model.order.ForEach(o =>
                    {
                        var col = model.columns[o.column];
                        orders.Add(new OrderByModel { Name = col.data, SortDirection = (o.dir.Equals("asc") ? SortDirection.Descending : SortDirection.Ascending) });
                    });
                }

                RoleModel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null).ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "role_id":
                            RoleModel.role_id = Convert.ToInt32(column.search.value);
                            break;
                        case "role_code":
                            RoleModel.role_code = column.search.value;
                            break;
                        case "role_name":
                            RoleModel.role_name = column.search.value;
                            break;
                        case "active_flag":
                            RoleModel.active_flag = Boolean.Parse(column.search.value == "true" ? "true" : "false");
                            break;
                    }
                });

                api_userandscreen.Role.GetRoleList(RoleModel, p =>
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
                data = result.Data != null ? result.Data.RoleResultModel : new List<RoleModel>()
            });
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult CreateRole(RoleViewModel view)
        {
            var rwm = new ResultWithModel<RoleResult>();
            view.FormAction.create_by = HttpContext.User.Identity.Name;
            try
            {
                if (ModelState.IsValid)
                {
                    api_userandscreen.Role.CreateRole(view.FormAction, p => {
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

        //Action Update
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult GetRoleEdit(int id)
        {
            var result = new ResultWithModel<RoleResult>();

            RoleModel model = new RoleModel();
            PagingModel paging = new PagingModel();
            paging.PageNumber = 0;
            paging.RecordPerPage = 0;
            model.paging = paging;
            model.role_id = id;
            model.create_by = HttpContext.User.Identity.Name;

            api_userandscreen.Role.GetRoleEdit(model, p =>
            {
                result = p;
            });
            return Json((result.Data.RoleResultModel.Count > 0 ? result.Data.RoleResultModel[0] : new RoleModel()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult EditRole(RoleViewModel view)
        {
            var rwm = new ResultWithModel<RoleResult>();

            try
            {
                if (ModelState.IsValid)
                {
                    api_userandscreen.Role.UpdateRole(view.FormAction, p => {
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
        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult DeleteRole(Data data)
        {
            var rwm = new ResultWithModel<RoleResult>();
            RoleModel view = new RoleModel();
            view.create_by = HttpContext.User.Identity.Name;
            view.role_id = data.role_id;
            try
            {
                api_userandscreen.Role.DeleteRole(view, p =>
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


        //====================================== ROLE AND SCREEN =======================================
        [HttpPost]
        public ActionResult SearchRoleScreen(DataTableAjaxPostModel model ,int role_id)
        {
            ResultWithModel<RoleAndScreenResult> result = new ResultWithModel<RoleAndScreenResult>();
            List<RoleAndScreenModel> roleAndScreenList = new List<RoleAndScreenModel>();
            RoleAndScreenModel RoleAndScreenModel = new RoleAndScreenModel();
            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                RoleAndScreenModel.paging = paging;

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

                RoleAndScreenModel.ordersby = orders;
                RoleAndScreenModel.role_id = role_id;

                api_userandscreen.RoleAndScreen.GetRoleScreenMappingList(RoleAndScreenModel, p =>
                {
                    result = p;
                });

                if(result.Data.RoleAndScreenResultModel != null)
                {
                    var groupHeader = result.Data.RoleAndScreenResultModel.OrderBy(a => a.row_order).Where(a => a.parent_screen_id == null).ToList();
                    int rowNumber = 1;
                    foreach (var header in groupHeader)
                    {
                        header.RowNumber = rowNumber;
                        roleAndScreenList.Add(header);
                        rowNumber++;

                        var groupParent = result.Data.RoleAndScreenResultModel.OrderBy(a => a.row_order).Where(a => a.parent_screen_id == header.screen_id).ToList();
                        foreach(var parent in groupParent)
                        {
                            parent.RowNumber = rowNumber;
                            parent.screen_name = "- " + parent.screen_name;
                            roleAndScreenList.Add(parent);
                            rowNumber++;
                        }

                    }
                }


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
                data = roleAndScreenList.Skip(model.start).Take(model.length)
                //data = result.Data.RoleAndScreenResultModel
            });
        }

        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Add(int role_id, string role_name)
        {
            ResultWithModel<RoleResult> result = new ResultWithModel<RoleResult>();
            RoleAndScreenModel RoleAndScreenModel = new RoleAndScreenModel();
            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                RoleAndScreenModel.paging = paging;
                //Add Orderby
                var orders = new List<OrderByModel>();
                RoleAndScreenModel.ordersby = orders;
                //Add role id
                RoleAndScreenModel.role_id = role_id;
                RoleAndScreenModel.role_name = role_name;

                if (RoleAndScreenModel.RoleAndScreenRightModal == null)
                {
                    RoleAndScreenModel.RoleAndScreenRightModal = new RoleAndScreenModel();
                }

                ViewBag.role_id = role_id;
                ViewBag.role_name = role_name;
            }
            catch
            {

            }
            return View(RoleAndScreenModel);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult CreateRoleAndScreen(RoleAndScreenModel model)
        {
            var rwm = new ResultWithModel<RoleAndScreenResult>();
            model.create_by = HttpContext.User.Identity.Name;
            try
            {
                if (ModelState.IsValid)
                {
                    api_userandscreen.RoleAndScreen.CreateRoleAndScreen(model, p => {
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

        //Get Data For Update
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult EditRoleAndScreen(int role_id, int screen_id)
        {
            var result = new ResultWithModel<RoleAndScreenResult>();

            RoleAndScreenModel model = new RoleAndScreenModel();
            PagingModel paging = new PagingModel();
            paging.PageNumber = 0;
            paging.RecordPerPage = 0;
            model.paging = paging;
            model.role_id = role_id;
            model.screen_id = screen_id;
            model.create_by = HttpContext.User.Identity.Name;

            api_userandscreen.RoleAndScreen.GetRoleAndScreenEditList(model, p =>
            {
                result = p;
            });
            return Json((result.Data.RoleAndScreenResultModel.Count > 0 ? result.Data.RoleAndScreenResultModel[0] : new RoleAndScreenModel()), JsonRequestBehavior.AllowGet);
        }

        //Action Update
        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult EditRoleAndScreen(RoleAndScreenModel model)
        {
            var rwm = new ResultWithModel<RoleAndScreenResult>();

            try
            {
                if (ModelState.IsValid)
                {
                    api_userandscreen.RoleAndScreen.UpdateRoleAndScreen(model, p => {
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
            public int role_id { get; set; }
            public int screen_id { get; set; }
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult DeleteRoleAndScreen(Data data)
        {
            var rwm = new ResultWithModel<RoleAndScreenResult>();
            RoleAndScreenModel model = new RoleAndScreenModel();
            model.create_by = HttpContext.User.Identity.Name;
            model.role_id = data.role_id;
            model.screen_id = data.screen_id;
            try
            {
                api_userandscreen.RoleAndScreen.DeleteRoleAndScreen(model, p =>
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
    }
}