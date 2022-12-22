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
    public class ReportRollOverTransactionController : BaseController
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
                Report_Header = "Roll Over Transaction Report : ธุรกรรม " + (reportCriteriaModel.repo_deal_type == null
                                    ? "Bilateral Repo & Private Repo"
                                    : reportCriteriaModel.repo_deal_type_name);
                Report_Header += $" (Report No.{reportid})";
                Report_File_Name = reportname_list[0].Text;
                //if (!reportentity.CheckDateTradeSettleMaturity(reportCriteriaModel))
                //{
                //    ViewBag.ErrorMessage =
                //        "Please select some date at Trade Date From / Settlement Date From / Maturity Date From !!!!";
                //    return View();
                //}

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

                if (collection["PDF"] != null)
                {

                }

                if (collection["Excel"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);

                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("RollOverTransaction");

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
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_DateFromTo.Trim() + report_search_creteria);
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
                    excelTemplate.CreateCellColHead(excelRow, 4, "Period");
                    excelTemplate.CreateCellColHead(excelRow, 5, "Inst.");
                    excelTemplate.CreateCellColHead(excelRow, 6, "Inst. Type");
                    excelTemplate.CreateCellColHead(excelRow, 7, "Tran. No.");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Contract No.");
                    excelTemplate.CreateCellColHead(excelRow, 9, "Counterparty Code");
                    excelTemplate.CreateCellColHead(excelRow, 10, "Counterparty Fund");
                    excelTemplate.CreateCellColHead(excelRow, 11, "Ref. No.");
                    excelTemplate.CreateCellColHead(excelRow, 12, "Purchase Price");
                    excelTemplate.CreateCellColHead(excelRow, 13, "Repurchase Price");
                    excelTemplate.CreateCellColHead(excelRow, 14, "Net Settle Amt. (+/ -)");
                    excelTemplate.CreateCellColHead(excelRow, 15, "Cur");
                    excelTemplate.CreateCellColHead(excelRow, 16, "Reference Rate");
                    excelTemplate.CreateCellColHead(excelRow, 17, "Spread");
                    excelTemplate.CreateCellColHead(excelRow, 18, "Fixing Date");
                    excelTemplate.CreateCellColHead(excelRow, 19, "Repo Int Rate");
                    excelTemplate.CreateCellColHead(excelRow, 20, "Cost Of Fund");
                    excelTemplate.CreateCellColHead(excelRow, 21, "Portfolio");

                    excelTemplate.CreateCellColHead(excelRow, 22, "Collateral Details");


                    excelTemplate.CreateCellColHead(excelRow, 30, "FO Apprv");
                    excelTemplate.CreateCellColHead(excelRow, 31, "Commentaries");
                    excelTemplate.CreateCellColHead(excelRow, 32, "Remark Amend/Cancel");
                    excelTemplate.CreateCellColHead(excelRow, 33, "Trans Type Name");
                    excelTemplate.CreateCellColHead(excelRow, 34, "Time");
                    excelTemplate.CreateCellColHead(excelRow, 35, "Input ID");
                    excelTemplate.CreateCellColHead(excelRow, 36, "Status");
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);

                    excelTemplate.CreateCellColHead(excelRow, 0, "No.");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Trade Date");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Settlement Date");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Maturity Date");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Period");
                    excelTemplate.CreateCellColHead(excelRow, 5, "Inst.");
                    excelTemplate.CreateCellColHead(excelRow, 6, "Inst. Type");
                    excelTemplate.CreateCellColHead(excelRow, 7, "Tran. No.");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Contract No.");
                    excelTemplate.CreateCellColHead(excelRow, 9, "Counterparty Code");
                    excelTemplate.CreateCellColHead(excelRow, 10, "Counterparty Fund");
                    excelTemplate.CreateCellColHead(excelRow, 11, "Ref. No.");
                    excelTemplate.CreateCellColHead(excelRow, 12, "Purchase Price");
                    excelTemplate.CreateCellColHead(excelRow, 13, "Repurchase Price");
                    excelTemplate.CreateCellColHead(excelRow, 14, "Net Settle Amt. (+/ -)");
                    excelTemplate.CreateCellColHead(excelRow, 15, "Cur");
                    excelTemplate.CreateCellColHead(excelRow, 16, "Reference Rate");
                    excelTemplate.CreateCellColHead(excelRow, 17, "Spread");
                    excelTemplate.CreateCellColHead(excelRow, 18, "Fixing Date");
                    excelTemplate.CreateCellColHead(excelRow, 19, "Repo Int Rate");
                    excelTemplate.CreateCellColHead(excelRow, 20, "Cost Of Fund");
                    excelTemplate.CreateCellColHead(excelRow, 21, "Portfolio");

                    excelTemplate.CreateCellColHead(excelRow, 22, "Sec. ID");
                    excelTemplate.CreateCellColHead(excelRow, 23, "ISIN Code");
                    excelTemplate.CreateCellColHead(excelRow, 24, "Cur.");
                    excelTemplate.CreateCellColHead(excelRow, 25, "Total Par Value");
                    excelTemplate.CreateCellColHead(excelRow, 26, "Par / Unit");
                    excelTemplate.CreateCellColHead(excelRow, 27, "Purchase Units");
                    excelTemplate.CreateCellColHead(excelRow, 28, "Net Settle Unit (+/-)");
                    excelTemplate.CreateCellColHead(excelRow, 29, "Port");

                    excelTemplate.CreateCellColHead(excelRow, 30, "FO Apprv");
                    excelTemplate.CreateCellColHead(excelRow, 31, "Commentaries");
                    excelTemplate.CreateCellColHead(excelRow, 32, "Remark Amend/Cancel");
                    excelTemplate.CreateCellColHead(excelRow, 33, "Trans Type Name");
                    excelTemplate.CreateCellColHead(excelRow, 34, "Time");
                    excelTemplate.CreateCellColHead(excelRow, 35, "Input ID");
                    excelTemplate.CreateCellColHead(excelRow, 36, "Status");

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
                            total_cashamount += double.Parse(dt.Rows[i]["purchase_price"].ToString());

                            excelTemplate.CreateCellColRight(excelRow, 0, (no).ToString());

                            double purchase_price = 0;
                            if (dt.Rows[i]["purchase_price"].ToString() != string.Empty)
                                purchase_price = double.Parse(dt.Rows[i]["purchase_price"].ToString());

                            excelTemplate.CreateCellCol2Decimal(excelRow, 12, purchase_price);


                            double repurchase_price = 0;
                            if (dt.Rows[i]["repurchase_price"].ToString() != string.Empty)
                                repurchase_price = double.Parse(dt.Rows[i]["repurchase_price"].ToString());

                            excelTemplate.CreateCellCol2Decimal(excelRow, 13, repurchase_price);

                            double net_settlement_amount = 0;
                            if (dt.Rows[i]["net_settlement_amount"].ToString() != string.Empty)
                                net_settlement_amount = double.Parse(dt.Rows[i]["net_settlement_amount"].ToString());

                            excelTemplate.CreateCellColDecimalBucket(excelRow, 14, net_settlement_amount, 2);
                        }

                        excelTemplate.CreateCellColCenter(excelRow, 1, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["trade_date"].ToString()));
                        excelTemplate.CreateCellColCenter(excelRow, 2, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["settlement_date"].ToString()));
                        excelTemplate.CreateCellColCenter(excelRow, 3, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["maturity_date"].ToString()));

                        double period = 0;
                        if (dt.Rows[i]["period"].ToString() != string.Empty)
                            period = double.Parse(dt.Rows[i]["period"].ToString());

                        excelTemplate.CreateCellColNumber(excelRow, 4, period);
                        excelTemplate.CreateCellColCenter(excelRow, 5, dt.Rows[i]["instrument_code"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 6, dt.Rows[i]["instrument_type"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 7, transNo);
                        excelTemplate.CreateCellColLeft(excelRow, 8, dt.Rows[i]["contract_no"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 9, dt.Rows[i]["counterparty_code"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 10, dt.Rows[i]["counterparty_fund"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 11, dt.Rows[i]["ref_trans_no"].ToString());


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
                            excelTemplate.CreateCellCol6Decimal(excelRow, 17, spread);
                        }

                        excelTemplate.CreateCellColCenter(excelRow, 18, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["fixing_date"].ToString()));

                        double repo_interest_rate = 0;
                        if (dt.Rows[i]["repo_interest_rate"].ToString() != string.Empty)
                            repo_interest_rate = double.Parse(dt.Rows[i]["repo_interest_rate"].ToString());

                        excelTemplate.CreateCellCol6Decimal(excelRow, 19, repo_interest_rate);

                        double cost_of_fund = 0;
                        if (dt.Rows[i]["cost_of_fund"].ToString() != string.Empty)
                            cost_of_fund = double.Parse(dt.Rows[i]["cost_of_fund"].ToString());

                        excelTemplate.CreateCellCol6Decimal(excelRow, 20, cost_of_fund);
                        excelTemplate.CreateCellColCenter(excelRow, 21, dt.Rows[i]["port"].ToString());

                        excelTemplate.CreateCellColLeft(excelRow, 22, dt.Rows[i]["sec_id_col"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 23, dt.Rows[i]["isin_code_col"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 24, dt.Rows[i]["cur_col"].ToString());

                        double purchase_unit_col = 0;
                        if (dt.Rows[i]["purchase_unit_col"].ToString() != string.Empty)
                            purchase_unit_col = double.Parse(dt.Rows[i]["purchase_unit_col"].ToString());

                        double par_unit_col = 0;
                        if (dt.Rows[i]["par_unit_col"].ToString() != string.Empty)
                            par_unit_col = double.Parse(dt.Rows[i]["par_unit_col"].ToString());

                        var totalPar = purchase_unit_col * par_unit_col;
                        total_purchaseunit += totalPar;

                        excelTemplate.CreateCellColNumber(excelRow, 25, totalPar);
                        excelTemplate.CreateCellColNumber(excelRow, 26, par_unit_col);


                        excelTemplate.CreateCellColDecimalBucket(excelRow, 27, purchase_unit_col, 0);

                        double net_settlemet_unit = 0;
                        if (dt.Rows[i]["net_settlemet_unit"].ToString() != string.Empty)
                            net_settlemet_unit = double.Parse(dt.Rows[i]["net_settlemet_unit"].ToString());

                        excelTemplate.CreateCellColDecimalBucket(excelRow, 28, net_settlemet_unit, 0);

                        excelTemplate.CreateCellColCenter(excelRow, 29, dt.Rows[i]["port_col"].ToString());


                        excelTemplate.CreateCellColCenter(excelRow, 30, dt.Rows[i]["fo_approve"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 31, dt.Rows[i]["commentaries"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 32, dt.Rows[i]["remark_cancel"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 33, dt.Rows[i]["trans_type_name"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 34, dt.Rows[i]["time"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 35, dt.Rows[i]["input_id"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 36, dt.Rows[i]["status"].ToString());

                    }

                    rowIndex++;

                    //footer
                    if (!string.IsNullOrEmpty(reportCriteriaModel.currency))
                    {
                        excelRow = sheet.CreateRow(rowIndex);

                        for (var i = 0; i <= 35; i++) excelTemplate.CreateCellFooter(excelRow, i, "");

                        excelTemplate.CreateCellFooter2Decimal(excelRow, 12, total_cashamount);
                        excelTemplate.CreateCellFooterNumber(excelRow, 25, total_purchaseunit);
                    }

                    for (var i = 1; i <= 35; i++)
                    {
                        sheet.AutoSizeColumn(i);

                        var colWidth = sheet.GetColumnWidth(i);
                        if (colWidth < 2000)
                            sheet.SetColumnWidth(i, 2000);
                        else
                            sheet.SetColumnWidth(i, colWidth + 200);
                    }

                    // Set Merge Cells
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 15));
                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 7));

                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 0, 0));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 1, 1));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 2, 2));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 3, 3));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 4, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 5, 5));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 6, 6));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 7, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 8, 8));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 9, 9));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 10, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 11, 11));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 12, 12));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 13, 13));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 14, 14));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 15, 15));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 16, 16));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 17, 17));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 18, 18));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 19, 19));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 20, 20));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 21, 21));

                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 22, 29));

                    sheet.AddMergedRegion(new CellRangeAddress(6, 6, 22, 22));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 6, 23, 23));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 6, 24, 24));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 6, 25, 25));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 6, 26, 26));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 6, 27, 27));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 6, 28, 28));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 6, 29, 29));


                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 30, 30));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 31, 31));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 32, 32));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 33, 33));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 34, 34));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 35, 35));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 6, 36, 36));

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

            apiReport.ReportData.RollOverTransactionReport(data, p =>
            {
                if (p.Success)
                {
                    dt = p.Data.RollOverTransactionReportResultModel.ToDataTable();
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