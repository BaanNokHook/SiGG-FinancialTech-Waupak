using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.Static;
using GM.Data.Result.Static;
using GM.Data.View.Static;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class PaymentMethodController : BaseController
    {
        StaticEntities api_StaticPayment = new StaticEntities();

        // Action : Index
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            var PaymentMethodView = new PaymentMethodViewModel();
            PaymentMethodModel PaymentMethod = new PaymentMethodModel();

            PaymentMethodView.FormSearch = PaymentMethod;
            PaymentMethodView.FormAction = PaymentMethod;
            ViewBag.SystemType = new SelectList(getAllSystemTypesList(), "Value", "Text");

            return View(PaymentMethodView);
        }

        public List<SelectListItem> getAllSystemTypesList()
        {
            List<SelectListItem> myList = new List<SelectListItem>();
            var data = new[]{
                 new SelectListItem{ Value="BOTH",Text="BOTH"},
                 new SelectListItem{ Value="FITS",Text="FITS"},
                 new SelectListItem{ Value="REPO",Text="REPO"},
             };
            myList = data.ToList();
            return myList;
        }

        public ActionResult FillPaymentFlag(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_StaticPayment.PaymentMethod.GetDDLPaymentFlag(datastr, p => {
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
            ResultWithModel<PaymentMethodResult> result = new ResultWithModel<PaymentMethodResult>();
            PaymentMethodModel PaymentMethodModel = new PaymentMethodModel();
            try
            {
                // string searchvalue = Request["search[value]"];

                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                PaymentMethodModel.paging = paging;

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

                PaymentMethodModel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null).ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "payment_method":
                            PaymentMethodModel.payment_method = column.search.value;
                            break;
                        case "payment_flag":
                            PaymentMethodModel.payment_flag = Convert.ToInt32(column.search.value);
                            break;
                        case "system_type":
                            PaymentMethodModel.system_type = column.search.value;
                            break;
                    }
                });

                api_StaticPayment.PaymentMethod.GetPaymentMethodList(PaymentMethodModel, p =>
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
                data = result.Data != null ? result.Data.PaymentMethodResultModel : new List<PaymentMethodModel>()
            });
        }

        // Action : Add
        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Create(PaymentMethodViewModel view)
        {
            var rwm = new ResultWithModel<PaymentMethodResult>();
            try
            {
                if (ModelState.IsValid)
                {
                    view.FormAction.create_by = HttpContext.User.Identity.Name;
                    api_StaticPayment.PaymentMethod.CreatePaymentMethod(view.FormAction, p => {
                        rwm = p;
                    });

                    if (!rwm.Success)
                    {
                        ModelState.AddModelError("", rwm.Message);
                    }
                }
                else
                {
                    var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();
                    Models.ForEach(field =>
                    {
                        field.Errors.ToList().ForEach(error =>
                        {
                            rwm.Message += error.ErrorMessage;
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                rwm.Message = ex.Message;
            }

            return Json(rwm, JsonRequestBehavior.AllowGet);
        }

        // Action : Edit
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Edit(string id)
        {
            var result = new ResultWithModel<PaymentMethodResult>();

            PaymentMethodModel model = new PaymentMethodModel();
            PagingModel paging = new PagingModel();
            paging.PageNumber = 0;
            paging.RecordPerPage = 0;
            model.paging = paging;
            model.payment_method = id;
            model.create_by = HttpContext.User.Identity.Name;

            api_StaticPayment.PaymentMethod.GetPaymentMethodEdit(model, p =>
            {
                result = p;
            });
            return Json((result.Data.PaymentMethodResultModel.Count > 0 ? result.Data.PaymentMethodResultModel[0] : new PaymentMethodModel()), JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Edit(PaymentMethodViewModel view)
        {
            var rwm = new ResultWithModel<PaymentMethodResult>();

            try
            {
                if (ModelState.IsValid)
                {
                    api_StaticPayment.PaymentMethod.UpdatePaymentMethod(view.FormAction, p => {
                        rwm = p;
                    });

                    if (!rwm.Success)
                    {
                        ModelState.AddModelError("", rwm.Message);
                    }
                }

                else
                {
                    var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();

                    Models.ForEach(field =>
                    {
                        field.Errors.ToList().ForEach(error =>
                        {
                            rwm.Message += error.ErrorMessage;
                        });

                    });
                }
            }
            catch (Exception ex)
            {
                rwm.Message = ex.Message;
            }

            return Json(rwm, JsonRequestBehavior.AllowGet);
        }

        // Action : Delete
        [RoleScreen(RoleScreen.DELETE)]
        public class Data
        {
            public string payment_method { get; set; }
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult Deletes(Data data)
        {
            var rwm = new ResultWithModel<PaymentMethodResult>();
            PaymentMethodModel view = new PaymentMethodModel();
            view.create_by = HttpContext.User.Identity.Name;
            view.payment_method =  data.payment_method;
            try
            {
                api_StaticPayment.PaymentMethod.DeletePaymentMethod(view, p =>
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
    }
}