using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.CounterParty;
using GM.Data.Model.ExternalInterface;
using GM.Data.Model.Security;
using GM.Data.Model.Static;
using GM.Data.Result.CounterParty;
using GM.Data.Result.ExternalInterface;
using GM.Data.Result.Security;
using GM.Data.Result.Static;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class SecurityController : BaseController
    {
        private StaticEntities api_static = new StaticEntities();
        ExternalInterfaceEntities api_externalInterface = new ExternalInterfaceEntities();
        SecurityEntities api_security = new SecurityEntities();
        AgencyAndRatingEntities api_agencyandrating = new AgencyAndRatingEntities();
        CounterPartyEntities api_counterparty = new CounterPartyEntities();
        Utility utility = new Utility();

        // GET: Security
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            ResultWithModel<SecurityResult> result = new ResultWithModel<SecurityResult>();
            SecurityModel securitymodel = new SecurityModel();
            try
            {
                // string searchvalue = Request["search[value]"];

                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                securitymodel.paging = paging;

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

                securitymodel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null).ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "instrument_code":
                            securitymodel.instrument_code = column.search.value;
                            break;
                        case "instrument_desc":
                            securitymodel.instrument_desc = column.search.value;
                            break;
                        case "instrumenttype":
                            securitymodel.instrumenttype = column.search.value;
                            break;
                        case "ISIN_code":
                            securitymodel.ISIN_code = column.search.value;
                            break;
                        case "issue_date":
                            securitymodel.issue_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "maturity_date":
                            securitymodel.maturity_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "issuer_name":
                            securitymodel.issuer_name = column.search.value;
                            break;
                        case "redemp_method":
                            securitymodel.redemp_method = column.search.value;
                            break;
                    }
                });

                api_security.Security.GetSecurityList(securitymodel, p =>
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
                data = result.Data != null ? result.Data.SecurityResultModel : null
            });
        }

        public List<SecurityEventModel> EventdataModel { get; set; }

        [HttpPost]
        public ActionResult CreatCashFlow(DataTableAjaxPostModel model, string instrument_id)
        {
            if (!string.IsNullOrEmpty(instrument_id))
            {
                ResultWithModel<SecurityEventResult> resultevents = new ResultWithModel<SecurityEventResult>();
                SecurityEventModel SecurityEvent = new SecurityEventModel();
                SecurityEvent.instrument_id = int.Parse(instrument_id);
                SecurityEvent.paging = new PagingModel();
                SecurityEvent.paging.PageNumber = model.pageno;
                SecurityEvent.paging.RecordPerPage = model.length;

                //if (Session["DataEventList"] == null)
                //{
                //    api_security.Security.GetSecurityEventsList(SecurityEvent, p =>
                //    {
                //        resultevents = p;
                //    });
                //    Session["DataEventList"] = resultevents.Data.SecurityEventsResultModel;
                //}
                //else
                //{
                //    resultevents.Data = new SecurityEventResult();
                //    resultevents.Data.SecurityEventsResultModel = (List<SecurityEventModel>)Session["DataEventList"];
                //}

                api_security.Security.GetSecurityEventsList(SecurityEvent, p =>
                {
                    resultevents = p;
                });

                return Json(new
                {
                    draw = model.draw,
                    recordsTotal = resultevents.HowManyRecord,
                    recordsFiltered = resultevents.HowManyRecord,
                    Message = resultevents.Message,
                    data = resultevents.Data.SecurityEventsResultModel
                });
            }
            else
            {
                SecurityEventModel EventModel = new SecurityEventModel();
                ResultWithModel<SecurityEventResult> result = new ResultWithModel<SecurityEventResult>();
                SecurityEventResult EventresModel = new SecurityEventResult();
                result.Data = EventresModel;
                try
                {
                  
                    if (Session["DataEventList"] != null)
                    {
                        result.Data.SecurityEventsResultModel = (List<SecurityEventModel>)Session["DataEventList"];
                    }
                    
                   var columns = model.columns.Where(o => o.search.value != null).ToList();
                    if (columns.Count > 0)
                    {
                        columns.ForEach(column =>
                        {
                            switch (column.data)
                            {
                                case "round_no":
                                    EventModel.round_no = int.Parse(column.search.value);
                                    break;
                                case "event_date":
                                    EventModel.event_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                                    break;
                                case "payment_date":
                                    EventModel.payment_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                                    break;
                                case "xi_date":
                                    EventModel.xi_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                                    break;
                                case "start_date":
                                    EventModel.start_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                                    break;
                                case "end_date":
                                    EventModel.end_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                                    break;
                                //case "coupon_date":
                                //    EventModel.coupon_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                                //    break;
                                //case "next_coupon_date":
                                //    EventModel.next_coupon_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                                //    break;
                                case "begining_par":
                                    EventModel.begining_par = decimal.Parse(column.search.value);
                                    break;
                                case "ending_par":
                                    EventModel.ending_par = decimal.Parse(column.search.value);
                                    break;
                                case "coupon_rate":
                                    EventModel.coupon_rate = decimal.Parse(column.search.value);
                                    break;
                                case "interest":
                                    EventModel.interest = decimal.Parse(column.search.value);
                                    break;
                                case "principal":
                                    EventModel.principal = decimal.Parse(column.search.value);
                                    break;
                                case "total_payment":
                                    EventModel.total_payment = decimal.Parse(column.search.value);
                                    break;
                                case "event_type":
                                    EventModel.event_type = column.search.value;
                                    break;
                                case "coupon_type":
                                    EventModel.coupon_type = column.search.value;
                                    break;
                                case "coupon_floating_index_code":
                                    EventModel.coupon_floating_index_code = column.search.value;
                                    break;
                                case "coupon_spread":
                                    EventModel.coupon_spread = decimal.Parse(column.search.value);
                                    break;
                                case "ai_amount":
                                    EventModel.ai_amount = decimal.Parse(column.search.value);
                                    break;
                                case "redemption_percent":
                                    EventModel.redemption_percent = decimal.Parse(column.search.value);
                                    break;
                                case "complete_flag":
                                    EventModel.complete_flag = Boolean.Parse(column.search.value);
                                    break;
                                case "datafrom":
                                    EventModel.datafrom = column.search.value;
                                    break;
                                case "rowno":
                                    EventModel.rowno = column.search.value;
                                    break;
                                case "action":
                                    EventModel.action = column.search.value;
                                    break;
                            }
                        });

                    }
                    if (model.columns[21].search.value == "create")
                    {
                        result.Data.SecurityEventsResultModel.Add(EventModel);
                    }
                    else if (model.columns[21].search.value == "update")
                    {
                        result.Data.SecurityEventsResultModel[int.Parse(model.columns[20].search.value)] = EventModel;
                    }
                    else if (model.columns[21].search.value == "delete")
                    {
                       var datafrom = result.Data.SecurityEventsResultModel[int.Parse(model.columns[20].search.value)].datafrom;
                        if (datafrom == "fromui")
                        {
                            result.Data.SecurityEventsResultModel.RemoveAt(int.Parse(model.columns[20].search.value));
                        }
                        else
                        {
                            result.Data.SecurityEventsResultModel[int.Parse(model.columns[20].search.value)].action = "delete";

                        }
                    }
                    Session["DataEventList"] = result.Data.SecurityEventsResultModel;
                }
                catch (Exception ex)
                {

                }
                return Json(new
                {
                    draw = model.draw,
                    recordsTotal = result.Data.SecurityEventsResultModel.Count(),
                    recordsFiltered = result.Data.SecurityEventsResultModel.Count(),
                    Message = result.Message,
                    data = result.Data != null ? result.Data.SecurityEventsResultModel : new List<SecurityEventModel>()
                });
            }
        }

        //[HttpPost]
        //public ActionResult CreatCashFlow(DataTableAjaxPostModel model)
        //{
        //    ResultWithModel<SecurityEventResult> result = new ResultWithModel<SecurityEventResult>();
        //    SecurityEventResult EventresModel = new SecurityEventResult();
        //    result.Data = EventresModel;
        //    try
        //    {
        //        //eventmodel.round_no = 17;
        //        //eventmodel.begining_par = 1000;
        //        DataTable dt = new DataTable();
        //        if (Session["DataEventList"] != null)
        //        {
        //            dt = Session["DataEventList"] as DataTable;
        //        }
        //        else
        //        {
        //            dt.Columns.Add("round_no");
        //            dt.Columns.Add("event_date");
        //            dt.Columns.Add("payment_date");
        //            dt.Columns.Add("xi_date");
        //            dt.Columns.Add("start_date");
        //            dt.Columns.Add("end_date");
        //            dt.Columns.Add("begining_par");
        //            dt.Columns.Add("ending_par");
        //            dt.Columns.Add("coupon_rate");
        //            dt.Columns.Add("interest");
        //            dt.Columns.Add("principal");
        //            dt.Columns.Add("total_payment");
        //            dt.Columns.Add("event_type");
        //            dt.Columns.Add("coupon_type");
        //            dt.Columns.Add("coupon_floating_index_code");
        //            dt.Columns.Add("coupon_spread");
        //            dt.Columns.Add("ai_amount");
        //            dt.Columns.Add("redemption_percent");
        //            dt.Columns.Add("complete_flag");
        //        }
        //        DataRow dr = dt.NewRow();

        //        //var columns = model.columns.Where(o => o.search.value != null).ToList();
        //        //if (columns.Count > 0)
        //        //{
        //        //    columns.ForEach(column =>
        //        //    {
        //        //        switch (column.data)
        //        //        {
        //        //            case "round_no":
        //        //                dr["round_no"] = column.search.value;
        //        //                break;
        //        //            case "event_date":
        //        //                dr["event_date"] = column.search.value;
        //        //                break;
        //        //            case "payment_date":
        //        //                dr["payment_date"] = column.search.value;
        //        //                break;
        //        //            case "xi_date":
        //        //                dr["xi_date"] = column.search.value;
        //        //                break;
        //        //            case "start_date":
        //        //                dr["start_date"] = column.search.value;
        //        //                break;
        //        //            case "end_date":
        //        //                dr["end_date"] = column.search.value;
        //        //                break;
        //        //            case "begining_par":
        //        //                dr["begining_par"] = column.search.value;
        //        //                break;
        //        //            case "ending_par":
        //        //                dr["ending_par"] = column.search.value;
        //        //                break;
        //        //            case "coupon_rate":
        //        //                dr["coupon_rate"] = column.search.value;
        //        //                break;
        //        //            case "interest":
        //        //                dr["interest"] = column.search.value;
        //        //                break;
        //        //            case "principal":
        //        //                dr["principal"] = column.search.value;
        //        //                break;
        //        //            case "total_payment":
        //        //                dr["total_payment"] = column.search.value;
        //        //                break;
        //        //            case "event_type":
        //        //                dr["event_type"] = column.search.value;
        //        //                break;
        //        //            case "coupon_type":
        //        //                dr["coupon_type"] = column.search.value;
        //        //                break;
        //        //            case "coupon_floating_index_code":
        //        //                dr["coupon_floating_index_code"] = column.search.value;
        //        //                break;
        //        //            case "coupon_spread":
        //        //                dr["coupon_spread"] = column.search.value;
        //        //                break;
        //        //            case "ai_amount":
        //        //                dr["ai_amount"] = column.search.value;
        //        //                break;
        //        //            case "redemption_percent":
        //        //                dr["redemption_percent"] = column.search.value;
        //        //                break;
        //        //            case "complete_flag":
        //        //                dr["complete_flag"] = column.search.value;
        //        //                break;
        //        //        }
        //        //    });
        //        //    dt.Rows.Add(dr);
        //        //    Session["DataEventList"] = dt;
        //        //    for (int i = 0; i < dt.Rows.Count; i++)
        //        //    {
        //        //        SecurityEventModel eventmodels = new SecurityEventModel();
        //        //        eventmodels.round_no = int.Parse(dt.Rows[i]["round_no"].ToString());
        //        //        if (dt.Rows[i]["event_date"].ToString() != "")
        //        //        {
        //        //            eventmodels.event_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["event_date"].ToString());
        //        //        }
        //        //        if (dt.Rows[i]["payment_date"].ToString() != "")
        //        //        {
        //        //            eventmodels.payment_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["payment_date"].ToString());
        //        //        }
        //        //        if (dt.Rows[i]["xi_date"].ToString() != "")
        //        //        {
        //        //            eventmodels.xi_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["xi_date"].ToString());
        //        //        }
        //        //        if (dt.Rows[i]["start_date"].ToString() != "")
        //        //        {
        //        //            eventmodels.start_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["start_date"].ToString());
        //        //        }
        //        //        if (dt.Rows[i]["end_date"].ToString() != "")
        //        //        {
        //        //            eventmodels.end_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["end_date"].ToString());
        //        //        }
        //        //        if (dt.Rows[i]["begining_par"].ToString() != "")
        //        //        {
        //        //            eventmodels.begining_par = int.Parse(dt.Rows[i]["begining_par"].ToString());
        //        //        }
        //        //        if (dt.Rows[i]["ending_par"].ToString() != "")
        //        //        {
        //        //            eventmodels.ending_par = int.Parse(dt.Rows[i]["ending_par"].ToString());
        //        //        }
        //        //        if (dt.Rows[i]["coupon_rate"].ToString() != "")
        //        //        {
        //        //            eventmodels.coupon_rate = int.Parse(dt.Rows[i]["coupon_rate"].ToString());
        //        //        }
        //        //        if (dt.Rows[i]["interest"].ToString() != "")
        //        //        {
        //        //            eventmodels.interest = int.Parse(dt.Rows[i]["interest"].ToString());
        //        //        }
        //        //        if (dt.Rows[i]["principal"].ToString() != "")
        //        //        {
        //        //            eventmodels.principal = int.Parse(dt.Rows[i]["principal"].ToString());
        //        //        }
        //        //        if (dt.Rows[i]["total_payment"].ToString() != "")
        //        //        {
        //        //            eventmodels.total_payment = int.Parse(dt.Rows[i]["total_payment"].ToString());
        //        //        }
        //        //        if (dt.Rows[i]["event_type"].ToString() != "")
        //        //        {
        //        //            eventmodels.event_type = dt.Rows[i]["event_type"].ToString();
        //        //        }
        //        //        if (dt.Rows[i]["coupon_type"].ToString() != "")
        //        //        {
        //        //            eventmodels.coupon_type = dt.Rows[i]["coupon_type"].ToString();
        //        //        }
        //        //        if (dt.Rows[i]["coupon_floating_index_code"].ToString() != "")
        //        //        {
        //        //            eventmodels.coupon_floating_index_code = dt.Rows[i]["coupon_floating_index_code"].ToString();
        //        //        }
        //        //        if (dt.Rows[i]["coupon_spread"].ToString() != "")
        //        //        {
        //        //            eventmodels.coupon_spread = int.Parse(dt.Rows[i]["coupon_spread"].ToString());
        //        //        }
        //        //        if (dt.Rows[i]["ai_amount"].ToString() != "")
        //        //        {
        //        //            eventmodels.ai_amount = int.Parse(dt.Rows[i]["ai_amount"].ToString());
        //        //        }
        //        //        if (dt.Rows[i]["redemption_percent"].ToString() != "")
        //        //        {
        //        //            eventmodels.redemption_percent = int.Parse(dt.Rows[i]["redemption_percent"].ToString());
        //        //        }
        //        //        if (dt.Rows[i]["complete_flag"].ToString() != "")
        //        //        {
        //        //            eventmodels.complete_flag = dt.Rows[i]["complete_flag"].ToString();
        //        //        }

        //        //        result.Data.Events.Add(eventmodels);
        //        //    }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return Json(new
        //    {
        //        draw = model.draw,
        //        recordsTotal = result.HowManyRecord,
        //        recordsFiltered = result.HowManyRecord,
        //        Message = result.Message,
        //        data = result.Data.Events
        //    });
        //}
        
        public class Data
        {
            public int issuer_id { get; set; }
            public int instrumentid { get; set; }

        }

        [HttpPost]
        public ActionResult SearchIssuerRating(Data data)
        {
            ResultWithModel<IssuerRatingResult> result = new ResultWithModel<IssuerRatingResult>();
            IssuerRatingModel issuerratingmodel = new IssuerRatingModel();
            issuerratingmodel.issuer_id = data.issuer_id;

            try
            {
                api_counterparty.Issuer.GetIssuerRatingList(issuerratingmodel, p =>
                {
                    result = p;
                });
            }
            catch (Exception ex)
            {

            }
            return Json(new
            {
                success = true,
                data = result.Data != null ? result.Data.IssuerRatingResultModel : new List<IssuerRatingModel>()
            }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Add(string instrument)
        {
          
            if (!string.IsNullOrEmpty(instrument))
            {
                new RoleScreenAttribute(RoleScreen.VIEW);

                ResultWithModel<SecurityResult> result = new ResultWithModel<SecurityResult>();
                ResultWithModel<SecurityGuarantorResult> resultguarantors = new ResultWithModel<SecurityGuarantorResult>();
                ResultWithModel<SecurityBondRatingResult> resultrating = new ResultWithModel<SecurityBondRatingResult>();
                ResultWithModel<SecurityEventResult> resultevents = new ResultWithModel<SecurityEventResult>();
                ResultWithModel<SecurityAdditionalCodeResult> resultadditionalcode = new ResultWithModel<SecurityAdditionalCodeResult>();

                SecurityModel Security = new SecurityModel();
                SecurityGuarantorModel SecurityGuarantors = new SecurityGuarantorModel();
                SecurityBondRatingModel SecurityBondRating = new SecurityBondRatingModel();
                SecurityEventModel SecurityEvent = new SecurityEventModel();
                SecurityAdditionalCodeModel SecurityAdditionalCode = new SecurityAdditionalCodeModel();

                try
                {
                    //Add Paging
                    PagingModel paging = new PagingModel();
                    Security.paging = paging;
                    //Add Orderby
                    var orders = new List<OrderByModel>();
                    Security.ordersby = orders;
                    //Add counterparty id
                    Security.instrument_id = int.Parse(instrument);
                    SecurityGuarantors.instrument_id = int.Parse(instrument);
                    SecurityBondRating.instrument_id = int.Parse(instrument);
                    SecurityEvent.instrument_id = int.Parse(instrument);
                    SecurityAdditionalCode.instrument_id = int.Parse(instrument);

                    api_security.Security.GetSecurityList(Security, p =>
                    {
                        result = p;
                    });
                    var xidate = result.Data.SecurityResultModel[0].xi_day;
                    var xadate = result.Data.SecurityResultModel[0].xa_day;
                    if (xidate.Length > 0)
                    {
                        result.Data.SecurityResultModel[0].xi_day = xidate.Substring(0, xidate.Length - 1);
                        result.Data.SecurityResultModel[0].xi_day_unit_value = xidate.Substring(xidate.Length - 1);

                        if (result.Data.SecurityResultModel[0].xi_day_unit_value == "D")
                        {
                            result.Data.SecurityResultModel[0].xi_day_unit_text = "Day";
                        }
                        if (result.Data.SecurityResultModel[0].xi_day_unit_value == "M")
                        {
                            result.Data.SecurityResultModel[0].xi_day_unit_text = "Month";
                        }

                    }
                    if(xadate.Length > 0)
                    {
                        result.Data.SecurityResultModel[0].xa_day = xadate.Substring(0, xadate.Length - 1);
                        result.Data.SecurityResultModel[0].xa_day_unit_value = xadate.Substring(xadate.Length - 1);

                        if (result.Data.SecurityResultModel[0].xa_day_unit_value == "D")
                        {
                            result.Data.SecurityResultModel[0].xa_day_unit_text = "Day";
                        }
                        if (result.Data.SecurityResultModel[0].xa_day_unit_value == "M")
                        {
                            result.Data.SecurityResultModel[0].xa_day_unit_text = "Month";
                        }

                    }

                    api_security.Security.GetSecurityGuarantorList(SecurityGuarantors, p =>
                    {
                        resultguarantors = p;
                    });

                    api_security.Security.GetSecurityRatingList(SecurityBondRating, p =>
                    {
                        resultrating = p;
                    });

                    //api_security.Security.GetSecurityEventsList(SecurityEvent, p =>
                    //{
                    //    resultevents = p;
                    //});

                    api_security.Security.GetSecurityAdditionalCodeList(SecurityAdditionalCode, p =>
                    {
                        resultadditionalcode = p;
                    });

                    Security = result.Data.SecurityResultModel[0];

                    if (resultguarantors.Data == null)
                    {
                        resultguarantors.Data = new SecurityGuarantorResult();
                    }
                    if (resultrating.Data == null)
                    {
                        resultrating.Data = new SecurityBondRatingResult();
                    }
                    if (resultevents.Data == null)
                    {
                        resultevents.Data = new SecurityEventResult();
                    }
                    if (resultadditionalcode.Data == null)
                    {
                        resultadditionalcode.Data = new SecurityAdditionalCodeResult();
                    }

                    Security.Guarantors = resultguarantors.Data.SecurityGuarantorsResultModel;
                    Security.GuarantorsRightModal = new SecurityGuarantorModel();

                    Security.BondRating = resultrating.Data.SecurityRatingResultModel;
                    Security.BondRatingRightModal = new SecurityBondRatingModel();

                    Security.Events = resultevents.Data.SecurityEventsResultModel;
                    Security.SecurityEventRightModal = new SecurityEventModel();

                    Security.AdditionalCodes = resultadditionalcode.Data.SecurityAdditionalCodeResultModel;
                    Security.AdditionalCodeRightModal = new SecurityAdditionalCodeModel();

                    Security.page_name = "editpage";
                                   
                    if (Security.active_flag == true && Security.verify_flag_bo == false)
                    {
                        Security.statusdata = "UnApprove";
                    }
                    else if (Security.active_flag == false)
                    {
                        Security.statusdata = "UnActive";
                    }
                    else if (Security.active_flag == true && Security.verify_flag_bo == true)
                    {
                        Security.statusdata = "Approve";
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                if (Session["DataEventList"] != null)
                {
                    Session["DataEventList"] = null;
                }
                return View(Security);
            }
            else
            {
                new RoleScreenAttribute(RoleScreen.CREATE);

                SecurityModel securitymodel = new SecurityModel();
                securitymodel.Guarantors = new List<SecurityGuarantorModel>();
                SecurityGuarantorModel guarantors = new SecurityGuarantorModel();
                securitymodel.Guarantors.Add(guarantors);
                securitymodel.GuarantorsRightModal = new SecurityGuarantorModel();

                securitymodel.BondRating = new List<SecurityBondRatingModel>();
                SecurityBondRatingModel bondrating = new SecurityBondRatingModel();
                securitymodel.BondRating.Add(bondrating);
                securitymodel.BondRatingRightModal = new SecurityBondRatingModel();

                securitymodel.Events = new List<SecurityEventModel>();
                SecurityEventModel events = new SecurityEventModel();
                securitymodel.Events.Add(events);
                securitymodel.SecurityEventRightModal = new SecurityEventModel();

                securitymodel.AdditionalCodes = new List<SecurityAdditionalCodeModel>();
                SecurityAdditionalCodeModel additionalcode = new SecurityAdditionalCodeModel();
                securitymodel.AdditionalCodes.Add(additionalcode);
                securitymodel.AdditionalCodeRightModal = new SecurityAdditionalCodeModel();

                securitymodel.statusdata = "New";
                securitymodel.page_name = "addpage";

                securitymodel.xa_day_unit_value = "D";
                securitymodel.xa_day_unit_text = "Day";

                securitymodel.xi_day_unit_value = "D";
                securitymodel.xi_day_unit_text = "Day";

                if (Session["DataEventList"] != null)
                {
                    Session["DataEventList"] = null;
                }
                return View(securitymodel);
            }
        }

        [HttpPost]
        public ActionResult Add(SecurityModel SecurityModel)
        {
            if (SecurityModel.page_name == "addpage")
            {
                new RoleScreenAttribute(RoleScreen.CREATE);

                ResultWithModel<SecurityResult> res = new ResultWithModel<SecurityResult>();
                SecurityModel.create_by = HttpContext.User.Identity.Name;

                SecurityModel.xa_day = SecurityModel.xa_day + SecurityModel.xa_day_unit_value;
                SecurityModel.xi_day = SecurityModel.xi_day + SecurityModel.xi_day_unit_value;

                SecurityModel = SetStatusFlag(SecurityModel);

                if (Session["DataEventList"] != null)
                {
                    SecurityModel.Events = (List<SecurityEventModel>)Session["DataEventList"];
                }

                if (ModelState.IsValid)
                {
                    //api_security.Security.Create
                    api_security.Security.CreateSecurity(SecurityModel, p =>
                    {
                        res = p;
                    });

                    if (res.Success)
                    {
                        return View("Index");
                    }
                    else
                    {
                        if (res.Message.Contains("Duplicate Instrument"))
                        { 
                            ModelState.AddModelError("instrument_code", "Duplicate Instrument Code");
                        }
                        else if (res.Message.Contains("Duplicate ISIN"))
                        {
                            ModelState.AddModelError("isin_code", "Duplicate ISIN Code");
                        }
                        else
                        {
                            ModelState.AddModelError("Exception", res.Message);
                            ViewBag.Message = res.Message;
                        }
                    }
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

                    //ViewBag.Message = res.Message;
                }
                if (SecurityModel.Guarantors == null)
                {
                    SecurityModel.Guarantors = new List<SecurityGuarantorModel>();
                    SecurityGuarantorModel guarantors = new SecurityGuarantorModel();
                    SecurityModel.Guarantors.Add(guarantors);
                    SecurityModel.GuarantorsRightModal = new SecurityGuarantorModel();
                }
                if (SecurityModel.BondRating == null)
                {
                    SecurityModel.BondRating = new List<SecurityBondRatingModel>();
                    SecurityBondRatingModel bondrating = new SecurityBondRatingModel();
                    SecurityModel.BondRating.Add(bondrating);
                    SecurityModel.BondRatingRightModal = new SecurityBondRatingModel();
                }
                if (SecurityModel.Events == null)
                {
                    SecurityModel.Events = new List<SecurityEventModel>();
                    SecurityEventModel events = new SecurityEventModel();
                    SecurityModel.Events.Add(events);
                    SecurityModel.SecurityEventRightModal = new SecurityEventModel();
                }
            }
            else
            {
                new RoleScreenAttribute(RoleScreen.EDIT);

                ResultWithModel<SecurityResult> res = new ResultWithModel<SecurityResult>();
                SecurityModel.create_by = HttpContext.User.Identity.Name;

                SecurityModel.xa_day = SecurityModel.xa_day + SecurityModel.xa_day_unit_value;
                SecurityModel.xi_day = SecurityModel.xi_day + SecurityModel.xi_day_unit_value;
                SecurityModel = SetStatusFlag(SecurityModel);

                if (Session["DataEventList"] != null)
                {
                    SecurityModel.Events = (List<SecurityEventModel>)Session["DataEventList"];
                }

                if (ModelState.IsValid)
                {
                    api_security.Security.UpdateSecurity(SecurityModel, p => {
                        res = p;
                    });

                    if (res.Success)
                    {
                        return RedirectToAction("Index");
                    }

                    else
                    {
                        if (res.Message.Contains("Duplicate Instrument"))
                        {
                            ModelState.AddModelError("instrument_code", "Duplicate Instrument Code");
                        }
                        else if (res.Message.Contains("Duplicate ISIN"))
                        {
                            ModelState.AddModelError("isin_code", "Duplicate ISIN Code");
                        }
                        else
                        {
                            ModelState.AddModelError("Exception", res.Message);
                            ViewBag.Message = res.Message;
                        }
                    }
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
                    //ViewBag.Message = res.Message;
                }

                if (SecurityModel.Guarantors == null)
                {
                    SecurityModel.Guarantors = new List<SecurityGuarantorModel>();
                }
                if (SecurityModel.GuarantorsRightModal == null)
                {
                    SecurityModel.GuarantorsRightModal = new SecurityGuarantorModel();
                }

                if (SecurityModel.BondRating == null)
                {
                    SecurityModel.BondRating = new List<SecurityBondRatingModel>();
                }
                if (SecurityModel.BondRatingRightModal == null)
                {
                    SecurityModel.BondRatingRightModal = new SecurityBondRatingModel();
                }

                if (SecurityModel.Events == null)
                {
                    SecurityModel.Events = new List<SecurityEventModel>();
                }
                if (SecurityModel.SecurityEventRightModal == null)
                {
                    SecurityModel.SecurityEventRightModal = new SecurityEventModel();
                }
            }
          
            return View(SecurityModel);
        }

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Edit(string instrumentcode)
        {
            return RedirectToAction("Add", new { instrument = instrumentcode } );
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult Deletes(Data data)
        {
            var rwm = new ResultWithModel<SecurityResult>();
            SecurityModel view = new SecurityModel();
            view.create_by = HttpContext.User.Identity.Name;
            view.instrument_id = data.instrumentid;
            try
            {
                api_security.Security.DeleteSecurity(view, p =>
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
                // return View();
            }
            // return Json(rwm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public JsonResult RequestEquity(InterfaceReqEquitySymbolModel model)
        {
            ResultWithModel<InterfaceReqEquitySymbolResult> result = new ResultWithModel<InterfaceReqEquitySymbolResult>();
            try
            {
                if (ModelState.IsValid)
                {
                    ResultWithModel<RpConfigResult> resultRpconfig = new ResultWithModel<RpConfigResult>();
                    List<RpConfigModel> rpConfigModelList = new List<RpConfigModel>();
                    DateTime date = DateTime.Now;

                    api_static.RpConfig.GetRpConfig("RP_EQUITY_INTERFACE_SYMBOL", string.Empty, p =>
                    {
                        resultRpconfig = p;
                    });

                    rpConfigModelList = resultRpconfig.Data.RpConfigResultModel;
                    model.ref_no = Guid.NewGuid().ToString().ToUpper();
                    model.channel = rpConfigModelList.FirstOrDefault(a => a.item_code == "CHANNEL")?.item_value;
                    model.type = rpConfigModelList.FirstOrDefault(a => a.item_code == "TYPE")?.item_value;
                    model.request_date = date.ToString("yyyyMMdd");
                    model.request_time = date.ToString("HH:MM:ss");
                    model.url_service = rpConfigModelList.FirstOrDefault(a => a.item_code == "SERVICE_URL")?.item_value;
                    model.create_by = User.UserId;

                    api_externalInterface.InterfaceEquitySymbol.InterfaceEquitySymbol(model, p =>
                    {
                        result = p;
                    });

                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.RefCode = -999;
                result.Message = ex.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public SecurityModel SetStatusFlag(SecurityModel model)
        {
            SecurityModel Security = model;
            if (Security.statusdata == "Approve")
            {
                Security.active_flag = true;
                Security.verify_flag_bo = true;
                Security.verify_flag = true;
            }
            else if (Security.statusdata == "UnApprove" || Security.statusdata == "New")
            {
                Security.active_flag = true;
                Security.verify_flag_bo = false;
                Security.verify_flag = false;
            }
            else if (Security.statusdata == "UnActive")
            {
                Security.active_flag = false;
                Security.verify_flag_bo = false;
                Security.verify_flag = false;
            }
            return Security;
        }

        public ActionResult FillProduct(string instrumenttype, string productname)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLProduct(instrumenttype, productname, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillSubProduct(string productcode, string subproduct)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLSubProduct(productcode, subproduct, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillBondType(string instrumenttypename, string productcode)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLBondType(instrumenttypename, productcode, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillBondSubType(string bondsubtypedesc, string productcode,string instrumenttypename)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLBondSubType(bondsubtypedesc, productcode, instrumenttypename, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillIssuer(string issuername)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLIssuer(issuername, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillRegister(string registername)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLRegister(registername, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillOptionType(string optionname)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLOptionType(optionname, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillSeniorityType(string senioritytypename)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLSeniorityType(senioritytypename, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillMarketCode(string marketname)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLMarketCode(marketname, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCur(string curtext)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLCur(curtext, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillHolidayCur(string curtext)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLHolidayCur(curtext, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult FillYearBasis(string yearbasistext)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLYearBasis(yearbasistext, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillTBMAListed(string tbmalisttext)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLTBMAListed(tbmalisttext, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillGuarantor(string guarantorname)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLGuarantorCode(guarantorname, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillAgency(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_agencyandrating.Agency.GetDDLAgencyCode(datastr, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillLocalRating(string agencycode, string shortlongterm, string datatext)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_agencyandrating.Rating.GetDDLLocalRating(agencycode, shortlongterm, datatext, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillForeignRating(string agencycode, string shortlongterm, string datatext)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_agencyandrating.Rating.GetDDLForeignRating(agencycode, shortlongterm, datatext, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillInstrument(string text)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLInstrumentCode(text, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillISINCode(string text)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLISINCode(text, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCouponType(string text)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLCouponType(text, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCouponFreq(string text)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLCouponFreq(text, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCompoundType(string text)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLCouponFreq(text, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DDLDayMonthText()
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            DDLItemModel item = new DDLItemModel();
            res.Add(new DDLItemModel()
            {
                Text = "Day",
                Value = "D"
            });
            res.Add(new DDLItemModel()
            {
                Text = "Month",
                Value = "M"
            });            
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DDLOwner()
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            DDLItemModel item = new DDLItemModel();
            res.Add(new DDLItemModel()
            {
                Text = "REPO",
                Value = "REPO"
            });
            res.Add(new DDLItemModel()
            {
                Text = "FIX",
                Value = "FIX"
            });
            res.Add(new DDLItemModel()
            {
                Text = "BOTH",
                Value = "BOTH"
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

    }
}