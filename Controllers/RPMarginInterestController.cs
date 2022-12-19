using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.PaymentProcess;
using GM.Data.Result.PaymentProcess;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class RPMarginInterestController : BaseController
    {
        PaymentProcessEntities api = new PaymentProcessEntities();
        SecurityEntities api_security = new SecurityEntities();
        GM.Data.Helper.Utility utility = new GM.Data.Helper.Utility();
        // GET: Security

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            ResultWithModel<RPMarginInterestResult> result = new ResultWithModel<RPMarginInterestResult>();
            RPMarginInterestModel rpMarginInterestModel = new RPMarginInterestModel();
            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                rpMarginInterestModel.paging = paging;

                //Add Orderby
                var orders = new List<OrderByModel>();

                if (model.order != null)
                {
                    model.order.ForEach(o =>
                    {
                        var col = model.columns[2];
                        orders.Add(new OrderByModel { Name = col.data, SortDirection = (o.dir.Equals("desc") ? SortDirection.Descending : SortDirection.Ascending) });
                    });
                }

                rpMarginInterestModel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null).ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "asof_date":
                            rpMarginInterestModel.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "counter_party_code":
                            rpMarginInterestModel.counter_party_code = column.search.value;
                            break;
                        case "cur":
                            rpMarginInterestModel.cur = column.search.value;
                            break;
                        case "trans_no":
                            rpMarginInterestModel.trans_no = column.search.value;
                            break;
                    }
                });

                StaticEntities db = new StaticEntities();
                //ResultWithModel<BusinessDateResult> rwm = new ResultWithModel<BusinessDateResult>();
                //BusinessDateModel BusinessDateModel = new BusinessDateModel();
                //PagingModel pagingBusDate = new PagingModel();
                //pagingBusDate.PageNumber = 1;
                //pagingBusDate.RecordPerPage = 20;
                //BusinessDateModel.paging = pagingBusDate;
                //if (rpMarginInterestModel.payment_date == null)
                //{
                //    db.BusinessDate.GetBusinessDateList(BusinessDateModel, p =>
                //    {
                //        rwm = p;
                //    });

                //    if (rwm.Success)
                //    {
                //        rpMarginInterestModel.payment_date = rwm.Data.BusinessDateResultModel[0].business_date;
                //    }
                //}

                api.RPMarginInterest.GetRPMarginInterestList(rpMarginInterestModel, p =>
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
                data = result.Data != null ? result.Data.RPMarginInterestResultModel : new List<RPMarginInterestModel>()
            });
        }

        private string WrapStringInQuotes(string input)
        {
            return @"""" + input + @"""";
        }

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Edit(string trans_no, string asof_date)
        {
            if (trans_no == null && asof_date == null )
            {
                return View("Index");
            }
            else
            {
                ResultWithModel<RPMarginInterestResult> result = new ResultWithModel<RPMarginInterestResult>();
                RPMarginInterestModel RPMarginInterestModel = new RPMarginInterestModel();

                RPMarginInterestModel.trans_no = string.IsNullOrEmpty(trans_no) == true ? RPMarginInterestModel.trans_no : trans_no;
                RPMarginInterestModel.asof_date = string.IsNullOrEmpty(asof_date) == true ? RPMarginInterestModel.asof_date : utility.ConvertStringToDatetimeFormatDDMMYYYY(asof_date); 
              

                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = 1;
                paging.RecordPerPage = 1;
                RPMarginInterestModel.paging = paging;

                api.RPMarginInterest.GetRPMarginInterestList(RPMarginInterestModel, p =>
                {
                    result = p;
                });

                RPMarginInterestModel = result.Data.RPMarginInterestResultModel[0];

                return View(RPMarginInterestModel);
            }
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Save(RPMarginInterestModel RPMarginInterestModel)
        {
            ResultWithModel<RPMarginInterestResult> res = new ResultWithModel<RPMarginInterestResult>();
            var Result = new List<object>();
            try
            {
                RPMarginInterestModel.update_by = HttpContext.User.Identity.Name;

                api.RPMarginInterest.UpdateRPMarginInterest(RPMarginInterestModel, p =>
                {
                    res = p;
                });             

                if (!res.Success)
                {
                    Result.Add(new { Message = res.Message });
                }
                else
                {
                    Result.Add(new { Message = res.Message });
                }
            }
            catch (Exception ex)
            {
                Result.Add(new { Message = ex.Message });
            }
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCounterPartyPayment(string counterpartyid)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api.RPCoupon.GetDDLPaymentMethod("CNP", "PAY", p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillMTCode(string paymentmethod, string transdealtype,string cur)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api.RPCoupon.GetDDLMTCode(paymentmethod, null, "CNP",cur, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCurrency(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLCur(datastr, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}