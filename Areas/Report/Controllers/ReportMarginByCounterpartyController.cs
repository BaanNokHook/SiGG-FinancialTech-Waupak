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
    public class ReportMarginByCounterpartyController : BaseController
    {
        private readonly CounterPartyEntities api_counterparty = new CounterPartyEntities();
        private readonly RPTransEntity api_deal = new RPTransEntity();
        private readonly SecurityEntities api_security = new SecurityEntities();
        private readonly StaticEntities api_static = new StaticEntities();
        private UserAndScreenEntities api_userandscreen = new UserAndScreenEntities();
        private readonly ReportEntities apiReport = new ReportEntities();
        public string Report_Bank = "SiGG Financial Bank";
        public string Report_Code = string.Empty;
        public string Report_Counter_Party = string.Empty;
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
                Report_Header = "Margin By Counter Party Report : ธุรกรรม " +
                                (reportCriteriaModel.repo_deal_type == null
                                    ? "Bilateral Repo & Private Repo"
                                    : reportCriteriaModel.repo_deal_type_name);
                Report_File_Name = reportname_list[0].Text;
                if (string.IsNullOrEmpty(reportCriteriaModel.call_date_from_string))
                {
                    ViewBag.ErrorMessage = "Please select Call Date From!!!!";
                    return View();
                }

                if (reportCriteriaModel.repo_deal_type != "BRP" &&
                    string.IsNullOrEmpty(reportCriteriaModel.counterparty_code))
                {
                    ViewBag.ErrorMessage = "Please select CounterParty!!!!";
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

                var counterPartyName = string.Empty;
                api_counterparty.CounterParty.GetCounterPartyByID(reportCriteriaModel.counterparty_code, p =>
                {
                    if (p.Data != null && p.Data.CounterPartyResultModel != null &&
                        p.Data.CounterPartyResultModel.Count > 0)
                        counterPartyName = p.Data.CounterPartyResultModel[0].counter_party_name;
                });

                string counterPartyFund = reportCriteriaModel.counterparty_fund_name;
                if (string.IsNullOrEmpty(reportCriteriaModel.counterparty_fund_id))
                {
                    counterPartyFund = "All";
                }

                if (collection["PDF"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);
                    var rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),
                        "MarginByCounterpartyReport.rpt"));
                    rd.SetDataSource(dt);
                    rd.SetParameterValue("asofdate", Report_DateFromTo);
                    rd.SetParameterValue("report_id", reportid);
                    rd.SetParameterValue("counterparty_name", reportCriteriaModel.counterparty_code_name);
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
                    var sheet = workbook.CreateSheet("MarginByCounterpartyReport");

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
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, "Counter Party : " + counterPartyName);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, "Counter Party Fund : " + counterPartyFund);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, $"System : Repo (Run date and time {DateTime.Now:dd/MM/yyyy HH:mm})");
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);

                    int col = 0;
                    // Add Header Table
                    excelTemplate.CreateCellColHead(excelRow, col++, "Call Date");
                    if (string.IsNullOrEmpty(reportCriteriaModel.counterparty_fund_id))
                        excelTemplate.CreateCellColHead(excelRow, col++, "Counter Party Fund");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Threshold");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Minimum Transfer");
                    excelTemplate.CreateCellColHead(excelRow, col++, "CCY");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Exposure");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Position Yesterday");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Position Yesterday");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Net Exposure");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Margin Call Rec= +,Pay= -");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Margin Balance");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Int. Margin");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Interest Per/Day");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Accru Int.");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Margin Activity");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Margin Activity");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Margin Activity");
                    excelTemplate.CreateCellColHead(excelRow, col, "Remark");

                    rowIndex++;
                    excelRow = sheet.CreateRow(rowIndex);

                    col = 0;
                    excelTemplate.CreateCellColHead(excelRow, col++, "Call Date");
                    if (string.IsNullOrEmpty(reportCriteriaModel.counterparty_fund_id))
                        excelTemplate.CreateCellColHead(excelRow, col++, "Counter Party Fund");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Threshold");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Minimum Transfer");
                    excelTemplate.CreateCellColHead(excelRow, col++, "CCY");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Exposure");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Margin");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Accrue Int.");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Net Exposure");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Margin Call Rec= +,Pay= -");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Margin Balance");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Int. Margin");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Interest Per/Day");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Accru Int.");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Interest Paid");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Tax 1%");
                    excelTemplate.CreateCellColHead(excelRow, col++, "Margin Payment");
                    excelTemplate.CreateCellColHead(excelRow, col, "Remark");

                    // Add Data Rows
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);
                        var columnIndex = 0;
                        var call_date = "";
                        if (dt.Rows[i]["call_date"].ToString() != string.Empty)
                            call_date = DateTime.Parse(dt.Rows[i]["call_date"].ToString()).ToString("dd/MM/yyyy");
                        excelTemplate.CreateCellColCenter(excelRow, columnIndex++, call_date);

                        if (string.IsNullOrEmpty(reportCriteriaModel.counterparty_fund_id))
                            excelTemplate.CreateCellColLeft(excelRow, columnIndex++, dt.Rows[i]["fund_engname"].ToString());

                        double threshold = 0;
                        if (dt.Rows[i]["threshold"].ToString() != string.Empty)
                            threshold = double.Parse(dt.Rows[i]["threshold"].ToString());
                        excelTemplate.CreateCellCol1Decimal(excelRow, columnIndex++, threshold);

                        double minimum_transfer = 0;
                        if (dt.Rows[i]["minimum_transfer"].ToString() != string.Empty)
                            minimum_transfer = double.Parse(dt.Rows[i]["minimum_transfer"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, minimum_transfer);


                        excelTemplate.CreateCellColCenter(excelRow, columnIndex++, dt.Rows[i]["cur"].ToString());

                        double today_exposure = 0;
                        if (dt.Rows[i]["today_exposure"].ToString() != string.Empty)
                            today_exposure = double.Parse(dt.Rows[i]["today_exposure"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, today_exposure);

                        double margin_position_yesterday = 0;
                        if (dt.Rows[i]["prev_position_margin"].ToString() != string.Empty)
                            margin_position_yesterday = double.Parse(dt.Rows[i]["prev_position_margin"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, margin_position_yesterday);

                        double accrue_int_yesterday = 0;
                        if (dt.Rows[i]["accrue_int_yesterday"].ToString() != string.Empty)
                            accrue_int_yesterday = double.Parse(dt.Rows[i]["accrue_int_yesterday"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, accrue_int_yesterday);

                        double net_exposure = 0;
                        if (dt.Rows[i]["net_exposure"].ToString() != string.Empty)
                            net_exposure = double.Parse(dt.Rows[i]["net_exposure"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, net_exposure);

                        double margin_call = 0;
                        if (dt.Rows[i]["margin_call"].ToString() != string.Empty)
                            margin_call = double.Parse(dt.Rows[i]["margin_call"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, margin_call);

                        double marginbalance = 0;
                        if (dt.Rows[i]["marginbalance"].ToString() != string.Empty)
                            marginbalance = double.Parse(dt.Rows[i]["marginbalance"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, marginbalance);

                        double int_margin = 0;
                        if (dt.Rows[i]["int_margin"].ToString() != string.Empty)
                            int_margin = double.Parse(dt.Rows[i]["int_margin"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, int_margin);

                        double interest_perday = 0;
                        if (dt.Rows[i]["interest_perday"].ToString() != string.Empty)
                            interest_perday = double.Parse(dt.Rows[i]["interest_perday"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, interest_perday);

                        double accrue_interest = 0;
                        if (dt.Rows[i]["accrue_interest"].ToString() != string.Empty)
                            accrue_interest = double.Parse(dt.Rows[i]["accrue_interest"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, accrue_interest);

                        double combine_int_amt = 0;
                        if (dt.Rows[i]["combine_int_amt"].ToString() != string.Empty)
                            combine_int_amt = double.Parse(dt.Rows[i]["combine_int_amt"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, combine_int_amt);

                        double int_rec_tax = 0;
                        if (dt.Rows[i]["int_rec_tax"].ToString() != string.Empty)
                            int_rec_tax = double.Parse(dt.Rows[i]["int_rec_tax"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, int_rec_tax);

                        double MarginPayment = 0;
                        if (dt.Rows[i]["MarginPayment"].ToString() != string.Empty)
                            MarginPayment = double.Parse(dt.Rows[i]["MarginPayment"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, MarginPayment);

                        excelTemplate.CreateCellColLeft(excelRow, columnIndex++, dt.Rows[i]["remark"].ToString());
                    }

                    rowIndex++;

                    for (var i = 0; i <= col; i++)
                    {
                        sheet.AutoSizeColumn(i);

                        if (i == 0)
                        {
                            sheet.SetColumnWidth(i, 3000);
                        }
                        else
                        {
                            var colWidth = sheet.GetColumnWidth(i);
                            if (colWidth < 2000)
                                sheet.SetColumnWidth(i, 2000);
                            else
                                sheet.SetColumnWidth(i, colWidth + 200);
                        }
                    }

                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 6, 0, 10));

                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 0, 0));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 1, 1));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 2, 2));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 3, 3));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 4, 4));

                    if (string.IsNullOrEmpty(reportCriteriaModel.counterparty_fund_id))
                    {
                        sheet.AddMergedRegion(new CellRangeAddress(7, 8, 5, 5));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 7, 6, 7));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 8, 8, 8));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 8, 9, 9));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 8, 10, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 8, 11, 11));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 8, 12, 12));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 8, 13, 13));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 7, 14, 16));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 8, 17, 17));
                        sheet.AddMergedRegion(new CellRangeAddress(8, 8, 14, 14));
                        sheet.AddMergedRegion(new CellRangeAddress(8, 8, 15, 15));
                        sheet.AddMergedRegion(new CellRangeAddress(8, 8, 16, 16));
                    }
                    else
                    {
                        sheet.AddMergedRegion(new CellRangeAddress(7, 7, 5, 6));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 8, 7, 7));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 8, 8, 8));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 8, 9, 9));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 8, 10, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 8, 11, 11));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 8, 12, 12));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 7, 13, 15));
                        sheet.AddMergedRegion(new CellRangeAddress(8, 8, 13, 13));
                        sheet.AddMergedRegion(new CellRangeAddress(8, 8, 14, 14));
                        sheet.AddMergedRegion(new CellRangeAddress(8, 8, 15, 15));
                        sheet.AddMergedRegion(new CellRangeAddress(7, 8, 16, 16));
                    }

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
            apiReport.ReportData.MarginByCounterparty(data, p =>
            {
                if (p.Success) dt = p.Data.MarginByCounterpartyReportResultModel.ToDataTable();
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