using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.UserAndScreen;
using GM.Data.Result.UserAndScreen;
using GM.Data.View.UserAndScreen;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]

    public class PolicyRateController : BaseController
    {
        UserAndScreenEntities Api_UserAndScreen = new UserAndScreenEntities();
        GM.Data.Helper.Utility utility = new GM.Data.Helper.Utility();
        // Action Index
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            ResultWithModel<PolicyRateResult> Result = new ResultWithModel<PolicyRateResult>();
            PolicyRateModel policyRateModel = new PolicyRateModel();

            //Add Paging
            PagingModel paging = new PagingModel();
            paging.PageNumber = model.pageno;
            paging.RecordPerPage = model.length;
            policyRateModel.paging = paging;

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

            policyRateModel.ordersby = orders;

            var columns = model.columns.Where(o => o.search.value != null).ToList();
            columns.ForEach(column =>
            {
                switch (column.data)
                {
                    case "policy_date":
                        policyRateModel.policy_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                        break;
                    case "cost_of_fund_date":
                        policyRateModel.cost_of_fund_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                        break;
                    case "cur":
                        policyRateModel.cur = column.search.value;
                        break;
                }
            });

            Api_UserAndScreen.PolicyRate.GetPolicyRateList(policyRateModel, p => {
                Result = p;
            });

            return Json(new
            {
                draw = model.draw,
                recordsTotal = Result.HowManyRecord,
                recordsFiltered = Result.HowManyRecord,
                data = Result.Data != null ? Result.Data.PolicyRateResultModel : new List<PolicyRateModel>()
            });
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Create(PolicyRateViewModel model)
        {
            var Result = new ResultWithModel<PolicyRateResult>();
            try
            {
                model.FormAction.create_by = HttpContext.User.Identity.Name;
                if (ModelState.IsValid)
                {
                    Api_UserAndScreen.PolicyRate.CreatePolicyRate(model.FormAction, p => {
                        Result = p;
                    });

                    if (!Result.Success)
                    {
                        ModelState.AddModelError("", Result.Message);
                    }
                }
                else
                {
                    var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();

                    Models.ForEach(field =>
                    {
                        field.Errors.ToList().ForEach(error =>
                        {
                            Result.Message += error.ErrorMessage;
                        });

                    });
                }
            }
            catch (Exception ex)
            {
                Result.Message = ex.Message;
            }

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Edit(string policy_date, string cur)
        {
            PolicyRateModel Model = new PolicyRateModel();
            ResultWithModel<PolicyRateResult> result = new ResultWithModel<PolicyRateResult>();
            PagingModel paging = new PagingModel();
            paging.PageNumber = 0;
            paging.RecordPerPage = 0;
            Model.paging = paging;
            Model.policy_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(policy_date);
            Model.cur = cur;
            Model.update_by = HttpContext.User.Identity.Name;

            Api_UserAndScreen.PolicyRate.GetPolicyRateList(Model, p => {
                result = p;
            });
            //return View(model);
            return Json((result.Data.PolicyRateResultModel.Count > 0 ? result.Data.PolicyRateResultModel[0] : new PolicyRateModel()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Edit(PolicyRateViewModel model)
        {
            var Result = new ResultWithModel<PolicyRateResult>();
            try
            {
                if (ModelState.IsValid)
                {
                    Api_UserAndScreen.PolicyRate.UpdatePolicyRate(model.FormAction, p => {
                        Result = p;
                    });

                    if (!Result.Success)
                    {
                        ModelState.AddModelError("", Result.Message);
                    }
                }
                else
                {
                    var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();
                    Models.ForEach(field =>
                    {
                        field.Errors.ToList().ForEach(error =>
                        {
                            Result.Message += error.ErrorMessage;
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                Result.Message = ex.Message;
            }

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public class Data
        {
            public string policy_date { get; set; }
            public string cur { get; set; }
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult Delete(Data data)
        {
            var Result = new ResultWithModel<PolicyRateResult>();
            PolicyRateModel Model = new PolicyRateModel();
            try
            {
                Model.policy_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(data.policy_date);
                Model.cur = data.cur;
                Model.create_by = HttpContext.User.Identity.Name;

                Api_UserAndScreen.PolicyRate.DeletePolicyRate(Model, p =>
                {
                    Result = p;
                });

                if (Result.Success == false)
                {
                    return Json(new { success = false, responseText = Result.Message ,refcode = Result.RefCode}, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, responseText = "Your message successfuly sent!", refcode = Result.RefCode }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.ToString(), refcode = Result.RefCode }, JsonRequestBehavior.AllowGet);
            }
        }

        // // Function : Binding DDL        

        public ActionResult FillCur(string cur)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            Api_UserAndScreen.TraderLimit.GetDDLCur(cur, p =>
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