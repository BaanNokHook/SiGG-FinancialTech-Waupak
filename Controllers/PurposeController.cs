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
    public class PurposeController : BaseController
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
            ResultWithModel<PurposeResult> result = new ResultWithModel<PurposeResult>();
            PurposeModel purposeModel = new PurposeModel();
            //Add Paging
            PagingModel paging = new PagingModel();
            paging.PageNumber = model.pageno;
            paging.RecordPerPage = model.length;
            purposeModel.paging = paging;
            purposeModel.create_by = User.UserId;

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

            purposeModel.ordersby = orders;

            var columns = model.columns.Where(o => o.search.value != null).ToList();
            columns.ForEach(column =>
            {
                switch (column.data)
                {
                    case "purpose":
                        purposeModel.purpose = column.search.value;
                        break;
                    case "description":
                        purposeModel.description = column.search.value;
                        break;
                }
            });


            apiStatic.Purpose.GetPurposeList(purposeModel, p => {
                result = p;
            });

            return Json(new
            {
                draw = model.draw,
                recordsTotal = result.HowManyRecord,
                recordsFiltered = result.HowManyRecord,
                data = result.Data != null ? result.Data.PurposeResultModel : new List<PurposeModel>()
            });

            //return Json(new {  data = result.Data.UserModels  }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Create(PurposeModel model)
        {
            ResultWithModel<PurposeResult> result = new ResultWithModel<PurposeResult>();
            try
            {
                if (ModelState.IsValid)
                {
                    model.create_by = User.UserId;
                    apiStatic.Purpose.Create(model, p =>
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
        public ActionResult GetEdit(string purpose)
        {
            ResultWithModel<PurposeResult> result = new ResultWithModel<PurposeResult>();
            try
            {
                PurposeModel model = new PurposeModel();
                PagingModel paging = new PagingModel();
                paging.PageNumber = 0;
                paging.RecordPerPage = 0;
                model.paging = paging;

                model.purpose = purpose;
                model.create_by = HttpContext.User.Identity.Name;

                apiStatic.Purpose.GetPurposeList(model, p =>
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
        public ActionResult Edit(PurposeModel model)
        {
            ResultWithModel<PurposeResult> result = new ResultWithModel<PurposeResult>();
            try
            {
                if (ModelState.IsValid)
                {
                    model.update_by = User.UserId;
                    apiStatic.Purpose.Update(model, p =>
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
        public ActionResult Delete(PurposeModel model)
        {
            ResultWithModel<PurposeResult> result = new ResultWithModel<PurposeResult>();
            try
            {
                model.update_by = User.UserId;
                apiStatic.Purpose.Delete(model, p =>
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
    }
}