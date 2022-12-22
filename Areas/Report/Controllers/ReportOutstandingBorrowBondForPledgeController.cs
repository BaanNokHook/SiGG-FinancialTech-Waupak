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
using System.Data;ss
using System.IO;
using System.Web.Mvc;

namespace GM.Application.Web.Areas.Report.Controllers
{
    [Authorize]
    [Audit]
    public class ReportOutstandingBorrowBondForPledgeController : BaseController
    {
        private SecurityEntities api_security = new SecurityEntities();
        private readonly ReportEntities apiReport = new ReportEntities();
        public string Report_Banak = "SiGG Financial Bank";
        public string Report_Code = string.Empty;
        public string Report_Date = string.Empty;
        public string Report_Header = string.Empty;

        public string Report_Name = string.Empty;
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
                var controller_name = ControllerContext.RouteData.Values["controller"].ToString();
                reportname_list = reportentity.Getreportname(controller_name);
                if (reportname_list.Count == 0)
                {
                    ViewBag.ErrorMessage =
                        "Can not Get Data Report ID from Service Static Method Config/getReportID and key = " +
                        controller_name + " in table gm_report !!!!";
                    throw new Exception(ViewBag.ErrorMessage);
                }

                reportid = reportname_list[0].Value.ToString();
                Report_Header = "Outstanding Borrow Bond For Pledge Repo Report";
                Report_Header += $" (Report No.{reportid})";
                Report_Name = "Outstanding Borrow Report";

                reportCriteriaModel.asofdate = string.IsNullOrEmpty(reportCriteriaModel.asofdate_string)
                    ? reportCriteriaModel.asofdate
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.asofdate_string);
                var businessdate = reportentity.Getbusinessdate();

                if (collection["PDF"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);
                    if (dt != null || dt.Rows.Count > 0)
                    {
                        var rd = new ReportDocument();
                        rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),
                            "OutstandingBorrowBornForPlegdeReport.rpt"));
                        rd.SetDataSource(dt);
                        rd.SetParameterValue("asofdate", reportCriteriaModel.asofdate_string);
                        rd.SetParameterValue("report_id", reportid);
                        Response.Buffer = false;
                        Response.ClearContent();
                        Response.ClearHeaders();
                        try
                        {
                            var stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                            stream.Seek(0, SeekOrigin.Begin);
                            return new FileStreamResult(stream, "application/pdf");
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                    ViewBag.ErrorMessage = "Cannot get data from resource.";
                    return View();
                }

                if (collection["Excel"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);

                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("OutstandingBorrowBornForPlegdeReport");

                    var excelTemplate = new ExcelTemplate(workbook);

                    // Add Header 
                    var rowIndex = 0;
                    var excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_Banak);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_Header);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0,
                        $"As of Date : {reportCriteriaModel.asofdate_string}");
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
                    excelTemplate.CreateCellColHead(excelRow, 0, "Bond Code");
                    excelTemplate.CreateCellColHead(excelRow, 1, "ISIN");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Par/Unit");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Borrow FITS");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Pledge");
                    excelTemplate.CreateCellColHead(excelRow, 5, "Non Pledge");
                    excelTemplate.CreateCellColHead(excelRow, 6, "Period");
                    excelTemplate.CreateCellColHead(excelRow, 7, "Port");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Type");

                    // Add Data Rows
                    if (dt.Rows.Count > 0)
                    {
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            rowIndex++;
                            excelRow = sheet.CreateRow(rowIndex);

                            excelTemplate.CreateCellColCenter(excelRow, 0, dt.Rows[i]["born_code"].ToString());
                            excelTemplate.CreateCellColCenter(excelRow, 1, dt.Rows[i]["isin_code"].ToString());

                            double par_unit = 0;
                            if (dt.Rows[i]["par_unit"].ToString() != string.Empty)
                                par_unit = double.Parse(dt.Rows[i]["par_unit"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 2, par_unit);

                            double borrow_fits = 0;
                            if (dt.Rows[i]["borrow_fits"].ToString() != string.Empty)
                                borrow_fits = double.Parse(dt.Rows[i]["borrow_fits"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 3, borrow_fits);

                            double pledge = 0;
                            if (dt.Rows[i]["pledge"].ToString() != string.Empty)
                                pledge = double.Parse(dt.Rows[i]["pledge"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 4, pledge);

                            double non_pledge = 0;
                            if (dt.Rows[i]["non_pledge"].ToString() != string.Empty)
                                non_pledge = double.Parse(dt.Rows[i]["non_pledge"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 5, non_pledge);

                            excelTemplate.CreateCellColNumber(excelRow, 6, dt.Rows[i]["period"].ToString());

                            excelTemplate.CreateCellColCenter(excelRow, 7, dt.Rows[i]["port"].ToString());
                            excelTemplate.CreateCellColCenter(excelRow, 8, dt.Rows[i]["type"].ToString());
                        }

                        #region :: Total Outstanding ::

                        var sum_borrow_fits = double.Parse(dt.Compute("Sum(borrow_fits)", string.Empty).ToString());
                        var sum_pledge = double.Parse(dt.Compute("Sum(pledge)", string.Empty).ToString());
                        var sum_non_pledge = double.Parse(dt.Compute("Sum(non_pledge)", string.Empty).ToString());

                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);
                        excelTemplate.CreateCellFooter(excelRow, 0, "Total");
                        sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 1, 2));
                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 3, sum_borrow_fits, 2);
                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 4, sum_pledge, 2);
                        excelTemplate.CreateCellFooterDecimalBucket(excelRow, 5, sum_non_pledge, 2);

                        #endregion
                    }

                    for (var i = 0; i <= 8; i++)
                    {
                        sheet.AutoSizeColumn(i);

                        if (i == 0)
                        {
                            sheet.SetColumnWidth(i, 4000);
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


                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 8));
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 8));
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 8));
                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 8));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 8));

                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 0, 0));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 1, 1));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 2, 2));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 3, 3));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 4, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 4, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 5, 5));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 6, 6));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 7, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 8, 8));

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
                ViewBag.ErrorMessage = ex.Message;
                return new EmptyResult();
            }
        }

        public DataTable GetReportData(ReportCriteriaModel data)
        {
            var dt = new DataTable();

            apiReport.ReportData.OutstandingBorrowBondForPledgeReport(data, p =>
            {
                if (p.Success) dt = p.Data.OutstandingBorrowBornForPlegdeResultModel.ToDataTable();
            });

            return dt;
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