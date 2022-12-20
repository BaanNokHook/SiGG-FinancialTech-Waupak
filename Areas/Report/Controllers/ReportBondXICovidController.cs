using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Mvc;
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

namespace GM.Application.Web.Areas.Report.Controllers
{
    [Authorize]
    [Audit]
    public class ReportBondXICovidController : BaseController
    {
        private readonly CounterPartyEntities api_counterparty = new CounterPartyEntities();
        private readonly SecurityEntities api_security = new SecurityEntities();
        private readonly StaticEntities api_static = new StaticEntities();
        private readonly ReportEntities apiReport = new ReportEntities();
        private readonly ReportEntitiesModel reportentity = new ReportEntitiesModel();
        private readonly Utility utility = new Utility();

        public string Report_Bank = "SiGG Financial Bank";
        public string Report_Code = string.Empty;
        public string Report_File_Name = string.Empty;
        public string Report_Header = string.Empty;
        public string Report_Name = string.Empty;
        public string Report_Port = string.Empty;

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(ReportCriteriaModel reportCriteriaModel, FormCollection collection)
        {
            var dt = new DataTable();
            var reportname_list = new List<DDLItemModel>();
            string reportid;
            var report_search_creteria = "";

            try
            {
                var controller_name = ControllerContext.RouteData.Values["controller"].ToString();
                reportname_list = reportentity.Getreportname(controller_name);
                reportid = reportname_list[0].Value.ToString();
                Report_Header = "Bond XI Covid Report";
                Report_File_Name = reportname_list[0].Text;

                reportCriteriaModel.xi_date = string.IsNullOrEmpty(reportCriteriaModel.xi_date_string)
                    ? reportCriteriaModel.xi_date
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.xi_date_string);
                reportCriteriaModel.payment_date = string.IsNullOrEmpty(reportCriteriaModel.payment_date_string)
                    ? reportCriteriaModel.payment_date
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.payment_date_string);

