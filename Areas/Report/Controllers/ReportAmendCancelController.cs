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
    public class ReportAmendCancelController : BaseController  
    {
         private readonly StaticEntities api_static = new StaticEntitiesEntities();  
         private readonly ReportEntities apiReport = new ReportEntities();
         
    }
}



// ---------------------------------------------------------------------------------------------------------------------------------
{
    [Authorize]
    [Audit]
    public class ReportAmendCancelController : BaseController
    {
        private readonly StaticEntities api_static = new StaticEntities();
        private readonly ReportEntities apiReport = new ReportEntities();
        public string Report_Bank = "SiGG Financial Bank";
        public string Report_Code = string.Empty;
        public string Report_DateFromTo = string.Empty;
        public string Report_File_Name = string.Empty;
        public string Report_Header = "Amend and Cancel Deal Daily Report";

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
                var ds = new DataSet();
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
                Report_Name = "Amend Cancel Report";

                if (string.IsNullOrEmpty(reportCriteriaModel.asofdate_string))
                {
                    ViewBag.ErrorMessage = "Please select As Of Date.";
                    return View();
                }

                reportCriteriaModel.asofdate = string.IsNullOrEmpty(reportCriteriaModel.asofdate_string)
                    ? reportCriteriaModel.asofdate
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.asofdate_string);

                Report_DateFromTo = reportCriteriaModel.asofdate == null
                    ? ""
                    : "As Of Date " + reportCriteriaModel.asofdate.Value.ToString("dd/MM/yyyy");
                var businessdate = reportentity.Getbusinessdate();

                if (!string.IsNullOrEmpty(reportCriteriaModel.trans_status_name))
                {
                    Report_DateFromTo += $" Trans Status: {reportCriteriaModel.trans_status_name}";
                }
                if (!string.IsNullOrEmpty(reportCriteriaModel.cust_type_name))
                {
                    Report_DateFromTo += $" Customer Type: {reportCriteriaModel.cust_type_name}";
                }
                if (!string.IsNullOrEmpty(reportCriteriaModel.report_type_name))
                {
                    Report_DateFromTo += $" Report Type: {reportCriteriaModel.report_type_name}";
                }

                Report_Header += $" (Report No.{reportid})";

                if (collection["PDF"] != null) return View();

                if (collection["Excel"] != null)
                {
                    ds = GetReportData(reportCriteriaModel);

                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("AmendCancelReport");

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
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0,
                        $"System : Repo (Run date and time {DateTime.Now:dd/MM/yyyy HH:mm})");
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellColHead(excelRow, 0, ds.Tables[1].Rows[0]["cust_type"].ToString());

                    rowIndex++;
                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellColHead(excelRow, 0, "หัวข้อ");
                    excelTemplate.CreateCellColHead(excelRow, 1, "จำนวนรายการ");
                    //Add Summary 
                    for (var i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);
                        excelTemplate.CreateCellColCenter(excelRow, 0, ds.Tables[1].Rows[i]["data_type"].ToString());
                        excelTemplate.CreateCellColRight(excelRow, 1, ds.Tables[1].Rows[i]["record"].ToString());
                    }

                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellColHead(excelRow, 0, ds.Tables[2].Rows[0]["cust_type"].ToString());

                    rowIndex++;
                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellColHead(excelRow, 0, "หัวข้อ");
                    excelTemplate.CreateCellColHead(excelRow, 1, "จำนวนรายการ");

                    //Add Summary 
                    for (var i = 0; i < ds.Tables[2].Rows.Count; i++)
                    {
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);
                        excelTemplate.CreateCellColCenter(excelRow, 0, ds.Tables[2].Rows[i]["data_type"].ToString());
                        excelTemplate.CreateCellColRight(excelRow, 1, ds.Tables[2].Rows[i]["record"].ToString());
                    }

                    // Add Header Table
                    rowIndex++;
                    excelRow = sheet.CreateRow(rowIndex);

                    excelTemplate.CreateCellColHead(excelRow, 0, "Deal Status");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Customer Type");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Report Type");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Old Deal No.");
                    excelTemplate.CreateCellColHead(excelRow, 4, "New Deal No.");
                    excelTemplate.CreateCellColHead(excelRow, 5, "Portfolio");
                    excelTemplate.CreateCellColHead(excelRow, 6, "Deal Type");
                    excelTemplate.CreateCellColHead(excelRow, 7, "Deal Date");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Maturity Date");
                    excelTemplate.CreateCellColHead(excelRow, 9, "Capture Date");
                    excelTemplate.CreateCellColHead(excelRow, 10, "Cancel Amend Date");
                    excelTemplate.CreateCellColHead(excelRow, 11, "Counterparty Code");
                    excelTemplate.CreateCellColHead(excelRow, 12, "Counterparty Name");
                    excelTemplate.CreateCellColHead(excelRow, 13, "Commitment CCY");
                    excelTemplate.CreateCellColHead(excelRow, 14, "Commitment Amount");
                    excelTemplate.CreateCellColHead(excelRow, 15, "Counterparty CCY");
                    excelTemplate.CreateCellColHead(excelRow, 16, "Counterparty Amount");
                    excelTemplate.CreateCellColHead(excelRow, 17, "Dealer Name");
                    excelTemplate.CreateCellColHead(excelRow, 18, "Dealer Cancel Amend Name");
                    excelTemplate.CreateCellColHead(excelRow, 19, "Cause");

                    // Add Data Rows
                    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);
                        excelTemplate.CreateCellColCenter(excelRow, 0, ds.Tables[0].Rows[i]["deal_status"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 1, ds.Tables[0].Rows[i]["cust_type"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 2, ds.Tables[0].Rows[i]["report_type"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 3, ds.Tables[0].Rows[i]["old_deal_no"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 4, ds.Tables[0].Rows[i]["new_deal_no"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 5, ds.Tables[0].Rows[i]["portfolio"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 6, ds.Tables[0].Rows[i]["deal_type"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 7, utility.ConvertStringToDatetimeFormatDDMMYYYY(ds.Tables[0].Rows[i]["deal_date"].ToString()));
                        excelTemplate.CreateCellColCenter(excelRow, 8, utility.ConvertStringToDatetimeFormatDDMMYYYY(ds.Tables[0].Rows[i]["maturity_Date"].ToString()));
                        excelTemplate.CreateCellColCenter(excelRow, 9, utility.ConvertStringToDatetimeFormatDDMMYYYY(ds.Tables[0].Rows[i]["capture_date"].ToString()));
                        excelTemplate.CreateCellColCenter(excelRow, 10, utility.ConvertStringToDatetimeFormatDDMMYYYY(ds.Tables[0].Rows[i]["cancel_amend_date"].ToString()));
                        excelTemplate.CreateCellColCenter(excelRow, 11,
                            ds.Tables[0].Rows[i]["counter_party_code"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 12,
                            ds.Tables[0].Rows[i]["counter_party_name"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 13,
                            ds.Tables[0].Rows[i]["commitment_ccy"].ToString());

                        double commitment_amt = 0;
                        if (ds.Tables[0].Rows[i]["commitment_amt"].ToString() != string.Empty)
                            commitment_amt = double.Parse(ds.Tables[0].Rows[i]["commitment_amt"].ToString());
                        excelTemplate.CreateCellColNumber(excelRow, 14, commitment_amt);

                        excelTemplate.CreateCellColCenter(excelRow, 15,
                            ds.Tables[0].Rows[i]["counter_party_ccy"].ToString());

                        double counter_party_amt = 0;
                        if (ds.Tables[0].Rows[i]["counter_party_amt"].ToString() != string.Empty)
                            counter_party_amt = double.Parse(ds.Tables[0].Rows[i]["counter_party_amt"].ToString());
                        excelTemplate.CreateCellColNumber(excelRow, 16, counter_party_amt);
                        excelTemplate.CreateCellColCenter(excelRow, 17, ds.Tables[0].Rows[i]["dealer_name"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 18,
                            ds.Tables[0].Rows[i]["dealer_cancel_amend_name"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 19, ds.Tables[0].Rows[i]["remark"].ToString());
                    }

                    for (var i = 0; i <= 19; i++)
                    {
                        sheet.AutoSizeColumn(i);

                        if (i == 0)
                        {
                            sheet.SetColumnWidth(i, 10000);
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

                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 0, 0));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 1, 1));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 2, 2));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 3, 3));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 4, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 5, 5));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 6, 6));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 7, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 8, 8));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 9, 9));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 10, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 11, 11));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 12, 12));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 13, 13));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 14, 14));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 15, 15));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 16, 16));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 17, 17));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 18, 18));
                    sheet.AddMergedRegion(new CellRangeAddress(18, 18, 19, 19));

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
                return View();
            }
        }

        public DataSet GetReportData(ReportCriteriaModel data)
        {
            var ds = new DataSet();
            apiReport.ReportData.AmendCancelReport(data, p =>
            {
                if (p.Success) ds = p.Data;
            });
            return ds;
        }

        public ActionResult FillTransStatus()
        {
            var controller_name = ControllerContext.RouteData.Values["controller"].ToString();
            var res = new List<DDLItemModel>();
            api_static.Config.GetConfigReport(controller_name, "report_status", p =>
            {
                if (p.Success)
                    res = p.Data.DDLItems;
                else
                    ViewBag.ErrorMessage = p.Message;
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCustType()
        {
            var controller_name = ControllerContext.RouteData.Values["controller"].ToString();
            var res = new List<DDLItemModel>();
            api_static.Config.GetConfigReport(controller_name, "cust_type", p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillReportType()
        {
            var controller_name = ControllerContext.RouteData.Values["controller"].ToString();
            var res = new List<DDLItemModel>();
            api_static.Config.GetConfigReport(controller_name, "report_type", p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}