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
using GM.Data.Model.Static;
using GM.Filters;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;

namespace GM.Application.Web.Areas.Report.Controllers
{
    [Authorize]
    [Audit]
    public class ReportPaymentController : BaseController
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
                Report_Header = "Payment Report";

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

                Report_Header += $" (Report No.{reportid})";

                var businessdate = reportentity.Getbusinessdate();

                if (collection["PDF"] != null)
                {
                    dt = GetReportData(reportCriteriaModel, ref Error_code, ref Error_desc);
                    if (Error_code != 0)
                    {
                        ViewBag.ErrorMessage = Error_desc;
                        return View();
                    }

                    var rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/CrystalReportsFile"), "PaymentReport.rpt"));
                    rd.SetDataSource(dt);
                    rd.SetParameterValue("asofdate", Report_DateFromTo);
                    rd.SetParameterValue("report_id", reportid);
                    rd.SetParameterValue("business_date", DateTime.Now);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    var stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return new FileStreamResult(stream, "application/pdf");
                }

                if (collection["Excel"] != null)
                {
                    dt = GetReportData(reportCriteriaModel, ref Error_code, ref Error_desc);
                    if (Error_code != 0)
                    {
                        ViewBag.ErrorMessage = Error_desc;
                        return View();
                    }

                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("PaymentReport");

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

                    // Add Header Table
                    excelTemplate.CreateCellColHead(excelRow, 0, "Event");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Transaction");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Security Code");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Payment Date");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Portfolio");
                    excelTemplate.CreateCellColHead(excelRow, 5, "Payment mode");
                    excelTemplate.CreateCellColHead(excelRow, 6, "CPTY");
                    excelTemplate.CreateCellColHead(excelRow, 7, "CCY");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Amount");
                    excelTemplate.CreateCellColHead(excelRow, 9, "Status Unit");
                    excelTemplate.CreateCellColHead(excelRow, 10, "Interest Amount");
                    excelTemplate.CreateCellColHead(excelRow, 11, "Tax Amount");
                    excelTemplate.CreateCellColHead(excelRow, 12, "Exchange Rate");
                    excelTemplate.CreateCellColHead(excelRow, 13, "Absorbed Tax(%)");
                    excelTemplate.CreateCellColHead(excelRow, 14, "Tax in ThaiBath");
                    excelTemplate.CreateCellColHead(excelRow, 15, "Fee");

                    // Add Data Rows
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);

                        excelTemplate.CreateCellColCenter(excelRow, 0, dt.Rows[i]["event_type"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 1, dt.Rows[i]["trans_no"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 2, dt.Rows[i]["instrument_code"].ToString());

                        if (dt.Rows[i]["payment_date"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 3, DateTime.Parse(dt.Rows[i]["payment_date"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 3, new DateTime());
                        }

                        excelTemplate.CreateCellColCenter(excelRow, 4, dt.Rows[i]["book"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 5, dt.Rows[i]["payment_method"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 6, dt.Rows[i]["counterparty"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 7, dt.Rows[i]["cur"].ToString());

                        double amount = 0;
                        if (dt.Rows[i]["amount"].ToString() != string.Empty)
                            amount = double.Parse(dt.Rows[i]["amount"].ToString());

                        excelTemplate.CreateCellColDecimalBucket(excelRow, 8, amount, 2);

                        excelTemplate.CreateCellColCenter(excelRow, 9, dt.Rows[i]["status_unit"].ToString());

                        double interest_rate = 0;
                        if (dt.Rows[i]["int_amt"].ToString() != string.Empty)
                        {
                            interest_rate = double.Parse(dt.Rows[i]["int_amt"].ToString());
                            excelTemplate.CreateCellCol2Decimal(excelRow, 10, interest_rate);
                        }
                        else
                        {
                            excelTemplate.CreateCellCol2Decimal(excelRow, 10, "");
                        }

                        double tax_rate = 0;
                        if (dt.Rows[i]["tax_amt"].ToString() != string.Empty)
                        {
                            tax_rate = double.Parse(dt.Rows[i]["tax_amt"].ToString());
                            excelTemplate.CreateCellCol2Decimal(excelRow, 11, tax_rate);
                        }
                        else
                        {
                            excelTemplate.CreateCellCol2Decimal(excelRow, 11, "");
                        }

                        double exch_rate = 0;
                        if (dt.Rows[i]["exch_rate"].ToString() != string.Empty)
                        {
                            exch_rate = double.Parse(dt.Rows[i]["exch_rate"].ToString());
                            excelTemplate.CreateCellCol2Decimal(excelRow, 12, tax_rate);
                        }
                        else
                        {
                            excelTemplate.CreateCellCol2Decimal(excelRow, 12, "");
                        }

                        double abs_tax = 0;
                        if (dt.Rows[i]["abs_tax"].ToString() != string.Empty)
                        {
                            abs_tax = double.Parse(dt.Rows[i]["abs_tax"].ToString());
                            excelTemplate.CreateCellColRight(excelRow, 13, abs_tax.ToString("N2") + "%");
                        }
                        else
                        {
                            excelTemplate.CreateCellColRight(excelRow, 13, "");
                        }

                        double tax_thai_baht = 0;
                        if (dt.Rows[i]["tax_thai_baht"].ToString() != string.Empty)
                        {
                            tax_thai_baht = double.Parse(dt.Rows[i]["tax_thai_baht"].ToString());
                            excelTemplate.CreateCellCol2Decimal(excelRow, 14, tax_thai_baht);
                        }
                        else
                        {
                            excelTemplate.CreateCellCol2Decimal(excelRow, 14, "");
                        }

                        double fee_amount = 0;
                        if (dt.Rows[i]["fee_amount"].ToString() != string.Empty)
                        {
                            fee_amount = double.Parse(dt.Rows[i]["fee_amount"].ToString());
                            if (fee_amount > 0)
                            {
                                excelTemplate.CreateCellColDecimalBucket(excelRow, 15, fee_amount, 2);
                            }
                            else
                            {
                                excelTemplate.CreateCellCol2Decimal(excelRow, 15, "");
                            }
                        }
                        else
                        {
                            excelTemplate.CreateCellCol2Decimal(excelRow, 15, "");
                        }
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

                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 10));

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
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        public DataTable GetReportData(ReportCriteriaModel data, ref int ErrorCode, ref string ErrorDesc)
        {
            var dt = new DataTable();
            //SqlParameter outRefCode = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //SqlParameter outMessage = new SqlParameter("@Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };
            //SqlParameter outServerity = new SqlParameter("@Serverity", SqlDbType.VarChar, 15) { Direction = ParameterDirection.Output };
            //SqlParameter outHowManyRecord = new SqlParameter("@HowManyRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

            //dt = (new DbHelper()).GetTableRow("RP_Report_Payment_300003_Proc", new[] { outRefCode, outMessage, outServerity, outHowManyRecord,
            //        new SqlParameter("@asof_date_from", data.asofdate_from),
            //        new SqlParameter("@asof_date_to", data.asofdate_to),
            //        new SqlParameter("@cur",  data.currency)
            //});
            //ErrorCode = int.Parse(outRefCode.Value.ToString());
            //ErrorDesc = outMessage.Value.ToString();

            var tmpErrorCode = 0;
            var tmpErrorDesc = string.Empty;
            apiReport.ReportData.PaymentReport(data, p =>
            {
                if (p.Success) dt = p.Data.PaymentReportResultModel.ToDataTable();

                tmpErrorCode = p.RefCode;
                tmpErrorDesc = p.Message;
            });

            ErrorCode = tmpErrorCode;
            ErrorDesc = tmpErrorDesc;
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