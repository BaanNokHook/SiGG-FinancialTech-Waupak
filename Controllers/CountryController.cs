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
    public class CountryController : BaseController
    {

        StaticEntities api_static = new StaticEntities();

        // GET: Role
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            string searchvalue = Request["search[value]"];
            ResultWithModel<CountryResult> result = new ResultWithModel<CountryResult>();
            CountryModel CountryModel = new CountryModel();
            //Add Paging
            PagingModel paging = new PagingModel();
            paging.PageNumber = model.pageno;
            paging.RecordPerPage = model.length;
            CountryModel.paging = paging;

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

            CountryModel.ordersby = orders;

            var columns = model.columns.Where(o => o.search.value != null).ToList();
            columns.ForEach(column =>
            {
                switch (column.data)
                {
                    case "country_code":
                        CountryModel.country_code = column.search.value;
                        break;
                    case "country_id":
                        CountryModel.country_id = Convert.ToInt32(column.search.value);
                        break;
                    case "country_desc":
                        CountryModel.country_desc = column.search.value;
                        break;
                    case "domicile_code":
                        CountryModel.domicile_code = column.search.value;
                        break;
                    case "domicile_desc":
                        CountryModel.domicile_desc = column.search.value;
                        break;
                    case "active_flag":
                        CountryModel.active_flag = Boolean.Parse(column.search.value == "true" ? "true" : "false");
                        break;
                }
            });


            api_static.Country.GetCountryList(CountryModel, p => {
                result = p;
            });

            return Json(new
            {
                draw = model.draw,
                recordsTotal = result.HowManyRecord,
                recordsFiltered = result.HowManyRecord,
                data = result.Data != null ? result.Data.CountryResultModel : new List<CountryModel>()
            });

            //return Json(new {  data = result.Data.UserModels  }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Create(CountryViewModel view)
        {
            var rwm = new ResultWithModel<CountryResult>();
            view.FormAction.create_by = HttpContext.User.Identity.Name;
            try
            {
                if (ModelState.IsValid)
                {
                    api_static.Country.CreateCountry(view.FormAction, p => {
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

        //Action Update
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Edit(int id)
        {
            var result = new ResultWithModel<CountryResult>();

            CountryModel model = new CountryModel();
            PagingModel paging = new PagingModel();
            paging.PageNumber = 0;
            paging.RecordPerPage = 0;
            model.paging = paging;
            model.country_id = id;
            model.create_by = HttpContext.User.Identity.Name;

            api_static.Country.GetCountryEdit(model, p =>
            {
                result = p;
            });
            return Json((result.Data.CountryResultModel.Count > 0 ? result.Data.CountryResultModel[0] : new CountryModel()), JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Edit(CountryViewModel view)
        {
            var rwm = new ResultWithModel<CountryResult>();

            try
            {
                if (ModelState.IsValid)
                {
                    api_static.Country.UpdateCountry(view.FormAction, p => {
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
            public int country_id { get; set; }
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult Deletes(Data data)
        {
            var rwm = new ResultWithModel<CountryResult>();
            CountryModel view = new CountryModel();
            view.create_by = HttpContext.User.Identity.Name;
            view.country_id = data.country_id;
            try
            {
                api_static.Country.DeleteCountry(view, p =>
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