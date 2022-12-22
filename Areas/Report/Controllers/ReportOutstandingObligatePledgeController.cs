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
using GM.Data.Model.Static;
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
    public class ReportOutstandingObligatePledgeController : BaseController
    {
        private RPTransEntity api_deal = new RPTransEntity();
        private readonly SecurityEntities api_security = new SecurityEntities();
        private readonly StaticEntities api_static = new StaticEntities();
        private UserAndScreenEntities api_userandscreen = new UserAndScreenEntities();
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
                var controller_name = ControllerContext.RouteData.Values["controller"].ToString();
                reportname_list = reportentity.Getreportname(controller_name);
                if (reportname_list.Count == 0)
                {
                    ViewBag.ErrorMessage =
                        "Can not Get Data Report ID from Service Static Method Config/getReportID and key = " +
                        controller_name + " in table gm_report !!!!";
                    return View();
                }

                reportid = reportname_list[0].Value.ToString();
                Report_Header = "Outstanding Obligate Pledge Report";
                Report_Header += $" (Report No.{reportid})";
                Report_Name = "Outstanding Obligate Pledge Report";
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
                reportCriteriaModel.start_date = string.IsNullOrEmpty(reportCriteriaModel.start_date_string)
                    ? reportCriteriaModel.start_date
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.start_date_string);
                reportCriteriaModel.expire_date = string.IsNullOrEmpty(reportCriteriaModel.expire_date_string)
                    ? reportCriteriaModel.expire_date
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.expire_date_string);

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
                        "OutstandingObligateReport.rpt"));
                    rd.SetDataSource(dt);
                    rd.SetParameterValue("asofdate", Report_DateFromTo);
                    rd.SetParameterValue("report_id", reportid);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    var stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return new FileStreamResult(stream, "application/pdf");
                }

                if (collection["Excel"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);

                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("OutstandingObligateReport");
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

                    //string ownerEndUer = string.Empty;
                    //List<ConfigModel> listConfig = reportentity.GetReportHeader(reportid);
                    //if (listConfig != null && listConfig.Count > 0)
                    //{
                    //    ownerEndUer = listConfig[0].item_value;
                    //}

                    //excelRow = sheet.CreateRow(rowIndex);
                    //excelTemplate.CreateCellHeaderLeft(excelRow, 0, ownerEndUer);
                    //rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, $"System : Repo (Run date and time {DateTime.Now:dd/MM/yyyy HH:mm})");
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);

                    // Add Header Table
                    excelTemplate.CreateCellColHead(excelRow, 0, "As of Date");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Repo Type");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Source");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Port");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Instrument Code");
                    excelTemplate.CreateCellColHead(excelRow, 5, "ISIN Code");
                    excelTemplate.CreateCellColHead(excelRow, 6, "Cur");
                    excelTemplate.CreateCellColHead(excelRow, 7, "Par/Unit");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Obligate");
                    excelTemplate.CreateCellColHead(excelRow, 9, "Pledge");
                    excelTemplate.CreateCellColHead(excelRow, 10, "Non Pledge");
                    excelTemplate.CreateCellColHead(excelRow, 11, "Period/Day");
                    excelTemplate.CreateCellColHead(excelRow, 12, "Start Date");
                    excelTemplate.CreateCellColHead(excelRow, 13, "End Date");

                    // Add Data Rows
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);

                        if (dt.Rows[i]["asof_date"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 0, DateTime.Parse(dt.Rows[i]["asof_date"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 0, new DateTime());
                        }

                        excelTemplate.CreateCellColCenter(excelRow, 1, dt.Rows[i]["repo_type"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 2, dt.Rows[i]["source"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 3, dt.Rows[i]["port"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 4, dt.Rows[i]["instrument_code"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 5, dt.Rows[i]["isin_code"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 6, dt.Rows[i]["cur"].ToString());

                        double par = 0;
                        if (dt.Rows[i]["par"].ToString() != string.Empty)
                            par = double.Parse(dt.Rows[i]["par"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 7, par);

                        double obligate = 0;
                        if (dt.Rows[i]["obligate"].ToString() != string.Empty)
                            obligate = double.Parse(dt.Rows[i]["obligate"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 8, obligate);

                        double pledge = 0;
                        if (dt.Rows[i]["pledge"].ToString() != string.Empty)
                            pledge = double.Parse(dt.Rows[i]["pledge"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 9, pledge);

                        double nonpledge = 0;
                        if (dt.Rows[i]["nonpledge"].ToString() != string.Empty)
                            nonpledge = double.Parse(dt.Rows[i]["nonpledge"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 10, nonpledge);

                        double period = 0;
                        if (dt.Rows[i]["period"].ToString() != string.Empty)
                            period = double.Parse(dt.Rows[i]["period"].ToString());
                        excelTemplate.CreateCellColNumber(excelRow, 11, period);

                        if (dt.Rows[i]["start_obligate_date"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 12, DateTime.Parse(dt.Rows[i]["start_obligate_date"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 12, new DateTime());
                        }

                        if (dt.Rows[i]["expire_obligate_date"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 13, DateTime.Parse(dt.Rows[i]["expire_obligate_date"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 13, new DateTime());
                        }
                    }

                    for (var i = 1; i <= 13; i++)
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

                    // Set Merge Cells Header : Report Code
                    startRow = 4;
                    endRow = 4;
                    startColumn = 0;
                    endColumn = 0;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 4;
                    endRow = 4;
                    startColumn = 1;
                    endColumn = 1;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 4;
                    endRow = 4;
                    startColumn = 2;
                    endColumn = 2;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 4;
                    endRow = 4;
                    startColumn = 3;
                    endColumn = 3;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 4;
                    endRow = 4;
                    startColumn = 4;
                    endColumn = 4;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 4;
                    endRow = 4;
                    startColumn = 5;
                    endColumn = 5;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 4;
                    endRow = 4;
                    startColumn = 6;
                    endColumn = 6;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 4;
                    endRow = 4;
                    startColumn = 7;
                    endColumn = 7;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 4;
                    endRow = 4;
                    startColumn = 8;
                    endColumn = 8;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 4;
                    endRow = 4;
                    startColumn = 9;
                    endColumn = 9;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 4;
                    endRow = 4;
                    startColumn = 10;
                    endColumn = 10;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 4;
                    endRow = 4;
                    startColumn = 11;
                    endColumn = 11;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 4;
                    endRow = 4;
                    startColumn = 12;
                    endColumn = 12;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 4;
                    endRow = 4;
                    startColumn = 13;
                    endColumn = 13;
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
            apiReport.ReportData.OutstandingObligatePledgeReport(data, p =>
            {
                if (p.Success) dt = p.Data.OutstandingObligatePledgeReportResultModel.ToDataTable();
            });

            return dt;
        }

        public ActionResult FillObligateType(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_static.Config.GetObligateType(datastr, p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
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

        public ActionResult FillInstrumentCode(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_security.Security.GetDDLInstrumentCode(datastr, p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillPort(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_static.Config.GetPortForReportDDL(datastr, p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}