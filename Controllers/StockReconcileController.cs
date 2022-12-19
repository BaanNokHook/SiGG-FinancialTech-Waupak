using GM.CommonLibs;
using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.Report;
using GM.Data.Model.StockReconcile;
using GM.Data.Result.StockReconcile;
using GM.Filters;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using GM.Application.Web.Areas.Report.Models;
using NPOI.SS.Util;
using GM.Data.Model.Static;

namespace GM.Application.Web.Controllers
{
    public class StockReconcileController : Controller
    {
        private static string Controller = "StockReconcileController";
        private static LogFile Log = new LogFile();
        private readonly SecurityEntities apiSecurity = new SecurityEntities();
        private readonly RPTransEntity api = new RPTransEntity();
        private readonly Utility utility = new Utility();
        private readonly ReportEntitiesModel reportentity = new ReportEntitiesModel();

        // GET: StockReconcile
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FillInstrumentCode(string datastr)
        {
            var res = new List<DDLItemModel>();
            apiSecurity.Security.GetDDLInstrumentCode(datastr, p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            ResultWithModel<StockReconcileResult> result = new ResultWithModel<StockReconcileResult>();
            StockReconcileModel stockModel = new StockReconcileModel();

            try
            {
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

                stockModel.OrdersBy = orders;

                var columns = model.columns.Where(o => o.search.value != null).ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "as_of_date":
                            stockModel.as_of_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "instrument_id":
                            if (column.search.value != "")
                            {
                                stockModel.instrument_id = column.search.value == "" ? 0 : System.Convert.ToInt32(column.search.value);
                            }
                            else
                            {
                                stockModel.instrument_id = null;
                            }

                            break;
                    }
                });

