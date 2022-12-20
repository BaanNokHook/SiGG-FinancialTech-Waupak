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
    public class ReportCallMarginController : BaseController
    {
        private readonly CounterPartyEntities api_counterparty = new CounterPartyEntities();
        private readonly RPTransEntity api_deal = new RPTransEntity();
        private readonly SecurityEntities api_security = new SecurityEntities();
        private readonly StaticEntities api_static = new StaticEntities();
        private readonly ReportEntities apiReport = new ReportEntities();
        public string Report_Bank = "SiGG Financial Bank";
        public string Report_Code = string.Empty;
        public string Report_DateFromTo = string.Empty;
        public string Report_File_Name = string.Empty;
        public string Report_Header = string.Empty;
        public string Report_Name = string.Empty;
        public string Report_Port = string.Empty;
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
                Report_Header = "Daily Call Margin Report";
                Report_File_Name = reportname_list[0].Text;
                if (string.IsNullOrEmpty(reportCriteriaModel.call_date_from_string))
                {
                    ViewBag.ErrorMessage = "Please select Call Date From!!!!";
                    return View();
                }

                reportCriteriaModel.call_date_from = string.IsNullOrEmpty(reportCriteriaModel.call_date_from_string)
                    ? reportCriteriaModel.call_date_from
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.call_date_from_string);
                reportCriteriaModel.call_date_to = string.IsNullOrEmpty(reportCriteriaModel.call_date_to_string)
                    ? reportCriteriaModel.call_date_to
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.call_date_to_string);

                Report_DateFromTo =
                    (reportCriteriaModel.call_date_from != null || reportCriteriaModel.call_date_to != null
                        ? "Call Date "
                        : "") +
                    (reportCriteriaModel.call_date_from == null
                        ? ""
                        : reportCriteriaModel.call_date_from.Value.ToString("dd/MM/yyyy")) +
                    (reportCriteriaModel.call_date_from != null && reportCriteriaModel.call_date_to != null
                        ? " - "
                        : "") + (reportCriteriaModel.call_date_to == null
                        ? ""
                        : reportCriteriaModel.call_date_to.Value.ToString("dd/MM/yyyy"));

                Report_Header += $" (Report No.{reportid})";

                var businessdate = reportentity.Getbusinessdate();

                if (collection["PDF"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);
                    var rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),
                        "CallMarginReport.rpt"));
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
                    var sheet = workbook.CreateSheet("CallMargin");

                    var excelTemplate = new ExcelTemplate(workbook);

                    // Add Header 
                    var rowIndex = 0;

                    var excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_Bank);
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
                    excelTemplate.CreateCellColHead(excelRow, 0, "No.");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Counter Party");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Counter Party Fund");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Threshold");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Minimum Transfer");
                    excelTemplate.CreateCellColHead(excelRow, 5, "CCY");
                    excelTemplate.CreateCellColHead(excelRow, 6, "Call Margin");
                    excelTemplate.CreateCellColHead(excelRow, 7, "Call Margin");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Remark");

                    rowIndex++;
                    excelRow = sheet.CreateRow(rowIndex);

                    excelTemplate.CreateCellColHead(excelRow, 0, "No.");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Counterparty");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Counter Party Fund");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Threshold");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Minimum Transfer");
                    excelTemplate.CreateCellColHead(excelRow, 5, "CCY");
                    excelTemplate.CreateCellColHead(excelRow, 6, "Receive (+)");
                    excelTemplate.CreateCellColHead(excelRow, 7, "Pay (-)");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Remark");

                    // Add Data Rows
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);
                        var columnIndex = 0;
                        excelTemplate.CreateCellColCenter(excelRow, columnIndex++, (i + 1).ToString());
                        excelTemplate.CreateCellColLeft(excelRow, columnIndex++,
                            dt.Rows[i]["counterparty_code"].ToString());

                        excelTemplate.CreateCellColLeft(excelRow, columnIndex++,
                           dt.Rows[i]["fund_engname"].ToString());

                        double threshold = 0;
                        if (dt.Rows[i]["threshold"].ToString() != string.Empty)
                            threshold = double.Parse(dt.Rows[i]["threshold"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, threshold);

                        //Minimun Transfer
                        double minimum_transfer = 0;
                        if (dt.Rows[i]["minimum_transfer"].ToString() != string.Empty)
                            minimum_transfer = double.Parse(dt.Rows[i]["minimum_transfer"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, minimum_transfer);

                        excelTemplate.CreateCellColCenter(excelRow, columnIndex++, dt.Rows[i]["cur"].ToString());

                        double receive = 0;
                        if (dt.Rows[i]["receive"].ToString() != string.Empty)
                            receive = double.Parse(dt.Rows[i]["receive"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, receive);

                        double pay = 0;
                        if (dt.Rows[i]["pay"].ToString() != string.Empty)
                            pay = double.Parse(dt.Rows[i]["pay"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, pay);

                        excelTemplate.CreateCellColLeft(excelRow, columnIndex++, dt.Rows[i]["remark"].ToString());
                    }

                    rowIndex++;

                    //footer
                    if (!string.IsNullOrEmpty(reportCriteriaModel.currency))
                    {
                        excelRow = sheet.CreateRow(rowIndex);

                        excelTemplate.CreateCellColHead(excelRow, 3, "Total");
                        excelTemplate.CreateCellColHead(excelRow, 4, "Total");
                        excelTemplate.CreateCellColHead(excelRow, 5, "Total");

                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 6, 0, 2);
                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 7, 0, 2);
                        if (dt.Rows.Count > 0)
                        {
                            var fn = "";
                            fn = string.Format("SUM(G8:G" + rowIndex + ")");
                            excelRow.GetCell(6).SetCellFormula(fn);

                            fn = string.Format("SUM(H8:H" + rowIndex + ")");
                            excelRow.GetCell(7).SetCellFormula(fn);
                        }
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

                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 3, 5));
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 10));

                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 0, 0));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 1, 1));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 2, 2));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 3, 3));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 4, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 5, 5));

                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 6, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 6, 6, 6));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 6, 7, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 8, 8));

                    var exportfile = new MemoryStream();
                    workbook.Write(exportfile);
                    Response.Clear();
                    Response.ClearContent();
                    Response.ClearHeaders();

                    Response.AppendHeader("Content-Type", "application/vnd.ms-excel");
                    Response.AppendHeader("Cache-Control", "must-revalidate, post-check=0, pre-check=0");
                    Response.AppendHeader("Cache-Control", "max-age=30");
                    Response.AppendHeader("Pragma", "public");
                    Response.AppendHeader("Content-disposition", "attachment; filename=" + Report_Name);

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
            apiReport.ReportData.CallMarginReport(data, p =>
            {
                if (p.Success) dt = p.Data.CallMarginReportResultModel.ToDataTable();
            });

            return dt;
        }

        public ActionResult FillRepoDealType(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_deal.RPDealEntry.GetDDLRepoDealType(p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillInstrumentType(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_deal.RPDealEntry.GetDDLInstrumentType(datastr, p =>
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

        public ActionResult FillCounterParty(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_counterparty.CounterPartyFund.GetDDLCounterParty(datastr, p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCounterPartyFund(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_counterparty.CounterPartyFund.GetDDLCounterPartyFund(datastr, p =>
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