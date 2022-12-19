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
    public class InitialMarginController : BaseController
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
            string searchvalue = Request["search[value]"];
            ResultWithModel<InitialMarginResult> result = new ResultWithModel<InitialMarginResult>();
            InitialMarginModel initialMarginModel = new InitialMarginModel();
            //Add Paging
            PagingModel paging = new PagingModel();
            paging.PageNumber = model.pageno;
            paging.RecordPerPage = model.length;
            initialMarginModel.paging = paging;

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

            initialMarginModel.ordersby = orders;

            var columns = model.columns.Where(o => o.search.value != null).ToList();
            columns.ForEach(column =>
            {
                switch (column.data)
                {
                    case "SECURITYTYPE_ID":
                        initialMarginModel.SECURITYTYPE_ID = column.search.value;
                        break;
                    case "COUPONTYPE_ID":
                        initialMarginModel.COUPONTYPE_ID = column.search.value;
                        break;
                    case "DESCRIPTION":
                        initialMarginModel.DESCRIPTION = column.search.value;
                        break;
                }
            });


            apiStatic.InitialMargin.GetInitialMarginList(initialMarginModel, p => {
                result = p;
            });

            return Json(new
            {
                draw = model.draw,
                recordsTotal = result.HowManyRecord,
                recordsFiltered = result.HowManyRecord,
                data = result.Data != null ? result.Data.InitialMarginResultModel : new List<InitialMarginModel>()
            });

            //return Json(new {  data = result.Data.UserModels  }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Create(InitialMarginModel model)
        {
            ResultWithModel<InitialMarginResult> result = new ResultWithModel<InitialMarginResult>();
            try
            {
                if (ModelState.IsValid)
                {
                    model.create_by = User.UserId;
                    apiStatic.InitialMargin.CreateInitialMargin(model, p =>
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
            catch (Exception Ex)
            {
                result.RefCode = 99;
                result.Message = Ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult GetEdit(int Id)
        {
            ResultWithModel<InitialMarginResult> result = new ResultWithModel<InitialMarginResult>();
            try
            {
                InitialMarginModel model = new InitialMarginModel();
                PagingModel paging = new PagingModel();
                paging.PageNumber = 0;
                paging.RecordPerPage = 0;
                model.paging = paging;

                model.ID = Id;
                model.create_by = HttpContext.User.Identity.Name;

                apiStatic.InitialMargin.GetInitialMarginList(model, p =>
                {
                    result = p;
                });
            }
            catch (Exception Ex)
            {

            }

            return Json(result, JsonRequestBehavior.AllowGet);
            //return Json((result.Data.InitialMarginResultModel.Count > 0 ? result.Data.InitialMarginResultModel[0] : new InitialMarginModel()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Edit(InitialMarginModel model)
        {
            ResultWithModel<InitialMarginResult> result = new ResultWithModel<InitialMarginResult>();
            try
            {
                if (ModelState.IsValid)
                {
                    model.update_by = User.UserId;
                    apiStatic.InitialMargin.UpdateInitialMargin(model, p =>
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
            catch (Exception Ex)
            {
                result.RefCode = 99;
                result.Message = Ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public ActionResult Delete(InitialMarginModel model)
        {
            ResultWithModel<InitialMarginResult> result = new ResultWithModel<InitialMarginResult>();
            try
            {
                model.update_by = User.UserId;
                apiStatic.InitialMargin.DeleteInitialMargin(model, p =>
                {
                    result = p;
                });

                if (result.Success)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception Ex)
            {
                result.RefCode = 99;
                result.Message = Ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCouponType(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            apiStatic.InitialMargin.GetDDlCouponType(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillSecurityType(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            apiStatic.InitialMargin.GetDDlSecurityType(datastr, p =>
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