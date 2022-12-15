using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.CounterParty;
using GM.Data.Result.CounterParty;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
   [Autorize]  
   [Audit]  
   public class CounterPartyFundController : BaseController
   {
      CounterPartyEntities api_counterparty = new CounterPartyEntities();   
      StaticEntities api_static = new StaticEntities();   

      // GET: CounterPartyFund   
      [RoleScreen(RoleScreen.VIEW)]   
      public ActionResult Index()   
      {
            return View();   
      }  

      [HttpPost]  
      public ActionResult Search(DataTableAjaxPostModel model)  
      {
            ResultWithModel<CounterPartyFundResult> result = new ResultWithModel<CounterPartyFundResult>();    
            CounterPartyFundModel counterpartyfundmodel = new CounterPartyFundModel();    
            try 
            {
                  // string searchvalue = Request["search[value]"];  

                  // Add Paging    
                  Paging paging - new PagingModel();   
                  paging.PageNumber = model.pageno;   
                  paging.RecordPerPage = model.length;   
                  counterpartyfundModel.paging = paging;   

                  // Add orderby
                  var orders = new List<OrderByModel>();   

                  if (model.order != null)   
                  {
                        model.order.ForEach(o =>   
                        {
                              var col = model.columns[o.column];   
                              orders.Add(new OrderByModel { Name = col.data, SortDirection = (o.dir.Equals("desc") ? SortDirection.Descending : SortDirection.Ascending) });   
                        });   
                  }   

                  counterPartyfundModel.orderby = orders;   

                  var columns = model.columns.Where(o => o.search.value != null).ToList();   
                  columns.ForEach(column =>  
                  {
                        switch (column.data)    
                        {
                              case "counter_party_code":  
                                    counterpartyfundModel.counter_party_code = column.search.value;  
                                    break;  
                              case "fund_code";  
                                    counterpartyfundmodel.fund_code = column.search.value;  
                                    break;   
                        }
                  });  

                  api_counterparty.CounterPartyFund.GetCounterPartyFundList(counterpartyfundmodel, p =>
                  {
                    result = p;
                  })   
            }  
            catch (Exception ex)  
            {
                result.Message = ex.Message;   
            }
            retun Json(new    
            {
                draw = model.draw,  
                recordsTotal = result.HowManyRecord,  
                recordsFiltered = result.HowManyRecord,   
                Message = result.Message,
                data = result.Data != null ? result.Data.CounterPartyFundResultModel : new List<CounterPartyFundModel>()   
            });   
      }

      [HttpPost]  
      public ActionResult Add(CounterPartyFundModel CounterPartyFundModel)   
      {
        if (CounterPartyFundModel.page_name == "addpage")   
        {
            new RoleScreenAttribute(RoleScreen.CREATE);   

            ResultWithModel<CounterPartyFundResult> res = new ResultWithModel<CounterPartyFundResult>();   
            CounterPartyFundModel.create_by = HttpContext.User.Identity.Name;   

            CounterPartyFundModel = SetStatusFlag(CounterPartyFundModel);   

            if (CounterPartyFundModel.title_name2 != null && CounterPartyFundModel.title_name2 != string.Empty)
            {
                CounterPartyFundModel.title_name = CounterPartyFundModel.title_name2;  
            }

            if (CounterPartyFundModel.margin_type_id == null)  
            {
                CounterPartyFundModel.Margin = null;   
            }  
            else 
            {
                CounterPartyFundModel.Margin = new CounterPartyFundMarginModel();   
                CounterPartyFundModel.Margin.borrow_only_flag = CounterPartyFundModel.borrow_only_flag;   
                CounterPartyFundModel.Margin.counter_party_id = CounterPartyFundModel.counter_party_id;   
                CounterPartyFundModel.Margin.except_margin_flag = CounterPartyFundModel.except_margin_flag;  
                CounterPartyFundModel.Margin.margin_in_term_id = CounterPartyFundModel.margin_in_term_id;
                CounterPartyFundModel.Margin.margin_in_type_id = CounterPartyFundModel.margin_in_type_id;
                CounterPartyFundModel.Margin.margin_type_id = CounterPartyFundModel.margin_type_id;  
                CounterPartyFundModel.Margin.threshold = CounterPartyFundModel.threshold;    
            }

            if (ModelState.IsValid)    
            {
                api_counterparty.CounterPartyFund.CreateCounterPartyFund(CounterPartyFundModel, p => 
                {
                    res = p;  
                });   

                if (res.Success)   
                {
                    return RedirectToAction("Index");    
                }  

                else 
                {
                    ModelState.AddModelError("Exception", res.Message);  
                }  
            }
            else
            {
                var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();   
                Models.ForEach(field => 
                {
                    field.Errors.ToList().forEach(error =>  
                    {
                        res.Message += error.ErrorMessage;  
                    });  

                });  
            }

            if (CounterPartyFundModel.Margin == null)  
            {
                CounterPartyFundModel.Margin = new CounterPartyFundMarginModel();   
            }  
            if (CounterPartyFundModel.Payment == null)  
            {
                CounterPartyFundModel.Payment = new List<CounterPartyFundPaymentModel>();   
            }
            if (CounterPartyFundModel.PaymentRightModal == null)   
            {
                CounterPartyFundModel.PaymentRightModal = new CounterPartyFundPaymentModel();  
            }
            if (CounterPartyFundModel.Identify == null)
            {
                CounterPartyFundModel.Identify = new List<CounterPartyFundIdentifyModel>();
            }
            if (CounterPartyFundModel.IdentifyRightModal == null)
            {
                CounterPartyFundModel.IdentifyRightModal = new CounterPartyFundIdentifyModel();
            }

            return View(CounterPartyFundModel);
        }
        else
        {
            new RoleScreenAttribute(RoleScreen.EDIT);  

            ResultWithModel<CounterPartyFundResult> res = new ResultWithModel<CounterPartyFundResult>();   
            CounterPartyFundModel.create_by = HttpContext.user.Identity.Name;  

            CounterPartyFundModel = SetStatusFlag(CounterPartyFundModel);  

            if (CounterPartyFundModel.title_name2 != null && CounterPartyFundModel.title_name2 != string.Empty)  
            {
                CounterPartyFundModel.title_name = CounterPartyFundModel.title_name2;    
            }

            if (CounterPartyFundModel.margin_type_id == null)   
            {
                CounterPartyFundModel.Margin = null;
            }
            else
            {
                CounterPartyFundModel.Margin = new CounterPartyFundMarginModel();  
                CounterPartyFundModel.Margin.borrow_only_flag = CounterPartyFundModel.borrow_only_flag;  
                CounterPartyFundModel.Margin.counter_party_id = CounterPartyFundModel.counter_party_id;  
                CounterPartyFundModel.Margin.except_margin_flag = CounterPartyFundModel.except_margin_flag;  
                CounterPartyFundModel.Margin.margin_in_term_id = CounterPartyFundModel.margin_in_term_id;  
                CounterPartyFundModel.Margin.margin_in_type_id = CounterPartyFundModel.margin_in_type_id;  
                CounterPartyFundModel.Margin.margin_type_id = CounterPartyFundModel.margin_type_id;   
                CounterPartyFundModel.Margin.threshold = CounterPartyFundModel.threshold;  
                CounterPartyFundModel.Margin.counter_party_id = CounterPartyFundModel.counter_party_id;  
            }
            if (CounterPartyFundModel.Payment != null)   
            {
                foreach (var banklist in CounterPartyFundModel.Payment)  
                {
                    banklist.counter_party_id = CounterPartyFundModel.counter_party_id;    
                }  
            } 
            else
            {
                CounterPartyFundModel.Payment = null;  
            }
            if (CounterPartyFundModel.Identity != null)   
            {
                foreach (var identity in CounterPartyFundModel.Identify)  
                {
                    identify.counter_party_id = CounterPartyFundModel.counter_party_id;    
                }  
            }
            else 
            {
                CounterPartyFundModel.identify = null;   
            }  

            if (ModelState,IsValid)   
            {
                api_counterparty.CounterPartyFund.UpdateCounterPartyFund(CounterPartyFundModel, p => 
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

            if (CounterPartyFundModel.Payment == null)   
            {
                CounterPartyFundModel.Payment = new List<CounterPartyFundPaymentModel>();  
            }
            if (CounterPartyFundModel.PaymentRightModal == null)   
            {
                CounterPartyFundModel.PaymentRightModal = new CounterPartyFundPaymentModel();
            } 
            if (CounterPartyFundModel.Identify == null)
            {
                CounterPartyFundModel.Identify = new List<CounterPartyFundIdentifyModel>();
            }
            if (CounterPartyFundModel.IdentifyRightModal == null)
            {
                CounterPartyFundModel.IdentifyRightModal = new CounterPartyFundIdentifyModel();
            }
            return View(CounterPartyFundModel);
        }
    }

    public ActionResult Add(string fundid)   
        {
            if (!string.IsNullOrEmpty(fundid))
            {
                new RoleScreenAttribute(RoleScreen.VIEW);

                ResultWithModel<CounterPartyFundResult> result = new ResultWithModel<CounterPartyFundResult>();
                ResultWithModel<CounterPartyFundPaymentResult> resultpayment = new ResultWithModel<CounterPartyFundPaymentResult>();
                ResultWithModel<CounterPartyFundIdentifyResult> resultidentify = new ResultWithModel<CounterPartyFundIdentifyResult>();
                ResultWithModel<CounterPartyFundMarginResult> resultmargin = new ResultWithModel<CounterPartyFundMarginResult>();
                //CounterPartyModel counterpartymodel = new CounterPartyModel();
                CounterPartyFundModel CounterPartyFund = new CounterPartyFundModel();
                CounterPartyFundPaymentModel CounterPartyFundPayment = new CounterPartyFundPaymentModel();
                CounterPartyFundIdentifyModel CounterPartyFundIdentify = new CounterPartyFundIdentifyModel();
                CounterPartyFundMarginModel CounterPartyFundMargin = new CounterPartyFundMarginModel();
                try
                {
                    //Add Paging
                    PagingModel paging = new PagingModel();
                    CounterPartyFund.paging = paging;
                    //Add Orderby
                    var orders = new List<OrderByModel>();
                    CounterPartyFund.ordersby = orders;
                    //Add counterparty id
                    CounterPartyFund.fund_id = int.Parse(fundid); ;
                    CounterPartyFundPayment.fund_id = int.Parse(fundid); ;
                    CounterPartyFundIdentify.fund_id = int.Parse(fundid); ;
                    CounterPartyFundMargin.fund_id = int.Parse(fundid); ;

                    api_counterparty.CounterPartyFund.GetCounterPartyFundList(CounterPartyFund, p =>
                    {
                        result = p;
                    });

                    api_counterparty.CounterPartyFund.GetCounterPartyFundMarginList(CounterPartyFundMargin, p =>
                    {
                        resultmargin = p;
                    });

                    api_counterparty.CounterPartyFund.GetCounterPartyFundPayment(CounterPartyFundPayment, p =>
                    {
                        resultpayment = p;
                    });

                    api_counterparty.CounterPartyFund.GetCounterPartyFundIdentifyList(CounterPartyFundIdentify, p =>
                    {
                        resultidentify = p;
                    });

                    CounterPartyFund = result.Data.CounterPartyFundResultModel[0];

                    if (resultmargin.Data == null)
                    {
                        resultmargin.Data = new CounterPartyFundMarginResult();
                    }
                    if (resultpayment.Data == null)
                    {
                        resultpayment.Data = new CounterPartyFundPaymentResult();
                    }
                    if (resultidentify.Data == null)
                    {
                        resultidentify.Data = new CounterPartyFundIdentifyResult();
                    }

                    if (resultmargin.Data.CounterPartyFundMarginResultModel.Count > 0)
                    {
                        CounterPartyFund.Margin = resultmargin.Data.CounterPartyFundMarginResultModel[0];
                        CounterPartyFund.borrow_only_flag = CounterPartyFund.Margin.borrow_only_flag;
                        //CounterPartyFund.counter_party_id = CounterPartyFund.Margin.counter_party_id;
                        CounterPartyFund.except_margin_flag = CounterPartyFund.Margin.except_margin_flag;
                        CounterPartyFund.margin_in_term_id = CounterPartyFund.Margin.margin_in_term_id;
                        CounterPartyFund.margin_in_type_id = CounterPartyFund.Margin.margin_in_type_id;
                        CounterPartyFund.margin_type_id = CounterPartyFund.Margin.margin_type_id;
                        CounterPartyFund.threshold = CounterPartyFund.Margin.threshold;
                        CounterPartyFund.margin_in_term_name = CounterPartyFund.Margin.margin_in_term_name;
                        CounterPartyFund.margin_in_type_name = CounterPartyFund.Margin.margin_in_type_name;
                        CounterPartyFund.margin_type_name = CounterPartyFund.Margin.margin_type_name;
                    }
                    else
                    {
                        CounterPartyFund.Margin = new CounterPartyFundMarginModel();
                    }


                    CounterPartyFund.Payment = resultpayment.Data.CounterPartyFundPaymentResultModel;
                    CounterPartyFund.PaymentRightModal = new CounterPartyFundPaymentModel();

                    CounterPartyFund.Identify = resultidentify.Data.CounterPartyFundIdentifyResultModel;
                    CounterPartyFund.IdentifyRightModal = new CounterPartyFundIdentifyModel();

                    if (CounterPartyFund.active_flag == true)
                    {
                        CounterPartyFund.statusdata = "Active";
                    }
                    else if (CounterPartyFund.active_flag == false)
                    {
                        CounterPartyFund.statusdata = "UnActive";
                    }
                    CounterPartyFund.page_name = "editpage";
                    return View(CounterPartyFund);
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                new RoleScreenAttribute(RoleScreen.CREATE);

                CounterPartyFundModel CounterPartyFund = new CounterPartyFundModel();

                CounterPartyFund.Payment = new List<CounterPartyFundPaymentModel>();
                CounterPartyFundPaymentModel payment = new CounterPartyFundPaymentModel();
                CounterPartyFund.Payment.Add(payment);
                CounterPartyFund.PaymentRightModal = new CounterPartyFundPaymentModel();

                CounterPartyFund.Identify = new List<CounterPartyFundIdentifyModel>();
                CounterPartyFundIdentifyModel identify = new CounterPartyFundIdentifyModel();
                CounterPartyFund.Identify.Add(identify);
                CounterPartyFund.IdentifyRightModal = new CounterPartyFundIdentifyModel();

                CounterPartyFund.Margin = new CounterPartyFundMarginModel();

                CounterPartyFund.statusdata = "New";
                CounterPartyFund.page_name = "addpage";

                return View(CounterPartyFund);
            }
        }

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Edit(int id)
        {
            return RedirectToAction("Add", new { fundid = id });
        }

        public class Data
        {
            public int fundid { get; set; }
            public string insumentID { get; set; }

        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult Deletes(Data data)
        {
            var rwm = new ResultWithModel<CounterPartyFundResult>();
            CounterPartyFundModel view = new CounterPartyFundModel();
            view.create_by = HttpContext.User.Identity.Name;
            view.fund_id = data.fundid;
            try
            {
                api_counterparty.CounterPartyFund.DeleteCounterPartyFund(view, p =>
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

        public CounterPartyFundModel SetStatusFlag(CounterPartyFundModel model)
        {
            CounterPartyFundModel CounterPartyFund = model;
            if (CounterPartyFund.statusdata == "New" || CounterPartyFund.statusdata == "Active")
            {
                CounterPartyFund.active_flag = true;
            }
            else if (CounterPartyFund.statusdata == "UnActive")
            {
                CounterPartyFund.active_flag = false;
            }
            return CounterPartyFund;
        }

        public ActionResult FillCounterParty(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_counterparty.CounterPartyFund.GetDDLCounterParty(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillTitle(string datastr)
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

        public ActionResult FillFundType(string datastr)
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

        public ActionResult FillCustodian(string datastr)
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

        //  Start DDL FOR Payment Tab
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
        //  End  DDL FOR Margin Tab

        //DDL FOR Payment Tab
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
        //End  FOR Payment Tab

        //DDL FOR Identify Tab
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
        //End  DDL FOR Identify Tab

        public ActionResult GetInsumentName(Data data)
        {
            //  RPTransResult res = new RPTransResult();
            List<CounterPartyFundModel> res = new List<CounterPartyFundModel>();
            CounterPartyFundModel counterPartyFundModel = new CounterPartyFundModel();
            counterPartyFundModel.counter_party_id = Convert.ToInt32(data.insumentID);

            api_counterparty.CounterPartyFund.GetCounterParty(counterPartyFundModel, p =>
            {
                if (p.Success)
                {
                    res = p.Data.CounterPartyFundResultModel;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}