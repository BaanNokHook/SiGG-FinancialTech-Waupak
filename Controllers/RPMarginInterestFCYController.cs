using GM.CommonLibs;
using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.PaymentProcess;
using GM.Data.Result.PaymentProcess;
using GM.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]

    public class RPMarginInterestFCYController : Controller
    {
        private static string Controller = "RPMarginInterestFCYCobtroller";
        private static LogFile Log = new LogFile();
        PaymentProcessEntities api = new PaymentProcessEntities();
        StaticEntities apiStatic = new StaticEntities();
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
            ResultWithModel<RPMarginInterestFCYResult> Result = new ResultWithModel<RPMarginInterestFCYResult>();
            RPMarginInterestFCYModel objModel = new RPMarginInterestFCYModel();

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


            var columns = model.columns.Where(o => o.search.value != null).ToList();
            columns.ForEach(column =>
            {
                switch (column.data)
                {
                    case "eom_date_from":
                        objModel.eom_date_from = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                        break;
                    case "eom_date_to":
                        objModel.eom_date_to = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                        break;
                    case "counter_party_id":
                        if (column.search.value != "")
                        {
                            objModel.counter_party_id = column.search.value == "" ? 0 : System.Convert.ToInt32(column.search.value);
                        }
                        else
                        {
                            objModel.counter_party_id = null;
                        }
                        break;
                    case "cur":
                        objModel.cur = column.search.value;
                        break;
                    case "margin_status":
                        objModel.margin_status = column.search.value;
                        break;
                    case "rec_pay_status":
                        objModel.rec_pay_status = column.search.value;
                        break;
                }
            });

            api.RPMarginInterestFCY.GetList(objModel, p =>
            {
                Result = p;
            });

            return Json(new
            {
                draw = model.draw,
                recordsTotal = Result.HowManyRecord,
                recordsFiltered = Result.HowManyRecord,
                data = Result.Data != null ? Result.Data.RPMarginInterestFCYResultModel : new List<RPMarginInterestFCYModel>()
            });
        }

        public ActionResult FillCur(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api.RPCallMargin.DDLCurrency(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCounterParty(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api.RPCallMargin.DDLCounterParty(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }


        [RoleScreen(RoleScreen.EDIT)]
        [HttpPost]
        public ActionResult Save(RPMarginInterestFCYModel model)
        {
            ResultWithModel<RPMarginInterestFCYResult> result = new ResultWithModel<RPMarginInterestFCYResult>();
            try
            {
                model.create_by = HttpContext.User.Identity.Name;
                api.RPMarginInterestFCY.Update(model, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception("[" + result.RefCode + "]" + result.Message);
                }
            }
            catch (Exception Ex)
            {
                result.Message = Ex.Message;
                result.Success = false;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult CheckPaymentDateInterestMargin(RPMarginInterestFCYModel model)
        {
            ResultWithModel<RPMarginInterestFCYResult> result = new ResultWithModel<RPMarginInterestFCYResult>();
            bool isSuccess = true;
            string message = string.Empty;
            try
            {
                //RPMarginInterestFCYModel model = new RPMarginInterestFCYModel();
                //model = JsonConvert.DeserializeObject<RPMarginInterestFCYModel>(data);

                api.RPMarginInterestFCY.CheckPaymentDateInterestMargin(model, p =>
                {
                    message = p.Message;
                });
            }
            catch (Exception ex)
            {
                Log.WriteLog(Controller, "Error : " + ex.Message);
                message = ex.Message;
                isSuccess = false;
            }
            return Json(new
            {
                Success = isSuccess,
                Message = message
            });
        }
    }
}