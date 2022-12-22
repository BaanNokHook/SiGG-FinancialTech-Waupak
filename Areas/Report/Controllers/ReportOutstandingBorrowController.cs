using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GM.Application.Web.Areas.Report.Models;
using GM.Application.Web.Controllers;
using GM.CommonLibs;
using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.Report;
using GM.Filters;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Mvc;

namespace GM.Application.Web.Areas.Report.Controllers
{
    [Authorize]
    [Audit]
    public class ReportOutstandingBorrowController : BaseController
    {
        private readonly StaticEntities api_static = new StaticEntities();
        private readonly ReportEntities apiReport = new ReportEntities();
        public string Report_Banak = "SiGG Financial Bank";
        public string Report_Code = string.Empty;
        public string Report_DateFromTo = string.Empty;
        public string Report_Header = string.Empty;

        public string Report_Name = string.Empty;
        private readonly ReportEntitiesModel reportentity = new ReportEntitiesModel();
        private readonly Utility utility = new Utility();

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(ReportCriteriaModel reportCriteriaModel, FormCollection collection)
        {
            try
            {
                var dt = new DataTable();
                var reportname_list = new List<DDLItemModel>();
                string reportid;
                var controller_name =
                    ControllerContext.RouteData.Values["controller"]
                        .ToString(); //Request.QueryString["controllername"];
                reportname_list = reportentity.Getreportname(controller_name);
                if (reportname_list.Count == 0)
                {
                    ViewBag.ErrorMessage =
                        "Can not Get Data Report ID from Service Static Method Config/getReportID and key = " +
                        controller_name + " in table gm_report !!!!";
                    return View();
                }

                reportid = reportname_list[0].Value.ToString();
                Report_Header = "Outstanding Borrow Bond For Pledge Repo Report";
                Report_Name = "Outstanding Borrow Report";
                if (string.IsNullOrEmpty(reportCriteriaModel.asofdate_from_string))
                {
                    ViewBag.ErrorMessage = "Please select As Of Date From";
                    return View();
                }

                reportCriteriaModel.asofdate_from = string.IsNullOrEmpty(reportCriteriaModel.asofdate_from_string)
                    ? reportCriteriaModel.asofdate_from
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.asofdate_from_string);
                reportCriteriaModel.asofdate_to = string.IsNullOrEmpty(reportCriteriaModel.asofdate_to_string)
                    ? reportCriteriaModel.asofdate_to
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.asofdate_to_string);

                Report_DateFromTo =
                    (reportCriteriaModel.asofdate_from != null || reportCriteriaModel.asofdate_to != null
                        ? "As Of  "
                        : "") +
                    (reportCriteriaModel.asofdate_from == null
                        ? ""
                        : reportCriteriaModel.asofdate_from.Value.ToString("dd/MM/yyyy")) +
                    (reportCriteriaModel.asofdate_from != null && reportCriteriaModel.asofdate_to != null
                        ? " - "
                        : "") + (reportCriteriaModel.asofdate_to == null
                        ? ""
                        : reportCriteriaModel.asofdate_to.Value.ToString("dd/MM/yyyy"));

                var businessdate = reportentity.Getbusinessdate();

