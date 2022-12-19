using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.Static;
using GM.Data.Result.Static;
using GM.Filters;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class UserSignNameController : BaseController
    {
        StaticEntities apiStatic = new StaticEntities();

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            ResultWithModel<UserSignNameResult> result = new ResultWithModel<UserSignNameResult>();
            UserSignNameModel userSignNameModel = new UserSignNameModel();
            //Add Paging
            PagingModel paging = new PagingModel();
            paging.PageNumber = model.pageno;
            paging.RecordPerPage = model.length;
            userSignNameModel.paging = paging;
            userSignNameModel.create_by = User.UserId;

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

            userSignNameModel.ordersby = orders;

            var columns = model.columns.Where(o => o.search.value != null).ToList();
            columns.ForEach(column =>
            {
                switch (column.data)
                {
                    case "fname":
                        userSignNameModel.fname = column.search.value;
                        break;
                    case "position":
                        userSignNameModel.position = column.search.value;
                        break;
                }
            });


            apiStatic.UserSignName.GetUserSignNameList(userSignNameModel, p => {
                result = p;
            });

            return Json(new
            {
                draw = model.draw,
                recordsTotal = result.HowManyRecord,
                recordsFiltered = result.HowManyRecord,
                data = result.Data != null ? result.Data.UserSignNameResultModel : new List<UserSignNameModel>()
            });
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Create(UserSignNameModel model)
        {
            ResultWithModel<UserSignNameResult> result = new ResultWithModel<UserSignNameResult>();
            try
            {
                if (ModelState.IsValid)
                {
                    model.create_by = User.UserId;
                    apiStatic.UserSignName.Create(model, p =>
                    {
                        result = p;
                    });

                    if (result.Success)
                    {
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ModelState.AddModelError("Exception", result.Message);
                }
            }
            catch (Exception ex)
            {
                result.RefCode = 99;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult GetEdit(int id)
        {
            ResultWithModel<UserSignNameResult> result = new ResultWithModel<UserSignNameResult>();
            try
            {
                UserSignNameModel model = new UserSignNameModel();
                PagingModel paging = new PagingModel();
                paging.PageNumber = 0;
                paging.RecordPerPage = 0;
                model.paging = paging;

                model.id = id;
                model.create_by = HttpContext.User.Identity.Name;

                apiStatic.UserSignName.GetUserSignNameList(model, p =>
                {
                    result = p;
                });
            }
            catch (Exception ex)
            {

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Edit(UserSignNameModel model)
        {
            ResultWithModel<UserSignNameResult> result = new ResultWithModel<UserSignNameResult>();
            try
            {
                if (ModelState.IsValid)
                {
                    model.update_by = User.UserId;
                    apiStatic.UserSignName.Update(model, p =>
                    {
                        result = p;
                    });

                    if (result.Success)
                    {
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ModelState.AddModelError("Exception", result.Message);
                }
            }
            catch (Exception ex)
            {
                result.RefCode = 99;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public ActionResult Delete(UserSignNameModel model)
        {
            ResultWithModel<UserSignNameResult> result = new ResultWithModel<UserSignNameResult>();
            try
            {
                model.update_by = User.UserId;
                apiStatic.UserSignName.Delete(model, p =>
                {
                    result = p;
                });

                if (result.Success)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                result.RefCode = 99;
                result.Message = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}