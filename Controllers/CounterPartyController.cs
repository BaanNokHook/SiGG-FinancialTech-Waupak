using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.CounterParty;
using GM.Data.Model.Static;
using GM.Data.Result.CounterParty;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]  
    [Audit]  
    public class CounterPartyController : BaseController  
    {
        CounterPartyEntities api_counterparty = new CounterPartyEntities();   
        StaticEntities api_static = new StaticEntities();   
        AgencyAndRatingEntities api_agencyandrating = new AgencyAndRatingEntities();   
        ExchangeRateEntities api_exchange = new ExchangeRateEntities();    

        // GET: CounterParty 
        [RoleScreen(RoleScreen.VIEW)]   
        public ActionResult Index()  
        {
            return View();
        }  

        [HttpPost]  
        public ActionResult Search(DataTableAjaxPostModel model)  
        {
            ResultWithModel<CounterPartyResult> result = new ResultWithModel<CounterPartyResult>();   
            CounterPartyModel counterpartymodel = new CounterPartyModel();   
            try  
            {
                  // string searchvalue = Request["search[value]"];   

                  //Add Paging  
                  PagingModel paging = new PagingModel();   
                  paging.PageNumber = model.pageno;   
                  paging.RecordPerPage = model.length;  
                  counterpartymodel.paging = paging;   
                  counterpartymodel.create_by = HttpContext.User.Identity.Name;  
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

                  counterpartymodel.ordersby = orders;   

                  var columns = model.columns.Where(o => o.search.value != null).ToList();   
                  columns.ForEach(column =>  
                  {
                        switch (column,data)  
                        {
                              case "counter_party_code":  
                                    counterpartymodel.counter_party_code = column.search.value;  
                                    break;  
                              case "counter_party_shortname":  
                                    counterpartymodel.counter_party_shortname = column.search.value;  
                                    break;   
                              case "counter_party_name":  
                                    counterpartymodel.counter_party_name = column.search.value;   
                                    break;  
                              case "counter_party_thainame":
                              counterpartymodel.counter_party_thainame = column.search.value;
                              break;
                              case "counter_party_type_code":
                              counterpartymodel.counter_party_type_code = column.search.value;
                              break;
                              case "tel_no":
                              counterpartymodel.tel_no = column.search.value;
                              break;
                        }
                  });

                api_counterparty.CounterParty.GetCounterPartyList(counterpartymodel, p =>
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
                data = result.Data != null ? result.Data.CounterPartyResultModel : new List<CounterPartyModel>()
            });
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Add(CounterPartyModel CounterPartyModel)
        {
            ResultWithModel<CounterPartyResult> res = new ResultWithModel<CounterPartyResult>();
            CounterPartyModel.create_by = HttpContext.User.Identity.Name;
            CounterPartyModel = SetStatusFlag(CounterPartyModel);

            if (CounterPartyModel.country_id != 65)
            {
                if (ModelState.ContainsKey("province_id"))
                {
                    ModelState["province_id"].Errors.Clear();
                }

                if (ModelState.ContainsKey("district_id"))
                {
                    ModelState["district_id"].Errors.Clear();
                }

                if (ModelState.ContainsKey("sub_district_id"))
                {
                    ModelState["sub_district_id"].Errors.Clear();
                }

                if (ModelState.ContainsKey("zipcode"))
                {
                    ModelState["zipcode"].Errors.Clear();
                }
            }

            if (CounterPartyModel.margin_type_id == null)
            {
                CounterPartyModel.Margin = null;
            }
            else
            {
                CounterPartyModel.Margin = new CounterPartyMarginModel();
                CounterPartyModel.Margin.borrow_only_flag = CounterPartyModel.borrow_only_flag;
                CounterPartyModel.Margin.counter_party_id = CounterPartyModel.counter_party_id;
                CounterPartyModel.Margin.except_margin_flag = CounterPartyModel.except_margin_flag;
                CounterPartyModel.Margin.margin_in_term_id = CounterPartyModel.margin_in_term_id;
                CounterPartyModel.Margin.margin_in_type_id = CounterPartyModel.margin_in_type_id;
                CounterPartyModel.Margin.margin_type_id = CounterPartyModel.margin_type_id;
                CounterPartyModel.Margin.threshold = CounterPartyModel.threshold;
                CounterPartyModel.Margin.minimum_transfer = CounterPartyModel.minimum_transfer;
                CounterPartyModel.Margin.margin_method = CounterPartyModel.margin_method;
            }

            if (ModelState.IsValid)
            {
                api_counterparty.CounterParty.CreateCounterParty(CounterPartyModel, p =>
                {
                    res = p;
                });

                if (res.Success)
                {
                    return RedirectToAction("Index");
                }

                else
                {
                    ViewBag.Message = res.Message;
                    ModelState.AddModelError("Exception", res.Message);
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
            }

            if (CounterPartyModel.Margin == null)
            {
                CounterPartyModel.Margin = new CounterPartyMarginModel();
            }

            if (CounterPartyModel.Payment == null)
            {
                CounterPartyModel.Payment = new List<CounterPartyPaymentModel>();
            }
            if (CounterPartyModel.PaymentRightModal == null)
            {
                CounterPartyModel.PaymentRightModal = new CounterPartyPaymentModel();
            }

            if (CounterPartyModel.Identify == null)
            {
                CounterPartyModel.Identify = new List<CounterPartyIdentifyModel>();
            }
            if (CounterPartyModel.IdentifyRightModal == null)
            {
                CounterPartyModel.IdentifyRightModal = new CounterPartyIdentifyModel();
            }

            if (CounterPartyModel.Rating == null)
            {
                CounterPartyModel.Rating = new List<CounterPartyRatingModel>();
            }
            if (CounterPartyModel.RatingRightModal == null)
            {
                CounterPartyModel.RatingRightModal = new CounterPartyRatingModel();
            }

            if (CounterPartyModel.Haircut == null)
            {
                CounterPartyModel.Haircut = new List<CounterPartyHaircutModel>();
            }
            if (CounterPartyModel.HaircutRightModel == null)
            {
                CounterPartyModel.HaircutRightModel = new CounterPartyHaircutModel();
            }

            if (CounterPartyModel.Exchange == null)
            {
                CounterPartyModel.Exchange = new List<CounterPartyExchRateModel>();
            }
            if (CounterPartyModel.ExchangeRightModel == null)
            {
                CounterPartyModel.ExchangeRightModel = new CounterPartyExchRateModel();
            }

            return View(CounterPartyModel);
        }

        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Add()
        {
            //Static DDL
            //ViewBag.Months = new SelectList(api_userandscreen.GetDDLMonths(), "Value", "Text");
            // ViewBag.Days = new SelectList(api_userandscreen.GetDDLDayOfMonth(DateTime.Now.Month), "Value", "Text");
            // ViewBag.ExcludeUnits = new SelectList(api_userandscreen.GetDDLExcludeUnit(), "Value", "Text");

            CounterPartyModel CounterParty = new CounterPartyModel();

            CounterParty.Payment = new List<CounterPartyPaymentModel>();
            CounterPartyPaymentModel payment = new CounterPartyPaymentModel();
            CounterParty.Payment.Add(payment);
            CounterParty.PaymentRightModal = new CounterPartyPaymentModel();

            CounterParty.Identify = new List<CounterPartyIdentifyModel>();
            CounterPartyIdentifyModel indentify = new CounterPartyIdentifyModel();
            CounterParty.Identify.Add(indentify);
            CounterParty.IdentifyRightModal = new CounterPartyIdentifyModel();

            CounterParty.Rating = new List<CounterPartyRatingModel>();
            CounterPartyRatingModel rating = new CounterPartyRatingModel();
            CounterParty.Rating.Add(rating);
            CounterParty.RatingRightModal = new CounterPartyRatingModel();

            CounterParty.Haircut = new List<CounterPartyHaircutModel>();
            CounterPartyHaircutModel haircut = new CounterPartyHaircutModel();
            CounterParty.Haircut.Add(haircut);
            CounterParty.HaircutRightModel = new CounterPartyHaircutModel();

            CounterParty.Exchange = new List<CounterPartyExchRateModel>();
            CounterPartyExchRateModel exchange = new CounterPartyExchRateModel();
            CounterParty.Exchange.Add(exchange);
            CounterParty.ExchangeRightModel = new CounterPartyExchRateModel();

            CounterParty.Margin = new CounterPartyMarginModel();

            CounterParty.statusdata = "UnApprove";

            //CounterParty.country_id = 65;
            //CounterParty.country_name = "ไทย";
            return View(CounterParty);
            //return View();
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Edit(CounterPartyModel CounterPartyModel)
        {
            ResultWithModel<CounterPartyResult> res = new ResultWithModel<CounterPartyResult>();
            CounterPartyModel.create_by = HttpContext.User.Identity.Name;
            CounterPartyModel = SetStatusFlag(CounterPartyModel);

            if (CounterPartyModel.country_id != 65)
            {
                if (ModelState.ContainsKey("province_id"))
                {
                    ModelState["province_id"].Errors.Clear();
                }

                if (ModelState.ContainsKey("district_id"))
                {
                    ModelState["district_id"].Errors.Clear();
                }

                if (ModelState.ContainsKey("sub_district_id"))
                {
                    ModelState["sub_district_id"].Errors.Clear();
                }

                if (ModelState.ContainsKey("zipcode"))
                {
                    ModelState["zipcode"].Errors.Clear();
                }
            }

            if (CounterPartyModel.margin_type_id == null)
            {
                CounterPartyModel.Margin = null;
            }
            else
            {
                CounterPartyModel.Margin = new CounterPartyMarginModel();
                CounterPartyModel.Margin.borrow_only_flag = CounterPartyModel.borrow_only_flag;
                CounterPartyModel.Margin.counter_party_id = CounterPartyModel.counter_party_id;
                CounterPartyModel.Margin.except_margin_flag = CounterPartyModel.except_margin_flag;
                CounterPartyModel.Margin.margin_in_term_id = CounterPartyModel.margin_in_term_id;
                CounterPartyModel.Margin.margin_in_type_id = CounterPartyModel.margin_in_type_id;
                CounterPartyModel.Margin.margin_type_id = CounterPartyModel.margin_type_id;
                CounterPartyModel.Margin.threshold = CounterPartyModel.threshold;
                CounterPartyModel.Margin.minimum_transfer = CounterPartyModel.minimum_transfer;
                CounterPartyModel.Margin.margin_method = CounterPartyModel.margin_method;
            }

            if (CounterPartyModel.Payment != null)
            {
                foreach (var banklist in CounterPartyModel.Payment)
                {
                    banklist.counter_party_id = CounterPartyModel.counter_party_id;
                }
            }
            else
            {
                CounterPartyModel.Payment = null;
            }
            if (CounterPartyModel.Identify != null)
            {
                foreach (var identify in CounterPartyModel.Identify)
                {
                    identify.counter_party_id = CounterPartyModel.counter_party_id;
                }
            }
            else
            {
                CounterPartyModel.Identify = null;
            }
            if (CounterPartyModel.Rating != null)
            {
                foreach (var rating in CounterPartyModel.Rating)
                {
                    rating.counter_party_id = CounterPartyModel.counter_party_id;
                }
            }
            else
            {
                CounterPartyModel.Rating = null;
            }

            if (CounterPartyModel.Haircut != null)
            {
                foreach (var haircut in CounterPartyModel.Haircut)
                {
                    haircut.counter_party_id = CounterPartyModel.counter_party_id;
                }
            }
            else
            {
                CounterPartyModel.Haircut = null;
            }

            if (CounterPartyModel.Exchange != null)
            {
                foreach (var exchange in CounterPartyModel.Exchange)
                {
                    exchange.counter_party_id = CounterPartyModel.counter_party_id;
                }
            }
            else
            {
                CounterPartyModel.Exchange = null;
            }

            if (ModelState.IsValid)
            {
                api_counterparty.CounterParty.UpdateCounterParty(CounterPartyModel, p =>
                {
                    res = p;
                });

                if (res.Success)
                {
                    return RedirectToAction("Index");
                }

                else
                {
                    ViewBag.Message = res.Message;
                    ModelState.AddModelError("Exception", res.Message);
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

                ViewBag.Message = res.Message;
            }

            if (CounterPartyModel.Payment == null)
            {
                CounterPartyModel.Payment = new List<CounterPartyPaymentModel>();
            }
            if (CounterPartyModel.PaymentRightModal == null)
            {
                CounterPartyModel.PaymentRightModal = new CounterPartyPaymentModel();
            }

            if (CounterPartyModel.Identify == null)
            {
                CounterPartyModel.Identify = new List<CounterPartyIdentifyModel>();
            }
            if (CounterPartyModel.IdentifyRightModal == null)
            {
                CounterPartyModel.IdentifyRightModal = new CounterPartyIdentifyModel();
            }

            if (CounterPartyModel.Rating == null)
            {
                CounterPartyModel.Rating = new List<CounterPartyRatingModel>();
            }
            if (CounterPartyModel.RatingRightModal == null)
            {
                CounterPartyModel.RatingRightModal = new CounterPartyRatingModel();
            }

            if (CounterPartyModel.Haircut == null)
            {
                CounterPartyModel.Haircut = new List<CounterPartyHaircutModel>();
            }
            if (CounterPartyModel.HaircutRightModel == null)
            {
                CounterPartyModel.HaircutRightModel = new CounterPartyHaircutModel();
            }

            if (CounterPartyModel.Exchange == null)
            {
                CounterPartyModel.Exchange = new List<CounterPartyExchRateModel>();
            }
            if (CounterPartyModel.ExchangeRightModel == null)
            {
                CounterPartyModel.ExchangeRightModel = new CounterPartyExchRateModel();
            }

            return View(CounterPartyModel);
        }

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Edit(int id)
        {
            if (id > 0)
            {
                ResultWithModel<CounterPartyResult> result = new ResultWithModel<CounterPartyResult>();
                ResultWithModel<CounterPartyPaymentResult> resultpayment = new ResultWithModel<CounterPartyPaymentResult>();
                ResultWithModel<CounterPartyIdentifyResult> resultidentify = new ResultWithModel<CounterPartyIdentifyResult>();
                ResultWithModel<CounterPartyRatingResult> resultrating = new ResultWithModel<CounterPartyRatingResult>();
                ResultWithModel<CounterPartyHaircutResult> resultHaircut = new ResultWithModel<CounterPartyHaircutResult>();
                ResultWithModel<CounterPartyExchangeResult> resultExchange = new ResultWithModel<CounterPartyExchangeResult>();
                ResultWithModel<CounterPartyMarginResult> resultmargin = new ResultWithModel<CounterPartyMarginResult>();
                //CounterPartyModel counterpartymodel = new CounterPartyModel();
                CounterPartyModel CounterParty = new CounterPartyModel();
                CounterPartyPaymentModel CounterPartyPayment = new CounterPartyPaymentModel();
                CounterPartyIdentifyModel CounterPartyIdentify = new CounterPartyIdentifyModel();
                CounterPartyRatingModel CounterPartyRating = new CounterPartyRatingModel();
                CounterPartyHaircutModel CounterPartyHaircut = new CounterPartyHaircutModel();
                CounterPartyExchRateModel CounterPartyExchange = new CounterPartyExchRateModel();
                CounterPartyMarginModel CounterPartyMargin = new CounterPartyMarginModel();
                try
                {
                    //Add Paging
                    PagingModel paging = new PagingModel();
                    CounterParty.paging = paging;
                    //Add Orderby
                    var orders = new List<OrderByModel>();
                    CounterParty.ordersby = orders;
                    //Add counterparty id
                    CounterParty.counter_party_id = id;
                    CounterPartyPayment.counter_party_id = id;
                    CounterPartyIdentify.counter_party_id = id;
                    CounterPartyRating.counter_party_id = id;
                    CounterPartyMargin.counter_party_id = id;
                    CounterPartyHaircut.counter_party_id = id;
                    CounterPartyExchange.counter_party_id = id;

                    api_counterparty.CounterParty.GetCounterPartyList(CounterParty, p =>
                    {
                        result = p;
                    });

                    api_counterparty.CounterParty.GetCounterPartyMarginList(CounterPartyMargin, p =>
                    {
                        resultmargin = p;
                    });

                    api_counterparty.CounterParty.GetCounterPartyPayment(CounterPartyPayment, p =>
                    {
                        resultpayment = p;
                    });

                    api_counterparty.CounterParty.GetCounterPartyIdentifyList(CounterPartyIdentify, p =>
                    {
                        resultidentify = p;
                    });

                    api_counterparty.CounterParty.GetCounterPartyRatingList(CounterPartyRating, p =>
                    {
                        resultrating = p;
                    });

                    api_counterparty.CounterParty.GetCounterPartyHaircutList(CounterPartyHaircut, p =>
                    {
                        resultHaircut = p;
                    });

                    api_counterparty.CounterParty.GetCounterPartyExchangeList(CounterPartyExchange, p =>
                    {
                        resultExchange = p;
                    });

                    CounterParty = result.Data.CounterPartyResultModel[0];

                    if (resultmargin.Data == null)
                    {
                        resultmargin.Data = new CounterPartyMarginResult();
                    }
                    if (resultpayment.Data == null)
                    {
                        resultpayment.Data = new CounterPartyPaymentResult();
                    }
                    if (resultidentify.Data == null)
                    {
                        resultidentify.Data = new CounterPartyIdentifyResult();
                    }
                    if (resultrating.Data == null)
                    {
                        resultrating.Data = new CounterPartyRatingResult();
                    }
                    if (resultHaircut.Data == null)
                    {
                        resultHaircut.Data = new CounterPartyHaircutResult();
                    }
                    if (resultExchange.Data == null)
                    {
                        resultExchange.Data = new CounterPartyExchangeResult();
                    }

                    if (resultmargin.Data.CounterPartyMarginResultModel.Count > 0)
                    {
                        CounterParty.Margin = resultmargin.Data.CounterPartyMarginResultModel[0];
                        CounterParty.borrow_only_flag = CounterParty.Margin.borrow_only_flag;
                        CounterParty.counter_party_id = CounterParty.Margin.counter_party_id;
                        CounterParty.except_margin_flag = CounterParty.Margin.except_margin_flag;
                        CounterParty.margin_in_term_id = CounterParty.Margin.margin_in_term_id;
                        CounterParty.margin_in_type_id = CounterParty.Margin.margin_in_type_id;
                        CounterParty.margin_type_id = CounterParty.Margin.margin_type_id;
                        CounterParty.threshold = CounterParty.Margin.threshold;
                        CounterParty.margin_in_term_name = CounterParty.Margin.margin_in_term_name;
                        CounterParty.margin_in_type_name = CounterParty.Margin.margin_in_type_name;
                        CounterParty.margin_type_name = CounterParty.Margin.margin_type_name;
                        CounterParty.minimum_transfer = CounterParty.Margin.minimum_transfer;
                        CounterParty.margin_method = CounterParty.Margin.margin_method;
                    }
                    else
                    {
                        CounterParty.Margin = new CounterPartyMarginModel();
                    }

                    CounterParty.Payment = resultpayment.Data.CounterPartyPaymentResultModel;
                    CounterParty.PaymentRightModal = new CounterPartyPaymentModel();

                    CounterParty.Identify = resultidentify.Data.CounterPartyIdentifyResultModel;
                    CounterParty.IdentifyRightModal = new CounterPartyIdentifyModel();

                    CounterParty.Rating = resultrating.Data.CounterPartyRatingResultModel;
                    CounterParty.RatingRightModal = new CounterPartyRatingModel();

                    CounterParty.Haircut = resultHaircut.Data.CounterPartyHaircutResultModel;
                    CounterParty.HaircutRightModel = new CounterPartyHaircutModel();

                    CounterParty.Exchange = resultExchange.Data.CounterPartyExchangeResultModel;
                    CounterParty.ExchangeRightModel = new CounterPartyExchRateModel();

                    if (CounterParty.active_flag == true && CounterParty.verify_flag_bo == false)
                    {
                        CounterParty.statusdata = "UnApprove";
                    }
                    else if (CounterParty.active_flag == false)
                    {
                        CounterParty.statusdata = "UnActive";
                    }
                    if (CounterParty.active_flag == true && CounterParty.verify_flag_bo == true)
                    {
                        CounterParty.statusdata = "Approve";
                    }
                }
                catch
                {

                }
                return View(CounterParty);
            }
            else
            {
                return View("Index");
            }
        }

        public class Data
        {
            public int counterpartyid { get; set; }
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult Deletes(Data data)
        {
            var rwm = new ResultWithModel<CounterPartyResult>();
            CounterPartyModel view = new CounterPartyModel();
            view.create_by = HttpContext.User.Identity.Name;
            view.counter_party_id = data.counterpartyid;
            try
            {
                api_counterparty.CounterParty.DeleteCounterParty(view, p =>
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

        public CounterPartyModel SetStatusFlag(CounterPartyModel model)
        {
            CounterPartyModel CounterParty = model;
            if (CounterParty.statusdata == "Approve")
            {
                CounterParty.active_flag = true;
                CounterParty.verify_flag_bo = true;
                CounterParty.verify_flag_fo = true;
            }
            else if (CounterParty.statusdata == "UnApprove" || CounterParty.statusdata == "New")
            {
                CounterParty.active_flag = true;
                CounterParty.verify_flag_bo = false;
                CounterParty.verify_flag_fo = false;
            }
            else if (CounterParty.statusdata == "UnActive")
            {
                CounterParty.active_flag = false;
                CounterParty.verify_flag_bo = false;
                CounterParty.verify_flag_fo = false;
            }
            return CounterParty;
        }

        //for ddl Start
        public ActionResult FillTitleName(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.Title.GetDDLTitleName(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCounterPartyTypeCode(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_counterparty.CounterParty.GetDDLCounterPartyTypeCode(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCounterPartyGroup(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_counterparty.CounterParty.GetDDLCounterPartyGroup(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillProvince(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.Province.GetDDLProvince(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillDistrict(string dataint, string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.District.GetDDLDistrict(dataint, datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillSubDistrict(string dataint, string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.SubDistrict.GetDDLSubDistrict(dataint, datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCountry(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.Country.GetDDLCountry(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillKTB_ISIC(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.KtbIsic.GetDDLKTB_ISIC(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCustodian_Id(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.Custodian.GetDDLCustodian(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillMarginType(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.Margin.GetDDLCounterPartyMarginType(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillMarginInType(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.Margin.GetDDLCounterPartyMarginInType(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillMarginInTerm(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.Margin.GetDDLCounterPartyMarginInTerm(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FillAccountType(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_counterparty.CounterParty.GetDDLAccountType(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FillBankName(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_counterparty.CounterParty.GetDDLBankName(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FillPaymentMethod(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_counterparty.CounterParty.GetDDLPaymentMethod(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FillUniqeuid(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_counterparty.CounterParty.GetDDLUniqueID(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FillIdentify(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_counterparty.CounterParty.GetDDLIdentifyType(datastr, p =>
            {
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
            api_agencyandrating.Agency.GetDDLAgencyCode(datastr, p =>
            {
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
            api_agencyandrating.Rating.GetDDLLocalRating(agencycode, shortlongterm.Length > 1 ? shortlongterm[0].ToString() : shortlongterm, datatext, p =>
             {
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
            api_agencyandrating.Rating.GetDDLForeignRating(agencycode, shortlongterm.Length > 1 ? shortlongterm[0].ToString() : shortlongterm, datatext, p =>
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
            api_static.Holiday.GetDDLCurrency(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillFormula(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.Config.GetConfigForDropdown("CP_HAIRCUT", p =>
            {
                if (p.Success)
                {
                    foreach (ConfigModel item in p.Data.ConfigResultModel)
                    {
                        DDLItemModel list = new DDLItemModel();
                        list.Text = item.item_code;
                        list.Value = item.item_code;
                        res.Add(list);
                    }
                }
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCalculateType(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.Config.GetConfigForDropdown("CP_CALTYPE", p =>
            {
                if (p.Success)
                {
                    foreach (ConfigModel item in p.Data.ConfigResultModel)
                    {
                        DDLItemModel list = new DDLItemModel();
                        list.Text = item.item_code;
                        list.Value = item.item_code;
                        res.Add(list);
                    }
                }
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillSourceType(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_exchange.ExchangeRate.GetDDLExchangeRateSource(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillRateType(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_exchange.ExchangeRate.GetDDLExchangeRateType(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHairCut()
        {
            try
            {
                return Json(new
                {
                    cur = "THB",
                    formula = "F1",
                    calType = "1+(Haircut)",
                    message = ""
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    message = ex.ToString()
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FillFundType(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_counterparty.CounterParty.GetDDLFundType(datastr, p =>
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