                if (collection["PDF"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);
                    var rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),
                        "OutstandingBorrowReport.rpt"));
                    rd.SetDataSource(dt);
                    rd.SetParameterValue("asofdate", DateTime.Now);
                    rd.SetParameterValue("report_id", reportid);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    var stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return new FileStreamResult(stream, "application/pdf");
                    //return File(stream, "application/pdf", "SettlementProductControlReport.pdf");
                }

                if (collection["Excel"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);

                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("OutstandingBorrowReport");

                    var excelTemplate = new ExcelTemplate(workbook);

                    // Add Header 
                    var rowIndex = 0;
                    var excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_Banak);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_Header);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_DateFromTo);
                    rowIndex++;


                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, "System : Repo");
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, "Report No." + reportid);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);

                    // Add Header Table
                    excelTemplate.CreateCellColHead(excelRow, 0, "Bond Code");
                    excelTemplate.CreateCellColHead(excelRow, 1, "ISIN");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Par/Unit");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Borrow FITS");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Pledge");
                    excelTemplate.CreateCellColHead(excelRow, 5, "Non Pledge");
                    excelTemplate.CreateCellColHead(excelRow, 6, "Period");
                    excelTemplate.CreateCellColHead(excelRow, 7, "Port");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Type");

                    // Add Data Rows
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);

                        excelTemplate.CreateCellColCenter(excelRow, 0, dt.Rows[i]["counterparty_code"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 1, dt.Rows[i]["isin_code"].ToString());

                        double par_unit = 0;
                        if (dt.Rows[i]["par_unit"].ToString() != string.Empty)
                            par_unit = double.Parse(dt.Rows[i]["par_unit"].ToString());
                        excelTemplate.CreateCellCol2Decimal(excelRow, 2, par_unit);
                        excelTemplate.CreateCellColCenter(excelRow, 3, dt.Rows[i]["borrow_fits"].ToString());

                        double pledge = 0;
                        if (dt.Rows[i]["pledge"].ToString() != string.Empty)
                            pledge = double.Parse(dt.Rows[i]["pledge"].ToString());
                        excelTemplate.CreateCellCol2Decimal(excelRow, 4, pledge);

                        double non_pledge = 0;
                        if (dt.Rows[i]["non_pledge"].ToString() != string.Empty)
                            non_pledge = double.Parse(dt.Rows[i]["non_pledge"].ToString());
                        excelTemplate.CreateCellCol2Decimal(excelRow, 5, non_pledge);

                        double period = 0;
                        if (dt.Rows[i]["period"].ToString() != string.Empty)
                            period = double.Parse(dt.Rows[i]["period"].ToString());
                        excelTemplate.CreateCellColNumber(excelRow, 6, period);

                        excelTemplate.CreateCellColCenter(excelRow, 7, dt.Rows[i]["port"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 8, dt.Rows[i]["type"].ToString());
                    }


                    for (var i = 1; i <= 8; i++)
                    {
                        sheet.AutoSizeColumn(i);

                        var colWidth = sheet.GetColumnWidth(i);
                        if (colWidth < 2000)
                            sheet.SetColumnWidth(i, 2000);
                        else
                            sheet.SetColumnWidth(i, colWidth + 200);
                    }

                    // Set Merge Cells Header Report_Banak
                    var startRow = 0;
                    var endRow = 0;
                    var startColumn = 0;
                    var endColumn = 10;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header Report_Name
                    startRow = 1;
                    endRow = 1;
                    startColumn = 0;
                    endColumn = 10;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header Report_DateFromTo
                    startRow = 2;
                    endRow = 2;
                    startColumn = 0;
                    endColumn = 10;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : Report Date
                    startRow = 3;
                    endRow = 3;
                    startColumn = 0;
                    endColumn = 10;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : Report System
                    startRow = 4;
                    endRow = 4;
                    startColumn = 0;
                    endColumn = 10;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : Report Code
                    startRow = 5;
                    endRow = 5;
                    startColumn = 0;
                    endColumn = 0;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 1;
                    endColumn = 1;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 2;
                    endColumn = 2;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 3;
                    endColumn = 3;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 4;
                    endColumn = 4;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 5;
                    endColumn = 5;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 6;
                    endColumn = 6;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 7;
                    endColumn = 7;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 8;
                    endColumn = 8;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    var exportfile = new MemoryStream();
                    workbook.Write(exportfile);
                    Response.Clear();
                    Response.ClearContent();
                    Response.ClearHeaders();

                    Response.AppendHeader("Content-Type", "application/vnd.ms-excel");
                    Response.AppendHeader("Cache-Control", "must-revalidate, post-check=0, pre-check=0");
                    Response.AppendHeader("Cache-Control", "max-age=30");
                    Response.AppendHeader("Pragma", "public");
                    Response.AppendHeader("Content-disposition", "attachment; filename=" + Report_Name + ".xls");

                    Response.BinaryWrite(exportfile.GetBuffer());
                    System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();

                    return View();
                }

                return View();
            }
            catch (Exception ex)
            {
                return View(ex.ToString());
            }
        }

        public DataTable GetReportData(ReportCriteriaModel data)
        {
            var dt = new DataTable();
            //SqlParameter outRefCode = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //SqlParameter outMessage = new SqlParameter("@Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };
            //SqlParameter outServerity = new SqlParameter("@Serverity", SqlDbType.VarChar, 15) { Direction = ParameterDirection.Output };
            //SqlParameter outHowManyRecord = new SqlParameter("@HowManyRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

            //dt = (new DbHelper()).GetTableRow("RP_Report_Outstanding_Borrow_Proc", new[] { outRefCode, outMessage, outServerity, outHowManyRecord,
            //        new SqlParameter("@as_of_date_from", data.asofdate_from),
            //        new SqlParameter("@as_of_date_to", data.asofdate_to),
            //        new SqlParameter("@currency",  data.currency)
            //});

            apiReport.ReportData.OutstandingBorrowReport(data, p =>
            {
                if (p.Success) dt = p.Data.OutstandingBorrowReportResultModel.ToDataTable();
            });

            return dt;
        }

        public ActionResult FillCurrency(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_static.Currency.GetDDLCurrency(datastr, p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}