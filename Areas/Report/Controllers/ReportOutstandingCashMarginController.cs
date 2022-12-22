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
    public class ReportOutstandingCashMarginController : BaseController
    {
        private readonly CounterPartyEntities api_counterparty = new CounterPartyEntities();
        private readonly RPTransEntity api_deal = new RPTransEntity();
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
                var type = string.IsNullOrEmpty(reportCriteriaModel.repo_deal_type_name)
                    ? "Private Repo"
                    : reportCriteriaModel.repo_deal_type_name;
                Report_Header = "Outstanding Cash Margin (" + type + ")";
                Report_File_Name = reportname_list[0].Text;
                if (string.IsNullOrEmpty(reportCriteriaModel.asofdate_from_string))
                {
                    ViewBag.ErrorMessage = "Please select As of Date From!!!!";
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
                        ? "As Of Date "
                        : "") +
                    (reportCriteriaModel.asofdate_from == null
                        ? ""
                        : reportCriteriaModel.asofdate_from.Value.ToString("dd/MM/yyyy")) +
                    (reportCriteriaModel.asofdate_from != null && reportCriteriaModel.asofdate_to != null
                        ? " - "
                        : "") + (reportCriteriaModel.asofdate_to == null
                        ? ""
                        : reportCriteriaModel.asofdate_to.Value.ToString("dd/MM/yyyy"));

                if (!string.IsNullOrEmpty(reportCriteriaModel.currency))
                {
                    Report_DateFromTo += $" Currency: {reportCriteriaModel.currency} ";
                }

                Report_Header += $" (Report No.{reportid})";

                var businessdate = reportentity.Getbusinessdate();

                if (collection["PDF"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);
                    var rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),
                        "OutstandingCashMarginReport.rpt"));
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
                    var sheet = workbook.CreateSheet("CashMarginReport");

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
                    if (reportCriteriaModel.repo_deal_type == "PRP")
                    {
                        // Add Header Table
                        excelTemplate.CreateCellColHead(excelRow, 0, "Counterparty Code");
                        excelTemplate.CreateCellColHead(excelRow, 1, "Counter Party Fund");
                        excelTemplate.CreateCellColHead(excelRow, 2, "Threshold");
                        excelTemplate.CreateCellColHead(excelRow, 3, "Minimum Transfer");
                        excelTemplate.CreateCellColHead(excelRow, 4, "CCY");
                        excelTemplate.CreateCellColHead(excelRow, 5, "Exposure");
                        excelTemplate.CreateCellColHead(excelRow, 6, "Position Yesterday");
                        excelTemplate.CreateCellColHead(excelRow, 7, "Position Yesterday");
                        excelTemplate.CreateCellColHead(excelRow, 8, "Net Exposure");
                        excelTemplate.CreateCellColHead(excelRow, 9, "Margin Call Rec= +,Pay= -");
                        excelTemplate.CreateCellColHead(excelRow, 10, "Margin Balance");
                        excelTemplate.CreateCellColHead(excelRow, 11, "Int. Margin");
                        excelTemplate.CreateCellColHead(excelRow, 12, "Accru Int.");
                        excelTemplate.CreateCellColHead(excelRow, 13, "Margin Activity");
                        excelTemplate.CreateCellColHead(excelRow, 14, "Margin Activity");
                        excelTemplate.CreateCellColHead(excelRow, 15, "Margin Activity");
                        excelTemplate.CreateCellColHead(excelRow, 16, "Remark");

                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);

                        excelTemplate.CreateCellColHead(excelRow, 0, "Counterparty Code");
                        excelTemplate.CreateCellColHead(excelRow, 1, "Counter Party Fund");
                        excelTemplate.CreateCellColHead(excelRow, 2, "Threshold");
                        excelTemplate.CreateCellColHead(excelRow, 3, "Minimum Transfer");
                        excelTemplate.CreateCellColHead(excelRow, 4, "CCY");
                        excelTemplate.CreateCellColHead(excelRow, 5, "Exposure");
                        excelTemplate.CreateCellColHead(excelRow, 6, "Margin");
                        excelTemplate.CreateCellColHead(excelRow, 7, "Accrue Int.");
                        excelTemplate.CreateCellColHead(excelRow, 8, "Net Exposure");
                        excelTemplate.CreateCellColHead(excelRow, 9, "Margin Call Rec= +,Pay= -");
                        excelTemplate.CreateCellColHead(excelRow, 10, "Margin Balance");
                        excelTemplate.CreateCellColHead(excelRow, 11, "Int. Margin");
                        excelTemplate.CreateCellColHead(excelRow, 12, "Accru Int.");
                        excelTemplate.CreateCellColHead(excelRow, 13, "Interest Paid");
                        excelTemplate.CreateCellColHead(excelRow, 14, "Tax 1%");
                        excelTemplate.CreateCellColHead(excelRow, 15, "Margin");
                        excelTemplate.CreateCellColHead(excelRow, 16, "Remark");

                        // Add Data Rows
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            rowIndex++;
                            var columnIndex = 0;
                            excelRow = sheet.CreateRow(rowIndex);

                            excelTemplate.CreateCellColCenter(excelRow, columnIndex++,
                                dt.Rows[i]["counterparty_code"].ToString());

                            excelTemplate.CreateCellColCenter(excelRow, columnIndex++, dt.Rows[i]["fund_engname"].ToString());

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

                            double exposure = 0;
                            if (dt.Rows[i]["exposure"].ToString() != string.Empty)
                                exposure = double.Parse(dt.Rows[i]["exposure"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, exposure);

                            double margin_position_yesterday = 0;
                            if (dt.Rows[i]["margin_position_yesterday"].ToString() != string.Empty)
                                margin_position_yesterday =
                                    double.Parse(dt.Rows[i]["margin_position_yesterday"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, margin_position_yesterday);

                            double accrue_int_position_yesterday = 0;
                            if (dt.Rows[i]["accrue_int_position_yesterday"].ToString() != string.Empty)
                                accrue_int_position_yesterday =
                                    double.Parse(dt.Rows[i]["accrue_int_position_yesterday"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, accrue_int_position_yesterday);

                            double net_exposure = 0;
                            if (dt.Rows[i]["net_exposure"].ToString() != string.Empty)
                                net_exposure = double.Parse(dt.Rows[i]["net_exposure"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, net_exposure);

                            double margin_call = 0;
                            if (dt.Rows[i]["margin_call"].ToString() != string.Empty)
                                margin_call = double.Parse(dt.Rows[i]["margin_call"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, margin_call);

                            double margin_balance = 0;
                            if (dt.Rows[i]["margin_balance"].ToString() != string.Empty)
                                margin_balance = double.Parse(dt.Rows[i]["margin_balance"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, margin_balance);

                            double int_margin = 0;
                            if (dt.Rows[i]["int_margin"].ToString() != string.Empty)
                                int_margin = double.Parse(dt.Rows[i]["int_margin"].ToString());
                            excelTemplate.CreateCellColDecimalBucket(excelRow, columnIndex++, int_margin, 6);

                            double accru_int = 0;
                            if (dt.Rows[i]["accru_int"].ToString() != string.Empty)
                                accru_int = double.Parse(dt.Rows[i]["accru_int"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, accru_int);

                            double interest_paid_margin_activity = 0;
                            if (dt.Rows[i]["interest_paid_margin_activity"].ToString() != string.Empty)
                                interest_paid_margin_activity =
                                    double.Parse(dt.Rows[i]["interest_paid_margin_activity"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, interest_paid_margin_activity);

                            double tax_margin_activity = 0;
                            if (dt.Rows[i]["tax_margin_activity"].ToString() != string.Empty)
                                tax_margin_activity = double.Parse(dt.Rows[i]["tax_margin_activity"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, tax_margin_activity);

                            double margin_activity = 0;
                            if (dt.Rows[i]["margin_activity"].ToString() != string.Empty)
                                margin_activity = double.Parse(dt.Rows[i]["margin_activity"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, columnIndex++, margin_activity);

                            excelTemplate.CreateCellColLeft(excelRow, columnIndex++, dt.Rows[i]["remark"].ToString());
                        }

                        if (!string.IsNullOrEmpty(reportCriteriaModel.currency) && dt.Rows.Count > 0)
                        {
                            rowIndex++;
                            excelRow = sheet.CreateRow(rowIndex);
                            excelTemplate.CreateCellFooterCenter(excelRow, 0, "Total");
                            excelTemplate.CreateCellFooter(excelRow, 1, "");
                            excelTemplate.CreateCellFooter(excelRow, 2, "");
                            excelTemplate.CreateCellFooter(excelRow, 3, "");
                            excelTemplate.CreateCellFooter(excelRow, 4, "");
                            excelTemplate.CreateCellFooterDecimalBucket(excelRow, 5, 0, 2);
                            excelTemplate.CreateCellFooterDecimalBucket(excelRow, 6, 0, 2);
                            excelTemplate.CreateCellFooterDecimalBucket(excelRow, 7, 0, 2);
                            excelTemplate.CreateCellFooter(excelRow, 8, "");
                            excelTemplate.CreateCellFooterDecimalBucket(excelRow, 9, 0, 2);
                            excelTemplate.CreateCellFooterDecimalBucket(excelRow, 10, 0, 2);
                            excelTemplate.CreateCellFooter(excelRow, 11, "");
                            excelTemplate.CreateCellFooterDecimalBucket(excelRow, 12, 0, 2);
                            excelTemplate.CreateCellFooter(excelRow, 13, "");
                            excelTemplate.CreateCellFooter(excelRow, 14, "");
                            excelTemplate.CreateCellFooter(excelRow, 15, "");
                            excelTemplate.CreateCellFooter(excelRow, 16, "");

                            var fn = "";
                            fn = string.Format("SUM(F8:F" + rowIndex + ")");
                            excelRow.GetCell(5).SetCellFormula(fn);

                            fn = string.Format("SUM(G8:G" + rowIndex + ")");
                            excelRow.GetCell(6).SetCellFormula(fn);

                            fn = string.Format("SUM(H8:H" + rowIndex + ")");
                            excelRow.GetCell(7).SetCellFormula(fn);

                            fn = string.Format("SUM(J8:J" + rowIndex + ")");
                            excelRow.GetCell(9).SetCellFormula(fn);

                            fn = string.Format("SUM(K8:K" + rowIndex + ")");
                            excelRow.GetCell(10).SetCellFormula(fn);

                            fn = string.Format("SUM(M8:M" + rowIndex + ")");
                            excelRow.GetCell(12).SetCellFormula(fn);

                            //Merge cell total
                            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 0));
                        }

                        for (var i = 0; i <= 16; i++)
                        {
                            sheet.AutoSizeColumn(i);

                            if (i == 0)
                            {
                                sheet.SetColumnWidth(i, 5000);
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


                        // Set Merge Cells Header Report_Banak
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
                        sheet.AddMergedRegion(new CellRangeAddress(5, 6, 8, 8));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 6, 9, 9));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 6, 10, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 6, 11, 11));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 6, 12, 12));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 5, 13, 15));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 6, 16, 16));

                        sheet.AddMergedRegion(new CellRangeAddress(6, 6, 6, 6));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 6, 7, 7));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 6, 13, 13));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 6, 14, 14));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 6, 15, 15));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 6, 16, 16));
                    }
                    else
                    {
                        // Add Header Table
                        excelTemplate.CreateCellColHead(excelRow, 0, "Deal No");
                        excelTemplate.CreateCellColHead(excelRow, 1, "Contract No");
                        excelTemplate.CreateCellColHead(excelRow, 2, "Inst.Type");
                        excelTemplate.CreateCellColHead(excelRow, 3, "Last Call Margin Date");
                        excelTemplate.CreateCellColHead(excelRow, 4, "Maturity Date");
                        excelTemplate.CreateCellColHead(excelRow, 5, "Period");
                        excelTemplate.CreateCellColHead(excelRow, 6, "CCY");
                        excelTemplate.CreateCellColHead(excelRow, 7, "Call Margin");
                        excelTemplate.CreateCellColHead(excelRow, 8, "Call Margin");
                        excelTemplate.CreateCellColHead(excelRow, 9, "Margin Position Yesterday");
                        excelTemplate.CreateCellColHead(excelRow, 10, "Margin Balance");
                        excelTemplate.CreateCellColHead(excelRow, 11, "Int. Rate");
                        excelTemplate.CreateCellColHead(excelRow, 12, "Interest on cash Margin");
                        excelTemplate.CreateCellColHead(excelRow, 13, "Interest on cash Margin");
                        excelTemplate.CreateCellColHead(excelRow, 14, "Interest on cash Margin");
                        excelTemplate.CreateCellColHead(excelRow, 15, "Close Margin");

                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);

                        excelTemplate.CreateCellColHead(excelRow, 0, "Deal No");
                        excelTemplate.CreateCellColHead(excelRow, 1, "Contract No");
                        excelTemplate.CreateCellColHead(excelRow, 2, "Inst.Type");
                        excelTemplate.CreateCellColHead(excelRow, 3, "Last Call Margin Date");
                        excelTemplate.CreateCellColHead(excelRow, 4, "Maturity Date");
                        excelTemplate.CreateCellColHead(excelRow, 5, "Period");
                        excelTemplate.CreateCellColHead(excelRow, 6, "CCY");
                        excelTemplate.CreateCellColHead(excelRow, 7, "Receive");
                        excelTemplate.CreateCellColHead(excelRow, 8, "Pay");
                        excelTemplate.CreateCellColHead(excelRow, 9, "Margin Position Yesterday");
                        excelTemplate.CreateCellColHead(excelRow, 10, "Margin Balance");
                        excelTemplate.CreateCellColHead(excelRow, 11, "Int. Rate");
                        excelTemplate.CreateCellColHead(excelRow, 12, "Receive");
                        excelTemplate.CreateCellColHead(excelRow, 13, "Tax(Amt.)");
                        excelTemplate.CreateCellColHead(excelRow, 14, "Paid");
                        excelTemplate.CreateCellColHead(excelRow, 15, "Close Margin");

                        // Add Data Rows
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            rowIndex++;
                            excelRow = sheet.CreateRow(rowIndex);

                            excelTemplate.CreateCellColCenter(excelRow, 0, dt.Rows[i]["dealNo"].ToString());
                            excelTemplate.CreateCellColLeft(excelRow, 1, dt.Rows[i]["ContractNo"].ToString());
                            excelTemplate.CreateCellColCenter(excelRow, 2, dt.Rows[i]["Insttype"].ToString());

                            var last_call_margin_date = "";
                            if (dt.Rows[i]["LastCall"].ToString() != string.Empty)
                                last_call_margin_date = DateTime.Parse(dt.Rows[i]["LastCall"].ToString())
                                    .ToString("dd/MM/yyyy");
                            excelTemplate.CreateCellColCenter(excelRow, 3, last_call_margin_date);

                            var maturity_date = "";
                            if (dt.Rows[i]["maturity_date"].ToString() != string.Empty)
                                maturity_date = DateTime.Parse(dt.Rows[i]["maturity_date"].ToString())
                                    .ToString("dd/MM/yyyy");
                            excelTemplate.CreateCellColCenter(excelRow, 4, maturity_date);

                            excelTemplate.CreateCellColCenter(excelRow, 5, dt.Rows[i]["Period"].ToString());
                            excelTemplate.CreateCellColCenter(excelRow, 6, dt.Rows[i]["Cur"].ToString());

                            double receive = 0;
                            if (dt.Rows[i]["last_margin_call_rec"].ToString() != string.Empty)
                                receive = double.Parse(dt.Rows[i]["last_margin_call_rec"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 7, receive);

                            double pay = 0;
                            if (dt.Rows[i]["last_margin_call_pay"].ToString() != string.Empty)
                                pay = double.Parse(dt.Rows[i]["last_margin_call_pay"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 8, pay);

                            double margin_position_yesterday = 0;
                            if (dt.Rows[i]["PositionYesterday"].ToString() != string.Empty)
                                margin_position_yesterday = double.Parse(dt.Rows[i]["PositionYesterday"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 9, margin_position_yesterday);

                            double margin_balance = 0;
                            if (dt.Rows[i]["Balance"].ToString() != string.Empty)
                                margin_balance = double.Parse(dt.Rows[i]["Balance"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 10, margin_balance);

                            double int_rate = 0;
                            if (dt.Rows[i]["IntRate"].ToString() != string.Empty)
                                int_rate = double.Parse(dt.Rows[i]["IntRate"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 11, int_rate);

                            double int_receive = 0;
                            if (dt.Rows[i]["IntRec"].ToString() != string.Empty)
                                int_receive = double.Parse(dt.Rows[i]["IntRec"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 12, int_receive);

                            double int_tax = 0;
                            if (dt.Rows[i]["IntTax"].ToString() != string.Empty)
                                int_tax = double.Parse(dt.Rows[i]["IntTax"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 13, int_tax);

                            double int_paid = 0;
                            if (dt.Rows[i]["IntPay"].ToString() != string.Empty)
                                int_paid = double.Parse(dt.Rows[i]["IntPay"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 14, int_paid);

                            double close_margin = 0;
                            if (dt.Rows[i]["CloseMargin"].ToString() != string.Empty)
                                close_margin = double.Parse(dt.Rows[i]["CloseMargin"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 15, close_margin);
                        }

                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);
                        var totalrow = rowIndex;
                        excelTemplate.CreateCellFooterCenter(excelRow, 0, "Total");
                        excelTemplate.CreateCellFooterCenter(excelRow, 1, "Total");
                        excelTemplate.CreateCellFooterCenter(excelRow, 2, "Total");
                        excelTemplate.CreateCellFooterCenter(excelRow, 3, "Total");
                        excelTemplate.CreateCellFooterCenter(excelRow, 4, "Total");
                        excelTemplate.CreateCellFooterCenter(excelRow, 5, "Total");
                        excelTemplate.CreateCellFooterCenter(excelRow, 6, "Total");


                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 7, 0, 2);
                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 8, 0, 2);
                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 9, 0, 2);
                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 10, 0, 2);
                        excelTemplate.CreateCellFooter(excelRow, 11, "");
                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 12, 0, 2);
                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 13, 0, 2);
                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 14, 0, 2);
                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 15, 0, 2);

                        if (dt.Rows.Count > 0)
                        {
                            var fn = "";
                            fn = string.Format("SUM(H8:H" + rowIndex + ")");
                            excelRow.GetCell(7).SetCellFormula(fn);

                            fn = string.Format("SUM(I8:I" + rowIndex + ")");
                            excelRow.GetCell(8).SetCellFormula(fn);

                            fn = string.Format("SUM(J8:J" + rowIndex + ")");
                            excelRow.GetCell(9).SetCellFormula(fn);

                            fn = string.Format("SUM(K8:K" + rowIndex + ")");
                            excelRow.GetCell(10).SetCellFormula(fn);

                            fn = string.Format("SUM(M8:M" + rowIndex + ")");
                            excelRow.GetCell(12).SetCellFormula(fn);

                            fn = string.Format("SUM(N8:N" + rowIndex + ")");
                            excelRow.GetCell(13).SetCellFormula(fn);

                            fn = string.Format("SUM(O8:O" + rowIndex + ")");
                            excelRow.GetCell(14).SetCellFormula(fn);

                            fn = string.Format("SUM(P8:P" + rowIndex + ")");
                            excelRow.GetCell(15).SetCellFormula(fn);
                        }

                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);
                        var row_call_margin = rowIndex;
                        excelTemplate.CreateCellFooterCenter(excelRow, 0, "Call Margin");
                        excelTemplate.CreateCellFooterCenter(excelRow, 1, "Call Margin");
                        excelTemplate.CreateCellFooterCenter(excelRow, 2, "Call Margin");
                        excelTemplate.CreateCellFooterCenter(excelRow, 3, "Call Margin");
                        excelTemplate.CreateCellFooterCenter(excelRow, 4, "Call Margin");
                        excelTemplate.CreateCellFooterCenter(excelRow, 5, "Call Margin");
                        excelTemplate.CreateCellFooterCenter(excelRow, 6, "Call Margin");

                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 7, 0, 2);
                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 8, 0, 2);
                        excelTemplate.CreateCellFooter(excelRow, 9, "");
                        excelTemplate.CreateCellFooter(excelRow, 10, "");
                        excelTemplate.CreateCellFooter(excelRow, 11, "");
                        excelTemplate.CreateCellFooter(excelRow, 12, "");
                        excelTemplate.CreateCellFooter(excelRow, 13, "");
                        excelTemplate.CreateCellFooter(excelRow, 14, "");
                        excelTemplate.CreateCellFooter(excelRow, 15, "");

                        if (dt.Rows.Count > 0)
                        {
                            var fn = "";
                            fn = string.Format("SUM(H8:H" + (rowIndex - 1) + ")");
                            excelRow.GetCell(7).SetCellFormula(fn);

                            fn = string.Format("SUM(I8:I" + (rowIndex - 1) + ")");
                            excelRow.GetCell(8).SetCellFormula(fn);
                        }

                        for (var i = 0; i <= 15; i++)
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

                        sheet.AddMergedRegion(new CellRangeAddress(row_call_margin, row_call_margin, 0, 6));
                        sheet.AddMergedRegion(new CellRangeAddress(totalrow, totalrow, 0, 6));
                        // Set Merge Cells Header Report_Banak
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
                        sheet.AddMergedRegion(new CellRangeAddress(5, 6, 6, 6));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 5, 7, 8));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 6, 7, 7));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 6, 8, 8));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 6, 9, 9));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 6, 10, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 6, 11, 11));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 5, 12, 14));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 6, 12, 12));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 6, 13, 13));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 6, 14, 14));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 6, 15, 15));
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
            try
            {
                apiReport.ReportData.OutstandingCashMarginReport(data, p =>
                {
                    if (p.Success) dt = p.Data.OutstandingCashMarginReportResultModel.ToDataTable();
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }

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

        public ActionResult FillCounterParty(string datastr, string type)
        {
            var res = new List<DDLItemModel>();
            apiReport.ReportData.DDLCounterPartyReport(datastr, type, p =>
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