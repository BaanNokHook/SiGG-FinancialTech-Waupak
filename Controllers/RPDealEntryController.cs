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
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class RPDealEntryController : BaseController
    {
        RPTransEntity api_RPTrans = new RPTransEntity();
        StaticEntities api_static = new StaticEntities();
        Utility utility = new Utility();
        private static string Controller = "RPDealEntryController";
        private static LogFile Log = new LogFile();

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            var RPDealEntryView = new RPDealEntryViewModel();
            RPTransModel RPTrans = new RPTransModel();
            RPTrans.user_id = User.UserId;
            RPTrans.username = User.UserEngName;

            RPDealEntryView.FormSearch = RPTrans;

            //ViewBag.UserTypes = new SelectList(getAllUserTypesList(), "Value", "Text");

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
                        //case "maturity_date":
                        //    RPTransModel.maturity_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                        //    break;
                        case "cur":
                            RPTransModel.cur = column.search.value;
                            break;
                        case "repo_deal_type":
                            RPTransModel.repo_deal_type = column.search.value;
                            break;
                        case "repo_deal_type_name":
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

                api_RPTrans.RPDealEntry.GetRPDealEntryList(RPTransModel, p =>
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

        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Add(string trans_no, bool isCopy = false)
        {
            RPTransModel RPTrans = new RPTransModel();

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



                        if (isCopy)
                        {
                            api_RPTrans.RPDealEntry.RPDealEntryCopyDeal(RPTrans, p =>
                            {
                                result = p;
                            });

                            RPTrans = result.Data.RPTransResultModel[0];
                            RPTrans.statusdata = "New";
                            RPTrans.user_id = User.UserId;
                            RPTrans.username = User.UserEngName;
                            RPTrans.trans_no = "          ";
                            RPTrans.page_name = "addpage";
                        }
                        else
                        {
                            api_RPTrans.RPDealEntry.RPDealEntryList(RPTrans, p =>
                            {
                                result = p;
                            });

                            RPTrans = result.Data.RPTransResultModel[0];
                            RPTrans.page_name = "editpage";
                        }

                        if (result.Data.RPTransResultModel.Count == 0)
                        {
                            return View("Index");
                        }

                        if (RPTrans.ismanual_cal == null)
                        {
                            RPTrans.ismanual_cal = false;
                        }

                        if (RPTrans.net_settement_flag == null)
                        {
                            RPTrans.net_settement_flag = false;
                        }

                        RPTrans.SessionName = Guid.NewGuid().ToString();
                        RPTrans.isCopy = isCopy;
                        //RPTrans.counter_party_name = RPTrans.counter_party_name.Split(':')[1].Trim();
                        RPTrans.counter_party_fund_name = RPTrans.counter_party_fund_name == null ? "None" : RPTrans.counter_party_fund_name;



                        if (Session[RPTrans.SessionName] != null)
                        {
                            Session[RPTrans.SessionName] = null;
                        }
                    }
                    catch (Exception)
                    {

                        //throw;
                    }
                }
                else
                {
                    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    JwtSecurityTokenHandler JwtSecurityTokenHandler = new JwtSecurityTokenHandler();

                    int tmpIntConvert = 0;
                    //LoginViewModel login = (LoginViewModel)JsonConvert.DeserializeObject(authTicket.UserData, typeof(LoginViewModel));
                    //var tokenS = JwtSecurityTokenHandler.ReadToken(login.token) as JwtSecurityToken;

                    RPTrans.statusdata = "New";
                    RPTrans.user_id = User.UserId;
                    RPTrans.username = User.UserEngName;

                    if (User.DeskGroupName != "" && User.DeskGroupName != null)
                    {
                        RPTrans.desk_group_name = User.DeskGroupName;
                        RPTrans.desk_group_id = int.TryParse(User.DeskGroupId, out tmpIntConvert) ? tmpIntConvert : 0;

                        RPTrans.port_name = User.DeskGroupName.ToUpper();
                        RPTrans.port = User.DeskGroupName.ToUpper();
                    }
                    else
                    {
                        RPTrans.port_name = "BANKING";
                        RPTrans.port = "BANKING";
                        List<DDLItemModel> tmpDeskGroup = GetDeskGroup("BANKING");
                        if (tmpDeskGroup.Count > 0)
                        {
                            RPTrans.desk_group_name = tmpDeskGroup[0].Text;
                            RPTrans.desk_group_id = int.TryParse(tmpDeskGroup[0].Value.ToString(), out tmpIntConvert) ? tmpIntConvert : 0;
                        }
                    }
                    if (User.TraderId != null)
                    {
                        List<DDLItemModel> tmpDealer = GetDealer(User.TraderId, RPTrans.port);
                        if (tmpDealer.Count > 0)
                        {
                            RPTrans.trader_id = User.TraderId;
                            RPTrans.trader_name = tmpDealer[0].Text;
                        }
                    }

                    //Set Default Value
                    RPTrans.trans_no = "          ";
                    RPTrans.page_name = "addpage";
                    RPTrans.exch_rate = 1;
                    RPTrans.interest_type = "FIXED";
                    RPTrans.cost_type = "FIXED";
                    RPTrans.cur = "THB";
                    RPTrans.cur_pair1 = "THB";
                    RPTrans.cur_pair2 = "THB";

                    RPTrans.deal_period = 1;
                    RPTrans.repo_deal_type = RPTrans.port != "BANKING" ? "PRP" : "BRP";

                    var res_trans_type = GetDDLTransType(null).Where(p => p.Value.ToString() == "Overnight").FirstOrDefault();
                    RPTrans.trans_type = res_trans_type.Value.ToString();
                    RPTrans.trans_type_name = res_trans_type.Text;

                    var res_instrument = GetDDLInstrumentType(null).FirstOrDefault();
                    RPTrans.trans_deal_type = res_instrument.Value.ToString();
                    RPTrans.trans_deal_type_name = res_instrument.Text;

                    var res_desk = GetDDLDesk(User.UserId, RPTrans.port, RPTrans.repo_deal_type).Where(p => p.Text == "BRP2").FirstOrDefault();
                    if (res_desk != null)
                    {
                        RPTrans.desk_book_name = res_desk.Text;
                        RPTrans.desk_book_id = int.TryParse(res_desk.Value.ToString(), out tmpIntConvert) ? tmpIntConvert : 0;
                    }

                    #region Prepare Counter Party
                    try
                    {
                        var res_counterparty = GetDDLCounterParty(null, RPTrans.repo_deal_type, RPTrans.trans_deal_type).FirstOrDefault();
                        RPTrans.counter_party_id = int.TryParse(res_counterparty.Value.ToString(), out tmpIntConvert) ? tmpIntConvert : 0;

                        RPTrans.counter_party_name = res_counterparty.Text;
                        RPTrans.wht_tax = decimal.Parse(res_counterparty.Value2.ToString());
                        RPTrans.swift_code = res_counterparty.Value3.ToString();
                        RPTrans.threshold = decimal.Parse(res_counterparty.Value4.ToString());

                        RPTrans.absorb = RPTrans.wht_tax + " " + GetAbsorbForString(RPTrans.counter_party_id.Value);
                    }
                    catch { }
                    #endregion

                    RPTrans.trans_in_term_id = 1;
                    RPTrans.trans_in_term_id_name = "At Maturity";

                    var res_basis = GetDDLBasisCode("ACT/365").FirstOrDefault();
                    if (res_basis != null)
                    {
                        RPTrans.basis_code = int.TryParse(res_basis.Value.ToString(), out tmpIntConvert) ? tmpIntConvert : 0;
                        RPTrans.basis_code_name = res_basis.Text;
                    }

                    var res_purpose = GetDDLPurpose("FINB").FirstOrDefault();
                    if (res_purpose != null)
                    {
                        RPTrans.purpose = res_purpose.Value.ToString();
                        RPTrans.purpose_name = res_purpose.Text;
                    }

                    var businessdate = Getbusinessdate();
                    RPTrans.trade_date = businessdate;
                    RPTrans.settlement_date = businessdate;
                    RPTrans.maturity_date = businessdate.AddDays(1);

                    RPTrans.ismanual_cal = false;
                    RPTrans.net_settement_flag = false;

                    //RPTrans.rp_source_value = "TBMA";
                    //RPTrans.rp_source_text = "TBMA";

                    //Data data = new Data();

                    //data.instrumenttype = RPTrans.trans_deal_type;
                    //data.inttype = RPTrans.interest_type;
                    //data.transtype = RPTrans.trans_type;
                    //data.period = RPTrans.deal_period.ToString();

                    ResultWithModel<RPTransResult> resultbailat = GetBilatContract(RPTrans);
                    RPTrans.bilateral_contract_no = resultbailat.Data != null && resultbailat.Data.RPTransResultModel.Count > 0
                        ? resultbailat.Data.RPTransResultModel[0].bilateral_contract_no : "";

                    //RPTrans.trade_date = null;

                    api_RPTrans.RPDealEntry.CostOfFund(RPTrans.cur, p =>
                    {
                        if (p.Data != null && p.Data.RPTransCheckCostOfFundResultModel != null && p.Data.RPTransCheckCostOfFundResultModel.Count > 0)
                        {
                            RPTrans.cost_of_fund = p.Data.RPTransCheckCostOfFundResultModel[0].cost_of_fund_rate;
                            RPTrans.cost_total = p.Data.RPTransCheckCostOfFundResultModel[0].cost_of_fund_rate;
                        }
                    });

                    RPTrans.SessionName = Guid.NewGuid().ToString();
                    if (Session[RPTrans.SessionName] != null)
                    {
                        Session[RPTrans.SessionName] = null;
                    }
                    if (RPTrans.ColateralList == null)
                    {
                        RPTrans.ColateralList = new List<RPTransColateralModel>();
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
        public ActionResult Add(RPTransModel rpTranModel)
        {
            ResultWithModel<RPTransResult> res = new ResultWithModel<RPTransResult>();
            try
            {
                List<RPTransColateralModel> listColl = (List<RPTransColateralModel>)Session[rpTranModel.SessionName];
                foreach (var item in listColl)
                {
                    if (!rpTranModel.ColateralList.Any(x => x.instrument_id == item.instrument_id && x.status == item.status))
                    {
                        rpTranModel.ColateralList.Add(item);
                    }
                }

                if (ModelState.ContainsKey("payment_method"))
                    ModelState["payment_method"].Errors.Clear();
                if (ModelState.ContainsKey("trans_mt_code"))
                    ModelState["trans_mt_code"].Errors.Clear();
                if (ModelState.ContainsKey("formula"))
                    ModelState["formula"].Errors.Clear();

                if (rpTranModel.repo_deal_type == "PRP")
                {
                    if (ModelState.ContainsKey("bilateral_contract_no"))
                    {
                        ModelState["bilateral_contract_no"].Errors.Clear();
                    }
                }

                if (rpTranModel.interest_type == "FIXED")
                {
                    if (ModelState.ContainsKey("interest_floating_index_code"))
                    {
                        ModelState["interest_floating_index_code"].Errors.Clear();
                    }
                }

                if (rpTranModel.deal_period == 0 || rpTranModel.deal_period == null)
                {
                    if (ModelState.ContainsKey("maturity_date"))
                    {
                        ModelState["maturity_date"].Errors.Clear();
                    }

                    rpTranModel.maturity_date = null;

                    if (ModelState.ContainsKey("deal_period"))
                    {
                        ModelState["deal_period"].Errors.Clear();
                    }

                    rpTranModel.deal_period = 0;
                }

                if (rpTranModel.rp_price_date_value != null && rpTranModel.rp_price_date_value != string.Empty)
                {
                    rpTranModel.market_date = DateTime.ParseExact(rpTranModel.rp_price_date_value.Split('|')[0], "ddMMyyyy", CultureInfo.InvariantCulture);
                    rpTranModel.marketdate_t = rpTranModel.rp_price_date_value.Split('|')[1];
                }

                rpTranModel.interest_rate = rpTranModel.interest_rate == null ? 0 : rpTranModel.interest_rate;
                rpTranModel.interest_spread = rpTranModel.interest_spread == null ? 0 : rpTranModel.interest_spread;
                rpTranModel.cost_of_fund = rpTranModel.cost_of_fund == null ? 0 : rpTranModel.cost_of_fund;
                rpTranModel.cost_spread = rpTranModel.cost_spread == null ? 0 : rpTranModel.cost_spread;

                if (rpTranModel.page_name == "addpage")
                {
                    rpTranModel.create_by = HttpContext.User.Identity.Name;
                    rpTranModel = SetStatusFlag(rpTranModel, rpTranModel.page_name);

                    if (ModelState.IsValid)
                    {
                        api_RPTrans.RPDealEntry.CreateTrans(rpTranModel, p =>
                        {
                            res = p;
                        });

                        if (!res.Success)
                        {
                            ModelState.AddModelError("Exception", res.Message);
                        }
                        else
                        {
                            Session[rpTranModel.SessionName] = null;
                        }
                    }
                    else
                    {
                        var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();
                        Models.ForEach(field =>
                        {
                            field.Errors.ToList().ForEach(error =>
                            {
                                if (!res.Message.Contains(error.ErrorMessage))
                                {
                                    res.Message += error.ErrorMessage;
                                }
                            });

                        });
                        rpTranModel.error_flag = "True";
                    }
                    if (rpTranModel.ColateralList == null)
                    {
                        rpTranModel.ColateralList = new List<RPTransColateralModel>();
                        RPTransColateralModel colat = new RPTransColateralModel();
                        rpTranModel.ColateralList.Add(colat);
                        rpTranModel.RPTransColateralModel = new RPTransColateralModel();
                    }
                    rpTranModel.remark_desc = res.Message;
                }
                else
                {
                    rpTranModel.create_by = HttpContext.User.Identity.Name;
                    rpTranModel = SetStatusFlag(rpTranModel, rpTranModel.page_name);

                    if (ModelState.IsValid)
                    {
                        api_RPTrans.RPDealEntry.UpdateTrans(rpTranModel, p =>
                        {
                            res = p;
                        });

                        if (!res.Success)
                        {
                            ModelState.AddModelError("Exception", res.Message);
                        }
                        else
                        {
                            Session[rpTranModel.SessionName] = null;
                        }
                    }
                    else
                    {
                        var modelState = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();
                        modelState.ForEach(field =>
                        {
                            field.Errors.ToList().ForEach(error =>
                            {
                                if (!res.Message.Contains(error.ErrorMessage))
                                {
                                    res.Message += error.ErrorMessage;
                                }

                            });

                        });
                        rpTranModel.error_flag = "True";
                    }
                    if (rpTranModel.ColateralList == null)
                    {
                        rpTranModel.ColateralList = new List<RPTransColateralModel>();
                        RPTransColateralModel Colat = new RPTransColateralModel();
                        rpTranModel.ColateralList.Add(Colat);
                        rpTranModel.RPTransColateralModel = new RPTransColateralModel();
                    }
                    rpTranModel.remark_desc = res.Message;
                }
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = ex.ToString();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Edit(string transno, bool isCopy = false)
        {
            return RedirectToAction("Add", new { trans_no = transno, isCopy = isCopy });
        }

        [HttpPost]
        public JsonResult Deletes(Data data)
        {
            var rwm = new ResultWithModel<RPTransResult>();
            RPTransModel view = new RPTransModel();
            view.create_by = HttpContext.User.Identity.Name;
            view.trans_no = data.transno;
            try
            {
                api_RPTrans.RPDealEntry.DeleteTrans(view, p =>
                {
                    rwm = p;
                });

                if (rwm.Success)
                {
                    return Json(new { success = true, responseText = "Your message successfuly sent!" }, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    ModelState.AddModelError("", rwm.Message);
                    return Json(new { success = false, responseText = rwm.Message }, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCheckCallFromText(Data data)
        {
            //  RPTransResult res = new RPTransResult();
            ResultWithModel<RPTransColateralResult> res = new ResultWithModel<RPTransColateralResult>();
            RPTransModel rpTran = new RPTransModel();
            RPTransColateralModel rpTransColateralModel = new RPTransColateralModel();
            ResultWithModel<RPTransColateralResult> resultcollold = new ResultWithModel<RPTransColateralResult>();

            rpTran.deal_period = string.IsNullOrEmpty(data.period) == true ? 0 : int.Parse(data.period);
            rpTran.wht_tax = string.IsNullOrEmpty(data.wht) == true ? 0 : decimal.Parse(data.wht);
            rpTran.basis_code = string.IsNullOrEmpty(data.yearbasis) == true ? 0 : int.Parse(data.yearbasis);
            rpTran.interest_total = string.IsNullOrEmpty(data.totalint) == true ? 0 : decimal.Parse(data.totalint);

            rpTran.rp_source_value = data.rpsource;
            rpTran.rp_price_date_value = data.rppricedate;
            rpTran.counter_party_id = string.IsNullOrEmpty(data.counterpartyid) == true ? 0 : int.Parse(data.counterpartyid);


            rpTran.instrument_code = data.instrumentcode;
            rpTran.trigger = data.trigger;

            if (string.IsNullOrEmpty(data.dm))
            {
                rpTransColateralModel.dm = null;
            }
            else
            {
                rpTransColateralModel.dm = decimal.Parse(data.dm);
            }
            if (string.IsNullOrEmpty(data.vm))
            {
                rpTransColateralModel.variation = null;
            }
            else
            {
                rpTransColateralModel.variation = decimal.Parse(data.vm);
            }
            if (string.IsNullOrEmpty(data.dirtyprice))
            {
                rpTransColateralModel.dirty_price = null;
            }
            else
            {
                rpTransColateralModel.dirty_price = decimal.Parse(data.dirtyprice);
            }

            if (string.IsNullOrEmpty(data.cleanprice))
            {
                rpTransColateralModel.clean_price = null;
            }
            else
            {
                rpTransColateralModel.clean_price = decimal.Parse(data.cleanprice);
            }


            rpTransColateralModel.par_unit = string.IsNullOrEmpty(data.parunit) == true ? 0 : decimal.Parse(data.parunit);
            rpTransColateralModel.ytm = string.IsNullOrEmpty(data.ytm) == true ? 0 : decimal.Parse(data.ytm);
            rpTransColateralModel.haircut = string.IsNullOrEmpty(data.hc) == true ? 0 : decimal.Parse(data.hc);
            rpTransColateralModel.unit = string.IsNullOrEmpty(data.unit) == true ? 0 : decimal.Parse(data.unit);
            rpTransColateralModel.par = string.IsNullOrEmpty(data.parval) == true ? 0 : decimal.Parse(data.parval);
            rpTransColateralModel.market_value = string.IsNullOrEmpty(data.secmarketval) == true ? 0 : decimal.Parse(data.secmarketval);
            rpTransColateralModel.cash_amount = string.IsNullOrEmpty(data.cashamount) == true ? 0 : decimal.Parse(data.cashamount);

            rpTransColateralModel.interest_amount = data.interest_amount;
            rpTransColateralModel.wht_amount = data.wht_amount;
            rpTransColateralModel.isLastRecord = data.isLastRecord;
            rpTransColateralModel.special_case_id = string.IsNullOrEmpty(data.specialCaseId) == true ? 0 : int.Parse(data.specialCaseId);

            rpTran.formula = data.formula;
            rpTran.cur = data.cur;
            rpTran.settlement_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(data.settlementdate);
            rpTran.special_case_id = string.IsNullOrEmpty(data.specialCaseId) == true ? 0 : int.Parse(data.specialCaseId);


            rpTran.RPTransColateralModel = rpTransColateralModel;

            api_RPTrans.RPDealEntry.CheckCollateral(rpTran, p =>
            {
                res = p;
            });

            if (Session[data.sessionName] != null)
            {
                resultcollold.Data = new RPTransColateralResult();
                resultcollold.Data.RPTransColateralResultModel = (List<RPTransColateralModel>)Session[data.sessionName];
                List<RPTransColateralModel> tmpDelete = new List<RPTransColateralModel>();
                tmpDelete = resultcollold.Data.RPTransColateralResultModel.Where(o => o.status == "delete").ToList();
                resultcollold.Data.RPTransColateralResultModel = resultcollold.Data.RPTransColateralResultModel.Where(o => o.status != "duplicate" && o.status != "NotHave" && o.status != "delete").ToList();
                if (res.Data.RPTransColateralResultModel.Count > 0)
                {
                    resultcollold.Data.RPTransColateralResultModel
                           .Where(c => c.instrument_id.Equals(int.Parse(data.instrumentcode))).ToList()
                           .ForEach(c =>
                           {
                               c.ytm = res.Data.RPTransColateralResultModel[0].ytm;
                               c.dm = res.Data.RPTransColateralResultModel[0].dm;
                               c.clean_price = res.Data.RPTransColateralResultModel[0].clean_price;
                               c.dirty_price = res.Data.RPTransColateralResultModel[0].dirty_price;
                               c.haircut = res.Data.RPTransColateralResultModel[0].haircut;
                               c.variation = res.Data.RPTransColateralResultModel[0].variation;
                               c.unit = res.Data.RPTransColateralResultModel[0].unit;
                               c.market_value = res.Data.RPTransColateralResultModel[0].market_value;
                               c.par = res.Data.RPTransColateralResultModel[0].par;
                               c.cash_amount = res.Data.RPTransColateralResultModel[0].cash_amount;
                               if (rpTran.trigger.ToUpper() != "PAR")
                               {
                                   c.dirty_price_after_hc = res.Data.RPTransColateralResultModel[0].dirty_price_after_hc;
                               }
                               c.wht_amount = res.Data.RPTransColateralResultModel[0].wht_amount;
                               c.interest_amount = res.Data.RPTransColateralResultModel[0].interest_amount;
                               c.terminate_amount = res.Data.RPTransColateralResultModel[0].terminate_amount;
                               if (res.Data.RPTransColateralResultModel[0].terminate_amount != null)
                               {
                                   c.temination_value = Decimal.Parse(res.Data.RPTransColateralResultModel[0].terminate_amount == "" ? "0" : res.Data.RPTransColateralResultModel[0].terminate_amount);
                               }
                               res.Data.RPTransColateralResultModel[0].trigger = data.trigger;
                           });
                }
                resultcollold.Data.RPTransColateralResultModel.AddRange(tmpDelete);
                Session[data.sessionName] = resultcollold.Data.RPTransColateralResultModel;
            }

            if (resultcollold.Data != null && res.Data.RPTransColateralResultModel.Count > 0)
            {
                res.Data.RPTransColateralResultModel[0].sum_cash_amount = resultcollold.Data.RPTransColateralResultModel.Where(x => x.status != "delete").Select(y => y.cash_amount).Sum();
                res.Data.RPTransColateralResultModel[0].sum_interest_amount = resultcollold.Data.RPTransColateralResultModel.Where(x => x.status != "delete").Select(y => y.interest_amount).Sum();
                res.Data.RPTransColateralResultModel[0].sum_temination_value = resultcollold.Data.RPTransColateralResultModel.Where(x => x.status != "delete").Select(y => y.temination_value).Sum();
                res.Data.RPTransColateralResultModel[0].sum_wht_amount = resultcollold.Data.RPTransColateralResultModel.Where(x => x.status != "delete").Select(y => y.wht_amount).Sum();
            }
            else
            {
                resultcollold.Data = new RPTransColateralResult();
                resultcollold.Data.RPTransColateralResultModel = new List<RPTransColateralModel>();
            }

            return Json(res.Data.RPTransColateralResultModel, JsonRequestBehavior.AllowGet);
        }

        public ResultWithModel<RPTransColateralResult> CheckCollateral(RPTransModel model, ResultWithModel<RPTransColateralResult> resultcoll)
        {
            try
            {
                resultcoll.Data.RPTransColateralResultModel[0].status = "add";
                resultcoll.Data.RPTransColateralResultModel[0].datafrom = "ui";

                model.trigger = "CASH_AMOUNT";
                model.RPTransColateralModel = resultcoll.Data.RPTransColateralResultModel[0];
                model.RPTransColateralModel.special_case_id = model.special_case_id;

                ResultWithModel<RPTransColateralResult> res = new ResultWithModel<RPTransColateralResult>();
                api_RPTrans.RPDealEntry.CheckCollateral(model, p =>
                {
                    res = p;
                });
                resultcoll.Data.RPTransColateralResultModel[0].instrument_code = res.Data.RPTransColateralResultModel[0].instrument_code;
                resultcoll.Data.RPTransColateralResultModel[0].ytm = res.Data.RPTransColateralResultModel[0].ytm;
                resultcoll.Data.RPTransColateralResultModel[0].dm = res.Data.RPTransColateralResultModel[0].dm;
                resultcoll.Data.RPTransColateralResultModel[0].clean_price = res.Data.RPTransColateralResultModel[0].clean_price;
                resultcoll.Data.RPTransColateralResultModel[0].dirty_price = res.Data.RPTransColateralResultModel[0].dirty_price;
                resultcoll.Data.RPTransColateralResultModel[0].haircut = res.Data.RPTransColateralResultModel[0].haircut;
                resultcoll.Data.RPTransColateralResultModel[0].variation = res.Data.RPTransColateralResultModel[0].variation;
                resultcoll.Data.RPTransColateralResultModel[0].unit = res.Data.RPTransColateralResultModel[0].unit;
                resultcoll.Data.RPTransColateralResultModel[0].par = res.Data.RPTransColateralResultModel[0].par;
                resultcoll.Data.RPTransColateralResultModel[0].market_value = res.Data.RPTransColateralResultModel[0].market_value;
                resultcoll.Data.RPTransColateralResultModel[0].cash_amount = res.Data.RPTransColateralResultModel[0].cash_amount;
                resultcoll.Data.RPTransColateralResultModel[0].dirty_price_after_hc = res.Data.RPTransColateralResultModel[0].dirty_price_after_hc;
                resultcoll.Data.RPTransColateralResultModel[0].interest_amount = res.Data.RPTransColateralResultModel[0].interest_amount;
                resultcoll.Data.RPTransColateralResultModel[0].wht_amount = res.Data.RPTransColateralResultModel[0].wht_amount;
                resultcoll.Data.RPTransColateralResultModel[0].terminate_amount = res.Data.RPTransColateralResultModel[0].terminate_amount;
                if (res.Data.RPTransColateralResultModel[0].terminate_amount != null)
                {
                    resultcoll.Data.RPTransColateralResultModel[0].temination_value = Decimal.Parse(res.Data.RPTransColateralResultModel[0].terminate_amount == "" ? "0" : res.Data.RPTransColateralResultModel[0].terminate_amount);
                }
            }
            catch
            {
            }
            return resultcoll;
        }

        [HttpPost]
        public ActionResult AddColl(DataTableAjaxPostModel model, string trans_no, string sessionName, bool isCopy = false)
        {
            RPTransModel RPTransModel = new RPTransModel();
            ResultWithModel<RPTransColateralResult> resultcoll = new ResultWithModel<RPTransColateralResult>();
            RPTransColateralModel RPTransColateralModel = new RPTransColateralModel();
            //for add collateral
            resultcoll.Data = new RPTransColateralResult();

            RPTransModel.instrument_code = model.columns[2].search.value;
            RPTransModel.rp_source_value = model.columns[30].search.value;
            RPTransModel.rp_price_date_value = model.columns[29].search.value;
            RPTransModel.purchase_price = model.columns[31].search.value != "" && model.columns[31].search.value != null ? decimal.Parse(model.columns[31].search.value) : 0;
            RPTransModel.counter_party_id = model.columns[32].search.value != "" && model.columns[32].search.value != null ? int.Parse(model.columns[32].search.value) : 0;
            RPTransModel.settlement_date = model.columns[25].search.value != "" && model.columns[25].search.value != null ? utility.ConvertStringToDatetimeFormatDDMMYYYY(model.columns[25].search.value) : RPTransModel.settlement_date;
            RPTransModel.maturity_date = model.columns[19].search.value != "" && model.columns[19].search.value != null ? utility.ConvertStringToDatetimeFormatDDMMYYYY(model.columns[19].search.value) : RPTransModel.maturity_date;

            //for check collateral
            RPTransModel.interest_total = model.columns[33].search.value != "" && model.columns[33].search.value != null ? decimal.Parse(model.columns[33].search.value) : 0;
            RPTransModel.deal_period = model.columns[34].search.value != "" && model.columns[34].search.value != null ? int.Parse(model.columns[34].search.value) : 0;
            RPTransModel.basis_code = model.columns[35].search.value != "" && model.columns[35].search.value != null ? int.Parse(model.columns[35].search.value) : 0;
            RPTransModel.wht_tax = model.columns[36].search.value != "" && model.columns[36].search.value != null ? decimal.Parse(model.columns[36].search.value) : 0;
            RPTransModel.formula = model.columns[37].search.value;
            RPTransModel.cur = model.columns[6].search.value;
            RPTransModel.port_name = model.columns[40].search.value;
            RPTransModel.special_case_id = model.columns[41].search.value != "" && model.columns[41].search.value != null ? int.Parse(model.columns[41].search.value) : 0;

            var maturity_date = model.columns[19].search.value != "" && model.columns[19].search.value != null ? utility.ConvertStringToDatetimeFormatDDMMYYYY(model.columns[19].search.value) : RPTransModel.maturity_date;

            //Add Paging
            PagingModel paging = new PagingModel();
            paging.PageNumber = model.pageno;
            paging.RecordPerPage = model.length;
            RPTransModel.paging = paging;


            if (model.columns[26].search.value == "add")
            {
                if (Session[sessionName] == null)
                {
                    api_RPTrans.RPDealEntry.AddRPTransColat(RPTransModel, p =>
                    {
                        resultcoll = p;
                    });
                    if (resultcoll.Data != null && resultcoll.Data.RPTransColateralResultModel.Count > 0)
                    {
                        resultcoll = CheckCollateral(RPTransModel, resultcoll);
                        api_RPTrans.RPDealEntry.CheckBondSign(
                              RPTransModel.settlement_date.Value.ToString("yyyy-MM-dd"),
                              resultcoll.Data.RPTransColateralResultModel[0].instrument_id.Value,
                              RPTransModel.port_name,
                              RPTransModel.maturity_date.Value.ToString("yyyy-MM-dd"),
                              p =>
                              {
                                  resultcoll.Data.RPTransColateralResultModel[0].message = p.Message;
                              }
                              );

                        // Change Collateral Data Maturity Date As Form Maturity Date When Add Instrument Type "BOTN"
                        if (resultcoll.Data.RPTransColateralResultModel[0].instrument_code.ToUpper().Contains("BOTN"))
                        {
                            resultcoll.Data.RPTransColateralResultModel[0].maturity_date = maturity_date;
                        }

                        Session[sessionName] = resultcoll.Data.RPTransColateralResultModel;
                    }
                    else
                    {
                        RPTransColateralModel trancol = new RPTransColateralModel();
                        trancol.status = "NotHave";
                        resultcoll.Data = new RPTransColateralResult();
                        resultcoll.Data.RPTransColateralResultModel.Add(trancol);
                    }
                }
                else
                {
                    ResultWithModel<RPTransColateralResult> resultcollappend = new ResultWithModel<RPTransColateralResult>();

                    resultcoll.Data = new RPTransColateralResult();
                    resultcoll.Data.RPTransColateralResultModel = (List<RPTransColateralModel>)Session[sessionName];
                    resultcoll.Data.RPTransColateralResultModel = resultcoll.Data.RPTransColateralResultModel.Where(o => o.status != "duplicate" && o.status != "NotHave").ToList();

                    var checkdata = resultcoll.Data.RPTransColateralResultModel.Where(o => RPTransModel.instrument_code == o.instrument_id.ToString() && o.status != "delete").ToList();

                    if (checkdata.Count == 0)
                    {
                        api_RPTrans.RPDealEntry.AddRPTransColat(RPTransModel, p =>
                        {
                            resultcollappend = p;
                        });

                        if (resultcollappend.Data != null && resultcollappend.Data.RPTransColateralResultModel.Count > 0)
                        {
                            resultcollappend = CheckCollateral(RPTransModel, resultcollappend);

                            api_RPTrans.RPDealEntry.CheckBondSign(RPTransModel.settlement_date.Value.ToString("yyyy-MM-dd"),
                                resultcollappend.Data.RPTransColateralResultModel[0].instrument_id.Value,
                                RPTransModel.port_name,
                                RPTransModel.maturity_date.Value.ToString("yyyy-MM-dd"),
                                p =>
                            {
                                resultcollappend.Data.RPTransColateralResultModel[0].message = p.Message;
                            });

                            // Change Collateral Data Maturity Date As Form Maturity Date When Add Instrument Type "BOTN"
                            if (resultcollappend.Data.RPTransColateralResultModel[0].instrument_code.ToUpper().Contains("BOTN"))
                            {
                                resultcollappend.Data.RPTransColateralResultModel[0].maturity_date = maturity_date;
                            }

                            resultcoll.Data.RPTransColateralResultModel.Add(resultcollappend.Data.RPTransColateralResultModel[0]);
                        }
                        else
                        {
                            RPTransColateralModel trancol = new RPTransColateralModel();
                            trancol.status = "NotHave";
                            resultcoll.Data.RPTransColateralResultModel.Add(trancol);
                        }
                        Session[sessionName] = resultcoll.Data.RPTransColateralResultModel;
                    }
                    else
                    {
                        RPTransColateralModel trancol = new RPTransColateralModel();
                        trancol.status = "duplicate";
                        resultcoll.Data.RPTransColateralResultModel.Add(trancol);
                    }
                }

            }
            else if (model.columns[26].search.value == "edit")
            {
                ResultWithModel<RPTransColateralResult> resultcollappend = new ResultWithModel<RPTransColateralResult>();
                resultcoll.Data = new RPTransColateralResult();
                resultcoll.Data.RPTransColateralResultModel = (List<RPTransColateralModel>)Session[sessionName];

                resultcoll.Data.RPTransColateralResultModel
                       .Where(c => c.instrument_code.Equals(RPTransModel.instrument_code) && c.status != "delete").ToList()
                       .ForEach(c =>
                       {
                           if (model.columns[28].search.value == "ui")
                           {
                               c.status = "add";
                           }
                           else
                           {
                               c.status = "edit";
                           }
                           c.port = model.columns[4].search.value;
                           c.port_name = model.columns[4].search.value;
                           c.ytm = model.columns[7].search.value != "" && model.columns[7].search.value != null ? decimal.Parse(model.columns[7].search.value) : 0;
                           c.dm = model.columns[8].search.value != "" && model.columns[8].search.value != null ? decimal.Parse(model.columns[8].search.value) : 0;
                           c.clean_price = model.columns[9].search.value != "" && model.columns[9].search.value != null ? decimal.Parse(model.columns[9].search.value) : 0;
                           c.dirty_price = model.columns[10].search.value != "" && model.columns[10].search.value != null ? decimal.Parse(model.columns[10].search.value) : 0;
                           //c.haircut_rate = model.columns[10].search.value != "" ? decimal.Parse(model.columns[10].search.value) : 0;
                           c.haircut = model.columns[11].search.value != "" && model.columns[11].search.value != null ? decimal.Parse(model.columns[11].search.value) : 0;
                           c.variation = model.columns[23].search.value != "" && model.columns[23].search.value != null ? decimal.Parse(model.columns[23].search.value) : 0;
                           c.unit = model.columns[12].search.value != "" && model.columns[12].search.value != null ? decimal.Parse(model.columns[12].search.value) : 0;
                           c.par = model.columns[13].search.value != "" && model.columns[13].search.value != null ? decimal.Parse(model.columns[13].search.value) : 0;
                           c.market_value = model.columns[14].search.value != "" && model.columns[14].search.value != null ? decimal.Parse(model.columns[14].search.value) : 0;
                           c.cash_amount = model.columns[15].search.value != "" && model.columns[15].search.value != null ? decimal.Parse(model.columns[15].search.value) : 0;
                           c.message = string.Empty;

                       });

                foreach (var item in resultcoll.Data.RPTransColateralResultModel)
                {
                    item.message = string.Empty;
                }

                Session[sessionName] = resultcoll.Data.RPTransColateralResultModel;
            }
            else if (model.columns[26].search.value == "delete")
            {
                ResultWithModel<RPTransColateralResult> resultcollappend = new ResultWithModel<RPTransColateralResult>();
                resultcoll.Data = new RPTransColateralResult();
                resultcoll.Data.RPTransColateralResultModel = (List<RPTransColateralModel>)Session[sessionName];
                if (model.columns[28].search.value == "ui")
                {

                    //var result = resultcoll.Data.RPTransColateralResultModel.Where(o => RPTransModel.instrument_code == o.instrument_code && o.datafrom == "ui").ToList();

                    resultcoll.Data.RPTransColateralResultModel.RemoveAll(o => RPTransModel.instrument_code == o.instrument_code && o.datafrom == "ui");
                    //resultcoll.Data.RPTransColateralResultModel = result;
                }
                else
                {
                    if (resultcoll.Data != null && resultcoll.Data.RPTransColateralResultModel != null)
                        resultcoll.Data.RPTransColateralResultModel.Where(c => c.instrument_code == RPTransModel.instrument_code).ToList().ForEach(c => { c.status = "delete"; });
                }

                foreach (var item in resultcoll.Data.RPTransColateralResultModel)
                {
                    item.message = string.Empty;
                }

                Session[sessionName] = resultcoll.Data.RPTransColateralResultModel;
            }
            else if (model.columns[26].search.value == "cancel")
            {
                ResultWithModel<RPTransColateralResult> resultcollappend = new ResultWithModel<RPTransColateralResult>();
                resultcoll.Data = new RPTransColateralResult();
                resultcoll.Data.RPTransColateralResultModel = (List<RPTransColateralModel>)Session[sessionName];

                Session[sessionName] = resultcoll.Data.RPTransColateralResultModel;

                foreach (var item in resultcoll.Data.RPTransColateralResultModel)
                {
                    item.message = string.Empty;
                }
            }
            else
            {
                if (Session[sessionName] == null)
                {
                    RPTransColateralModel.trans_no = trans_no;
                    api_RPTrans.RPDealEntry.GetRPDealEntryColateralList(RPTransColateralModel, p =>
                    {
                        resultcoll = p;
                    });

                    if (resultcoll.Data.RPTransColateralResultModel.Count > 0)
                    {
                        foreach (var item in resultcoll.Data.RPTransColateralResultModel)
                        {
                            if (isCopy)
                            {
                                item.status = "add";
                                item.datafrom = "ui";
                            }
                            else
                            {
                                item.status = "edit";
                            }

                            Session[sessionName] = resultcoll.Data.RPTransColateralResultModel;
                        }
                    }
                    else
                    {
                        ResultWithModel<RPTransColateralResult> resultcollappend = new ResultWithModel<RPTransColateralResult>();
                        resultcoll.Data = new RPTransColateralResult();
                        //resultcoll.Data.RPTransColateralResultModel = (List<RPTransColateralModel>)Session[sessionName];
                        Session[sessionName] = resultcoll.Data.RPTransColateralResultModel;
                    }

                    foreach (var item in resultcoll.Data.RPTransColateralResultModel)
                    {
                        item.message = string.Empty;
                    }
                }

            }

            if (resultcoll.Data.RPTransColateralResultModel.Count > 0)
            {
                resultcoll.Data.RPTransColateralResultModel[0].sum_cash_amount = resultcoll.Data.RPTransColateralResultModel.Where(x => x.status != "delete").Select(y => y.cash_amount).Sum();
                resultcoll.Data.RPTransColateralResultModel[0].sum_interest_amount = resultcoll.Data.RPTransColateralResultModel.Where(x => x.status != "delete").Select(y => y.interest_amount).Sum();
                resultcoll.Data.RPTransColateralResultModel[0].sum_wht_amount = resultcoll.Data.RPTransColateralResultModel.Where(x => x.status != "delete").Select(y => y.wht_amount).Sum();
                resultcoll.Data.RPTransColateralResultModel[0].sum_temination_value = resultcoll.Data.RPTransColateralResultModel.Where(x => x.status != "delete").Select(y => y.temination_value).Sum();

                int i = 1;
                foreach (var item in resultcoll.Data.RPTransColateralResultModel.Where(x => x.status != "delete"))
                {
                    item.RowNumber = i++;
                }
                resultcoll.HowManyRecord = i - 1;
            }

            return Json(new
            {
                draw = model.draw,
                recordsTotal = resultcoll.HowManyRecord,
                recordsFiltered = resultcoll.HowManyRecord,
                data = resultcoll.Data != null && resultcoll.Data.RPTransColateralResultModel != null ? resultcoll.Data.RPTransColateralResultModel.Where(x => x.status != "delete").ToList() : new List<RPTransColateralModel>(),
            });
        }

        public class Data
        {
            public string tradedate { get; set; }
            public string settlementdate { get; set; }
            public string maturitydate { get; set; }
            public string instrumenttype { get; set; }
            public string purchaseprice { get; set; }
            public string period { get; set; }
            public string cur { get; set; }
            public string wht { get; set; }
            public string inttype { get; set; }
            public string intrate { get; set; }
            public string intspread { get; set; }
            public string transno { get; set; }
            public string transtype { get; set; }
            public string yearbasis { get; set; }
            public string totalint { get; set; }
            public string instrumentcode { get; set; }
            public string parunit { get; set; }
            public string ytm { get; set; }
            public string dm { get; set; }
            public string cleanprice { get; set; }
            public string dirtyprice { get; set; }
            public string hc { get; set; }
            public string vm { get; set; }
            public string unit { get; set; }
            public string parval { get; set; }
            public string secmarketval { get; set; }
            public string cashamount { get; set; }
            public string trigger { get; set; }
            public string formula { get; set; }
            public string rpsource { get; set; }
            public string rppricedate { get; set; }
            public string counterpartyid { get; set; }
            public string repo_deal_type { get; set; }
            public string counter_party_id { get; set; }
            public decimal exch_rate { get; set; }
            public char tenor_type { get; set; }
            public decimal wht_amount { get; set; }
            public decimal interest_amount { get; set; }
            public bool isLastRecord { get; set; }
            public string specialCaseId { get; set; }

            public string sessionName { get; set; }
        }

        public ActionResult GetCallPrice(Data data)//string tradedate, string settlementdate, string instrumenttype, string purchaseprice, string period, string wht, string intrate, string intspread)
        {
            List<RPTransModel> res = new List<RPTransModel>();
            try
            {
                //  RPTransResult res = new RPTransResult();

                RPTransModel RPTran = new RPTransModel();
                RPTran.purchase_price = string.IsNullOrEmpty(data.purchaseprice) == true ? 0 : decimal.Parse(data.purchaseprice);
                RPTran.deal_period = string.IsNullOrEmpty(data.period) == true ? 0 : int.Parse(data.period);
                RPTran.wht_tax = string.IsNullOrEmpty(data.wht) == true ? 0 : decimal.Parse(data.wht);
                RPTran.basis_code = string.IsNullOrEmpty(data.yearbasis) == true ? 0 : int.Parse(data.yearbasis);
                RPTran.interest_total = string.IsNullOrEmpty(data.totalint) == true ? 0 : decimal.Parse(data.totalint);
                RPTran.counter_party_id = string.IsNullOrEmpty(data.counter_party_id) == true ? 0 : int.Parse(data.counter_party_id);
                RPTran.cur = data.cur;
                RPTran.repo_deal_type = data.repo_deal_type;
                RPTran.trans_deal_type = data.instrumenttype;

                api_RPTrans.RPDealEntry.GetCallPrice(RPTran, p =>
                {
                    if (p.Success)
                    {
                        res = p.Data.RPTransResultModel;
                    }
                });
                if (res.Count == 0)
                {
                    res = new List<RPTransModel>();
                }
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                RPTransModel model = new RPTransModel();
                model.message = ex.Message.ToString();
                res.Add(model);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetRPDealCheckDate(Data data)//string tradedate, string settlementdate, string instrumenttype, string purchaseprice, string period, string wht, string intrate, string intspread)
        {
            List<RPTransModel> res = new List<RPTransModel>();
            RPTransModel RPTran = new RPTransModel();
            RPTran.trade_date = string.IsNullOrEmpty(data.tradedate) == true ? RPTran.trade_date : utility.ConvertStringToDatetimeFormatDDMMYYYY(data.tradedate);
            RPTran.settlement_date = string.IsNullOrEmpty(data.settlementdate) == true ? RPTran.settlement_date : utility.ConvertStringToDatetimeFormatDDMMYYYY(data.settlementdate);
            RPTran.maturity_date = string.IsNullOrEmpty(data.maturitydate) == true ? RPTran.maturity_date : utility.ConvertStringToDatetimeFormatDDMMYYYY(data.maturitydate);
            RPTran.deal_period = string.IsNullOrEmpty(data.period) == true ? 0 : int.Parse(data.period);
            RPTran.cur = string.IsNullOrEmpty(data.cur) == true ? RPTran.cur : data.cur;

            api_RPTrans.RPDealEntry.GetRPDealCheckDate(RPTran, p =>
            {
                if (p.Success)
                {
                    res = p.Data.RPTransResultModel;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPeriod(Data data)//string tradedate, string settlementdate, string instrumenttype, string purchaseprice, string period, string wht, string intrate, string intspread)
        {
            List<RPTransModel> res = new List<RPTransModel>();
            RPTransModel RPTran = new RPTransModel();
            RPTran.settlement_date = string.IsNullOrEmpty(data.settlementdate) == true ? RPTran.settlement_date : utility.ConvertStringToDatetimeFormatDDMMYYYY(data.settlementdate);
            RPTran.deal_period = string.IsNullOrEmpty(data.period) == true ? 0 : int.Parse(data.period);
            RPTran.cur = string.IsNullOrEmpty(data.cur) == true ? RPTran.cur : data.cur;
            RPTran.trans_type = data.transtype;
            RPTran.tenor_type = data.tenor_type;


            api_RPTrans.RPDealEntry.GetPeriod(RPTran, p =>
            {
                if (p.Success)
                {
                    res = p.Data.RPTransResultModel;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRPTransCheckInterest(Data data)
        {
            ResultWithModel<RPTransCheckInterestResult> result = new ResultWithModel<RPTransCheckInterestResult>();
            RPTransCheckInterestModel CheckInt = new RPTransCheckInterestModel();
            CheckInt.trans_no = data.transno;
            api_RPTrans.RPDealEntry.GetRPDealEntryCheckIntList(CheckInt, p =>
            {
                if (p.Success)
                {
                    result = p;
                }
            });

            return Json(result.Data.RPTransCheckInterestResultModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRPTransCheckCostOfFund(Data data)
        {
            ResultWithModel<RPTransCheckCostOfFundResult> result = new ResultWithModel<RPTransCheckCostOfFundResult>();
            RPTransCheckCostOfFundModel CheckInt = new RPTransCheckCostOfFundModel();
            CheckInt.trans_no = data.transno;
            api_RPTrans.RPDealEntry.GetRPDealEntryCheckCostOfFundList(CheckInt, p =>
            {
                if (p.Success)
                {
                    result = p;
                }
            });

            return Json(result.Data.RPTransCheckCostOfFundResultModel, JsonRequestBehavior.AllowGet);
        }

        public RPTransModel SetStatusFlag(RPTransModel model, string page_name)
        {
            RPTransModel trans = model;

            trans.trans_state = "FO-Create";
            trans.trans_status = "Open";
            if (page_name == "addpage")
            {
                trans.trans_state = "FO-Create";
                trans.trans_status = "Open";
            }
            else if (page_name == "editpage")
            {
                trans.trans_state = "FO-Create";
                trans.trans_status = "Collect";
            }
            return trans;
        }

        public DateTime Getbusinessdate()
        {
            StaticEntities db = new StaticEntities();
            ResultWithModel<BusinessDateResult> rwm = new ResultWithModel<BusinessDateResult>();
            BusinessDateModel BusinessDateModel = new BusinessDateModel();
            DateTime Date = new DateTime();

            //Add Paging
            PagingModel paging = new PagingModel();
            paging.PageNumber = 1;
            paging.RecordPerPage = 20;
            BusinessDateModel.paging = paging;

            db.BusinessDate.GetBusinessDateList(BusinessDateModel, p =>
            {
                rwm = p;
            });

            if (rwm.Success)
            {
                string StrDate = string.Empty;
                List<BusinessDateModel> ListData = rwm.Data.BusinessDateResultModel;
                Date = rwm.Data.BusinessDateResultModel[0].business_date;
            }
            return Date;
        }

        public ActionResult GetBilatContractFromAjax(Data data)
        {
            RPTransModel model = new RPTransModel();
            model.trans_deal_type = data.instrumenttype;
            model.interest_type = data.inttype;
            model.trans_type = data.transtype;
            model.deal_period = string.IsNullOrEmpty(data.period) == true ? 0 : int.Parse(data.period);
            model.repo_deal_type = data.repo_deal_type;
            model.trade_date = string.IsNullOrEmpty(data.tradedate) == true ? model.trade_date : utility.ConvertStringToDatetimeFormatDDMMYYYY(data.tradedate);

            ResultWithModel<RPTransResult> resultbailat = GetBilatContract(model);

            #region Update DataCollateList BOTN
            List<RPTransColateralModel> listColl = (List<RPTransColateralModel>)Session[data.sessionName];
            string botn_isin_code = "";
            string bilateral_contract_no = resultbailat.Data.RPTransResultModel != null ? resultbailat.Data.RPTransResultModel.FirstOrDefault().bilateral_contract_no : "";
            if (listColl != null)
            {
                listColl.ForEach(delegate (RPTransColateralModel Item)
                {
                    if (Item.instrument_code.Contains("BOTN"))
                    {
                        if (!string.IsNullOrEmpty(Item.isin_code))
                        {
                            botn_isin_code = Item.isin_code.Contains("-") ? Item.isin_code.Split('-')[0] + "-" : "BOTN-";
                        }
                        else
                        {
                            botn_isin_code = "";
                        }
                        Item.isin_code = botn_isin_code + bilateral_contract_no;
                    }
                });

                Session[data.sessionName] = listColl;
            }
            #endregion

            return Json(resultbailat.Data.RPTransResultModel, JsonRequestBehavior.AllowGet);
        }

        public ResultWithModel<RPTransResult> GetBilatContract(RPTransModel model)
        {
            ResultWithModel<RPTransResult> resultbailat = new ResultWithModel<RPTransResult>();
            try
            {
                api_RPTrans.RPDealEntry.GetRPDealBilatNo(model, p =>
                {
                    if (p.Success)
                    {
                        resultbailat = p;
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resultbailat;
        }

        //Start Function Get List<DDLItemModel>
        public List<DDLItemModel> GetDDLTransType(string datastr)
        {

            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLTransType(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return res;
        }

        public List<DDLItemModel> GetDDLInstrumentType(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLInstrumentType(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return res;
        }

        public List<DDLItemModel> GetDDLCounterParty(string datastr, string instrumentcode, string dealtype)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLCounterParty(datastr, instrumentcode, dealtype, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return res;
        }

        public List<DDLItemModel> GetDDLCounterPartyById(string datastr, string instrumentcode, string dealtype)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLCounterPartyById(datastr, instrumentcode, dealtype, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return res;
        }

        public List<DDLItemModel> GetDDLBasisCode(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLBasisCode(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return res;
        }

        public List<DDLItemModel> GetDDLPurpose(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLPurpose(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return res;
        }

        public List<DDLItemModel> GetDDLDesk(string userid, string port, string instrument)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLDeskGroup(userid, port, instrument, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return res;
        }

        public List<DDLItemModel> GetDeskGroup(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLDeskGroup(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return res;
        }
        //END Function Get List<DDLItemModel>

        #region 
        //DropDownList
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

        public ActionResult FillPortUser(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLPortfolio(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillPortTrans(string port)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLPortfolioTrans(port, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillAppendDeal(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.Config.GetConfigForDropdown("EXTEND_DEAL", p =>
            {
                if (p.Success)
                {
                    foreach (ConfigModel item in p.Data.ConfigResultModel)
                    {
                        DDLItemModel list = new DDLItemModel();
                        list.Text = item.item_desc;
                        list.Value = item.item_code;
                        res.Add(list);
                    }
                }
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillDesk(string userid, string port, string instrument)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLDeskGroup(userid, port, instrument, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillDealer(string datastr, string port)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLDealerCode(datastr, port, p =>
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
            res = GetDDLInstrumentType(datastr);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillTransType(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            res = GetDDLTransType(datastr);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillPurpose(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLPurpose(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillRpIntTerm(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLRpIntTerm(datastr, p =>
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
            api_RPTrans.RPDealEntry.GetDDLCurrency(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCounterparty(string datastr, string instrumentcode, string dealtype, bool byId = false)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            if (byId)
            {
                res = GetDDLCounterPartyById(datastr, instrumentcode, dealtype);
            }
            else
            {
                res = GetDDLCounterParty(datastr, instrumentcode, dealtype);
            }


            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCounterPartyFund(string counterpartyid)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetCounterPartyFundByCounterPartyID(counterpartyid, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCounterPartyPayment(string counterpartyid, string payment_flag)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLPaymentMethodByCounterPartyID(counterpartyid, payment_flag, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillMarginPayment(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLPaymentMethod(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillInstrumentCode(string instrument, string cur_pair2, string repo_deal_type = null)//, string maturitydate)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLInstrument(instrument, cur_pair2, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            // Filter new condition if repo deral type = Private Repo to remove item start wording "BOTN" .
            if (repo_deal_type == "PRP")
            {
                res = res.Where(x => !x.Text.StartsWith("BOTN")).ToList();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillISINcode(string isincode, string instrumentcode, string cur)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLISINcode(isincode, instrumentcode, cur, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillBasisCode(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            res = GetDDLBasisCode(datastr);

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillRemark(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLTransRemark(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDDLMarketPrice(string pricesource, string tradedate, string settlementdate, string curr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLMarketPrice(pricesource, tradedate, settlementdate, curr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDDLPriceSource()
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLPriceSource(p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillFloatRate(string cur, string date)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLFloatRate(cur, date, p =>
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
            api_RPTrans.RPDealEntry.GetDDLMTCode(paymentmethod, transdealtype, cur, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillFormula(string curpair2, string counterpartyid)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLFormula(curpair2, counterpartyid, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillSpecialCase(string search)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLSpecialCase(search, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        //End DropDownList

        /// <summary>
        ///  Check Over limit for you.
        /// </summary>
        /// <param name="RPTranModel"></param>
        /// <returns></returns>
        public ActionResult CheckLimit(RPTransModel model)
        {
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

        [HttpPost]
        public ActionResult GetAbsorb(int counter_party_id)
        {
            try
            {
                string absorb = "%";
                api_RPTrans.RPDealEntry.CheckAbsorb(counter_party_id, p =>
                {
                    absorb = p.Message;
                });

                return Json(new
                {
                    absorb = absorb,
                    returnCode = 0
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    absorb = "%",
                    returnCode = 1,
                    Message = ex.ToString()
                });
            }
        }

        public string GetAbsorbForString(int counter_party_id)
        {
            try
            {
                string absorb = "%";
                api_RPTrans.RPDealEntry.CheckAbsorb(counter_party_id, p =>
                {
                    absorb = p.Message;
                });

                return absorb;
            }
            catch (Exception ex)
            {
                return "%";
            }
        }

        public List<DDLItemModel> GetDealer(string datastr, string port)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_RPTrans.RPDealEntry.GetDDLDealerCode(datastr, port, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return res;
        }

        [HttpPost]
        public ActionResult SetPortColl(int instrument_id, string port, string sessionName)
        {
            try
            {
                List<RPTransColateralModel> listColl = (List<RPTransColateralModel>)Session[sessionName];
                foreach (var item in listColl)
                {
                    if (item.instrument_id == instrument_id)
                    {
                        item.port = port;
                        item.port_name = port;
                    }
                }

                Session[sessionName] = listColl;

                return Json(new
                {
                    returnCode = 0
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    returnCode = 1,
                    Message = ex.ToString()
                });
            }
        }

        #endregion

        // Action : Cancel

        public ActionResult Select(string id)
        {
            List<string> ListTrans = new List<string>();
            ListTrans = JsonConvert.DeserializeObject<List<string>>(id);
            ListTrans.Sort();
            Session["ListTrans"] = ListTrans;

            return RedirectToAction("Cancel", new { trans_no = ListTrans[0] });
        }

        public ActionResult Cancel(string trans_no)
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

                api_RPTrans.RPDealEntry.RPDealEntryList(RPTransModel, p =>
                {
                    result = p;
                });

                RPTransModel = result.Data.RPTransResultModel[0];
                //RPTransModel.counter_party_name = RPTransModel.counter_party_name.Split(':')[1].Trim();
                RPTransModel.counter_party_fund_name = RPTransModel.counter_party_fund_name == null ? "None" : RPTransModel.counter_party_fund_name;

                // Step 2 : Set Enable Btn_PreviousNext
                if (Enable_BtnPreviousNext(ref StrMsg, ref RPTransModel) == false)
                {
                    throw new Exception("Enable_BtnPreviousNext() => " + StrMsg);
                }

                // Step 3 : Check Trans State
                if (RPTransModel.trans_state.ToUpper() == "FO-CREATE" || RPTransModel.trans_state.ToUpper() == "FO-REJECTAPPROVE")
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
            RPTransModel modelTrans = new RPTransModel();

            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                modelTrans.paging = paging;

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

                modelTrans.ordersby = orders;
                modelTrans.trans_no = trans_no;

                api_RPTrans.RPDealVerify.GetRPDealVerifyColateralList(modelTrans, p =>
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
        public ActionResult Cancel_Trans(RPTransModel model)
        {
            string StrMsg = string.Empty;
            string Next_TransNo = string.Empty;
            var Result = new List<object>();
            bool success = false;
            try
            {
                ResultWithModel<RPTransResult> rwm = new ResultWithModel<RPTransResult>();
                //Step 1 : Cancel TransNo
                model.create_by = HttpContext.User.Identity.Name;
                model.trans_status = "Cancel";

                api_RPTrans.RPDealEntry.UpdateTrans(model, p =>
                {
                    rwm = p;
                });

                if (!rwm.Success)
                {
                    throw new Exception("UpdateTrans() => " + rwm.Message);
                }
                else
                {
                    success = true;
                    bool IsPayment = false;
                    //Step 2 : Check
                    if (!CheckReleaseMessageConfirm(ref StrMsg, ref IsPayment, model))
                    {
                        Log.WriteLog(Controller, StrMsg);
                        throw new Exception("CheckReleaseMessageConfirm() => " + StrMsg);
                    }

                    //Step 3 : ReleaseMessage Confirm
                    if (IsPayment == true && model.repo_deal_type == "BRP" && model.trans_deal_type == "LD")
                    {
                        if (!ReleaseMessageConfirm(ref StrMsg, model))
                        {
                            Log.WriteLog(Controller, StrMsg);
                            throw new Exception("But Release Message Fail => " + StrMsg);
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                StrMsg = Ex.Message;
            }

            Result.Add(new { Message = StrMsg, Success = success });
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CheckHoliday(string date, string cur)
        {
            bool isHoliday = false;
            StaticEntities staticApi = new StaticEntities();
            ResultWithModel<RpHolidayResult> rwm = new ResultWithModel<RpHolidayResult>();

            RpHolidayModel model = new RpHolidayModel();
            model.check_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(date);
            model.cur = cur;
            staticApi.Holiday.CheckHoliday(model, p =>
            {
                rwm = p;
            });

            if (rwm.Success)
            {
                if (rwm.Data != null)
                {
                    isHoliday = rwm.Data.RpHolidayResultModel[0].is_holiday;
                }
            }

            return Json(isHoliday, JsonRequestBehavior.AllowGet);
        }


        private bool CheckReleaseMessageConfirm(ref string ReturnMsg, ref bool IsPayment, RPTransModel RPTransModel)
        {
            try
            {
                PaymentProcessEntities api_RPReleaseMsg = new PaymentProcessEntities();
                ResultWithModel<RPReleaseMsgCheckPaymentResult> Result = new ResultWithModel<RPReleaseMsgCheckPaymentResult>();
                RPReleaseMsgCheckPaymentModel ChkPaymentModel = new RPReleaseMsgCheckPaymentModel();

                ChkPaymentModel.from_page = "Settlement";
                ChkPaymentModel.event_type = "CANCEL";
                ChkPaymentModel.trans_deal_type = RPTransModel.trans_deal_type;
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

        private bool ReleaseMessageConfirm(ref string ReturnMsg, RPTransModel model)
        {
            try
            {
                RPTransModel modelConfirm = new RPTransModel();
                modelConfirm.trans_no = model.trans_no;
                modelConfirm.cur = model.cur;
                modelConfirm.event_type = "CANCEL";
                modelConfirm.message_type = "CANCEL";
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
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
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

        [HttpPost]
        public ActionResult GetPopupRefno(RPTransModel model)
        {
            var result = new ResultWithModel<RPTransResult>();
            try
            {
                api_RPTrans.RPDealEntry.GetPopupRefno(model, p =>
                {
                    result = p;
                });
                return Json(new
                {
                    returnCode = 0,
                    data = result.Data != null ? result.Data.RPTransResultModel : new List<RPTransModel>()
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    returnCode = 1,
                    Message = ex.ToString()
                });
            }
        }

        [HttpPost]
        public ActionResult CompareDealPrice(decimal price1, string cur1, decimal price2, string cur2)
        {
            string isEqual = "N";
            try
            {
                api_RPTrans.RPDealEntry.CompareDealPrice(price1, cur1, price2, cur2, p =>
                {
                     isEqual = p.Message;
                });

                return Json(new
                {
                    isEqual = isEqual,
                    returnCode = 0
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isEqual = "N",
                    returnCode = 1,
                    Message = ex.ToString()
                });
            }
        }

    }
}