using GM.CommonLibs;
using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.PaymentProcess;
using GM.Data.Model.Static;
using GM.Data.Result.PaymentProcess;
using GM.Data.Result.Static;
using GM.Filters;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class RPCouponController : BaseController
    {
        PaymentProcessEntities api = new PaymentProcessEntities();
        StaticEntities apiStatic = new StaticEntities();
        GM.Data.Helper.Utility utility = new GM.Data.Helper.Utility();

        private static string Controller = "RPCouponController";
        private static LogFile Log = new LogFile();

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
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
                                rpCouponModel.instrument_id = Convert.ToInt32(column.search.value);
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
                                rpCouponModel.counter_party_id = Convert.ToInt32(column.search.value);
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

                api.RPCoupon.GetRPCouponList(rpCouponModel, p =>
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

        private string WrapStringInQuotes(string input)
        {
            return @"""" + input + @"""";
        }

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Edit(string instrument_code, int instrument_id, string counter_party_code, int counter_party_id, string fund_code, string cur, string trans_deal_type, string payment_date)
        {
            if (instrument_code == "null" && counter_party_code == "null" &&
                fund_code == "null" && payment_date == "null" && cur == "null"
                && trans_deal_type == "null")
            {
                return View("Index");
            }
            else
            {
                ResultWithModel<RPCouponResult> result = new ResultWithModel<RPCouponResult>();
                ResultWithModel<RPCouponDetailResult> resultDetail = new ResultWithModel<RPCouponDetailResult>();
                RPCouponModel model = new RPCouponModel();

                model.instrument_code = instrument_code != "null" ? instrument_code.Trim() : null;
                model.counter_party_code = counter_party_code != "null" ? counter_party_code.Trim() : null;
                model.fund_code = fund_code != "null" ? fund_code.Trim() : null;
                model.cur = cur != "null" ? cur.Trim() : null;
                model.payment_date = DateTime.ParseExact(payment_date, "dd/MM/yyyy", null);
                model.trans_deal_type = trans_deal_type;
                model.mt_code = trans_deal_type == "LD" ? "MT202" : "MT298";
                model.instrument_id = instrument_id;
                model.counter_party_id = counter_party_id;

                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = 1;
                paging.RecordPerPage = 1;
                model.paging = paging;

                api.RPCoupon.CheckRPCoupon(model, p =>
                {
                    result = p;
                });

                if (result.Data.RPCouponResultModel.Count == 0)
                {
                    api.RPCoupon.GetRPCouponGet(model, p =>
                    {
                        result = p;
                    });

                    api.RPCoupon.GetRPCouponDetail(model, p =>
                    {
                        resultDetail = p;
                    });

                    model = result.Data.RPCouponResultModel[0];
                    model.MODE = "";
                }
                else
                {
                    api.RPCoupon.GetRPCouponDetail(model, p =>
                    {
                        resultDetail = p;
                    });

                    model = result.Data.RPCouponResultModel[0];
                    model.MODE = "";
                }

                if (resultDetail.Data != null)
                {
                    model.Port_AFS = resultDetail.Data.RPCouponDetailResultModel != null &&
                            resultDetail.Data.RPCouponDetailResultModel.Count > 0 && resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "AFS").FirstOrDefault() != null
                            ? resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "AFS").FirstOrDefault() : new RPCouponDetailModel();
                    model.Port_HTM = resultDetail.Data.RPCouponDetailResultModel != null &&
                        resultDetail.Data.RPCouponDetailResultModel.Count > 0 && resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "HTM").FirstOrDefault() != null
                        ? resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "HTM").FirstOrDefault() : new RPCouponDetailModel();
                    model.Port_TRD = resultDetail.Data.RPCouponDetailResultModel != null &&
                        resultDetail.Data.RPCouponDetailResultModel.Count > 0 && resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "TRD").FirstOrDefault() != null
                        ? resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "TRD").FirstOrDefault() : new RPCouponDetailModel();

                    model.Port_MEMO_BNK = resultDetail.Data.RPCouponDetailResultModel != null &&
                       resultDetail.Data.RPCouponDetailResultModel.Count > 0 && resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "MEMO-BNK").FirstOrDefault() != null
                       ? resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "MEMO-BNK").FirstOrDefault() : new RPCouponDetailModel();

                    model.Port_MEMO_TRD = resultDetail.Data.RPCouponDetailResultModel != null &&
                       resultDetail.Data.RPCouponDetailResultModel.Count > 0 && resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "MEMO-TRD").FirstOrDefault() != null
                       ? resultDetail.Data.RPCouponDetailResultModel.Where(x => x.port == "MEMO-TRD").FirstOrDefault() : new RPCouponDetailModel();

                    model.unit = model.Port_AFS.unit + model.Port_HTM.unit + model.Port_TRD.unit +
                        model.Port_MEMO_BNK.unit + model.Port_MEMO_TRD.unit;
                    model.interest_amount = model.Port_AFS.interest_amount + model.Port_HTM.interest_amount +
                        model.Port_TRD.interest_amount + model.Port_MEMO_BNK.interest_amount + model.Port_MEMO_TRD.interest_amount;
                    model.interest_amount_adj = model.Port_AFS.interest_amount_adj + model.Port_HTM.interest_amount_adj +
                        model.Port_TRD.interest_amount_adj + model.Port_MEMO_BNK.interest_amount_adj + model.Port_MEMO_TRD.interest_amount_adj;
                    model.wht_int_amount = model.Port_AFS.wht_int_amount + model.Port_HTM.wht_int_amount +
                     model.Port_TRD.wht_int_amount + model.Port_MEMO_BNK.wht_int_amount + model.Port_MEMO_TRD.wht_int_amount;
                    model.wht_int_amount_adj = model.Port_AFS.wht_int_amount_adj + model.Port_HTM.wht_int_amount_adj +
                        model.Port_TRD.wht_int_amount_adj + model.Port_MEMO_BNK.wht_int_amount_adj + model.Port_MEMO_TRD.wht_int_amount_adj;

                }
                return View(model);
            }
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Save(RPCouponModel model)
        {
            ResultWithModel<RPCouponResult> res = new ResultWithModel<RPCouponResult>();
            ResultWithModel<RPReleaseMessageResult> resMassage = new ResultWithModel<RPReleaseMessageResult>();
            var result = new List<object>();
            string StrMsg = string.Empty;
            try
            {
                model.create_by = HttpContext.User.Identity.Name;
                model.counter_party_fund_id = model.counter_party_fund_id == 0 ? null : model.counter_party_fund_id;
                model.payment_date = model.payment_date.Value.Date;
                model.ListPort = new List<RPCouponDetailModel>();

                if (model.Port_AFS.unit > 0)
                {
                    model.Port_AFS.port = "AFS";
                    model.ListPort.Add(model.Port_AFS);
                }

                if (model.Port_HTM.unit > 0)
                {
                    model.Port_HTM.port = "HTM";
                    model.ListPort.Add(model.Port_HTM);
                }

                if (model.Port_TRD.unit > 0)
                {
                    model.Port_TRD.port = "TRD";
                    model.ListPort.Add(model.Port_TRD);
                }

                if (model.Port_MEMO_BNK.unit > 0)
                {
                    model.Port_MEMO_BNK.port = "MEMO-BNK";
                    model.ListPort.Add(model.Port_MEMO_BNK);
                }

                if (model.Port_MEMO_TRD.unit > 0)
                {
                    model.Port_MEMO_TRD.port = "MEMO-TRD";
                    model.ListPort.Add(model.Port_MEMO_TRD);
                }

                model.unit = 0;
                model.interest_amount = 0;
                model.interest_amount_adj = 0;
                model.wht_int_amount = 0;
                model.wht_int_amount_adj = 0;

                foreach (var item in model.ListPort)
                {
                    model.unit += item.unit;
                    model.interest_amount += item.interest_amount;
                    model.interest_amount_adj += item.interest_amount_adj;
                    model.wht_int_amount += item.wht_int_amount;
                    model.wht_int_amount_adj += item.wht_int_amount_adj;
                }

                if (ModelState.IsValid)
                {
                    api.RPCoupon.CreateRPCoupon(model, p =>
                    {
                        res = p;
                    });
                }
                else
                {
                    var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();
                    Models.ForEach(field =>
                    {
                        field.Errors.ToList().ForEach(error =>
                        {
                            res.Message += error.ErrorMessage;
                        });

                    });
                }

                #region Release Message
                //if (res.Message == "")
                //{
                //    api.RPCoupon.CheckRPCoupon(model, p =>
                //    {
                //        model = p.Data.RPCouponResultModel[0];
                //    });
                //    model.event_type = "Coupon";
                //    //RPCouponModel.cur = "";
                //    model.create_by = User.Identity.Name;
                //    api.RPCoupon.GenRPReleaseMessage(model, p =>
                //     {
                //         resMassage = p;
                //     });
                //    if (resMassage.Success)
                //    {
                //        //Step 3 : Write File ReleaseMsg
                //        Log.WriteLog(Controller, "Write File ReleaseMsg");
                //        RPReleaseMessageModel ReleaseMsg = new RPReleaseMessageModel();
                //        ReleaseMsg = resMassage.Data.RPReleaseMessageResultModel[0];
                //        FileEntity FileEnt = new FileEntity();
                //        FileEnt.FileName = ReleaseMsg.file_name.Replace("/", "_");
                //        FileEnt.FilePath = Server.MapPath(ReleaseMsg.file_path);
                //        FileEnt.Values = ReleaseMsg.result;
                //        Log.WriteLog(Controller, "- FileName = " + FileEnt.FileName);
                //        Log.WriteLog(Controller, "- FilePath = " + FileEnt.FilePath);
                //        WriteFile ObjWriteFile = new WriteFile();
                //        if (ObjWriteFile.StreamWriter(ref FileEnt) == false)
                //        {
                //            throw new Exception("StreamWriter() => " + FileEnt.Msg);
                //        }
                //        Log.WriteLog(Controller, "Write File = Success.");
                //        //Step 4 : Sftp File REPO_OUT & REPO_BACKOUT
                //        Log.WriteLog(Controller, "ReleaseMsg To SFTP");
                //        SftpEntity SftpOutEnt = new SftpEntity();
                //        SftpEntity SftpBackOutEnt = new SftpEntity();
                //        if (Search_ConfigReleaseMsg(ref StrMsg, ref SftpOutEnt, ref SftpBackOutEnt) == false)
                //        {
                //            throw new Exception("Search_ConfigReleaseMsg() => " + FileEnt.Msg);
                //        }
                //        Log.WriteLog(Controller, "Read Config = Success.");
                //        if (ReleaseMsg.Enable == "Y")
                //        {
                //            // Step 4.1 : Sftp REPO_OUT
                //            ArrayList ListFile = new ArrayList();
                //            ArrayList ListFileSuccess = new ArrayList();
                //            ArrayList ListFileError = new ArrayList();
                //            SftpOutEnt.LocalPath = FileEnt.FilePath;
                //            ListFile.Add(FileEnt.FileName);
                //            SftpUtility ObjectSftp = new SftpUtility();
                //            if (!ObjectSftp.UploadSFTPList(ref StrMsg, SftpOutEnt, ListFile, ref ListFileError, ref ListFileSuccess))
                //            {
                //                throw new Exception("UploadSFTPList() => " + StrMsg);
                //            }
                //            if (ListFileError.Count > 0)
                //            {
                //                throw new Exception("UploadSFTPList() => " + ListFileError[0].ToString());
                //            }
                //            foreach (var FileSuccess in ListFileSuccess)
                //            {
                //                Log.WriteLog(Controller, "- SFTP Success " + FileSuccess.ToString());
                //            }
                //            // Step 4.2 : Sftp REPO_BACKOUT
                //            ListFile = new ArrayList();
                //            ListFileSuccess = new ArrayList();
                //            ListFileError = new ArrayList();
                //            SftpBackOutEnt.LocalPath = FileEnt.FilePath;
                //            ListFile.Add(FileEnt.FileName);
                //            ObjectSftp = new SftpUtility();
                //            if (!ObjectSftp.UploadSFTPList(ref StrMsg, SftpBackOutEnt, ListFile, ref ListFileError, ref ListFileSuccess))
                //            {
                //                throw new Exception("UploadSFTPList() => " + StrMsg);
                //            }
                //            if (ListFileError.Count > 0)
                //            {
                //                throw new Exception("UploadSFTPList() => " + ListFileError[0].ToString());
                //            }
                //            foreach (var FileSuccess in ListFileSuccess)
                //            {
                //                Log.WriteLog(Controller, "- SFTP Success " + FileSuccess.ToString());
                //            }
                //        }
                //        LogInOutModel logInOutModel = new LogInOutModel();
                //        logInOutModel.module_name = "GenReleaseMessage";
                //        logInOutModel.action_name = model.payment_method;
                //        logInOutModel.svc_req = model.mt_code;
                //        logInOutModel.guid = Guid.NewGuid().ToString();
                //        logInOutModel.ref_id = model.trans_no;
                //        apiStatic.LogInOut.Add(logInOutModel, p => { });
                //    }
                //    else
                //    {
                //        res.Message = "Save Complete And Error : " + resMassage.Message;
                //    }
                //}
                #endregion

                result.Add(new { Message = res.Message });
            }
            catch (Exception ex)
            {
                result.Add(new { Message = ex.Message });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillPaymentMethod(string transdealtype)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api.RPCoupon.GetDDLPaymentMethod("CNP", transdealtype, p =>
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
            api.RPCoupon.GetDDLMTCode(paymentmethod, transdealtype, "CNP", cur, p =>
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
            api.RPCoupon.DDLCounterParty(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCounterPartyFund(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api.RPCoupon.DDLCounterPartyFund(datastr, p =>
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
            api.RPCoupon.DDLInstrument(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
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

    }
}