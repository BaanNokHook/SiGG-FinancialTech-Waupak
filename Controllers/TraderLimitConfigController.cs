using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
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

    public class TraderLimitConfigController : Controller
    {
        UserAndScreenEntities Api_UserAndScreen = new UserAndScreenEntities();
        Utility utility = new Utility();

        // Action Index
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            ResultWithModel<TraderLimitConfigResult> Result = new ResultWithModel<TraderLimitConfigResult>();
            TraderLimitConfigModel objModel = new TraderLimitConfigModel();

            //Add Paging
            PagingModel paging = new PagingModel();
            paging.PageNumber = model.pageno;
            paging.RecordPerPage = model.length;
            objModel.paging = paging;

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

            objModel.ordersby = orders;
            objModel.active_flag = true;

            var columns = model.columns.Where(o => o.search.value != null).ToList();
            columns.ForEach(column =>
            {
                switch (column.data)
                {
                    case "user_id":
                        objModel.user_id = column.search.value;
                        break;
                    case "active_flag":
                        objModel.active_flag = Boolean.Parse(column.search.value == "true" ? "true" : "false");
                        break;
                }
            });

            Api_UserAndScreen.TraderLimitConfig.GetList(objModel, p =>
            {
                Result = p;
            });

            return Json(new
            {
                draw = model.draw,
                recordsTotal = Result.HowManyRecord,
                recordsFiltered = Result.HowManyRecord,
                data = Result.Data != null ? Result.Data.TraderLimitConfigResultModel : new List<TraderLimitConfigModel>()
            });
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Create(TraderLimitConfigViewModel model)
        {
            var Result = new ResultWithModel<TraderLimitConfigResult>();
            try
            {
                model.FormAction.create_by = HttpContext.User.Identity.Name;
                if (ModelState.IsValid)
                {
                    Api_UserAndScreen.TraderLimitConfig.Add(model.FormAction, p =>
                    {
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
        public ActionResult Edit(int id, string id1,bool active_flag)
        {
            TraderLimitConfigModel model = new TraderLimitConfigModel();
            ResultWithModel<TraderLimitConfigResult> result = new ResultWithModel<TraderLimitConfigResult>();
            PagingModel paging = new PagingModel();
            paging.PageNumber = 0;
            paging.RecordPerPage = 0;
            model.paging = paging;
            model.limit_id = id;
            model.active_flag = active_flag;
            model.effective_date = string.IsNullOrEmpty(id1) == true ? model.effective_date : utility.ConvertStringToDatetimeFormatDDMMYYYY(id1);

            Api_UserAndScreen.TraderLimitConfig.Find(model, p =>
            {
                result = p;
            });
            return Json((result.Data.TraderLimitConfigResultModel.Count > 0 ? result.Data.TraderLimitConfigResultModel[0] : new TraderLimitConfigModel()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Edit(TraderLimitConfigViewModel model)
        {
            var Result = new ResultWithModel<TraderLimitConfigResult>();
            try
            {
                if (ModelState.IsValid)
                {
                    model.FormAction.update_by = HttpContext.User.Identity.Name;
                    Api_UserAndScreen.TraderLimitConfig.Update(model.FormAction, p =>
                    {
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
            public int limit_id { get; set; }
            public string effective_date { get; set; }
        }


        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult Delete(Data data)
        {
            var Result = new ResultWithModel<TraderLimitConfigResult>();
            TraderLimitConfigModel model = new TraderLimitConfigModel();
            try
            {
                model.limit_id = data.limit_id;
                model.effective_date = string.IsNullOrEmpty(data.effective_date) == true ? model.effective_date : utility.ConvertStringToDatetimeFormatDDMMYYYY(data.effective_date);
                model.update_by = HttpContext.User.Identity.Name;

                Api_UserAndScreen.TraderLimitConfig.Delete(model, p =>
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