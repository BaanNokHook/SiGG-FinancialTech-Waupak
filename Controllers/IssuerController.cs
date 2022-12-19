using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.CounterParty;
using GM.Data.Result.CounterParty;
using GM.Data.View.CounterParty;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class IssuerController : BaseController
    {
        CounterPartyEntities api_CptyIssuer = new CounterPartyEntities();
        StaticEntities api_static = new StaticEntities();
        AgencyAndRatingEntities api_agencyandrating = new AgencyAndRatingEntities();

        // Action : Index
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            var IssuerView = new IssuerViewModel();
            IssuerModel issuer = new IssuerModel();

            IssuerView.FormSearch = issuer;

            return View(IssuerView);
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            ResultWithModel<IssuerResult> result = new ResultWithModel<IssuerResult>();
            IssuerModel IssuerModel = new IssuerModel();
            try
            {
                // string searchvalue = Request["search[value]"];

                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                IssuerModel.paging = paging;

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

                IssuerModel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null).ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "issuer_code":
                            IssuerModel.issuer_code = column.search.value;
                            break;
                        case "issuer_name":
                            IssuerModel.issuer_name = column.search.value;
                            break;
                        case "issuer_shortname":
                            IssuerModel.issuer_shortname = column.search.value;
                            break;
                        case "issuer_thainame":
                            IssuerModel.issuer_thainame = column.search.value;
                            break;
                        case "issuer_type_code":
                            IssuerModel.issuer_type_code = column.search.value;
                            break;
                        case "issuer_type_desc":
                            IssuerModel.issuer_type_desc = column.search.value;
                            break;
                        case "tel_no":
                            IssuerModel.tel_no = column.search.value;
                            break;

                    }
                });

                api_CptyIssuer.Issuer.GetIssuerList(IssuerModel, p =>
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
                data = result.Data != null ? result.Data.IssuerResultModel : new List<IssuerModel>()
            });
        }

        public class Data
        {
            public int issuer_id { get; set; }
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult Deletes(Data data)
        {
            var rwm = new ResultWithModel<IssuerResult>();
            IssuerModel view = new IssuerModel();
            view.create_by = HttpContext.User.Identity.Name;
            view.issuer_id = data.issuer_id;

            try
            {
                api_CptyIssuer.Issuer.DeleteIssuer(view, p =>
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

        // Action : Add
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Add()
        {
            IssuerModel Issuer = new IssuerModel();
            Issuer.Identify = new List<IssuerIdentifyModel>();
            Issuer.Rating = new List<IssuerRatingModel>();

            IssuerIdentifyModel indentify = new IssuerIdentifyModel();
            Issuer.Identify.Add(indentify);
            Issuer.IdentifyRightModal = new IssuerIdentifyModel();

            IssuerRatingModel rating = new IssuerRatingModel();
            Issuer.Rating.Add(rating);
            Issuer.RatingRightModal = new IssuerRatingModel();

            Issuer.statusdata = "New";

            return View(Issuer);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Add(IssuerModel IssuerModel)
        {
            ResultWithModel<IssuerResult> rwm = new ResultWithModel<IssuerResult>();

            IssuerModel.create_by = HttpContext.User.Identity.Name;
            IssuerModel = SetStatusFlag(IssuerModel);

            if (ModelState.IsValid)
            {
                api_CptyIssuer.Issuer.CreateIssuer(IssuerModel, p => {
                    rwm = p;
                });

                if (rwm.Success)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    if (rwm.Message.Contains("Duplicate"))
                    {
                        ModelState.AddModelError("issuer_code", "Duplicate Issuer Code");
                    }
                    else
                    {
                        ModelState.AddModelError("Exception", rwm.Message);
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
                        rwm.Message += error.ErrorMessage;
                    });

                });
            }

            if (IssuerModel.Identify == null)
            {
                IssuerModel.Identify = new List<IssuerIdentifyModel>();
            }
            if (IssuerModel.IdentifyRightModal == null)
            {
                IssuerModel.IdentifyRightModal = new IssuerIdentifyModel();
            }

            if (IssuerModel.Rating == null)
            {
                IssuerModel.Rating = new List<IssuerRatingModel>();
            }
            if (IssuerModel.RatingRightModal == null)
            {
                IssuerModel.RatingRightModal = new IssuerRatingModel();
            }

            return View(IssuerModel);
        }

        public IssuerModel SetStatusFlag(IssuerModel model)
        {
            IssuerModel CounterParty = model;
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

        // Action : Edit
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Edit(int id)
        {
            ResultWithModel<IssuerResult> result = new ResultWithModel<IssuerResult>();
            ResultWithModel<IssuerIdentifyResult> resultidentify = new ResultWithModel<IssuerIdentifyResult>();
            ResultWithModel<IssuerRatingResult> resultrating = new ResultWithModel<IssuerRatingResult>();

            IssuerModel Issuer = new IssuerModel();
            IssuerIdentifyModel IssuerIdentify = new IssuerIdentifyModel();
            IssuerRatingModel IssuerRating = new IssuerRatingModel();

            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                Issuer.paging = paging;

                //Add Orderby
                var orders = new List<OrderByModel>();
                Issuer.ordersby = orders;

                //Add counterparty id
                Issuer.issuer_id = id;
                IssuerIdentify.issuer_id = id;
                IssuerRating.issuer_id = id;

                api_CptyIssuer.Issuer.GetIssuerList(Issuer, p =>
                {
                    result = p;
                });

                api_CptyIssuer.Issuer.GetIssuerIdentifyList(IssuerIdentify, p =>
                {
                    resultidentify = p;
                });

                api_CptyIssuer.Issuer.GetIssuerRatingList(IssuerRating, p =>
                {
                    resultrating = p;
                });

                Issuer = result.Data.IssuerResultModel[0];
                if (resultidentify.Data == null)
                {
                    resultidentify.Data = new IssuerIdentifyResult();
                }
                if (resultrating.Data == null)
                {
                    resultrating.Data = new IssuerRatingResult();
                }


                Issuer.Identify = resultidentify.Data.IssuerIdentifyResultModel;
                Issuer.IdentifyRightModal = new IssuerIdentifyModel();

                Issuer.Rating = resultrating.Data.IssuerRatingResultModel;
                Issuer.RatingRightModal = new IssuerRatingModel();

                if (Issuer.active_flag == true && Issuer.verify_flag_bo == false)
                {
                    Issuer.statusdata = "UnApprove";
                }
                else if (Issuer.active_flag == false)
                {
                    Issuer.statusdata = "UnActive";
                }
                if (Issuer.active_flag == true && Issuer.verify_flag_bo == true)
                {
                    Issuer.statusdata = "Approve";
                }
            }
            catch
            {

            }
            return View(Issuer);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Edit(IssuerModel IssuerModel)
        {
            ResultWithModel<IssuerResult> res = new ResultWithModel<IssuerResult>();
            IssuerModel.create_by = HttpContext.User.Identity.Name;

            IssuerModel = SetStatusFlag(IssuerModel);

            if (IssuerModel.Identify != null)
            {
                foreach (var identify in IssuerModel.Identify)
                {
                    identify.issuer_id = IssuerModel.issuer_id;
                }
            }
            else
            {
                IssuerModel.Identify = null;
            }
            if (IssuerModel.Rating != null)
            {
                foreach (var rating in IssuerModel.Rating)
                {
                    rating.issuer_id = IssuerModel.issuer_id;
                }
            }
            else
            {
                IssuerModel.Rating = null;
            }
            if (ModelState.IsValid)
            {
                api_CptyIssuer.Issuer.UpdateIssuer(IssuerModel, p => {
                    res = p;
                });

                if (res.Success)
                {
                    return RedirectToAction("Index");
                }

                else
                {
                    if (res.Message.Contains("Duplicate"))
                    {
                        ModelState.AddModelError("issuer_code", "Duplicate Issuer Code");
                    }
                    else
                    {
                        ModelState.AddModelError("Exception", res.Message);
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
            }

            if (IssuerModel.Identify == null)
            {
                IssuerModel.Identify = new List<IssuerIdentifyModel>();
            }
            if (IssuerModel.IdentifyRightModal == null)
            {
                IssuerModel.IdentifyRightModal = new IssuerIdentifyModel();
            }

            if (IssuerModel.Rating == null)
            {
                IssuerModel.Rating = new List<IssuerRatingModel>();
            }
            if (IssuerModel.RatingRightModal == null)
            {
                IssuerModel.RatingRightModal = new IssuerRatingModel();
            }
            return View(IssuerModel);
        }

        // Panel : Issuer Detail
        public ActionResult FillIssuerType(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_CptyIssuer.Issuer.GetDDLIssuerType(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillIssuerGroup(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_CptyIssuer.Issuer.GetDDLIssuerGroup(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillTitleName(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.Title.GetDDLTitleName(datastr, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        // Panel : Issuer Address
        public ActionResult FillProvince(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.Province.GetDDLProvince(datastr, p => {
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
            api_static.District.GetDDLDistrict(dataint, datastr, p => {
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
            api_static.SubDistrict.GetDDLSubDistrict(dataint, datastr, p => {
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
            api_static.Country.GetDDLCountry(datastr, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        // Panel : Issuer Tax & Other
        public ActionResult FillKTB_ISIC(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_static.KtbIsic.GetDDLKTB_ISIC(datastr, p => {
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
            api_static.Custodian.GetDDLCustodian(datastr, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        // Modal : Identify
        public ActionResult FillUniqeuid(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_CptyIssuer.Issuer.GetDDLUniqueID(datastr, p => {
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
            api_CptyIssuer.Issuer.GetDDLIdentifyType(datastr, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        // Modal : Rating
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

    }
}