                api.StockReconcile.GetStockReconcileList(stockModel, p =>
                {
                    result = p;
                });

            }
            catch (Exception ex)
            {
                Log.WriteLog(Controller, "Error : " + ex.Message);
                result.Message = ex.Message;
            }
            return Json(new
            {
                draw = model.draw,
                recordsTotal = result.HowManyRecord,
                recordsFiltered = result.HowManyRecord,
                Message = result.Message,
                data = result.Data != null ? result.Data.StockReconcileListResultModel : new List<StockReconcileModel>()
            });
        }

        public ActionResult GetStockReconcile(StockReconcileModel model)
        {
            ResultWithModel<StockReconcileResult> res = new ResultWithModel<StockReconcileResult>();
            api.StockReconcile.GetStockReconcileList(model, p =>
            {
                res = p;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Save(StockReconcileModel model)
        {

            model.recorded_by = HttpContext.User.Identity.Name;
            ResultWithModel<StockReconcileResult> res = new ResultWithModel<StockReconcileResult>();
            api.StockReconcile.SaveStockReconcile(model, p =>
            {
                res = p;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        #region :: Gen Excel ::
        [HttpPost]
        public ActionResult GenExcel(ReportCriteriaModel model)
        {
            try
            {
                ReportEntitiesModel reportentity = new ReportEntitiesModel();

                var dt = new DataTable();
                var reportname_list = new List<DDLItemModel>();
                string reportid;
                var controller_name = ControllerContext.RouteData.Values["controller"].ToString();
                reportname_list = reportentity.Getreportname(controller_name);

                if (reportname_list.Count == 0)
                {
                    return Content("Can not Get Data Report ID from Service Static Method Config/getReportID and key = " +
                                   controller_name + " in table gm_report !!!!", "text/html");
                }

                reportid = reportname_list[0].Value.ToString();
                string Report_File_Name = reportname_list[0].Text;
                string Report_Header = "Stock Reconcile Report";
                Report_Header += $" (Report No.{reportid})";

                model.asofdate = utility.ConvertStringToDatetimeFormatDDMMYYYY(model.asofdate_string);

                dt = GetReportData(model);

                string Report_DateFromTo = (!string.IsNullOrEmpty(model.asofdate_string))
                    ? "As Of Date " + model.asofdate_string
                    : string.Empty;

                var workbook = new HSSFWorkbook();
                var sheet = workbook.CreateSheet("StockReconcile");

                var excelTemplate = new ExcelTemplate(workbook);

                // Add Header 
                var rowIndex = 0;

                var excelRow = sheet.CreateRow(rowIndex);
                excelTemplate.CreateCellHeaderLeft(excelRow, 0, "ธนาคารกรุงไทย(KRUNGTHAI BANK)");
                rowIndex++;

                excelRow = sheet.CreateRow(rowIndex);
                excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_Header);
                rowIndex++;

                excelRow = sheet.CreateRow(rowIndex);
                excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_DateFromTo);
                rowIndex++;

                string ownerEndUer = string.Empty;
                List<ConfigModel> listConfig = reportentity.GetReportHeader(reportid);
                if (listConfig != null && listConfig.Count > 0)
                {
                    ownerEndUer = listConfig[0].item_value;
                }

                excelRow = sheet.CreateRow(rowIndex);
                excelTemplate.CreateCellHeaderLeft(excelRow, 0, ownerEndUer);
                rowIndex++;

                excelRow = sheet.CreateRow(rowIndex);
                excelTemplate.CreateCellHeaderLeft(excelRow, 0, $"System : Repo (Run date and time {DateTime.Now:dd/MM/yyyy HH:mm})");
                rowIndex++;

                excelRow = sheet.CreateRow(rowIndex);

                // Add Header Table
                excelTemplate.CreateCellColHead(excelRow, 0, "Sec Code");
                excelTemplate.CreateCellColHead(excelRow, 1, "AFS");
                excelTemplate.CreateCellColHead(excelRow, 2, "HTM");
                excelTemplate.CreateCellColHead(excelRow, 3, "MEMO-BNK");
                excelTemplate.CreateCellColHead(excelRow, 4, "MEMO-TRD");
                excelTemplate.CreateCellColHead(excelRow, 5, "Total");
                excelTemplate.CreateCellColHead(excelRow, 6, "OutStanding PTI");
                excelTemplate.CreateCellColHead(excelRow, 7, "Obligate Unit");
                excelTemplate.CreateCellColHead(excelRow, 8, "Difference");
                excelTemplate.CreateCellColHead(excelRow, 9, "Remark");

                int rowStart = rowIndex + 2;
                int rowEnd = rowStart;
                // Add Data Rows
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    rowIndex++;
                    excelRow = sheet.CreateRow(rowIndex);

                    excelTemplate.CreateCellColLeft(excelRow, 0, dt.Rows[i]["instrument_code"].ToString());

                    if (dt.Columns.Contains("afs_unit") && dt.Rows[i]["afs_unit"].ToString() != string.Empty)
                    {
                        double afs_unit = double.Parse(dt.Rows[i]["afs_unit"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 1, afs_unit, 2);
                    }
                    else
                    {
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 1, 0.00);
                    }

                    if (dt.Columns.Contains("htm_unit") && dt.Rows[i]["htm_unit"].ToString() != string.Empty)
                    {
                        double htm_unit = double.Parse(dt.Rows[i]["htm_unit"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 2, htm_unit, 2);
                    }
                    else
                    {
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 2, 0.00);
                    }

                    if (dt.Columns.Contains("memo_bnk_unit") && dt.Rows[i]["memo_bnk_unit"].ToString() != string.Empty)
                    {
                        double memo_bnk_unit = double.Parse(dt.Rows[i]["memo_bnk_unit"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 3, memo_bnk_unit, 2);
                    }
                    else
                    {
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 3, 0.00);
                    }

                    if (dt.Columns.Contains("memo_trd_unit") && dt.Rows[i]["memo_trd_unit"].ToString() != string.Empty)
                    {
                        double memo_trd_unit = double.Parse(dt.Rows[i]["memo_trd_unit"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 4, memo_trd_unit, 2);
                    }
                    else
                    {
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 4, 0.00);
                    }

                    if (dt.Columns.Contains("total_unit") && dt.Rows[i]["total_unit"].ToString() != string.Empty)
                    {
                        double total_unit = double.Parse(dt.Rows[i]["total_unit"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 5, total_unit, 2);
                    }
                    else
                    {
                        excelTemplate.CreateCellColCenter(excelRow, 5, 0.00);
                    }

                    if (dt.Columns.Contains("outstanding_unit") && dt.Rows[i]["outstanding_unit"].ToString() != string.Empty)
                    {
                        double outstanding_unit = double.Parse(dt.Rows[i]["outstanding_unit"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 6, outstanding_unit, 2);
                    }
                    else
                    {
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 6, 0.00);
                    }

                    if (dt.Columns.Contains("obligate_unit") && dt.Rows[i]["obligate_unit"].ToString() != string.Empty)
                    {
                        double outstanding_unit = double.Parse(dt.Rows[i]["obligate_unit"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 7, outstanding_unit, 2);
                    }
                    else
                    {
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 7, 0.00);
                    }

                    if (dt.Columns.Contains("diff_unit") && dt.Rows[i]["diff_unit"].ToString() != string.Empty)
                    {
                        double diff_unit = double.Parse(dt.Rows[i]["diff_unit"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 8, diff_unit, 2);
                    }
                    else
                    {
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 8, 0.00);
                    }

                    excelTemplate.CreateCellColLeft(excelRow, 9, dt.Rows[i]["remark"].ToString());
                }

                rowIndex++;
                //footer
                if (dt.Rows.Count > 0)
                {
                    rowEnd = rowIndex;
                    excelRow = sheet.CreateRow(rowIndex);

                    excelTemplate.CreateCellFooter(excelRow, 0, "Total");
                    excelTemplate.CreateCellFooterDecimalBucket(excelRow, 1, 0, 2);
                    excelTemplate.CreateCellFooterDecimalBucket(excelRow, 2, 0, 2);
                    excelTemplate.CreateCellFooterDecimalBucket(excelRow, 3, 0, 2);
                    excelTemplate.CreateCellFooterDecimalBucket(excelRow, 4, 0, 2);
                    excelTemplate.CreateCellFooterDecimalBucket(excelRow, 5, 0, 2);
                    excelTemplate.CreateCellFooterDecimalBucket(excelRow, 6, 0, 2);
                    excelTemplate.CreateCellFooterDecimalBucket(excelRow, 7, 0, 2);
                    excelTemplate.CreateCellFooterDecimalBucket(excelRow, 8, 0, 2);
                    excelTemplate.CreateCellFooterCenter(excelRow, 9, "");

                    var fn = "";
                    fn = string.Format("SUM(B{0}:B{1})", rowStart, rowEnd);
                    excelRow.GetCell(1).SetCellFormula(fn);

                    fn = string.Format("SUM(C{0}:C{1})", rowStart, rowEnd);
                    excelRow.GetCell(2).SetCellFormula(fn);

                    fn = string.Format("SUM(D{0}:D{1})", rowStart, rowEnd);
                    excelRow.GetCell(3).SetCellFormula(fn);

                    fn = string.Format("SUM(E{0}:E{1})", rowStart, rowEnd);
                    excelRow.GetCell(4).SetCellFormula(fn);

                    fn = string.Format("SUM(F{0}:F{1})", rowStart, rowEnd);
                    excelRow.GetCell(5).SetCellFormula(fn);

                    fn = string.Format("SUM(G{0}:G{1})", rowStart, rowEnd);
                    excelRow.GetCell(6).SetCellFormula(fn);

                    fn = string.Format("SUM(H{0}:H{1})", rowStart, rowEnd);
                    excelRow.GetCell(7).SetCellFormula(fn);

                    fn = string.Format("SUM(I{0}:I{1})", rowStart, rowEnd);
                    excelRow.GetCell(8).SetCellFormula(fn);
                }

                for (var i = 0; i <= 9; i++)
                {
                    sheet.AutoSizeColumn(i);

                    var colWidth = sheet.GetColumnWidth(i);
                    if (colWidth < 2000)
                        sheet.SetColumnWidth(i, 3000);
                    else
                        sheet.SetColumnWidth(i, colWidth + 500);
                }

                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 3));
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 3));
                sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 3));
                sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 3));
                sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 3));

                var exportfile = new MemoryStream();
                workbook.Write(exportfile);
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();

                Response.AppendHeader("Content-Type", "application/vnd.ms-excel");
                Response.AppendHeader("Cache-Control", "must-revalidate, post-check=0, pre-check=0");
                Response.AppendHeader("Cache-Control", "max-age=30");
                Response.AppendHeader("Pragma", "public");
                Response.AppendHeader("Content-disposition", $"attachment; filename={Report_File_Name}_{DateTime.Now.ToString("yyyyMMdd_HHmm")}.xls");

                Response.BinaryWrite(exportfile.GetBuffer());
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
                return View("Index");
            }
            catch (Exception ex)
            {
                return View(ex.ToString());
            }
        }

        public DataTable GetReportData(ReportCriteriaModel data)
        {
            var dt = new DataTable();
            ReportEntities apiReport = new ReportEntities();
            apiReport.ReportData.StockReconcileReport(data, p =>
            {
                if (p.Success) dt = p.Data.StockReconcileReportResultModel.ToDataTable();
            });

            return dt;
        }
        #endregion
    }
}