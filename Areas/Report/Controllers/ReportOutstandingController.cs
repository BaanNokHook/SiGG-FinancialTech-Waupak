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
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Areas.Report.Controllers
{
    [Authorize]
    [Audit]
    public class ReportOutstandingController : BaseController
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
            ReportCriteriaModel model = new ReportCriteriaModel();
            model.currency = "THB";

            if (User.DeskGroupName != null && User.DeskGroupName != "")
            {
                model.port = User.DeskGroupName.ToUpper();
                model.port_name = User.DeskGroupName.ToUpper();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ReportCriteriaModel model, FormCollection collection)
        {
            var Error_desc = string.Empty;
            var Error_code = 0;
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
                Report_File_Name = reportname_list[0].Text;
                Report_Header = "รายงาน Outstanding";

                if (model.trans_deal_type == "BR")
                {
                    Report_Header += " ของเงินกู้ธุรกรรม ";
                }
                else if (model.trans_deal_type == "LD")
                {
                    Report_Header += " ของเงินให้กู้ธุรกรรม ";
                }
                else
                {
                    Report_Header += " ของเงินให้กู้ / เงินกู้ธุรกรรม ";
                }

                Report_Header += (model.repo_deal_type == null
                    ? "Bilateral Repo & Private Repo"
                    : model.repo_deal_type_name);

                Report_Header += $" (Report No.{reportid})";

                model.asofdate = string.IsNullOrEmpty(model.asofdate_string)
                    ? model.asofdate
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(model.asofdate_string);

                Report_DateFromTo += (model.asofdate != null || model.asofdate != null ? "As Of Date " : "") +
                                     (model.asofdate == null ? "" : model.asofdate.Value.ToString("dd/MM/yyyy"));

                if (model.currency != null)
                {
                    Report_DateFromTo += ", Currency " + model.currency;
                }

                if (model.counterparty_code_name != null)
                {
                    Report_DateFromTo += ", Counter Party " + model.counterparty_code_name;
                }

                if (model.instrument_code_name != null)
                {
                    Report_DateFromTo += ", Security Name " + model.instrument_code_name;
                }

                Report_Port = string.IsNullOrEmpty(model.port) ? "Port : ALL" : "Port : " + model.port_name;

                var businessdate = reportentity.Getbusinessdate();
                var portfolio = string.IsNullOrEmpty(model.port_name) ? "" : model.port_name;
                if (collection["PDF"] != null)
                {
                    dt = GetOutstanding(model, ref Error_code, ref Error_desc);
                    var rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),
                        "OutstandingReport.rpt"));
                    rd.SetDataSource(dt);
                    rd.SetParameterValue("asofdate", Report_DateFromTo);
                    rd.SetParameterValue("businessdate", businessdate.Value.ToString("dd/MM/yyyy"));
                    rd.SetParameterValue("portfolio", portfolio);
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
                    dt = GetOutstanding(model, ref Error_code, ref Error_desc);
                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("Outstanding");

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
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_Port);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, $"System : Repo (Run date and time {DateTime.Now:dd/MM/yyyy HH:mm})");
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
                    excelTemplate.CreateCellColHead(excelRow, 7, "Trans. No.");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Contract No.");
                    excelTemplate.CreateCellColHead(excelRow, 9, "Counterparty Name");
                    excelTemplate.CreateCellColHead(excelRow, 10, "Counterparty Code");

                    excelTemplate.CreateCellColHead(excelRow, 11, "Counterparty Fund Name");
                    excelTemplate.CreateCellColHead(excelRow, 12, "Counterparty Fund Code");

                    excelTemplate.CreateCellColHead(excelRow, 13, "Purchase Price");
                    excelTemplate.CreateCellColHead(excelRow, 14, "Currency");
                    excelTemplate.CreateCellColHead(excelRow, 15, "Reference Rate");
                    excelTemplate.CreateCellColHead(excelRow, 16, "Spread");
                    excelTemplate.CreateCellColHead(excelRow, 17, "Fixing Date");
                    excelTemplate.CreateCellColHead(excelRow, 18, "Repo Interest Rate");
                    excelTemplate.CreateCellColHead(excelRow, 19, "Accrued Interest");

                    excelTemplate.CreateCellColHead(excelRow, 20, "Portfolio");
                    excelTemplate.CreateCellColHead(excelRow, 21, "Tran. Status");
                    excelTemplate.CreateCellColHead(excelRow, 22, "Trans Type Name");

                    excelTemplate.CreateCellColHead(excelRow, 23, "Collateral Details");

                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);

                    excelTemplate.CreateCellColHead(excelRow, 0, "No.");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Trade Date");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Settlement Date");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Maturity Date");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Period");
                    excelTemplate.CreateCellColHead(excelRow, 5, "Inst.");
                    excelTemplate.CreateCellColHead(excelRow, 6, "Inst. Type");
                    excelTemplate.CreateCellColHead(excelRow, 7, "Trans. No.");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Contract No.");
                    excelTemplate.CreateCellColHead(excelRow, 9, "Counterparty Name");
                    excelTemplate.CreateCellColHead(excelRow, 10, "Counterparty Code");

                    excelTemplate.CreateCellColHead(excelRow, 11, "Counterparty Fund Name");
                    excelTemplate.CreateCellColHead(excelRow, 12, "Counterparty Fund Code");

                    excelTemplate.CreateCellColHead(excelRow, 13, "Purchase Price");
                    excelTemplate.CreateCellColHead(excelRow, 14, "Currency");
                    excelTemplate.CreateCellColHead(excelRow, 15, "Reference Rate");
                    excelTemplate.CreateCellColHead(excelRow, 16, "Spread");
                    excelTemplate.CreateCellColHead(excelRow, 17, "Fixing Date");
                    excelTemplate.CreateCellColHead(excelRow, 18, "Repo Interest Rate");
                    excelTemplate.CreateCellColHead(excelRow, 19, "Accrued Interest");

                    excelTemplate.CreateCellColHead(excelRow, 20, "Portfolio");
                    excelTemplate.CreateCellColHead(excelRow, 21, "Tran. Status");
                    excelTemplate.CreateCellColHead(excelRow, 22, "Trans Type Name");

                    excelTemplate.CreateCellColHead(excelRow, 23, "Sec. ID");
                    excelTemplate.CreateCellColHead(excelRow, 24, "ISIN Code");
                    excelTemplate.CreateCellColHead(excelRow, 25, "SEC TYPE");
                    excelTemplate.CreateCellColHead(excelRow, 26, "Cur.");
                    excelTemplate.CreateCellColHead(excelRow, 27, "Purchase Units");
                    excelTemplate.CreateCellColHead(excelRow, 28, "Par/Unit");
                    excelTemplate.CreateCellColHead(excelRow, 29, "Total Par Value");
                    excelTemplate.CreateCellColHead(excelRow, 30, "Cash Amount");
                    excelTemplate.CreateCellColHead(excelRow, 31, "Market Value");
                    excelTemplate.CreateCellColHead(excelRow, 32, "Maturity Date");
                    excelTemplate.CreateCellColHead(excelRow, 33, "Coupon Rate");
                    excelTemplate.CreateCellColHead(excelRow, 34, "Sec MTM");
                    excelTemplate.CreateCellColHead(excelRow, 35, "Port");

                    double total_cashamount = 0;
                    double total_brp_led = 0;
                    double total_prp_led = 0;
                    double total_led = 0;
                    double total_brp_brw = 0;
                    double total_prp_brw = 0;
                    double total_brw = 0;

                    double total_cash_x_rate = 0;

                    #region :: Loop Writing Data Record ::

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

                            double cash_amount = 0;
                            if (dt.Rows[i]["cash_amount"].ToString() != string.Empty)
                                cash_amount = double.Parse(dt.Rows[i]["cash_amount"].ToString());

                            double repo_int_rate_cal = 0;
                            if (dt.Rows[i]["repo_int_rate"].ToString() != string.Empty)
                                repo_int_rate_cal = double.Parse(dt.Rows[i]["repo_int_rate"].ToString());

                            if (dt.Rows[i]["cash_amount"].ToString() != string.Empty)
                                cash_amount = double.Parse(dt.Rows[i]["cash_amount"].ToString());

                            total_cashamount += cash_amount;
                            total_cash_x_rate += cash_amount * repo_int_rate_cal;

                            if (dt.Rows[i]["instrument_type"].ToString() == Constant.TRANS_DEAL_TYPE_LENDING)
                            {
                                total_led += cash_amount;

                                if (dt.Rows[i]["repo_deal_type"].ToString() == "BRP")
                                    total_brp_led += cash_amount;
                                else if (dt.Rows[i]["repo_deal_type"].ToString() == "PRP")
                                    total_prp_led += cash_amount;
                            }
                            else if (dt.Rows[i]["instrument_type"].ToString() == Constant.TRANS_DEAL_TYPE_BORROWING)
                            {
                                total_brw += cash_amount;
                                if (dt.Rows[i]["repo_deal_type"].ToString() == "BRP")
                                    total_brp_brw += cash_amount;
                                else if (dt.Rows[i]["repo_deal_type"].ToString() == "PRP")
                                    total_prp_brw += cash_amount;
                            }

                            excelTemplate.CreateCellColRight(excelRow, 0, (no).ToString());
                            excelTemplate.CreateCellColDecimalBucket(excelRow, 13, cash_amount, 2);

                            double accru_int = 0;
                            if (dt.Columns.Contains("accru_int") && dt.Rows[i]["accru_int"].ToString() != string.Empty)
                            {
                                accru_int = double.Parse(dt.Rows[i]["accru_int"].ToString());
                                excelTemplate.CreateCellColDecimalBucket(excelRow, 19, accru_int, 2);
                            }
                            else
                            {
                                excelTemplate.CreateCellColCenter(excelRow, 19, string.Empty);
                            }

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
                        excelTemplate.CreateCellColCenter(excelRow, 7, dt.Rows[i]["trans_no"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 8, dt.Rows[i]["contract_no"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 9, dt.Rows[i]["counterparty_name"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 10, dt.Rows[i]["counterparty_code"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 11, dt.Rows[i]["counterparty_fund_name"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 12, dt.Rows[i]["counterparty_fund_code"].ToString());

                        excelTemplate.CreateCellColCenter(excelRow, 14, dt.Rows[i]["currency"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 15, dt.Rows[i]["reference_rate"].ToString());

                        double spread = 0;
                        if (dt.Rows[i]["spread"].ToString() != string.Empty)
                            spread = double.Parse(dt.Rows[i]["spread"].ToString());

                        if (dt.Rows[i]["reference_rate"].ToString() == "FIXED")
                        {
                            excelTemplate.CreateCellColLeft(excelRow, 16, "");
                        }
                        else
                        {
                            excelTemplate.CreateCellColDecimalBucket(excelRow, 16, spread, 6);
                        }

                        excelTemplate.CreateCellColCenter(excelRow, 17, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["fixing_date"].ToString()));

                        double repo_int_rate = 0;

                        if (dt.Rows[i]["repo_int_rate"].ToString() != string.Empty)
                            repo_int_rate = double.Parse(dt.Rows[i]["repo_int_rate"].ToString());

                        excelTemplate.CreateCellColDecimalBucket(excelRow, 18, repo_int_rate, 6);

                        excelTemplate.CreateCellColCenter(excelRow, 20, dt.Rows[i]["portfolio"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 21, dt.Rows[i]["tran_status"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 22, dt.Rows[i]["trans_type_name"].ToString());

                        excelTemplate.CreateCellColLeft(excelRow, 23, dt.Rows[i]["security_id_col"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 24, dt.Rows[i]["isin_code"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 25, dt.Rows[i]["security_type_col"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 26, dt.Rows[i]["cur_col"].ToString());

                        double purchase_units_col = 0;
                        if (dt.Rows[i]["purchase_units_col"].ToString() != string.Empty)
                            purchase_units_col = double.Parse(dt.Rows[i]["purchase_units_col"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 27, purchase_units_col, 2);

                        double par_unit_col = 0;
                        if (dt.Rows[i]["par_unit_col"].ToString() != string.Empty)
                            par_unit_col = double.Parse(dt.Rows[i]["par_unit_col"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 28, par_unit_col, 2);

                        double total_par_value_col = 0;
                        if (dt.Rows[i]["total_par_value_col"].ToString() != string.Empty)
                            total_par_value_col = double.Parse(dt.Rows[i]["total_par_value_col"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 29, total_par_value_col, 2);

                        double cash_amount_col = 0;
                        if (dt.Rows[i]["cash_amount_col"].ToString() != string.Empty)
                            cash_amount_col = double.Parse(dt.Rows[i]["cash_amount_col"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 30, cash_amount_col, 2);

                        double market_value_col = 0;
                        if (dt.Rows[i]["market_value_col"].ToString() != string.Empty)
                            market_value_col = double.Parse(dt.Rows[i]["market_value_col"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 31, market_value_col, 2);

                        excelTemplate.CreateCellColCenter(excelRow, 32, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["mature_date_col"].ToString()));

                        double coupon_rate_col = 0;
                        if (dt.Rows[i]["coupon_rate_col"].ToString() != string.Empty)
                            coupon_rate_col = double.Parse(dt.Rows[i]["coupon_rate_col"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 33, coupon_rate_col, 6);

                        double security_mtm_col = 0;
                        if (dt.Rows[i]["security_mtm_col"].ToString() != string.Empty)
                            security_mtm_col = double.Parse(dt.Rows[i]["security_mtm_col"].ToString());
                        excelTemplate.CreateCellColDecimalBucket(excelRow, 34, security_mtm_col, 2);
                        excelTemplate.CreateCellColCenter(excelRow, 35, dt.Rows[i]["port_col"].ToString());
                    }

                    #endregion

                    // Add by Paweekorn.l
                    // Check Currency is null not show total data
                    var group_cur_collection =
                        (from row in dt.AsEnumerable() group row by row["currency"] into g select g.Key).ToList();

                    if (!string.IsNullOrEmpty(model.currency) && !string.IsNullOrWhiteSpace(model.currency) ||
                        group_cur_collection.Count == 1)
                    {
                        #region :: Total Repo Outstanding ::

                        var avg_intrate = total_cashamount > 0 ? total_cash_x_rate / total_cashamount : 0;
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);

                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 13, total_cashamount, 2);
                        excelTemplate.CreateCellFooter(excelRow, 17, "Avg Int");
                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 18, avg_intrate, 6);

                        //rowIndex++;
                        //excelRow = sheet.CreateRow(rowIndex);
                        //sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 10, 12));
                        //excelTemplate.CreateCellFooter(excelRow, 10, "Total Cash Amount");
                        //excelTemplate.CreateCellFooter(excelRow, 11, "Total Cash Amount");
                        //excelTemplate.CreateCellFooter(excelRow, 12, "");
                       

                        if(model.repo_deal_type == null && model.trans_deal_type == null)
                        {
                            rowIndex++;
                            rowIndex++;
                            excelRow = sheet.CreateRow(rowIndex);
                            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 10, 12));
                            excelTemplate.CreateCellFooter(excelRow, 10, "Outstanding Bilateral Repo - Lending");
                            excelTemplate.CreateCellFooter(excelRow, 11, "Outstanding Bilateral Repo - Lending");
                            excelTemplate.CreateCellFooter(excelRow, 12, "");
                            excelTemplate.CreateCellFooterDecimalBucket(excelRow, 13, total_brp_led, 2);

                            rowIndex++;
                            excelRow = sheet.CreateRow(rowIndex);
                            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 10, 12));
                            excelTemplate.CreateCellFooter(excelRow, 10, "Outstanding Private Repo - Lending");
                            excelTemplate.CreateCellFooter(excelRow, 11, "Outstanding Private Repo - Lending");
                            excelTemplate.CreateCellFooter(excelRow, 12, "");
                            excelTemplate.CreateCellFooterDecimalBucket(excelRow, 13, total_prp_led, 2);

                            rowIndex++;
                            var rowTotalLending = rowIndex + 1;
                            excelRow = sheet.CreateRow(rowIndex);
                            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 10, 12));
                            excelTemplate.CreateCellFooter(excelRow, 10, "Total Outstanding Repo - Lending");
                            excelTemplate.CreateCellFooter(excelRow, 11, "Total Outstanding Repo - Lending");
                            excelTemplate.CreateCellFooter(excelRow, 12, "");
                            excelTemplate.CreateCellFooterDecimalBucket(excelRow, 13, total_led, 2);

                            rowIndex++;
                            excelRow = sheet.CreateRow(rowIndex);
                            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 10, 12));
                            excelTemplate.CreateCellFooter(excelRow, 10, "Outstanding Bilateral Repo - Borrowing");
                            excelTemplate.CreateCellFooter(excelRow, 11, "Outstanding Bilateral Repo - Borrowing");
                            excelTemplate.CreateCellFooter(excelRow, 12, "");
                            excelTemplate.CreateCellFooterDecimalBucket(excelRow, 13, total_brp_brw, 2);

                            rowIndex++;
                            excelRow = sheet.CreateRow(rowIndex);
                            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 10, 12));
                            excelTemplate.CreateCellFooter(excelRow, 10, "Outstanding Private Repo - Borrowing");
                            excelTemplate.CreateCellFooter(excelRow, 11, "Outstanding Private Repo - Borrowing");
                            excelTemplate.CreateCellFooter(excelRow, 12, "");
                            excelTemplate.CreateCellFooterDecimalBucket(excelRow, 13, total_prp_brw, 2);

                            rowIndex++;
                            var rowTotalBorrowing = rowIndex + 1;
                            excelRow = sheet.CreateRow(rowIndex);
                            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 10, 12));
                            excelTemplate.CreateCellFooter(excelRow, 10, "Total Outstanding Repo - Borrowing");
                            excelTemplate.CreateCellFooter(excelRow, 11, "Total Outstanding Repo - Borrowing");
                            excelTemplate.CreateCellFooter(excelRow, 12, "");
                            excelTemplate.CreateCellFooterDecimalBucket(excelRow, 13, total_brw, 2);

                            rowIndex++;
                            excelRow = sheet.CreateRow(rowIndex);
                            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 10, 12));
                            excelTemplate.CreateCellFooter(excelRow, 10, "Total Outstanding Repo");
                            excelTemplate.CreateCellFooter(excelRow, 11, "Total Outstanding Repo");
                            excelTemplate.CreateCellFooter(excelRow, 12, "");
                            excelTemplate.CreateCellFooterDecimalBucket(excelRow, 13, total_cashamount, 2);

                            var fn = string.Format("N{0}-N{1}", rowTotalLending, rowTotalBorrowing);
                            excelRow.GetCell(13).SetCellFormula(fn);
                        }

                        if (dt.Rows.Count > 0) 
                        {
                            rowIndex++;
                            excelRow = sheet.CreateRow(rowIndex);
                            excelTemplate.CreateCellHeaderLeft(excelRow, 0,
                                "*Remark :  Total Outstanding Repo มีค่าเป็นบวก คือ net Lending, มีค่าติดลบ คือ net Borrowing");
                        }
                      
                        #endregion
                    }
                  
                    for (var i = 1; i <= 36; i++)
                    {
                        sheet.AutoSizeColumn(i);

                        var colWidth = sheet.GetColumnWidth(i);
                        if (colWidth < 2000)
                            sheet.SetColumnWidth(i, 2000);
                        else
                            sheet.SetColumnWidth(i, colWidth + 200);
                    }

                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 0, 10));

                    sheet.AddMergedRegion(new CellRangeAddress(6, 6, 23, 35));

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

                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 23, 23));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 24, 24));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 25, 25));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 26, 26));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 27, 27));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 28, 28));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 29, 29));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 30, 30));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 31, 31));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 32, 32));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 33, 33));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 34, 34));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 35, 35));

                    var exportfile = new MemoryStream();
                    workbook.Write(exportfile);
                    Response.Clear();
                    Response.ClearContent();
                    Response.ClearHeaders();

                    Response.AppendHeader("Content-Type", "application/vnd.ms-excel");
                    Response.AppendHeader("Cache-Control", "must-revalidate, post-check=0, pre-check=0");
                    Response.AppendHeader("Cache-Control", "max-age=30");
                    Response.AppendHeader("Pragma", "public");
                    Response.AppendHeader("Content-disposition", "attachment; filename=" + Report_File_Name + ".xls");

                    Response.BinaryWrite(exportfile.GetBuffer());
                    System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
                    return View();
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                throw ex;
            }
        }

        public DataTable GetOutstanding(ReportCriteriaModel data, ref int ErrorCode, ref string ErrorDesc)
        {
            var dt = new DataTable();

            var tmpErrorCode = 0;
            var tmpErrorDesc = string.Empty;
            apiReport.ReportData.OutstandingReport(data, p =>
            {
                if (p.Success) dt = p.Data.OutstandingReportResultModel.ToDataTable();

                tmpErrorCode = p.RefCode;
                tmpErrorDesc = p.Message;
            });

            ErrorCode = tmpErrorCode;
            ErrorDesc = tmpErrorDesc;
            return dt;
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

        public ActionResult FillRepoDealType(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_deal.RPDealEntry.GetDDLRepoDealType(p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillInstrument(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_deal.RPDealEntry.GetDDLInstrumentType(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
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