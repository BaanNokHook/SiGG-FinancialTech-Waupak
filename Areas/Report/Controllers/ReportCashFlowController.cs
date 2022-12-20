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
    public class ReportCashFlowController : BaseController
    {
        private readonly RPTransEntity api_deal = new RPTransEntity();
        private readonly StaticEntities api_static = new StaticEntities();
        private UserAndScreenEntities api_userandscreen = new UserAndScreenEntities();
        private readonly ReportEntities apiReport = new ReportEntities();
        public string Report_Bank = "SiGG Financial Bank";
        public string Report_Code = string.Empty;
        public string Report_DateFromTo = string.Empty;
        public string Report_Header = string.Empty;

        public string Report_Name = "CashFlowReport";
        public string Report_Port = string.Empty;
        private readonly ReportEntitiesModel reportentity = new ReportEntitiesModel();
        private readonly Utility utility = new Utility();

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            ReportCriteriaModel model = new ReportCriteriaModel();

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
                Report_Header = "Cash Flow Report : ธุรกรรม " + (reportCriteriaModel.repo_deal_type == null
                                    ? "Bilateral Repo & Private Repo"
                                    : reportCriteriaModel.repo_deal_type_name);
                Report_Header += $" (Report No.{reportid})";

                if (string.IsNullOrEmpty(reportCriteriaModel.asofdate_from_string))
                {
                    ViewBag.ErrorMessage = "Please select As Of Date From";
                    return View();
                }

                reportCriteriaModel.asofdate_from = string.IsNullOrEmpty(reportCriteriaModel.asofdate_from_string)
                    ? reportCriteriaModel.asofdate_from
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.asofdate_from_string);
                reportCriteriaModel.asofdate_to = string.IsNullOrEmpty(reportCriteriaModel.asofdate_to_string)
                    ? reportCriteriaModel.asofdate_to
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.asofdate_to_string);
                Report_Port = string.IsNullOrEmpty(reportCriteriaModel.port)
                    ? "Port : ALL"
                    : "Port : " + reportCriteriaModel.port_name;

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

                // Add Currency
                Report_DateFromTo += ", Currency : " + reportCriteriaModel.currency;

                var businessdate = reportentity.Getbusinessdate();

                if (collection["PDF"] != null)
                {
                    //dt = GetReportData(reportCriteriaModel);
                    //ReportDocument rd = new ReportDocument();
                    //rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"), "ProfitAndLossReport.rpt"));
                    //rd.SetDataSource(dt);
                    //rd.SetParameterValue("asofdate", DateTime.Now.ToString("dd/MM/yyyy"));
                    //rd.SetParameterValue("report_id", reportid);
                    //rd.SetParameterValue("business_date", DateTime.Now);
                    //Response.Buffer = false;
                    //Response.ClearContent();
                    //Response.ClearHeaders();
                    //try
                    //{
                    //    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //    stream.Seek(0, SeekOrigin.Begin);
                    //    return new FileStreamResult(stream, "application/pdf");
                    //    //return File(stream, "application/pdf", "SettlementProductControlReport.pdf");
                    //}
                    //catch (Exception)
                    //{
                    //    throw;
                    //}
                }
                else if (collection["Excel"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);
                    if (dt.Rows.Count > 0 && dt.Rows[0].ItemArray.Length == 6)
                    {
                        ViewBag.ErrorMessage = "ErrorProcedure " + dt.Rows[0][4] + ": " + dt.Rows[0][5];
                        return View();
                    }

                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("Cash Flow");

                    var excelTemplate = new ExcelTemplate(workbook, colFooterColor: 22, colBorderColor: 8);

                    var colCenter = excelTemplate.CellStyle(verticalAlignment: VerticalAlignment.Center,
                        horizontalAlignment: HorizontalAlignment.Center, borderColor: 8, foregroundColor: 9);

                    var colNumber = excelTemplate.CellStyle(horizontalAlignment: HorizontalAlignment.Right,
                        verticalAlignment: VerticalAlignment.Center,
                        borderColor: 8, foregroundColor: 9, dataFormat: "#,##0.00_);[Red](#,##0.00)");

                    var colTotalPRP = excelTemplate.CellStyle(horizontalAlignment: HorizontalAlignment.Left,
                        verticalAlignment: VerticalAlignment.Center,
                        borderColor: 8, foregroundColor: 49, dataFormat: "#,##0.00_);[Red](#,##0.00)");
                    var colTotalBRP = excelTemplate.CellStyle(horizontalAlignment: HorizontalAlignment.Left,
                        verticalAlignment: VerticalAlignment.Center,
                        borderColor: 8, foregroundColor: 44, dataFormat: "#,##0.00_);[Red](#,##0.00)");
                    var colTotalColl = excelTemplate.CellStyle(horizontalAlignment: HorizontalAlignment.Left,
                        verticalAlignment: VerticalAlignment.Center,
                        borderColor: 8, foregroundColor: 45, dataFormat: "#,##0.00_);[Red](#,##0.00)");

                    var colNumPRP = excelTemplate.CellStyle(horizontalAlignment: HorizontalAlignment.Right,
                        verticalAlignment: VerticalAlignment.Center,
                        borderColor: 8, foregroundColor: 49, dataFormat: "#,##0.00_);[Red](#,##0.00)");
                    var colNumBRP = excelTemplate.CellStyle(horizontalAlignment: HorizontalAlignment.Right,
                        verticalAlignment: VerticalAlignment.Center,
                        borderColor: 8, foregroundColor: 44, dataFormat: "#,##0.00_);[Red](#,##0.00)");
                    var colNumColl = excelTemplate.CellStyle(horizontalAlignment: HorizontalAlignment.Right,
                        verticalAlignment: VerticalAlignment.Center,
                        borderColor: 8, foregroundColor: 45, dataFormat: "#,##0.00_);[Red](#,##0.00)");

                    var colFooter = excelTemplate.CellStyle(fontBoldWeight: FontBoldWeight.Bold, horizontalAlignment: HorizontalAlignment.Right,
                        verticalAlignment: VerticalAlignment.Center,
                        borderColor: 8, foregroundColor: 22, dataFormat: "#,##0.00_);[Red](#,##0.00)");

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

                    sheet.CreateRow(rowIndex);
                    //do something

                    if (reportCriteriaModel.currency != "THB")
                    {
                        #region Set Left Template

                        if (string.IsNullOrEmpty(reportCriteriaModel.repo_deal_type))
                        {
                            var rowForThis = rowIndex + 1;
                            //Row Temple Format And Data
                            //Start IN
                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "BRP", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Borrowing");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "BRP", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Repay Lending");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "BRP", colCenter);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "Total IN BRP", colTotalBRP);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Private-Repo", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Borrowing");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Private-Repo", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Repay Lending");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Private-Repo", colCenter);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "Total IN Private-Repo", colTotalPRP);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Margin", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Margin Receive");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Margin", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Interest Receive on Margin Pay");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Margin", colCenter);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "Total IN From Margin", colTotalColl);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellFooterRight(excelRow, 1, "Total IN");
                            excelTemplate.CreateCellFooterRight(excelRow, 2, "Total IN");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;
                            //End IN
                            //Start Out
                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "BRP", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Lending");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "BRP", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Repay Borrowing");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "BRP", colCenter);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "Total OUT BRP", colTotalBRP);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Private-Repo", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Lending");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Private-Repo", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Repay Borrowing");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Private-Repo", colCenter);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "Total OUT Private-Repo", colTotalPRP);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Margin", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Margin Pay");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Margin", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Interest Pay on Margin Receive");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Margin", colCenter);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "Total OUT From Margin", colTotalColl);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellFooterRight(excelRow, 1, "Total OUT");
                            excelTemplate.CreateCellFooterRight(excelRow, 2, "Total OUT");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;
                            //End OUT

                            //NET Layo out
                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellCustomStyle(excelRow, 0, "PRP-NET POSITION", colTotalPRP);
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "PRP-NET POSITION", colTotalPRP);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "PRP-NET POSITION", colTotalPRP);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellCustomStyle(excelRow, 0, "BRP-NET POSITION", colTotalBRP);
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "BRP-NET POSITION", colTotalBRP);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "BRP-NET POSITION", colTotalBRP);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellCustomStyle(excelRow, 0, "Coll-NET POSITION", colTotalColl);
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Coll-NET POSITION", colTotalColl);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "Coll-NET POSITION", colTotalColl);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellFooterRight(excelRow, 0, "Net Total");
                            excelTemplate.CreateCellFooterRight(excelRow, 1, "Net Total");
                            excelTemplate.CreateCellFooterRight(excelRow, 2, "Net Total");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowIndex = rowForThis++;
                        }
                        else
                        {
                            var rowForThis = rowIndex + 1;
                            var repo_deal_type_name = reportCriteriaModel.repo_deal_type == "PRP" ? "Private-Repo" : "BRP";
                            //Start IN
                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, repo_deal_type_name, colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Borrowing");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, repo_deal_type_name, colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Repay Lending");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, repo_deal_type_name, colCenter);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, string.Format("Total IN {0}", repo_deal_type_name),
                                reportCriteriaModel.repo_deal_type == "PRP" ? colTotalPRP : colTotalBRP);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Margin", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Margin Receive");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Margin", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Interest Receive on Margin Pay");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Margin", colCenter);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "Total IN From Margin", colTotalColl);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellFooterRight(excelRow, 1, "Total IN");
                            excelTemplate.CreateCellFooterRight(excelRow, 2, "Total IN");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;
                            //END IN
                            //Start Out
                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, repo_deal_type_name, colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Lending");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, repo_deal_type_name, colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Repay Borrowing");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, repo_deal_type_name, colCenter);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, string.Format("Total OUT {0}", repo_deal_type_name),
                                reportCriteriaModel.repo_deal_type == "PRP" ? colTotalPRP : colTotalBRP);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Margin", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Margin Pay");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Margin", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Interest Pay on Margin Receive");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Margin", colCenter);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "Total OUT From Margin", colTotalColl);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellFooterRight(excelRow, 1, "Total OUT");
                            excelTemplate.CreateCellFooterRight(excelRow, 2, "Total OUT");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            //End OUT

                            if (reportCriteriaModel.repo_deal_type == "PRP")
                            {
                                excelRow = sheet.CreateRow(rowForThis);
                                excelTemplate.CreateCellCustomStyle(excelRow, 0, "PRP-NET POSITION", colTotalPRP);
                                excelTemplate.CreateCellCustomStyle(excelRow, 1, "PRP-NET POSITION", colTotalPRP);
                                excelTemplate.CreateCellCustomStyle(excelRow, 2, "PRP-NET POSITION", colTotalPRP);
                                rowForThis++;
                            }
                            else
                            {
                                excelRow = sheet.CreateRow(rowForThis);
                                excelTemplate.CreateCellCustomStyle(excelRow, 0, "BRP-NET POSITION", colTotalBRP);
                                excelTemplate.CreateCellCustomStyle(excelRow, 1, "BRP-NET POSITION", colTotalBRP);
                                excelTemplate.CreateCellCustomStyle(excelRow, 2, "BRP-NET POSITION", colTotalBRP);
                                rowForThis++;
                            }
                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellCustomStyle(excelRow, 0, "Coll-NET POSITION", colTotalColl);
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Coll-NET POSITION", colTotalColl);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "Coll-NET POSITION", colTotalColl);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellFooterRight(excelRow, 0, "Net Total");
                            excelTemplate.CreateCellFooterRight(excelRow, 1, "Net Total");
                            excelTemplate.CreateCellFooterRight(excelRow, 2, "Net Total");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowIndex = rowForThis++;
                        }

                        #endregion

                        string endLetter = "";

                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            var row = rowIndex;
                            var col = i + 3;
                            var colLetter = excelTemplate.GetExcelColumnName(col + 1);
                            endLetter = colLetter;

                            sheet.SetColumnWidth(i + 3, 5000);

                            if (string.IsNullOrEmpty(reportCriteriaModel.repo_deal_type))
                            {
                                // for all repo deal type
                                excelRow = sheet.GetRow(6);
                                excelTemplate.CreateCellColHead(excelRow, col, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["cashflow_date"].ToString()));

                                double in_bilet_brw = 0;
                                if (dt.Rows[i]["in_bilet_brw"].ToString() != string.Empty)
                                    in_bilet_brw = double.Parse(dt.Rows[i]["in_bilet_brw"].ToString());

                                excelRow = sheet.GetRow(7);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_bilet_brw, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double in_bilet_led = 0;
                                if (dt.Rows[i]["in_bilet_led"].ToString() != string.Empty)
                                    in_bilet_led = double.Parse(dt.Rows[i]["in_bilet_led"].ToString());

                                excelRow = sheet.GetRow(8);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_bilet_led, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(9);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumBRP);
                                excelRow.GetCell(col).SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 8, colLetter, 9));

                                double in_private_brw = 0;
                                if (dt.Rows[i]["in_private_brw"].ToString() != string.Empty)
                                    in_private_brw = double.Parse(dt.Rows[i]["in_private_brw"].ToString());

                                excelRow = sheet.GetRow(10);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_private_brw, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double in_private_led = 0;
                                if (dt.Rows[i]["in_private_led"].ToString() != string.Empty)
                                    in_private_led = double.Parse(dt.Rows[i]["in_private_led"].ToString());

                                excelRow = sheet.GetRow(11);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_private_led, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(12);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumPRP);
                                excelRow.GetCell(col).SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 11, colLetter, 12));

                                double in_col_margin_receive = 0;
                                if (dt.Rows[i]["in_col_margin_receive"].ToString() != string.Empty)
                                    in_col_margin_receive = double.Parse(dt.Rows[i]["in_col_margin_receive"].ToString());

                                excelRow = sheet.GetRow(13);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_col_margin_receive, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double in_col_interest_receive = 0;
                                if (dt.Rows[i]["in_col_interest_receive"].ToString() != string.Empty)
                                    in_col_interest_receive = double.Parse(dt.Rows[i]["in_col_interest_receive"].ToString());

                                excelRow = sheet.GetRow(14);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_col_interest_receive, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(15);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumColl);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 14, colLetter, 15));

                                excelRow = sheet.GetRow(16);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}+{4}{5}", colLetter, 10, colLetter, 13, colLetter, 16));

                                double out_bilet_led = 0;
                                if (dt.Rows[i]["out_bilet_led"].ToString() != string.Empty)
                                    out_bilet_led = double.Parse(dt.Rows[i]["out_bilet_led"].ToString());

                                excelRow = sheet.GetRow(17);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_bilet_led, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double out_bilet_brw = 0;
                                if (dt.Rows[i]["out_bilet_brw"].ToString() != string.Empty)
                                    out_bilet_brw = double.Parse(dt.Rows[i]["out_bilet_brw"].ToString());

                                excelRow = sheet.GetRow(18);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_bilet_brw, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(19);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumBRP);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 18, colLetter, 19));

                                double out_private_led = 0;
                                if (dt.Rows[i]["out_private_led"].ToString() != string.Empty)
                                    out_private_led = double.Parse(dt.Rows[i]["out_private_led"].ToString());

                                excelRow = sheet.GetRow(20);

                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_private_led, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double out_private_brw = 0;
                                if (dt.Rows[i]["out_private_brw"].ToString() != string.Empty)
                                    out_private_brw = double.Parse(dt.Rows[i]["out_private_brw"].ToString());

                                excelRow = sheet.GetRow(21);

                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_private_brw, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(22);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumPRP);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 21, colLetter, 22));

                                double out_col_margin_pay = 0;
                                if (dt.Rows[i]["out_col_margin_pay"].ToString() != string.Empty)
                                    out_col_margin_pay = double.Parse(dt.Rows[i]["out_col_margin_pay"].ToString());

                                excelRow = sheet.GetRow(23);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_col_margin_pay, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double out_col_interest_pay = 0;
                                if (dt.Rows[i]["out_col_interest_pay"].ToString() != string.Empty)
                                    out_col_interest_pay = double.Parse(dt.Rows[i]["out_col_interest_pay"].ToString());

                                excelRow = sheet.GetRow(24);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_col_interest_pay, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(25);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumColl);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 24, colLetter, 25));

                                excelRow = sheet.GetRow(26);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}+{4}{5}", colLetter, 20, colLetter, 23, colLetter, 26));

                                excelRow = sheet.GetRow(27);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumPRP);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}-{2}{3}", colLetter, 13, colLetter, 23));

                                excelRow = sheet.GetRow(28);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumBRP);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}-{2}{3}", colLetter, 10, colLetter, 20));

                                excelRow = sheet.GetRow(29);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumColl);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}-{2}{3}", colLetter, 16, colLetter, 26));

                                excelRow = sheet.GetRow(30);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}-{2}{3}", colLetter, 17, colLetter, 27));
                            }
                            else if (reportCriteriaModel.repo_deal_type.ToUpper() == "PRP")
                            {
                                //for private type
                                excelRow = sheet.GetRow(6);
                                excelTemplate.CreateCellColHead(excelRow, col, dt.Rows[i]["cashflow_date"].ToString());

                                double in_private_brw = 0;
                                if (dt.Rows[i]["in_private_brw"].ToString() != string.Empty)
                                    in_private_brw = double.Parse(dt.Rows[i]["in_private_brw"].ToString());

                                excelRow = sheet.GetRow(7);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_private_brw, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double in_private_led = 0;
                                if (dt.Rows[i]["in_private_led"].ToString() != string.Empty)
                                    in_private_led = double.Parse(dt.Rows[i]["in_private_led"].ToString());

                                excelRow = sheet.GetRow(8);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_private_led, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(9);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumPRP);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 8, colLetter, 9));

                                double in_col_margin_receive = 0;
                                if (dt.Rows[i]["in_col_margin_receive"].ToString() != string.Empty)
                                    in_col_margin_receive = double.Parse(dt.Rows[i]["in_col_margin_receive"].ToString());

                                excelRow = sheet.GetRow(10);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_col_margin_receive, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double in_col_interest_receive = 0;
                                if (dt.Rows[i]["in_col_interest_receive"].ToString() != string.Empty)
                                    in_col_interest_receive = double.Parse(dt.Rows[i]["in_col_interest_receive"].ToString());

                                excelRow = sheet.GetRow(11);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_col_interest_receive, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(12);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumColl);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 11, colLetter, 12));

                                excelRow = sheet.GetRow(13);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 10, colLetter, 13));

                                double out_private_led = 0;
                                if (dt.Rows[i]["out_private_led"].ToString() != string.Empty)
                                    out_private_led = double.Parse(dt.Rows[i]["out_private_led"].ToString());

                                excelRow = sheet.GetRow(14);

                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_private_led, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double out_private_brw = 0;
                                if (dt.Rows[i]["out_private_brw"].ToString() != string.Empty)
                                    out_private_brw = double.Parse(dt.Rows[i]["out_private_brw"].ToString());

                                excelRow = sheet.GetRow(15);

                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_private_brw, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(16);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumPRP);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 15, colLetter, 16));

                                double out_col_margin_pay = 0;
                                if (dt.Rows[i]["out_col_margin_pay"].ToString() != string.Empty)
                                    out_col_margin_pay = double.Parse(dt.Rows[i]["out_col_margin_pay"].ToString());

                                excelRow = sheet.GetRow(17);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_col_margin_pay, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double out_col_interest_pay = 0;
                                if (dt.Rows[i]["out_col_interest_pay"].ToString() != string.Empty)
                                    out_col_interest_pay = double.Parse(dt.Rows[i]["out_col_interest_pay"].ToString());

                                excelRow = sheet.GetRow(18);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_col_interest_pay, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(19);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumColl);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 18, colLetter, 19));

                                excelRow = sheet.GetRow(20);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 17, colLetter, 20));

                                excelRow = sheet.GetRow(21);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumPRP);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}-{2}{3}", colLetter, 10, colLetter, 17));

                                excelRow = sheet.GetRow(22);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumColl);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}-{2}{3}", colLetter, 13, colLetter, 20));

                                excelRow = sheet.GetRow(23);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}-{2}{3}", colLetter, 14, colLetter, 21));
                            }
                            else if (reportCriteriaModel.repo_deal_type.ToUpper() == "BRP")
                            {
                                excelRow = sheet.GetRow(6);
                                excelTemplate.CreateCellColHead(excelRow, col, dt.Rows[i]["cashflow_date"].ToString());

                                double in_bilet_brw = 0;
                                if (dt.Rows[i]["in_bilet_brw"].ToString() != string.Empty)
                                    in_bilet_brw = double.Parse(dt.Rows[i]["in_bilet_brw"].ToString());

                                excelRow = sheet.GetRow(7);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_bilet_brw, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double in_bilet_led = 0;
                                if (dt.Rows[i]["in_bilet_led"].ToString() != string.Empty)
                                    in_bilet_led = double.Parse(dt.Rows[i]["in_bilet_led"].ToString());

                                excelRow = sheet.GetRow(8);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_bilet_led, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(9);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumBRP);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 8, colLetter, 9));

                                double in_col_margin_receive = 0;
                                if (dt.Rows[i]["in_col_margin_receive"].ToString() != string.Empty)
                                    in_col_margin_receive = double.Parse(dt.Rows[i]["in_col_margin_receive"].ToString());

                                excelRow = sheet.GetRow(10);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_col_margin_receive, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double in_col_interest_receive = 0;
                                if (dt.Rows[i]["in_col_interest_receive"].ToString() != string.Empty)
                                    in_col_interest_receive = double.Parse(dt.Rows[i]["in_col_interest_receive"].ToString());

                                excelRow = sheet.GetRow(11);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_col_interest_receive, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(12);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumColl);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 11, colLetter, 12));

                                excelRow = sheet.GetRow(13);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 10, colLetter, 13));

                                double out_bilet_led = 0;
                                if (dt.Rows[i]["out_bilet_led"].ToString() != string.Empty)
                                    out_bilet_led = double.Parse(dt.Rows[i]["out_bilet_led"].ToString());

                                excelRow = sheet.GetRow(14);

                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_bilet_led, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double out_bilet_brw = 0;
                                if (dt.Rows[i]["out_bilet_brw"].ToString() != string.Empty)
                                    out_bilet_brw = double.Parse(dt.Rows[i]["out_bilet_brw"].ToString());

                                excelRow = sheet.GetRow(15);

                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_bilet_brw, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(16);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumBRP);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 15, colLetter, 16));

                                double out_col_margin_pay = 0;
                                if (dt.Rows[i]["out_col_margin_pay"].ToString() != string.Empty)
                                    out_col_margin_pay = double.Parse(dt.Rows[i]["out_col_margin_pay"].ToString());

                                excelRow = sheet.GetRow(17);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_col_margin_pay, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double out_col_interest_pay = 0;
                                if (dt.Rows[i]["out_col_interest_pay"].ToString() != string.Empty)
                                    out_col_interest_pay = double.Parse(dt.Rows[i]["out_col_interest_pay"].ToString());

                                excelRow = sheet.GetRow(18);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_col_interest_pay, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(19);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumColl);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 18, colLetter, 19));

                                excelRow = sheet.GetRow(20);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 17, colLetter, 20));

                                excelRow = sheet.GetRow(21);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumBRP);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}-{2}{3}", colLetter, 10, colLetter, 17));

                                excelRow = sheet.GetRow(22);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumColl);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}-{2}{3}", colLetter, 13, colLetter, 20));

                                excelRow = sheet.GetRow(23);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}-{2}{3}", colLetter, 14, colLetter, 21));
                            }
                        }

                        #region Merge Cell 

                        sheet.SetColumnWidth(1, 3500);
                        sheet.SetColumnWidth(2, 7000);

                        sheet.GetRow(6).HeightInPoints = 19;
                        sheet.GetRow(7).HeightInPoints = 19;
                        sheet.GetRow(8).HeightInPoints = 19;
                        sheet.GetRow(9).HeightInPoints = 19;
                        sheet.GetRow(10).HeightInPoints = 19;
                        sheet.GetRow(11).HeightInPoints = 19;
                        sheet.GetRow(12).HeightInPoints = 19;
                        sheet.GetRow(13).HeightInPoints = 19;
                        sheet.GetRow(14).HeightInPoints = 19;
                        sheet.GetRow(15).HeightInPoints = 19;

                        sheet.GetRow(16).HeightInPoints = 19;
                        sheet.GetRow(17).HeightInPoints = 19;
                        sheet.GetRow(18).HeightInPoints = 19;
                        sheet.GetRow(19).HeightInPoints = 19;
                        sheet.GetRow(20).HeightInPoints = 19;
                        sheet.GetRow(21).HeightInPoints = 19;
                        sheet.GetRow(22).HeightInPoints = 19;
                        sheet.GetRow(23).HeightInPoints = 19;
                        //sheet.GetRow(24).HeightInPoints = 19;
                        //sheet.GetRow(25).HeightInPoints = 19;

                        if (string.IsNullOrEmpty(reportCriteriaModel.repo_deal_type))
                        {
                            sheet.GetRow(24).HeightInPoints = 19;
                            sheet.GetRow(25).HeightInPoints = 19;
                            sheet.GetRow(26).HeightInPoints = 19;
                            sheet.GetRow(27).HeightInPoints = 19;
                            sheet.GetRow(28).HeightInPoints = 19;
                            sheet.GetRow(29).HeightInPoints = 19;
                            sheet.GetRow(30).HeightInPoints = 19;
                            //sheet.GetRow(31).HeightInPoints = 19;
                            //sheet.GetRow(32).HeightInPoints = 19;
                        }

                        // Set Merge Cells Header Report_Bank
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 5, 0, 10));

                        if (string.IsNullOrEmpty(reportCriteriaModel.repo_deal_type))
                        {

                            //for all
                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(7, 16, 0, 0));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(7, 9, 1, 1));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(10, 12, 1, 1));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(13, 15, 1, 1));

                            //Merge Rows Data Format Report Row Total In
                            sheet.AddMergedRegion(new CellRangeAddress(16, 16, 1, 2));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(17, 26, 0, 0));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(17, 19, 1, 1));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(20, 22, 1, 1));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(23, 25, 1, 1));

                            //Merge Rows Data Format Report Row Total In
                            sheet.AddMergedRegion(new CellRangeAddress(26, 26, 1, 2));

                            //Merge Rows Data Format Report Row Net PRP-NET POSITION
                            sheet.AddMergedRegion(new CellRangeAddress(27, 27, 0, 2));

                            //Merge Rows Data Format Report Row Net BRP-NET POSITION
                            sheet.AddMergedRegion(new CellRangeAddress(28, 28, 0, 2));

                            //Merge Rows Data Format Report Row Net Coll-NET POSITION
                            sheet.AddMergedRegion(new CellRangeAddress(29, 29, 0, 2));

                            //Merge Rows Data Format Report Row Net TOTAL
                            sheet.AddMergedRegion(new CellRangeAddress(30, 30, 0, 2));
                        }
                        else
                        {
                            // For only Repo Deal Type
                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(7, 13, 0, 0));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(7, 9, 1, 1));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(10, 12, 1, 1));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(14, 20, 0, 0));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(14, 16, 1, 1));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(17, 19, 1, 1));

                            //Merge Rows Data Format Report Row Total In
                            sheet.AddMergedRegion(new CellRangeAddress(13, 13, 1, 2));

                            //Merge Rows Data Format Report Row Total Out
                            sheet.AddMergedRegion(new CellRangeAddress(20, 20, 1, 2));

                            //Merge Rows Data Format Report Row Net Type
                            sheet.AddMergedRegion(new CellRangeAddress(21, 21, 0, 2));

                            //Merge Rows Data Format Report Row Net Coll
                            sheet.AddMergedRegion(new CellRangeAddress(22, 22, 0, 2));

                            //Merge Rows Data Format Report Row Net TOTAL
                            sheet.AddMergedRegion(new CellRangeAddress(23, 23, 0, 2));
                        }

                        #endregion

                    }
                    else
                    {
                        #region Set Left Template

                        if (string.IsNullOrEmpty(reportCriteriaModel.repo_deal_type))
                        {
                            var rowForThis = rowIndex + 1;
                            //Row Temple Format And Data
                            //Start IN
                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "BRP", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Borrowing");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "BRP", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Repay Lending");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "BRP", colCenter);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "Total IN BRP", colTotalBRP);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Private-Repo", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Borrowing");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Private-Repo", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Repay Lending");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Private-Repo", colCenter);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "Total IN Private-Repo", colTotalPRP);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellFooterRight(excelRow, 1, "Total IN");
                            excelTemplate.CreateCellFooterRight(excelRow, 2, "Total IN");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;
                            //End IN
                            //Start Out
                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "BRP", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Lending");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "BRP", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Repay Borrowing");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "BRP", colCenter);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "Total OUT BRP", colTotalBRP);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Private-Repo", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Lending");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Private-Repo", colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Repay Borrowing");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Private-Repo", colCenter);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "Total OUT Private-Repo", colTotalPRP);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellFooterRight(excelRow, 1, "Total OUT");
                            excelTemplate.CreateCellFooterRight(excelRow, 2, "Total OUT");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;
                            //End OUT

                            //NET Layo out
                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellCustomStyle(excelRow, 0, "PRP-NET POSITION", colTotalPRP);
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "PRP-NET POSITION", colTotalPRP);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "PRP-NET POSITION", colTotalPRP);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellCustomStyle(excelRow, 0, "BRP-NET POSITION", colTotalBRP);
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "BRP-NET POSITION", colTotalBRP);
                            excelTemplate.CreateCellCustomStyle(excelRow, 2, "BRP-NET POSITION", colTotalBRP);
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellFooterRight(excelRow, 0, "Net Total");
                            excelTemplate.CreateCellFooterRight(excelRow, 1, "Net Total");
                            excelTemplate.CreateCellFooterRight(excelRow, 2, "Net Total");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowIndex = rowForThis++;
                        }
                        else
                        {
                            var rowForThis = rowIndex + 1;
                            var repo_deal_type_name = reportCriteriaModel.repo_deal_type == "PRP" ? "Private-Repo" : "BRP";
                            //Start IN
                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, repo_deal_type_name, colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Borrowing");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, repo_deal_type_name, colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Repay Lending");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, repo_deal_type_name, colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "");
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "IN");
                            excelTemplate.CreateCellFooterRight(excelRow, 1, "Total IN");
                            excelTemplate.CreateCellFooterRight(excelRow, 2, "Total IN");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;
                            //END IN
                            //Start Out
                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, repo_deal_type_name, colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Lending");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, repo_deal_type_name, colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "Repay Borrowing");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, repo_deal_type_name, colCenter);
                            excelTemplate.CreateCellColLeft(excelRow, 2, "");
                            rowForThis++;

                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellColHead(excelRow, 0, "OUT");
                            excelTemplate.CreateCellFooterRight(excelRow, 1, "Total OUT");
                            excelTemplate.CreateCellFooterRight(excelRow, 2, "Total OUT");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowForThis++;

                            //End OUT
                            excelRow = sheet.CreateRow(rowForThis);
                            excelTemplate.CreateCellFooterRight(excelRow, 0, "Net Total");
                            excelTemplate.CreateCellFooterRight(excelRow, 1, "Net Total");
                            excelTemplate.CreateCellFooterRight(excelRow, 2, "Net Total");
                            excelRow.GetCell(2).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                            rowIndex = rowForThis++;
                        }

                        #endregion

                        string endLetter = "";

                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            var row = rowIndex;
                            var col = i + 3;
                            var colLetter = excelTemplate.GetExcelColumnName(col + 1);
                            endLetter = colLetter;

                            sheet.SetColumnWidth(i + 3, 5000);

                            if (string.IsNullOrEmpty(reportCriteriaModel.repo_deal_type))
                            {
                                // for all repo deal type
                                excelRow = sheet.GetRow(6);

                                excelTemplate.CreateCellColHead(excelRow, col, utility.ConvertStringToDatetimeFormatDDMMYYYY(dt.Rows[i]["cashflow_date"].ToString()));

                                double in_bilet_brw = 0;
                                if (dt.Rows[i]["in_bilet_brw"].ToString() != string.Empty)
                                    in_bilet_brw = double.Parse(dt.Rows[i]["in_bilet_brw"].ToString());

                                in_bilet_brw = in_bilet_brw / 1000000;

                                excelRow = sheet.GetRow(7);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_bilet_brw, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double in_bilet_led = 0;
                                if (dt.Rows[i]["in_bilet_led"].ToString() != string.Empty)
                                    in_bilet_led = double.Parse(dt.Rows[i]["in_bilet_led"].ToString());

                                in_bilet_led = in_bilet_led / 1000000;

                                excelRow = sheet.GetRow(8);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_bilet_led, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(9);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumBRP);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 8, colLetter, 9));

                                double in_private_brw = 0;
                                if (dt.Rows[i]["in_private_brw"].ToString() != string.Empty)
                                    in_private_brw = double.Parse(dt.Rows[i]["in_private_brw"].ToString());

                                in_private_brw = in_private_brw / 1000000;

                                excelRow = sheet.GetRow(10);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_private_brw, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double in_private_led = 0;
                                if (dt.Rows[i]["in_private_led"].ToString() != string.Empty)
                                    in_private_led = double.Parse(dt.Rows[i]["in_private_led"].ToString());

                                in_private_led = in_private_led / 1000000;

                                excelRow = sheet.GetRow(11);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_private_led, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(12);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumPRP);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 11, colLetter, 12));

                                excelRow = sheet.GetRow(13);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 10, colLetter, 13));

                                double out_bilet_led = 0;
                                if (dt.Rows[i]["out_bilet_led"].ToString() != string.Empty)
                                    out_bilet_led = double.Parse(dt.Rows[i]["out_bilet_led"].ToString());

                                out_bilet_led = out_bilet_led / 1000000;

                                excelRow = sheet.GetRow(14);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_bilet_led, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double out_bilet_brw = 0;
                                if (dt.Rows[i]["out_bilet_brw"].ToString() != string.Empty)
                                    out_bilet_brw = double.Parse(dt.Rows[i]["out_bilet_brw"].ToString());

                                out_bilet_brw = out_bilet_brw / 1000000;

                                excelRow = sheet.GetRow(15);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_bilet_brw, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(16);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumBRP);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 15, colLetter, 16));

                                double out_private_led = 0;
                                if (dt.Rows[i]["out_private_led"].ToString() != string.Empty)
                                    out_private_led = double.Parse(dt.Rows[i]["out_private_led"].ToString());

                                out_private_led = out_private_led / 1000000;

                                excelRow = sheet.GetRow(17);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_private_led, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double out_private_brw = 0;
                                if (dt.Rows[i]["out_private_brw"].ToString() != string.Empty)
                                    out_private_brw = double.Parse(dt.Rows[i]["out_private_brw"].ToString());

                                out_private_brw = out_private_brw / 1000000;

                                excelRow = sheet.GetRow(18);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_private_brw, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(19);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumPRP);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 18, colLetter, 19));

                                excelRow = sheet.GetRow(20);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 17, colLetter, 20));

                                excelRow = sheet.GetRow(21);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumPRP);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}-{2}{3}", colLetter, 13, colLetter, 20));

                                excelRow = sheet.GetRow(22);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colNumBRP);
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}-{2}{3}", colLetter, 10, colLetter, 17));

                                excelRow = sheet.GetRow(23);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}-{2}{3}", colLetter, 14, colLetter, 21));
                            }
                            else if (reportCriteriaModel.repo_deal_type.ToUpper() == "PRP")
                            {
                                //for private type
                                excelRow = sheet.GetRow(6);
                                excelTemplate.CreateCellColHead(excelRow, col, dt.Rows[i]["cashflow_date"].ToString());

                                double in_private_brw = 0;
                                if (dt.Rows[i]["in_private_brw"].ToString() != string.Empty)
                                    in_private_brw = double.Parse(dt.Rows[i]["in_private_brw"].ToString());

                                in_private_brw = in_private_brw / 1000000;


                                excelRow = sheet.GetRow(7);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_private_brw, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double in_private_led = 0;
                                if (dt.Rows[i]["in_private_led"].ToString() != string.Empty)
                                    in_private_led = double.Parse(dt.Rows[i]["in_private_led"].ToString());

                                in_private_led = in_private_led / 1000000;

                                excelRow = sheet.GetRow(8);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_private_led, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(9);
                                excelTemplate.CreateCellColLeft(excelRow, col, "");

                                excelRow = sheet.GetRow(10);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 8, colLetter, 9));

                                double out_private_led = 0;
                                if (dt.Rows[i]["out_private_led"].ToString() != string.Empty)
                                    out_private_led = double.Parse(dt.Rows[i]["out_private_led"].ToString());

                                out_private_led = out_private_led / 1000000;

                                excelRow = sheet.GetRow(11);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_private_led, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double out_private_brw = 0;
                                if (dt.Rows[i]["out_private_brw"].ToString() != string.Empty)
                                    out_private_brw = double.Parse(dt.Rows[i]["out_private_brw"].ToString());

                                out_private_brw = out_private_brw / 1000000;

                                excelRow = sheet.GetRow(12);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_private_brw, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(13);
                                excelTemplate.CreateCellColLeft(excelRow, col, "");

                                excelRow = sheet.GetRow(14);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 12, colLetter, 13));

                                excelRow = sheet.GetRow(15);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}-{2}{3}", colLetter, 11, colLetter, 15));
                            }
                            else if (reportCriteriaModel.repo_deal_type.ToUpper() == "BRP")
                            {
                                excelRow = sheet.GetRow(6);
                                excelTemplate.CreateCellColHead(excelRow, col, dt.Rows[i]["cashflow_date"].ToString());

                                double in_bilet_brw = 0;
                                if (dt.Rows[i]["in_bilet_brw"].ToString() != string.Empty)
                                    in_bilet_brw = double.Parse(dt.Rows[i]["in_bilet_brw"].ToString());

                                in_bilet_brw = in_bilet_brw / 1000000;

                                excelRow = sheet.GetRow(7);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_bilet_brw, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double in_bilet_led = 0;
                                if (dt.Rows[i]["in_bilet_led"].ToString() != string.Empty)
                                    in_bilet_led = double.Parse(dt.Rows[i]["in_bilet_led"].ToString());

                                in_bilet_led = in_bilet_led / 1000000;

                                excelRow = sheet.GetRow(8);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, in_bilet_led, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(9);
                                excelTemplate.CreateCellColLeft(excelRow, col, "");

                                excelRow = sheet.GetRow(10);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 8, colLetter, 9));

                                double out_bilet_led = 0;
                                if (dt.Rows[i]["out_bilet_led"].ToString() != string.Empty)
                                    out_bilet_led = double.Parse(dt.Rows[i]["out_bilet_led"].ToString());

                                if (reportCriteriaModel.currency == "THB")
                                {
                                    out_bilet_led = out_bilet_led / 1000000;
                                }

                                excelRow = sheet.GetRow(11);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_bilet_led, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                double out_bilet_brw = 0;
                                if (dt.Rows[i]["out_bilet_brw"].ToString() != string.Empty)
                                    out_bilet_brw = double.Parse(dt.Rows[i]["out_bilet_brw"].ToString());

                                if (reportCriteriaModel.currency == "THB")
                                {
                                    out_bilet_brw = out_bilet_brw / 1000000;
                                }

                                excelRow = sheet.GetRow(12);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, out_bilet_brw, colNumber);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;

                                excelRow = sheet.GetRow(13);
                                excelTemplate.CreateCellColLeft(excelRow, col, "");

                                excelRow = sheet.GetRow(14);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}+{2}{3}", colLetter, 12, colLetter, 13));

                                excelRow = sheet.GetRow(15);
                                excelTemplate.CreateCellCustomStyle(excelRow, col, 0, colFooter);
                                excelRow.GetCell(col).CellStyle.VerticalAlignment = VerticalAlignment.Center;
                                excelRow.GetCell(col)
                                    .SetCellFormula(string.Format("{0}{1}-{2}{3}", colLetter, 11, colLetter, 15));
                            }
                        }

                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);
                        excelTemplate.CreateCellHeaderLeft(excelRow, 0, "remark: หน่วยของเงินเป็นหลักล้าน");

                        #region Merge Cell 

                        sheet.SetColumnWidth(1, 3500);
                        sheet.SetColumnWidth(2, 6000);


                        sheet.GetRow(6).HeightInPoints = 19;
                        sheet.GetRow(7).HeightInPoints = 19;
                        sheet.GetRow(8).HeightInPoints = 19;
                        sheet.GetRow(9).HeightInPoints = 19;
                        sheet.GetRow(10).HeightInPoints = 19;
                        sheet.GetRow(11).HeightInPoints = 19;
                        sheet.GetRow(12).HeightInPoints = 19;
                        sheet.GetRow(13).HeightInPoints = 19;
                        sheet.GetRow(14).HeightInPoints = 19;
                        sheet.GetRow(15).HeightInPoints = 19;

                        if (string.IsNullOrEmpty(reportCriteriaModel.repo_deal_type))
                        {
                            sheet.GetRow(16).HeightInPoints = 19;
                            sheet.GetRow(17).HeightInPoints = 19;
                            sheet.GetRow(18).HeightInPoints = 19;
                            sheet.GetRow(19).HeightInPoints = 19;
                            sheet.GetRow(20).HeightInPoints = 19;
                            sheet.GetRow(21).HeightInPoints = 19;
                            sheet.GetRow(22).HeightInPoints = 19;
                            sheet.GetRow(23).HeightInPoints = 19;
                        }

                        // Set Merge Cells Header Report_Bank
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 10));
                        sheet.AddMergedRegion(new CellRangeAddress(5, 5, 0, 10));

                        if (string.IsNullOrEmpty(reportCriteriaModel.repo_deal_type))
                        {
                            //for all
                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(7, 13, 0, 0));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(7, 9, 1, 1));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(10, 12, 1, 1));

                            //Merge Rows Data Format Report Row Total In
                            sheet.AddMergedRegion(new CellRangeAddress(13, 13, 1, 2));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(14, 20, 0, 0));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(14, 16, 1, 1));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(17, 19, 1, 1));

                            //Merge Rows Data Format Report Row Total In
                            sheet.AddMergedRegion(new CellRangeAddress(20, 20, 1, 2));

                            //Merge Rows Data Format Report Row Net PRP-NET POSITION
                            sheet.AddMergedRegion(new CellRangeAddress(21, 21, 0, 2));

                            //Merge Rows Data Format Report Row Net BRP-NET POSITION
                            sheet.AddMergedRegion(new CellRangeAddress(22, 22, 0, 2));

                            //Merge Rows Data Format Report Row Net TOTAL
                            sheet.AddMergedRegion(new CellRangeAddress(23, 23, 0, 2));
                        }
                        else
                        {
                            // For only Repo Deal Type
                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(7, 10, 0, 0));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(7, 9, 1, 1));

                            //Merge Rows Data Format Report
                            sheet.AddMergedRegion(new CellRangeAddress(11, 14, 0, 0));

                            //Merge Rows Data Format Report Row Total In
                            sheet.AddMergedRegion(new CellRangeAddress(11, 13, 1, 1));

                            //Merge Rows Data Format Report Row Total In
                            sheet.AddMergedRegion(new CellRangeAddress(10, 10, 1, 2));

                            //Merge Rows Data Format Report Row Total In
                            sheet.AddMergedRegion(new CellRangeAddress(14, 14, 1, 2));

                            //Merge Rows Data Format Report Row Net PRP-NET POSITION
                            sheet.AddMergedRegion(new CellRangeAddress(15, 15, 0, 2));
                        }

                        #endregion

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
                    Response.AppendHeader("Content-disposition", "attachment; filename=" + Report_Name + ".xls");

                    Response.BinaryWrite(exportfile.GetBuffer());
                    System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();

                    return View();
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }

            return View();
        }

        public DataTable GetReportData(ReportCriteriaModel data)
        {
            var dt = new DataTable();
            apiReport.ReportData.CashFlowReport(data, p =>
            {
                if (p.Success && p.Data != null && p.Data.CashFlowReportResultModel != null)
                    dt = p.Data.CashFlowReportResultModel.ToDataTable();
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

        public ActionResult FillPort(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_static.Config.GetPortForReportDDL(datastr, p =>
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
    }
}