using GM.CommonLibs;
using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.PaymentProcess;
using GM.Data.Model.Provider;
using GM.Data.Model.Static;
using GM.Data.Result.PaymentProcess;
using GM.Data.Result.Static;
using GM.Filters;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Spire.Xls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class RPCallMarginController : BaseController
    {
        private static string Controller = "RPCallMarginController";
        private static LogFile Log = new LogFile();
        PaymentProcessEntities api = new PaymentProcessEntities();
        StaticEntities apiStatic = new StaticEntities();
        GM.Data.Helper.Utility utility = new GM.Data.Helper.Utility();
        ExternalInterfaceEntities apiEx = new ExternalInterfaceEntities();

        public class Data
        {
            [Display(Name = "Pay/Receive")]
            public string margin_type { get; set; }

            [Display(Name = "Cntr Name")]
            public string counter_party_name { get; set; }

            [Display(Name = "Margin Amt")]
            public decimal margin_amt { get; set; }

            [Display(Name = "Int on Cash Margin")]
            public decimal int_cash_margin { get; set; }

            [Display(Name = "Status")]
            public string status { get; set; }

            [Display(Name = "Payment Method")]
            public string payment_method { get; set; }
        }
        public class DataForAdjust
        {
            public string asofdate { get; set; }
            public int ctpyID { get; set; }
            public decimal todayExposure { get; set; }
            public decimal totalIntRec { get; set; }
            public decimal totalIntPay { get; set; }
            public decimal intTax { get; set; }

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

            public string swift_channel { get; set; }
        }

        // GET: RPCallMargin
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            ResultWithModel<RPCallMarginResult> result = new ResultWithModel<RPCallMarginResult>();
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
                        var col = model.columns[2];
                        orders.Add(new OrderByModel { Name = col.data, SortDirection = (o.dir.Equals("desc") ? SortDirection.Descending : SortDirection.Ascending) });
                    });
                }

                rpCallMarginModel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null).ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "call_date":
                            rpCallMarginModel.call_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "interes_rate":
                            rpCallMarginModel.interes_rate = column.search.value == "" ? 0 : Convert.ToDecimal(column.search.value);
                            break;
                        case "cur":
                            rpCallMarginModel.cur = column.search.value;
                            break;
                        case "counter_party_id":
                            if (column.search.value != "")
                            {
                                rpCallMarginModel.counter_party_id = column.search.value == "" ? 0 : Convert.ToInt32(column.search.value);
                            }
                            else
                            {
                                rpCallMarginModel.counter_party_id = null;
                            }

                            break;
                    }
                });

                api.RPCallMargin.RPCallMarginList(rpCallMarginModel, p =>
                {
                    result = p;
                });
            }
            catch (Exception ex)
            {
                Log.WriteLog(Controller, "Error : " + ex.Message);
                result.Message = ex.Message;
            }
            return Json(new
            {
                draw = model.draw,
                recordsTotal = result.HowManyRecord,
                recordsFiltered = result.HowManyRecord,
                Message = result.Message,
                data = result.Data != null ? result.Data.RPCallMarginResultModel : new List<RPCallMarginModel>()
            });
        }

        [HttpPost]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult SearchPRP(DataTableAjaxPostModel model)
        {
            ResultWithModel<RPCallMarginPRPResult> result = new ResultWithModel<RPCallMarginPRPResult>();
            string call_date = null;
            string cur = null;
            string counter_party_id = null;
            int isCall = 0;
            var columns = model.columns.Where(o => o.search.value != null).ToList();
            columns.ForEach(column =>
            {
                switch (column.data)
                {
                    case "call_date":
                        call_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value).ToString("yyyy-MM-dd");
                        break;
                    case "cur":
                        cur = column.search.value;
                        break;
                    case "counter_party_id":
                        counter_party_id = column.search.value;
                        break;
                    case "isCall":
                        isCall = Convert.ToInt32(column.search.value);
                        break;
                }
            });

            api.RPCallMargin.MarginDetailPRP(call_date, call_date, counter_party_id, cur, p =>
            {
                result = p;
                if (result.Data != null)
                {
                    result.HowManyRecord = result.Data.RPCallMarginPRPResultModel.Count();
                    foreach (var tmpResult in result.Data.RPCallMarginPRPResultModel)
                    {
                        tmpResult.isCall = isCall;
                    }
                }
            });
            return Json(new
            {
                draw = model.draw,
                recordsTotal = result.HowManyRecord,
                recordsFiltered = result.HowManyRecord,
                Message = result.Message,
                data = result.Data != null ? result.Data.RPCallMarginPRPResultModel : new List<RPCallMarginPRPModel>()
            });
        }

        [HttpPost]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult SearchBRP(DataTableAjaxPostModel model)
        {
            string call_date = null;
            string cur = null;
            string counter_party_id = null;
            var columns = model.columns.Where(o => o.search.value != null).ToList();
            columns.ForEach(column =>
            {
                switch (column.data)
                {
                    case "call_date":
                        call_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value).ToString("yyyy-MM-dd");
                        break;
                    case "cur":
                        cur = column.search.value;
                        break;
                    case "counter_party_id":
                        counter_party_id = column.search.value;
                        break;
                }
            });

            ResultWithModel<RPCallMarginBRPResult> result = new ResultWithModel<RPCallMarginBRPResult>();
            api.RPCallMargin.MarginDetailBRP(call_date, counter_party_id, cur, p =>
            {
                result = p;
            });

            return Json(new
            {
                draw = model.draw,
                recordsTotal = result.HowManyRecord,
                recordsFiltered = result.HowManyRecord,
                Message = result.Message,
                data = result.Data != null ? result.Data.RPCallMarginBRPResultModel : new List<RPCallMarginBRPModel>()
            });
        }

        public ActionResult FillPaymentMethod(string transdealtype)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api.RPCallMargin.DDLPaymentMethod(transdealtype, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillMTCode(string paymentmethod, string transdealtype, string cur)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api.RPCallMargin.DDLMTCode(paymentmethod, transdealtype, cur, p =>
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

        [HttpPost]
        public ActionResult GetInterestRate(string date, string cur)
        {
            try
            {
                decimal interRestRate = 0;
                api.RPCallMargin.GetInterestRate(date, cur, p =>
               {
                   interRestRate = Convert.ToDecimal(p.Message);
               });

                return Json(new
                {
                    interRest = interRestRate,
                    returnCode = 0
                });
            }
            catch (Exception ex)
            {
                Log.WriteLog(Controller, "Error : " + ex.Message);
                return Json(new
                {
                    interRest = 0.00,
                    returnCode = 1,
                    Message = ex.ToString()
                });
            }
        }

        [HttpPost]
        public ActionResult GetNextBusinessDate(string date, string cur)
        {
            try
            {
                string nextBusinessDate = string.Empty;
                api.RPCallMargin.GetNextBusinessDate(date, cur, p =>
                {
                    nextBusinessDate = Convert.ToDateTime(p.Message).ToString("MM/dd/yyyy");
                });

                return Json(new
                {
                    NextBusinessDate = nextBusinessDate,
                    returnCode = 0
                });
            }
            catch (Exception ex)
            {
                Log.WriteLog(Controller, "Error : " + ex.Message);
                return Json(new
                {
                    NextBusinessDate = "",
                    returnCode = 1,
                    Message = ex.ToString()
                });
            }
        }

        [RoleScreen(RoleScreen.VIEW)]
        [HttpPost]
        public ActionResult Process(string tradeDate, string cur, decimal rate, decimal? holiday_rate)
        {
            ResultWithModel<RPCallMarginResult> result = new ResultWithModel<RPCallMarginResult>();
            RPCallMarginModel rpCallMarginModel = new RPCallMarginModel();
            bool isSuccess = true;
            string message = string.Empty;
            try
            {
                api.RPCallMargin.ActionInterestRate(tradeDate, cur, rate, p =>
                 {
                     result = p;
                 });

                if (result.Success)
                {
                    api.RPCallMargin.CallMargin(tradeDate, cur, holiday_rate, p =>
                    {
                        result = p;
                    });

                    if (!result.Success)
                    {
                        message = result.Message;
                        isSuccess = false;
                    }
                }
                else
                {
                    message = "Not Call Margin : Save InteresRate Fail";
                    isSuccess = false;
                }
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

        [RoleScreen(RoleScreen.VIEW)]
        [HttpPost]
        public ActionResult AdjustPRP(string data)
        {
            ResultWithModel<RPCallMarginResult> result = new ResultWithModel<RPCallMarginResult>();
            bool isSuccess = true;
            string message = string.Empty;
            try
            {
                List<RPCallMarginPRPModel> ListTrans = new List<RPCallMarginPRPModel>();
                ListTrans = JsonConvert.DeserializeObject<List<RPCallMarginPRPModel>>(data);

                api.RPCallMargin.AdjustPRP(ListTrans[0], p =>
                {
                    result = p;
                });

                isSuccess = result.Success;
                message = result.Message;
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

        [RoleScreen(RoleScreen.VIEW)]
        [HttpPost]
        public ActionResult AdjustBRP(string data)
        {
            ResultWithModel<RPCallMarginResult> result = new ResultWithModel<RPCallMarginResult>();
            bool isSuccess = true;
            string message = string.Empty;
            try
            {
                List<RPCallMarginBRPModel> ListTrans = new List<RPCallMarginBRPModel>();
                ListTrans = JsonConvert.DeserializeObject<List<RPCallMarginBRPModel>>(data);
                foreach (var item in ListTrans)
                {
                    api.RPCallMargin.AdjustBRP(item, p =>
                    {
                        result = p;
                    });
                    if (isSuccess)
                    {
                        isSuccess = result.Success;
                    }
                    if (!result.Success)
                    {
                        message += "<li style='text-align:left; color:red;'> Contract No. " + item.contract_no + " : " + result.Message + " </li>";
                    }
                }

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

        [RoleScreen(RoleScreen.VIEW)]
        [HttpPost]
        public ActionResult MarginDetail(string asOfDate, string counterPartyId)
        {
            ResultWithModel<RPCallMarginDetailResult> result = new ResultWithModel<RPCallMarginDetailResult>();
            RPCallMarginModel rpCallMarginModel = new RPCallMarginModel();
            bool isSuccess = true;
            string message = string.Empty;
            string[] listCounterParty = counterPartyId.Split('|');
            List<string> listTransNo = new List<string>();
            foreach (var cptID in listCounterParty.Where(x => x != ""))
            {
                try
                {
                    api.RPCallMargin.MarginDetail(asOfDate, cptID, p =>
                    {
                        result = p;
                    });

                    if (result.Success)
                    {
                        foreach (var item in result.Data.RPCallMarginDetailResultModel)
                        {
                            listTransNo.Add(item.trans_no);
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = result.Message
                        });
                    }

                    listTransNo = listTransNo.Distinct().ToList();
                }
                catch (Exception ex)
                {
                    Log.WriteLog(Controller, "Error : " + ex.Message);
                    return Json(new
                    {
                        Success = false,
                        Message = ex.Message
                    });
                }
            }
            return Json(new
            {
                Success = isSuccess,
                Message = message,
                result = listTransNo
            });
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult CheckSettlementStatus(RPCallMarginModel model)
        {
            string StrMsg = string.Empty;
            string NextTransNo = string.Empty;
            var Result = new List<object>();
            ResultWithModel<CheckSettlementStatusResponseModel> result = new ResultWithModel<CheckSettlementStatusResponseModel>();

            try
            {
                //List<RPCallMarginModel> listMargin = JsonConvert.DeserializeObject<List<RPCallMarginModel>>(listData);
                //foreach (RPCallMarginModel model in listData)
                {
                    string dealNo = model.call_date.Value.ToString("yyyyMMdd") + "_" + model.counter_party_id + "_" + model.cur + (model.eom_int_flag == "Y" ? "_Int" : "");

                    Log.WriteLog(Controller, "Start CheckSettlementStatus ==========");
                    Log.WriteLog(Controller, "- trans_no = " + dealNo);
                    Log.WriteLog(Controller, "- payment_method = " + model.payment_method);
                    Log.WriteLog(Controller, "- mt_code = " + model.mt_code);
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
                        DealNo = dealNo,
                        DealType = "MARGIN",
                        Product = string.Empty,
                        ProductGroup = string.Empty,
                        Ccy = model.cur,
                        MTType = model.mt_code.Replace("MT", ""),
                        ValueDate = model.call_date.Value.ToString("yyyyMMdd")
                    };

                    data.SettlementInfo = new List<RequestDetail>();
                    data.SettlementInfo.Add(request);

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
                            throw new Exception("ResponseBody No Data");
                        }
                        else if (result.Data.ResponseBody[0].ReturnId != 0)
                        {
                            throw new Exception(result.Data.ResponseBody[0].ReturnMessage);
                        }
                    }
                    else
                    {
                        throw new Exception(result.Data.Header.ResponseMessage);
                    }

                    if (result.Data.ResponseBody[0].SettlementStatus == (int)SettlementStatus.Complete)
                    {
                        RPReleaseCyberPayModel cbpModel = new RPReleaseCyberPayModel();
                        cbpModel.DealNo = dealNo;
                        cbpModel.DealType = "MARGIN";
                        cbpModel.MTType = model.mt_code.Replace("MT", "");
                        cbpModel.ValueDate = model.call_date.Value.ToString("yyyyMMdd");
                        cbpModel.counter_party_id = model.counter_party_id.ToString();
                        cbpModel.ccy = model.cur;
                        cbpModel.Seq = 1;
                        cbpModel.create_by = model.create_by;
                        cbpModel.SettlementStatus = (int)SettlementStatus.Complete;

                        ResultWithModel<RPReleaseCyberPayResult> rwmUpdate = new ResultWithModel<RPReleaseCyberPayResult>();
                        api.RPReleaseCyberPay.Update(cbpModel, p =>
                        {
                            rwmUpdate = p;
                        });
                    }
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

        [HttpGet]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult ReleaseMessage(string data)
        {
            string StrMsg = string.Empty;
            bool returnCode = true;
            var Result = new List<object>();

            try
            {
                List<DataForReleaseMsg> listData = JsonConvert.DeserializeObject<List<DataForReleaseMsg>>(data);
                foreach (DataForReleaseMsg dataReleaseMsg in listData)
                {
                    RPCallMarginModel model = new RPCallMarginModel();
                    ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();

                    //Init Model for Release Message
                    model.create_by = HttpContext.User.Identity.Name;
                    model.counter_party_id = dataReleaseMsg.counter_party_id;
                    model.call_date = dataReleaseMsg.call_date;
                    model.cur = dataReleaseMsg.cur;
                    model.payment_method = dataReleaseMsg.payment_method;
                    model.mt_code = dataReleaseMsg.mt_code;
                    model.event_type = "Margin";
                    model.brp_contract_no = dataReleaseMsg.brp_contract_no;

                    api.RPReleaseMessage.GenRPReleaseMessageByMargin(model, p =>
                    {
                        result = p;
                    });

                    if (!result.Success)
                    {
                        throw new Exception("ReleaseMessage : " + result.Message);
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

                    RPReleaseCyberPayModel cbpModel = new RPReleaseCyberPayModel();
                    cbpModel.DealNo = model.call_date.Value.ToString("yyyyMMdd") + "_" + model.counter_party_id + "_" + model.cur;
                    cbpModel.DealType = "MARGIN";
                    cbpModel.MTType = model.mt_code.Replace("MT", "");
                    cbpModel.ValueDate = model.call_date.Value.ToString("yyyyMMdd");
                    cbpModel.counter_party_id = model.counter_party_id.ToString();
                    cbpModel.ccy = model.cur;
                    cbpModel.Seq = 1;
                    cbpModel.create_by = model.create_by;

                    ResultWithModel<RPReleaseCyberPayResult> rwmSearch = new ResultWithModel<RPReleaseCyberPayResult>();
                    api.RPReleaseCyberPay.Search(cbpModel, p =>
                    {
                        rwmSearch = p;
                    });

                    if (rwmSearch.Success)
                    {
                        //Step 1 : Init Model
                        InsertSettlementInfoRequestModel req = new InsertSettlementInfoRequestModel();
                        req.Header = new RequestHeader
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

                        req.SettlementInfo = new List<RequestInsertSettlementInfo>();
                        req.SettlementInfo.AddRange(request);

                        ResultWithModel<InsertSettlementInfoResponseModel> resultInsert = new ResultWithModel<InsertSettlementInfoResponseModel>();

                        apiEx.InterfaceCyberPay.InsertSettlementInfo(req, p =>
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
                    logInOutModel.svc_res = model.brp_contract_no;
                    if (dataReleaseMsg.call_date != null)
                    {
                        logInOutModel.ref_id = dataReleaseMsg.call_date.Value.ToString("yyyyMMdd") + "_" + dataReleaseMsg.counter_party_id + "_" + dataReleaseMsg.cur;
                    }
                    else
                    {
                        logInOutModel.ref_id = DateTime.Now.ToString("yyyyMMdd") + "_" + dataReleaseMsg.counter_party_id + "_" + dataReleaseMsg.cur;
                    }
                    apiStatic.LogInOut.Add(logInOutModel, p => { });
                }
            }
            catch (Exception ex)
            {
                StrMsg = ex.Message;
                returnCode = false;
                Log.WriteLog(Controller, "Error : " + ex.Message);
            }
            finally
            {
                Log.WriteLog(Controller, "End ReleaseMessage ==========");
            }
            return Json(new { Message = StrMsg, returnCode = returnCode }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult ReleaseMessageCyberPay(string data)
        {
            string StrMsg = string.Empty;
            bool returnCode = true;
            var Result = new List<object>();

            try
            {
                List<RPCallMarginModel> listData = JsonConvert.DeserializeObject<List<RPCallMarginModel>>(data);
                foreach (RPCallMarginModel model in listData)
                {
                    //RPCallMarginModel model = new RPCallMarginModel();
                    ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();
                    model.create_by = HttpContext.User.Identity.Name;
                    model.event_type = "Margin";

                    api.RPReleaseMessage.GenRPReleaseMessageByMarginCyberPay(model, p =>
                    {
                        result = p;
                    });

                    if (!result.Success)
                    {
                        throw new Exception("ReleaseMessage : " + result.Message);
                    }

                    RPReleaseCyberPayModel cbpModel = new RPReleaseCyberPayModel();
                    cbpModel.DealNo = model.call_date.Value.ToString("yyyyMMdd") + "_" + model.counter_party_id + "_" + model.cur;
                    cbpModel.DealType = "MARGIN";
                    cbpModel.MTType = model.mt_code.Replace("MT", "");
                    cbpModel.ValueDate = model.call_date.Value.ToString("yyyyMMdd");
                    cbpModel.counter_party_id = model.counter_party_id.ToString();
                    cbpModel.ccy = model.cur;
                    cbpModel.Seq = 1;
                    cbpModel.create_by = model.create_by;

                    ResultWithModel<RPReleaseCyberPayResult> rwmSearch = new ResultWithModel<RPReleaseCyberPayResult>();
                    ResultWithModel<InsertSettlementInfoResponseModel> resultInsert = new ResultWithModel<InsertSettlementInfoResponseModel>();
                    api.RPReleaseCyberPay.Search(cbpModel, p =>
                    {
                        rwmSearch = p;
                    });

                    if (rwmSearch.Success)
                    {
                        //Step 1 : Init Model
                        InsertSettlementInfoRequestModel req = new InsertSettlementInfoRequestModel();
                        req.Header = new RequestHeader
                        {
                            SystemCode = "REPO",
                            AuthorityCode = "REPO",
                            RefCode = DateTime.Now.ToString("yyyyMMddTHHmmssfff"),
                            RequestDate = DateTime.Now.ToString("yyyyMMdd"),
                            RequestTime = DateTime.Now.ToString("HH:mm:ss"),
                            WSMode = 1
                        };

                        List<RequestInsertSettlementInfo> request = JsonConvert.DeserializeObject<List<RequestInsertSettlementInfo>>(JsonConvert.SerializeObject(rwmSearch.Data.RPReleaseCyberPayResultModel));

                        req.SettlementInfo = new List<RequestInsertSettlementInfo>();
                        req.SettlementInfo.AddRange(request);

                        cbpModel.SettlementStatus = req.SettlementInfo[0].SettlementStatus;

                        apiEx.InterfaceCyberPay.InsertSettlementInfo(req, p =>
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
                    api.RPReleaseCyberPay.Update(cbpModel, p =>
                    {
                        rwmUpdate = p;
                    });

                    LogInOutModel logInOutModel = new LogInOutModel();
                    logInOutModel.module_name = "GenReleaseMessage";
                    logInOutModel.action_name = model.payment_method;
                    logInOutModel.svc_req = model.mt_code;
                    logInOutModel.guid = Guid.NewGuid().ToString();
                    logInOutModel.svc_res = model.brp_contract_no;
                    if (model.call_date != null)
                    {
                        logInOutModel.ref_id = model.call_date.Value.ToString("yyyyMMdd") + "_" + model.counter_party_id + "_" + model.cur;
                    }
                    else
                    {
                        logInOutModel.ref_id = DateTime.Now.ToString("yyyyMMdd") + "_" + model.counter_party_id + "_" + model.cur;
                    }
                    apiStatic.LogInOut.Add(logInOutModel, p => { });

                }
            }
            catch (Exception ex)
            {
                StrMsg = ex.Message;
                returnCode = false;
                Log.WriteLog(Controller, "Error : " + ex.Message);
            }
            finally
            {
                Log.WriteLog(Controller, "End ReleaseMessage ==========");
            }
            return Json(new { Message = StrMsg, returnCode = returnCode }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult ReleaseMessageInterestMargin(string data)
        {
            string StrMsg = string.Empty;
            bool returnCode = true;
            var Result = new List<object>();

            try
            {
                List<RPCallMarginModel> listData = JsonConvert.DeserializeObject<List<RPCallMarginModel>>(data);
                foreach (RPCallMarginModel model in listData)
                {
                    //RPCallMarginModel model = new RPCallMarginModel();
                    ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();

                    //Init Model for Release Message
                    model.create_by = HttpContext.User.Identity.Name;
                    model.event_type = "MARGIN-INT";

                    api.RPReleaseMessage.GenRPReleaseMessageByInterestMargin(model, p =>
                    {
                        result = p;
                    });

                    if (!result.Success)
                    {
                        throw new Exception("ReleaseMessage : " + result.Message);
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

                    RPReleaseCyberPayModel cbpModel = new RPReleaseCyberPayModel();
                    cbpModel.DealNo = model.call_date.Value.ToString("yyyyMMdd") + "_" + model.counter_party_id + "_" + model.cur;
                    cbpModel.DealType = "MARGIN-INT";
                    cbpModel.MTType = model.mt_code.Replace("MT", "");
                    cbpModel.ValueDate = model.call_date.Value.ToString("yyyyMMdd");
                    cbpModel.counter_party_id = model.counter_party_id.ToString();
                    cbpModel.ccy = model.cur;
                    cbpModel.Seq = 1;
                    cbpModel.create_by = model.create_by;

                    ResultWithModel<RPReleaseCyberPayResult> rwmSearch = new ResultWithModel<RPReleaseCyberPayResult>();
                    api.RPReleaseCyberPay.Search(cbpModel, p =>
                    {
                        rwmSearch = p;
                    });

                    if (rwmSearch.Success)
                    {
                        //Step 1 : Init Model
                        InsertSettlementInfoRequestModel req = new InsertSettlementInfoRequestModel();
                        req.Header = new RequestHeader
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

                        req.SettlementInfo = new List<RequestInsertSettlementInfo>();
                        req.SettlementInfo.AddRange(request);

                        ResultWithModel<InsertSettlementInfoResponseModel> resultInsert = new ResultWithModel<InsertSettlementInfoResponseModel>();

                        apiEx.InterfaceCyberPay.InsertSettlementInfo(req, p =>
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
                    logInOutModel.module_name = "GenReleaseMessageInterestMargin";
                    logInOutModel.action_name = model.payment_method;
                    logInOutModel.svc_req = model.mt_code;
                    logInOutModel.guid = Guid.NewGuid().ToString();
                    logInOutModel.svc_res = model.brp_contract_no;
                    if (model.call_date != null)
                    {
                        logInOutModel.ref_id = model.call_date.Value.ToString("yyyyMMdd") + "_" + model.counter_party_id + "_" + model.cur + "_Int";
                    }
                    else
                    {
                        logInOutModel.ref_id = DateTime.Now.ToString("yyyyMMdd") + "_" + model.counter_party_id + "_" + model.cur + "_Int";
                    }
                    apiStatic.LogInOut.Add(logInOutModel, p => { });
                }
            }
            catch (Exception ex)
            {
                StrMsg = ex.Message;
                returnCode = false;
                Log.WriteLog(Controller, "Error : " + ex.Message);
            }
            finally
            {
                Log.WriteLog(Controller, "End ReleaseMessage ==========");
            }
            return Json(new { Message = StrMsg, returnCode = returnCode }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult ReleaseMessageInterestMarginCyberPay(string data)
        {
            string StrMsg = string.Empty;
            bool returnCode = true;
            var Result = new List<object>();

            try
            {
                List<RPCallMarginModel> listData = JsonConvert.DeserializeObject<List<RPCallMarginModel>>(data);
                foreach (RPCallMarginModel model in listData)
                {
                    ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();

                    //Init Model for Release Message
                    model.create_by = HttpContext.User.Identity.Name;
                    model.event_type = "MARGIN-INT";

                    api.RPReleaseMessage.GenRPReleaseMessageByInterestMarginCyberPay(model, p =>
                    {
                        result = p;
                    });

                    if (!result.Success)
                    {
                        throw new Exception("ReleaseMessage : " + result.Message);
                    }

                    RPReleaseCyberPayModel cbpModel = new RPReleaseCyberPayModel();
                    cbpModel.DealNo = model.call_date.Value.ToString("yyyyMMdd") + "_" + model.counter_party_id + "_" + model.cur + "_Int";
                    cbpModel.DealType = "MARGIN-INT";
                    cbpModel.MTType = model.mt_code.Replace("MT", "");
                    cbpModel.ValueDate = model.call_date.Value.ToString("yyyyMMdd");
                    cbpModel.counter_party_id = model.counter_party_id.ToString();
                    cbpModel.ccy = model.cur;
                    cbpModel.Seq = 1;
                    cbpModel.create_by = model.create_by;

                    ResultWithModel<RPReleaseCyberPayResult> rwmSearch = new ResultWithModel<RPReleaseCyberPayResult>();
                    ResultWithModel<InsertSettlementInfoResponseModel> resultInsert = new ResultWithModel<InsertSettlementInfoResponseModel>();
                    api.RPReleaseCyberPay.Search(cbpModel, p =>
                    {
                        rwmSearch = p;
                    });

                    if (rwmSearch.Success)
                    {
                        //Step 1 : Init Model
                        InsertSettlementInfoRequestModel req = new InsertSettlementInfoRequestModel();
                        req.Header = new RequestHeader
                        {
                            SystemCode = "REPO",
                            AuthorityCode = "REPO",
                            RefCode = DateTime.Now.ToString("yyyyMMddTHHmmssfff"),
                            RequestDate = DateTime.Now.ToString("yyyyMMdd"),
                            RequestTime = DateTime.Now.ToString("HH:mm:ss"),
                            WSMode = 1
                        };

                        List<RequestInsertSettlementInfo> request = JsonConvert.DeserializeObject<List<RequestInsertSettlementInfo>>(JsonConvert.SerializeObject(rwmSearch.Data.RPReleaseCyberPayResultModel));

                        req.SettlementInfo = new List<RequestInsertSettlementInfo>();
                        req.SettlementInfo.AddRange(request);

                        cbpModel.SettlementStatus = req.SettlementInfo[0].SettlementStatus;

                        apiEx.InterfaceCyberPay.InsertSettlementInfo(req, p =>
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
                    api.RPReleaseCyberPay.Update(cbpModel, p =>
                    {
                        rwmUpdate = p;
                    });

                    LogInOutModel logInOutModel = new LogInOutModel();
                    logInOutModel.module_name = "GenReleaseMessageInterestMargin";
                    logInOutModel.action_name = model.payment_method;
                    logInOutModel.svc_req = model.mt_code;
                    logInOutModel.guid = Guid.NewGuid().ToString();
                    logInOutModel.svc_res = model.brp_contract_no;
                    if (model.call_date != null)
                    {
                        logInOutModel.ref_id = model.call_date.Value.ToString("yyyyMMdd") + "_" + model.counter_party_id + "_" + model.cur + "_Int";
                    }
                    else
                    {
                        logInOutModel.ref_id = DateTime.Now.ToString("yyyyMMdd") + "_" + model.counter_party_id + "_" + model.cur + "_Int";
                    }
                    apiStatic.LogInOut.Add(logInOutModel, p => { });
                }
            }
            catch (Exception ex)
            {
                StrMsg = ex.Message;
                returnCode = false;
                Log.WriteLog(Controller, "Error : " + ex.Message);
            }
            finally
            {
                Log.WriteLog(Controller, "End ReleaseMessage ==========");
            }
            return Json(new { Message = StrMsg, returnCode = returnCode }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult GenExcel(string data)
        {
            string StrMsg = string.Empty;
            var Result = new List<object>();
            ResultWithModel<RPCallMarginResult> ResultModel = new ResultWithModel<RPCallMarginResult>();
            RPCallMarginResult RPCallMarginModel = new RPCallMarginResult();
            string filename = "";
            try
            {
                // Step 1 : Select Trans
                List<Data> ListTrans = new List<Data>();
                ListTrans = JsonConvert.DeserializeObject<List<Data>>(data);

                // Step 2 : Export Excel XLS
                DataTable Dt_Export = new DataTable();
                ExcelEntity ExcelEnt = new ExcelEntity();

                Dt_Export = ToDataTable(ListTrans);
                ExcelEnt.FileName = "MarginInterest.xls";
                ExcelEnt.SheetName = "MarginInterest";
                filename = "MarginInterest.xls";

                //save the file to server temp folder
                string importexportpath = ConfigurationManager.AppSettings["ImportExportPath"].ToString();
                string fullPath = Path.Combine(Server.MapPath(importexportpath), ExcelEnt.FileName);

                HSSFWorkbook HssWorkbook = new HSSFWorkbook();
                var sheet = HssWorkbook.CreateSheet(ExcelEnt.SheetName);

                //===== Create Column Name Style
                ICellStyle Column_Name_Style = HssWorkbook.CreateCellStyle();
                IFont FontStyle = HssWorkbook.CreateFont();
                FontStyle.Boldweight = (short)FontBoldWeight.Bold;
                FontStyle.FontName = "Tahoma";
                Column_Name_Style.SetFont(FontStyle);
                Column_Name_Style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                Column_Name_Style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                Column_Name_Style.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                Column_Name_Style.ShrinkToFit = false;
                //Column_Name_Style.BorderBottom = BorderStyle.

                //===== Create Detail Style
                ICellStyle Detail_CellStyle = HssWorkbook.CreateCellStyle();
                FontStyle = HssWorkbook.CreateFont();
                FontStyle.FontHeightInPoints = 8;
                FontStyle.FontName = "Tahoma";
                Detail_CellStyle.SetFont(FontStyle);
                Detail_CellStyle.WrapText = true;

                ICellStyle Detail_Number_CellStyle = HssWorkbook.CreateCellStyle();
                FontStyle = HssWorkbook.CreateFont();
                FontStyle.FontHeightInPoints = 8;
                Detail_Number_CellStyle.SetFont(FontStyle);
                Detail_Number_CellStyle.DataFormat = HssWorkbook.CreateDataFormat().GetFormat("#,##0");

                ICellStyle Detail_2Decimal_CellStyle = HssWorkbook.CreateCellStyle();
                FontStyle = HssWorkbook.CreateFont();
                FontStyle.FontHeightInPoints = 8;
                Detail_2Decimal_CellStyle.SetFont(FontStyle);
                Detail_2Decimal_CellStyle.DataFormat = HssWorkbook.CreateDataFormat().GetFormat("#,##0.00");


                //===== Add Header Columns
                int rowIndex = 1;
                IRow ExcelRow = sheet.CreateRow(rowIndex);

                int iCol = 1;
                foreach (DataColumn Column in Dt_Export.Columns)
                {
                    ExcelRow.CreateCell(iCol).SetCellValue(Column.ColumnName);
                    ExcelRow.GetCell(iCol).CellStyle = Column_Name_Style;
                    sheet.SetColumnWidth(iCol, 5000);
                    iCol += 1;
                }
                rowIndex = rowIndex + 1;
                ExcelRow = sheet.CreateRow(rowIndex);

                //===== Add Detail
                for (int i = 0; i < Dt_Export.Rows.Count; i++)
                {
                    ExcelRow.CreateCell(1).SetCellValue(Dt_Export.Rows[i][0].ToString());
                    ExcelRow.GetCell(1).CellStyle = Detail_CellStyle;

                    ExcelRow.CreateCell(2).SetCellValue(Dt_Export.Rows[i][1].ToString());
                    ExcelRow.GetCell(2).CellStyle = Detail_CellStyle;

                    ExcelRow.CreateCell(3).SetCellValue(Dt_Export.Rows[i][2].ToString());
                    ExcelRow.GetCell(3).CellStyle = Detail_CellStyle;

                    ExcelRow.CreateCell(4).SetCellValue(Dt_Export.Rows[i][3].ToString());
                    ExcelRow.GetCell(4).CellStyle = Detail_CellStyle;

                    ExcelRow.CreateCell(5).SetCellValue(Dt_Export.Rows[i][4].ToString());
                    ExcelRow.GetCell(5).CellStyle = Detail_CellStyle;

                    ExcelRow.CreateCell(6).SetCellValue(Dt_Export.Rows[i][5].ToString());
                    ExcelRow.GetCell(6).CellStyle = Detail_CellStyle;

                    rowIndex = rowIndex + 1;
                    ExcelRow = sheet.CreateRow(rowIndex);
                }

                if (System.IO.File.Exists(fullPath) == true)
                    System.IO.File.Delete(fullPath);
                FileStream FileData = new FileStream(fullPath, FileMode.Create);
                HssWorkbook.Write(FileData);
                FileData.Close();
            }
            catch (Exception ex)
            {
                StrMsg = ex.Message;
                Log.WriteLog(Controller, "Error : " + ex.Message);
            }
            return Json(new { fileName = filename, errorMessage = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult GenPDF(string data)
        {
            string StrMsg = string.Empty;
            var Result = new List<object>();
            ResultWithModel<RPCallMarginResult> ResultModel = new ResultWithModel<RPCallMarginResult>();
            RPCallMarginResult RPCallMarginModel = new RPCallMarginResult();
            string filename = "";
            try
            {
                // Step 1 : Select Trans
                List<Data> ListTrans = new List<Data>();
                ListTrans = JsonConvert.DeserializeObject<List<Data>>(data);

                // Step 2 : Export Excel XLS
                DataTable Dt_Export = new DataTable();
                ExcelEntity ExcelEnt = new ExcelEntity();

                Dt_Export = ToDataTable(ListTrans);
                ExcelEnt.FileName = "MarginInterest.xls";
                ExcelEnt.SheetName = "MarginInterest";
                filename = "MarginInterest.pdf";

                //save the file to server temp folder
                string importexportpath = ConfigurationManager.AppSettings["ImportExportPath"].ToString();
                string fullPath = Path.Combine(Server.MapPath(importexportpath), ExcelEnt.FileName);

                HSSFWorkbook HssWorkbook = new HSSFWorkbook();
                var sheet = HssWorkbook.CreateSheet(ExcelEnt.SheetName);

                //===== Create Column Name Style
                ICellStyle Column_Name_Style = HssWorkbook.CreateCellStyle();
                IFont FontStyle = HssWorkbook.CreateFont();
                FontStyle.Boldweight = (short)FontBoldWeight.Bold;
                FontStyle.FontName = "Tahoma";
                Column_Name_Style.SetFont(FontStyle);
                Column_Name_Style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                Column_Name_Style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                Column_Name_Style.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                Column_Name_Style.ShrinkToFit = false;
                //Column_Name_Style.BorderBottom = BorderStyle.

                //===== Create Detail Style
                ICellStyle Detail_CellStyle = HssWorkbook.CreateCellStyle();
                FontStyle = HssWorkbook.CreateFont();
                FontStyle.FontHeightInPoints = 8;
                FontStyle.FontName = "Tahoma";
                Detail_CellStyle.SetFont(FontStyle);
                Detail_CellStyle.WrapText = true;

                ICellStyle Detail_Number_CellStyle = HssWorkbook.CreateCellStyle();
                FontStyle = HssWorkbook.CreateFont();
                FontStyle.FontHeightInPoints = 8;
                Detail_Number_CellStyle.SetFont(FontStyle);
                Detail_Number_CellStyle.DataFormat = HssWorkbook.CreateDataFormat().GetFormat("#,##0");

                ICellStyle Detail_2Decimal_CellStyle = HssWorkbook.CreateCellStyle();
                FontStyle = HssWorkbook.CreateFont();
                FontStyle.FontHeightInPoints = 8;
                Detail_2Decimal_CellStyle.SetFont(FontStyle);
                Detail_2Decimal_CellStyle.DataFormat = HssWorkbook.CreateDataFormat().GetFormat("#,##0.00");


                //===== Add Header Columns
                int rowIndex = 1;
                IRow ExcelRow = sheet.CreateRow(rowIndex);

                int iCol = 1;
                foreach (DataColumn Column in Dt_Export.Columns)
                {
                    ExcelRow.CreateCell(iCol).SetCellValue(Column.ColumnName);
                    ExcelRow.GetCell(iCol).CellStyle = Column_Name_Style;
                    sheet.SetColumnWidth(iCol, 5000);
                    iCol += 1;
                }
                rowIndex = rowIndex + 1;
                ExcelRow = sheet.CreateRow(rowIndex);

                //===== Add Detail
                for (int i = 0; i < Dt_Export.Rows.Count; i++)
                {
                    ExcelRow.CreateCell(1).SetCellValue(Dt_Export.Rows[i][0].ToString());
                    ExcelRow.GetCell(1).CellStyle = Detail_CellStyle;

                    ExcelRow.CreateCell(2).SetCellValue(Dt_Export.Rows[i][1].ToString());
                    ExcelRow.GetCell(2).CellStyle = Detail_CellStyle;

                    ExcelRow.CreateCell(3).SetCellValue(Dt_Export.Rows[i][2].ToString());
                    ExcelRow.GetCell(3).CellStyle = Detail_CellStyle;

                    ExcelRow.CreateCell(4).SetCellValue(Dt_Export.Rows[i][3].ToString());
                    ExcelRow.GetCell(4).CellStyle = Detail_CellStyle;

                    ExcelRow.CreateCell(5).SetCellValue(Dt_Export.Rows[i][4].ToString());
                    ExcelRow.GetCell(5).CellStyle = Detail_CellStyle;

                    ExcelRow.CreateCell(6).SetCellValue(Dt_Export.Rows[i][5].ToString());
                    ExcelRow.GetCell(6).CellStyle = Detail_CellStyle;

                    rowIndex = rowIndex + 1;
                    ExcelRow = sheet.CreateRow(rowIndex);
                }

                if (System.IO.File.Exists(fullPath) == true)
                    System.IO.File.Delete(fullPath);
                FileStream FileData = new FileStream(fullPath, FileMode.Create);
                HssWorkbook.Write(FileData);
                FileData.Close();

                Workbook workbook = new Workbook();
                workbook.LoadFromFile(fullPath);
                workbook.SaveToFile(fullPath.Replace("xls", "pdf"), Spire.Xls.FileFormat.PDF);
            }
            catch (Exception ex)
            {
                StrMsg = ex.Message;
                Log.WriteLog(Controller, "Error : " + ex.Message);
            }
            return Json(new { fileName = filename, errorMessage = StrMsg }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Download(string filename)
        {
            //get the temp folder and file path in server
            string importexportpath = ConfigurationManager.AppSettings["ImportExportPath"].ToString();
            string fullPath = Path.Combine(Server.MapPath(importexportpath), filename);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/vnd.ms-excel", filename);
        }

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult DownloadPDF(string filename)
        {
            //get the temp folder and file path in server
            string importexportpath = ConfigurationManager.AppSettings["ImportExportPath"].ToString();
            string fullPath = Path.Combine(Server.MapPath(importexportpath), filename);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/pdf", filename);
        }

        private DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Defining type of data column gives proper data table 
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //Get all the properties
            foreach (PropertyInfo prop in Props)
            {
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                foreach (var item in prop.CustomAttributes)
                {
                    if (item.AttributeType.Name == "DisplayAttribute")
                    {
                        //Setting column names as Property names
                        dataTable.Columns.Add(item.NamedArguments[0].TypedValue.Value.ToString(), type);
                    }
                }
            }

            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
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
            catch (Exception ex)
            {
                Log.WriteLog(Controller, "Error : " + ex.Message);
                ReturnMsg = ex.Message;
                return false;
            }

            return true;
        }

        public ActionResult FillMarginType()
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            res.Add(new DDLItemModel() { Text = "มี Call Margin", Value = "Y" });
            res.Add(new DDLItemModel() { Text = "ไม่มี Call Margin", Value = "N" });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetMarginTrigger(string category, string item_code)
        {
            try
            {
                decimal marginTrigger = 0;
                api.RPCallMargin.GetMarginTrigger(category, item_code, p =>
                {
                    marginTrigger = Convert.ToDecimal(p.Message);
                });

                return Json(new
                {
                    marginTrigger = marginTrigger,
                    returnCode = 0
                });
            }
            catch (Exception ex)
            {
                Log.WriteLog(Controller, "Error : " + ex.Message);
                return Json(new
                {
                    marginTrigger = 0.00,
                    returnCode = 1,
                    Message = ex.ToString()
                });
            }
        }


        [HttpPost]
        public ActionResult CheckMarginTrigger(string data)
        {
            ResultWithModel<RPCallMarginResult> result = new ResultWithModel<RPCallMarginResult>();
            bool isSuccess = true;
            string message = string.Empty;
            bool isTrigger = false;
            try
            {
                RPCallMarginPRPModel model = new RPCallMarginPRPModel();
                model = JsonConvert.DeserializeObject<RPCallMarginPRPModel>(data);

                api.RPCallMargin.CheckMarginTrigger(model, p =>
                {

                    isTrigger = Convert.ToBoolean(p.Message);

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
                Message = message,
                isTrigger = isTrigger
            });
        }

        [RoleScreen(RoleScreen.VIEW)]
        [HttpPost]
        public ActionResult CheckMarginIntRate(RPCallMarginModel model)
        {
            ResultWithModel<RPCallMarginResult> result = new ResultWithModel<RPCallMarginResult>();
            bool isSuccess = true;
            string message = string.Empty;
            string flag_holiday = "N";
            decimal margin_int_rate_holiday = 0;
            try
            {
                api.RPCallMargin.CheckMarginIntRate(model, p =>
                {
                    result = p;
                });

                if (result.Success && result.Data != null && result.Data.RPCallMarginResultModel != null && result.Data.RPCallMarginResultModel.Count > 0)
                {
                    flag_holiday = result.Data.RPCallMarginResultModel[0].flag_holiday;
                    margin_int_rate_holiday = result.Data.RPCallMarginResultModel[0].margin_int_rate_holiday;
                }
                else
                {
                    message = result.Message;
                    isSuccess = false;
                }
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
                Message = message,
                flag_holiday = flag_holiday,
                margin_int_rate_holiday = margin_int_rate_holiday
            });
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult SyncTransaction(RPCallMarginModel model)
        {
            var Result = new List<object>();
            ResultWithModel<CheckSettlementStatusResponseModel> result = new ResultWithModel<CheckSettlementStatusResponseModel>();

            ResultWithModel<RPCallMarginResult> res = new ResultWithModel<RPCallMarginResult>();
            api.RPCallMargin.RPCallMarginListReleaseMessage(model, p =>
            {
                res = p;
            });

            if (res.Data != null && res.Data.RPCallMarginResultModel != null && res.Data.RPCallMarginResultModel.Count > 0)
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

                    foreach (var item in res.Data.RPCallMarginResultModel)
                    {
                        RequestDetail request = new RequestDetail
                        {
                            Seq = index++,
                            SourceSystem = "REPO",
                            DealNo = item.trans_no,
                            DealType = "MARGIN",
                            Product = string.Empty,
                            ProductGroup = string.Empty,
                            Ccy = item.cur,
                            MTType = !string.IsNullOrEmpty(item.mt_code) ? item.mt_code.Replace("MT", "") : (item.margin_type.Contains("Pay") ? "202" : "298"),
                            ValueDate = item.call_date.Value.ToString("yyyyMMdd")
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
                                var deal = res.Data.RPCallMarginResultModel.Find(x => x.trans_no == item.DealNo);
                                if (deal != null)
                                {

                                    RPReleaseCyberPayModel cbpModel = new RPReleaseCyberPayModel();
                                    cbpModel.DealNo = deal.trans_no;
                                    cbpModel.DealType = "MARGIN";
                                    cbpModel.MTType = deal.mt_code.Replace("MT", "");
                                    cbpModel.ValueDate = deal.call_date.Value.ToString("yyyyMMdd");
                                    cbpModel.counter_party_id = deal.counter_party_id.ToString();
                                    cbpModel.ccy = deal.cur;
                                    cbpModel.Seq = 1;
                                    cbpModel.create_by = deal.create_by;
                                    cbpModel.SettlementStatus = (int)SettlementStatus.Complete;

                                    ResultWithModel<RPReleaseCyberPayResult> rwmUpdate = new ResultWithModel<RPReleaseCyberPayResult>();
                                    api.RPReleaseCyberPay.Update(cbpModel, p =>
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
        public ActionResult SyncTransactionInt(RPCallMarginModel model)
        {
            var Result = new List<object>();
            ResultWithModel<CheckSettlementStatusResponseModel> result = new ResultWithModel<CheckSettlementStatusResponseModel>();

            ResultWithModel<RPCallMarginResult> res = new ResultWithModel<RPCallMarginResult>();
            api.RPCallMargin.RPInterestMarginListReleaseMessage(model, p =>
            {
                res = p;
            });

            if (res.Data != null && res.Data.RPCallMarginResultModel != null && res.Data.RPCallMarginResultModel.Count > 0)
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

                    foreach (var item in res.Data.RPCallMarginResultModel)
                    {
                        RequestDetail request = new RequestDetail
                        {
                            Seq = index++,
                            SourceSystem = "REPO",
                            DealNo = item.trans_no,
                            DealType = "MARGIN",
                            Product = string.Empty,
                            ProductGroup = string.Empty,
                            Ccy = item.cur,
                            MTType =  "202" ,
                            ValueDate = item.int_rec_pay_date.Value.ToString("yyyyMMdd")
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
                                var deal = res.Data.RPCallMarginResultModel.Find(x => x.trans_no == item.DealNo);
                                if (deal != null)
                                {

                                    RPReleaseCyberPayModel cbpModel = new RPReleaseCyberPayModel();
                                    cbpModel.DealNo = deal.trans_no;
                                    cbpModel.DealType = "MARGIN";
                                    cbpModel.MTType = deal.mt_code.Replace("MT", "");
                                    cbpModel.ValueDate = deal.call_date.Value.ToString("yyyyMMdd");
                                    cbpModel.counter_party_id = deal.counter_party_id.ToString();
                                    cbpModel.ccy = deal.cur;
                                    cbpModel.Seq = 1;
                                    cbpModel.create_by = deal.create_by;
                                    cbpModel.SettlementStatus = (int)SettlementStatus.Complete;

                                    ResultWithModel<RPReleaseCyberPayResult> rwmUpdate = new ResultWithModel<RPReleaseCyberPayResult>();
                                    api.RPReleaseCyberPay.Update(cbpModel, p =>
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
    }
}