                reportCriteriaModel.asofdate_from = string.IsNullOrEmpty(reportCriteriaModel.asofdate_from_string)
                    ? reportCriteriaModel.asofdate_from
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.asofdate_from_string);
                reportCriteriaModel.asofdate_to = string.IsNullOrEmpty(reportCriteriaModel.asofdate_to_string)
                    ? reportCriteriaModel.asofdate_to
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.asofdate_to_string);


                if (reportname_list.Count == 0)
                    throw new Exception(
                        $"Can not Get Data Report ID from Service Static Method Config/getReportID and key = {controller_name} in table gm_report !!!!");

                if (string.IsNullOrEmpty(reportCriteriaModel.xi_date_string) &&
                    string.IsNullOrEmpty(reportCriteriaModel.payment_date_string) &&
                    (string.IsNullOrEmpty(reportCriteriaModel.asofdate_from_string) ||
                    string.IsNullOrEmpty(reportCriteriaModel.asofdate_to_string)))
                    throw new Exception("Please specify date field for searching.");

                if (!string.IsNullOrEmpty(reportCriteriaModel.xi_date_string))
                    report_search_creteria += $"XI Date: {reportCriteriaModel.xi_date_string} ";
                if (!string.IsNullOrEmpty(reportCriteriaModel.payment_date_string))
                    report_search_creteria += $"Payment Date: {reportCriteriaModel.payment_date_string} ";

                report_search_creteria += (reportCriteriaModel.asofdate_from != null || reportCriteriaModel.asofdate_to != null ? "As Of Date " : "") +
                                  (reportCriteriaModel.asofdate_from == null ? "" : reportCriteriaModel.asofdate_from.Value.ToString("dd/MM/yyyy")) +
                                  (reportCriteriaModel.asofdate_from != null && reportCriteriaModel.asofdate_to != null ? " - " : "") + 
                                  (reportCriteriaModel.asofdate_to == null ? "" : reportCriteriaModel.asofdate_to.Value.ToString("dd/MM/yyyy"));


                if (!string.IsNullOrEmpty(reportCriteriaModel.counterparty_code_name))
                    report_search_creteria += $"Counter Party: {reportCriteriaModel.counterparty_code_name} ";
                if (!string.IsNullOrEmpty(reportCriteriaModel.currency))
                    report_search_creteria += $"Currency: {reportCriteriaModel.currency} ";
                if (!string.IsNullOrEmpty(reportCriteriaModel.instrument_id))
                    report_search_creteria += $"Security: {reportCriteriaModel.instrument_code_name} ";

                Report_Header += $" (Report No.{reportid})";

                if (collection["Excel"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);

                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("BondXICovid");

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
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, report_search_creteria);
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

                    // Add Header Table
                    excelTemplate.CreateCellColHead(excelRow, 0, "No.");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Trans No.");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Trade Date");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Settlement Date");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Maturity Date");
                    excelTemplate.CreateCellColHead(excelRow, 5, "Payment Date");
                    excelTemplate.CreateCellColHead(excelRow, 6, "Face Amount");
                    excelTemplate.CreateCellColHead(excelRow, 7, "Send Data Date");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Book Closing Date");
                    excelTemplate.CreateCellColHead(excelRow, 9, "Security Symbol");
                    excelTemplate.CreateCellColHead(excelRow, 10, "ISIN");
                    excelTemplate.CreateCellColHead(excelRow, 11, "Cpty");
                    excelTemplate.CreateCellColHead(excelRow, 12, "Cpty Book Closed");
                    excelTemplate.CreateCellColHead(excelRow, 13, "Port");
                    excelTemplate.CreateCellColHead(excelRow, 14, "Lend/Borrow");
                    excelTemplate.CreateCellColHead(excelRow, 15, "Coupon %");
                    excelTemplate.CreateCellColHead(excelRow, 16, "Day/Month");
                    excelTemplate.CreateCellColHead(excelRow, 17, "Interest");
                    excelTemplate.CreateCellColHead(excelRow, 18, "Tax (WHT 1%)");
                    excelTemplate.CreateCellColHead(excelRow, 19, "Net Interest");


                    // Add Data Rows
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);

                        excelTemplate.CreateCellColRight(excelRow, 0, dt.Rows[i]["NO"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 1, dt.Rows[i]["TransNo"].ToString());

                        if (dt.Rows[i]["trade_date"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 2, DateTime.Parse(dt.Rows[i]["trade_date"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 2, new DateTime());
                        }

                        if (dt.Rows[i]["settlement_date"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 3, DateTime.Parse(dt.Rows[i]["settlement_date"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 3, new DateTime());
                        }

                        if (dt.Rows[i]["maturity_date"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 4, DateTime.Parse(dt.Rows[i]["maturity_date"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 4, new DateTime());
                        }

                        if (dt.Rows[i]["Payment_Date"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 5, DateTime.Parse(dt.Rows[i]["Payment_Date"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 5, new DateTime());
                        }

                        double _FaceAmount = 0;
                        if (dt.Rows[i]["FaceAmount"].ToString() != string.Empty)
                            _FaceAmount = double.Parse(dt.Rows[i]["FaceAmount"].ToString());
                        excelTemplate.CreateCellCol2Decimal(excelRow, 6, _FaceAmount);

                        if (dt.Rows[i]["SendData_Date"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 7, DateTime.Parse(dt.Rows[i]["SendData_Date"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 7, new DateTime());
                        }

                        if (dt.Rows[i]["BookClosing_Date"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 8, DateTime.Parse(dt.Rows[i]["BookClosing_Date"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 8, new DateTime());
                        }

                        excelTemplate.CreateCellColLeft(excelRow, 9, dt.Rows[i]["InstrumentName"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 10, dt.Rows[i]["isin_code"].ToString());
                        excelTemplate.CreateCellColNumber(excelRow, 11, dt.Rows[i]["CounterParty"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 12, dt.Rows[i]["BookClosed"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 13, dt.Rows[i]["Port"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 14, dt.Rows[i]["Lend_Borrow"].ToString());

                        double _Coupon = 0;
                        if (dt.Rows[i]["Coupon"].ToString() != string.Empty)
                            _Coupon = double.Parse(dt.Rows[i]["Coupon"].ToString());
                        excelTemplate.CreateCellCol6Decimal(excelRow, 15, _Coupon);

                        excelTemplate.CreateCellColNumber(excelRow, 16, dt.Rows[i]["Day_month"].ToString());

                        double _Interest = 0;
                        if (dt.Rows[i]["Interest"].ToString() != string.Empty)
                            _Interest = double.Parse(dt.Rows[i]["Interest"].ToString());
                        excelTemplate.CreateCellCol2Decimal(excelRow, 17, _Interest);

                        double _Wht = 0;
                        if (dt.Rows[i]["Wht"].ToString() != string.Empty)
                            _Wht = double.Parse(dt.Rows[i]["Wht"].ToString());
                        excelTemplate.CreateCellCol2Decimal(excelRow, 18, _Wht);

                        double _NetInterest = 0;
                        if (dt.Rows[i]["NetInterest"].ToString() != string.Empty)
                            _NetInterest = double.Parse(dt.Rows[i]["NetInterest"].ToString());
                        excelTemplate.CreateCellCol2Decimal(excelRow, 19, _NetInterest);
                    }

                    rowIndex++;

                    for (var i = 1; i <= 19; i++)
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
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 7));

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
                    //return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View();
        }

        public DataTable GetReportData(ReportCriteriaModel data)
        {
            var dt = new DataTable();
            apiReport.ReportData.BondXICovidReport(data, p =>
            {
                if (p.Success) dt = p.Data.BondXICovidReportResultModel.ToDataTable();
            });
            return dt;
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

        public ActionResult FillCurrency(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_static.Currency.GetDDLCurrency(datastr, p =>
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

        public ActionResult GetBusinessDate()
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