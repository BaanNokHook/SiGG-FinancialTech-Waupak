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
    public class TraderController : BaseController
    {
        // GET: Trader
        UserAndScreenEntities api_userandscreen = new UserAndScreenEntities();

        // GET: Role
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            // ViewBag.ActiveFlags     = new SelectList(api_master.GetDDLActiveFlags(), "Value", "Text");
            //  ViewBag.DeskGroups      = new SelectList(api_master.GetDDLDeskGroups(), "Value", "Text");
            return View();
        }
              
        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            string searchvalue = Request["search[value]"];
            ResultWithModel<TraderResult> result = new ResultWithModel<TraderResult>();
            TraderModel tradermodel = new TraderModel();
            //Add Paging
            PagingModel paging = new PagingModel();
            paging.PageNumber = model.pageno;
            paging.RecordPerPage = model.length;
            tradermodel.paging = paging;

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

            tradermodel.ordersby = orders;

            var columns = model.columns.Where(o => o.search.value != null).ToList();
            columns.ForEach(column =>
            {
                switch (column.data)
                {
                    case "trader_id":
                        tradermodel.trader_id = column.search.value;
                        break;
                    case "trader_engname":
                        tradermodel.trader_engname = column.search.value;
                        break;
                    case "trader_thainame":
                        tradermodel.trader_thainame = column.search.value;
                        break;
                    case "active_flag":
                        tradermodel.active_flag = Boolean.Parse(column.search.value == "true" ? "true" : "false");
                        break;
                }
            });

            api_userandscreen.Trader.GetTraderList(tradermodel, p => {
                result = p;
            });
            return Json(new
            {
                draw = model.draw,
                recordsTotal = result.HowManyRecord,
                recordsFiltered = result.HowManyRecord,
                data = result.Data != null ? result.Data.TraderResultModel : new List<TraderModel>()
            });
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Create(TraderViewModel view)
        {
            var rwm = new ResultWithModel<TraderResult>();
            view.FormAction.create_by = HttpContext.User.Identity.Name;
            try
            {
                //if (ModelState.ContainsKey("FormAction.desk_group_id"))
                //{
                //    ModelState["FormAction.desk_group_id"].Errors.Clear();
                //}

                if (ModelState.IsValid)
                {
                    api_userandscreen.Trader.CreateTrader(view.FormAction, p => {
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

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Edit(string id)
        {
            TraderModel model = new TraderModel();
            ResultWithModel<TraderResult> result = new ResultWithModel<TraderResult>();
            PagingModel paging = new PagingModel();
            paging.PageNumber = 0;
            paging.RecordPerPage = 0;
            model.paging = paging;
            model.trader_id = id;
            model.create_by = HttpContext.User.Identity.Name;

            api_userandscreen.Trader.GetTraderEdit(model, p => {
                result = p;
            });
            //return View(model);
            return Json((result.Data.TraderResultModel.Count > 0 ? result.Data.TraderResultModel[0] : new TraderModel()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Edit(TraderViewModel view)
        {
            var rwm = new ResultWithModel<TraderResult>();

            try
            {
                if (ModelState.IsValid)
                {

                    api_userandscreen.Trader.UpdateTrader(view.FormAction, p => {
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

        public class Data
        {
            public string traderid { get; set; }
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult Deletes(Data data)
        {
            var rwm = new ResultWithModel<TraderResult>();
            TraderModel view = new TraderModel();
            view.create_by = HttpContext.User.Identity.Name;
            view.trader_id = data.traderid;
            try
            {
                api_userandscreen.Trader.DeleteTrader(view, p =>
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
    }
}