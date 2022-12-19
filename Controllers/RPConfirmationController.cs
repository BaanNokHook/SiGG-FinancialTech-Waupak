using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.PaymentProcess;
using GM.Data.Model.RPTransaction;
using GM.Data.Result.PaymentProcess;
using GM.Data.Result.RPTransaction;
using GM.Data.View.RPTransaction;
using GM.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Routing;
using GM.CommonLibs;
using GM.CommonLibs.ClassLibs;
using GM.Data.Model.Static;
using GM.Data.Result.Static;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class RPConfirmationController : BaseController
    {
        RPTransEntity api_RPTrans = new RPTransEntity();
        StaticEntities staticEntities = new StaticEntities();
        Utility utility = new Utility();
        PaymentProcessEntities apiRPReleaseMsg = new PaymentProcessEntities();
        private static string controller = "RPConfirmationController";
        private static LogFile Log = new LogFile();
        private SftpEntity SftpOutEnt = new SftpEntity();
        private SftpEntity SftpBackOutEnt = new SftpEntity();

        // GET: RPConfirmation
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AmendConfirm()
        {
            var viewModel = new RPDealEntryViewModel();
            RPTransModel model = new RPTransModel();

            model.event_type = "AmendConfirmation";
            model.event_type_name = "Amend Confirmation";
            viewModel.FormSearch = model;

            return View("AmendConfirm", viewModel);
        }

        public ActionResult EarlyTermination()
        {
            var viewModel = new RPDealEntryViewModel();
            RPTransModel model = new RPTransModel();

            model.event_type = "EarlyConfirmation";
            model.event_type_name = "Early Terminate Confirmation";
            viewModel.FormSearch = model;

            return View("EarlyTermination", viewModel);
        }

        public ActionResult AmendDeal()
        {
            var viewModel = new RPDealEntryViewModel();
            RPTransModel model = new RPTransModel();

            model.event_type = "AmendDeal";
            model.event_type_name = "Amend Deal";
            viewModel.FormSearch = model;

            return View("AmendDeal", viewModel);
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

                api_RPTrans.RPDealSettlement.GetRPDealConfirmationList(RPTransModel, p =>
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

        [HttpPost]
        public ActionResult SearchConfirm(DataTableAjaxPostModel model)
        {
            ResultWithModel<RPTransResult> result = new ResultWithModel<RPTransResult>();
            RPTransModel transModel = new RPTransModel();
            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                transModel.paging = paging;

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
                transModel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null && o.search.value != "null").ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "from_trans_no":
                            transModel.from_trans_no = column.search.value;
                            break;
                        case "to_trans_no":
                            transModel.to_trans_no = column.search.value;
                            break;
                        case "policy_date":
                            transModel.policy_date = DateTime.ParseExact(column.search.value, "yyyyMMdd",
                                                        CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                            break;
                        case "from_trade_date":
                            transModel.from_trade_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_trade_date":
                            transModel.to_trade_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "from_settlement_date":
                            transModel.from_settlement_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_settlement_date":
                            transModel.to_settlement_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "from_maturity_date":
                            transModel.from_maturity_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_maturity_date":
                            transModel.to_maturity_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "message_type":
                            transModel.message_type = column.search.value;
                            break;
                    }
                });

                apiRPReleaseMsg.RPReleaseMessage.GetConfirmBilateralList(transModel, p =>
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

        [HttpPost]
        public ActionResult SearchEarlyTerminate(DataTableAjaxPostModel model)
        {
            ResultWithModel<RPTransResult> result = new ResultWithModel<RPTransResult>();
            RPTransModel transModel = new RPTransModel();
            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                transModel.paging = paging;

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
                transModel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null && o.search.value != "null").ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "from_trans_no":
                            transModel.from_trans_no = column.search.value;
                            break;
                        case "to_trans_no":
                            transModel.to_trans_no = column.search.value;
                            break;
                    }
                });

                apiRPReleaseMsg.RPReleaseMessage.GetConfirmEarlyList(transModel, p =>
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

        [HttpPost]
        public ActionResult SearchAmendDeal(DataTableAjaxPostModel model)
        {
            ResultWithModel<RPTransResult> result = new ResultWithModel<RPTransResult>();
            RPTransModel transModel = new RPTransModel();
            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                transModel.paging = paging;

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
                transModel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null && o.search.value != "null").ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "from_trans_no":
                            transModel.from_trans_no = column.search.value;
                            break;
                        case "to_trans_no":
                            transModel.to_trans_no = column.search.value;
                            break;
                        case "from_trade_date":
                            transModel.from_trade_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_trade_date":
                            transModel.to_trade_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "from_settlement_date":
                            transModel.from_settlement_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_settlement_date":
                            transModel.to_settlement_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "from_maturity_date":
                            transModel.from_maturity_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_maturity_date":
                            transModel.to_maturity_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "message_type":
                            transModel.message_type = column.search.value;
                            break;
                    }
                });

                apiRPReleaseMsg.RPReleaseMessage.GetConfirmAmendDealList(transModel, p =>
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
        public ActionResult FillUser(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            UserAndScreenEntities api = new UserAndScreenEntities();
            api.User.GetDDlUserLdap(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSignname(RPConfirmationModel model)
        {
            ResultWithModel<RPConfirmationResult> rwm = new ResultWithModel<RPConfirmationResult>();
            PaymentProcessEntities api = new PaymentProcessEntities();

            if (model.arr_trans_no != null && model.arr_trans_no.Length == 1)
            {
                model.trans_no = model.arr_trans_no[0];
                model.recorded_flag = 'T';
            }

            model.create_by = User.UserId;


            api.RPConfirmation.GetSignName(model, p =>
            {
                if (p.Success)
                {
                    rwm = p;
                }
            });

            object result = new { };

            if (rwm.Data != null)
            {
                var data = rwm.Data.RPConfirmationResultModel.FirstOrDefault();
                if (data != null)
                {
                    result = new
                    {
                        data.print_confirm_bo1_by,
                        data.sign_name_1,
                        data.position_name_1,
                        data.print_confirm_bo2_by,
                        data.sign_name_2,
                        data.position_name_2
                    };
                }
                else
                {
                    result = new
                    {
                        print_confirm_bo1_by = "",
                        sign_name_1 = "",
                        position_name_1 = "",
                        print_confirm_bo2_by = "",
                        sign_name_2 = "",
                        position_name_2 = ""
                    };
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateSignName(RPConfirmationModel model)
        {
            ResultWithModel<RPConfirmationResult> rwm = new ResultWithModel<RPConfirmationResult>();

            try
            {
                //update sign name
                if (!string.IsNullOrEmpty(model.print_confirm_bo1_by) || !string.IsNullOrEmpty(model.print_confirm_bo2_by))
                {
                    PaymentProcessEntities api = new PaymentProcessEntities();
                    foreach (var trans_no in model.arr_trans_no)
                    {
                        RPConfirmationModel updateModel = new RPConfirmationModel();
                        updateModel.update_by = User.UserId;
                        updateModel.trans_no = trans_no;
                        updateModel.print_confirm_bo1_by = model.print_confirm_bo1_by;
                        updateModel.print_confirm_bo2_by = model.print_confirm_bo2_by;

                        api.RPConfirmation.UpdateSignName(updateModel, p =>
                        {
                            rwm = p;
                        });
                    }
                }

                return Json(new { Status = "Success" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error" });
            }
        }

        [HttpGet]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult DownloadPDF(string trans_no, string print_confirm_bo1_by, string print_confirm_bo2_by)
        {
            try
            {
                if (string.IsNullOrEmpty(trans_no))
                {
                    return Content("Not Trans No!!!");
                }

                using (WebClient client = new WebClient())
                {
                    //string url = this.Url.Action("DownloadPDF", "RPConfirmationReport",
                    //    new
                    //    {
                    //        trans_no = trans_no,
                    //        print_confirm_bo1_by = print_confirm_bo1_by,
                    //        print_confirm_bo2_by = print_confirm_bo2_by,
                    //        access_token = "repo2019"
                    //    }, this.Request.Url.Scheme);

                    string urlReport = string.Format(PublicConfig.GetValue("Reportpath") + "/RPConfirmationReport/DownloadPDF?trans_no={0}&print_confirm_bo1_by={1}&print_confirm_bo2_by={2}&access_token=repo2019",
                        trans_no, print_confirm_bo1_by, print_confirm_bo2_by);

                    if (urlReport.StartsWith("https://"))
                        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, errors) => true;

                    byte[] byteFile = client.DownloadData(urlReport);
                    return new FileContentResult(byteFile, "application/pdf");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message, "text/html");
            }
        }

        [HttpPost]
        public ActionResult ReleaseMessage(RPConfirmationModel model)
        {
            string StrMsg = string.Empty;
            bool IsPayment = false;
            bool isSuccess = true;

            try
            {
                List<RPTransModel> rpTransModels = new List<RPTransModel>();

                //Get data trans deal
                if (model.arr_trans_no != null)
                {
                    foreach (var transNo in model.arr_trans_no)
                    {
                        ResultWithModel<RPTransResult> result = new ResultWithModel<RPTransResult>();

                        RPTransModel search = new RPTransModel();
                        //Step 1 : Select Detail From [TransNo]
                        search.paging = new PagingModel();
                        search.ordersby = new List<OrderByModel>();
                        search.trans_no = transNo;

                        api_RPTrans.RPDealApprove.GetRPDealApproveDetail(search, p =>
                        {
                            result = p;
                        });

                        if (!result.Success)
                        {
                            throw new Exception("Search trans no." + transNo + " error:" + result.Message);
                        }

                        if (result.Data.RPTransResultModel.Count == 0)
                        {
                            throw new Exception("Trans no." + transNo + " not found.");
                        }

                        rpTransModels.Add(result.Data.RPTransResultModel[0]);
                    }
                }

                //Read Config
                SftpOutEnt = new SftpEntity();
                SftpBackOutEnt = new SftpEntity();
                if (!SearchConfigReleaseMsgConfirm(ref StrMsg, ref SftpOutEnt, ref SftpBackOutEnt))
                {
                    throw new Exception("Search_ConfigReleaseMsg() => " + StrMsg);
                }

                foreach (var rpTransModel in rpTransModels)
                {
                    if (!CheckReleaseMessageConfirm(ref StrMsg, ref IsPayment, rpTransModel))
                    {
                        Log.WriteLog(controller, StrMsg);
                        throw new Exception("Check_ReleaseMessageConfirm() => " + StrMsg);
                    }

                    //ReleaseMessage Confirm
                    if (IsPayment && rpTransModel.repo_deal_type == "BRP" && rpTransModel.trans_deal_type == "LD")
                    {
                        if (!ReleaseMessageConfirm(ref StrMsg, rpTransModel, "MT518"))
                        {
                            Log.WriteLog(controller, StrMsg);
                            throw new Exception("But Release Message Fail => " + StrMsg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StrMsg = ex.Message;
                isSuccess = false;
            }

            return Json(new { Message = StrMsg, success = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult ReleaseMessage298(RPConfirmationModel model)
        {
            string StrMsg = string.Empty;
            bool IsPayment = false;
            bool isSuccess = true;

            try
            {
                List<RPTransModel> rpTransModels = new List<RPTransModel>();

                //Get data trans deal
                if (model.arr_trans_no != null)
                {
                    foreach (var transNo in model.arr_trans_no)
                    {
                        ResultWithModel<RPTransResult> result = new ResultWithModel<RPTransResult>();

                        RPTransModel search = new RPTransModel();
                        //Step 1 : Select Detail From [TransNo]
                        search.paging = new PagingModel();
                        search.ordersby = new List<OrderByModel>();
                        search.trans_no = transNo;

                        api_RPTrans.RPDealApprove.GetRPDealApproveDetail(search, p =>
                        {
                            result = p;
                        });

                        if (!result.Success)
                        {
                            throw new Exception("Search trans no." + transNo + " error:" + result.Message);
                        }

                        if (result.Data.RPTransResultModel.Count == 0)
                        {
                            throw new Exception("Trans no." + transNo + " not found.");
                        }

                        rpTransModels.Add(result.Data.RPTransResultModel[0]);
                    }
                }

                //Read Config
                SftpOutEnt = new SftpEntity();
                SftpBackOutEnt = new SftpEntity();
                if (!SearchConfigReleaseMsgConfirm(ref StrMsg, ref SftpOutEnt, ref SftpBackOutEnt))
                {
                    throw new Exception("Search_ConfigReleaseMsg() => " + StrMsg);
                }

                foreach (var rpTransModel in rpTransModels)
                {

                    //ReleaseMessage Confirm 298
                    if (rpTransModel.repo_deal_type == "PRP")
                    {
                        if (!ReleaseMessageConfirm(ref StrMsg, rpTransModel, "MT298"))
                        {
                            Log.WriteLog(controller, StrMsg);
                            throw new Exception("But Release Message Fail => " + StrMsg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StrMsg = ex.Message;
                isSuccess = false;
            }

            return Json(new { Message = StrMsg, success = isSuccess }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetReleaseMT(RPConfirmationModel model)
        {
            ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();
            RPReleaseMessageResult RPReleaseMessageResultModel = new RPReleaseMessageResult();
            PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();
            try
            {
                ResultWithModel<RPTransResult> resTransModel = new ResultWithModel<RPTransResult>();
                RPTransModel search = new RPTransModel();
                //Step 1 : Select Detail From [TransNo]
                search.paging = new PagingModel();
                search.ordersby = new List<OrderByModel>();
                search.trans_no = model.trans_no;

                api_RPTrans.RPDealApprove.GetRPDealApproveDetail(search, p =>
                {
                    resTransModel = p;
                });

                if (!resTransModel.Success)
                {
                    throw new Exception("Search trans no." + model.trans_no + " error:" + result.Message);
                }

                if (resTransModel.Data.RPTransResultModel.Count == 0)
                {
                    throw new Exception("Trans no." + model.trans_no + " not found.");
                }

                RPTransModel RPTransModel = resTransModel.Data.RPTransResultModel[0];

                //View ReleaseMsg
                result = new ResultWithModel<RPReleaseMessageResult>();
                var orders = new List<OrderByModel>();
                PagingModel paging = new PagingModel();

                paging.PageNumber = 0;
                paging.RecordPerPage = 0;
                RPTransModel.paging = paging;
                RPTransModel.ordersby = orders;

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

        private bool ReleaseMessageConfirm(ref string ReturnMsg, RPTransModel model, string mt_code = "MT518")
        {
            try
            {
                RPTransModel modelConfirm = new RPTransModel();
                modelConfirm.trans_no = model.trans_no;
                modelConfirm.cur = model.cur;
                modelConfirm.event_type = "CONFIRM";
                modelConfirm.message_type = "CONFIRM";
                modelConfirm.create_by = HttpContext.User.Identity.Name;
                modelConfirm.payment_method = "SWIFT";
                modelConfirm.trans_mt_code = mt_code;
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
                logInOutModel.svc_req = mt_code;
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

        public ActionResult FillEventType(string event_type)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            res.Add(new DDLItemModel() { Text = "Confirmation", Value = "Confirmation" });
            res.Add(new DDLItemModel() { Text = "Amend Confirmation", Value = "AmendConfirmation" });
            res.Add(new DDLItemModel() { Text = "Early Terminate Confirmation", Value = "EarlyConfirmation" });
            res.Add(new DDLItemModel() { Text = "Amend Deal", Value = "AmendDeal" });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillMessageType(string messageType)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            res.Add(new DDLItemModel() { Text = "All", Value = "" });
            res.Add(new DDLItemModel() { Text = "Cancel", Value = "CANCEL" });
            res.Add(new DDLItemModel() { Text = "Confirm", Value = "CONFIRM" });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillPolicyDate(string cur = "THB")
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            apiRPReleaseMsg.RPReleaseMessage.GetDDLPolicyDate(cur, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems != null ?
                    p.Data.DDLItems.OrderByDescending(x => x.Value).ToList()
                    : p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMessageConfirm(string trans_no, string event_type, string mt_code)
        {
            ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();
            RPReleaseMessageResult RPReleaseMessageResultModel = new RPReleaseMessageResult();
            PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();
            try
            {
                RPTransModel model = new RPTransModel()
                {
                    trans_no = trans_no,
                    event_type = event_type
                };

                RPTransModel modelConfirm = new RPTransModel();
                modelConfirm.trans_no = trans_no;
                modelConfirm.cur = "THB";
                modelConfirm.event_type = "CONFIRM";
                modelConfirm.message_type = "CONFIRM";
                modelConfirm.create_by = HttpContext.User.Identity.Name;
                modelConfirm.payment_method = "SWIFT";
                modelConfirm.trans_mt_code = mt_code;

                //Step 1 : ReleaseMessage TransNo
                api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageConfirm(modelConfirm, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception("GenRPReleaseMessage Confirm Fail.");
                }

                //Step 2 : View ReleaseMsg
                result = new ResultWithModel<RPReleaseMessageResult>();
                var orders = new List<OrderByModel>();
                PagingModel paging = new PagingModel();

                paging.PageNumber = 0;
                paging.RecordPerPage = 0;
                model.paging = paging;
                model.ordersby = orders;

                api_RPReleaseMsg.RPReleaseMessage.GetMessageConfirm(model, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result.Data != null ? result.Data.RPReleaseMessageResultModel : new List<RPReleaseMessageModel>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMessageConfirmChange(string trans_no, string event_type, string policy_date, string message_type)
        {
            ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();
            RPReleaseMessageResult RPReleaseMessageResultModel = new RPReleaseMessageResult();
            PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();
            RPTransModel modelConfirm = new RPTransModel();
            RPReleaseMessageModel releaseMsg = new RPReleaseMessageModel();
            try
            {
                if (message_type == "CANCEL")
                {
                    modelConfirm.trans_no = trans_no;
                    modelConfirm.cur = "THB";
                    modelConfirm.event_type = "CANCEL";
                    modelConfirm.message_type = "CANCEL";
                    modelConfirm.create_by = HttpContext.User.Identity.Name;
                    modelConfirm.payment_method = "SWIFT";
                    modelConfirm.trans_mt_code = "MT518";
                    modelConfirm.policy_date = policy_date;

                    //Step 1 : ReleaseMessage TransNo
                    api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageConfirm(modelConfirm, p =>
                    {
                        result = p;
                    });

                    if (!result.Success)
                    {
                        throw new Exception("GenRPReleaseMessage Confirm Fail.");
                    }

                    releaseMsg = result.Data.RPReleaseMessageResultModel[0];
                }


                if (message_type == "" || message_type == "CONFIRM")
                {
                    modelConfirm = new RPTransModel();
                    modelConfirm.trans_no = trans_no;
                    modelConfirm.cur = "THB";
                    modelConfirm.event_type = "CHANGE";
                    modelConfirm.message_type = "CHANGE";
                    modelConfirm.create_by = HttpContext.User.Identity.Name;
                    modelConfirm.payment_method = "SWIFT";
                    modelConfirm.trans_mt_code = "MT518";
                    modelConfirm.policy_date = policy_date;

                    api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageConfirm(modelConfirm, p =>
                    {
                        result = p;
                        if (result.Data != null && result.Data.RPReleaseMessageResultModel.Count > 0)
                        {
                            releaseMsg.result += result.Data.RPReleaseMessageResultModel[0].result;
                        }
                    });

                    if (!result.Success)
                    {
                        throw new Exception("GenRPReleaseMessage Confirm Fail.");
                    }
                }

                //Step 2 : View ReleaseMsg
                result = new ResultWithModel<RPReleaseMessageResult>();

                RPTransModel model = new RPTransModel()
                {
                    trans_no = trans_no,
                    //event_type = event_type // ของเดิมเป็น CHANGE
                    event_type = modelConfirm.message_type,
                    message_type = message_type
                };

                var orders = new List<OrderByModel>();
                PagingModel paging = new PagingModel();

                paging.PageNumber = 0;
                paging.RecordPerPage = 0;
                model.paging = paging;
                model.ordersby = orders;

                api_RPReleaseMsg.RPReleaseMessage.GetMessageConfirm(model, p =>
                {
                    result = p;
                });

                if (result.Data != null && result.Data.RPReleaseMessageResultModel.Count() > 0)
                {
                    int lastIndex = result.Data.RPReleaseMessageResultModel.Count() - 1;
                    result.Data.RPReleaseMessageResultModel[lastIndex].mt_message = result.Data.RPReleaseMessageResultModel[lastIndex].mt_message.Replace("$", "");
                }

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result.Data != null ? result.Data.RPReleaseMessageResultModel : new List<RPReleaseMessageModel>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMessageEarly(string trans_no, string event_type, string policy_date)
        {
            ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();
            RPReleaseMessageResult RPReleaseMessageResultModel = new RPReleaseMessageResult();
            PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();
            try
            {

                RPTransModel modelConfirm = new RPTransModel();
                modelConfirm.trans_no = trans_no;
                modelConfirm.cur = "THB";
                modelConfirm.event_type = "CANCEL";
                modelConfirm.message_type = "CANCEL";
                modelConfirm.create_by = HttpContext.User.Identity.Name;
                modelConfirm.payment_method = "SWIFT";
                modelConfirm.trans_mt_code = "MT518";

                //Step 1 : ReleaseMessage TransNo
                api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageConfirm(modelConfirm, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception("GenRPReleaseMessage Confirm Fail.");
                }

                Log.WriteLog(controller, "Write File ReleaseMsg Confirm");
                RPReleaseMessageModel releaseMsg = new RPReleaseMessageModel();
                releaseMsg = result.Data.RPReleaseMessageResultModel[0];

                modelConfirm = new RPTransModel();
                modelConfirm.trans_no = trans_no;
                modelConfirm.cur = "THB";
                modelConfirm.event_type = "CHANGE-EAR";
                modelConfirm.message_type = "CHANGE-EAR";
                modelConfirm.create_by = HttpContext.User.Identity.Name;
                modelConfirm.payment_method = "SWIFT";
                modelConfirm.trans_mt_code = "MT518";

                api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageConfirm(modelConfirm, p =>
                {
                    result = p;
                    if (result.Data != null && result.Data.RPReleaseMessageResultModel.Count > 0)
                    {
                        releaseMsg.result += result.Data.RPReleaseMessageResultModel[0].result;
                    }
                });

                if (!result.Success)
                {
                    throw new Exception("GenRPReleaseMessage Confirm Fail.");
                }

                //Step 2 : View ReleaseMsg
                result = new ResultWithModel<RPReleaseMessageResult>();

                RPTransModel model = new RPTransModel()
                {
                    trans_no = trans_no,
                    event_type = event_type
                };

                var orders = new List<OrderByModel>();
                PagingModel paging = new PagingModel();

                paging.PageNumber = 0;
                paging.RecordPerPage = 0;
                model.paging = paging;
                model.ordersby = orders;

                api_RPReleaseMsg.RPReleaseMessage.GetMessageConfirm(model, p =>
                {
                    result = p;
                });

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result.Data != null ? result.Data.RPReleaseMessageResultModel : new List<RPReleaseMessageModel>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMessageConfirmAmendDeal(string trans_no, string message_type)
        {
            ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();
            RPReleaseMessageResult RPReleaseMessageResultModel = new RPReleaseMessageResult();
            PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();
            RPTransModel modelConfirm = new RPTransModel();
            RPReleaseMessageModel releaseMsg = new RPReleaseMessageModel();
            try
            {
                if (message_type == "CANCEL")
                {
                    modelConfirm.trans_no = trans_no;
                    modelConfirm.cur = "THB";
                    modelConfirm.event_type = "CANCEL";
                    modelConfirm.message_type = "CANCEL";
                    modelConfirm.create_by = HttpContext.User.Identity.Name;
                    modelConfirm.payment_method = "SWIFT";
                    modelConfirm.trans_mt_code = "MT518";

                    //Step 1 : ReleaseMessage TransNo
                    api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageConfirmAmendDeal(modelConfirm, p =>
                    {
                        result = p;
                    });

                    if (!result.Success)
                    {
                        throw new Exception("GenRPReleaseMessage Confirm Fail.");
                    }

                    releaseMsg = result.Data.RPReleaseMessageResultModel[0];
                }


                if (message_type == "" || message_type == "CONFIRM")
                {
                    modelConfirm = new RPTransModel();
                    modelConfirm.trans_no = trans_no;
                    modelConfirm.cur = "THB";
                    modelConfirm.event_type = "CONFIRM";
                    modelConfirm.message_type = "CONFIRM";
                    modelConfirm.create_by = HttpContext.User.Identity.Name;
                    modelConfirm.payment_method = "SWIFT";
                    modelConfirm.trans_mt_code = "MT518";

                    api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageConfirmAmendDeal(modelConfirm, p =>
                    {
                        result = p;
                        if (result.Data != null && result.Data.RPReleaseMessageResultModel.Count > 0)
                        {
                            releaseMsg.result += result.Data.RPReleaseMessageResultModel[0].result;
                        }
                    });

                    if (!result.Success)
                    {
                        throw new Exception("GenRPReleaseMessage Confirm Fail.");
                    }
                }

                //Step 2 : View ReleaseMsg
                result = new ResultWithModel<RPReleaseMessageResult>();

                RPTransModel model = new RPTransModel()
                {
                    trans_no = trans_no,
                    event_type = message_type == "CANCEL" ? "CANCEL"  : "AMEND",
                    message_type = message_type
                };

                var orders = new List<OrderByModel>();
                PagingModel paging = new PagingModel();

                //paging.PageNumber = 0;
                //paging.RecordPerPage = 0;
                //model.paging = paging;
                //model.ordersby = orders;

                api_RPReleaseMsg.RPReleaseMessage.GetMessageConfirm(model, p =>
                {
                    result = p;
                });

                if (result.Data != null && result.Data.RPReleaseMessageResultModel.Count() > 0)
                {
                    int lastIndex = result.Data.RPReleaseMessageResultModel.Count() - 1;
                    result.Data.RPReleaseMessageResultModel[lastIndex].mt_message = result.Data.RPReleaseMessageResultModel[lastIndex].mt_message.Replace("$", "");
                }

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(result.Data != null ? result.Data.RPReleaseMessageResultModel : new List<RPReleaseMessageModel>(), JsonRequestBehavior.AllowGet);
        }


        private bool ReleaseMessageConfirmChange(ref string returnMsg, string trans_no, string policy_date, string message_type)
        {
            ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();
            PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();
            RPTransModel modelConfirm = new RPTransModel();
            RPReleaseMessageModel releaseMsg = new RPReleaseMessageModel();
            bool isResult = true;
            try
            {
                //Step 1 : ReleaseMessage TransNo

                if (message_type == "CANCEL" || message_type == "CHANGE-EARLY")
                {
                    modelConfirm.trans_no = trans_no;
                    modelConfirm.cur = "THB";
                    modelConfirm.event_type = "CANCEL";
                    modelConfirm.message_type = "CANCEL";
                    modelConfirm.create_by = HttpContext.User.Identity.Name;
                    modelConfirm.payment_method = "SWIFT";
                    modelConfirm.trans_mt_code = "MT518";
                    modelConfirm.policy_date = policy_date;

                    api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageConfirm(modelConfirm, p =>
                    {
                        result = p;
                    });

                    if (!result.Success)
                    {
                        throw new Exception("GenRPReleaseMessage Confirm Fail.");
                    }
                    else
                    {
                        Log.WriteLog(controller, "Write File ReleaseMsg CANCEL");
                    }

                    releaseMsg = result.Data.RPReleaseMessageResultModel[0];

                }

                if (message_type == "CONFIRM")
                {
                    modelConfirm = new RPTransModel();
                    modelConfirm.trans_no = trans_no;
                    modelConfirm.cur = "THB";
                    modelConfirm.event_type = "CHANGE";
                    modelConfirm.message_type = "CHANGE";
                    modelConfirm.create_by = HttpContext.User.Identity.Name;
                    modelConfirm.payment_method = "SWIFT";
                    modelConfirm.trans_mt_code = "MT518";
                    modelConfirm.policy_date = policy_date;

                    api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageConfirm(modelConfirm, p =>
                    {
                        result = p;
                        //if (result.Data != null && result.Data.RPReleaseMessageResultModel.Count > 0)
                        //{
                        //    releaseMsg.result += result.Data.RPReleaseMessageResultModel[0].result;
                        //}
                        releaseMsg = result.Data.RPReleaseMessageResultModel[0];
                    });

                    if (!result.Success)
                    {
                        throw new Exception("GenRPReleaseMessage Confirm Fail.");
                    }
                    else
                    {
                        Log.WriteLog(controller, "Write File ReleaseMsg CHANGE");
                    }
                }
                else if (message_type == "CHANGE-EARLY")
                {
                    modelConfirm = new RPTransModel();
                    modelConfirm.trans_no = trans_no;
                    modelConfirm.cur = "THB";
                    modelConfirm.event_type = "CHANGE-EAR";
                    modelConfirm.message_type = "CHANGE-EAR";
                    modelConfirm.create_by = HttpContext.User.Identity.Name;
                    modelConfirm.payment_method = "SWIFT";
                    modelConfirm.trans_mt_code = "MT518";

                    api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageConfirm(modelConfirm, p =>
                    {
                        result = p;
                        if (result.Data != null && result.Data.RPReleaseMessageResultModel.Count > 0)
                        {
                            releaseMsg.result += result.Data.RPReleaseMessageResultModel[0].result;
                        }
                    });

                    if (!result.Success)
                    {
                        throw new Exception("GenRPReleaseMessage Confirm Fail.");
                    }
                    else
                    {
                        Log.WriteLog(controller, "Write File ReleaseMsg CHANGE-EAR");
                    }
                }

                if (!string.IsNullOrEmpty(releaseMsg.result) && message_type == "CANCEL")
                {
                    releaseMsg.result = releaseMsg.result.Remove(releaseMsg.result.LastIndexOf('$')) + "\r\n";
                }

                //Step 2 : StreamWriter File
                FileEntity FileEnt = new FileEntity();
                FileEnt.FileName = releaseMsg.file_name.Replace("/", "_");
                FileEnt.FilePath = Server.MapPath(releaseMsg.file_path);
                FileEnt.Values = releaseMsg.result;


                WriteFile ObjWriteFile = new WriteFile();
                if (ObjWriteFile.StreamWriter(ref FileEnt) == false)
                {
                    throw new Exception("StreamWriter() => " + FileEnt.Msg);
                }
                Log.WriteLog(controller, "Write File = Success.");

                //Step 3 : SFTP File
                Log.WriteLog(controller, "ReleaseMsg To SFTP");
                string StrMsg = string.Empty;
                if (releaseMsg.Enable == "Y")
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
                returnMsg = " & Release Message Confirm Successfully.";

                LogInOutModel logInOutModel = new LogInOutModel();
                logInOutModel.module_name = "GenReleaseMessageChange";
                logInOutModel.action_name = "SWIFT";
                logInOutModel.svc_req = "MT518";
                logInOutModel.guid = Guid.NewGuid().ToString();
                logInOutModel.ref_id = modelConfirm.trans_no;
                logInOutModel.svc_type = modelConfirm.policy_date;
                logInOutModel.status = message_type;

                staticEntities.LogInOut.Add(logInOutModel, p => { });
            }
            catch (Exception ex)
            {
                Log.WriteLog(controller, $"Release Message Fail : [Trans_No: {trans_no}]. {ex.Message}");
                returnMsg = "But Release Message Confirm Fail ";
                isResult = false;
            }
            finally
            {
                Log.WriteLog(controller, "End ReleaseMessage Confirm ==========");
            }
            return isResult;
        }

        [HttpPost]
        public ActionResult ReleaseMessageConfirmChange(string trans_no, string policy_date, string message_type)
        {
            string msg = string.Empty;
            bool isPayment = false;
            bool isSuccess = true;

            try
            {
                //Read Config
                SftpOutEnt = new SftpEntity();
                SftpBackOutEnt = new SftpEntity();
                if (!SearchConfigReleaseMsgConfirm(ref msg, ref SftpOutEnt, ref SftpBackOutEnt))
                {
                    throw new Exception("Search ConfigReleaseMsg() => " + msg);
                }


                if (!CheckReleaseMessageConfirm(ref msg, ref isPayment, new RPTransModel() { trans_deal_type = "LD" }))
                {
                    Log.WriteLog(controller, msg);
                    throw new Exception("ReleaseMessageConfirm => " + msg);
                }

                if (isPayment)
                {
                    if (!ReleaseMessageConfirmChange(ref msg, trans_no, policy_date, message_type))
                    {
                        Log.WriteLog(controller, msg);
                        throw new Exception("Release Message Fail => " + msg);
                    }
                }

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                isSuccess = false;
            }

            return Json(new { Message = msg, success = isSuccess }, JsonRequestBehavior.AllowGet);
        }

        private bool ReleaseMessageEarly(ref string returnMsg, string trans_no)
        {
            try
            {
                RPTransModel modelConfirm = new RPTransModel();
                modelConfirm.trans_no = trans_no;
                modelConfirm.cur = "THB";
                modelConfirm.event_type = "CANCEL";
                modelConfirm.message_type = "CANCEL";
                modelConfirm.create_by = HttpContext.User.Identity.Name;
                modelConfirm.payment_method = "SWIFT";
                modelConfirm.trans_mt_code = "MT518";

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

                Log.WriteLog(controller, "Write File ReleaseMsg Confirm");
                RPReleaseMessageModel releaseMsg = new RPReleaseMessageModel();
                releaseMsg = result.Data.RPReleaseMessageResultModel[0];

                modelConfirm = new RPTransModel();
                modelConfirm.trans_no = trans_no;
                modelConfirm.cur = "THB";
                modelConfirm.event_type = "CHANGE-EAR";
                modelConfirm.message_type = "CHANGE-EAR";
                modelConfirm.create_by = HttpContext.User.Identity.Name;
                modelConfirm.payment_method = "SWIFT";
                modelConfirm.trans_mt_code = "MT518";

                api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageConfirm(modelConfirm, p =>
                {
                    result = p;
                    if (result.Data != null && result.Data.RPReleaseMessageResultModel.Count > 0)
                    {
                        releaseMsg.result += result.Data.RPReleaseMessageResultModel[0].result;
                    }
                });

                if (!result.Success)
                {
                    throw new Exception("GenRPReleaseMessage Confirm Fail.");
                }

                //Step 2 : StreamWriter File
                FileEntity FileEnt = new FileEntity();
                FileEnt.FileName = releaseMsg.file_name.Replace("/", "_");
                FileEnt.FilePath = Server.MapPath(releaseMsg.file_path);
                FileEnt.Values = releaseMsg.result;


                WriteFile ObjWriteFile = new WriteFile();
                if (ObjWriteFile.StreamWriter(ref FileEnt) == false)
                {
                    throw new Exception("StreamWriter() => " + FileEnt.Msg);
                }
                Log.WriteLog(controller, "Write File = Success.");

                //Step 3 : SFTP File
                Log.WriteLog(controller, "ReleaseMsg To SFTP");
                string StrMsg = string.Empty;
                if (releaseMsg.Enable == "Y")
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
                returnMsg = " & Release Message Confirm Successfully.";

                LogInOutModel logInOutModel = new LogInOutModel();
                logInOutModel.module_name = "GenReleaseMessageChange";
                logInOutModel.action_name = "SWIFT";
                logInOutModel.svc_req = "MT518_EARLY";
                logInOutModel.guid = Guid.NewGuid().ToString();
                logInOutModel.ref_id = modelConfirm.trans_no;

                staticEntities.LogInOut.Add(logInOutModel, p => { });
            }
            catch (Exception ex)
            {
                Log.WriteLog(controller, $"Release Message Fail : [Trans_No: {trans_no}]. {ex.Message}");
                returnMsg = "But Release Message Confirm Fail ";
            }
            finally
            {
                Log.WriteLog(controller, "End ReleaseMessage Confirm ==========");
            }
            return true;
        }

        [HttpPost]
        public ActionResult ReleaseMessageEarly(string trans_no, string policy_date, string message_type)
        {
            string msg = string.Empty;
            bool isPayment = false;
            bool isSuccess = true;

            try
            {
                //Read Config
                SftpOutEnt = new SftpEntity();
                SftpBackOutEnt = new SftpEntity();
                if (!SearchConfigReleaseMsgConfirm(ref msg, ref SftpOutEnt, ref SftpBackOutEnt))
                {
                    throw new Exception("Search ConfigReleaseMsg() => " + msg);
                }


                if (!CheckReleaseMessageConfirm(ref msg, ref isPayment, new RPTransModel() { trans_deal_type = "LD" }))
                {
                    Log.WriteLog(controller, msg);
                    throw new Exception("ReleaseMessageConfirm => " + msg);
                }

                if (isPayment)
                {
                    if (!ReleaseMessageConfirmChange(ref msg, trans_no, policy_date, message_type))
                    {
                        Log.WriteLog(controller, msg);
                        throw new Exception("Release Message Fail => " + msg);
                        isSuccess = false;
                    }
                }

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                isSuccess = false;
            }

            return Json(new { Message = msg, success = isSuccess }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ReleaseMessageAmendConfirm(string trans_no, string message_type)
        {
            string msg = string.Empty;
            bool isPayment = false;
            bool isSuccess = true;

            try
            {
                //Read Config
                SftpOutEnt = new SftpEntity();
                SftpBackOutEnt = new SftpEntity();
                if (!SearchConfigReleaseMsgConfirm(ref msg, ref SftpOutEnt, ref SftpBackOutEnt))
                {
                    throw new Exception("Search ConfigReleaseMsg() => " + msg);
                }


                if (!CheckReleaseMessageConfirm(ref msg, ref isPayment, new RPTransModel() { trans_deal_type = "LD" }))
                {
                    Log.WriteLog(controller, msg);
                    throw new Exception("ReleaseMessageConfirm => " + msg);
                }

                if (isPayment)
                {
                    if (!ReleaseMessageAmendConfirm(ref msg, trans_no, message_type))
                    {
                        Log.WriteLog(controller, msg);
                        throw new Exception("Release Message Fail => " + msg);
                    }
                }

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                isSuccess = false;
            }

            return Json(new { Message = msg, success = isSuccess }, JsonRequestBehavior.AllowGet);
        }

        private bool ReleaseMessageAmendConfirm(ref string returnMsg, string trans_no, string message_type)
        {
            ResultWithModel<RPReleaseMessageResult> result = new ResultWithModel<RPReleaseMessageResult>();
            PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();
            RPTransModel modelConfirm = new RPTransModel();
            RPReleaseMessageModel releaseMsg = new RPReleaseMessageModel();
            bool isResult = true;
            try
            {
                //Step 1 : ReleaseMessage TransNo

                if (message_type == "CANCEL")
                {
                    modelConfirm.trans_no = trans_no;
                    modelConfirm.cur = "THB";
                    modelConfirm.event_type = "CANCEL";
                    modelConfirm.message_type = "CANCEL";
                    modelConfirm.create_by = HttpContext.User.Identity.Name;
                    modelConfirm.payment_method = "SWIFT";
                    modelConfirm.trans_mt_code = "MT518";

                    api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageConfirmAmendDeal(modelConfirm, p =>
                    {
                        result = p;
                    });

                    if (!result.Success)
                    {
                        throw new Exception("GenRPReleaseMessage Confirm Fail.");
                    }
                    else
                    {
                        Log.WriteLog(controller, "Write File ReleaseMsg CANCEL");
                    }

                    releaseMsg = result.Data.RPReleaseMessageResultModel[0];

                }

                if (message_type == "CONFIRM")
                {
                    modelConfirm = new RPTransModel();
                    modelConfirm.trans_no = trans_no;
                    modelConfirm.cur = "THB";
                    modelConfirm.event_type = "CONFIRM";
                    modelConfirm.message_type = "CONFIRM";
                    modelConfirm.create_by = HttpContext.User.Identity.Name;
                    modelConfirm.payment_method = "SWIFT";
                    modelConfirm.trans_mt_code = "MT518";

                    api_RPReleaseMsg.RPReleaseMessage.GenRPReleaseMessageConfirmAmendDeal(modelConfirm, p =>
                    {
                        result = p;
                        releaseMsg = result.Data.RPReleaseMessageResultModel[0];
                    });

                    if (!result.Success)
                    {
                        throw new Exception("GenRPReleaseMessage Confirm Fail.");
                    }
                    else
                    {
                        Log.WriteLog(controller, "Write File ReleaseMsg CHANGE");
                    }
                }

                if (!string.IsNullOrEmpty(releaseMsg.result) && releaseMsg.result.LastIndexOf('$') > -1)
                {
                    releaseMsg.result = releaseMsg.result.Remove(releaseMsg.result.LastIndexOf('$')) + "\r\n";
                }
                else 
                {
                    releaseMsg.result += "\r\n";
                }


                //Step 2 : StreamWriter File
                FileEntity FileEnt = new FileEntity();
                FileEnt.FileName = releaseMsg.file_name.Replace("/", "_");
                FileEnt.FilePath = Server.MapPath(releaseMsg.file_path);
                FileEnt.Values = releaseMsg.result;


                WriteFile ObjWriteFile = new WriteFile();
                if (ObjWriteFile.StreamWriter(ref FileEnt) == false)
                {
                    throw new Exception("StreamWriter() => " + FileEnt.Msg);
                }
                Log.WriteLog(controller, "Write File = Success.");

                //Step 3 : SFTP File
                Log.WriteLog(controller, "ReleaseMsg To SFTP");
                string StrMsg = string.Empty;
                if (releaseMsg.Enable == "Y")
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
                returnMsg = " & Release Message Confirm Successfully.";

                LogInOutModel logInOutModel = new LogInOutModel();
                logInOutModel.module_name = "GenReleaseMessageAmend";
                logInOutModel.action_name = "SWIFT";
                logInOutModel.svc_req = "MT518";
                logInOutModel.guid = Guid.NewGuid().ToString();
                logInOutModel.ref_id = modelConfirm.trans_no;
                logInOutModel.status = message_type == "CONFIRM" ? "AMEND" : message_type;

                staticEntities.LogInOut.Add(logInOutModel, p => { });
            }
            catch (Exception ex)
            {
                Log.WriteLog(controller, $"Release Message Fail : [Trans_No: {trans_no}]. {ex.Message}");
                returnMsg = "But Release Message Confirm Fail ";
                isResult = false;
            }
            finally
            {
                Log.WriteLog(controller, "End ReleaseMessage Confirm ==========");
            }
            return isResult;
        }


    }
}