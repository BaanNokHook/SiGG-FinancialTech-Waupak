using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GM.CommonLibs;
using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.PaymentProcess;
using GM.Data.Model.Provider;
using GM.Data.Model.Report;
using GM.Data.Model.RPTransaction;
using GM.Data.Model.Static;
using GM.Data.Result.PaymentProcess;
using GM.Data.Result.RPTransaction;
using GM.Data.Result.Static;
using GM.Data.View.PaymentProcess;
using GM.Data.View.RPTransaction;
using GM.Filters;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using SortDirection = GM.Data.Model.Common.SortDirection;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class RPReleaseMessageController : BaseController
    {
        private static string Controller = "RPReleaseMessageController";
        private static LogFile Log = new LogFile();
        PaymentProcessEntities apiRPReleaseMsg = new PaymentProcessEntities();
        StaticEntities apiStatic = new StaticEntities();
        Utility utility = new Utility();
        ExternalInterfaceEntities apiEx = new ExternalInterfaceEntities();

        public class Data
        {
            public string trans_no { get; set; }
            public string from_page { get; set; }
            public string event_type { get; set; }
            public string trans_deal_type { get; set; }
            public string payment_method { get; set; }
            public string trans_mt_code { get; set; }
            public string cur { get; set; }
        }

        public class DataForReleaseMsg
        {
            public int counter_party_id { get; set; }
            public DateTime? call_date { get; set; }
            public string payment_method { get; set; }
            public string mt_code { get; set; }
            public string cur { get; set; }
            public int row { get; set; }
            public string counter_party_name { get; set; }
            public string brp_contract_no { get; set; }
        }

        // Action : Index
        public ActionResult GetReleaseMT(Data data)
        {
            RPTransModel RPTransModel = new RPTransModel();
            ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();
            RPReleaseMessageResult RPReleaseMessageResultModel = new RPReleaseMessageResult();
            PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();
            try
            {
                //if (data.from_page != "COUPON") 
                {
                    #region Check ReleaseMsg
                    // Step 1 : Check ReleaseMsg
                    bool IsPayment = false;

                    ResultWithModel<RPReleaseMsgCheckPaymentResult> Result = new ResultWithModel<RPReleaseMsgCheckPaymentResult>();
                    RPReleaseMsgCheckPaymentModel ChkPaymentModel = new RPReleaseMsgCheckPaymentModel();

                    ChkPaymentModel.from_page = data.from_page;
                    ChkPaymentModel.event_type = data.event_type;
                    ChkPaymentModel.trans_deal_type = data.trans_deal_type;
                    ChkPaymentModel.payment_method = data.payment_method;
                    ChkPaymentModel.mt_code = data.trans_mt_code;

                    api_RPReleaseMsg.RPReleaseMessage.CheckPaymentMethod(ChkPaymentModel, p =>
                    {
                        if (p.Success)
                        {
                            Result = p;
                        }
                        else
                        {
                            throw new Exception(Result.Message);
                        }
                    });

                    IsPayment = Result.Data.RPReleaseMsgCheckPaymentResultModel[0].is_payment;

                    if (IsPayment == true)
                    {
                        if (data.from_page != "COUPON")
                        {
                            // ไม่ต้อง Gen ก่อนดู Message
                            #endregion

                            #region Gen ReleaseMsg 
                            // Step 2 : Gen ReleaseMsg 
                            RPTransModel = new RPTransModel();
                            RPTransModel.trans_no = data.trans_no;
                            RPTransModel.event_type = data.from_page;
                            RPTransModel.payment_method = data.payment_method;
                            RPTransModel.trans_mt_code = data.trans_mt_code;
                            RPTransModel.cur = data.cur;
                            RPTransModel.create_by = HttpContext.User.Identity.Name;
                            api_RPReleaseMsg.RPReleaseMessage.GenRPMessage(RPTransModel, p =>
                            {
                                result = p;
                            });
                            if (!result.Success)
                            {
                                throw new Exception("GenRPReleaseMessage() => " + result.Message);
                            }
                            #endregion
                        }
                        else
                        {
                            RPCouponModel model = new RPCouponModel();
                            model.trans_cno = data.trans_no;
                            model.event_type = data.from_page;
                            model.payment_method = data.payment_method;
                            model.mt_code = data.trans_mt_code;
                            model.cur = data.cur;
                            model.create_by = HttpContext.User.Identity.Name;
                            model.event_type = "Coupon";

                            apiRPReleaseMsg.RPCoupon.GenRPReleaseMessage(model, p =>
                            {
                                result = p;
                            });

                            if (!result.Success)
                            {
                                throw new Exception("GenRPReleaseMessageCoupon() => " + result.Message);
                            }
                        }
                    }
                }

                // Step 3 : View ReleaseMsg 
                RPTransModel = new RPTransModel();
                result = new ResultWithModel<RPReleaseMessageResult>();
                var orders = new List<OrderByModel>();
                PagingModel paging = new PagingModel();

                paging.PageNumber = 0;
                paging.RecordPerPage = 0;
                RPTransModel.paging = paging;
                RPTransModel.ordersby = orders;

                RPTransModel.trans_no = data.trans_no;
                RPTransModel.payment_method = data.payment_method;
                RPTransModel.trans_mt_code = data.trans_mt_code;
                RPTransModel.cur = data.cur;

                api_RPReleaseMsg.RPReleaseMessage.GetRPReleaseMessageMTList(RPTransModel, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception("RPReleaseMessage() => " + result.Message);
                }

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(result.Data != null ? result.Data.RPReleaseMessageResultModel : new List<RPReleaseMessageModel>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReleaseMTNetSettle(Data data)
        {
            RPTransModel RPTransModel = new RPTransModel();
            ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();
            RPReleaseMessageResult RPReleaseMessageResultModel = new RPReleaseMessageResult();
            PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();
            try
            {

                ResultWithModel<RPReleaseMsgCheckPaymentResult> Result = new ResultWithModel<RPReleaseMsgCheckPaymentResult>();

                #region Gen ReleaseMsg 
                // Step 1 : Gen ReleaseMsg 
                RPTransModel = new RPTransModel();
                RPTransModel.trans_no = data.trans_no;
                RPTransModel.event_type = data.from_page;
                RPTransModel.payment_method = data.payment_method;
                RPTransModel.trans_mt_code = data.trans_mt_code;
                RPTransModel.cur = data.cur;
                RPTransModel.create_by = HttpContext.User.Identity.Name;
                api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageNetSettle(RPTransModel, p =>
                {
                    result = p;
                });
                if (!result.Success)
                {
                    throw new Exception("GenRPReleaseMessageNetSettle() => " + result.Message);
                }
                #endregion

                //Step 2 : View ReleaseMsg
                RPTransModel = new RPTransModel();
                result = new ResultWithModel<RPReleaseMessageResult>();
                var orders = new List<OrderByModel>();
                PagingModel paging = new PagingModel();

                paging.PageNumber = 0;
                paging.RecordPerPage = 0;
                RPTransModel.paging = paging;
                RPTransModel.ordersby = orders;

                RPTransModel.trans_no = data.trans_no;
                RPTransModel.payment_method = data.payment_method;
                RPTransModel.trans_mt_code = data.trans_mt_code;
                RPTransModel.cur = data.cur;

                api_RPReleaseMsg.RPReleaseMessage.GetRPReleaseMessageMTList(RPTransModel, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception("RPReleaseMessage() => " + result.Message);
                }

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(result.Data != null ? result.Data.RPReleaseMessageResultModel : new List<RPReleaseMessageModel>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReleaseCoupon(Data data)
        {
            RPTransModel RPTransModel = new RPTransModel();
            ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();
            RPReleaseMessageResult RPReleaseMessageResultModel = new RPReleaseMessageResult();
            PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();
            try
            {
                RPTransModel = new RPTransModel();
                result = new ResultWithModel<RPReleaseMessageResult>();
                var orders = new List<OrderByModel>();
                PagingModel paging = new PagingModel();

                paging.PageNumber = 0;
                paging.RecordPerPage = 0;
                RPTransModel.paging = paging;
                RPTransModel.ordersby = orders;

                RPTransModel.trans_no = data.trans_no;
                RPTransModel.payment_method = data.payment_method;
                RPTransModel.trans_mt_code = data.trans_mt_code;
                RPTransModel.cur = data.cur;

                api_RPReleaseMsg.RPReleaseMessage.GetRPReleaseMessageMTList(RPTransModel, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception("RPReleaseMessage() => " + result.Message);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(result.Data != null ? result.Data.RPReleaseMessageResultModel : new List<RPReleaseMessageModel>(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult GetReleaseCallMargin(string data)
        {
            ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();

            try
            {
                DataForReleaseMsg tmpData = JsonConvert.DeserializeObject<DataForReleaseMsg>(data);
                PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();

                // Step 2 : Gen ReleaseMsg 
                RPCallMarginModel model = new RPCallMarginModel();
                model.create_by = HttpContext.User.Identity.Name;
                model.counter_party_id = tmpData.counter_party_id;
                model.call_date = tmpData.call_date;
                model.cur = tmpData.cur;
                model.payment_method = tmpData.payment_method;
                model.mt_code = tmpData.mt_code;
                model.event_type = "Margin";
                model.brp_contract_no = tmpData.brp_contract_no;

                api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageByMargin(model, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception("ReleaseMessage : " + result.Message);
                }

                api_RPReleaseMsg.RPReleaseMessage.RPReleaseMessageCallMargin(model, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception("GetReleaseMessage : " + result.Message);
                }
            }
            catch (Exception Ex)
            {
                result.Message = Ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult GetReleaseInterestMargin(string data)
        {
            ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();

            try
            {
                DataForReleaseMsg tmpData = JsonConvert.DeserializeObject<DataForReleaseMsg>(data);
                PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();

                // Step 2 : Gen ReleaseMsg 
                RPCallMarginModel model = new RPCallMarginModel();
                model.create_by = HttpContext.User.Identity.Name;
                model.counter_party_id = tmpData.counter_party_id;
                model.call_date = tmpData.call_date;
                model.cur = tmpData.cur;
                model.payment_method = tmpData.payment_method;
                model.mt_code = tmpData.mt_code;
                model.event_type = "Margin";
                model.brp_contract_no = tmpData.brp_contract_no;
                model.eom_int_flag = "Y";

                api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageByInterestMargin(model, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception("ReleaseMessage : " + result.Message);
                }

                api_RPReleaseMsg.RPReleaseMessage.RPReleaseMessageCallMargin(model, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception("GetReleaseMessage : " + result.Message);
                }
            }
            catch (Exception Ex)
            {
                result.Message = Ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index(string search)
        {
            var RPReleaseMessageView = new RPDealEntryViewModel();
            RPTransModel model = new RPTransModel();

            if (search == "callback")
            {
                if (TempData["Search-RPReleaseMessage"] != null)
                {
                    model = (RPTransModel)TempData["Search-RPReleaseMessage"];
                }
            }
            else if (search == "pti")
            {
                return RedirectToAction("index", "RPReleaseMessagePti", new { search = "callback" });
            }
            else if (search == "Maturity")
            {
                model.event_type = "Maturity";
            }
            else if (search == "Net-Settlement")
            {
                model.event_type = "Net-Settlement";
            }
            else
            {
                model.event_type = "Settlement";
            }

            RPReleaseMessageView.FormSearch = model;

            return View(RPReleaseMessageView);
        }

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult IndexNetSettlement(string search)
        {
            var RPReleaseMessageView = new RPDealEntryViewModel();
            RPTransModel model = new RPTransModel();

            if (search == "callback")
            {
                if (TempData["Search-RPReleaseMessage"] != null)
                {
                    model = (RPTransModel)TempData["Search-RPReleaseMessage"];
                }
            }

            model.event_type = "Net-Settlement";

            RPReleaseMessageView.FormSearch = model;

            return View(RPReleaseMessageView);
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            ResultWithModel<RPTransResult> result = new ResultWithModel<RPTransResult>();
            RPTransModel RPTransModel = new RPTransModel();
            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                RPTransModel.paging = paging;

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
                RPTransModel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null).ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "from_trans_no":
                            RPTransModel.from_trans_no = column.search.value;
                            break;
                        case "to_trans_no":
                            RPTransModel.to_trans_no = column.search.value;
                            break;
                        case "counter_party_code":
                            RPTransModel.counter_party_code = column.search.value;
                            break;
                        case "counter_party_name":
                            RPTransModel.counter_party_name = column.search.value;
                            break;
                        case "from_trade_date":
                            RPTransModel.from_trade_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_trade_date":
                            RPTransModel.to_trade_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "from_settlement_date":
                            RPTransModel.from_settlement_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_settlement_date":
                            RPTransModel.to_settlement_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "from_maturity_date":
                            RPTransModel.from_maturity_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_maturity_date":
                            RPTransModel.to_maturity_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "swift_channel":
                            RPTransModel.swift_channel = column.search.value;
                            break;
                        case "cur":
                            RPTransModel.cur = column.search.value;
                            break;
                        case "event_type":
                            RPTransModel.event_type = column.search.value;
                            break;
                        case "payment_method":
                            RPTransModel.payment_method = column.search.value;
                            break;
                        case "trans_mt_code":
                            RPTransModel.trans_mt_code = column.search.value;
                            break;
                    }
                });

                RPTransModel.payment_method = "SWIFT";

                apiRPReleaseMsg.RPReleaseMessage.GetRPReleaseMessageList(RPTransModel, p =>
                {
                    result = p;
                });

                TempData["Search-RPReleaseMessage"] = RPTransModel;

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
                data = result.Data != null ? result.Data.RPTransResultModel : new List<RPTransModel>()
            });
        }

        public ActionResult Select(string id, string evnet = "callback")
        {
            List<string> listTrans = new List<string>();
            listTrans = JsonConvert.DeserializeObject<List<string>>(id);
            listTrans.Sort();
            Session["ListTrans"] = listTrans;
            Session["evnet"] = evnet;
            return RedirectToAction("Release", new { trans_no = listTrans[0], event_type = evnet });
        }

        [HttpPost]
        public ActionResult SearchNetSettle(DataTableAjaxPostModel model)
        {
            ResultWithModel<RPTransResult> result = new ResultWithModel<RPTransResult>();
            RPTransModel RPTransModel = new RPTransModel();
            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                RPTransModel.paging = paging;

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
                RPTransModel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null).ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "from_trans_no":
                            RPTransModel.from_trans_no = column.search.value;
                            break;
                        case "to_trans_no":
                            RPTransModel.to_trans_no = column.search.value;
                            break;
                        case "counter_party_code":
                            RPTransModel.counter_party_code = column.search.value;
                            break;
                        case "counter_party_name":
                            RPTransModel.counter_party_name = column.search.value;
                            break;
                        case "from_trade_date":
                            RPTransModel.from_trade_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_trade_date":
                            RPTransModel.to_trade_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "from_settlement_date":
                            RPTransModel.from_settlement_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_settlement_date":
                            RPTransModel.to_settlement_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "from_maturity_date":
                            RPTransModel.from_maturity_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_maturity_date":
                            RPTransModel.to_maturity_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                    }
                });

                apiRPReleaseMsg.RPReleaseMessage.GetRPReleaseMessageListNetSettle(RPTransModel, p =>
                {
                    result = p;
                });

                TempData["Search-RPReleaseMessage"] = RPTransModel;

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
                data = result.Data != null ? result.Data.RPTransResultModel : new List<RPTransModel>()
            });
        }

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Release(string trans_no, string event_type)
        {
            string StrMsg = string.Empty;
            ResultWithModel<RPTransResult> result = new ResultWithModel<RPTransResult>();
            RPTransModel model = new RPTransModel();

            try
            {
                //Step 0 : Check trans_no
                if (string.IsNullOrEmpty(trans_no) == true)
                {
                    return RedirectToAction("Index", new { search = "search" });
                }

                //Step 1 : Select Detail From [TransNo]
                //Add Paging
                PagingModel paging = new PagingModel();
                model.paging = paging;

                //Add Orderby
                var orders = new List<OrderByModel>();
                model.ordersby = orders;

                //Add trans_no
                model.trans_no = trans_no;

                apiRPReleaseMsg.RPReleaseMessage.GetRPReleaseMessageDetail(model, p =>
                {
                    result = p;
                });

                model = result.Data.RPTransResultModel[0];
                model.event_type = event_type;

                //Step 2 : Set Enable Btn_PreviousNext
                if (Enable_BtnPreviousNext(ref StrMsg, ref model) == false)
                {
                    throw new Exception("Enable_BtnPreviousNext() => " + StrMsg);
                }

                ViewBag.eventType = event_type;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Search_Colateral(DataTableAjaxPostModel model, string trans_no)
        {
            ResultWithModel<RPTransColateralResult> result = new ResultWithModel<RPTransColateralResult>();
            RPTransModel RPTransModel = new RPTransModel();

            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                RPTransModel.paging = paging;

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
                RPTransModel.ordersby = orders;

                RPTransModel.trans_no = trans_no;

                apiRPReleaseMsg.RPReleaseMessage.GetRPDealSettlementColateralList(RPTransModel, p =>
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
                data = result.Data != null ? result.Data.RPTransColateralResultModel : new List<RPTransColateralModel>()
            });
        }

        public bool Enable_BtnPreviousNext(ref string ReturnMsg, ref RPTransModel RPTransModel)
        {
            try
            {
                if (Session["ListTrans"] != null && Session["ListTrans"].ToString() != string.Empty)
                {
                    List<string> ListTrans = new List<string>();
                    ListTrans = (List<string>)Session["ListTrans"];

                    int IndexTransNo = 0;

                    for (int i = 0; i < ListTrans.Count; i++)
                    {
                        if (ListTrans[i].Contains(RPTransModel.trans_no) == true)
                        {
                            IndexTransNo = i;
                            break;
                        }
                    }

                    if (IndexTransNo == 0)
                    {
                        RPTransModel.btn_Previous = false;
                    }
                    else
                    {
                        RPTransModel.btn_Previous = true;
                    }

                    if (IndexTransNo == ListTrans.Count - 1)
                    {
                        RPTransModel.btn_Next = false;
                    }
                    else
                    {
                        RPTransModel.btn_Next = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }
            return true;
        }

        public ActionResult Previous(string id)
        {
            string Previous_TransNo = string.Empty;
            List<string> ListTrans = new List<string>();

            //Step 1 : Check List [TransNo] To Approve
            if (Session["ListTrans"] == null || Session["ListTrans"].ToString() == string.Empty)
            {
                return RedirectToAction("Index", new { search = "search" });
            }

            ListTrans = (List<string>)Session["ListTrans"];

            int Count_ListTransNo = ListTrans.Count;

            for (int i = 0; i < ListTrans.Count; i++)
            {
                if (ListTrans[i].Contains(id) == true)
                {
                    Previous_TransNo = ListTrans[i - 1];
                    break;
                }
            }

            return RedirectToAction("Release", new { trans_no = Previous_TransNo, event_type = Session["evnet"].ToString() });
        }

        public ActionResult Next(string id)
        {
            string Next_TransNo = string.Empty;
            List<string> ListTrans = new List<string>();

            //Step 1 : Check List [TransNo] To Approve
            if (Session["ListTrans"] == null || Session["ListTrans"].ToString() == string.Empty)
            {
                return RedirectToAction("Index", new { search = "search" });
            }

            ListTrans = (List<string>)Session["ListTrans"];
            for (int i = 0; i < ListTrans.Count; i++)
            {
                if (ListTrans[i].Contains(id) == true)
                {
                    Next_TransNo = ListTrans[i + 1];
                    break;
                }
            }

            return RedirectToAction("Release", new { trans_no = Next_TransNo, event_type = Session["evnet"].ToString() });
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult CheckSettlementStatus(RPTransModel model)
        {
            var Result = new List<object>();
            ResultWithModel<CheckSettlementStatusResponseModel> result = new ResultWithModel<CheckSettlementStatusResponseModel>();

            try
            {
                Log.WriteLog(Controller, "Start CheckSettlementStatus ==========");
                Log.WriteLog(Controller, "- trans_no = " + model.trans_no);
                Log.WriteLog(Controller, "- payment_method = " + model.payment_method);
                Log.WriteLog(Controller, "- trans_mt_code = " + model.trans_mt_code);
                Log.WriteLog(Controller, "- cur = " + model.cur);

                //Step 1 : Init Model
                CheckSettlementStatusRequestModel data = new CheckSettlementStatusRequestModel();
                data.Header = new RequestHeader
                {
                    SystemCode = "REPO",
                    AuthorityCode = "REPO",
                    RefCode = DateTime.Now.ToString("yyyyMMddTHHmmssfff"),
                    RequestDate = DateTime.Now.ToString("yyyyMMdd"),
                    RequestTime = DateTime.Now.ToString("HH:mm:ss"),
                    WSMode = 4
                };

                RequestDetail request = new RequestDetail
                {
                    Seq = 1,
                    SourceSystem = "REPO",
                    DealNo = model.trans_no,
                    DealType = "TRANS",
                    Product = string.Empty,
                    ProductGroup = string.Empty,
                    Ccy = model.cur,
                    MTType = model.trans_mt_code.Replace("MT", ""),
                    ValueDate = Session["evnet"] != null && Session["evnet"].ToString().ToUpper() == "SETTLEMENT" ? model.settlement_date.Value.ToString("yyyyMMdd") : model.maturity_date.Value.ToString("yyyyMMdd")
                };

                data.SettlementInfo = new List<RequestDetail>();
                data.SettlementInfo.Add(request);

                apiEx.InterfaceCyberPay.CheckSettlementStatus(data, p =>
                {
                    result = p;
                });

                if (result.Success)
                {
                    if (result.Data != null && result.Data.Header != null && result.Data.Header.ResponseId == 0)
                    {
                        if (result.Data.ResponseBody == null || result.Data.ResponseBody.Count <= 0)
                        {
                            //throw new Exception("ResponseBody No Data");
                            Log.WriteLog(Controller, "ResponseBody No Data");
                        }
                        else if (result.Data.ResponseBody[0].ReturnId != 0)
                        {
                            Log.WriteLog(Controller, "result.Data.ResponseBody[0].ReturnId = " + result.Data.ResponseBody[0].ReturnId);
                            Log.WriteLog(Controller, "result.Data.ResponseBody[0].ReturnMessage = " + result.Data.ResponseBody[0].ReturnMessage);
                            //throw new Exception(result.Data.ResponseBody[0].ReturnMessage);
                        }
                        else if (result.Data.ResponseBody[0].SettlementStatus == (int)SettlementStatus.Complete)
                        {
                            RPReleaseCyberPayModel cbpModel = new RPReleaseCyberPayModel();
                            cbpModel.DealNo = model.trans_no;
                            cbpModel.DealType = "TRANS";
                            cbpModel.MTType = model.trans_mt_code.Replace("MT", "");
                            cbpModel.ValueDate = Session["evnet"].ToString().ToUpper() == "SETTLEMENT" ? model.settlement_date.Value.ToString("yyyyMMdd")
                                                                                                        : model.maturity_date.Value.ToString("yyyyMMdd");
                            cbpModel.counter_party_id = model.counter_party_id.ToString();
                            cbpModel.ccy = model.cur;
                            cbpModel.Seq = 1;
                            cbpModel.create_by = model.create_by;
                            cbpModel.SettlementStatus = (int)SettlementStatus.Complete;

                            ResultWithModel<RPReleaseCyberPayResult> rwmUpdate = new ResultWithModel<RPReleaseCyberPayResult>();
                            apiRPReleaseMsg.RPReleaseCyberPay.Update(cbpModel, p =>
                            {
                                rwmUpdate = p;
                            });
                        }
                    }
                    else
                    {
                        Log.WriteLog(Controller, "result.Data.Header.ResponseMessage = " + result.Data.Header.ResponseMessage);
                        //throw new Exception(result.Data.Header.ResponseMessage);
                    }
                }
                else
                {
                    Log.WriteLog(Controller, " apiEx.InterfaceCyberPay.CheckSettlementStatus : " + result.Message);
                    //throw new Exception("CheckSettlementStatus() => " + result.Message);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            finally
            {
                Log.WriteLog(Controller, "End CheckSettlementStatus ==========");
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult InsertSettlementInfo(RPTransModel model)
        {
            string StrMsg = string.Empty;
            string NextTransNo = string.Empty;
            ResultWithModel<InsertSettlementInfoResponseModel> result = new ResultWithModel<InsertSettlementInfoResponseModel>();

            try
            {
                Log.WriteLog(Controller, "Start InsertSettlementInfo ==========");
                Log.WriteLog(Controller, "- trans_no = " + model.trans_no);
                Log.WriteLog(Controller, "- payment_method = " + model.payment_method);
                Log.WriteLog(Controller, "- trans_mt_code = " + model.trans_mt_code);
                Log.WriteLog(Controller, "- cur = " + model.cur);

                RPReleaseCyberPayModel cbpModel = new RPReleaseCyberPayModel();
                cbpModel.DealNo = model.trans_no;
                cbpModel.DealType = "TRANS";
                cbpModel.MTType = model.trans_mt_code.Replace("MT", "");
                cbpModel.ValueDate = Session["evnet"].ToString().ToUpper() == "SETTLEMENT" ? model.settlement_date.Value.ToString("yyyyMMdd")
                                                                                            : model.maturity_date.Value.ToString("yyyyMMdd");
                cbpModel.counter_party_id = model.counter_party_id.ToString();
                cbpModel.ccy = model.cur;
                cbpModel.Seq = 1;
                cbpModel.create_by = model.create_by;

                ResultWithModel<RPReleaseCyberPayResult> rwmSearch = new ResultWithModel<RPReleaseCyberPayResult>();
                apiRPReleaseMsg.RPReleaseCyberPay.Search(cbpModel, p =>
                {
                    rwmSearch = p;
                });

                if (rwmSearch.Success)
                {
                    //Step 1 : Init Model
                    InsertSettlementInfoRequestModel data = new InsertSettlementInfoRequestModel();
                    data.Header = new RequestHeader
                    {
                        SystemCode = "REPO",
                        AuthorityCode = "REPO",
                        RefCode = DateTime.Now.ToString("yyyyMMddTHHmmssfff"),
                        RequestDate = DateTime.Now.ToString("yyyyMMdd"),
                        RequestTime = DateTime.Now.ToString("HH:mm:ss"),
                        WSMode = 1
                    };

                    List<RequestInsertSettlementInfo> request = JsonConvert.DeserializeObject<List<RequestInsertSettlementInfo>>(JsonConvert.SerializeObject(rwmSearch.Data.RPReleaseCyberPayResultModel));

                    data.SettlementInfo = new List<RequestInsertSettlementInfo>();
                    data.SettlementInfo.AddRange(request);

                    apiEx.InterfaceCyberPay.InsertSettlementInfo(data, p =>
                    {
                        result = p;
                    });

                    if (!result.Success)
                    {
                        throw new Exception("InsertSettlementInfo => " + result.Message);
                    }

                    if (result.Data != null && result.Data.Header != null && result.Data.Header.ResponseId != -99)
                    {
                        if (result.Data.ResponseBody == null || result.Data.ResponseBody.Count <= 0)
                        {
                            throw new Exception("ResponseBody No Data");
                        }
                        else if (result.Data.ResponseBody[0].ReturnId == -99)
                        {
                            throw new Exception(result.Data.ResponseBody[0].ReturnMessage);
                        }
                    }
                    else
                    {
                        throw new Exception(result.Data.Header.ResponseMessage);
                    }
                }
                else
                {
                    throw new Exception(rwmSearch.Message);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            finally
            {
                Log.WriteLog(Controller, "End InsertSettlementInfo ==========");
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult SyncTransaction(RPTransModel model)
        {
            var Result = new List<object>();
            ResultWithModel<CheckSettlementStatusResponseModel> result = new ResultWithModel<CheckSettlementStatusResponseModel>();

            ResultWithModel<RPTransResult> res = new ResultWithModel<RPTransResult>();
            apiRPReleaseMsg.RPReleaseMessage.GetRPReleaseMessageList(model, p =>
            {
                res = p;
            });

            if (res.Data != null && res.Data.RPTransResultModel != null && res.Data.RPTransResultModel.Count > 0)
            {
                try
                {
                    int index = 1;
                    CheckSettlementStatusRequestModel data = new CheckSettlementStatusRequestModel();
                    data.Header = new RequestHeader
                    {
                        SystemCode = "REPO",
                        AuthorityCode = "REPO",
                        RefCode = DateTime.Now.ToString("yyyyMMddTHHmmssfff"),
                        RequestDate = DateTime.Now.ToString("yyyyMMdd"),
                        RequestTime = DateTime.Now.ToString("HH:mm:ss"),
                        WSMode = 4
                    };

                    data.SettlementInfo = new List<RequestDetail>();

                    foreach (var item in res.Data.RPTransResultModel)
                    {
                        RequestDetail request = new RequestDetail
                        {
                            Seq = index++,
                            SourceSystem = "REPO",
                            DealNo = item.trans_no,
                            DealType = "TRANS",
                            Product = string.Empty,
                            ProductGroup = string.Empty,
                            Ccy = item.cur,
                            MTType = item.trans_mt_code.Replace("MT", ""),
                            ValueDate = item.event_type.ToUpper() == "SETTLEMENT" ? item.settlement_date.Value.ToString("yyyyMMdd") : item.maturity_date.Value.ToString("yyyyMMdd")
                        };

                        data.SettlementInfo.Add(request);
                    }

                    apiEx.InterfaceCyberPay.CheckSettlementStatus(data, p =>
                    {
                        result = p;
                    });

                    if (!result.Success)
                    {
                        throw new Exception("GenRPReleaseMessage() => " + result.Message);
                    }

                    if (result.Data != null && result.Data.Header != null && result.Data.Header.ResponseId == 0)
                    {
                        if (result.Data.ResponseBody == null || result.Data.ResponseBody.Count <= 0)
                        {
                            throw new Exception("No Response Data.");
                        }

                        foreach (var item in result.Data.ResponseBody)
                        {
                            if (item.SettlementStatus == (int)SettlementStatus.Complete)
                            {
                                var deal = res.Data.RPTransResultModel.Find(x => x.trans_no == item.DealNo);
                                if (deal != null)
                                {

                                    RPReleaseCyberPayModel cbpModel = new RPReleaseCyberPayModel();
                                    cbpModel.DealNo = deal.trans_no;
                                    cbpModel.DealType = "TRANS";
                                    cbpModel.MTType = deal.trans_mt_code.Replace("MT", "");
                                    cbpModel.ValueDate = deal.event_type.ToUpper() == "SETTLEMENT" ? deal.settlement_date.Value.ToString("yyyyMMdd")
                                                                                                                : deal.maturity_date.Value.ToString("yyyyMMdd");
                                    cbpModel.counter_party_id = deal.counter_party_id.ToString();
                                    cbpModel.ccy = deal.cur;
                                    cbpModel.Seq = 1;
                                    cbpModel.create_by = deal.create_by;
                                    cbpModel.SettlementStatus = (int)SettlementStatus.Complete;

                                    ResultWithModel<RPReleaseCyberPayResult> rwmUpdate = new ResultWithModel<RPReleaseCyberPayResult>();
                                    apiRPReleaseMsg.RPReleaseCyberPay.Update(cbpModel, p =>
                                    {
                                        rwmUpdate = p;
                                    });
                                }

                            }
                        }

                    }
                    else
                    {
                        throw new Exception(result.Data.Header.ResponseMessage);
                    }



                }
                catch (Exception ex)
                {
                    result.Message = ex.Message;
                    result.Success = false;
                }
                finally
                {
                    Log.WriteLog(Controller, "End CheckSettlementStatus ==========");
                }
            }
            else
            {
                result.Message = "No data available for syncing.";
                result.Success = false;
            }



            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult SyncTransactionNet(RPTransModel model)
        {
            var Result = new List<object>();
            ResultWithModel<CheckSettlementStatusResponseModel> result = new ResultWithModel<CheckSettlementStatusResponseModel>();

            ResultWithModel<RPTransResult> res = new ResultWithModel<RPTransResult>();
            apiRPReleaseMsg.RPReleaseMessage.GetRPReleaseMessageListNetSettle(model, p =>
            {
                res = p;
            });

            if (res.Data != null && res.Data.RPTransResultModel != null && res.Data.RPTransResultModel.Count > 0)
            {
                try
                {
                    int index = 1;
                    CheckSettlementStatusRequestModel data = new CheckSettlementStatusRequestModel();
                    data.Header = new RequestHeader
                    {
                        SystemCode = "REPO",
                        AuthorityCode = "REPO",
                        RefCode = DateTime.Now.ToString("yyyyMMddTHHmmssfff"),
                        RequestDate = DateTime.Now.ToString("yyyyMMdd"),
                        RequestTime = DateTime.Now.ToString("HH:mm:ss"),
                        WSMode = 4
                    };

                    data.SettlementInfo = new List<RequestDetail>();

                    foreach (var item in res.Data.RPTransResultModel)
                    {
                        RequestDetail request = new RequestDetail
                        {
                            Seq = index++,
                            SourceSystem = "REPO",
                            DealNo = item.trans_no,
                            DealType = "TRANS",
                            Product = string.Empty,
                            ProductGroup = string.Empty,
                            Ccy = item.cur,
                            MTType = item.trans_mt_code.Replace("MT", ""),
                            ValueDate = item.settlement_date.Value.ToString("yyyyMMdd")
                        };

                        data.SettlementInfo.Add(request);
                    }

                    apiEx.InterfaceCyberPay.CheckSettlementStatus(data, p =>
                    {
                        result = p;
                    });

                    if (!result.Success)
                    {
                        throw new Exception("GenRPReleaseMessage() => " + result.Message);
                    }

                    if (result.Data != null && result.Data.Header != null && result.Data.Header.ResponseId == 0)
                    {
                        if (result.Data.ResponseBody == null || result.Data.ResponseBody.Count <= 0)
                        {
                            throw new Exception("No Response Data.");
                        }

                        foreach (var item in result.Data.ResponseBody)
                        {
                            if (item.SettlementStatus == (int)SettlementStatus.Complete)
                            {
                                var deal = res.Data.RPTransResultModel.Find(x => x.trans_no == item.DealNo);
                                if (deal != null)
                                {

                                    RPReleaseCyberPayModel cbpModel = new RPReleaseCyberPayModel();
                                    cbpModel.DealNo = deal.trans_no;
                                    cbpModel.DealType = "TRANS";
                                    cbpModel.MTType = deal.trans_mt_code.Replace("MT", "");
                                    cbpModel.ValueDate = deal.settlement_date.Value.ToString("yyyyMMdd");
                                    cbpModel.counter_party_id = deal.counter_party_id.ToString();
                                    cbpModel.ccy = deal.cur;
                                    cbpModel.Seq = 1;
                                    cbpModel.create_by = deal.create_by;
                                    cbpModel.SettlementStatus = (int)SettlementStatus.Complete;

                                    ResultWithModel<RPReleaseCyberPayResult> rwmUpdate = new ResultWithModel<RPReleaseCyberPayResult>();
                                    apiRPReleaseMsg.RPReleaseCyberPay.Update(cbpModel, p =>
                                    {
                                        rwmUpdate = p;
                                    });
                                }

                            }
                        }

                    }
                    else
                    {
                        throw new Exception(result.Data.Header.ResponseMessage);
                    }



                }
                catch (Exception ex)
                {
                    result.Message = ex.Message;
                    result.Success = false;
                }
                finally
                {
                    Log.WriteLog(Controller, "End CheckSettlementStatus ==========");
                }
            }
            else
            {
                result.Message = "No data available for syncing.";
                result.Success = false;
            }



            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult ReleaseMessage(RPTransModel model)
        {
            string StrMsg = string.Empty;
            string NextTransNo = string.Empty;
            var Result = new List<object>();
            bool Status = true;

            try
            {
                Log.WriteLog(Controller, "Start ReleaseMessage ==========");
                Log.WriteLog(Controller, "- trans_no = " + model.trans_no);
                Log.WriteLog(Controller, "- payment_method = " + model.payment_method);
                Log.WriteLog(Controller, "- trans_mt_code = " + model.trans_mt_code);
                Log.WriteLog(Controller, "- cur = " + model.cur);
                if (model.event_type == null && Session["evnet"] != null)
                {
                    model.event_type = Session["evnet"].ToString();
                }

                Log.WriteLog(Controller, "- event_type = " + model.event_type);


                ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();

                //Step 1 : ReleaseMessage TransNo
                model.create_by = HttpContext.User.Identity.Name;

                apiRPReleaseMsg.RPReleaseMessage.GenRPReleaseMessage(model, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception("GenRPReleaseMessage() => " + result.Message);
                }

                if (model.payment_method == "DVP/RVP")
                {
                    RPReleaseMessageModel ReleaseMsg = new RPReleaseMessageModel();
                    ReleaseMsg = result.Data.RPReleaseMessageResultModel[0];

                    //Step 2 : Remove TransNo ReleaseMessage = [Success] From ListTrans                
                    if (Remove_TransNoFromList(ref StrMsg, ref NextTransNo, model) == false)
                    {
                        throw new Exception("Remove_TransNoFromList() => " + StrMsg);
                    }

                    LogInOutModel logInOutModel = new LogInOutModel();
                    logInOutModel.module_name = "GenReleaseMessage";
                    logInOutModel.action_name = model.payment_method;
                    logInOutModel.svc_req = model.trans_mt_code;
                    logInOutModel.guid = Guid.NewGuid().ToString();
                    logInOutModel.ref_id = model.trans_no;

                    apiStatic.LogInOut.Add(logInOutModel, p => { });

                    Result.Add(new { Success = Status, Message = StrMsg, trans_no = NextTransNo, file_name = ReleaseMsg.file_name, file_content = ReleaseMsg.result });
                }
                else
                {
                    //Step 3 : Write File ReleaseMsg
                    Log.WriteLog(Controller, "Write File ReleaseMsg");
                    RPReleaseMessageModel ReleaseMsg = new RPReleaseMessageModel();
                    ReleaseMsg = result.Data.RPReleaseMessageResultModel[0];

                    FileEntity FileEnt = new FileEntity();
                    FileEnt.FileName = ReleaseMsg.file_name.Replace("/", "_");
                    FileEnt.FilePath = Server.MapPath(ReleaseMsg.file_path);
                    FileEnt.Values = ReleaseMsg.result;

                    Log.WriteLog(Controller, "- FileName = " + FileEnt.FileName);
                    Log.WriteLog(Controller, "- FilePath = " + FileEnt.FilePath);

                    WriteFile ObjWriteFile = new WriteFile();
                    if (ObjWriteFile.StreamWriter(ref FileEnt) == false)
                    {
                        throw new Exception("StreamWriter() => " + FileEnt.Msg);
                    }
                    Log.WriteLog(Controller, "Write File = Success.");

                    //Step 4 : Sftp File REPO_OUT & REPO_BACKOUT
                    Log.WriteLog(Controller, "ReleaseMsg To SFTP");
                    SftpEntity SftpOutEnt = new SftpEntity();
                    SftpEntity SftpBackOutEnt = new SftpEntity();
                    if (Search_ConfigReleaseMsg(ref StrMsg, ref SftpOutEnt, ref SftpBackOutEnt) == false)
                    {
                        throw new Exception("Search_ConfigReleaseMsg() => " + FileEnt.Msg);
                    }
                    Log.WriteLog(Controller, "Read Config = Success.");


                    RPReleaseCyberPayModel cbpModel = new RPReleaseCyberPayModel();
                    cbpModel.DealNo = model.trans_no;
                    cbpModel.DealType = "TRANS";
                    cbpModel.MTType = model.trans_mt_code.Replace("MT", "");
                    cbpModel.ValueDate = Session["evnet"].ToString().ToUpper() == "SETTLEMENT" ? model.settlement_date.Value.ToString("yyyyMMdd")
                                                                                                : model.maturity_date.Value.ToString("yyyyMMdd");
                    cbpModel.counter_party_id = model.counter_party_id.ToString();
                    cbpModel.ccy = model.cur;
                    cbpModel.Seq = 1;
                    cbpModel.create_by = model.create_by;

                    ResultWithModel<RPReleaseCyberPayResult> rwmSearch = new ResultWithModel<RPReleaseCyberPayResult>();
                    apiRPReleaseMsg.RPReleaseCyberPay.Search(cbpModel, p =>
                    {
                        rwmSearch = p;
                    });

                    if (rwmSearch.Success)
                    {
                        //Step 1 : Init Model
                        InsertSettlementInfoRequestModel data = new InsertSettlementInfoRequestModel();
                        data.Header = new RequestHeader
                        {
                            SystemCode = "REPO",
                            AuthorityCode = "REPO",
                            RefCode = DateTime.Now.ToString("yyyyMMddTHHmmssfff"),
                            RequestDate = DateTime.Now.ToString("yyyyMMdd"),
                            RequestTime = DateTime.Now.ToString("HH:mm:ss"),
                            WSMode = 1
                        };

                        List<RequestInsertSettlementInfo> request = JsonConvert.DeserializeObject<List<RequestInsertSettlementInfo>>(JsonConvert.SerializeObject(rwmSearch.Data.RPReleaseCyberPayResultModel));

                        if (request.Count > 0)
                        {
                            request[0].SettlementStatus = (int)SettlementStatus.Ack;
                        }

                        data.SettlementInfo = new List<RequestInsertSettlementInfo>();
                        data.SettlementInfo.AddRange(request);

                        ResultWithModel<InsertSettlementInfoResponseModel> resultInsert = new ResultWithModel<InsertSettlementInfoResponseModel>();

                        apiEx.InterfaceCyberPay.InsertSettlementInfo(data, p =>
                        {
                            resultInsert = p;
                        });

                        if (!resultInsert.Success)
                        {
                            Log.WriteLog(Controller, "InsertSettlementInfo => " + result.Message);
                        }

                        if (resultInsert.Data != null && resultInsert.Data.Header != null && resultInsert.Data.Header.ResponseId != -99)
                        {
                            if (resultInsert.Data.ResponseBody == null || resultInsert.Data.ResponseBody.Count <= 0)
                            {
                                Log.WriteLog(Controller, "InsertSettlementInfo => ResponseBody No Data");
                            }
                            else if (resultInsert.Data.ResponseBody[0].ReturnId == -99)
                            {
                                Log.WriteLog(Controller, "InsertSettlementInfo => " + resultInsert.Data.ResponseBody[0].ReturnMessage);
                            }
                        }
                        else
                        {
                            Log.WriteLog(Controller, "InsertSettlementInfo => " + resultInsert.Data.Header.ResponseMessage);
                        }
                    }

                    if (ReleaseMsg.Enable == "Y")
                    {
                        // Step 4.1 : Sftp REPO_OUT
                        ArrayList ListFile = new ArrayList();
                        ArrayList ListFileSuccess = new ArrayList();
                        ArrayList ListFileError = new ArrayList();

                        SftpOutEnt.LocalPath = FileEnt.FilePath;
                        ListFile.Add(FileEnt.FileName);

                        SftpUtility ObjectSftp = new SftpUtility();
                        if (!ObjectSftp.UploadSFTPList(ref StrMsg, SftpOutEnt, ListFile, ref ListFileError, ref ListFileSuccess))
                        {
                            throw new Exception("UploadSFTPList() => " + StrMsg);
                        }

                        if (ListFileError.Count > 0)
                        {
                            throw new Exception("UploadSFTPList() => " + ListFileError[0].ToString());
                        }

                        foreach (var FileSuccess in ListFileSuccess)
                        {
                            Log.WriteLog(Controller, "- SFTP " + SftpOutEnt.RemoteServerPath + " " + FileSuccess.ToString() + " Success.");
                        }

                        // Step 4.2 : Sftp REPO_BACKOUT
                        ListFile = new ArrayList();
                        ListFileSuccess = new ArrayList();
                        ListFileError = new ArrayList();

                        SftpBackOutEnt.LocalPath = FileEnt.FilePath;
                        ListFile.Add(FileEnt.FileName);

                        ObjectSftp = new SftpUtility();
                        if (!ObjectSftp.UploadSFTPList(ref StrMsg, SftpBackOutEnt, ListFile, ref ListFileError, ref ListFileSuccess))
                        {
                            throw new Exception("UploadSFTPList() => " + StrMsg);
                        }

                        if (ListFileError.Count > 0)
                        {
                            throw new Exception("UploadSFTPList() => " + ListFileError[0].ToString());
                        }

                        foreach (var FileSuccess in ListFileSuccess)
                        {
                            Log.WriteLog(Controller, "- SFTP " + SftpBackOutEnt.RemoteServerPath + " " + FileSuccess.ToString() + " Success.");
                        }
                    }

                    LogInOutModel logInOutModel = new LogInOutModel();
                    logInOutModel.module_name = "GenReleaseMessage";
                    logInOutModel.action_name = model.payment_method;
                    logInOutModel.svc_req = model.trans_mt_code;
                    logInOutModel.guid = Guid.NewGuid().ToString();
                    logInOutModel.ref_id = model.trans_no;

                    apiStatic.LogInOut.Add(logInOutModel, p => { });

                    //Step 2 : Remove TransNo ReleaseMessage = [Success] From ListTrans                
                    if (Remove_TransNoFromList(ref StrMsg, ref NextTransNo, model) == false)
                    {
                        throw new Exception("Remove_TransNoFromList() => " + StrMsg);
                    }

                    Result.Add(new { Success = Status, Message = StrMsg, trans_no = NextTransNo });
                }

            }
            catch (Exception Ex)
            {
                Status = false;
                StrMsg = Ex.Message;
                Log.WriteLog(Controller, "Error " + Ex.Message);
                Result.Add(new { Success = Status, Message = StrMsg, trans_no = NextTransNo });
            }
            finally
            {
                Log.WriteLog(Controller, "End ReleaseMessage ==========");
            }

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult ReleaseMessageCyberPay(RPTransModel model)
        {
            string StrMsg = string.Empty;
            string NextTransNo = string.Empty;
            var Result = new List<object>();
            bool Status = true;


            try
            {
                Log.WriteLog(Controller, "Start ReleaseMessage CyberPay");
                Log.WriteLog(Controller, "- trans_no = " + model.trans_no);
                Log.WriteLog(Controller, "- payment_method = " + model.payment_method);
                Log.WriteLog(Controller, "- trans_mt_code = " + model.trans_mt_code);
                Log.WriteLog(Controller, "- swift_channel = " + model.swift_channel);
                Log.WriteLog(Controller, "- cur = " + model.cur);
                if (model.event_type == null && Session["evnet"] != null)
                {
                    model.event_type = Session["evnet"].ToString();
                    Log.WriteLog(Controller, "- Session[evnet].ToString() = " + Session["evnet"].ToString());
                }

                Log.WriteLog(Controller, "- event_type = " + model.event_type);

                ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();

                //Step : GenRPReleaseMessageCyberPay TransNo
                model.create_by = HttpContext.User.Identity.Name;

                apiRPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageCyberPay(model, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception("GenRPReleaseMessage() => " + result.Message);
                }

                RPReleaseCyberPayModel cbpModel = new RPReleaseCyberPayModel();
                cbpModel.DealNo = model.trans_no;
                cbpModel.DealType = "TRANS";
                cbpModel.MTType = model.trans_mt_code.Replace("MT", "");
                cbpModel.ValueDate = model.event_type.ToUpper() == "SETTLEMENT" ? model.settlement_date.Value.ToString("yyyyMMdd")
                                                                                            : model.maturity_date.Value.ToString("yyyyMMdd");
                cbpModel.counter_party_id = model.counter_party_id.ToString();
                cbpModel.ccy = model.cur;
                cbpModel.Seq = 1;
                cbpModel.create_by = model.create_by;

                ResultWithModel<RPReleaseCyberPayResult> rwmSearch = new ResultWithModel<RPReleaseCyberPayResult>();
                ResultWithModel<InsertSettlementInfoResponseModel> resultInsert = new ResultWithModel<InsertSettlementInfoResponseModel>();

                apiRPReleaseMsg.RPReleaseCyberPay.Search(cbpModel, p =>
                {
                    rwmSearch = p;
                });

                if (rwmSearch.Success)
                {
                    Log.WriteLog(Controller, "rwmSearch = " + JsonConvert.SerializeObject(rwmSearch));

                    //Step 1 : Init Model
                    InsertSettlementInfoRequestModel data = new InsertSettlementInfoRequestModel();
                    data.Header = new RequestHeader
                    {
                        SystemCode = "REPO",
                        AuthorityCode = "REPO",
                        RefCode = DateTime.Now.ToString("yyyyMMddTHHmmssfff"),
                        RequestDate = DateTime.Now.ToString("yyyyMMdd"),
                        RequestTime = DateTime.Now.ToString("HH:mm:ss"),
                        WSMode = 1
                    };

                    List<RequestInsertSettlementInfo> request = JsonConvert.DeserializeObject<List<RequestInsertSettlementInfo>>(JsonConvert.SerializeObject(rwmSearch.Data.RPReleaseCyberPayResultModel));

                    data.SettlementInfo = new List<RequestInsertSettlementInfo>();
                    data.SettlementInfo.AddRange(request);

                    cbpModel.SettlementStatus = data.SettlementInfo[0].SettlementStatus;

                    apiEx.InterfaceCyberPay.InsertSettlementInfo(data, p =>
                    {
                        resultInsert = p;
                    });

                    if (!resultInsert.Success)
                    {
                        throw new Exception("InsertSettlementInfo => " + result.Message);
                    }

                    if (resultInsert.Data != null && resultInsert.Data.Header != null && resultInsert.Data.Header.ResponseId != -99)
                    {
                        if (resultInsert.Data.ResponseBody == null || resultInsert.Data.ResponseBody.Count <= 0)
                        {
                            throw new Exception("ResponseBody No Data");
                        }
                        else if (resultInsert.Data.ResponseBody[0].ReturnId == -99)
                        {
                            throw new Exception(resultInsert.Data.ResponseBody[0].ReturnMessage);
                        }
                    }
                    else
                    {
                        throw new Exception(resultInsert.Data.Header.ResponseMessage);
                    }
                }
                else
                {
                    throw new Exception(resultInsert.Message);
                }

                cbpModel.send_status = true;
                ResultWithModel<RPReleaseCyberPayResult> rwmUpdate = new ResultWithModel<RPReleaseCyberPayResult>();
                apiRPReleaseMsg.RPReleaseCyberPay.Update(cbpModel, p =>
                {
                    rwmUpdate = p;
                });

                LogInOutModel logInOutModel = new LogInOutModel();
                logInOutModel.module_name = "GenReleaseMessage";
                logInOutModel.action_name = model.payment_method;
                logInOutModel.svc_req = model.trans_mt_code;
                logInOutModel.guid = Guid.NewGuid().ToString();
                logInOutModel.ref_id = model.trans_no;

                apiStatic.LogInOut.Add(logInOutModel, p => { });

                //Step : Remove TransNo ReleaseMessage = [Success] From ListTrans                
                if (Remove_TransNoFromList(ref StrMsg, ref NextTransNo, model) == false)
                {
                    throw new Exception("Remove_TransNoFromList() => " + StrMsg);
                }

                Result.Add(new { Success = Status, Message = StrMsg, trans_no = NextTransNo });
            }
            catch (Exception Ex)
            {
                Status = false;
                StrMsg = Ex.Message;
                Log.WriteLog(Controller, "Error " + Ex.Message);
                Result.Add(new { Success = Status, Message = StrMsg, trans_no = NextTransNo });
            }
            finally
            {
                Log.WriteLog(Controller, "End ReleaseMessage ==========");
            }

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult ReleaseMessageNetSettle(RPTransModel model)
        {
            string StrMsg = string.Empty;
            string NextTransNo = string.Empty;
            var Result = new List<object>();
            bool Status = true;

            try
            {
                Log.WriteLog(Controller, "Start ReleaseMessageNetSettle ==========");
                Log.WriteLog(Controller, "- trans_no = " + model.trans_no);
                Log.WriteLog(Controller, "- payment_method = " + model.payment_method);
                Log.WriteLog(Controller, "- trans_mt_code = " + model.trans_mt_code);
                Log.WriteLog(Controller, "- swift_channel = " + model.swift_channel);
                Log.WriteLog(Controller, "- cur = " + model.cur);
                if (model.event_type == null && Session["evnet"] != null)
                {
                    model.event_type = Session["evnet"].ToString();
                }

                Log.WriteLog(Controller, "- event_type = " + model.event_type);


                ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();

                //Step 1 : ReleaseMessageNetSettle TransNo
                model.create_by = HttpContext.User.Identity.Name;

                apiRPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageNetSettle(model, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception("GenRPReleaseMessage() => " + result.Message);
                }



                //Step 3 : Write File ReleaseMsg
                Log.WriteLog(Controller, "Write File ReleaseMsg");
                RPReleaseMessageModel ReleaseMsg = new RPReleaseMessageModel();
                ReleaseMsg = result.Data.RPReleaseMessageResultModel[0];

                FileEntity FileEnt = new FileEntity();
                FileEnt.FileName = ReleaseMsg.file_name.Replace("/", "_");
                FileEnt.FilePath = Server.MapPath(ReleaseMsg.file_path);
                FileEnt.Values = ReleaseMsg.result;

                Log.WriteLog(Controller, "- FileName = " + FileEnt.FileName);
                Log.WriteLog(Controller, "- FilePath = " + FileEnt.FilePath);

                WriteFile ObjWriteFile = new WriteFile();
                if (ObjWriteFile.StreamWriter(ref FileEnt) == false)
                {
                    throw new Exception("StreamWriter() => " + FileEnt.Msg);
                }
                Log.WriteLog(Controller, "Write File = Success.");

                //Step 4 : Sftp File REPO_OUT & REPO_BACKOUT
                Log.WriteLog(Controller, "ReleaseMsg To SFTP");
                SftpEntity SftpOutEnt = new SftpEntity();
                SftpEntity SftpBackOutEnt = new SftpEntity();
                if (Search_ConfigReleaseMsg(ref StrMsg, ref SftpOutEnt, ref SftpBackOutEnt) == false)
                {
                    throw new Exception("Search_ConfigReleaseMsg() => " + FileEnt.Msg);
                }
                Log.WriteLog(Controller, "Read Config = Success.");

                if (ReleaseMsg.Enable == "Y")
                {
                    // Step 4.1 : Sftp REPO_OUT
                    ArrayList ListFile = new ArrayList();
                    ArrayList ListFileSuccess = new ArrayList();
                    ArrayList ListFileError = new ArrayList();

                    SftpOutEnt.LocalPath = FileEnt.FilePath;
                    ListFile.Add(FileEnt.FileName);

                    SftpUtility ObjectSftp = new SftpUtility();
                    if (!ObjectSftp.UploadSFTPList(ref StrMsg, SftpOutEnt, ListFile, ref ListFileError, ref ListFileSuccess))
                    {
                        throw new Exception("UploadSFTPList() => " + StrMsg);
                    }

                    if (ListFileError.Count > 0)
                    {
                        throw new Exception("UploadSFTPList() => " + ListFileError[0].ToString());
                    }

                    foreach (var FileSuccess in ListFileSuccess)
                    {
                        Log.WriteLog(Controller, "- SFTP " + SftpOutEnt.RemoteServerPath + " " + FileSuccess.ToString() + " Success.");
                    }

                    // Step 4.2 : Sftp REPO_BACKOUT
                    ListFile = new ArrayList();
                    ListFileSuccess = new ArrayList();
                    ListFileError = new ArrayList();

                    SftpBackOutEnt.LocalPath = FileEnt.FilePath;
                    ListFile.Add(FileEnt.FileName);

                    ObjectSftp = new SftpUtility();
                    if (!ObjectSftp.UploadSFTPList(ref StrMsg, SftpBackOutEnt, ListFile, ref ListFileError, ref ListFileSuccess))
                    {
                        throw new Exception("UploadSFTPList() => " + StrMsg);
                    }

                    if (ListFileError.Count > 0)
                    {
                        throw new Exception("UploadSFTPList() => " + ListFileError[0].ToString());
                    }

                    foreach (var FileSuccess in ListFileSuccess)
                    {
                        Log.WriteLog(Controller, "- SFTP " + SftpBackOutEnt.RemoteServerPath + " " + FileSuccess.ToString() + " Success.");
                    }
                }

                LogInOutModel logInOutModel = new LogInOutModel();
                logInOutModel.module_name = "GenReleaseMessageNetSettle";
                logInOutModel.action_name = model.payment_method;
                logInOutModel.svc_req = model.trans_mt_code;
                logInOutModel.guid = Guid.NewGuid().ToString();
                logInOutModel.ref_id = model.trans_no;

                apiStatic.LogInOut.Add(logInOutModel, p => { });

                Result.Add(new { Success = Status, Message = StrMsg, trans_no = NextTransNo });


            }
            catch (Exception Ex)
            {
                Status = false;
                StrMsg = Ex.Message;
                Log.WriteLog(Controller, "Error " + Ex.Message);
                Result.Add(new { Success = Status, Message = StrMsg, trans_no = NextTransNo });
            }
            finally
            {
                Log.WriteLog(Controller, "End ReleaseMessageNetSettle ==========");
            }

            return Json(Result[0], JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult ReleaseMessageNetSettleCyberPay(RPTransModel model)
        {
            string StrMsg = string.Empty;
            string NextTransNo = string.Empty;
            var Result = new List<object>();
            bool Status = true;


            try
            {
                Log.WriteLog(Controller, "Start ReleaseMessage CyberPay");
                Log.WriteLog(Controller, "- trans_no = " + model.trans_no);
                Log.WriteLog(Controller, "- payment_method = " + model.payment_method);
                Log.WriteLog(Controller, "- trans_mt_code = " + model.trans_mt_code);
                Log.WriteLog(Controller, "- swift_channel = " + model.swift_channel);
                Log.WriteLog(Controller, "- cur = " + model.cur);
                //if (model.event_type == null && Session["evnet"] != null)
                //{
                //    model.event_type = Session["evnet"].ToString();
                //}

                //Log.WriteLog(Controller, "- event_type = " + model.event_type);

                ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();

                //Step : GenRPReleaseMessageCyberPay TransNo
                model.create_by = HttpContext.User.Identity.Name;

                apiRPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageNetSettleCyberPay(model, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception("GenRPReleaseMessage() => " + result.Message);
                }

                RPReleaseCyberPayModel cbpModel = new RPReleaseCyberPayModel();
                cbpModel.DealNo = model.trans_no;
                cbpModel.DealType = "TRANS";
                cbpModel.MTType = model.trans_mt_code.Replace("MT", "");
                cbpModel.ValueDate =  model.settlement_date.Value.ToString("yyyyMMdd");
                cbpModel.counter_party_id = model.counter_party_id.ToString();
                cbpModel.ccy = model.cur;
                cbpModel.Seq = 1;
                cbpModel.create_by = model.create_by;

                ResultWithModel<RPReleaseCyberPayResult> rwmSearch = new ResultWithModel<RPReleaseCyberPayResult>();
                ResultWithModel<InsertSettlementInfoResponseModel> resultInsert = new ResultWithModel<InsertSettlementInfoResponseModel>();

                apiRPReleaseMsg.RPReleaseCyberPay.Search(cbpModel, p =>
                {
                    rwmSearch = p;
                });

                if (rwmSearch.Success)
                {
                    //Step 1 : Init Model
                    InsertSettlementInfoRequestModel data = new InsertSettlementInfoRequestModel();
                    data.Header = new RequestHeader
                    {
                        SystemCode = "REPO",
                        AuthorityCode = "REPO",
                        RefCode = DateTime.Now.ToString("yyyyMMddTHHmmssfff"),
                        RequestDate = DateTime.Now.ToString("yyyyMMdd"),
                        RequestTime = DateTime.Now.ToString("HH:mm:ss"),
                        WSMode = 1
                    };

                    List<RequestInsertSettlementInfo> request = JsonConvert.DeserializeObject<List<RequestInsertSettlementInfo>>(JsonConvert.SerializeObject(rwmSearch.Data.RPReleaseCyberPayResultModel));

                    data.SettlementInfo = new List<RequestInsertSettlementInfo>();
                    data.SettlementInfo.AddRange(request);

                    cbpModel.SettlementStatus = data.SettlementInfo[0].SettlementStatus;

                    apiEx.InterfaceCyberPay.InsertSettlementInfo(data, p =>
                    {
                        resultInsert = p;
                    });

                    if (!resultInsert.Success)
                    {
                        throw new Exception("InsertSettlementInfo => " + result.Message);
                    }

                    if (resultInsert.Data != null && resultInsert.Data.Header != null && resultInsert.Data.Header.ResponseId != -99)
                    {
                        if (resultInsert.Data.ResponseBody == null || resultInsert.Data.ResponseBody.Count <= 0)
                        {
                            throw new Exception("ResponseBody No Data");
                        }
                        else if (resultInsert.Data.ResponseBody[0].ReturnId == -99)
                        {
                            throw new Exception(resultInsert.Data.ResponseBody[0].ReturnMessage);
                        }
                    }
                    else
                    {
                        throw new Exception(resultInsert.Data.Header.ResponseMessage);
                    }
                }
                else
                {
                    throw new Exception(resultInsert.Message);
                }

                cbpModel.send_status = true;
                ResultWithModel<RPReleaseCyberPayResult> rwmUpdate = new ResultWithModel<RPReleaseCyberPayResult>();
                apiRPReleaseMsg.RPReleaseCyberPay.Update(cbpModel, p =>
                {
                    rwmUpdate = p;
                });

                LogInOutModel logInOutModel = new LogInOutModel();
                logInOutModel.module_name = "GenReleaseMessageNetSettle";
                logInOutModel.action_name = model.payment_method;
                logInOutModel.svc_req = model.trans_mt_code;
                logInOutModel.guid = Guid.NewGuid().ToString();
                logInOutModel.ref_id = model.trans_no;

                apiStatic.LogInOut.Add(logInOutModel, p => { });

                //Step : Remove TransNo ReleaseMessage = [Success] From ListTrans                
                if (Remove_TransNoFromList(ref StrMsg, ref NextTransNo, model) == false)
                {
                    throw new Exception("Remove_TransNoFromList() => " + StrMsg);
                }

                Result.Add(new { Success = Status, Message = StrMsg, trans_no = NextTransNo });
            }
            catch (Exception Ex)
            {
                Status = false;
                StrMsg = Ex.Message;
                Log.WriteLog(Controller, "Error " + Ex.Message);
                Result.Add(new { Success = Status, Message = StrMsg, trans_no = NextTransNo });
            }
            finally
            {
                Log.WriteLog(Controller, "End ReleaseMessage ==========");
            }

            return Json(Result[0], JsonRequestBehavior.AllowGet);
        }

        private bool Search_ConfigReleaseMsg(ref string ReturnMsg, ref SftpEntity SftpOutEnt, ref SftpEntity SftpBackOutEnt)
        {
            try
            {
                RpSftpModel SftpModel = new RpSftpModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                StaticEntities StaticEnt = new StaticEntities();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RELEASE_MSG", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception(ResultRpconfig.Message);
                }

                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                SftpOutEnt.RemoteServerName = RpConfigModel.FirstOrDefault(a => a.item_code == "IP")?.item_value;
                SftpOutEnt.RemotePort = RpConfigModel.FirstOrDefault(a => a.item_code == "PORT")?.item_value;
                SftpOutEnt.RemoteUserName = RpConfigModel.FirstOrDefault(a => a.item_code == "USER")?.item_value;
                SftpOutEnt.RemotePassword = RpConfigModel.FirstOrDefault(a => a.item_code == "PASSWORD")?.item_value;
                SftpOutEnt.RemoteServerPath = RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SFTP_REPO_OUT")?.item_value;
                SftpOutEnt.RemoteSshHostKeyFingerprint = RpConfigModel.FirstOrDefault(a => a.item_code == "SSHHOSTKEYFINGERPRINT")?.item_value;
                SftpOutEnt.RemoteSshPrivateKeyPath = RpConfigModel.FirstOrDefault(a => a.item_code == "SSHPRIVATEKEYPATH")?.item_value;
                SftpOutEnt.NoOfFailRetry = System.Convert.ToInt32(RpConfigModel.FirstOrDefault(a => a.item_code == "FAIL_RETRY")?.item_value);

                Log.WriteLog(Controller, "SFTP OutEnt");
                Log.WriteLog(Controller, "RemoteServerName = " + SftpOutEnt.RemoteServerName);
                Log.WriteLog(Controller, "RemotePort = " + SftpOutEnt.RemotePort);
                Log.WriteLog(Controller, "RemoteServerPath = " + SftpOutEnt.RemoteServerPath);
                Log.WriteLog(Controller, "RemoteSshHostKeyFingerprint = " + SftpOutEnt.RemoteSshHostKeyFingerprint);
                Log.WriteLog(Controller, "RemoteSshPrivateKeyPath = " + SftpOutEnt.RemoteSshPrivateKeyPath);
                Log.WriteLog(Controller, "NoOfFailRetry File = " + SftpOutEnt.NoOfFailRetry);

                SftpBackOutEnt.RemoteServerName = RpConfigModel.FirstOrDefault(a => a.item_code == "IP")?.item_value;
                SftpBackOutEnt.RemotePort = RpConfigModel.FirstOrDefault(a => a.item_code == "PORT")?.item_value;
                SftpBackOutEnt.RemoteUserName = RpConfigModel.FirstOrDefault(a => a.item_code == "USER")?.item_value;
                SftpBackOutEnt.RemotePassword = RpConfigModel.FirstOrDefault(a => a.item_code == "PASSWORD")?.item_value;
                SftpBackOutEnt.RemoteServerPath = RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SFTP_REPO_BACKOUT")?.item_value;
                SftpBackOutEnt.RemoteSshHostKeyFingerprint = RpConfigModel.FirstOrDefault(a => a.item_code == "SSHHOSTKEYFINGERPRINT")?.item_value;
                SftpBackOutEnt.RemoteSshPrivateKeyPath = RpConfigModel.FirstOrDefault(a => a.item_code == "SSHPRIVATEKEYPATH")?.item_value;
                SftpBackOutEnt.NoOfFailRetry = System.Convert.ToInt32(RpConfigModel.FirstOrDefault(a => a.item_code == "FAIL_RETRY")?.item_value);

                Log.WriteLog(Controller, "SFTP BackOutEnt");
                Log.WriteLog(Controller, "RemoteServerName = " + SftpBackOutEnt.RemoteServerName);
                Log.WriteLog(Controller, "RemotePort = " + SftpBackOutEnt.RemotePort);
                Log.WriteLog(Controller, "RemoteServerPath = " + SftpBackOutEnt.RemoteServerPath);
                Log.WriteLog(Controller, "RemoteSshHostKeyFingerprint = " + SftpBackOutEnt.RemoteSshHostKeyFingerprint);
                Log.WriteLog(Controller, "RemoteSshPrivateKeyPath = " + SftpBackOutEnt.RemoteSshPrivateKeyPath);
                Log.WriteLog(Controller, "NoOfFailRetry File = " + SftpBackOutEnt.NoOfFailRetry);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        public bool Remove_TransNoFromList(ref string ReturnMsg, ref string NextTransNo, RPTransModel RPTransModel)
        {
            try
            {
                int NextIndex;
                int TargetIndex = -999;
                List<string> ListTrans = new List<string>();
                if (Session["ListTrans"] != null && Session["ListTrans"].ToString() != string.Empty)
                {
                    ListTrans = (List<string>)Session["ListTrans"];

                    //Step 1 : For Loop Find TargetIndex To Delete
                    for (int i = 0; i < ListTrans.Count; i++)
                    {
                        if (ListTrans[i].Contains(RPTransModel.trans_no) == true)
                        {
                            TargetIndex = i; //Keep Index To Delete
                            NextIndex = i + 1; //Keep Next Index To Approve
                            if (NextIndex < ListTrans.Count)
                            {
                                NextTransNo = ListTrans[NextIndex];
                            }
                            else
                            {
                                if (ListTrans.Count > 1)
                                {
                                    NextTransNo = ListTrans[0];
                                }
                            }
                            break;
                        }
                    }

                    //Step 2 : Delete TransNo From TargetIndex
                    if (TargetIndex != -999)
                    {
                        ListTrans.RemoveAt(TargetIndex);
                    }

                    //Step 3 : Delete TransNo From TargetIndex
                    if (ListTrans.Count == 0)
                    {
                        Session["ListTrans"] = string.Empty;
                    }
                    else
                    {
                        Session["ListTrans"] = ListTrans;
                    }
                }
                else
                {
                    Session["ListTrans"] = string.Empty;
                }
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        // Action : Index Coupon
        public ActionResult IndexCoupon()
        {
            var RPCouponView = new RPCouponViewModel();
            RPCouponModel RPCoupon = new RPCouponModel();

            RPCoupon.event_type = "Coupon";
            RPCouponView.FormSearch = RPCoupon;

            return View("IndexCoupon", RPCouponView);
        }
        public ActionResult IndexCallMargin()
        {
            var RPCallMarginView = new RPCallMarginViewModel();
            RPCallMarginModel rPCallMargin = new RPCallMarginModel();

            rPCallMargin.event_type = "Margin";
            RPCallMarginView.FormSearch = rPCallMargin;

            return View("IndexCallMargin", RPCallMarginView);
        }

        public ActionResult IndexInterestMargin()
        {
            var RPCallMarginView = new RPCallMarginViewModel();
            RPCallMarginModel rPCallMargin = new RPCallMarginModel();

            rPCallMargin.event_type = "Interest Margin";
            RPCallMarginView.FormSearch = rPCallMargin;

            return View("IndexInterestMargin", RPCallMarginView);
        }

        [HttpPost]
        public ActionResult SearchCoupon(DataTableAjaxPostModel model)
        {
            ResultWithModel<RPCouponResult> result = new ResultWithModel<RPCouponResult>();
            RPCouponModel rpCouponModel = new RPCouponModel();
            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                rpCouponModel.paging = paging;

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

                rpCouponModel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null).ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "instrument_code":
                            rpCouponModel.instrument_code = column.search.value;
                            break;
                        case "instrument_id":
                            if (column.search.value != "")
                            {
                                rpCouponModel.instrument_id = System.Convert.ToInt32(column.search.value);
                            }
                            else
                            {
                                rpCouponModel.instrument_id = null;
                            }
                            break;
                        case "counter_party_code":
                            rpCouponModel.counter_party_code = column.search.value;
                            break;
                        case "counter_party_id":
                            if (column.search.value != "")
                            {
                                rpCouponModel.counter_party_id = System.Convert.ToInt32(column.search.value);
                            }
                            else
                            {
                                rpCouponModel.counter_party_id = null;
                            }
                            break;
                        case "fund_code":
                            rpCouponModel.fund_code = column.search.value;
                            break;
                        case "payment_date":
                            rpCouponModel.payment_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                    }
                });

                StaticEntities db = new StaticEntities();
                ResultWithModel<BusinessDateResult> rwm = new ResultWithModel<BusinessDateResult>();
                BusinessDateModel BusinessDateModel = new BusinessDateModel();
                PagingModel pagingBusDate = new PagingModel();
                pagingBusDate.PageNumber = 1;
                pagingBusDate.RecordPerPage = 20;
                BusinessDateModel.paging = pagingBusDate;
                if (rpCouponModel.payment_date == null)
                {
                    db.BusinessDate.GetBusinessDateList(BusinessDateModel, p =>
                    {
                        rwm = p;
                    });

                    if (rwm.Success)
                    {
                        rpCouponModel.payment_date = rwm.Data.BusinessDateResultModel[0].business_date;
                    }
                }

                apiRPReleaseMsg.RPReleaseMessage.GetRPCouponList(rpCouponModel, p =>
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
                data = result.Data != null ? result.Data.RPCouponResultModel : null
            });
        }

        [HttpPost]
        public ActionResult SearchCallMargin(DataTableAjaxPostModel model)
        {
            ResultWithModel<RPCallMarginResult> Result = new ResultWithModel<RPCallMarginResult>();
            RPCallMarginModel rpCallMarginModel = new RPCallMarginModel();

            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                rpCallMarginModel.paging = paging;

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
                rpCallMarginModel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null).ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "from_as_of_date":
                            rpCallMarginModel.from_as_of_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_as_of_date":
                            rpCallMarginModel.to_as_of_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "from_call_date":
                            rpCallMarginModel.from_call_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_call_date":
                            rpCallMarginModel.to_call_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "cur":
                            rpCallMarginModel.cur = column.search.value;
                            break;
                        case "counter_party_id":
                            if (column.search.value != "")
                            {
                                rpCallMarginModel.counter_party_id = System.Convert.ToInt32(column.search.value);
                            }
                            else
                            {
                                rpCallMarginModel.counter_party_id = null;
                            }
                            break;
                    }
                });

                apiRPReleaseMsg.RPCallMargin.RPCallMarginListReleaseMessage(rpCallMarginModel, p =>
                {
                    Result = p;
                });

            }
            catch (Exception ex)
            {
                Result.Message = ex.Message;
            }
            return Json(new
            {
                draw = model.draw,
                recordsTotal = Result.HowManyRecord,
                recordsFiltered = Result.HowManyRecord,
                Message = Result.Message,
                data = Result.Data != null ? Result.Data.RPCallMarginResultModel : new List<RPCallMarginModel>()
            });
        }

        [HttpPost]
        public ActionResult SearchInterestMargin(DataTableAjaxPostModel model)
        {
            ResultWithModel<RPCallMarginResult> Result = new ResultWithModel<RPCallMarginResult>();
            RPCallMarginModel rpCallMarginModel = new RPCallMarginModel();

            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                rpCallMarginModel.paging = paging;

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

                rpCallMarginModel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null).ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "payment_date_from":
                            rpCallMarginModel.payment_date_from = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "payment_date_to":
                            rpCallMarginModel.payment_date_to = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "from_as_of_date":
                            rpCallMarginModel.from_as_of_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_as_of_date":
                            rpCallMarginModel.to_as_of_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "from_call_date":
                            rpCallMarginModel.from_call_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_call_date":
                            rpCallMarginModel.to_call_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "cur":
                            rpCallMarginModel.cur = column.search.value;
                            break;
                        case "counter_party_id":
                            if (column.search.value != "")
                            {
                                rpCallMarginModel.counter_party_id = System.Convert.ToInt32(column.search.value);
                            }
                            else
                            {
                                rpCallMarginModel.counter_party_id = null;
                            }
                            break;
                    }
                });

                apiRPReleaseMsg.RPCallMargin.RPInterestMarginListReleaseMessage(rpCallMarginModel, p =>
                {
                    Result = p;
                });

            }
            catch (Exception ex)
            {
                Result.Message = ex.Message;
            }
            return Json(new
            {
                draw = model.draw,
                recordsTotal = Result.HowManyRecord,
                recordsFiltered = Result.HowManyRecord,
                Message = Result.Message,
                data = Result.Data != null ? Result.Data.RPCallMarginResultModel : new List<RPCallMarginModel>()
            });
        }

        public ActionResult SelectCoupon(string id)
        {
            List<string> ListCoupon = new List<string>();
            ListCoupon = JsonConvert.DeserializeObject<List<string>>(id);
            ListCoupon.Sort();
            Session["ListCoupon"] = ListCoupon;

            return RedirectToAction("ReleaseCoupon", new { trans_cno = ListCoupon[0] });
        }

        // Action : Release Coupon
        public ActionResult ReleaseCoupon(string trans_cno)
        {
            string StrMsg = string.Empty;
            DataTableAjaxPostModel model = new DataTableAjaxPostModel();
            ResultWithModel<RPCouponDetailResult> resultDetail = new ResultWithModel<RPCouponDetailResult>();
            ResultWithModel<RPCouponResult> Result = new ResultWithModel<RPCouponResult>();
            RPCouponModel rpCouponModel = new RPCouponModel();


            try
            {
                //Step 0 : Check trans_cno
                if (string.IsNullOrEmpty(trans_cno) == true)
                {
                    return RedirectToAction("IndexCoupon");
                }

                //Step 1 : Select Detail From [TranscNo]
                //Add Paging
                PagingModel paging = new PagingModel();
                rpCouponModel.paging = paging;

                //Add Orderby
                var orders = new List<OrderByModel>();
                rpCouponModel.ordersby = orders;

                //Add trans_no
                rpCouponModel.trans_cno = trans_cno;

                apiRPReleaseMsg.RPReleaseMessage.GetRPCouponGet(rpCouponModel, p =>
                {
                    Result = p;
                });

                if (!Result.Success)
                {
                    throw new Exception("GetRPCouponGet() => " + Result.Message);
                }

                apiRPReleaseMsg.RPReleaseMessage.GetRPCouponDetail(rpCouponModel, p =>
                {
                    resultDetail = p;
                });

                if (!resultDetail.Success)
                {
                    throw new Exception("GetRPCouponDetail() => " + resultDetail.Message);
                }

                rpCouponModel = Result.Data.RPCouponResultModel[0];
                rpCouponModel.Port_AFS = resultDetail.Data.RPCouponDetailResultModel != null &&
               resultDetail.Data.RPCouponDetailResultModel.Count > 0 && resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "AFS").FirstOrDefault() != null
               ? resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "AFS").FirstOrDefault() : new RPCouponDetailModel();

                rpCouponModel.Port_HTM = resultDetail.Data.RPCouponDetailResultModel != null &&
                    resultDetail.Data.RPCouponDetailResultModel.Count > 0 && resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "HTM").FirstOrDefault() != null
                    ? resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "HTM").FirstOrDefault() : new RPCouponDetailModel();

                rpCouponModel.Port_TRD = resultDetail.Data.RPCouponDetailResultModel != null &&
                    resultDetail.Data.RPCouponDetailResultModel.Count > 0 && resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "TRD").FirstOrDefault() != null
                    ? resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "TRD").FirstOrDefault() : new RPCouponDetailModel();

                rpCouponModel.Port_MEMO_BNK = resultDetail.Data.RPCouponDetailResultModel != null &&
                     resultDetail.Data.RPCouponDetailResultModel.Count > 0 && resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "MEMO-BNK").FirstOrDefault() != null
                     ? resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "MEMO-BNK").FirstOrDefault() : new RPCouponDetailModel();

                rpCouponModel.Port_MEMO_TRD = resultDetail.Data.RPCouponDetailResultModel != null &&
                   resultDetail.Data.RPCouponDetailResultModel.Count > 0 && resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "MEMO-TRD").FirstOrDefault() != null
                   ? resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "MEMO-TRD").FirstOrDefault() : new RPCouponDetailModel();

                //Step 2 : Set Enable Btn_PreviousNext
                if (Enable_BtnPreviousNextCoupon(ref StrMsg, ref rpCouponModel) == false)
                {
                    throw new Exception("Enable_BtnPreviousNextCoupon() => " + StrMsg);
                }
            }
            catch (Exception Ex)
            {
                resultDetail.Message = Ex.Message;
            }

            return View(rpCouponModel);
        }

        public bool Enable_BtnPreviousNextCoupon(ref string ReturnMsg, ref RPCouponModel RPCouponModel)
        {
            try
            {
                List<string> ListCoupon = new List<string>();
                if (Session["ListCoupon"] != null && Session["ListCoupon"].ToString() != string.Empty)
                {
                    ListCoupon = (List<string>)Session["ListCoupon"];
                }

                int IndexTransNo = 0;

                for (int i = 0; i < ListCoupon.Count; i++)
                {
                    if (ListCoupon[i].Contains(RPCouponModel.trans_cno) == true)
                    {
                        IndexTransNo = i;
                        break;
                    }
                }

                if (IndexTransNo == 0)
                {
                    RPCouponModel.btn_Previous = false;
                }
                else
                {
                    RPCouponModel.btn_Previous = true;
                }

                if (IndexTransNo == ListCoupon.Count - 1)
                {
                    RPCouponModel.btn_Next = false;
                }
                else
                {
                    RPCouponModel.btn_Next = true;
                }
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }
            return true;
        }

        public ActionResult PreviousCoupon(string id)
        {
            string Previous_TranscNo = string.Empty;
            List<string> ListCoupon = new List<string>();

            //Step 1 : Check List [TranscNo] To ReleaseMsg
            if (Session["ListCoupon"] == null || Session["ListCoupon"].ToString() == string.Empty)
            {
                return RedirectToAction("IndexCoupon");
            }

            ListCoupon = (List<string>)Session["ListCoupon"];

            int Count_ListTransNo = ListCoupon.Count;

            for (int i = 0; i < ListCoupon.Count; i++)
            {
                if (ListCoupon[i].Contains(id) == true)
                {
                    Previous_TranscNo = ListCoupon[i - 1];
                    break;
                }
            }

            return RedirectToAction("ReleaseCoupon", new { trans_cno = Previous_TranscNo });
        }

        public ActionResult NextCoupon(string id)
        {
            string Next_TranscNo = string.Empty;
            List<string> ListCoupon = new List<string>();

            //Step 1 : Check List [TranscNo] To ReleaseMsg
            if (Session["ListCoupon"] == null || Session["ListCoupon"].ToString() == string.Empty)
            {
                return RedirectToAction("IndexCoupon");
            }

            ListCoupon = (List<string>)Session["ListCoupon"];

            for (int i = 0; i < ListCoupon.Count; i++)
            {
                if (ListCoupon[i].Contains(id) == true)
                {
                    Next_TranscNo = ListCoupon[i + 1];
                    break;
                }
            }

            return RedirectToAction("ReleaseCoupon", new { trans_cno = Next_TranscNo });
        }

        [HttpPost]
        public ActionResult ReleaseMessageCoupon(RPCouponModel model)
        {
            string StrMsg = string.Empty;
            string NextTransNo = string.Empty;
            var Result = new List<object>();

            try
            {
                ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();

                //Step 1 : ReleaseMessage TransNo
                model.create_by = HttpContext.User.Identity.Name;
                model.event_type = "Coupon";

                apiRPReleaseMsg.RPCoupon.GenRPReleaseMessage(model, p =>
                {
                    result = p;

                });

                if (result.Success)
                {

                    //Step 3 : Write File ReleaseMsg
                    Log.WriteLog(Controller, "Write File ReleaseMsg");
                    RPReleaseMessageModel ReleaseMsg = new RPReleaseMessageModel();
                    ReleaseMsg = result.Data.RPReleaseMessageResultModel[0];

                    FileEntity FileEnt = new FileEntity();
                    FileEnt.FileName = ReleaseMsg.file_name.Replace("/", "_");
                    FileEnt.FilePath = Server.MapPath(ReleaseMsg.file_path);
                    FileEnt.Values = ReleaseMsg.result;

                    Log.WriteLog(Controller, "- FileName = " + FileEnt.FileName);
                    Log.WriteLog(Controller, "- FilePath = " + FileEnt.FilePath);

                    WriteFile ObjWriteFile = new WriteFile();
                    if (ObjWriteFile.StreamWriter(ref FileEnt) == false)
                    {
                        throw new Exception("StreamWriter() => " + FileEnt.Msg);
                    }
                    Log.WriteLog(Controller, "Write File = Success.");

                    //Step 4 : Sftp File REPO_OUT & REPO_BACKOUT
                    Log.WriteLog(Controller, "ReleaseMsg To SFTP");
                    SftpEntity SftpOutEnt = new SftpEntity();
                    SftpEntity SftpBackOutEnt = new SftpEntity();
                    if (Search_ConfigReleaseMsg(ref StrMsg, ref SftpOutEnt, ref SftpBackOutEnt) == false)
                    {
                        throw new Exception("Search_ConfigReleaseMsg() => " + FileEnt.Msg);
                    }
                    Log.WriteLog(Controller, "Read Config = Success.");

                    if (ReleaseMsg.Enable == "Y")
                    {
                        // Step 4.1 : Sftp REPO_OUT
                        ArrayList ListFile = new ArrayList();
                        ArrayList ListFileSuccess = new ArrayList();
                        ArrayList ListFileError = new ArrayList();

                        SftpOutEnt.LocalPath = FileEnt.FilePath;
                        ListFile.Add(FileEnt.FileName);

                        SftpUtility ObjectSftp = new SftpUtility();
                        if (!ObjectSftp.UploadSFTPList(ref StrMsg, SftpOutEnt, ListFile, ref ListFileError, ref ListFileSuccess))
                        {
                            throw new Exception("UploadSFTPList() => " + StrMsg);
                        }

                        if (ListFileError.Count > 0)
                        {
                            throw new Exception("UploadSFTPList() => " + ListFileError[0].ToString());
                        }

                        foreach (var FileSuccess in ListFileSuccess)
                        {
                            Log.WriteLog(Controller, "- SFTP Success " + FileSuccess.ToString());
                        }

                        // Step 4.2 : Sftp REPO_BACKOUT
                        ListFile = new ArrayList();
                        ListFileSuccess = new ArrayList();
                        ListFileError = new ArrayList();

                        SftpBackOutEnt.LocalPath = FileEnt.FilePath;
                        ListFile.Add(FileEnt.FileName);

                        ObjectSftp = new SftpUtility();
                        if (!ObjectSftp.UploadSFTPList(ref StrMsg, SftpBackOutEnt, ListFile, ref ListFileError, ref ListFileSuccess))
                        {
                            throw new Exception("UploadSFTPList() => " + StrMsg);
                        }

                        if (ListFileError.Count > 0)
                        {
                            throw new Exception("UploadSFTPList() => " + ListFileError[0].ToString());
                        }

                        foreach (var FileSuccess in ListFileSuccess)
                        {
                            Log.WriteLog(Controller, "- SFTP Success " + FileSuccess.ToString());
                        }
                    }

                    LogInOutModel logInOutModel = new LogInOutModel();
                    logInOutModel.module_name = "GenReleaseMessage";
                    logInOutModel.action_name = model.payment_method;
                    logInOutModel.svc_req = model.mt_code;
                    logInOutModel.guid = Guid.NewGuid().ToString();
                    logInOutModel.ref_id = model.trans_cno;

                    apiStatic.LogInOut.Add(logInOutModel, p => { });
                }
                else
                {
                    StrMsg = "Save Complete And Error : " + result.Message;
                }

                //Step 2 : Remove TransNo ReleaseMessage = [Success] From ListTrans                
                if (Remove_TranscNoFromList(ref StrMsg, ref NextTransNo, model) == false)
                {
                    throw new Exception("Remove_TranscNoFromList() => " + StrMsg);
                }
            }
            catch (Exception Ex)
            {
                StrMsg = Ex.Message;
            }

            Result.Add(new { Message = StrMsg, trans_cno = NextTransNo });
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public bool Remove_TranscNoFromList(ref string ReturnMsg, ref string Next_TranscNo, RPCouponModel RPCouponModel)
        {
            try
            {
                int NextIndex;
                int TargetIndex = -999;
                List<string> ListCoupon = new List<string>();
                if (Session["ListCoupon"] != null && Session["ListCoupon"].ToString() != string.Empty)
                {
                    ListCoupon = (List<string>)Session["ListCoupon"];
                }

                //Step 1 : For Loop Find TargetIndex To Delete
                for (int i = 0; i < ListCoupon.Count; i++)
                {
                    if (ListCoupon[i].Contains(RPCouponModel.trans_cno) == true)
                    {
                        TargetIndex = i; //Keep Index To Delete
                        NextIndex = i + 1; //Keep Next Index To Release
                        if (NextIndex < ListCoupon.Count)
                        {
                            Next_TranscNo = ListCoupon[NextIndex];
                        }
                        else
                        {
                            if (ListCoupon.Count > 1)
                            {
                                Next_TranscNo = ListCoupon[0];
                            }
                        }
                        break;
                    }
                }

                //Step 2 : Delete TranscNo From TargetIndex
                if (TargetIndex != -999)
                {
                    ListCoupon.RemoveAt(TargetIndex);
                }

                //Step 3 : Delete TranscNo From TargetIndex
                if (ListCoupon.Count == 0)
                {
                    Session["ListCoupon"] = string.Empty;
                }
                else
                {
                    Session["ListCoupon"] = ListCoupon;
                }
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        // Function : Binding DDL

        public ActionResult FillTransDealType(string trans_deal_type)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            apiRPReleaseMsg.RPReleaseMessage.GetDDLTransDealType(trans_deal_type, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillEventType(string event_type)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            apiRPReleaseMsg.RPReleaseMessage.GetDDLEventType(event_type, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCounterPartyPayment(string counterpartyid)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            apiRPReleaseMsg.RPReleaseMessage.GetDDLPaymentMethodByCounterPartyID(counterpartyid, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillPaymentMethod(string payment_method)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            apiRPReleaseMsg.RPReleaseMessage.GetDDLPaymentMethod(payment_method, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillTransMtCode(string payment_method, string trans_deal_type, string event_type, string cur, string repo_deal_type)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            apiRPReleaseMsg.RPReleaseMessage.GetDDLTransMtCode(payment_method, trans_deal_type, event_type, cur, repo_deal_type, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCur(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            apiRPReleaseMsg.RPCallMargin.DDLCurrency(datastr, p =>
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
            apiRPReleaseMsg.RPCallMargin.DDLCounterParty(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillInstrument(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            apiRPReleaseMsg.RPCoupon.DDLInstrument(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillSwiftChannel(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            apiRPReleaseMsg.RPReleaseMessage.GetDDLSwiftChannel(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        // Function : Check

        public class DataChkPayment
        {
            public string from_page { get; set; }

            public string event_type { get; set; }

            public string trans_deal_type { get; set; }

            public string payment_method { get; set; }

            public string mt_code { get; set; }

            public string repo_deal_type { get; set; }
        }

        public ActionResult CheckPaymentMethodFromAjax(DataChkPayment DataChkPayment)
        {
            ResultWithModel<RPReleaseMsgCheckPaymentResult> Result = new ResultWithModel<RPReleaseMsgCheckPaymentResult>();
            try
            {
                RPReleaseMsgCheckPaymentModel ChkPaymentModel = new RPReleaseMsgCheckPaymentModel();

                ChkPaymentModel.from_page = DataChkPayment.from_page;
                ChkPaymentModel.event_type = DataChkPayment.event_type;
                ChkPaymentModel.trans_deal_type = DataChkPayment.trans_deal_type;
                ChkPaymentModel.payment_method = DataChkPayment.payment_method;
                ChkPaymentModel.mt_code = DataChkPayment.mt_code;
                ChkPaymentModel.repo_deal_type = DataChkPayment.repo_deal_type;

                apiRPReleaseMsg.RPReleaseMessage.CheckPaymentMethod(ChkPaymentModel, p =>
                {
                    if (p.Success)
                    {
                        Result = p;
                    }
                });
            }
            catch (Exception ex)
            {
                Log.WriteLog(Controller, "- CheckPaymentMethodFromAjax = " + ex.Message);
            }

            return Json(Result.Data == null ? new List<RPReleaseMsgCheckPaymentModel>() : Result.Data.RPReleaseMsgCheckPaymentResultModel, JsonRequestBehavior.AllowGet);
        }

        private bool CheckReleaseMessageConfirm(ref string ReturnMsg, ref bool IsPayment, string trans_deal_type, string event_type)
        {
            try
            {
                PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();
                ResultWithModel<RPReleaseMsgCheckPaymentResult> Result = new ResultWithModel<RPReleaseMsgCheckPaymentResult>();
                RPReleaseMsgCheckPaymentModel ChkPaymentModel = new RPReleaseMsgCheckPaymentModel();

                ChkPaymentModel.from_page = "Settlement";
                ChkPaymentModel.event_type = event_type.ToString();
                ChkPaymentModel.trans_deal_type = trans_deal_type;
                ChkPaymentModel.payment_method = "SWIFT";
                ChkPaymentModel.mt_code = "MT518";

                api_RPReleaseMsg.RPReleaseMessage.CheckPaymentMethod(ChkPaymentModel, p =>
                {
                    if (p.Success)
                    {
                        Result = p;
                    }
                    else
                    {
                        throw new Exception(Result.Message);
                    }
                });

                IsPayment = Result.Data.RPReleaseMsgCheckPaymentResultModel[0].is_payment;

            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        private bool ReleaseMessageConfirm(ref string ReturnMsg, RPTransModel model, string event_type)
        {
            try
            {
                RPTransModel modelConfirm = new RPTransModel();
                modelConfirm.trans_no = model.trans_no;
                modelConfirm.cur = model.cur;
                modelConfirm.event_type = event_type;
                modelConfirm.message_type = event_type;
                modelConfirm.create_by = HttpContext.User.Identity.Name;
                modelConfirm.payment_method = "SWIFT";
                modelConfirm.trans_mt_code = "MT518";
                Log.WriteLog(Controller, "Start ReleaseMessage Confirm Settlement ==========");
                Log.WriteLog(Controller, "- trans_no = " + model.trans_no);
                Log.WriteLog(Controller, "- cur = " + model.cur);


                //Step 1 : ReleaseMessage TransNo
                ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();
                PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();

                api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageConfirm(modelConfirm, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception("GenRPReleaseMessage Confirm Fail.");
                }

                if (result.Data != null && result.Data.RPReleaseMessageResultModel.Count() > 0)
                {
                    //Step 2 : StreamWriter File
                    Log.WriteLog(Controller, "Write File ReleaseMsg Confirm");
                    RPReleaseMessageModel ReleaseMsg = new RPReleaseMessageModel();
                    ReleaseMsg = result.Data.RPReleaseMessageResultModel[0];

                    FileEntity FileEnt = new FileEntity();
                    FileEnt.FileName = ReleaseMsg.file_name.Replace("/", "_");
                    FileEnt.FilePath = Server.MapPath(ReleaseMsg.file_path);
                    FileEnt.Values = ReleaseMsg.result;

                    Log.WriteLog(Controller, "- FileName = " + FileEnt.FileName);
                    Log.WriteLog(Controller, "- FilePath = " + FileEnt.FilePath);

                    WriteFile ObjWriteFile = new WriteFile();
                    if (ObjWriteFile.StreamWriter(ref FileEnt) == false)
                    {
                        throw new Exception("StreamWriter() => " + FileEnt.Msg);
                    }
                    Log.WriteLog(Controller, "Write File = Success.");

                    //Step 3 : SFTP File
                    Log.WriteLog(Controller, "ReleaseMsg To SFTP");
                    string StrMsg = string.Empty;
                    SftpEntity SftpOutEnt = new SftpEntity();
                    SftpEntity SftpBackOutEnt = new SftpEntity();
                    if (!SearchConfigReleaseMsgConfirm(ref StrMsg, ref SftpOutEnt, ref SftpBackOutEnt))
                    {
                        throw new Exception("Search_ConfigReleaseMsg() => " + FileEnt.Msg);
                    }
                    Log.WriteLog(Controller, "Read Config = Success.");

                    if (ReleaseMsg.Enable == "Y")
                    {
                        // Step 4.1 : Sftp REPO_OUT
                        ArrayList ListFile = new ArrayList();
                        ArrayList ListFileSuccess = new ArrayList();
                        ArrayList ListFileError = new ArrayList();

                        SftpOutEnt.LocalPath = FileEnt.FilePath;
                        ListFile.Add(FileEnt.FileName);

                        SftpUtility ObjectSftp = new SftpUtility();
                        if (!ObjectSftp.UploadSFTPList(ref StrMsg, SftpOutEnt, ListFile, ref ListFileError, ref ListFileSuccess))
                        {
                            throw new Exception("UploadSFTPList() => " + StrMsg);
                        }

                        if (ListFileError.Count > 0)
                        {
                            throw new Exception("UploadSFTPList() => " + ListFileError[0].ToString());
                        }

                        foreach (var FileSuccess in ListFileSuccess)
                        {
                            Log.WriteLog(Controller, "- SFTP " + SftpOutEnt.RemoteServerPath + " " + FileSuccess.ToString() + " Success.");
                        }

                        // Step 4.2 : Sftp REPO_BACKOUT
                        ListFile = new ArrayList();
                        ListFileSuccess = new ArrayList();
                        ListFileError = new ArrayList();

                        SftpBackOutEnt.LocalPath = FileEnt.FilePath;
                        ListFile.Add(FileEnt.FileName);

                        ObjectSftp = new SftpUtility();
                        if (!ObjectSftp.UploadSFTPList(ref StrMsg, SftpBackOutEnt, ListFile, ref ListFileError, ref ListFileSuccess))
                        {
                            throw new Exception("UploadSFTPList() => " + StrMsg);
                        }

                        if (ListFileError.Count > 0)
                        {
                            throw new Exception("UploadSFTPList() => " + ListFileError[0].ToString());
                        }

                        foreach (var FileSuccess in ListFileSuccess)
                        {
                            Log.WriteLog(Controller, "- SFTP " + SftpBackOutEnt.RemoteServerPath + " " + FileSuccess.ToString() + " Success.");
                        }
                    }
                    else
                    {
                        Log.WriteLog(Controller, "Release Message Confirm [Disable]");
                    }
                    ReturnMsg = " & Release Message Confirm Successfully.";
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(Controller, $"Release Message Fail : [Trans_No: {model.trans_no}]. {ex.Message}");
                ReturnMsg = "But Release Message Confirm Fail ";
            }
            finally
            {
                Log.WriteLog(Controller, "End ReleaseMessage Settlement ==========");
            }
            return true;
        }

        private bool SearchConfigReleaseMsgConfirm(ref string ReturnMsg, ref SftpEntity SftpOutEnt, ref SftpEntity SftpBackOutEnt)
        {
            try
            {
                RpSftpModel SftpModel = new RpSftpModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                StaticEntities StaticEnt = new StaticEntities();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("CONFIRM_MSG", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception(ResultRpconfig.Message);
                }

                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                SftpOutEnt.RemoteServerName = RpConfigModel.FirstOrDefault(a => a.item_code == "IP")?.item_value;
                SftpOutEnt.RemotePort = RpConfigModel.FirstOrDefault(a => a.item_code == "PORT")?.item_value;
                SftpOutEnt.RemoteUserName = RpConfigModel.FirstOrDefault(a => a.item_code == "USER")?.item_value;
                SftpOutEnt.RemotePassword = RpConfigModel.FirstOrDefault(a => a.item_code == "PASSWORD")?.item_value;
                SftpOutEnt.RemoteServerPath = RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SFTP_REPO_OUT")?.item_value;
                SftpOutEnt.RemoteSshHostKeyFingerprint = RpConfigModel.FirstOrDefault(a => a.item_code == "SSHHOSTKEYFINGERPRINT")?.item_value;
                SftpOutEnt.RemoteSshPrivateKeyPath = RpConfigModel.FirstOrDefault(a => a.item_code == "SSHPRIVATEKEYPATH")?.item_value;
                SftpOutEnt.NoOfFailRetry = System.Convert.ToInt32(RpConfigModel.FirstOrDefault(a => a.item_code == "FAIL_RETRY")?.item_value);

                SftpBackOutEnt.RemoteServerName = RpConfigModel.FirstOrDefault(a => a.item_code == "IP")?.item_value;
                SftpBackOutEnt.RemotePort = RpConfigModel.FirstOrDefault(a => a.item_code == "PORT")?.item_value;
                SftpBackOutEnt.RemoteUserName = RpConfigModel.FirstOrDefault(a => a.item_code == "USER")?.item_value;
                SftpBackOutEnt.RemotePassword = RpConfigModel.FirstOrDefault(a => a.item_code == "PASSWORD")?.item_value;
                SftpBackOutEnt.RemoteServerPath = RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SFTP_REPO_BACKOUT")?.item_value;
                SftpBackOutEnt.RemoteSshHostKeyFingerprint = RpConfigModel.FirstOrDefault(a => a.item_code == "SSHHOSTKEYFINGERPRINT")?.item_value;
                SftpBackOutEnt.RemoteSshPrivateKeyPath = RpConfigModel.FirstOrDefault(a => a.item_code == "SSHPRIVATEKEYPATH")?.item_value;
                SftpBackOutEnt.NoOfFailRetry = System.Convert.ToInt32(RpConfigModel.FirstOrDefault(a => a.item_code == "FAIL_RETRY")?.item_value);

                Log.WriteLog(Controller, "SFTP OutEnt / BackOutEnt");
                Log.WriteLog(Controller, "RemoteServerName = " + SftpBackOutEnt.RemoteServerName);
                Log.WriteLog(Controller, "RemotePort = " + SftpBackOutEnt.RemotePort);
                Log.WriteLog(Controller, "RemoteServerPath = " + SftpOutEnt.RemoteServerPath);
                Log.WriteLog(Controller, "RemoteServerPath = " + SftpBackOutEnt.RemoteServerPath);
                Log.WriteLog(Controller, "RemoteSshHostKeyFingerprint = " + SftpBackOutEnt.RemoteSshHostKeyFingerprint);
                Log.WriteLog(Controller, "RemoteSshPrivateKeyPath = " + SftpBackOutEnt.RemoteSshPrivateKeyPath);
                Log.WriteLog(Controller, "NoOfFailRetry File = " + SftpBackOutEnt.NoOfFailRetry);
            }
            catch (Exception ex)
            {
                ReturnMsg = ex.Message;
                return false;
            }

            return true;
        }

        [HttpGet]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult DownloadPDF(string asofdate, string counter_party_id)
        {
            try
            {
                var username = HttpContext.User.Identity.Name;
                var filename = "MarginNoticeReport_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".doc";
                Log.WriteLog(Controller, username + " Export " + filename);

                var rd = new ReportDocument();
                var dtResult = GetDataReport(asofdate, counter_party_id);

                if (dtResult.Rows.Count > 0)
                {
                    rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"), "MarginNoticeReport.rpt"));
                    rd.SetDataSource(dtResult);

                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.AppendHeader("Content-disposition", "attachment; filename=" + filename);
                    var stream = rd.ExportToStream(ExportFormatType.WordForWindows);
                    stream.Seek(0, SeekOrigin.Begin);
                    return new FileStreamResult(stream, "application/vnd.ms-word.document");
                }
                return RedirectToAction("NoData");
            }
            catch (Exception ex)
            {
                return Content(ex.Message, "text/html");
            }
        }

        public DataTable GetDataReport(string asofdate, string counter_party_id)
        {
            var apiReport = new ReportEntities();
            var dt = new DataTable();
            var data = new ReportCriteriaModel();
            data.asofdate = utility.ConvertStringToDatetimeFormatDDMMYYYY(asofdate);
            data.counterparty_id = counter_party_id;
            apiReport.ReportData.MarginNotiecReport(data, p =>
            {
                if (p.Success) dt = p.Data.MarginNoticeReportResultModel.ToDataTable();
            });

            return dt;
        }
    }
}