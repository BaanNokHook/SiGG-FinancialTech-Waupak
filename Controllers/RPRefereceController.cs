using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.MarketRate;
using GM.Data.Result.MarketRate;
using GM.Data.View.MarketRate;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using GM.CommonLibs;
using NPOI.HSSF.UserModel;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class RPRefereceController : BaseController
    {
        MarketRateEntities api_MarketRate = new MarketRateEntities();
        Utility utility = new Utility();

        // Action : Index
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            var RPRefereceView = new RPRefereceViewModel();
            RPRefereceModel RPReferece = new RPRefereceModel();

            RPRefereceView.FormSearch = RPReferece;
            return View(RPRefereceView);
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            ResultWithModel<RPRefereceResult> result = new ResultWithModel<RPRefereceResult>();
            RPRefereceModel RPRefereceModel = new RPRefereceModel();
            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                RPRefereceModel.paging = paging;

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

                RPRefereceModel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null).ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "price_source":
                            RPRefereceModel.price_source = column.search.value;
                            break;
                        case "instrument_code":
                            RPRefereceModel.instrument_code = column.search.value;
                            break;
                        case "asof_date":
                            RPRefereceModel.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                    }
                });

                api_MarketRate.RPReferece.GetRPRefereceList(RPRefereceModel, p =>
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
                data = result.Data != null ? result.Data.RPRefereceResultModel:new List<RPRefereceModel>()
            });
        }

        public ActionResult Add(string price_source, string asof_date, string instrument_id, string marketdate_t)
        {
            if (!string.IsNullOrEmpty(instrument_id))
            {
                ResultWithModel<RPRefereceResult> result = new ResultWithModel<RPRefereceResult>();
                RPRefereceModel RPReferece = new RPRefereceModel();

                try
                {
                    //Add Paging
                    PagingModel paging = new PagingModel();
                    RPReferece.paging = paging;
                    //Add Orderby
                    var orders = new List<OrderByModel>();
                    RPReferece.ordersby = orders;
                    //Add id
                    RPReferece.price_source = price_source;
                    RPReferece.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(asof_date);
                    RPReferece.instrument_id = instrument_id;
                    RPReferece.marketdate_t = marketdate_t;
                    RPReferece.page_name = "editpage";

                    api_MarketRate.RPReferece.GetRPRefereceList(RPReferece, p =>
                    {
                        result = p;
                    });

                    RPReferece = result.Data.RPRefereceResultModel[0];

                }
                catch (Exception)
                {

                    throw;
                }
                ViewBag.page_name = "editpage";
                //ViewBag.marketdate_t = new SelectList(Getmarketdate_t_List(), "Value", "Text");

                return View(RPReferece);
            }
            else
            {
                RPRefereceModel RPReferece = new RPRefereceModel();
                RPReferece.instrument_id = "";
                RPReferece.page_name = "addpage";

                //ViewBag.marketdate_t = new SelectList(Getmarketdate_t_List(), "Value", "Text");
                //ViewBag.marketdate_t = Getmarketdate_t_List();
                return View(RPReferece);
            }
        }

        [HttpPost]
        public ActionResult Add(RPRefereceModel RPReferece)
        {
            if (RPReferece.page_name == "addpage")
            {
                new RoleScreenAttribute(RoleScreen.CREATE);
                ResultWithModel<RPRefereceResult> rwm = new ResultWithModel<RPRefereceResult>();

                RPReferece.create_by = HttpContext.User.Identity.Name;

                if (ModelState.IsValid)
                {
                    RPReferece.processdate = RPReferece.asof_date;

                    if (RPReferece.price_source == "TBMA")
                    {
                        RPReferece.maturity_date = null;
                        RPReferece.avgbidding = null;
                        RPReferece.govtinterpolatedyield = null;
                        RPReferece.ttm = null;
                        RPReferece.spread = null;
                        RPReferece.referenceyield = null;
                        RPReferece.settlementdate = null;
                        RPReferece.settlementdate = null;
                        RPReferece.bondtype = null;
                    }

                    api_MarketRate.RPReferece.CreateRPReferece(RPReferece, p =>
                    {
                        rwm = p;
                    });

                    if (rwm.Success)
                    {
                        return RedirectToAction("Index");
                    }

                    else
                    {
                        ViewBag.Message = rwm.Message;
                        ModelState.AddModelError("Exception", rwm.Message);
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
            else
            {
                new RoleScreenAttribute(RoleScreen.EDIT);
                ResultWithModel<RPRefereceResult> res = new ResultWithModel<RPRefereceResult>();
                RPReferece.create_by = HttpContext.User.Identity.Name;

                if (ModelState.IsValid)
                {
                    RPReferece.processdate = RPReferece.asof_date;

                    api_MarketRate.RPReferece.UpdateRPReferece(RPReferece, p =>
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
            }

            //ViewBag.marketdate_t = new SelectList(Getmarketdate_t_List(), "Value", "Text", RPReferece.marketdate_t);
            return View(RPReferece);
        }

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Edit(string price_source, string asof_date, string instrument_id, string marketdate_t)
        {
            return RedirectToAction("Add", new { price_source = price_source, asof_date = asof_date, instrument_id = instrument_id, marketdate_t = marketdate_t });
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult GenExcel(string asof_date, string instrument_code)
        {
            try
            {
                ResultWithModel<RPRefereceResult> result = new ResultWithModel<RPRefereceResult>();
                RPRefereceModel model = new RPRefereceModel();
                model.price_source = "TBMA";
                model.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(asof_date);
                model.instrument_code = instrument_code;
                model.paging = new PagingModel(){PageNumber = 1, RecordPerPage = 9999999};
                api_MarketRate.RPReferece.GetRPRefereceList(model, p =>
                {
                    result = p;
                });

                if (result.Success == false)
                {
                    throw new Exception("ExportRPReferce() => " + result.Message);
                }

                var resList = result.Data.RPRefereceResultModel;
                
                HSSFWorkbook workbook = new HSSFWorkbook();
                var sheet = workbook.CreateSheet("MerketPrice");
                
                //DataTable dt = new DataTable();
                //dt = resList.ToDataTable();

                var excelTemplate = new ExcelTemplate(workbook);

                // Add Header
                var rowIndex = 0;

                var excelRow = sheet.CreateRow(rowIndex);
                // Add Header Table
                excelTemplate.CreateCellColHead(excelRow, 0, "Symbol");
                excelTemplate.CreateCellColHead(excelRow, 1, "AI %");
                excelTemplate.CreateCellColHead(excelRow, 2, "Gross Price %");
                excelTemplate.CreateCellColHead(excelRow, 3, "Clean Price %");
                excelTemplate.CreateCellColHead(excelRow, 4, "Modified Duration");
                excelTemplate.CreateCellColHead(excelRow, 5, "Convexity");
                excelTemplate.CreateCellColHead(excelRow, 6, "Market Price T");

                // Add Data Rows
                if (resList.Any())
                {
                    resList = resList.OrderBy(a => a.instrument_code).ThenBy(a => a.marketdate_t).ToList();
                    foreach (var item in resList)
                    {
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);
                        excelTemplate.CreateCellColCenter(excelRow, 0, item.instrument_code);

                        double ai = 0;
                        if (item.ai.HasValue)
                            ai = (double)item.ai.Value;

                        excelTemplate.CreateCellCol6Decimal(excelRow, 1, ai);

                        double grossPrice = 0;
                        if (item.gross_price.HasValue)
                            grossPrice = (double)item.gross_price.Value;

                        excelTemplate.CreateCellCol6Decimal(excelRow, 2, grossPrice);

                        double cleanPrice = 0;
                        if (item.clean_price.HasValue)
                            cleanPrice = (double)item.clean_price.Value;

                        excelTemplate.CreateCellCol6Decimal(excelRow, 3, cleanPrice);

                        double modifiedDuration = 0;
                        if (item.modifiedduration.HasValue)
                            modifiedDuration = (double)item.modifiedduration.Value;

                        excelTemplate.CreateCellCol6Decimal(excelRow, 4, modifiedDuration);

                        double convexity = 0;
                        if (item.convexity.HasValue)
                            convexity = (double)item.convexity.Value;

                        excelTemplate.CreateCellCol6Decimal(excelRow, 5, convexity);

                        excelTemplate.CreateCellColNumber(excelRow, 6, item.marketdate_t);
                    }
                }

                for (int i = 0; i <= 6; i++)
                {
                    sheet.AutoSizeColumn(i);

                    var colWidth = sheet.GetColumnWidth(i);
                    if (colWidth < 2000)
                        sheet.SetColumnWidth(i, 2000);
                    else
                        sheet.SetColumnWidth(i, colWidth + 200);
                }

                var exportfile = new MemoryStream();
                workbook.Write(exportfile);
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();

                Response.Headers.Add("Content-Type", "application/vnd.ms-excel");
                Response.Headers.Add("Cache-Control", "must-revalidate, post-check=0, pre-check=0");
                Response.Headers.Add("Cache-Control", "max-age=30");
                Response.Headers.Add("Pragma", "public");
                Response.Headers.Add("Content-disposition", "attachment; filename=MarketPrice_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xls");

                Response.BinaryWrite(exportfile.GetBuffer());
                Response.End();
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                throw ex;
            }
            return View("Index");

        }

        public class Data
        {
            public string price_source { get; set; }
            public string asof_date { get; set; }
            public string instrument_id { get; set; }
            public string marketdate_t { get; set; }
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult Deletes(Data data)
        {
            var rwm = new ResultWithModel<RPRefereceResult>();
            RPRefereceModel view = new RPRefereceModel();
            view.create_by = HttpContext.User.Identity.Name;

            view.price_source = data.price_source;
            view.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(data.asof_date);
            view.instrument_id = data.instrument_id;
            view.marketdate_t = data.marketdate_t;
            try
            {
                api_MarketRate.RPReferece.DeleteRPReferece(view, p =>
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

        public ActionResult FillInstrumentCode(string instrument)//, string maturitydate)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_MarketRate.RPReferece.GetDDLInstrument(instrument, p =>
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
            api_MarketRate.RPReferece.GetDDLPriceSource(p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        //public List<SelectListItem> Getmarketdate_t_List()
        //{
        //    List<SelectListItem> myList = new List<SelectListItem>();

        //    myList.Add(new SelectListItem { Value = "0", Text = "0" });
        //    myList.Add(new SelectListItem { Value = "1", Text = "1", Selected = true});
        //    myList.Add(new SelectListItem { Value = "2", Text = "2" });

        //    //var data = new[]{
        //    //     new SelectListItem{ Value="0",Text="0"},
        //    //     new SelectListItem{ Value="1",Text="1", Selected=true},
        //    //     new SelectListItem{ Value="2",Text="2"},

        //    return myList;
        //}

        
    }
}