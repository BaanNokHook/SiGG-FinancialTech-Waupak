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

    public class TraderLimitController : Controller
    {
        UserAndScreenEntities Api_UserAndScreen = new UserAndScreenEntities();

        // Action Index
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            ResultWithModel<TraderLimitResult> Result = new ResultWithModel<TraderLimitResult>();
            TraderLimitModel TraderLimitModel = new TraderLimitModel();

            //Add Paging
            PagingModel paging = new PagingModel();
            paging.PageNumber = model.pageno;
            paging.RecordPerPage = model.length;
            TraderLimitModel.paging = paging;

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

            TraderLimitModel.ordersby = orders;
            TraderLimitModel.active_flag = true;

            var columns = model.columns.Where(o => o.search.value != null).ToList();
            columns.ForEach(column =>
            {
                switch (column.data)
                {
                    case "user_id":
                        TraderLimitModel.user_id = column.search.value;
                        break;
                    case "desk_group_id":
                        TraderLimitModel.desk_group_id = int.Parse(column.search.value);
                        break;
                    case "cur":
                        TraderLimitModel.cur = column.search.value;
                        break;
                    case "active_flag":
                        TraderLimitModel.active_flag = Boolean.Parse(column.search.value == "true" ? "true" : "false");
                        break;
                }
            });

            Api_UserAndScreen.TraderLimit.GetTraderLimitList(TraderLimitModel, p => {
                Result = p;
            });

            return Json(new
            {
                draw = model.draw,
                recordsTotal = Result.HowManyRecord,
                recordsFiltered = Result.HowManyRecord,
                data = Result.Data != null ? Result.Data.TraderLimitResultModel : new List<TraderLimitModel>()
            });
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Create(TraderLimitViewModel model)
        {
            var Result = new ResultWithModel<TraderLimitResult>();
            try
            {
                model.FormAction.create_by = HttpContext.User.Identity.Name;
                if (ModelState.IsValid)
                {
                    Api_UserAndScreen.TraderLimit.CreateTraderLimit(model.FormAction, p => {
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
        public ActionResult Edit(string id, string id1)
        {
            TraderLimitModel Model = new TraderLimitModel();
            ResultWithModel<TraderLimitResult> result = new ResultWithModel<TraderLimitResult>();
            PagingModel paging = new PagingModel();
            paging.PageNumber = 0;
            paging.RecordPerPage = 0;
            Model.paging = paging;
            Model.user_id = id;
            Model.cur = id1;
            Model.create_by = HttpContext.User.Identity.Name;

            Api_UserAndScreen.TraderLimit.GetTraderLimitEdit(Model, p => {
                result = p;
            });
            //return View(model);
            return Json((result.Data.TraderLimitResultModel.Count > 0 ? result.Data.TraderLimitResultModel[0] : new TraderLimitModel()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Edit(TraderLimitViewModel model)
        {
            var Result = new ResultWithModel<TraderLimitResult>();
            try
            {
                if (ModelState.IsValid)
                {
                    Api_UserAndScreen.TraderLimit.UpdateTraderLimit(model.FormAction, p => {
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

        public class Data
        {
            public string user_id { get; set; }
            public string cur { get; set; }
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult Delete(Data data)
        {
            var Result = new ResultWithModel<TraderLimitResult>();
            TraderLimitModel Model = new TraderLimitModel();
            try
            {
                Model.user_id = data.user_id;
                Model.cur = data.cur;
                Model.create_by = HttpContext.User.Identity.Name;

                Api_UserAndScreen.TraderLimit.DeleteTraderLimit(Model, p =>
                {
                    Result = p;
                });

                if (Result.Success == false)
                {
                    return Json(new { success = false, responseText = Result.Message }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, responseText = "Your message successfuly sent!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        // // Function : Binding DDL
        public ActionResult FillUser(string user_id)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            Api_UserAndScreen.TraderLimit.GetDDLUser(user_id, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillDeskGroup(string desk_group_name)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            Api_UserAndScreen.TraderLimit.GetDDLDeskGroup(desk_group_name, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCur(string cur)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            Api_UserAndScreen.TraderLimit.GetDDLCur(cur, p =>
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