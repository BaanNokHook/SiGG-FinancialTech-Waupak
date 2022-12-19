using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.RPTransaction;
using GM.Data.Result.RPTransaction;
using GM.Data.View.RPTransaction;
using GM.Filters;
using Newtonsoft.Json;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class RPEarlyTerminationController : BaseController
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

                api_RPTrans.RPEarlyTermination.GetRPEarlyTerminationList(RPTransModel, p =>
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

        [HttpPost]
        public ActionResult Submit(RPTransModel model)
        {
            string StrMsg = string.Empty;
            string Next_TransNo = string.Empty;

            try
            {
                ResultWithModel<RPTransResult> rwm = new ResultWithModel<RPTransResult>();
                model.update_by = User.UserId;
                model.user_id = User.UserId;

                api_RPTrans.RPEarlyTermination.UpdateRPEarlyTermination(model, p =>
                {
                    rwm = p;
                });

                if (!rwm.Success)
                {
                    throw new Exception(rwm.Message);
                }

                // Step 2 : Remove TransNo Approve = [Success] From ListTrans                
                if (Session["ListTrans"] != null && Session["ListTrans"].ToString() != ""
                    && Remove_TransNoFromList(ref StrMsg, ref Next_TransNo, model) == false)
                {
                    throw new Exception("Remove_TransNoFromList() => " + StrMsg);
                }

            }
            catch (Exception Ex)
            {
                StrMsg = Ex.Message;
            }

            return Json(new { Message = StrMsg, trans_no = Next_TransNo }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Select(string id)
        {
            List<string> ListTrans = new List<string>();
            ListTrans = JsonConvert.DeserializeObject<List<string>>(id);
            ListTrans.Sort();
            Session["ListTrans"] = ListTrans;

            return RedirectToAction("Detail", new { trans_no = ListTrans[0] });
        }

        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Detail(string trans_no)
        {
            RPTransModel RPTrans = new RPTransModel();
            string StrMsg = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(trans_no))
                {
                    ResultWithModel<RPTransResult> result = new ResultWithModel<RPTransResult>();
                    ResultWithModel<RPTransColateralResult> resultcoll = new ResultWithModel<RPTransColateralResult>();


                    RPTransColateralModel RPTransColateral = new RPTransColateralModel();

                    try
                    {
                        //Add Paging
                        PagingModel paging = new PagingModel();
                        RPTrans.paging = paging;
                        //Add Orderby
                        var orders = new List<OrderByModel>();
                        RPTrans.ordersby = orders;
                        //Add counterparty id
                        RPTrans.trans_no = trans_no;
                        RPTransColateral.trans_no = trans_no;

                        api_RPTrans.RPEarlyTermination.GetRPEarlyTerminationDetail(RPTrans, p =>
                        {
                            result = p;
                        });

                        if (result.Data.RPTransResultModel.Count == 0)
                        {
                            return RedirectToAction("Index");
                        }

                        RPTrans = result.Data.RPTransResultModel[0];

                        if (RPTrans.ismanual_cal == null)
                        {
                            RPTrans.ismanual_cal = false;
                        }

                        if (RPTrans.net_settement_flag == null)
                        {
                            RPTrans.net_settement_flag = false;
                        }

                        List<DDLItemModel> res = new List<DDLItemModel>();
                        api_RPTrans.RPEarlyTermination.GetDDLTransRemark("", p =>
                        {
                            if (p.Success)
                            {
                                res = p.Data.DDLItems;
                                if (res != null)
                                {
                                    RPTrans.remark_id = res.FirstOrDefault()?.Value.ToString();
                                    RPTrans.remark_desc = res.FirstOrDefault()?.Text;
                                }
                            }
                        });

                        RPTrans.counter_party_fund_name = RPTrans.counter_party_fund_name == null ? "None" : RPTrans.counter_party_fund_name;

                        // Step 2 : Set Enable Btn_PreviousNext
                        if (Enable_BtnPreviousNext(ref StrMsg, ref RPTrans) == false)
                        {
                            throw new Exception("Enable_BtnPreviousNext() => " + StrMsg);
                        }
                    }
                    catch (Exception)
                    {

                        //throw;
                    }
                }
            }
            catch (Exception ex)
            {
                return View(new RPTransModel());
            }

            return View(RPTrans);
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

                api_RPTrans.RPEarlyTermination.GetRPEarlyTerminationCollateralList(RPTransModel, p =>
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

        [HttpPost]
        public ActionResult Calculate(string trans_no, string terminate_date, string asof_date)
        {
            ResultWithModel<RPTransResult> res = new ResultWithModel<RPTransResult>();

            RPTransModel model = new RPTransModel();
            model.trans_no = trans_no;
            model.terminate_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(terminate_date);
            model.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(asof_date);
            model.user_id = User.UserId;
            api_RPTrans.RPEarlyTermination.CalculateRPEarlyTermination(model, p =>
            {
                if (p.Success)
                {
                    res = p;
                }
            });

            return Json(new
            {
                success = res.Success,
                Message = res.Message,
                data = res.Data?.RPTransResultModel[0]
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Reset(string trans_no)
        {
            ResultWithModel<RPTransResult> res = new ResultWithModel<RPTransResult>();

            RPTransModel model = new RPTransModel();
            model.trans_no = trans_no;
            model.user_id = User.UserId;
            api_RPTrans.RPEarlyTermination.ResetRPEarlyTermination(model, p =>
            {
                if (p.Success)
                {
                    res = p;
                }
            });

            return Json(new
            {
                success = res.Success,
                Message = res.Message,
                data = res.Data?.RPTransResultModel[0]
            }, JsonRequestBehavior.AllowGet);
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

            return RedirectToAction("Detail", new { trans_no = Previous_TransNo });
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

            return RedirectToAction("Detail", new { trans_no = Next_TransNo });
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

        public ActionResult FillRemark(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPEarlyTermination.GetDDLTransRemark(datastr, p =>
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