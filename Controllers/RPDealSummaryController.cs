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
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class RPDealSummaryController : BaseController
    {
        RPTransEntity api_RPTrans = new RPTransEntity();
        Utility utility = new Utility();

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            var RPDealEntryView = new RPDealEntryViewModel();
            RPTransModel RPTrans = new RPTransModel();

            RPDealEntryView.FormSearch = RPTrans;

            return View(RPDealEntryView);
        }

        public ActionResult FillTransState(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealSummary.GetDDLTransState(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillTransStatus(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealSummary.GetDDLTransStatus(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
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
                        case "trans_state":
                            RPTransModel.trans_state = column.search.value;
                            break;
                        case "trans_status":
                            RPTransModel.trans_status = column.search.value;
                            break;
                        case "counter_party_name":
                            RPTransModel.counter_party_code = column.search.value;
                            break;
                    }
                });

                api_RPTrans.RPDealSummary.GetRPDealSummaryList(RPTransModel, p =>
                {
                    result = p;
                });

                //throw new Exception("Tessssssssssssssss");
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

            //return Json(new
            //{
            //    draw = model.draw,
            //    recordsTotal = result.HowManyRecord,
            //    recordsFiltered = result.HowManyRecord,
            //    Message = result.Message,
            //    data = result.Data.RPTransResultModel
            //});
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

                api_RPTrans.RPDealSummary.GetRPDealSummaryDetail(RPTransModel, p =>
                {
                    result = p;
                });

                RPTransModel = result.Data.RPTransResultModel[0];

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
                    case "BO-APPROVE":
                    case "BO-SETTLEMENT":
                    case "BO-MATURITY":
                        {
                            RPTransModel.trans_state_style = "label-deal-accept";
                            break;
                        }
                    case "FO-REJECTAPPROVE":
                    case "BO-REJECTAPPROVE":
                    case "BO-REJECTSETTLEMENT":
                    case "BO-REJECTMATURITY":
                        {
                            RPTransModel.trans_state_style = "label-deal-unaccept";
                            break;
                        }
                    default:
                        {
                            RPTransModel.trans_state_style = "label-default";
                            break;
                        }
                }


                #endregion

                //Step 2 : Set Enable Btn_PreviousNext
                if (Enable_BtnPreviousNext(ref StrMsg, ref RPTransModel) == false)
                {
                    throw new Exception("Enable_BtnPreviousNext() : " + StrMsg);
                }

                if (RPTransModel.net_settement_flag == null) 
                {
                    RPTransModel.net_settement_flag = false;
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

                api_RPTrans.RPDealSummary.GetRPDealSummaryColateralList(RPTransModel, p =>
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
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }
    }
}