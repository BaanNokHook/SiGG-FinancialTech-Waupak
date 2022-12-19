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
using GM.Data.Result.Static;
using GM.Data.View.RPTransaction;
using GM.Filters;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{

    [Authorize]
    [Audit]
    public class RPDealMaturityController : BaseController
    {
        RPTransEntity api_RPTrans = new RPTransEntity();
        Utility utility = new Utility();
        private static string Controller = "RPDealMaturityController";
        private static LogFile Log = new LogFile();

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            var RPDealMaturityView = new RPDealEntryViewModel();
            RPTransModel RPTrans = new RPTransModel();

            RPDealMaturityView.FormSearch = RPTrans;

            return View(RPDealMaturityView);
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
                        case "maturity_date":
                            RPTransModel.maturity_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "cur":
                            RPTransModel.cur = column.search.value;
                            break;
                        case "repo_deal_type":
                            RPTransModel.repo_deal_type = column.search.value;
                            break;
                        case "trans_deal_type_name":
                            RPTransModel.trans_deal_type = column.search.value;
                            break;
                        case "trans_type":
                            RPTransModel.trans_type = column.search.value;
                            break;
                        case "port":
                            RPTransModel.port = column.search.value;
                            break;
                        case "purpose":
                            RPTransModel.purpose = column.search.value;
                            break;
                        case "counter_party_name":
                            RPTransModel.counter_party_code = column.search.value;
                            break;
                    }
                });

                api_RPTrans.RPDealMaturity.GetRPDealMaturityList(RPTransModel, p =>
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
                data = result.Data != null ? result.Data.RPTransResultModel : new List<RPTransModel>()
            });
        }

        public ActionResult Select(string id)
        {
            List<string> ListTrans = new List<string>();
            ListTrans = JsonConvert.DeserializeObject<List<string>>(id);
            ListTrans.Sort();
            Session["ListTrans"] = ListTrans;

            return RedirectToAction("Approve", new { trans_no = ListTrans[0] });
        }

        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Approve(string trans_no)
        {
            string StrMsg = string.Empty;
            DataTableAjaxPostModel model = new DataTableAjaxPostModel();
            ResultWithModel<RPTransResult> result = new ResultWithModel<RPTransResult>();
            ResultWithModel<RPTransColateralResult> resultColateral = new ResultWithModel<RPTransColateralResult>();
            RPTransModel RPTransModel = new RPTransModel();
            RPTransColateralModel RPTransColateralModel = new RPTransColateralModel();

            try
            {
                //Step 0 : Check trans_no
                if (string.IsNullOrEmpty(trans_no) == true)
                {
                    return RedirectToAction("Index");
                }

                //Step 1 : Select Detail From [TransNo]
                //Add Paging
                PagingModel paging = new PagingModel();
                RPTransModel.paging = paging;

                //Add Orderby
                var orders = new List<OrderByModel>();
                RPTransModel.ordersby = orders;

                //Add trans_no
                RPTransModel.trans_no = trans_no;

                api_RPTrans.RPDealMaturity.GetRPDealMaturityDetail(RPTransModel, p =>
                {
                    result = p;
                });

                RPTransModel = result.Data.RPTransResultModel[0];

                //Step 2 : Set Enable Btn_PreviousNext
                if (Enable_BtnPreviousNext(ref StrMsg, ref RPTransModel) == false)
                {
                    throw new Exception("Enable_BtnPreviousNext() => " + StrMsg);
                }

                #region Set Label Style By Status

                switch (RPTransModel.trans_status.ToUpper())
                {
                    case "OPEN":
                        {
                            RPTransModel.trans_status_style = "label-deal-new";
                            break;
                        }
                    case "CORRECT":
                        {
                            RPTransModel.trans_status_style = "label-deal-accept";
                            break;
                        }
                    case "CANCEL":
                        {
                            RPTransModel.trans_status_style = "label-deal-unaccept";
                            break;
                        }
                    default:
                        {
                            RPTransModel.trans_status_style = "label-default";
                            break;
                        }
                }

                switch (RPTransModel.trans_state.ToUpper())
                {
                    case "BO-SETTLEMENT":
                        {
                            RPTransModel.trans_state_style = "label-deal-new";
                            break;
                        }
                    case "BO-REJECTSETTLEMENT":
                        {
                            RPTransModel.trans_state_style = "label-deal-unaccept";
                            break;
                        }
                    case "BO-REJECTMATURITY":
                        {
                            RPTransModel.trans_state_style = "label-deal-unaccept";
                            break;
                        }
                    default:
                        {
                            RPTransModel.trans_status_style = "label-default";
                            break;
                        }
                }


                #endregion

                // Step 3 : Check Trans State
                if (RPTransModel.trans_state.ToUpper() == "BO-SETTLEMENT")
                {
                    return View(RPTransModel);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception Ex)
            {
                result.Message = Ex.Message;
            }

            return View(RPTransModel);
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

                api_RPTrans.RPDealMaturity.GetRPDealMaturityColateralList(RPTransModel, p =>
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
                List<string> ListTrans = new List<string>();
                if (Session["ListTrans"] != null && Session["ListTrans"].ToString() != string.Empty)
                {
                    ListTrans = (List<string>)Session["ListTrans"];
                }

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
                return RedirectToAction("Index");
            }

            ListTrans = (List<string>)Session["ListTrans"];

            for (int i = 0; i < ListTrans.Count; i++)
            {
                if (ListTrans[i].Contains(id) == true)
                {
                    Previous_TransNo = ListTrans[i - 1];
                    break;
                }
            }

            return RedirectToAction("Approve", new { trans_no = Previous_TransNo });
        }

        public ActionResult Next(string id)
        {
            string NextTransNo = string.Empty;
            List<string> ListTrans = new List<string>();

            //Step 1 : Check List [TransNo] To Approve
            if (Session["ListTrans"] == null || Session["ListTrans"].ToString() == string.Empty)
            {
                return RedirectToAction("Index");
            }

            ListTrans = (List<string>)Session["ListTrans"];

            for (int i = 0; i < ListTrans.Count; i++)
            {
                if (ListTrans[i].Contains(id) == true)
                {
                    NextTransNo = ListTrans[i + 1];
                    break;
                }
            }

            return RedirectToAction("Approve", new { trans_no = NextTransNo });
        }

        [HttpPost]
        public ActionResult Approve_Trans(RPTransModel RPTransModel)
        {
            bool Status = true;
            string StrMsg = string.Empty;
            string NextTransNo = string.Empty;
            var Result = new List<object>();

            try
            {
                ResultWithModel<RPTransResult> rwm = new ResultWithModel<RPTransResult>();
                //Step 1 : Approve TransNo
                RPTransModel.update_by = HttpContext.User.Identity.Name;
                RPTransModel.trans_state = "APPROVE";
                RPTransModel.payment_method = RPTransModel.payment_method;
                RPTransModel.deal_remark = RPTransModel.deal_remark;

                if (RPTransModel.payment_method_text.Contains("("))
                {
                    RPTransModel.nosto_vosto_code = RPTransModel.payment_method_text.Split('(')[1].Replace(")", "");
                }

                api_RPTrans.RPDealMaturity.UpdateRPDealMaturity(RPTransModel, p =>
                {
                    rwm = p;
                });

                if (rwm.Success == false)
                {
                    Status = false;
                    Log.WriteLog(Controller, rwm.Message);
                    throw new Exception("Approve Fail");
                }

                //Step 2 : Remove TransNo Approve = [Success] From ListTrans                
                if (Remove_TransNoFromList(ref StrMsg, ref NextTransNo, RPTransModel) == false)
                {
                    Log.WriteLog(Controller, StrMsg);
                    throw new Exception("Remove_TransNoFromList() => " + StrMsg);
                }

                #region ReleaseMessage
                //if (RPTransModel.payment_method != "DVP/RVP")
                //{
                //    //Step 3 : Check ReleaseMessage
                //    bool IsPayment = false;
                //    if (Check_ReleaseMessage(ref StrMsg, ref IsPayment, RPTransModel) == false)
                //    {
                //        Log.WriteLog(Controller, StrMsg);
                //        throw new Exception("Check_ReleaseMessage() => " + StrMsg);
                //    }

                //    //Step 4 : ReleaseMessage TransNo
                //    if (IsPayment == true)
                //    {
                //        if (Release_Message(ref StrMsg, RPTransModel) == false)
                //        {
                //            Log.WriteLog(Controller, StrMsg);
                //            throw new Exception("But Release Message Fail => " + StrMsg);
                //        }
                //    }

                //    //StrMsg += " & Release Message Successfully.";
                //}
                #endregion
            }
            catch (Exception Ex)
            {
                //StrMsg = Ex.Message;
                //StrMsg = " But Release Message Fail.";
            }

            Result.Add(new { Success = Status, Message = StrMsg, trans_no = NextTransNo });
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UnApprove_Trans(RPTransModel RPTransModel)
        {
            bool Status = true;
            string StrMsg = string.Empty;
            string NextTransNo = string.Empty;
            var Result = new List<object>();

            try
            {
                ResultWithModel<RPTransResult> rwm = new ResultWithModel<RPTransResult>();
                //Step 1 : UnApprove TransNo
                RPTransModel.update_by = HttpContext.User.Identity.Name;
                RPTransModel.trans_state = "UNAPPROVE";

                api_RPTrans.RPDealMaturity.UpdateRPDealMaturity(RPTransModel, p =>
                {
                    rwm = p;
                });

                if (!rwm.Success)
                {
                    Status = false;
                    throw new Exception("UpdateRPDealSettlement() => " + StrMsg);
                }

                //Step 2 : Remove TransNo UnApprove = [Success] From ListTrans                
                if (Remove_TransNoFromList(ref StrMsg, ref NextTransNo, RPTransModel) == false)
                {
                    throw new Exception("Remove_TransNoFromList() => " + StrMsg);
                }

            }
            catch (Exception Ex)
            {
                StrMsg = Ex.Message;
            }

            Result.Add(new { Success = Status, Message = StrMsg, trans_no = NextTransNo });
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        private bool Check_ReleaseMessage(ref string ReturnMsg, ref bool IsPayment, RPTransModel RPTransModel)
        {
            try
            {
                PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();
                ResultWithModel<RPReleaseMsgCheckPaymentResult> Result = new ResultWithModel<RPReleaseMsgCheckPaymentResult>();
                RPReleaseMsgCheckPaymentModel ChkPaymentModel = new RPReleaseMsgCheckPaymentModel();

                ChkPaymentModel.from_page = "Maturity";
                ChkPaymentModel.event_type = "TRANS";
                ChkPaymentModel.trans_deal_type = RPTransModel.trans_deal_type;
                ChkPaymentModel.payment_method = RPTransModel.payment_method;
                ChkPaymentModel.mt_code = RPTransModel.trans_mt_code;

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

        private bool Release_Message(ref string ReturnMsg, RPTransModel RPTransModel)
        {
            try
            {
                Log.WriteLog(Controller, "Start ReleaseMessage Maturity ==========");
                Log.WriteLog(Controller, "- trans_no = " + RPTransModel.trans_no);
                Log.WriteLog(Controller, "- payment_method = " + RPTransModel.payment_method);
                Log.WriteLog(Controller, "- trans_mt_code = " + RPTransModel.trans_mt_code);
                Log.WriteLog(Controller, "- cur = " + RPTransModel.cur);
                Log.WriteLog(Controller, "- event_type = " + RPTransModel.event_type);

                //Step 1 : ReleaseMessage TransNo
                ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();
                RPTransModel.create_by = HttpContext.User.Identity.Name;
                RPTransModel.event_type = "Maturity";
                PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();

                api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessage(RPTransModel, p =>
                {
                    result = p;
                });

                if (result.Success == false)
                {
                    throw new Exception("GenRPReleaseMessage() => " + result.Message);
                }

                //Step 2 : StreamWriter File
                //Log.WriteLog(Controller, "Write File ReleaseMsg");
                //RPReleaseMessageModel ReleaseMsg = new RPReleaseMessageModel();
                //ReleaseMsg = result.Data.RPReleaseMessageResultModel[0];

                //FileEntity FileEnt = new FileEntity();
                //FileEnt.FileName = ReleaseMsg.file_name.Replace("/", "_");
                //FileEnt.FilePath = Server.MapPath(ReleaseMsg.file_path);
                //FileEnt.Values = ReleaseMsg.result;

                //Log.WriteLog(Controller, "- FileName = " + FileEnt.FileName);
                //Log.WriteLog(Controller, "- FilePath = " + FileEnt.FilePath);

                //WriteFile ObjWriteFile = new WriteFile();
                //if (ObjWriteFile.StreamWriter(ref FileEnt) == false)
                //{
                //    throw new Exception("StreamWriter() => " + FileEnt.Msg);
                //}
                //Log.WriteLog(Controller, "Write File = Success.");

                //Step 3 : SFTP File
                //Log.WriteLog(Controller, "ReleaseMsg To SFTP");
                //string StrMsg = string.Empty;
                //SftpEntity SftpOutEnt = new SftpEntity();
                //SftpEntity SftpBackOutEnt = new SftpEntity();
                //if (Search_ConfigReleaseMsg(ref StrMsg, ref SftpOutEnt, ref SftpBackOutEnt) == false)
                //{
                //    throw new Exception("Search_ConfigReleaseMsg() => " + FileEnt.Msg);
                //}
                //Log.WriteLog(Controller, "Read Config = Success.");

                //if (ReleaseMsg.Enable == "Y")
                //{
                //    // Step 4.1 : Sftp REPO_OUT
                //    ArrayList ListFile = new ArrayList();
                //    ArrayList ListFileSuccess = new ArrayList();
                //    ArrayList ListFileError = new ArrayList();

                //    SftpOutEnt.LocalPath = FileEnt.FilePath;
                //    ListFile.Add(FileEnt.FileName);

                //    SftpUtility ObjectSftp = new SftpUtility();
                //    if (!ObjectSftp.UploadSFTPList(ref StrMsg, SftpOutEnt, ListFile, ref ListFileError, ref ListFileSuccess))
                //    {
                //        throw new Exception("UploadSFTPList() => " + StrMsg);
                //    }

                //    if (ListFileError.Count > 0)
                //    {
                //        throw new Exception("UploadSFTPList() => " + ListFileError[0].ToString());
                //    }

                //    foreach (var FileSuccess in ListFileSuccess)
                //    {
                //        Log.WriteLog(Controller, "- SFTP " + SftpOutEnt.RemoteServerPath + " " + FileSuccess.ToString() + " Success.");
                //    }

                //    // Step 4.2 : Sftp REPO_BACKOUT
                //    ListFile = new ArrayList();
                //    ListFileSuccess = new ArrayList();
                //    ListFileError = new ArrayList();

                //    SftpBackOutEnt.LocalPath = FileEnt.FilePath;
                //    ListFile.Add(FileEnt.FileName);

                //    ObjectSftp = new SftpUtility();
                //    if (!ObjectSftp.UploadSFTPList(ref StrMsg, SftpBackOutEnt, ListFile, ref ListFileError, ref ListFileSuccess))
                //    {
                //        throw new Exception("UploadSFTPList() => " + StrMsg);
                //    }

                //    if (ListFileError.Count > 0)
                //    {
                //        throw new Exception("UploadSFTPList() => " + ListFileError[0].ToString());
                //    }

                //    foreach (var FileSuccess in ListFileSuccess)
                //    {
                //        Log.WriteLog(Controller, "- SFTP " + SftpBackOutEnt.RemoteServerPath + " " + FileSuccess.ToString() + " Success.");
                //    }
                //}
                //else
                //{
                //    Log.WriteLog(Controller, "Release Message [Disable]");
                //}

            }
            catch (Exception Ex)
            {
                ReturnMsg = "But Release Message Fail " + Ex.Message;
            }
            finally
            {
                Log.WriteLog(Controller, "End ReleaseMessage Maturity ==========");
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
                }

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
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
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

                //Log.WriteLog(Controller, "SFTP OutEnt");
                //Log.WriteLog(Controller, "RemoteServerName = " + SftpOutEnt.RemoteServerName);
                //Log.WriteLog(Controller, "RemotePort = " + SftpOutEnt.RemotePort);
                //Log.WriteLog(Controller, "RemoteServerPath = " + SftpOutEnt.RemoteServerPath);
                //Log.WriteLog(Controller, "RemoteSshHostKeyFingerprint = " + SftpOutEnt.RemoteSshHostKeyFingerprint);
                //Log.WriteLog(Controller, "RemoteSshPrivateKeyPath = " + SftpOutEnt.RemoteSshPrivateKeyPath);
                //Log.WriteLog(Controller, "NoOfFailRetry File = " + SftpOutEnt.NoOfFailRetry);

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
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        // Function : Binding DDL

        public ActionResult FillCounterPartyPayment(string counterpartyid)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealMaturity.GetDDLPaymentMethodByCounterPartyID(counterpartyid, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillTransMtCode(string payment_method, string trans_deal_type, string cur, string repo_deal_type)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealMaturity.GetDDLTransMtCode(payment_method, trans_deal_type, cur, repo_deal_type, p =>
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