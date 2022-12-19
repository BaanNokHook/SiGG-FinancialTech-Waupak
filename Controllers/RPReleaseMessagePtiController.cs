using GM.CommonLibs;
using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.PaymentProcess;
using GM.Data.Model.RPTransaction;
using GM.Data.Model.Static;
using GM.Data.Result.PaymentProcess;
using GM.Data.Result.RPTransaction;
using GM.Data.View.RPTransaction;
using GM.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class RPReleaseMessagePtiController : BaseController
    {
        private static string Controller = "RPReleaseMessageController";
        private static LogFile Log = new LogFile();
        PaymentProcessEntities apiRPReleaseMsg = new PaymentProcessEntities();
        StaticEntities apiStatic = new StaticEntities();
        Utility utility = new Utility();

        // Function : Binding DDL
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

        public ActionResult FillPaymentMethod(string payment_method)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            apiRPReleaseMsg.RPReleaseMessage.GetDDLPaymentMethodPti(payment_method, p =>
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
            apiRPReleaseMsg.RPReleaseMessage.GetDDLEventTypePti(event_type, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillTransMtCode(string payment_method, string trans_deal_type, string event_type, string cur)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            apiRPReleaseMsg.RPReleaseMessage.GetDDLTransMtCode(payment_method, trans_deal_type, event_type, cur,null, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        // Action : Index
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index(string search)
        {
            var RPReleaseMessageView = new RPDealEntryViewModel();
            RPTransModel RPTrans = new RPTransModel();

            if (search == "callback")
            {
                if (TempData["Search-RPReleaseMessagePti"] != null)
                {
                    RPTrans = (RPTransModel)TempData["Search-RPReleaseMessagePti"];
                }
            }
            else if (search == "Maturity")
            {
                RPTrans.event_type = "Maturity";
                //TempData["Search-RPReleaseMessage"] = RPTrans;
            }
            else
            {
                RPTrans.event_type = "Settlement";
            }

            RPReleaseMessageView.FormSearch = RPTrans;

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
                        case "event_type":
                            RPTransModel.event_type = column.search.value;
                            break;
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
                            //if (RPTransModel.event_type == "Settlement")
                            //{
                                RPTransModel.from_settlement_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            //}
                            break;
                        case "to_settlement_date":
                            //if (RPTransModel.event_type == "Settlement")
                            //{
                                RPTransModel.to_settlement_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            //}
                            break;
                        case "from_maturity_date":
                            if (RPTransModel.event_type == "Maturity")
                            {
                                RPTransModel.from_maturity_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            }  
                            break;
                        case "to_maturity_date":
                            if (RPTransModel.event_type == "Maturity")
                            {
                                RPTransModel.to_maturity_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            }
                            break;
                        case "trans_deal_type":
                            RPTransModel.trans_deal_type = column.search.value;
                            break;
                        case "trans_deal_type_name":
                            RPTransModel.trans_deal_type_name = column.search.value;
                            break;
                        case "payment_method":
                            RPTransModel.payment_method = column.search.value;
                            break;
                        case "trans_mt_code":
                            RPTransModel.trans_mt_code = column.search.value;
                            break;
                    }
                });


                RPTransModel.payment_method = "DVP/RVP";
                apiRPReleaseMsg.RPReleaseMessage.GetRPReleaseMessageListPTI(RPTransModel, p =>
                {
                    result = p;
                });

                TempData["Search-RPReleaseMessagePti"] = RPTransModel;

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

        public ActionResult Select(string id)
        {
            List<string> ListTrans = new List<string>();
            ListTrans = JsonConvert.DeserializeObject<List<string>>(id);
            ListTrans.Sort();
            Session["ListTrans"] = ListTrans;

            return RedirectToAction("Release", "RPReleaseMessage", new { trans_no = ListTrans[0], event_type = "pti" });

        }

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

        public ActionResult GetReleaseMT(Data data)
        {
            RPTransModel RPTransModel = new RPTransModel();
            ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();
            RPReleaseMessageResult RPReleaseMessageResultModel = new RPReleaseMessageResult();

            try
            {
                // Step 1 : Check ReleaseMsg
                bool IsPayment = false;
                PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();
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
                    // ไม่ต้อง Gen ก่อนดู Message
                    #region Gen ReleaseMsg 
                    // Step 2 : Gen ReleaseMsg 
                    RPTransModel = new RPTransModel();
                    RPTransModel.trans_no = data.trans_no;
                    RPTransModel.event_type = data.from_page;
                    RPTransModel.payment_method = data.payment_method;
                    RPTransModel.trans_mt_code = data.trans_mt_code;
                    RPTransModel.cur = data.cur;
                    RPTransModel.create_by = HttpContext.User.Identity.Name;
                    api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessage(RPTransModel, p =>
                    {
                        result = p;
                    });
                    if (!result.Success)
                    {
                        throw new Exception("GenRPReleaseMessage() => " + result.Message);
                    }
                    #endregion

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
                else
                {
                    result.Data = RPReleaseMessageResultModel;
                }

            }
            catch (Exception Ex)
            {
                result.Message = Ex.Message;
            }

            return Json(result.Data.RPReleaseMessageResultModel, JsonRequestBehavior.AllowGet);
        }

        private class FilePtiEntity {
            public string file_name = string.Empty;
            public string file_content = string.Empty;
            public string lineNo_First = string.Empty;
            public string lineNo_End = string.Empty;
        }

        public ActionResult ReleaseMessagePti(List<RPTransModel> models)
        {
            string StrMsg = string.Empty;
            var Result = new List<object>();
            bool Status = true;

            FilePtiEntity PtiResult = new FilePtiEntity();

            try
            {
                ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < models.Count; i++)
                {
                    // Step 1 : Gen RPReleaseMessage PTI
                    RPTransModel RPTransModel = new RPTransModel();

                    RPTransModel.trans_no = models[i].trans_no;
                    RPTransModel.payment_method = models[i].payment_method;
                    RPTransModel.trans_mt_code = models[i].trans_mt_code;
                    RPTransModel.event_type = models[i].event_type;
                    RPTransModel.cur = models[i].cur;
                    RPTransModel.create_by = HttpContext.User.Identity.Name;

                    apiRPReleaseMsg.RPReleaseMessage.GenRPReleaseMessage(RPTransModel, p =>
                    {
                        result = p;
                    });

                    if (!result.Success)
                    {
                        throw new Exception("GenRPReleaseMessage() => " + result.Message);
                    }

                    RPReleaseMessageModel ReleaseMsg = new RPReleaseMessageModel();
                    ReleaseMsg = result.Data.RPReleaseMessageResultModel[0];
                    string[] strResult = ReleaseMsg.result.Replace(Environment.NewLine, ",").Split(',');

                    PtiResult.file_name = ReleaseMsg.file_name;
                    PtiResult.lineNo_First = strResult[0];
                    PtiResult.lineNo_End = strResult[strResult.Length - 2];

                    if (i == 0)
                    {
                        sb.Append(PtiResult.lineNo_First);
                        sb.AppendLine();
                    }

                    for (int j = 0; j < strResult.Length; j++)
                    {
                        if (j == 0)
                        {
                            continue;
                        }

                        if (strResult[j] == "</ns0:CSDXML>" || strResult[j].Trim() == string.Empty)
                        {
                            continue;
                        }
                        else
                        {
                            sb.Append(strResult[j]);
                            sb.AppendLine();
                        }
                    };

                    LogInOutModel logInOutModel = new LogInOutModel();
                    logInOutModel.module_name = "GenReleaseMessage";
                    logInOutModel.action_name = models[i].payment_method;
                    logInOutModel.svc_req = models[i].trans_mt_code;
                    logInOutModel.guid = Guid.NewGuid().ToString();
                    logInOutModel.ref_id = models[i].trans_no;
                    apiStatic.LogInOut.Add(logInOutModel, p => { });

                }

                sb.Append(PtiResult.lineNo_End);
                sb.AppendLine();

                PtiResult.file_content = sb.ToString();

                Result.Add(new { Success = Status, Message = StrMsg, file_name = PtiResult.file_name, file_content = PtiResult.file_content });

            }
            catch (Exception Ex)
            {
                Status = false;
                StrMsg = Ex.Message;
                Log.WriteLog(Controller, "Error " + Ex.Message);
                Result.Add(new { Success = Status, Message = StrMsg });
            }

            return Json(Result, JsonRequestBehavior.AllowGet);
        }


    }
}