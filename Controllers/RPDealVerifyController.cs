using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.RPTransaction;
using GM.Data.Result.RPTransaction;
using GM.Data.View.RPTransaction;
using GM.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class RPDealVerifyController : BaseController
    {
        RPTransEntity api_RPTrans = new RPTransEntity();
        Utility utility = new Utility();

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            var RPDealEntryView = new RPDealEntryViewModel();
            RPTransModel model = new RPTransModel();
            model.user_id = User.UserId;
            model.username = User.UserEngName;
            model.port = User.DeskGroupName.ToUpper();
            model.port_name = User.DeskGroupName.ToUpper();

            RPDealEntryView.FormSearch = model;

            return View(RPDealEntryView);
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
                        case "user_id":
                            RPTransModel.user_id = column.search.value;
                            break;
                    }
                });

                api_RPTrans.RPDealVerify.GetRPDealVerifyList(RPTransModel, p =>
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
                success = false,
                draw = model.draw,
                recordsTotal = result.HowManyRecord,
                recordsFiltered = result.HowManyRecord,
                Message = result.Message,
                data = result.Data != null ? result.Data.RPTransResultModel : new List<RPTransModel>(),
            }, JsonRequestBehavior.AllowGet);
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
                // Step 0 : Check trans_no
                if (string.IsNullOrEmpty(trans_no) == true)
                {
                    return RedirectToAction("Index");
                }

                // Step 1 : Select Detail From [TransNo]
                //Add Paging
                PagingModel paging = new PagingModel();
                RPTransModel.paging = paging;

                //Add Orderby
                var orders = new List<OrderByModel>();
                RPTransModel.ordersby = orders;

                //Add trans_no
                RPTransModel.trans_no = trans_no;

                api_RPTrans.RPDealVerify.GetRPDealVerifyDetail(RPTransModel, p =>
                {
                    result = p;
                });

                RPTransModel = result.Data.RPTransResultModel[0];

                RPTransModel.SessionName = Guid.NewGuid().ToString();

                // Step 2 : Set Enable Btn_PreviousNext
                if (Enable_BtnPreviousNext(ref StrMsg, ref RPTransModel) == false)
                {
                    throw new Exception("Enable_BtnPreviousNext() => " + StrMsg);
                }

                // Step 3 Set Label Style By Status

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
                    case "FO-CREATE":
                        {
                            RPTransModel.trans_state_style = "label-deal-new";
                            break;
                        }
                    case "FO-APPROVE":
                        {
                            RPTransModel.trans_state_style = "label-deal-accept";
                            break;
                        }
                    case "BO-REJECTAPPROVE":
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


                // Step 4 : Check Trans State
                if (RPTransModel.trans_state.ToUpper() == "FO-CREATE" || RPTransModel.trans_state.ToUpper() == "BO-REJECTAPPROVE")
                {
                    RPTransModel.btn_Approve = true;
                    RPTransModel.btn_UnApprove = true;

                    return View(RPTransModel);
                }
                else if (RPTransModel.trans_state.ToUpper() == "FO-APPROVE")
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
        public ActionResult Search_Colateral(DataTableAjaxPostModel model, string trans_no,string sessionName)
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

                api_RPTrans.RPDealVerify.GetRPDealVerifyColateralList(RPTransModel, p =>
                {
                    result = p;
                });

                Session[sessionName] = result.Data.RPTransColateralResultModel;
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
        public ActionResult Approve_Trans(RPTransModel RPTransModel)
        {
            string StrMsg = string.Empty;
            string Next_TransNo = string.Empty;
            var Result = new List<object>();

            try
            {
                ResultWithModel<RPTransResult> rwm = new ResultWithModel<RPTransResult>();
                // Step 1 : Approve TransNo
                RPTransModel.update_by = HttpContext.User.Identity.Name;
                RPTransModel.trans_state = "APPROVE";

                api_RPTrans.RPDealVerify.UpdateRPDealVerify(RPTransModel, p =>
                {
                    rwm = p;
                });

                if (!rwm.Success)
                {
                    throw new Exception(rwm.Message);
                }

                // Step 2 : Remove TransNo Approve = [Success] From ListTrans                
                if (Session["ListTrans"] != null && Session["ListTrans"].ToString() != ""
                    && Remove_TransNoFromList(ref StrMsg, ref Next_TransNo, RPTransModel) == false)
                {
                    throw new Exception("Remove_TransNoFromList() => " + StrMsg);
                }

                if (RPTransModel.cur == "THB")
                {
                    // Step 3 : Export Excel & SFTP To TBMA
                    ResultWithModel<RPReportTBMAResult> ResultModel = new ResultWithModel<RPReportTBMAResult>();
                    PagingModel paging = new PagingModel();
                    paging.PageNumber = 1;
                    paging.RecordPerPage = 100;
                    RPTransModel.paging = paging;

                    //Add Orderby
                    var orders = new List<OrderByModel>();
                    RPTransModel.ordersby = orders;
                    RPTransModel.create_by = HttpContext.User.Identity.Name;

                    api_RPTrans.RPReportTBMA.ExportRPReportTBMA(RPTransModel, p =>
                    {
                        ResultModel = p;
                    });

                    if (ResultModel.Success == false)
                    {
                        throw new Exception("ExportRPReportTBMA() => " + ResultModel.Message);
                    }
                }
            }
            catch (Exception Ex)
            {
                StrMsg = Ex.Message;
            }

            Result.Add(new { Message = StrMsg, trans_no = Next_TransNo });
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

                api_RPTrans.RPDealVerify.UpdateRPDealVerify(RPTransModel, p =>
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

        /// <summary>
        ///  Check Over limit for you.
        /// </summary>
        /// <param name="RPTranModel"></param>
        /// <returns></returns>
        public ActionResult CheckLimit(RPTransModel model)
        {
            model.user_id = HttpContext.User.Identity.Name;;

            ResultWithModel<RPTransResult> res = new ResultWithModel<RPTransResult>();

            List<RPTransColateralModel> listColl = (List<RPTransColateralModel>)Session[model.SessionName];
            listColl = listColl.Where(o => o.status != "duplicate" && o.status != "NotHave" && o.status != "delete").ToList();

            model.RPTransColateralModel = listColl.Count() > 0 ? listColl[0] : null;

            try
            {
                //if (RpTranModel.repo_deal_type != "BRP")
                //{
                api_RPTrans.RPDealEntry.CheckLimit(model, p =>
                {
                    res = p;
                });
                //}
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillUserID(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLUserID(datastr, p =>
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