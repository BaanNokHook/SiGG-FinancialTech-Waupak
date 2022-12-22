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
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
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
    public class ReportProfitAndLossController : BaseController
    {
        private readonly RPTransEntity api_deal = new RPTransEntity();
        private readonly StaticEntities api_static = new StaticEntities();
        private readonly UserAndScreenEntities api_userandscreen = new UserAndScreenEntities();
        private readonly ReportEntities apiReport = new ReportEntities();
        private readonly CounterPartyEntities api_counterparty = new CounterPartyEntities();

        public string Report_Bank = "SiGG Financial Bank";
        public string Report_Code = string.Empty;
        public string Report_DateFromTo = string.Empty;
        public string Report_File_Name = string.Empty;
        public string Report_Header = string.Empty;
        public string Report_Port = string.Empty;
        private readonly ReportEntitiesModel reportentity = new ReportEntitiesModel();
        private readonly Utility utility = new Utility();

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            var businessdate = reportentity.Getbusinessdate();
            ReportCriteriaModel model = new ReportCriteriaModel();
            if (businessdate != null)
            {
                model.asofdate_from_string = businessdate.Value.ToString("dd/MM/yyyy");
                model.asofdate_to_string = businessdate.Value.ToString("dd/MM/yyyy");
            }


            if (User.DeskGroupName != null && User.DeskGroupName != "")
            {
                model.port = User.DeskGroupName.ToUpper();
                model.port_name = User.DeskGroupName.ToUpper();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ReportCriteriaModel reportCriteriaModel, FormCollection collection)
        {
            var Error_desc = string.Empty;
            try
            {
                var dataSet = new DataSet();
                var dataDefault = new DataTable();
                var dataExtends = new DataTable();
                var reportname_list = new List<DDLItemModel>();
                string reportid;
                string report_search_creteria = "";
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
                Report_Header = "Profit and Loss Report ธุรกรรม ";

                Report_Header += (reportCriteriaModel.repo_deal_type == null
                   ? "Bilateral Repo & Private Repo"
                   : reportCriteriaModel.repo_deal_type_name);

                Report_File_Name = reportname_list[0].Text;
                reportCriteriaModel.excel_category = string.IsNullOrEmpty(reportCriteriaModel.excel_category)
                    ? "D"
                    : reportCriteriaModel.excel_category;
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
                Report_Port = string.IsNullOrEmpty(reportCriteriaModel.port)
                    ? "Port : ALL"
                    : "Port : " + reportCriteriaModel.port_name;
                Report_Header += $" (Report No.{reportid})";

                if (!string.IsNullOrEmpty(reportCriteriaModel.counterparty_id))
                {
                    string[] counterparty_name = reportCriteriaModel.counterparty_code_name.Split(':');
                    if(counterparty_name.Length > 1)
                    {
                        report_search_creteria += $", Counter Party : {counterparty_name[1].Trim()} ";
                    }
                    
                }

                if (!string.IsNullOrEmpty(reportCriteriaModel.currency))
                {
                    report_search_creteria += $", Currency: {reportCriteriaModel.currency} ";
                }

                if (collection["PDF"] != null)
                {
                   
                }
                else if (collection["Excel"] != null)
                {
                    dataSet = GetReportData(reportCriteriaModel);
                    if (dataSet.Tables.Count > 0)
                    {
                        dataDefault = dataSet.Tables[0];
                    }
                    else
                    {
                        dataDefault = new DataTable();
                    }

                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("ProfitAndLossReport");

                    var excelTemplate = new ExcelTemplate(workbook);

                    if (reportCriteriaModel.excel_category == "D")
                    {
                        #region Add Header And Set Cell Header Style

                        // Add Header 
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
                        excelTemplate.CreateCellHeaderLeft(excelRow, 0, $"System : Repo (Run date and time {DateTime.Now:dd/MM/yyyy HH:mm})");
                        rowIndex++;

                        excelRow = sheet.CreateRow(rowIndex);

                        // Add Header Table
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 0, "As Of Date");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 1, "CounterParty Code");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 2, "CounterParty Name");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 3, "Trans No.");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 4, "Trans Type Name");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 5, "Cur");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 6, "Lending Borrow");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 7, "Trade Date");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 8, "Purchase Date");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 9, "RePurchase Date");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 10, "Purchase Price");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 11, "Repo Ref Rate");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 12, "Repo Spread");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 13, "Fixing Date");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 14, "Int Rate");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 15, "RePurchase Price");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 16, "User ID");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 17, "Year Basis");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 18, "Instrument");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 19, "Port");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 20, "Funding Ref Rate");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 21, "Funding Spread");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 22, "Funding Fixing Date");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 23, "Fund Cost");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 24, "PL Rate");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 25, "PL Amount in original CCY");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 26, "Daily PL in Original CCY");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 27, "Daily PL in THB");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 28, "Daily INT in Original CCY");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 29, "Daily INT in THB");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 30, "Daily COF in Original CCY");
                        excelTemplate.CreateCellColHeadWrapText(excelRow, 31, "Daily COF in THB");
                        rowIndex++;
                        #endregion

                        double sum_purchase_price = 0;
                        double sum_daily_pl_ccy = 0;
                        double sum_daily_pl_thb = 0;
                        try
                        {
                            for (var i = 0; i < dataDefault.Rows.Count; i++)
                            {
                                rowIndex++;
                                excelRow = sheet.CreateRow(rowIndex);

                                if (dataDefault.Rows[i]["asof_date"].ToString() != string.Empty)
                                {
                                    excelTemplate.CreateCellColCenter(excelRow, 0, DateTime.Parse(dataDefault.Rows[i]["asof_date"].ToString()));
                                }
                                else
                                {
                                    excelTemplate.CreateCellColCenter(excelRow, 0, new DateTime());
                                }

                                excelTemplate.CreateCellColLeft(excelRow, 1,
                                    dataDefault.Rows[i]["counterparty_code"].ToString());
                                excelTemplate.CreateCellColLeft(excelRow, 2,
                                    dataDefault.Rows[i]["counterparty_name"].ToString());
                                excelTemplate.CreateCellColCenter(excelRow, 3, dataDefault.Rows[i]["trans_no"].ToString());
                                excelTemplate.CreateCellColCenter(excelRow, 4, dataDefault.Rows[i]["trans_type"].ToString());
                                excelTemplate.CreateCellColCenter(excelRow, 5, dataDefault.Rows[i]["cur"].ToString());
                                excelTemplate.CreateCellColCenter(excelRow, 6,
                                    dataDefault.Rows[i]["trans_deal_type"].ToString());

                                if (dataDefault.Rows[i]["trade_date"].ToString() != string.Empty)
                                {
                                    excelTemplate.CreateCellColCenter(excelRow, 7, DateTime.Parse(dataDefault.Rows[i]["trade_date"].ToString()));
                                }
                                else
                                {
                                    excelTemplate.CreateCellColCenter(excelRow, 7, new DateTime());
                                }

                                if (dataDefault.Rows[i]["purchase_date"].ToString() != string.Empty)
                                {
                                    excelTemplate.CreateCellColCenter(excelRow, 8, DateTime.Parse(dataDefault.Rows[i]["purchase_date"].ToString()));
                                }
                                else
                                {
                                    excelTemplate.CreateCellColCenter(excelRow, 8, new DateTime());
                                }

                                if (dataDefault.Rows[i]["repurchase_date"].ToString() != string.Empty)
                                {
                                    excelTemplate.CreateCellColCenter(excelRow, 9, DateTime.Parse(dataDefault.Rows[i]["repurchase_date"].ToString()));
                                }
                                else
                                {
                                    excelTemplate.CreateCellColCenter(excelRow, 9, new DateTime());
                                }

                                double purchase_price = 0;
                                if (dataDefault.Rows[i]["purchase_price"].ToString() != string.Empty)
                                {
                                    purchase_price = double.Parse(dataDefault.Rows[i]["purchase_price"].ToString());
                                    sum_purchase_price += purchase_price;
                                }

                                excelTemplate.CreateCellCol2RedDecimal(excelRow, 10, purchase_price);

                                excelTemplate.CreateCellColCenter(excelRow, 11, dataDefault.Rows[i]["Repo_Ref_Rate"].ToString());

                                if (dataDefault.Rows[i]["Repo_Ref_Rate"].ToString() == "FIXED")
                                {
                                    excelTemplate.CreateCellColCenter(excelRow, 12, string.Empty);
                                }
                                else
                                {
                                    double repo_Spread = 0;
                                    if (dataDefault.Rows[i]["repo_Spread"].ToString() != string.Empty)
                                    {
                                        repo_Spread = double.Parse(dataDefault.Rows[i]["repo_Spread"].ToString());
                                        excelTemplate.CreateCellCol4RedDecimal(excelRow, 12, repo_Spread);
                                    }
                                    else
                                    {
                                        excelTemplate.CreateCellColCenter(excelRow, 12, string.Empty);
                                    }
                                }

                                if (dataDefault.Rows[i]["fixing_date"].ToString() != string.Empty)
                                {
                                    excelTemplate.CreateCellColCenter(excelRow, 13, DateTime.Parse(dataDefault.Rows[i]["fixing_date"].ToString()));
                                }
                                else
                                {
                                    excelTemplate.CreateCellColCenter(excelRow, 13, new DateTime());
                                }

                                double int_rate = 0;
                                if (dataDefault.Rows[i]["int_rate"].ToString() != string.Empty)
                                    int_rate = double.Parse(dataDefault.Rows[i]["int_rate"].ToString());
                                excelTemplate.CreateCellCol6Decimal(excelRow, 14, int_rate);

                                double repurchase_price = 0;
                                if (dataDefault.Rows[i]["repurchase_price"].ToString() != string.Empty)
                                    repurchase_price = double.Parse(dataDefault.Rows[i]["repurchase_price"].ToString());
                                excelTemplate.CreateCellCol2RedDecimal(excelRow, 15, repurchase_price);

                                excelTemplate.CreateCellColCenter(excelRow, 16, dataDefault.Rows[i]["trader_id"].ToString());

                                double year_basis = 0;
                                if (dataDefault.Rows[i]["year_basis"].ToString() != string.Empty)
                                    year_basis = double.Parse(dataDefault.Rows[i]["year_basis"].ToString());
                                excelTemplate.CreateCellColNumber(excelRow, 17, year_basis);

                                excelTemplate.CreateCellColCenter(excelRow, 18, dataDefault.Rows[i]["instrument"].ToString());
                                excelTemplate.CreateCellColCenter(excelRow, 19, dataDefault.Rows[i]["port"].ToString());

                                excelTemplate.CreateCellColCenter(excelRow, 20, dataDefault.Rows[i]["funding_ref_Rate"].ToString());

                                if (dataDefault.Rows[i]["funding_ref_Rate"].ToString() == "FIXED")
                                {
                                    excelTemplate.CreateCellColCenter(excelRow, 21, string.Empty);
                                }
                                else
                                {
                                    double funding_spread = 0;
                                    if (dataDefault.Rows[i]["funding_spread"].ToString() != string.Empty)
                                    {
                                        funding_spread = double.Parse(dataDefault.Rows[i]["funding_spread"].ToString());
                                        excelTemplate.CreateCellCol4RedDecimal(excelRow, 21, funding_spread);
                                    }
                                    else
                                    {
                                        excelTemplate.CreateCellColCenter(excelRow, 21, string.Empty);
                                    }
                                }

                                if (dataDefault.Rows[i]["funding_fixing_date"].ToString() != string.Empty)
                                {
                                    excelTemplate.CreateCellColCenter(excelRow, 22, DateTime.Parse(dataDefault.Rows[i]["funding_fixing_date"].ToString()));
                                }
                                else
                                {
                                    excelTemplate.CreateCellColCenter(excelRow, 22, new DateTime());
                                }

                                double fund_code = 0;
                                if (dataDefault.Rows[i]["fund_code"].ToString() != string.Empty)
                                    fund_code = double.Parse(dataDefault.Rows[i]["fund_code"].ToString());
                                excelTemplate.CreateCellCol6Decimal(excelRow, 23, fund_code);

                                double pl_rate = 0;
                                if (dataDefault.Rows[i]["pl_rate"].ToString() != string.Empty)
                                    pl_rate = double.Parse(dataDefault.Rows[i]["pl_rate"].ToString());
                                excelTemplate.CreateCellCol6Decimal(excelRow, 24, pl_rate);

                                double pl_amount = 0;
                                if (dataDefault.Rows[i]["pl_amount"].ToString() != string.Empty)
                                    pl_amount = double.Parse(dataDefault.Rows[i]["pl_amount"].ToString());
                                excelTemplate.CreateCellCol2RedDecimal(excelRow, 25, pl_amount);

                                double daily_pl_ccy = 0;
                                if (dataDefault.Rows[i]["daily_pl_ccy"].ToString() != string.Empty)
                                    daily_pl_ccy = double.Parse(dataDefault.Rows[i]["daily_pl_ccy"].ToString());
                                excelTemplate.CreateCellCol2RedDecimal(excelRow, 26, daily_pl_ccy);

                                sum_daily_pl_ccy += daily_pl_ccy;

                                double daily_pl = 0;
                                if (dataDefault.Rows[i]["daily_pl"].ToString() != string.Empty)
                                    daily_pl = double.Parse(dataDefault.Rows[i]["daily_pl"].ToString());
                                excelTemplate.CreateCellCol2RedDecimal(excelRow, 27, daily_pl);

                                sum_daily_pl_thb += daily_pl;

                                double daily_int_ccy = 0;
                                if (dataDefault.Rows[i]["daily_int_ccy"].ToString() != string.Empty)
                                    daily_int_ccy = double.Parse(dataDefault.Rows[i]["daily_int_ccy"].ToString());
                                excelTemplate.CreateCellCol2RedDecimal(excelRow, 28, daily_int_ccy);

                                double daily_int = 0;
                                if (dataDefault.Rows[i]["daily_int"].ToString() != string.Empty)
                                    daily_int = double.Parse(dataDefault.Rows[i]["daily_int"].ToString());
                                excelTemplate.CreateCellCol4RedDecimal(excelRow, 29, daily_int);

                                double daily_cof_ccy = 0;
                                if (dataDefault.Rows[i]["daily_cof_ccy"].ToString() != string.Empty)
                                    daily_cof_ccy = double.Parse(dataDefault.Rows[i]["daily_cof_ccy"].ToString());
                                excelTemplate.CreateCellCol2RedDecimal(excelRow, 30, daily_cof_ccy);

                                double daily_cof = 0;
                                if (dataDefault.Rows[i]["daily_cof"].ToString() != string.Empty)
                                    daily_cof = double.Parse(dataDefault.Rows[i]["daily_cof"].ToString());
                                excelTemplate.CreateCellCol4RedDecimal(excelRow, 31, daily_cof);
                            }
                        }
                        catch (Exception)
                        {
                        }

                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);


                        var footerunderline = excelTemplate.CellStyle(FontBoldWeight.Bold,
                            VerticalAlignment.Center, HorizontalAlignment.Right, "#,##0.00_);[Red](#,##0.00)",
                            22, 9, HSSFColor.Black.Index, FontUnderlineType.Double);

                        excelTemplate.CreateCellCustomStyle(excelRow, 10, sum_purchase_price, footerunderline);
                        excelTemplate.CreateCellCustomStyle(excelRow, 26, sum_daily_pl_ccy, footerunderline);
                        excelTemplate.CreateCellCustomStyle(excelRow, 27, sum_daily_pl_thb, footerunderline);

                        #region For Owner Report Header 
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 5, 0, 10));

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
                        sheet.AddMergedRegion(new CellRangeAddress(6, 7, 25, 25));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 7, 26, 26));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 7, 27, 27));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 7, 28, 28));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 7, 29, 29));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 7, 30, 30));
                        sheet.AddMergedRegion(new CellRangeAddress(6, 7, 31, 31));
                        #endregion

                        #region For Not Owner Header Report
                        //sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 9));
                        //sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 9));
                        //sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 9));
                        //sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 9));
                        //sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 9));

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
                        //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 25, 25));
                        //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 26, 26));
                        //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 27, 27));
                        //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 28, 28));
                        //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 29, 29));
                        //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 30, 30));
                        //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 31, 31));
                        #endregion

                        int rowHeadEarly = 0;
                        if (dataSet.Tables.Count > 1)
                        {
                            try
                            {
                                DataTable dataEarly = dataSet.Tables[1];

                                if (dataEarly.Rows.Count > 0)
                                {

                                    sum_purchase_price = 0;
                                    double sum_fee_ccy = 0;
                                    double sum_fee_thb = 0;

                                    #region Add Header And Set Cell Header Style

                                    rowIndex += 2;
                                    excelRow = sheet.CreateRow(rowIndex);

                                    rowHeadEarly = rowIndex;

                                    // Add Header Table
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 0, "As Of Date");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 1, "CounterParty Code");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 2, "CounterParty Name");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 3, "Trans No.");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 4, "Trans Type Name");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 5, "Cur");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 6, "Lending Borrow");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 7, "Trade Date");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 8, "Purchase Date");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 9, "RePurchase Date");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 10, "Purchase Price");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 11, "Repo Ref Rate");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 12, "Repo Spread");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 13, "Fixing Date");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 14, "Int Rate");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 15, "RePurchase Price");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 16, "User ID");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 17, "Year Basis");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 18, "Instrument");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 19, "Port");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 20, "Funding Ref Rate");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 21, "Funding Spread");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 22, "Funding Fixing Date");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 23, "Early Terminate Fee (%)");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 24, "PL Rate");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 25, "PL Amount in original CCY");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 26, "Early Terminate Fee (Amt.) in Original CCY");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 27, "Early Terminate Fee (Amt.) in THB");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 28, "Daily INT in Original CCY");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 29, "Daily INT in THB");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 30, "Daily COF in Original CCY");
                                    excelTemplate.CreateCellColHeadWrapText(excelRow, 31, "Daily COF in THB");

                                    rowIndex += 2;
                                    #endregion

                                    for (var i = 0; i < dataEarly.Rows.Count; i++)
                                    {
                                        rowIndex++;
                                        excelRow = sheet.CreateRow(rowIndex);

                                        if (dataEarly.Rows[i]["asof_date"].ToString() != string.Empty)
                                        {
                                            excelTemplate.CreateCellColCenter(excelRow, 0, DateTime.Parse(dataEarly.Rows[i]["asof_date"].ToString()));
                                        }
                                        else
                                        {
                                            excelTemplate.CreateCellColCenter(excelRow, 0, new DateTime());
                                        }

                                        excelTemplate.CreateCellColLeft(excelRow, 1,
                                            dataEarly.Rows[i]["counterparty_code"].ToString());
                                        excelTemplate.CreateCellColLeft(excelRow, 2,
                                            dataEarly.Rows[i]["counterparty_name"].ToString());
                                        excelTemplate.CreateCellColCenter(excelRow, 3, dataEarly.Rows[i]["trans_no"].ToString());
                                        excelTemplate.CreateCellColCenter(excelRow, 4, dataEarly.Rows[i]["trans_type"].ToString());
                                        excelTemplate.CreateCellColCenter(excelRow, 5, dataEarly.Rows[i]["cur"].ToString());
                                        excelTemplate.CreateCellColCenter(excelRow, 6,
                                            dataEarly.Rows[i]["trans_deal_type"].ToString());

                                        if (dataEarly.Rows[i]["trade_date"].ToString() != string.Empty)
                                        {
                                            excelTemplate.CreateCellColCenter(excelRow, 7, DateTime.Parse(dataEarly.Rows[i]["trade_date"].ToString()));
                                        }
                                        else
                                        {
                                            excelTemplate.CreateCellColCenter(excelRow, 7, new DateTime());
                                        }

                                        if (dataEarly.Rows[i]["purchase_date"].ToString() != string.Empty)
                                        {
                                            excelTemplate.CreateCellColCenter(excelRow, 8, DateTime.Parse(dataEarly.Rows[i]["purchase_date"].ToString()));
                                        }
                                        else
                                        {
                                            excelTemplate.CreateCellColCenter(excelRow, 8, new DateTime());
                                        }

                                        if (dataEarly.Rows[i]["repurchase_date"].ToString() != string.Empty)
                                        {
                                            excelTemplate.CreateCellColCenter(excelRow, 9, DateTime.Parse(dataEarly.Rows[i]["repurchase_date"].ToString()));
                                        }
                                        else
                                        {
                                            excelTemplate.CreateCellColCenter(excelRow, 9, new DateTime());
                                        }

                                        double purchase_price = 0;
                                        if (dataEarly.Rows[i]["purchase_price"].ToString() != string.Empty)
                                        {
                                            purchase_price = double.Parse(dataEarly.Rows[i]["purchase_price"].ToString());
                                            sum_purchase_price += purchase_price;
                                        }

                                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 10, purchase_price);

                                        excelTemplate.CreateCellColCenter(excelRow, 11, dataEarly.Rows[i]["Repo_Ref_Rate"].ToString());

                                        if (dataEarly.Rows[i]["Repo_Ref_Rate"].ToString() == "FIXED")
                                        {
                                            excelTemplate.CreateCellColCenter(excelRow, 12, string.Empty);
                                        }
                                        else
                                        {
                                            double repo_Spread = 0;
                                            if (dataEarly.Rows[i]["repo_Spread"].ToString() != string.Empty)
                                            {
                                                repo_Spread = double.Parse(dataEarly.Rows[i]["repo_Spread"].ToString());
                                                excelTemplate.CreateCellCol4RedDecimal(excelRow, 12, repo_Spread);
                                            }
                                            else
                                            {
                                                excelTemplate.CreateCellColCenter(excelRow, 12, string.Empty);
                                            }
                                        }

                                        if (dataEarly.Rows[i]["fixing_date"].ToString() != string.Empty)
                                        {
                                            excelTemplate.CreateCellColCenter(excelRow, 13, DateTime.Parse(dataEarly.Rows[i]["fixing_date"].ToString()));
                                        }
                                        else
                                        {
                                            excelTemplate.CreateCellColCenter(excelRow, 13, new DateTime());
                                        }

                                        double int_rate = 0;
                                        if (dataEarly.Rows[i]["int_rate"].ToString() != string.Empty)
                                        {
                                            int_rate = double.Parse(dataEarly.Rows[i]["int_rate"].ToString());
                                            excelTemplate.CreateCellCol6Decimal(excelRow, 14, int_rate);
                                        }

                                        double repurchase_price = 0;
                                        if (dataEarly.Rows[i]["repurchase_price"].ToString() != string.Empty)
                                        {
                                            repurchase_price = double.Parse(dataEarly.Rows[i]["repurchase_price"].ToString());
                                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 15, repurchase_price);
                                        }

                                        excelTemplate.CreateCellColCenter(excelRow, 16, dataEarly.Rows[i]["trader_id"].ToString());

                                        double year_basis = 0;
                                        if (dataEarly.Rows[i]["year_basis"].ToString() != string.Empty)
                                        {
                                            year_basis = double.Parse(dataEarly.Rows[i]["year_basis"].ToString());
                                            excelTemplate.CreateCellColNumber(excelRow, 17, year_basis);
                                        }

                                        excelTemplate.CreateCellColCenter(excelRow, 18, dataEarly.Rows[i]["instrument"].ToString());
                                        excelTemplate.CreateCellColCenter(excelRow, 19, dataEarly.Rows[i]["port"].ToString());

                                        excelTemplate.CreateCellColCenter(excelRow, 20, dataEarly.Rows[i]["funding_ref_Rate"].ToString());

                                        if (dataEarly.Rows[i]["funding_ref_Rate"].ToString() == "FIXED")
                                        {
                                            excelTemplate.CreateCellColCenter(excelRow, 21, string.Empty);
                                        }
                                        else
                                        {
                                            double funding_spread = 0;
                                            if (dataEarly.Rows[i]["funding_spread"].ToString() != string.Empty)
                                            {
                                                funding_spread = double.Parse(dataEarly.Rows[i]["funding_spread"].ToString());
                                                excelTemplate.CreateCellCol4RedDecimal(excelRow, 21, funding_spread);
                                            }
                                            else
                                            {
                                                excelTemplate.CreateCellColCenter(excelRow, 21, string.Empty);
                                            }
                                        }

                                        if (dataEarly.Rows[i]["funding_fixing_date"].ToString() != string.Empty)
                                        {
                                            excelTemplate.CreateCellColCenter(excelRow, 22, DateTime.Parse(dataEarly.Rows[i]["funding_fixing_date"].ToString()));
                                        }
                                        else
                                        {
                                            excelTemplate.CreateCellColCenter(excelRow, 22, new DateTime());
                                        }

                                        /////////EARLY TERMINATE/////////
                                        double earlyterminate_fee = 0;
                                        if (dataEarly.Rows[i]["earlyterminate_fee"].ToString() != string.Empty)
                                            earlyterminate_fee = double.Parse(dataEarly.Rows[i]["earlyterminate_fee"].ToString());
                                        excelTemplate.CreateCellCol6Decimal(excelRow, 23, earlyterminate_fee);
                                        /////////EARLY TERMINATE/////////

                                        double pl_rate = 0;
                                        if (dataEarly.Rows[i]["pl_rate"].ToString() != string.Empty)
                                        {
                                            pl_rate = double.Parse(dataEarly.Rows[i]["pl_rate"].ToString());
                                            excelTemplate.CreateCellCol6Decimal(excelRow, 24, pl_rate);
                                        }
                                        else
                                        {
                                            excelTemplate.CreateCellColLeft(excelRow, 24, "");
                                        }

                                        double pl_amount = 0;
                                        if (dataEarly.Rows[i]["pl_amount"].ToString() != string.Empty)
                                        {
                                            pl_amount = double.Parse(dataEarly.Rows[i]["pl_amount"].ToString());
                                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 25, pl_amount);
                                        }
                                        else
                                        {
                                            excelTemplate.CreateCellColLeft(excelRow, 25, "");
                                        }

                                        /////////EARLY TERMINATE/////////

                                        double early_terminate_Fee_ccy = 0;
                                        if (dataEarly.Rows[i]["early_terminate_Fee_ccy"].ToString() != string.Empty)
                                            early_terminate_Fee_ccy = double.Parse(dataEarly.Rows[i]["early_terminate_Fee_ccy"].ToString());
                                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 26, early_terminate_Fee_ccy);
                                        sum_fee_ccy += early_terminate_Fee_ccy;

                                        double early_terminate_Fee_thb = 0;
                                        if (dataEarly.Rows[i]["early_terminate_Fee_thb"].ToString() != string.Empty)
                                            early_terminate_Fee_thb = double.Parse(dataEarly.Rows[i]["early_terminate_Fee_thb"].ToString());
                                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 27, early_terminate_Fee_thb);
                                        sum_fee_thb += early_terminate_Fee_thb;

                                        /////////EARLY TERMINATE/////////

                                        double daily_int_ccy = 0;
                                        if (dataEarly.Rows[i]["daily_int_ccy"].ToString() != string.Empty)
                                        {
                                            daily_int_ccy = double.Parse(dataEarly.Rows[i]["daily_int_ccy"].ToString());
                                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 28, daily_int_ccy);
                                        }
                                        else
                                        {
                                            excelTemplate.CreateCellColLeft(excelRow, 28, "");
                                        }

                                        double daily_int = 0;
                                        if (dataEarly.Rows[i]["daily_int"].ToString() != string.Empty)
                                        {
                                            daily_int = double.Parse(dataEarly.Rows[i]["daily_int"].ToString());
                                            excelTemplate.CreateCellCol4RedDecimal(excelRow, 29, daily_int);
                                        }
                                        else
                                        {
                                            excelTemplate.CreateCellColLeft(excelRow, 29, "");
                                        }

                                        double daily_cof_ccy = 0;
                                        if (dataEarly.Rows[i]["daily_cof_ccy"].ToString() != string.Empty)
                                        {
                                            daily_cof_ccy = double.Parse(dataEarly.Rows[i]["daily_cof_ccy"].ToString());
                                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 30, daily_cof_ccy);
                                        }
                                        else
                                        {
                                            excelTemplate.CreateCellColLeft(excelRow, 30, "");
                                        }

                                        double daily_cof = 0;
                                        if (dataEarly.Rows[i]["daily_cof"].ToString() != string.Empty)
                                        {
                                            daily_cof = double.Parse(dataEarly.Rows[i]["daily_cof"].ToString());
                                            excelTemplate.CreateCellCol4RedDecimal(excelRow, 31, daily_cof);
                                        }
                                        else
                                        {
                                            excelTemplate.CreateCellColLeft(excelRow, 31, "");
                                        }
                                    }

                                    rowIndex++;
                                    excelRow = sheet.CreateRow(rowIndex);

                                    var footerunderlineEar = excelTemplate.CellStyle(FontBoldWeight.Bold,
                                        VerticalAlignment.Center, HorizontalAlignment.Right, "#,##0.00_);[Red](#,##0.00)",
                                        22, 9, HSSFColor.Black.Index, FontUnderlineType.Double);

                                    excelTemplate.CreateCellCustomStyle(excelRow, 10, sum_purchase_price, footerunderlineEar);
                                    excelTemplate.CreateCellCustomStyle(excelRow, 26, sum_fee_ccy, footerunderlineEar);
                                    excelTemplate.CreateCellCustomStyle(excelRow, 27, sum_fee_thb, footerunderlineEar);

                                    rowIndex += 2;
                                    excelRow = sheet.CreateRow(rowIndex);
                                    excelTemplate.CreateCellFooterCenter(excelRow, 23, "Grand Total");
                                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 23, 25));

                                    excelTemplate.CreateCellCustomStyle(excelRow, 26, sum_daily_pl_ccy + sum_fee_ccy, footerunderlineEar);
                                    excelTemplate.CreateCellCustomStyle(excelRow, 27, sum_daily_pl_thb + sum_fee_thb, footerunderlineEar);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }

                        for (var i = 0; i <= 31; i++)
                        {
                            sheet.AutoSizeColumn(i);

                            if (i == 0)
                            {
                                sheet.SetColumnWidth(i, 3000);
                            }
                            else
                            {
                                var colWidth = sheet.GetColumnWidth(i);
                                if (colWidth < 3000)
                                    sheet.SetColumnWidth(i, 3500);
                                else
                                    sheet.SetColumnWidth(i, colWidth + 200);
                            }
                        }

                        #region For Not Owner Header Report
                        //sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 9));
                        //sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 9));
                        //sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 9));
                        //sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 9));
                        //sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 9));

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
                        //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 25, 25));
                        //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 26, 26));
                        //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 27, 27));
                        //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 28, 28));
                        //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 29, 29));
                        //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 30, 30));
                        //sheet.AddMergedRegion(new CellRangeAddress(5, 6, 31, 31));
                        //sheet.SetColumnWidth(25, 6000);
                        //sheet.SetColumnWidth(26, 6000);
                        #endregion

                        #region Merged Early

                        if (rowHeadEarly > 0)
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 0, 0));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 1, 1));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 2, 2));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 3, 3));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 4, 4));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 5, 5));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 6, 6));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 7, 7));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 8, 8));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 9, 9));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 10, 10));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 11, 11));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 12, 12));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 13, 13));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 14, 14));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 15, 15));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 16, 16));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 17, 17));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 18, 18));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 19, 19));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 20, 20));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 21, 21));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 22, 22));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 23, 23));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 24, 24));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 25, 25));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 26, 26));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 27, 27));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 28, 28));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 29, 29));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 30, 30));
                            sheet.AddMergedRegion(new CellRangeAddress(rowHeadEarly, rowHeadEarly + 2, 31, 31));

                            sheet.SetColumnWidth(23, 3000);
                            sheet.SetColumnWidth(24, 3000);
                            sheet.SetColumnWidth(25, 6000);
                            sheet.SetColumnWidth(26, 6000);
                            sheet.SetColumnWidth(27, 6000);
                        }
                        
                        #endregion

                    }
                    else
                    {

                        double sum_daily_pl = 0;

                        // Set Merge Cells Header
                        #region Add Header And Set Cell Header Style

                        // Add Header 
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
                        excelTemplate.CreateCellHeaderLeft(excelRow, 0, $"System : Repo (Run date and time {DateTime.Now:dd/MM/yyyy HH:mm})");
                        rowIndex++;

                        var colLeft = excelTemplate.CellStyle(FontBoldWeight.Bold,
                            VerticalAlignment.Center, HorizontalAlignment.Left,
                            borderColor: 22, foregroundColor: 48, fontColor: HSSFColor.White.Index);

                        excelRow = sheet.CreateRow(rowIndex);
                        excelTemplate.CreateCellHeaderLeft(excelRow, 0, "");
                        rowIndex++;

                        excelRow = sheet.CreateRow(rowIndex);
                        excelTemplate.CreateCellCustomStyle(excelRow, 0, "DAILY_PL", colLeft);
                        rowIndex++;

                        excelRow = sheet.CreateRow(rowIndex);
                        excelTemplate.CreateCellCustomStyle(excelRow, 0, "MTD_PL", colLeft);
                        rowIndex++;

                        excelRow = sheet.CreateRow(rowIndex);
                        excelTemplate.CreateCellCustomStyle(excelRow, 0, "YTD_PL", colLeft);
                        rowIndex++;

                        sheet.SetColumnWidth(0, 4000);

                        #endregion

                        try
                        {
                            for (var i = 0; i < dataDefault.Rows.Count; i++)
                            {
                                sheet.SetColumnWidth(i + 1, 4000);

                                var asof_date = "";
                                if (dataDefault.Rows[i]["asof_date"].ToString() != string.Empty)
                                    asof_date = DateTime.Parse(dataDefault.Rows[i]["asof_date"].ToString())
                                        .ToString("dd/MM/yyyy");
                                excelTemplate.CreateCellByRowNumCenterHeader(sheet, 6, i + 1, asof_date);

                                double daily_pl = 0;
                                if (dataDefault.Rows[i]["daily_pl"].ToString() != string.Empty)
                                {
                                    daily_pl = double.Parse(dataDefault.Rows[i]["daily_pl"].ToString());
                                    excelTemplate.CreateCellByRowNum2RedDecimal(sheet, 7, i + 1, daily_pl);
                                }
                                else
                                {
                                    excelRow = sheet.GetRow(5);
                                    excelTemplate.CreateCellColCenter(excelRow, i + 1, "");
                                }

                                sum_daily_pl += daily_pl;

                                double mtd_pl = 0;
                                if (dataDefault.Rows[i]["mtd_pl"].ToString() != string.Empty)
                                {
                                    mtd_pl = double.Parse(dataDefault.Rows[i]["mtd_pl"].ToString());
                                    excelTemplate.CreateCellByRowNum2RedDecimal(sheet, 8, i + 1, mtd_pl);
                                }
                                else
                                {
                                    excelRow = sheet.GetRow(6);
                                    excelTemplate.CreateCellColCenter(excelRow, i + 1, "");
                                }

                                double YTD_PL = 0;
                                if (dataDefault.Rows[i]["ytd_pl"].ToString() != string.Empty)
                                {
                                    YTD_PL = double.Parse(dataDefault.Rows[i]["ytd_pl"].ToString());
                                    excelTemplate.CreateCellByRowNum2RedDecimal(sheet, 9, i + 1, YTD_PL);
                                }
                                else
                                {
                                    excelRow = sheet.GetRow(7);
                                    excelTemplate.CreateCellColCenter(excelRow, i + 1, "");
                                }
                            }
                        }
                        catch
                        {

                        }

                        sheet.SetColumnWidth(dataDefault.Rows.Count + 1, 5000);

                        //excelTemplate.CreateCellByRowNumCenterHeader(sheet, 5, dataDefault.Rows.Count + 1, "TOTAL");
                        //excelTemplate.CreateCellByRowNum2RedDecimal(sheet, 6, dataDefault.Rows.Count + 1, sum_daily_pl);

                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 5, 0, 10));

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
                    Response.AppendHeader("Content-disposition", "attachment; filename=" + Report_File_Name + ".xls");

                    Response.BinaryWrite(exportfile.GetBuffer());
                    System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();

                    return View("Index");
                }
                else
                {
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Index");
            }

            return View("Index");
        }

        public DataSet GetReportData(ReportCriteriaModel data)
        {
            DataSet ds = new DataSet();
            apiReport.ReportData.ProfitAndLossReport(data, p =>
            {
                if (p.Success)
                {
                    ds = p.Data;
                }
            });
            return ds;
        }

        public ActionResult FillUser(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_userandscreen.User.GetDDLUser(datastr, p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillTraderID(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_userandscreen.User.GetDDLTraderId(datastr, p =>
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

        public ActionResult FillInstrumentType(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_deal.RPDealEntry.GetDDLInstrumentType(datastr, p =>
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

        public ActionResult FillCounterParty(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_counterparty.CounterPartyFund.GetDDLCounterParty(datastr, p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}