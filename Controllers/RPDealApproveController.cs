using GM.CommonLibs;
using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.ExternalInterface;
using GM.Data.Model.PaymentProcess;
using GM.Data.Model.RPTransaction;
using GM.Data.Model.Static;
using GM.Data.Result.ExternalInterface;
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
    public class RPDealApproveController : BaseController
    {
        RPTransEntity api_RPTrans = new RPTransEntity();
        StaticEntities staticEntities = new StaticEntities();
        Utility utility = new Utility();
        private static string controller = "RPDealApproveController";
        private static LogFile Log = new LogFile();

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            var RPDealApproveView = new RPDealEntryViewModel();
            RPTransModel RPTrans = new RPTransModel();

            RPDealApproveView.FormSearch = RPTrans;

            return View(RPDealApproveView);
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

                api_RPTrans.RPDealApprove.GetRPDealApproveList(RPTransModel, p =>
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

                api_RPTrans.RPDealApprove.GetRPDealApproveDetail(RPTransModel, p =>
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
                    case "FO-APPROVE":
                        {
                            RPTransModel.trans_state_style = "label-deal-new";
                            break;
                        }
                    case "BO-APPROVE":
                        {
                            RPTransModel.trans_state_style = "label-deal-accept";
                            break;
                        }
                    case "BO-REJECTAPPROVE":
                        {
                            RPTransModel.trans_state_style = "label-deal-unaccept";
                            break;
                        }
                    case "BO-REJECTSETTLEMENT":
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
                if (RPTransModel.trans_state.ToUpper() == "FO-APPROVE" || RPTransModel.trans_state.ToUpper() == "BO-REJECTSETTLEMENT" || RPTransModel.trans_state.ToUpper() == "BO-REJECTAPPROVE")
                {
                    RPTransModel.btn_Approve = true;
                    RPTransModel.btn_UnApprove = true;
                    return View(RPTransModel);
                }
                else if (RPTransModel.trans_state.ToUpper() == "BO-APPROVE")
                {
                    RPTransModel.btn_Approve = false;
                    RPTransModel.btn_UnApprove = true;
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

                api_RPTrans.RPDealApprove.GetRPDealApproveColateralList(RPTransModel, p =>
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
                if(Session["ListTrans"] != null && Session["ListTrans"].ToString() != string.Empty)
                {
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
                return RedirectToAction("Index");
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

            return RedirectToAction("Approve", new { trans_no = Previous_TransNo });
        }

        public ActionResult Next(string id)
        {
            string Next_TransNo = string.Empty;
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
                    Next_TransNo = ListTrans[i + 1];
                    break;
                }
            }

            return RedirectToAction("Approve", new { trans_no = Next_TransNo });
        }

        [HttpPost]
        public ActionResult Approve_Trans(RPTransModel model)
        {
            string StrMsg = string.Empty;
            string NextTransNo = string.Empty;
            var Result = new List<object>();
            bool IsPayment = false;
            bool isSuccess = true;

            try
            {
                ResultWithModel<RPTransResult> rwm = new ResultWithModel<RPTransResult>();
                //Step 1 : Approve TransNo
                model.update_by = HttpContext.User.Identity.Name;
                model.trans_state = "APPROVE";

                api_RPTrans.RPDealApprove.UpdateRPDealApprove(model, p =>
                {
                    rwm = p;
                });

                if (!rwm.Success)
                {
                    throw new Exception(rwm.Message);
                }

                //Step 2 : Remove TransNo Approve = [Success] From ListTrans                
                if (Remove_TransNoFromList(ref StrMsg, ref NextTransNo, model) == false)
                {
                    throw new Exception("Remove_TransNoFromList() => " + StrMsg);
                }

                //Step 3 : send confirmation
                #region send confirmation
                ResultWithModel<RpConfigResult> resultRpconfig = new ResultWithModel<RpConfigResult>();
                StaticEntities staticEnt = new StaticEntities();
                List<RpConfigModel> rpConfigModelList = new List<RpConfigModel>();

                staticEnt.RpConfig.GetRpConfig("RP_CCM_INTERFACE", string.Empty, p =>
                {
                    resultRpconfig = p;
                });

                if (resultRpconfig.RefCode != 0)
                {
                    throw new Exception("GetRpConfig() => [" + resultRpconfig.RefCode.ToString() + "]" + resultRpconfig.Message);
                }

                rpConfigModelList = resultRpconfig.Data.RpConfigResultModel;
                var rpConfig = rpConfigModelList.FirstOrDefault(a => a.item_code == "ENABLE");
                if (rpConfig != null && rpConfig.item_value == "Y")
                {
                    ExternalInterfaceEntities interfaceEnt = new ExternalInterfaceEntities();

                    //select RP Confirmation
                    DateTime now = DateTime.Now;
                    ResultWithModel<InterfaceConfirmationResult> rwmCCM = new ResultWithModel<InterfaceConfirmationResult>();
                    InterfaceCCMSearch ccmSearch = new InterfaceCCMSearch();
                    ccmSearch.search_date = DateTime.Today;
                    ccmSearch.search_trans_no = model.trans_no;

                    //Add Paging
                    PagingModel paging = new PagingModel();
                    paging.PageNumber = 0;
                    paging.RecordPerPage = 99999;
                    ccmSearch.paging = paging;

                    //Add Orderby
                    var orders = new List<OrderByModel>();
                    ccmSearch.ordersby = orders;

                    interfaceEnt.InterfaceConfirmation.GetInterfaceCCMList(ccmSearch, p =>
                    {
                        rwmCCM = p;
                    });

                    ResultWithModel<InterfaceConfirmationResult> res = new ResultWithModel<InterfaceConfirmationResult>();
                    if (rwmCCM.Success && rwmCCM.Data.InterfaceConfirmationResultModel.Count > 0)
                    {
                        InterfaceConfirmationModel infCmmModel = rwmCCM.Data.InterfaceConfirmationResultModel[0];
                        infCmmModel.TransDate = now.ToString("yyyyMMdd");
                        infCmmModel.TransTime = now.ToString("HH:mm:ss");
                        infCmmModel.guid = Guid.NewGuid().ToString();
                        infCmmModel.create_by = User.UserId;
                        infCmmModel.RpConfigModel = rpConfigModelList;

                        interfaceEnt.InterfaceConfirmation.SendConfirmation(infCmmModel, p =>
                        {
                            res = p;
                        });

                        //if (!res.Success)
                        //{
                        //    throw new Exception(res.Message);
                        //}
                    }
                }
                #endregion

                //if (!CheckReleaseMessageConfirm(ref StrMsg, ref IsPayment, model))
                //{
                //    Log.WriteLog(controller, StrMsg);
                //    throw new Exception("Check_ReleaseMessageConfirm() => " + StrMsg);
                //}

                ////Step 5 : ReleaseMessage Confirm
                //if (IsPayment && model.repo_deal_type == "BRP" && model.trans_deal_type == "LD")
                //{
                //    if (!ReleaseMessageConfirm(ref StrMsg, model))
                //    {
                //        Log.WriteLog(controller, StrMsg);
                //        throw new Exception("But Release Message Fail => " + StrMsg);
                //    }
                //}
            }
            catch (Exception Ex)
            {
                StrMsg = Ex.Message;
                isSuccess = false;
            }

            Result.Add(new { Message = StrMsg, trans_no = NextTransNo, success = isSuccess });
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UnApprove_Trans(RPTransModel RPTransModel)
        {
            string StrMsg = string.Empty;
            string NextTransNo = string.Empty;
            var Result = new List<object>();

            try
            {
                ResultWithModel<RPTransResult> rwm = new ResultWithModel<RPTransResult>();
                //Step 1 : UnApprove TransNo
                RPTransModel.update_by = HttpContext.User.Identity.Name;
                RPTransModel.trans_state = "UNAPPROVE";

                api_RPTrans.RPDealApprove.UpdateRPDealApprove(RPTransModel, p =>
                {
                    rwm = p;
                });

                if (!rwm.Success)
                {
                    throw new Exception(rwm.Message);
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

            Result.Add(new { Message = StrMsg, trans_no = NextTransNo });
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public bool Remove_TransNoFromList(ref string ReturnMsg, ref string Next_TransNo, RPTransModel RPTransModel)
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
                                Next_TransNo = ListTrans[NextIndex];
                            }
                            else
                            {
                                if (ListTrans.Count > 1)
                                {
                                    Next_TransNo = ListTrans[0];
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
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        private bool CheckReleaseMessageConfirm(ref string ReturnMsg, ref bool IsPayment, RPTransModel model)
        {
            try
            {
                PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();
                ResultWithModel<RPReleaseMsgCheckPaymentResult> Result = new ResultWithModel<RPReleaseMsgCheckPaymentResult>();
                RPReleaseMsgCheckPaymentModel ChkPaymentModel = new RPReleaseMsgCheckPaymentModel();

                ChkPaymentModel.from_page = "Settlement";
                ChkPaymentModel.event_type = "CONFIRM";
                ChkPaymentModel.trans_deal_type = model.trans_deal_type;
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

                Log.WriteLog(controller, "SFTP OutEnt / BackOutEnt");
                Log.WriteLog(controller, "RemoteServerName = " + SftpBackOutEnt.RemoteServerName);
                Log.WriteLog(controller, "RemotePort = " + SftpBackOutEnt.RemotePort);
                Log.WriteLog(controller, "RemoteServerPath = " + SftpOutEnt.RemoteServerPath);
                Log.WriteLog(controller, "RemoteServerPath = " + SftpBackOutEnt.RemoteServerPath);
                Log.WriteLog(controller, "RemoteSshHostKeyFingerprint = " + SftpBackOutEnt.RemoteSshHostKeyFingerprint);
                Log.WriteLog(controller, "RemoteSshPrivateKeyPath = " + SftpBackOutEnt.RemoteSshPrivateKeyPath);
                Log.WriteLog(controller, "NoOfFailRetry File = " + SftpBackOutEnt.NoOfFailRetry);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        private bool ReleaseMessageConfirm(ref string ReturnMsg, RPTransModel model)
        {
            try
            {
                RPTransModel modelConfirm = new RPTransModel();
                modelConfirm.trans_no = model.trans_no;
                modelConfirm.cur = model.cur;
                modelConfirm.event_type = "CONFIRM";
                modelConfirm.message_type = "CONFIRM";
                modelConfirm.create_by = HttpContext.User.Identity.Name;
                modelConfirm.payment_method = "MT518";
                Log.WriteLog(controller, "Start ReleaseMessage Confirm Settlement ==========");
                Log.WriteLog(controller, "- trans_no = " + model.trans_no);
                Log.WriteLog(controller, "- cur = " + model.cur);


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

                //Step 2 : StreamWriter File
                Log.WriteLog(controller, "Write File ReleaseMsg Confirm");
                RPReleaseMessageModel ReleaseMsg = new RPReleaseMessageModel();
                ReleaseMsg = result.Data.RPReleaseMessageResultModel[0];

                FileEntity FileEnt = new FileEntity();
                FileEnt.FileName = ReleaseMsg.file_name.Replace("/", "_");
                FileEnt.FilePath = Server.MapPath(ReleaseMsg.file_path);
                FileEnt.Values = ReleaseMsg.result;

                Log.WriteLog(controller, "- FileName = " + FileEnt.FileName);
                Log.WriteLog(controller, "- FilePath = " + FileEnt.FilePath);

                WriteFile ObjWriteFile = new WriteFile();
                if (ObjWriteFile.StreamWriter(ref FileEnt) == false)
                {
                    throw new Exception("StreamWriter() => " + FileEnt.Msg);
                }
                Log.WriteLog(controller, "Write File = Success.");

                //Step 3 : SFTP File
                Log.WriteLog(controller, "ReleaseMsg To SFTP");
                string StrMsg = string.Empty;
                SftpEntity SftpOutEnt = new SftpEntity();
                SftpEntity SftpBackOutEnt = new SftpEntity();
                if (!SearchConfigReleaseMsgConfirm(ref StrMsg, ref SftpOutEnt, ref SftpBackOutEnt))
                {
                    throw new Exception("Search_ConfigReleaseMsg() => " + FileEnt.Msg);
                }
                Log.WriteLog(controller, "Read Config = Success.");

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
                        Log.WriteLog(controller, "- SFTP " + SftpOutEnt.RemoteServerPath + " " + FileSuccess.ToString() + " Success.");
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
                        Log.WriteLog(controller, "- SFTP " + SftpBackOutEnt.RemoteServerPath + " " + FileSuccess.ToString() + " Success.");
                    }
                }
                else
                {
                    Log.WriteLog(controller, "Release Message Confirm [Disable]");
                }
                ReturnMsg = " & Release Message Confirm Successfully.";

                LogInOutModel logInOutModel = new LogInOutModel();
                logInOutModel.module_name = "GenReleaseMessage";
                logInOutModel.action_name = "SWIFT";
                logInOutModel.svc_req = "MT518";
                logInOutModel.guid = Guid.NewGuid().ToString();
                logInOutModel.ref_id = model.trans_no;

                staticEntities.LogInOut.Add(logInOutModel, p => { });
            }
            catch (Exception ex)
            {
                Log.WriteLog(controller, $"Release Message Fail : [Trans_No: {model.trans_no}]. {ex.Message}");
                ReturnMsg = "But Release Message Confirm Fail ";
            }
            finally
            {
                Log.WriteLog(controller, "End ReleaseMessage Confirm ==========");
            }
            return true;
        }

    }
}