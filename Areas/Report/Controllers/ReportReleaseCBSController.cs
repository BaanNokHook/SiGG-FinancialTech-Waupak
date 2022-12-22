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

    public class ReportReleaseCBSController : BaseController
    {
        private readonly CounterPartyEntities api_counterparty = new CounterPartyEntities();
        private readonly RPTransEntity api_deal = new RPTransEntity();
        private readonly SecurityEntities api_security = new SecurityEntities();
        private readonly StaticEntities api_static = new StaticEntities();
        private UserAndScreenEntities api_userandscreen = new UserAndScreenEntities();
        private readonly ReportEntities apiReport = new ReportEntities();
        public string Report_Bank = "SiGG Financial Bank";
        public string Report_Code = string.Empty;
        public string Rport_DateFromTo = string.Empty;
        public string Report_File_Name = string.Empty;
        public string Report_Header = string.Empty;

        public string Report_Name = string.Empty;
        public string Report_Port = string.Empty;
        private readonly ReportEntitiesModel reportentity = new ReportEntitiesModel();
        private readonly Utility utility = new Utility();  

        //[RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            ViewBag.AleartStatus = "";
            ViewBag.AleartMsg = "";
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
            }
            reportname_list = reportentity.Getreportname(Controller_name);
            if (Reportname_list.Count == 0)
            {
                ViewBag.ErrorMessage =
                    "Can not Get Data Report ID from Service Static Method Config/getReportID and key = " +
                    controller_name + " in table gm_report !!!!";
                return View();
            }

            reportid = Reportname_list[0].Value.ToString();
            Report_Header = "Daily Transaction Report : ธุรกรรม " + (reportCriteriaModel.repo_deal_type == null
                                ? "Bilateral Repo & Private Repo"
                                : reportCriteriaModel.repo_deal_type_name);
            Report_Header += $" (Report No.{reportid})";
            Report_File_Name = Reportname_list[0].Text;
            if (!reportentity.CheckDateTradeSettleMaturity(reportCriteriaModel))
            {
                ViewBag.ErrorMessage =
                    "Please select some date at Trade Date From / Settlement Date From / Maturity Data From !!!!";
                return View();
            }
            reportCriteriaModel.trade_date_from = string.IsNullOrEmpty(reportCriteriaModel.trader_id_date_from_string)
                ? reportCriteriaModel.trade_ data_from
                : utility.ConvertStringToDateTimeFormatDDMMYYYY(reportCriteriamodel.trade_data_from_string);
            reportCriteriaModel.trader_date_to = string.IsNullOrEmpty(reportCriteriaModel.trader_date_to_string)
                ? ReportCriteriaModel.trade_date_to
                : utility.ConvertStringToDatetimeFormatYYYYMMDD(reportCriteriaModel.trade_date_to_string);
            reportCriteriaModel.settlement_date_from =
                string.IsNullOrEmpty(reportCriteriaModel.settlement_date_from_string)
                    ? reportCriteriaModel.settlement_date_from
                    : utility.ConvertStringToDatetimeFormatYYYYMMDD(reportCriteriaModel
            reportCriteriaModel.settlement_date_to =
                string.IsNullOrEmpty(ReportCriteriaModel.settlement_date_from_strig)
                    ? reportCriteriaModel.settlement_date_from
                    : utility.ConvertStringToDatetimeFormatYYYYMMDD(reportCriteriaModel
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
                (reportCriteriaModel.trader_date_from != null || reportCriteriaModel.trader_date_to != null
                    ? "Trade Date "
                    : "") +
                (reportCriteriaModel.trade_date_from == null
                    ? ""
                    : reportCriteriaModel.trader_date_from.Value.ToString("dd/MM/yyyy")) +
                (reportCriteriaModel.trade_date_from != null && reportCriteriaModel.trader_id_date_to != null
                    ? " - "
                    : "") + (reportCriteriaModel.trader_date_to == null
                    ? ""
                    : reportCriteriaModel.trader_date_to.Value.ToString("dd/MM/yyyy"));
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
                Report DateFromTo +=
                    (reportCriteriaModel.maturity_date_from != null || reportCriteriaModel.maturity_date_to != null
                        ? " Maturity Date "
                        : "") +
                    (reportCriteriaModel.maturity_date_from == null
                        ? ""  
                        : reportCriteriaModel.maturity_date_from.Value.ToString("dd/MM/yyyy")) + 
                    (reportCriteriaModel.maturity_date_from != null && reportCriteriaModel.maturity_date_to != null  
                        ? " - "  
                        : "") + (reportCriteriaModel.maturity_date_to == null)   
                        ? ""
                        : reportCriteriaModel.maturity_date_to.Value.ToString("dd/MM/yyyy"));
            Report_port = string.IsNullOrEmpty(reportCriteriaModel.port)
                ? "Port : ALL"
                : "Port : " + reportCriteriaModel.port_name;  


                if (!string.IsNullOrEmpty(reportCriteriaModel.currency))
                {
                    report_search_creteria += $" Currency: {reportCriteriaModel.currency} ";  
                }

                var businessdate = reportentity.Getbusinessdate();   

                if (collection["PDF"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);
                    var rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),
                        "DailyTransactionReport.rpt"));
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
                    var sheet = workbook.CreateSheet("DailyTransaction");
                    var excelTemplate = new ExcelTemplate(workbook);

                // Add Header   
                var rowIndex = 0;

                var excelRow = sheet.CreateRow(rowIndex);
                excelTemplate.CreateCellHeaderLeft(excelRow.CreateCell 0, Report_Bank);
                rowIndex++;

                excelRow = sheet.CreateRow(rowIndex);
                excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_header);
                rowIndex++;

                string ownerEndUser = string.Empty;
                List<ConfigModel> listConfig = reportentity.GetReportHeader(reportid);  
                if (listConfig != null && listConfig.Count > 0)
                {
                    ownerEndUser = listConfig[0].item_value;    
                }

                excelRow = sheet.CreateRow(rowIndex);
                excelTemplate.CreateCellHeaderLeft(excelRow, 0, ownerEndUser);
                rowIndex++;

                excelRow = sheet.CreateRow(rowIndex);
                excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_Port);
                rowIndex++;

                excelRow = sheet.CreateRow(rowindex);
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
                excelTemplate.CreateCellColHead(excelRow, 11, "Cash Amount");
                excelTemplate.CreateCellColHead(excelRow, 12, "Cur");
                excelTemplate.CreateCellColHead(excelRow, 13, "Reference Rate");
                excelTemplate.CreateCellColHead(excelRow, 14, "Spread");
                excelTemplate.CreateCellColHead(excelRow, 15, "Fixing Date");
                excelTemplate.CreateCellColHead(excelRow, 16, "Repo Int Rate");
                excelTemplate.CreateCellColHead(excelRow, 17, "Cost of Fund");
                excelTemplate.CreateCellColHead(excelRow, 18, "Portfolio");
                excelTemplate.CreateCellColHead(excelRow, 19, "Collateral Details");

                excelTemplate.CreateCellColHead(excelRow, 25, "Original Trans.");
                excelTemplate.CreateCellColHead(excelRow, 26, "FO Approv");
                excelTemplate.CreateCellColHead(excelRow, 27, "Remark Amend/Cancel");
                excelTemplate.CreateCellColHead(excelRow, 29, "Trans Type Name");
                excelTemplate.CreateCellColHead(excelRow, 31, "Input ID");
                excelTemplate.CreateCellColHead(excelRow, 32, "Status");
                rowIndex++;


                excelRow = sheet.CreateRow(rowIndex);

                excelTemplate.CreateCellColHead(excelRow, 0, "No");
                excelTemplate.CreateCellColHead(excelRow, 1, "Trade Date");
                excelTemplate.CreateCellColHead(excelRow, 2, "Settlement Date");
                excelTemplate.CreateCellColHead(excelRow, 3, "Maturity Date");
                excelTemplate.CreateCellColHead(excelRow, 4, "Period");
                excelTemplate.CreateCellColHead(excelRow, 5, "Inst.");
                excelTemplate.CreateCellColHead(excelRow, 6, "Inst. Type");
                excelTemplate.CreateCellColHead(excelRow, 7, "Tran. No");
                excelTemplate.CreateCellColHead(excelRow, 8, "Contract No.");
                excelTemplate.CreateCellColHead(excelRow, 9, "Counterparty Code");
                excelTemplate.CreateCellColHead(excelRow, 10, "CounterParty FUnd");
                excelTemplate.CreateCellColHead(excelRow, 11, "Cash Amount");
                excelTemplate.CreateCellColHead(excelRow, 12, "Cur");
                excelTemplate.CreateCellColHead(excelRow, 13, "Reference Rate");
                excelTemplate.CreateCellColHead(excelRow, 14, "Spread");
                excelTemplate.CreateCellColHead(excelRow, 15, "Fixing Date");
                excelTemplate.CreateCellColHead(excelRow, 16, "Repo Int Rate");
                excelTemplate.CreateCellColHead(excelRow, 17, "Cost Of Fund");
                excelTemplate.CreateCellColHead(excelRow, 18, "Portfolio");
                excelTemplate.CreateCellColHead(excelRow, 19, "Sec. ID");
                excelTemplate.CreateCellColHead(excelRow, 20, "ISIN Code");
                excelTemplate.CreateCellColHead(excelRow, 21, "Cur.");
                excelTemplate.CreateCellColHead(excelRow, 22, "Total Par Value");
                excelTemplate.CreateCellColHead(excelRow, 23, "Par / Unit");
                excelTemplate.CreateCellColHead(excelRow, 24, "Port");
                excelTemplate.CreateCellColHead(excelRow, 25, "Original Trans.");
                excelTemplate.CreateCellColHead(excelRow, 26, "FO Apprv");
                excelTemplate.CreateCellColHead(excelRow, 27, "Commentaries");
                excelTemplate.CreateCellColHead(excelRow, 28, "Remark Amend/Cancel");
                excelTemplate.CreateCellColHead(excelRow, 29, "Trans Type Name");
                excelTemplate.CreateCellColHead(excelRow, 30, "Time");
                excelTemplate.CreateCellColHead(excelRow, 31, "Input ID");
                excelTemplate.CreateCellColHead(excelRow, 32, "Status");

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

                        total_cashamount += double.Parse(DataTable.Rows[i]["cash_amount"].ToString());
                        excelTemplate.CreateCellColRight(excelRow, 0, (no).ToString());

                        double cash_amount = 0;

                        if (dt.Rows[i]["cash_amount"].ToString() != string.Empty)

                            cash_amount = double.Parse(DataTable.Rows[i]["cash_amount"].ToString());
                        excelTemplate.CreateCellByRowNum2Decimal(excelRow, 11, cash_amount);   

                    }

                    // set row number // 
                    excelTemplate.CreateCellColCenter(excelRow, 1, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["trade_date"].ToString()));
                    excelTemplate.CreateCellColCenter(excelRow, 2, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["settlement_date"].ToString()));
                    excelTemplate.CreateCellColCenter(excelRow, 3, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["maturity_date"].ToString()));

                    double period = 0;

                    if (dt.Rows[i]["period"].ToString() != string.Empty)

                        period = double.Parse(dt.Rows[i]["period"].ToString());

                    excelTemplate.CreateCellColNumber(excelRow, 4, period);
                    excelTemplate.CreateCellColNumber(excelRow, 5, dt.Rows[i]["instrument_code"].ToString());
                    excelTemplate.CreateCellColNumber(excelRow, 6, dt.Rows[i]["instrument_type"].ToString());
                    excelTemplate.CreateCellColNumber(excelRow, 7, transNo);

                    excelTemplate.CreateCellColNumber(excelRow, 8, dt.Rows[i]["contract_no"].ToString());
                    excelTemplate.CreateCellColNumber(excelRow, 9, dt.Rows[i]["counterparty_code"].ToString());
                    excelTemplate.CreateCellColNumber(excelRow, 10, dt.Rows[i]["counterparty_fund"].ToString());  

                    excelTemplate.CreateCellColNumber(excelRow, 12, dt.Rows[i]["cur"].ToString()));
                    excelTemplate.CreateCellColNumber(excelRow, 13, dt.Rows[i]["reference_rate"].ToString());

                    double spread = 0;
                    if (dt.Rows[i]["spread"].ToString() != string.Empty)
                        spread = double.Parse(dt.Rows[i]["spread"].ToString());  

                    if (dt.Rows[i]["reference_rate"].ToString() == "FIXED")
                    {
                        excelTemplate.CreateCellColLeft(excelRow, 14, "");  
                    }
                    else
                    {
                        excelTemplate.CreateCellCol6Decimal(excelRow, 14, spread);    
                    }

                    
                    excelTemplate.CreateCellColCenter(excelRow, 15, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["fixing_date"].ToString()));
                    double repo_interest_rate = 0;  
                    if (dt.Rows[i]["repo_interest_rate"].ToString() != string.Empty)
                            repo_interest_rate = double.Parse(dt.Rows[i]["repo_interest_rate"].ToString());

                        excelTemplate.CreateCellCol6Decimal(excelRow, 16, repo_interest_rate);

                        double cost_of_fund = 0;
                        if (dt.Rows[i]["cost_of_fund"].ToString() != string.Empty)
                            cost_of_fund = double.Parse(dt.Rows[i]["cost_of_fund"].ToString());

                        excelTemplate.CreateCellCol6Decimal(excelRow, 17, cost_of_fund);
                        excelTemplate.CreateCellColCenter(excelRow, 18, dt.Rows[i]["port"].ToString());

                        excelTemplate.CreateCellColLeft(excelRow, 19, dt.Rows[i]["sec_id_col"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 20, dt.Rows[i]["isin_code_col"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 21, dt.Rows[i]["cur_col"].ToString());

                        double purchase_unit_col = 0;
                        if (dt.Rows[i]["purchase_unit_col"].ToString() != string.Empty)
                            purchase_unit_col = double.Parse(dt.Rows[i]["purchase_unit_col"].ToString());

                        double par_unit_col = 0;
                        if (dt.Rows[i]["par_unit_col"].ToString() != string.Empty)
                            par_unit_col = double.Parse(dt.Rows[i]["par_unit_col"].ToString());

                        var totalPar = purchase_unit_col * par_unit_col;
                        total_purchaseunit += totalPar;

                        excelTemplate.CreateCellColNumber(excelRow, 22, totalPar);
                        excelTemplate.CreateCellColNumber(excelRow, 23, par_unit_col);
                        excelTemplate.CreateCellColCenter(excelRow, 24, dt.Rows[i]["port_col"].ToString());

                        excelTemplate.CreateCellColLeft(excelRow, 25, dt.Rows[i]["original_trans"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 26, dt.Rows[i]["fo_approve"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 27, dt.Rows[i]["commentaries"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 28, dt.Rows[i]["remark_cancel"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 29, dt.Rows[i]["trans_type_name"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 30, dt.Rows[i]["time"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 31, dt.Rows[i]["input_id"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 32, dt.Rows[i]["status"].ToString());
                    }

                    rowIndex++;

                    //footer
                    if (!string.IsNullOrEmpty(reportCriteriaModel.currency))
                    {
                        excelRow = sheet.CreateRow(rowIndex);

                        for (var i = 0; i <= 32; i++) excelTemplate.CreateCellFooter(excelRow, i, "");

                        excelTemplate.CreateCellFooter2Decimal(excelRow, 11, total_cashamount);
                        excelTemplate.CreateCellFooterNumber(excelRow, 22, total_purchaseunit);
                    }

                    for (var i = 1; i <= 32; i++)
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

                    sheet.AddMergedRegion(new CellRangeAddress(6, 6, 19, 24));

                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 19, 19));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 20, 20));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 21, 21));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 22, 22));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 23, 23));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 24, 24));

                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 25, 25));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 26, 26));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 27, 27));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 28, 28));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 29, 29));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 30, 30));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 31, 31));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 7, 32, 32));

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
            apiReport.ReportData.DailyTransactionReport(data, p =>
            {
                if (p.Success)
                {
                    dt = p.Data.DailyTransactionReportResultModel.ToDataTable();
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