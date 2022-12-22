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
    public class ReportOutstandingBilateralPrivateController : BaseController
    {
        private readonly StaticEntities api_static = new StaticEntities();
        private readonly ReportEntities apiReport = new ReportEntities();
        public string Report_Bank = "SiGG Financial Bank";
        public string Report_Code = string.Empty;
        public string Report_DateFromTo = string.Empty;
        public string Report_File_Name = string.Empty;
        public string Report_Header = string.Empty;

        public string Report_Name = string.Empty;
        private readonly ReportEntitiesModel reportentity = new ReportEntitiesModel();

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
                Report_File_Name = reportname_list[0].Text;
                Report_Name = "Outstanding Bilatereal Private Report";

                if (collection["PDF"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);
                    var rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),
                        "OutstandingBilateralPrivateReport.rpt"));
                    rd.SetDataSource(dt);
                    rd.SetParameterValue("asofdate", DateTime.Now);
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
                    var sheet = workbook.CreateSheet("OutstandingBilateralPrivateReport");

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
                    excelTemplate.CreateCellColHead(excelRow, 0, "No.");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Trans No.");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Contract No.");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Inst.");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Inst. Type");
                    excelTemplate.CreateCellColHead(excelRow, 5, "Counterparty Name");
                    excelTemplate.CreateCellColHead(excelRow, 6, "Trade Date");
                    excelTemplate.CreateCellColHead(excelRow, 7, "Settle. Date");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Mature Date");
                    excelTemplate.CreateCellColHead(excelRow, 9, "Period");
                    excelTemplate.CreateCellColHead(excelRow, 10, "Purchase Price");
                    excelTemplate.CreateCellColHead(excelRow, 11, "CCY");
                    excelTemplate.CreateCellColHead(excelRow, 12, "Policy Rate");
                    excelTemplate.CreateCellColHead(excelRow, 13, "Spread");
                    excelTemplate.CreateCellColHead(excelRow, 14, "Repo Rate");
                    excelTemplate.CreateCellColHead(excelRow, 15, "Collateral Details");
                    excelTemplate.CreateCellColHead(excelRow, 16, "Collateral Details");
                    excelTemplate.CreateCellColHead(excelRow, 17, "Collateral Details");
                    excelTemplate.CreateCellColHead(excelRow, 18, "Collateral Details");
                    excelTemplate.CreateCellColHead(excelRow, 19, "Collateral Details");
                    excelTemplate.CreateCellColHead(excelRow, 20, "Collateral Details");
                    excelTemplate.CreateCellColHead(excelRow, 21, "Collateral Details");
                    excelTemplate.CreateCellColHead(excelRow, 22, "Collateral Details");
                    excelTemplate.CreateCellColHead(excelRow, 23, "Collateral Details");
                    excelTemplate.CreateCellColHead(excelRow, 24, "Port");
                    excelTemplate.CreateCellColHead(excelRow, 25, "Current Status");

                    rowIndex++;
                    excelRow = sheet.CreateRow(rowIndex);

                    excelTemplate.CreateCellColHead(excelRow, 0, "No.");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Trans No.");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Contract No.");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Inst.");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Inst. Type");
                    excelTemplate.CreateCellColHead(excelRow, 5, "Counterparty Name");
                    excelTemplate.CreateCellColHead(excelRow, 6, "Trade Date");
                    excelTemplate.CreateCellColHead(excelRow, 7, "Settle. Date");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Mature Date");
                    excelTemplate.CreateCellColHead(excelRow, 9, "Period");
                    excelTemplate.CreateCellColHead(excelRow, 10, "Purchase Price");
                    excelTemplate.CreateCellColHead(excelRow, 11, "CCY");
                    excelTemplate.CreateCellColHead(excelRow, 12, "Policy Rate");
                    excelTemplate.CreateCellColHead(excelRow, 13, "Spread");
                    excelTemplate.CreateCellColHead(excelRow, 14, "Repo Rate");
                    excelTemplate.CreateCellColHead(excelRow, 15, "Sec Code");
                    excelTemplate.CreateCellColHead(excelRow, 16, "H");
                    excelTemplate.CreateCellColHead(excelRow, 17, "Unit");
                    excelTemplate.CreateCellColHead(excelRow, 18, "Par Value");
                    excelTemplate.CreateCellColHead(excelRow, 19, "Cash Amount");
                    excelTemplate.CreateCellColHead(excelRow, 20, "Market Value");
                    excelTemplate.CreateCellColHead(excelRow, 21, "Remark");
                    excelTemplate.CreateCellColHead(excelRow, 22, "Col");
                    excelTemplate.CreateCellColHead(excelRow, 23, "Sec");
                    excelTemplate.CreateCellColHead(excelRow, 24, "Port");
                    excelTemplate.CreateCellColHead(excelRow, 25, "Current Status");

                    // Add Data Rows
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);


                        excelTemplate.CreateCellColCenter(excelRow, 0, i + 1);
                        excelTemplate.CreateCellColCenter(excelRow, 1, dt.Rows[i]["trans_no"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 2, dt.Rows[i]["contract_no"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 3, dt.Rows[i]["instrument_code"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 4, dt.Rows[i]["instrument_type"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 5, dt.Rows[i]["counterparty_name"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 6, dt.Rows[i]["trade_date"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 7, dt.Rows[i]["settlement_date"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 8, dt.Rows[i]["maturity_date"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 9, dt.Rows[i]["period"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 10, dt.Rows[i]["purchase_price"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 11, dt.Rows[i]["cur"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 12, dt.Rows[i]["policy_rate"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 13, dt.Rows[i]["spread"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 14, dt.Rows[i]["repo_rate"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 15, dt.Rows[i]["sec_code_col"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 16, dt.Rows[i]["h_col"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 17, dt.Rows[i]["unit_col"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 18, dt.Rows[i]["par_value_col"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 19, dt.Rows[i]["cash_amount_col"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 20, dt.Rows[i]["market_value_col"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 21, dt.Rows[i]["reserve_col"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 22, dt.Rows[i]["col_col"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 23, dt.Rows[i]["sec_col"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 24, dt.Rows[i]["port"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 25, dt.Rows[i]["current_status"].ToString());
                    }


                    for (var i = 1; i <= 25; i++)
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
                    endRow = 6;
                    startColumn = 0;
                    endColumn = 0;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 6;
                    startColumn = 1;
                    endColumn = 1;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 6;
                    startColumn = 2;
                    endColumn = 2;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 6;
                    startColumn = 3;
                    endColumn = 3;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 4;
                    endColumn = 5;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 6;
                    endRow = 6;
                    startColumn = 4;
                    endColumn = 4;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 6;
                    endRow = 6;
                    startColumn = 5;
                    endColumn = 5;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 6;
                    startColumn = 6;
                    endColumn = 6;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 6;
                    startColumn = 7;
                    endColumn = 7;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 6;
                    startColumn = 8;
                    endColumn = 8;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 6;
                    startColumn = 9;
                    endColumn = 9;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 6;
                    startColumn = 10;
                    endColumn = 10;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 6;
                    startColumn = 11;
                    endColumn = 11;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 6;
                    startColumn = 12;
                    endColumn = 12;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 6;
                    startColumn = 13;
                    endColumn = 13;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 6;
                    startColumn = 14;
                    endColumn = 14;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 5;
                    startColumn = 15;
                    endColumn = 23;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 6;
                    endRow = 6;
                    startColumn = 15;
                    endColumn = 15;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 6;
                    endRow = 6;
                    startColumn = 16;
                    endColumn = 16;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 6;
                    endRow = 6;
                    startColumn = 17;
                    endColumn = 17;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 6;
                    endRow = 6;
                    startColumn = 18;
                    endColumn = 18;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 6;
                    endRow = 6;
                    startColumn = 19;
                    endColumn = 19;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 6;
                    endRow = 6;
                    startColumn = 20;
                    endColumn = 20;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 6;
                    endRow = 6;
                    startColumn = 21;
                    endColumn = 21;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 6;
                    endRow = 6;
                    startColumn = 22;
                    endColumn = 22;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 6;
                    endRow = 6;
                    startColumn = 23;
                    endColumn = 23;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 6;
                    startColumn = 24;
                    endColumn = 24;
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startColumn, endColumn));

                    // Set Merge Cells Header : columns
                    startRow = 5;
                    endRow = 6;
                    startColumn = 25;
                    endColumn = 25;
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
            //SqlParameter outRefCode = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //SqlParameter outMessage = new SqlParameter("@Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };
            //SqlParameter outServerity = new SqlParameter("@Serverity", SqlDbType.VarChar, 15) { Direction = ParameterDirection.Output };
            //SqlParameter outHowManyRecord = new SqlParameter("@HowManyRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

            //dt = (new DbHelper()).GetTableRow("RP_Report_Outstanding_Bilateral_Private_Proc", new[] { outRefCode, outMessage, outServerity, outHowManyRecord,
            //        new SqlParameter("@as_of_date_from", data.asofdate_from),
            //        new SqlParameter("@as_of_date_to", data.asofdate_to),
            //        new SqlParameter("@currency",  data.currency)
            //});

            apiReport.ReportData.OutstandingBilateralPrivateReport(data, p =>
            {
                if (p.Success) dt = p.Data.OutstandingBilateralPrivateReportResultModel.ToDataTable();
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