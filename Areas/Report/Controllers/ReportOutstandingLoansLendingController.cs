using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Mvc;
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

namespace GM.Application.Web.Areas.Report.Controllers
{
    [Authorize]
    [Audit]
    public class ReportOutstandingLoansLendingController : BaseController
    {
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
                Report_Header = "Outstanding Loans Lending Report";

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

                if (collection["PDF"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);
                    var rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),
                        "OutstandingLoansLending.rpt"));
                    rd.SetDataSource(dt);
                    rd.SetParameterValue("asofdate", DateTime.Now.ToString("dd/MM/yyyy"));
                    rd.SetParameterValue("report_id", reportid);
                    rd.SetParameterValue("business_date", DateTime.Now);
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
                    var sheet = workbook.CreateSheet("OutstandingLoansLending");

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

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, "System : Repo");
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, "Report No." + reportid);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);

                    // Add Header Table
                    excelTemplate.CreateCellColHead(excelRow, 0, "Trans No");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Contract");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Trade Date");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Settlement Date");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Maturity Date");
                    excelTemplate.CreateCellColHead(excelRow, 5, "Period");
                    excelTemplate.CreateCellColHead(excelRow, 6, "Counterparty Code");
                    excelTemplate.CreateCellColHead(excelRow, 7, "Counterparty Name");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Inst");
                    excelTemplate.CreateCellColHead(excelRow, 9, "Type");
                    excelTemplate.CreateCellColHead(excelRow, 10, "Purchase Price");
                    excelTemplate.CreateCellColHead(excelRow, 11, "Policy Rate");
                    excelTemplate.CreateCellColHead(excelRow, 12, "Spread");
                    excelTemplate.CreateCellColHead(excelRow, 13, "Repo Rate");
                    excelTemplate.CreateCellColHead(excelRow, 14, "Cur");
                    excelTemplate.CreateCellColHead(excelRow, 15, "Int Amount");
                    excelTemplate.CreateCellColHead(excelRow, 16, "Tax Amount");
                    excelTemplate.CreateCellColHead(excelRow, 17, "Termination");
                    excelTemplate.CreateCellColHead(excelRow, 18, "Accru Interest");
                    excelTemplate.CreateCellColHead(excelRow, 19, "Port");
                    excelTemplate.CreateCellColHead(excelRow, 20, "Trans Status");

                    // Add Data Rows
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        rowIndex++;

                        excelRow = sheet.CreateRow(rowIndex);

                        excelTemplate.CreateCellColCenter(excelRow, 0, dt.Rows[i]["trans_no"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 1, dt.Rows[i]["contract"].ToString());

                        var trade_date = "";
                        if (dt.Rows[i]["trade_date"].ToString() != string.Empty)
                            trade_date = DateTime.Parse(dt.Rows[i]["trade_date"].ToString()).ToString("dd/MM/yyyy");
                        excelTemplate.CreateCellColCenter(excelRow, 2, trade_date);

                        var settlement_date = "";
                        if (dt.Rows[i]["settlement_date"].ToString() != string.Empty)
                            settlement_date = DateTime.Parse(dt.Rows[i]["settlement_date"].ToString())
                                .ToString("dd/MM/yyyy");
                        excelTemplate.CreateCellColCenter(excelRow, 3, settlement_date);

                        var maturity_date = "";
                        if (dt.Rows[i]["maturity_date"].ToString() != string.Empty)
                            maturity_date = DateTime.Parse(dt.Rows[i]["maturity_date"].ToString())
                                .ToString("dd/MM/yyyy");
                        excelTemplate.CreateCellColCenter(excelRow, 4, maturity_date);


                        var period = 0;
                        if (dt.Rows[i]["period"].ToString() != string.Empty)
                            period = int.Parse(dt.Rows[i]["period"].ToString());
                        excelTemplate.CreateCellColNumber(excelRow, 5, period);


                        excelTemplate.CreateCellColCenter(excelRow, 6, dt.Rows[i]["counterparty_code"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 7, dt.Rows[i]["counterparty_name"].ToString());

                        excelTemplate.CreateCellColCenter(excelRow, 8, dt.Rows[i]["instrument"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 9, dt.Rows[i]["instrument_type"].ToString());

                        double purchase_price = 0;
                        if (dt.Rows[i]["purchase_price"].ToString() != string.Empty)
                            purchase_price = double.Parse(dt.Rows[i]["purchase_price"].ToString());
                        excelTemplate.CreateCellCol2Decimal(excelRow, 10, purchase_price);

                        double policy_rate = 0;
                        if (dt.Rows[i]["policy_rate"].ToString() != string.Empty)
                            policy_rate = double.Parse(dt.Rows[i]["policy_rate"].ToString());
                        excelTemplate.CreateCellCol2Decimal(excelRow, 11, policy_rate);

                        double spread = 0;
                        if (dt.Rows[i]["spread"].ToString() != string.Empty)
                            spread = double.Parse(dt.Rows[i]["spread"].ToString());

                        excelTemplate.CreateCellCol2Decimal(excelRow, 12, spread);

                        double repo_rate = 0;
                        if (dt.Rows[i]["repo_rate"].ToString() != string.Empty)
                            repo_rate = double.Parse(dt.Rows[i]["repo_rate"].ToString());
                        excelTemplate.CreateCellCol2Decimal(excelRow, 13, repo_rate);

                        excelTemplate.CreateCellColCenter(excelRow, 14, dt.Rows[i]["cur"].ToString());

                        double int_amount = 0;
                        if (dt.Rows[i]["int_amount"].ToString() != string.Empty)
                            int_amount = double.Parse(dt.Rows[i]["int_amount"].ToString());
                        excelTemplate.CreateCellCol2Decimal(excelRow, 15, int_amount);

                        double tax_amount = 0;
                        if (dt.Rows[i]["tax_amount"].ToString() != string.Empty)
                            tax_amount = double.Parse(dt.Rows[i]["tax_amount"].ToString());
                        excelTemplate.CreateCellCol2Decimal(excelRow, 16, tax_amount);


                        excelTemplate.CreateCellColCenter(excelRow, 17, dt.Rows[i]["termination"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 18, dt.Rows[i]["accru_interest"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 19, dt.Rows[i]["port"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 20, dt.Rows[i]["trans_status"].ToString());
                    }

                    for (var i = 1; i <= 14; i++)
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

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 9;
                    endColumn = 9;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 10;
                    endColumn = 10;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 11;
                    endColumn = 11;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 12;
                    endColumn = 12;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 13;
                    endColumn = 13;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 14;
                    endColumn = 14;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 15;
                    endColumn = 15;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 16;
                    endColumn = 16;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 17;
                    endColumn = 17;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 18;
                    endColumn = 18;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 19;
                    endColumn = 19;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 20;
                    endColumn = 20;
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

            //SqlParameter outRefCode = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //SqlParameter outMessage = new SqlParameter("@Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };
            //SqlParameter outServerity = new SqlParameter("@Serverity", SqlDbType.VarChar, 15) { Direction = ParameterDirection.Output };
            //SqlParameter outHowManyRecord = new SqlParameter("@HowManyRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

            //dt = (new DbHelper()).GetTableRow("RP_Report_Outstanding_Loans_Lending_Proc", new[] { outRefCode, outMessage, outServerity, outHowManyRecord,
            //        new SqlParameter("@as_of_date_from", data.asofdate_from),
            //        new SqlParameter("@as_of_date_to", data.asofdate_to),
            //        new SqlParameter("@currency",  data.currency)
            //});

            apiReport.ReportData.OutstandingLoansLendingReport(data, p =>
            {
                if (p.Success) dt = p.Data.OutstandingLoansLendingReportResultModel.ToDataTable();
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