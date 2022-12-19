using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.ExchangeRate;
using GM.Data.Model.GLProcess;
using GM.Data.Model.Static;
using GM.Data.Result.ExchangeRate;
using GM.Data.Result.GLProcess;
using GM.Data.Result.Static;
using GM.Data.View.GLProcess;
using GM.Filters;

namespace GM.Application.Web.Controllers
{
    [Authorize]  
    [Audit]
    public class GLAdjustController : BaseController  
    {
      private readonly GLProcessEntities api;  
      private readonly Utility utility;  

      public GLAdjustController()  
      {
            api = new GLProcessEntities();  
            utility = new Utility();  
      }

      [RoleScreen(RoleScreen.VIEW)]
      public ActionResult Index()  
      {
            return View();  
      }

      [RoleScreenRole(RoleScreen.CREATE)]
      public ActionResult Add()  
      {
            return View();  
      }  

      [HttpPost]
      [RoleScreen(RoleScreen.CREATE)]  
      public JsonResult Add(List<GLAdjustModel> items)
      {
            ResultWithModel<GLAdjustResult> result = new ResultWithModel<GLAdjustResult>();

            if (items == null)
            {
                result.Success = false;
                result.Message = "Data not found";
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                items.ForEach(s => s.create_by = User.UserId);
                api.GLAdjust.CreateGLAdjust(items, p =>
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
      public ActionResult Edit(string adjust_num)
      {
            ResultWithModel<GLAdjustResult> result = new ResultWithModel<GLAdjustResult>();
            GLAdjustModel model = new GLAdjustModel();
            model.paging = new PagingModel(){ PageNumber = 1, RecordPerPage = 1 };
            model.ordersby = new List<OrderByModel>();
            model.adjust_num = adjust_num;
            model.create_by = User.UserId;

            api.GLAdjust.GetGLAdjustDetail(model, p =>
            {
                result = p;
            });

            model = result.Data.GLAdjustResultModel[0];
            model.account_no = string.Empty;
            model.account_name = string.Empty;
            model.dr_cr = string.Empty;
            model.amount = 0;
            model.cost_center = string.Empty;
            model.note = string.Empty;
            return View(model);
      }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public JsonResult Edit(List<GLAdjustModel> items)
        {
            ResultWithModel<GLAdjustResult> result = new ResultWithModel<GLAdjustResult>();

            if (items == null)
            {
                result.Success = false;
                result.Message = "Data not found";
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                items.ForEach(s => s.create_by = User.UserId);
                api.GLAdjust.UpdateGLAdjust(items, p =>
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
        public ActionResult Delete(string adjust_num)
        {
            ResultWithModel<GLAdjustResult> result = new ResultWithModel<GLAdjustResult>();
            try
            {
                GLAdjustModel model = new GLAdjustModel();
                model.adjust_num = adjust_num;
                model.update_by = User.UserId;
                api.GLAdjust.DeleteGLAdjust(model, p =>
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

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            ResultWithModel<GLAdjustResult> Result = new ResultWithModel<GLAdjustResult>();
            GLAdjustModel glAdjustModel = new GLAdjustModel();

            //Add Paging
            PagingModel paging = new PagingModel();
            paging.PageNumber = model.pageno;
            paging.RecordPerPage = model.length;
            glAdjustModel.paging = paging;
            glAdjustModel.create_by = User.UserId;

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

            glAdjustModel.ordersby = orders;

            var columns = model.columns.Where(o => o.search.value != null).ToList();
            columns.ForEach(column =>
            {
                switch (column.data)
                {
                    case "adjust_num":
                        glAdjustModel.adjust_num = column.search.value;
                        break;
                    case "posting_date":
                        glAdjustModel.posting_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                        break;
                    case "trans_port":
                        glAdjustModel.trans_port = column.search.value;
                        break;
                    case "trans_no":
                        glAdjustModel.trans_no = column.search.value;
                        break;
                    case "cur":
                        glAdjustModel.cur = column.search.value;
                        break;
                }
            });

            api.GLAdjust.GetGLAdjustList(glAdjustModel, p =>
            {
                Result = p;
            });

            return Json(new
            {
                draw = model.draw,
                recordsTotal = Result.HowManyRecord,
                recordsFiltered = Result.HowManyRecord,
                data = Result.Data != null ? Result.Data.GLAdjustResultModel : new List<GLAdjustModel>()
            });
        }

        [HttpGet]
        public ActionResult GetDetail(string adjust_num)
        {
            ResultWithModel<GLAdjustResult> result = new ResultWithModel<GLAdjustResult>();
            GLAdjustModel detailModel = new GLAdjustModel();
            detailModel.paging = new PagingModel(){PageNumber = 1, RecordPerPage = 99999};
            detailModel.ordersby = new List<OrderByModel>();
            detailModel.adjust_num = adjust_num;
            detailModel.create_by = User.UserId;

            api.GLAdjust.GetGLAdjustDetail(detailModel, p =>
            {
                result = p;
            });

            return Json(new
            {
                recordsTotal = result.HowManyRecord,
                recordsFiltered = result.HowManyRecord,
                data = result.Data != null ? result.Data.GLAdjustResultModel : new List<GLAdjustModel>()
                
            }, JsonRequestBehavior.AllowGet);
        }

        #region :: DropDown ::
        public ActionResult FillCur(string datastr)
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

        public ActionResult FillAdjustPort(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api.GLAdjust.GetDDlAdjustPort(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillAdjustTrans(string trans_date, string trans_no)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            var date = utility.ConvertStringToDatetimeFormatDDMMYYYY(trans_date);
            api.GLAdjust.GetDDlAdjustTrans(date.ToString("yyyyMMdd"), trans_no, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillAdjustCounterParty(string counter_party)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api.GLAdjust.GetDDlAdjustCounterParty(counter_party, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillAdjustCostCenter(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api.GLAdjust.GetDDlAdjustCostCenter(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillAdjustAccountCode(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api.GLAdjust.GetDDlAdjustAccountCode(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}