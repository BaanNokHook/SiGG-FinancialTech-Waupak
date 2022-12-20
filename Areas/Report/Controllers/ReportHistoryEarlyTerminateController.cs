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
    public class ReportHistoryEarlyTerminateController : BaseController
    {
        private readonly CounterPartyEntities api_counterparty = new CounterPartyEntities();
        private readonly RPTransEntity api_deal = new RPTransEntity();
        private readonly SecurityEntities api_security = new SecurityEntities();
        private readonly StaticEntities api_static = new StaticEntities();
        private UserAndScreenEntities api_userandscreen = new UserAndScreenEntities();
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
            ViewBag.AlertStatus = "";
            ViewBag.AlertMsg = "";
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
                string report_search_creteria = "";
                var controller_name = ControllerContext.RouteData.Values["controller"].ToString();
                reportname_list = reportentity.Getreportname(controller_name);
                if (reportname_list.Count == 0)
                {
                    ViewBag.ErrorMessage = "Can not Get Data Report ID from Service Static Method Config/getReportID and key = " +
                        controller_name + " in table gm_report !!!!";
                    return View();
                }

                reportid = reportname_list[0].Value.ToString();
                Report_Header = "Early Terminate Report : ธุรกรรม Bilateral Repo";
                Report_Header += $" (Report No.{reportid})";
                Report_File_Name = reportname_list[0].Text;

                reportCriteriaModel.terminate_date_from = string.IsNullOrEmpty(reportCriteriaModel.terminate_date_from_string)
                 ? reportCriteriaModel.terminate_date_from
                 : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.terminate_date_from_string);
                reportCriteriaModel.terminate_date_to = string.IsNullOrEmpty(reportCriteriaModel.terminate_date_to_string)
                    ? reportCriteriaModel.terminate_date_to
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.terminate_date_to_string);

                reportCriteriaModel.trade_date_from = string.IsNullOrEmpty(reportCriteriaModel.trade_date_from_string)
                    ? reportCriteriaModel.trade_date_from
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.trade_date_from_string);
                reportCriteriaModel.trade_date_to = string.IsNullOrEmpty(reportCriteriaModel.trade_date_to_string)
                    ? reportCriteriaModel.trade_date_to
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.trade_date_to_string);
                reportCriteriaModel.settlement_date_from =
                    string.IsNullOrEmpty(reportCriteriaModel.settlement_date_from_string)
                        ? reportCriteriaModel.settlement_date_from
                        : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel
                            .settlement_date_from_string);
                reportCriteriaModel.settlement_date_to =
                    string.IsNullOrEmpty(reportCriteriaModel.settlement_date_to_string)
                        ? reportCriteriaModel.settlement_date_to
                        : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.settlement_date_to_string);
                reportCriteriaModel.maturity_date_from =
                    string.IsNullOrEmpty(reportCriteriaModel.maturity_date_from_string)
                        ? reportCriteriaModel.maturity_date_from
                        : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.maturity_date_from_string);
                reportCriteriaModel.maturity_date_to = string.IsNullOrEmpty(reportCriteriaModel.maturity_date_to_string)
                    ? reportCriteriaModel.maturity_date_to
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.maturity_date_to_string);

                Report_DateFromTo =
                   (reportCriteriaModel.terminate_date_from != null || reportCriteriaModel.terminate_date_to != null
                       ? "Terminate Date "
                       : "") +
                   (reportCriteriaModel.terminate_date_from == null
                       ? ""
                       : reportCriteriaModel.terminate_date_from.Value.ToString("dd/MM/yyyy")) +
                   (reportCriteriaModel.terminate_date_from != null && reportCriteriaModel.terminate_date_to != null
                       ? " - "
                       : "") + (reportCriteriaModel.terminate_date_to == null
                       ? ""
                       : reportCriteriaModel.terminate_date_to.Value.ToString("dd/MM/yyyy"));

                Report_DateFromTo +=
                    (reportCriteriaModel.trade_date_from != null || reportCriteriaModel.trade_date_to != null
                        ? "Trade Date "
                        : "") +
                    (reportCriteriaModel.trade_date_from == null
                        ? ""
                        : reportCriteriaModel.trade_date_from.Value.ToString("dd/MM/yyyy")) +
                    (reportCriteriaModel.trade_date_from != null && reportCriteriaModel.trade_date_to != null
                        ? " - "
                        : "") + (reportCriteriaModel.trade_date_to == null
                        ? ""
                        : reportCriteriaModel.trade_date_to.Value.ToString("dd/MM/yyyy"));
                Report_DateFromTo +=
                    (reportCriteriaModel.settlement_date_from != null || reportCriteriaModel.settlement_date_to != null
                        ? " Settlement Date "
                        : "") +
                    (reportCriteriaModel.settlement_date_from == null
                        ? ""
                        : reportCriteriaModel.settlement_date_from.Value.ToString("dd/MM/yyyy")) +
                    (reportCriteriaModel.settlement_date_from != null && reportCriteriaModel.settlement_date_to != null
                        ? " - "
                        : "") + (reportCriteriaModel.settlement_date_to == null
                        ? ""
                        : reportCriteriaModel.settlement_date_to.Value.ToString("dd/MM/yyyy"));
                Report_DateFromTo +=
                    (reportCriteriaModel.maturity_date_from != null || reportCriteriaModel.maturity_date_to != null
                        ? " Maturity Date "
                        : "") +
                    (reportCriteriaModel.maturity_date_from == null
                        ? ""
                        : reportCriteriaModel.maturity_date_from.Value.ToString("dd/MM/yyyy")) +
                    (reportCriteriaModel.maturity_date_from != null && reportCriteriaModel.maturity_date_to != null
                        ? " - "
                        : "") + (reportCriteriaModel.maturity_date_to == null
                        ? ""
                        : reportCriteriaModel.maturity_date_to.Value.ToString("dd/MM/yyyy"));
                Report_Port = string.IsNullOrEmpty(reportCriteriaModel.port)
                    ? "Port : ALL"
                    : "Port : " + reportCriteriaModel.port_name;


                if (!string.IsNullOrEmpty(reportCriteriaModel.currency))
                {
                    report_search_creteria += $" Currency: {reportCriteriaModel.currency} ";
                }

                var businessdate = reportentity.Getbusinessdate();

                if (collection["Excel"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);

                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("EarlyTerminate");

                    var excelTemplate = new ExcelTemplate(workbook);

                    var rowIndex = 0;

                    var excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_Bank);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_Header);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_DateFromTo + report_search_creteria);
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
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_Port);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0,
                        $"System : Repo (Run date and time {DateTime.Now:dd/MM/yyyy HH:mm})");
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);

                    // Add Header Table
                    excelTemplate.CreateCellColHead(excelRow, 0, "No.");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Trade Date");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Settlement Date");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Maturity Date");

                    excelTemplate.CreateCellColHead(excelRow, 4, "Early Date");

                    excelTemplate.CreateCellColHead(excelRow, 5, "Period");

                    excelTemplate.CreateCellColHead(excelRow, 6, "BRP Term");
                    excelTemplate.CreateCellColHead(excelRow, 7, "Early Days");

                    excelTemplate.CreateCellColHead(excelRow, 8, "Inst.");
                    excelTemplate.CreateCellColHead(excelRow, 9, "Inst. Type");
                    excelTemplate.CreateCellColHead(excelRow, 10, "Tran. No.");
                    excelTemplate.CreateCellColHead(excelRow, 11, "Contract No.");
                    excelTemplate.CreateCellColHead(excelRow, 12, "Counterparty Code");
                    excelTemplate.CreateCellColHead(excelRow, 13, "Counterparty Fund");
                    excelTemplate.CreateCellColHead(excelRow, 14, "Cash Amount");
                    excelTemplate.CreateCellColHead(excelRow, 15, "Cur");
                    excelTemplate.CreateCellColHead(excelRow, 16, "Reference Rate");
                    excelTemplate.CreateCellColHead(excelRow, 17, "Spread");
                    excelTemplate.CreateCellColHead(excelRow, 18, "Fixing Date");
                    excelTemplate.CreateCellColHead(excelRow, 19, "Repo Int Rate");

                    excelTemplate.CreateCellColHead(excelRow, 20, "Interest Amount");
                    excelTemplate.CreateCellColHead(excelRow, 21, "Fee Rate (%)");
                    excelTemplate.CreateCellColHead(excelRow, 22, "Fee\nRec = +, Pay = -");

                    excelTemplate.CreateCellColHead(excelRow, 23, "Cost Of Fund");
                    excelTemplate.CreateCellColHead(excelRow, 24, "Portfolio");
                    excelTemplate.CreateCellColHead(excelRow, 25, "Collateral Details");

                    excelTemplate.CreateCellColHead(excelRow, 31, "Original Trans.");
                    excelTemplate.CreateCellColHead(excelRow, 32, "FO Apprv");
                    excelTemplate.CreateCellColHead(excelRow, 33, "Commentaries");
                    excelTemplate.CreateCellColHead(excelRow, 34, "Remark Amend/Cancel");
                    excelTemplate.CreateCellColHead(excelRow, 35, "Trans Type Name");
                    excelTemplate.CreateCellColHead(excelRow, 36, "Time");
                    excelTemplate.CreateCellColHead(excelRow, 37, "Input ID");
                    excelTemplate.CreateCellColHead(excelRow, 38, "Status");
                    excelTemplate.CreateCellColHead(excelRow, 39, "Termination Date/Time");
                    excelTemplate.CreateCellColHead(excelRow, 40, "Termination By");
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);

                    excelTemplate.CreateCellColHead(excelRow, 0, "No.");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Trade Date");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Settlement Date");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Maturity Date");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Early Date");
                    excelTemplate.CreateCellColHead(excelRow, 5, "Period");
                    excelTemplate.CreateCellColHead(excelRow, 6, "BRP Term");
                    excelTemplate.CreateCellColHead(excelRow, 7, "Early Days");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Inst.");
                    excelTemplate.CreateCellColHead(excelRow, 9, "Inst. Type");
                    excelTemplate.CreateCellColHead(excelRow, 10, "Tran. No.");
                    excelTemplate.CreateCellColHead(excelRow, 11, "Contract No.");
                    excelTemplate.CreateCellColHead(excelRow, 12, "Counterparty Code");
                    excelTemplate.CreateCellColHead(excelRow, 13, "Counterparty Fund");
                    excelTemplate.CreateCellColHead(excelRow, 14, "Cash Amount");
                    excelTemplate.CreateCellColHead(excelRow, 15, "Cur");
                    excelTemplate.CreateCellColHead(excelRow, 16, "Reference Rate");
                    excelTemplate.CreateCellColHead(excelRow, 17, "Spread");
                    excelTemplate.CreateCellColHead(excelRow, 18, "Fixing Date");
                    excelTemplate.CreateCellColHead(excelRow, 19, "Repo Int Rate");
                    excelTemplate.CreateCellColHead(excelRow, 20, "Interest Amount");
                    excelTemplate.CreateCellColHead(excelRow, 21, "Fee Rate (%)");
                    excelTemplate.CreateCellColHead(excelRow, 22, "");
                    excelTemplate.CreateCellColHead(excelRow, 23, "Cost Of Fund");
                    excelTemplate.CreateCellColHead(excelRow, 24, "Portfolio");
                    excelTemplate.CreateCellColHead(excelRow, 25, "Sec. ID");
                    excelTemplate.CreateCellColHead(excelRow, 26, "ISIN Code");
                    excelTemplate.CreateCellColHead(excelRow, 27, "Cur.");
                    excelTemplate.CreateCellColHead(excelRow, 28, "Total Par Value");
                    excelTemplate.CreateCellColHead(excelRow, 29, "Par / Unit");
                    excelTemplate.CreateCellColHead(excelRow, 30, "Port");
                    excelTemplate.CreateCellColHead(excelRow, 31, "Original Trans.");
                    excelTemplate.CreateCellColHead(excelRow, 32, "FO Apprv");
                    excelTemplate.CreateCellColHead(excelRow, 33, "Commentaries");
                    excelTemplate.CreateCellColHead(excelRow, 34, "Remark Amend/Cancel");
                    excelTemplate.CreateCellColHead(excelRow, 35, "Trans Type Name");
                    excelTemplate.CreateCellColHead(excelRow, 36, "Time");
                    excelTemplate.CreateCellColHead(excelRow, 37, "Input ID");
                    excelTemplate.CreateCellColHead(excelRow, 38, "Status");
                    excelTemplate.CreateCellColHead(excelRow, 39, "Termination Date/Time");
                    excelTemplate.CreateCellColHead(excelRow, 40, "Termination By");
                    excelRow.GetCell(22).CellStyle.WrapText = true;

                    double total_cashamount = 0;
                    double total_purchaseunit = 0;

                    string tempTransNo = string.Empty;
                    int no = 0;

                    // Add Data Rows
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);
                        string transNo = dt.Rows[i]["trans_no"].ToString();

                        if (tempTransNo != transNo)
                        {
                            no++;
                            tempTransNo = transNo;
                            total_cashamount += double.Parse(dt.Rows[i]["cash_amount"].ToString());

                            excelTemplate.CreateCellColRight(excelRow, 0, (no).ToString());

                            double cash_amount = 0;
                            if (dt.Rows[i]["cash_amount"].ToString() != string.Empty)
                                cash_amount = double.Parse(dt.Rows[i]["cash_amount"].ToString());

                            excelTemplate.CreateCellColDecimalBucket(excelRow, 14, cash_amount, 2);

                        }

                        // set row number //
                        excelTemplate.CreateCellColCenter(excelRow, 1, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["trade_date"].ToString()));
                        excelTemplate.CreateCellColCenter(excelRow, 2, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["settlement_date"].ToString()));
                        excelTemplate.CreateCellColCenter(excelRow, 3, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["maturity_date"].ToString()));
                        excelTemplate.CreateCellColCenter(excelRow, 4, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["early_date"].ToString()));

                        double period = 0;
                        if (dt.Rows[i]["period"].ToString() != string.Empty)
                            period = double.Parse(dt.Rows[i]["period"].ToString());
                        excelTemplate.CreateCellColNumber(excelRow, 5, period);

                        double BRP_term = 0;
                        if (dt.Rows[i]["BRP_term"].ToString() != string.Empty)
                            BRP_term = double.Parse(dt.Rows[i]["BRP_term"].ToString());
                        excelTemplate.CreateCellColNumber(excelRow, 6, BRP_term);

                        double early_days = 0;
                        if (dt.Rows[i]["early_days"].ToString() != string.Empty)
                            early_days = double.Parse(dt.Rows[i]["early_days"].ToString());
                        excelTemplate.CreateCellColNumber(excelRow, 7, early_days);

                        excelTemplate.CreateCellColCenter(excelRow, 8, dt.Rows[i]["instrument_code"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 9, dt.Rows[i]["instrument_type"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 10, transNo);
                        excelTemplate.CreateCellColLeft(excelRow, 11, dt.Rows[i]["contract_no"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 12, dt.Rows[i]["counterparty_code"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 13, dt.Rows[i]["counterparty_fund"].ToString());


                        excelTemplate.CreateCellColCenter(excelRow, 15, dt.Rows[i]["cur"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 16, dt.Rows[i]["reference_rate"].ToString());


                        double spread = 0;
                        if (dt.Rows[i]["spread"].ToString() != string.Empty)
                            spread = double.Parse(dt.Rows[i]["spread"].ToString());

                        if (dt.Rows[i]["reference_rate"].ToString() == "FIXED")
                        {
                            excelTemplate.CreateCellColLeft(excelRow, 17, "");
                        }
                        else
                        {
                            excelTemplate.CreateCellColDecimalBucket(excelRow, 17, spread, 6);
                        }

                        excelTemplate.CreateCellColCenter(excelRow, 18, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["fixing_date"].ToString()));

                        double repo_interest_rate = 0;
                        if (dt.Rows[i]["repo_interest_rate"].ToString() != string.Empty)
                            repo_interest_rate = double.Parse(dt.Rows[i]["repo_interest_rate"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 19, repo_interest_rate, 6);

                        double interest_amount = 0;
                        if (dt.Rows[i]["interest_amount"].ToString() != string.Empty)
                            interest_amount = double.Parse(dt.Rows[i]["interest_amount"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 20, interest_amount, 2);

                        double fee_Rate = 0;
                        if (dt.Rows[i]["fee_Rate"].ToString() != string.Empty)
                            fee_Rate = double.Parse(dt.Rows[i]["fee_Rate"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 21, fee_Rate, 6);

                        double fee = 0;
                        if (dt.Rows[i]["fee"].ToString() != string.Empty)
                            fee = double.Parse(dt.Rows[i]["fee"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 22, fee, 2);

                        double cost_of_fund = 0;
                        if (dt.Rows[i]["cost_of_fund"].ToString() != string.Empty)
                            cost_of_fund = double.Parse(dt.Rows[i]["cost_of_fund"].ToString());

                        excelTemplate.CreateCellColDecimalBucket(excelRow, 23, cost_of_fund, 6);
                        excelTemplate.CreateCellColCenter(excelRow, 24, dt.Rows[i]["port"].ToString());

                        excelTemplate.CreateCellColLeft(excelRow, 25, dt.Rows[i]["sec_id_col"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 26, dt.Rows[i]["isin_code_col"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 27, dt.Rows[i]["cur_col"].ToString());

                        double purchase_unit_col = 0;
                        if (dt.Rows[i]["purchase_unit_col"].ToString() != string.Empty)
                            purchase_unit_col = double.Parse(dt.Rows[i]["purchase_unit_col"].ToString());

                        double par_unit_col = 0;
                        if (dt.Rows[i]["par_unit_col"].ToString() != string.Empty)
                            par_unit_col = double.Parse(dt.Rows[i]["par_unit_col"].ToString());

                        var totalPar = purchase_unit_col * par_unit_col;
                        total_purchaseunit += totalPar;

                        excelTemplate.CreateCellColDecimalBucket(excelRow, 28, totalPar, 2);
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 29, par_unit_col, 2);
                        excelTemplate.CreateCellColCenter(excelRow, 30, dt.Rows[i]["port_col"].ToString());

                        excelTemplate.CreateCellColLeft(excelRow, 31, dt.Rows[i]["original_trans"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 32, dt.Rows[i]["fo_approve"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 33, dt.Rows[i]["commentaries"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 34, dt.Rows[i]["remark_cancel"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 35, dt.Rows[i]["trans_type_name"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 36, dt.Rows[i]["time"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 37, dt.Rows[i]["input_id"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 38, dt.Rows[i]["status"].ToString());
                        if (dt.Rows[i]["terminate_date"] != null && dt.Rows[i]["terminate_date"].ToString() != "")
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 39, utility.ConvertStringToDatetimeFormatDDMMYYYY_HHMM(dt.Rows[i]["terminate_date"].ToString()).ToString("dd/MM/yyyy HH:mm"));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 39, "");
                        }

                        excelTemplate.CreateCellColCenter(excelRow, 40, dt.Rows[i]["terminate_by"].ToString());
                    }

                    rowIndex++;

                    //footer
                    if (!string.IsNullOrEmpty(reportCriteriaModel.currency))
                    {
                        excelRow = sheet.CreateRow(rowIndex);

                        for (var i = 0; i <= 40; i++) excelTemplate.CreateCellFooter(excelRow, i, "");

                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 14, total_cashamount, 2);
                        excelTemplate.CreateCellFooterNumber(excelRow, 27, total_purchaseunit);
                    }

                    for (var i = 1; i <= 40; i++)
                    {
                        sheet.AutoSizeColumn(i);

                        var colWidth = sheet.GetColumnWidth(i);
                        if (colWidth < 2000)
                            sheet.SetColumnWidth(i, 2000);
                        else
                            sheet.SetColumnWidth(i, colWidth + 200);
                    }

                    // Set Merge Cells

                    #region For Owner Report Header
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 15));
                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 15));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 0, 7));

                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 0, 0));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 1, 1));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 2, 2));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 3, 3));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 4, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 5, 5));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 6, 6));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 7, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 8, 8));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 9, 9));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 10, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 11, 11));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 12, 12));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 13, 13));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 14, 14));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 15, 15));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 16, 16));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 17, 17));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 18, 18));

                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 19, 19));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 20, 20));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 21, 21));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 22, 22));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 23, 23));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 24, 24));

                    sheet.AddMergedRegion(new CellRangeAddress(6, 6, 25, 30));

                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 25, 25));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 26, 26));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 27, 27));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 28, 28));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 29, 29));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 30, 30));

                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 31, 31));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 32, 32));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 33, 33));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 34, 34));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 35, 35));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 36, 36));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 37, 37));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 38, 38));

                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 39, 39));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 40, 40));

                    #endregion


                    #region For Not Owner Header Report
                    //sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 7));
                    //sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 7));
                    //sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 15));
                    //sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 7));
                    //sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 7));

                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 0, 0));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 1, 1));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 2, 2));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 3, 3));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 4, 4));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 5, 5));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 6, 6));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 7, 7));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 8, 8));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 9, 9));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 10, 10));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 11, 11));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 12, 12));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 13, 13));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 14, 14));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 15, 15));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 16, 16));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 17, 17));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 18, 18));

                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 19, 19));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 20, 20));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 21, 21));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 22, 22));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 23, 23));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 24, 24));

                    //sheet.AddMergedRegion(new CellRangeAddress(5, 5, 25, 30));

                    //sheet.AddMergedRegion(new CellRangeAddress(6, 6, 25, 25));
                    //sheet.AddMergedRegion(new CellRangeAddress(6, 6, 26, 26));
                    //sheet.AddMergedRegion(new CellRangeAddress(6, 6, 27, 27));
                    //sheet.AddMergedRegion(new CellRangeAddress(6, 6, 28, 28));
                    //sheet.AddMergedRegion(new CellRangeAddress(6, 6, 29, 29));
                    //sheet.AddMergedRegion(new CellRangeAddress(6, 6, 30, 30));

                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 31, 31));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 32, 32));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 33, 33));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 34, 34));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 35, 35));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 36, 36));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 37, 37));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 38, 38));

                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 39, 39));
                    //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 40, 40));
                    #endregion

                    var exportfile = new MemoryStream();
                    workbook.Write(exportfile);
                    Response.Clear();
                    Response.ClearContent();
                    Response.ClearHeaders();

                    Response.Headers.Add("Content-Type", "application/vnd.ms-excel");
                    Response.Headers.Add("Cache-Control", "must-revalidate, post-check=0, pre-check=0");
                    Response.Headers.Add("Cache-Control", "max-age=30");
                    Response.Headers.Add("Pragma", "public");
                    Response.Headers.Add("Content-disposition", "attachment; filename=" + Report_File_Name + ".xls");

                    Response.BinaryWrite(exportfile.GetBuffer());
                    Response.End();
                    ViewBag.AlertStatus = "";
                    ViewBag.AlertMsg = "";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }

            ViewBag.AlertStatus = "";
            ViewBag.AlertMsg = "";
            return View();
        }

        public DataTable GetReportData(ReportCriteriaModel data)
        {
            var dt = new DataTable();
            apiReport.ReportData.HistoryEarlyTerminateReport(data, p =>
            {
                if (p.Success)
                {
                    dt = p.Data.HistoryEarlyTerminateReportResultModel.ToDataTable();
                }
                else
                {
                    throw new Exception(p.Message);
                }
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

        public ActionResult GetBusinessDate(string datastr)
        {
            var res = new ResultJsonModel();
            try
            {
                DateTime? Date = new DateTime();
                Date = reportentity.Getbusinessdate();
                if (Date != null) res.Data = Date.Value.ToString("MM/dd/yyyy");
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                res.Error = true;
                res.Error_detail = ex.ToString();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }
    }
}