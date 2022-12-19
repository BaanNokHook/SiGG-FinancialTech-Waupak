using GM.CommonLibs;
using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Result.ExternalInterface;
using GM.Data.View.ExternalInterface;
using GM.Filters;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]

    public class ThorRateController : BaseController
    {
        ExternalInterfaceEntities api = new ExternalInterfaceEntities();
        Utility utility = new Utility();
        private static LogFile Log = new LogFile();
        private static string controller = "ThorRate";
        // Action Index
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            ThorRateViewModel model = new ThorRateViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            ResultWithModel<ThorRateResult> Result = new ResultWithModel<ThorRateResult>();
            ThorRateModel rateModel = new ThorRateModel();

            //Add Paging
            PagingModel paging = new PagingModel();
            paging.PageNumber = model.pageno;
            paging.RecordPerPage = model.length;
            rateModel.paging = paging;

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

            rateModel.ordersby = orders;

            var columns = model.columns.Where(o => o.search.value != null).ToList();
            columns.ForEach(column =>
            {
                switch (column.data)
                {
                    case "asof_date_from":
                        rateModel.asof_date_from = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                        break;
                    case "asof_date_to":
                        rateModel.asof_date_to = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                        break;
                    case "ccy":
                        rateModel.ccy = column.search.value;
                        break;
                }
            });

            api.InterfaceThorRate.GetThorRate(rateModel, p =>
            {
                Result = p;
            });

            return Json(new
            {
                draw = model.draw,
                recordsTotal = Result.HowManyRecord,
                recordsFiltered = Result.HowManyRecord,
                data = Result.Data != null ? Result.Data.ThorRateResultModel : new List<ThorRateModel>()
            });
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Create(ThorRateViewModel model)
        {
            var Result = new ResultWithModel<ThorRateResult>();
            try
            {
                model.FormAction.create_by = HttpContext.User.Identity.Name;
                if (ModelState.IsValid)
                {
                    api.InterfaceThorRate.CreateThorRate(model.FormAction, p =>
                    {
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
        public ActionResult Edit(Data data)
        {
            ThorRateModel model = new ThorRateModel();
            ResultWithModel<ThorRateResult> result = new ResultWithModel<ThorRateResult>();
            PagingModel paging = new PagingModel();
            paging.PageNumber = 0;
            paging.RecordPerPage = 0;
            model.paging = paging;
            model.asof_date_from = utility.ConvertStringToDatetimeFormatDDMMYYYY(data.asof_date);
            model.asof_date_to = utility.ConvertStringToDatetimeFormatDDMMYYYY(data.asof_date);
            model.curve_id = data.curve_id;
            model.ccy = data.ccy;
            model.index_type = data.index_type;

            api.InterfaceThorRate.GetThorRate(model, p =>
            {
                result = p;
            });
            //return View(model);
            return Json((result.Data.ThorRateResultModel.Count > 0 ? result.Data.ThorRateResultModel[0] : new ThorRateModel()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Edit(ThorRateViewModel model)
        {
            var Result = new ResultWithModel<ThorRateResult>();
            try
            {
                if (ModelState.IsValid)
                {
                    model.FormAction.update_by = HttpContext.User.Identity.Name;
                    api.InterfaceThorRate.UpdateThorRate(model.FormAction, p =>
                    {
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
            public string asof_date { get; set; }
            public string curve_id { get; set; }
            public string ccy { get; set; }
            public string index_type { get; set; }
        }

        [HttpPost]
        [RoleScreen(RoleScreen.DELETE)]
        public JsonResult Delete(Data data)
        {
            var res = new ResultWithModel<ThorRateResult>();
            ThorRateModel model = new ThorRateModel();
            try
            {
                model.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(data.asof_date);
                model.curve_id = data.curve_id;
                model.ccy = data.ccy;
                model.index_type = data.index_type;
                model.update_by = HttpContext.User.Identity.Name;

                api.InterfaceThorRate.DeleteThorRate(model, p =>
                {
                    res = p;
                });

                if (res.Success == false)
                {
                    return Json(new { success = false, responseText = res.Message, refcode = res.RefCode }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, responseText = "Your message successfuly sent!", refcode = res.RefCode }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.ToString(), refcode = res.RefCode }, JsonRequestBehavior.AllowGet);
            }
        }

        // // Function : Binding DDL        

        public ActionResult FillCur(string cur)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api.InterfaceThorRate.GetDDLCur(cur, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult ExportData(ThorRateModel model)
        {
            string strMsg = string.Empty;
            string filename = "";
            try
            {
                // Export Excel XLS
                DataTable dt = new DataTable();
                ExcelEntity ExcelEnt = new ExcelEntity();
                model.mode = "EXPORT";
                dt = GetReportData(model);
                if (dt == null)
                {
                    throw new Exception("No Data.");
                }

                ExcelEnt.FileName = "ThorRate.xls";
                ExcelEnt.SheetName = "ThorRate";
                filename = "ThorRate.xls";

                //save the file to server temp folder
                string importexportpath = ConfigurationManager.AppSettings["ImportExportPath"].ToString();
                string fullPath = Path.Combine(Server.MapPath(importexportpath), ExcelEnt.FileName);

                HSSFWorkbook workbook = new HSSFWorkbook();
                var sheet = workbook.CreateSheet(ExcelEnt.SheetName);

                var excelTemplate = new ExcelTemplate(workbook);

                // Add Header 
                var rowIndex = 0;
                var excelRow = sheet.CreateRow(rowIndex);

                int colIndex = 0;
                foreach (var item in dt.Columns.Cast<DataColumn>())
                {
                    excelTemplate.CreateCellColHead(excelRow, colIndex, item.ColumnName.ToUpper());
                    colIndex++;
                }

                int row = 0;
                foreach (var rows in dt.Rows)
                {
                    rowIndex++;
                    excelRow = sheet.CreateRow(rowIndex);

                    for (int i = 0; i < colIndex; i++)
                    {
                        excelTemplate.CreateCellColLeft(excelRow, i, dt.Rows[row][i].ToString());
                    }
                    row++;
                }

                for (var i = 0; i <= colIndex; i++)
                {
                    sheet.AutoSizeColumn(i);

                    var colWidth = sheet.GetColumnWidth(i);
                    if (colWidth < 2000)
                        sheet.SetColumnWidth(i, 2000);
                    else
                        sheet.SetColumnWidth(i, colWidth + 200);
                }

                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);

                FileStream FileData = new FileStream(fullPath, FileMode.Create);
                workbook.Write(FileData);
                FileData.Close();
            }
            catch (Exception ex)
            {
                strMsg = ex.Message;
                Log.WriteLog(controller, "Error : " + ex.Message);
            }
            return Json(new { fileName = filename, errorMessage = strMsg }, JsonRequestBehavior.AllowGet);
        }

        public DataTable GetReportData(ThorRateModel model)
        {
            ResultWithModel<ThorRateResult> result = new ResultWithModel<ThorRateResult>();

            //Add Paging
            PagingModel paging = new PagingModel();
            model.paging = paging;

            var dt = new DataTable();

            api.InterfaceThorRate.GetThorRateDataTable(model, p =>
            {
                if (p.Success)
                {
                    if (p.Data.Tables.Count > 0 && p.Data.Tables[0].Rows.Count > 0)
                    {
                        dt = p.Data.Tables[0];
                    }
                    else
                    {
                        dt = null;
                    }
                }
            });
            return dt;
        }

        [HttpGet]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Download(string filename)
        {
            //get the temp folder and file path in server
            string importexportpath = ConfigurationManager.AppSettings["ImportExportPath"].ToString();
            string fullPath = Path.Combine(Server.MapPath(importexportpath), filename);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/vnd.ms-excel", filename);
        }
    }
}