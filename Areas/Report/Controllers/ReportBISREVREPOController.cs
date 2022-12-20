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
    public class ReportBISREVREPOController : BaseController
    {
        private readonly ReportEntities apiReport = new ReportEntities();
        private readonly StaticEntities apiStatic = new StaticEntities();
        public string Report_Bank = "SiGG Financial Bank";
        public string Report_Code = string.Empty;
        public string Report_DateFromTo = string.Empty;
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
                Report_Name = "BISREVREPOReport.xls";
                if (reportname_list.Count == 0)
                {
                    ViewBag.ErrorMessage =
                        "Can not Get Data Report ID from Service Static Method Config/getReportID and key = " +
                        controller_name + " in table gm_report !!!!";
                    return View();
                }

                reportid = reportname_list[0].Value.ToString();
                Report_Header = "BISREVREPO Report";

                if (string.IsNullOrEmpty(reportCriteriaModel.asofdate_string))
                {
                    ViewBag.ErrorMessage = "Please select As Of Date From";
                    return View();
                }

                reportCriteriaModel.asofdate = string.IsNullOrEmpty(reportCriteriaModel.asofdate_string)
                    ? reportCriteriaModel.asofdate
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.asofdate_string);

                Report_DateFromTo = (reportCriteriaModel.asofdate != null ? "As Of Date " : "") +
                                    (reportCriteriaModel.asofdate == null
                                        ? ""
                                        : reportCriteriaModel.asofdate.Value.ToString("dd/MM/yyyy"));

                Report_Header += $" (Report No.{reportid})";

                var businessdate = reportentity.Getbusinessdate();

                if (collection["PDF"] != null) return View();

                if (collection["Excel"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);

                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("BISREVREPO");

                    var excelTemplate = new ExcelTemplate(workbook);

                    var transNo = "";

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

                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 0, "CIF");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 1, "Deal No.");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 2, "Customer Code");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 3, "ชื่อลูกหนี้");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 4, "Exposure Group ของลูกหนี้");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 5, "สกุลเงินของยอดหนี้");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 6, "ยอดหนี้ (ยอดทำ Reverse Repo)");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 7, "ยอด Cash Margin");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 8, "ยอดดอกเบี้ยค้างรับ");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 9, "ยอดดอกเบี้ยค้างรับของ Cash Margin");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 10, "Banking Book หรือ Trading Book");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 11, "ประเทศลูกหนี้");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 12, "สกุลเงินของประเทศลูกหนี้");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 13, "Rating ของประเทศลูกหนี้");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 14, "Trade Date");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 15, "Maturity Date");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 16, "ชื่อย่อของตราสารที่ลูกหนี้นำมาวางเป็นประกัน");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 17, "ชื่อเต็มของตราสารที่ลูกหนี้นำมาวางเป็นประกัน");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 18, "สกุลเงินของตราสาร");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 19, "ยอด Outstanding ของตราสาร Face Value");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 20, "มูลค่าของตราสารที่ลูกหนี้ได้นำมาวางเป็นประกัน");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 21, "Exposure Group ของตราสารที่ลูกหนี้ได้นำมาวางเป็นประกัน");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 22, "Rating ของตราสาร");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 23, "สถาบันจัดอันดับ Rating ของตราสาร");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 24, "Rating ของผู้ออกตราสาร");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 25, "สถาบันจัดอันดับ Rating ของผู้ออกตราสาร");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 26, "ประเทศของผู้ออกตราสาร");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 27, "Rating ของประเทศผู้ออกตราสาร");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 28, "Core Market Participant");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 29, "BOT’s Criteria");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 30, "DATA DATE");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 31, "Bond Rating Assessment Date");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 32, "Issuer Rating Assessment Date");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 33, "Credit Mitigant Adjusted Flag");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 34, "Principal Only Flag");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 35, "Bond Type");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 36, "Bond Issuer");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 37, "Effective Maturity Day Number");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 38, "GL Account");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 39, "Internal Organization");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 40, "Portfolio");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 41, "Netting Set ID");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 42, "In Default Flag");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 43, "High Risk Category Flag");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 44, "Collateral Type");

                    // Add Data Rows
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);

                        excelTemplate.CreateCellColLeft(excelRow, 0, dt.Rows[i]["CNTR_SWIFT_CODE"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 1, dt.Rows[i]["TRANS_NO"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 2, dt.Rows[i]["CNTR_CODE"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 3, dt.Rows[i]["CNTR_NAME"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 4, dt.Rows[i]["CNTR_EXP_GRP"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 5, dt.Rows[i]["CUR"].ToString());

                        double PurchaseAmt = 0;
                        if (dt.Rows[i]["BOND_CASH_AMT"].ToString() != string.Empty)
                            PurchaseAmt = double.Parse(dt.Rows[i]["BOND_CASH_AMT"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 6, PurchaseAmt);

                        double CashMargin = 0;
                        if (dt.Rows[i]["CASH_MARGIN"].ToString() != string.Empty)
                            CashMargin = double.Parse(dt.Rows[i]["CASH_MARGIN"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 7, CashMargin);

                        if (transNo != dt.Rows[i]["TRANS_NO"].ToString())
                        {
                            double InterestAmt = 0;
                            if (dt.Rows[i]["INT_AMT"].ToString() != string.Empty)
                                InterestAmt = double.Parse(dt.Rows[i]["INT_AMT"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 8, InterestAmt);

                            transNo = dt.Rows[i]["TRANS_NO"].ToString();
                        }
                        else 
                        {
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 8, 0);
                        }

                        double InterestAmtMargin = 0;
                        if (dt.Rows[i]["INT_CASH_MARGIN"].ToString() != string.Empty)
                            InterestAmtMargin = double.Parse(dt.Rows[i]["INT_CASH_MARGIN"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 9, InterestAmtMargin);

                        excelTemplate.CreateCellColCenter(excelRow, 10, dt.Rows[i]["PORT_CODE"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 11, dt.Rows[i]["CNTR_COUNTRY"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 12, dt.Rows[i]["CNTR_COUNTRY_CUR"].ToString());

                        excelTemplate.CreateCellColCenter(excelRow, 13, dt.Rows[i]["CNTR_COUNTRY_RATING"].ToString());

                        if (dt.Rows[i]["TRADE_DATE"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 14, DateTime.Parse(dt.Rows[i]["TRADE_DATE"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 14, new DateTime());
                        }

                        if (dt.Rows[i]["MATURITY_DATE"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 15, DateTime.Parse(dt.Rows[i]["MATURITY_DATE"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 15, new DateTime());
                        }

                        excelTemplate.CreateCellColLeft(excelRow, 16, dt.Rows[i]["BOND_CODE"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 17, dt.Rows[i]["BOND_ISIN_CODE"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 18, dt.Rows[i]["BOND_CUR"].ToString());

                        double FaceValue = 0;
                        if (dt.Rows[i]["BOND_FACE_VALUE"].ToString() != string.Empty)
                            FaceValue = double.Parse(dt.Rows[i]["BOND_FACE_VALUE"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 19, FaceValue);

                        double InstrumentPurchase = 0;
                        if (dt.Rows[i]["BOND_MTM_AMT"].ToString() != string.Empty)
                            InstrumentPurchase = double.Parse(dt.Rows[i]["BOND_MTM_AMT"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 20, InstrumentPurchase);

                        double InstrumentExposure = 0;
                        if (dt.Rows[i]["BOND_EXP_GRP"].ToString() != string.Empty)
                            InstrumentExposure = double.Parse(dt.Rows[i]["BOND_EXP_GRP"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 21, InstrumentExposure);

                   
                        excelTemplate.CreateCellColCenter(excelRow, 22, dt.Rows[i]["BOND_AGENCY_RATING"].ToString());

                        excelTemplate.CreateCellColLeft(excelRow, 23, dt.Rows[i]["BOND_NAME"].ToString());

                        excelTemplate.CreateCellColCenter(excelRow, 24, dt.Rows[i]["ISSUER_AGENCY_RATING"].ToString());

                        excelTemplate.CreateCellColLeft(excelRow, 25, dt.Rows[i]["ISSUER_RATING"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 26, dt.Rows[i]["ISSUER_COUNTRY"].ToString());

                        excelTemplate.CreateCellColCenter(excelRow, 27, dt.Rows[i]["ISSUER_COUNTRY_RATING"].ToString());

                        excelTemplate.CreateCellColCenter(excelRow, 28, dt.Rows[i]["CORE_MARKET"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 29, dt.Rows[i]["BOT_CRITERIA"].ToString());

                        if (dt.Rows[i]["ASOF_DATE"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 30, DateTime.Parse(dt.Rows[i]["ASOF_DATE"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 30, new DateTime());
                        }

                        if (dt.Rows[i]["BOND_RATING_ASS_DATE"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 31, DateTime.Parse(dt.Rows[i]["BOND_RATING_ASS_DATE"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 31, new DateTime());
                        }

                        if (dt.Rows[i]["ISSUER_RATING_ASS_DATE"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 32, DateTime.Parse(dt.Rows[i]["ISSUER_RATING_ASS_DATE"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 32, new DateTime());
                        }

                        excelTemplate.CreateCellColCenter(excelRow, 33, dt.Rows[i]["CR_MIT_ADJ_FLAG"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 34, dt.Rows[i]["PRINCIPAL_ONLY_FLAG"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 35, dt.Rows[i]["BOND_TYPE"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 36, dt.Rows[i]["ISSUER_CODE"].ToString());

                        double EffectiveMat = 0;
                        if (dt.Rows[i]["PERIOD_DAY"].ToString() != string.Empty)
                            EffectiveMat = double.Parse(dt.Rows[i]["PERIOD_DAY"].ToString());
                        excelTemplate.CreateCellColNumber(excelRow, 37, EffectiveMat);

                        excelTemplate.CreateCellColLeft(excelRow, 38, dt.Rows[i]["GL_ACC_NO"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 39, dt.Rows[i]["INTERNAL_ORG"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 40, dt.Rows[i]["BOND_PORT"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 41, dt.Rows[i]["NETTING_ID"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 42, dt.Rows[i]["IN_DEFAULT_FLAG"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 43, dt.Rows[i]["HIGH_RISK_FLAG"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 44, dt.Rows[i]["COLL_TYPE"].ToString());
                    }

                    for (var i = 1; i <= 44; i++)
                    {
                        sheet.AutoSizeColumn(i);

                        var colWidth = sheet.GetColumnWidth(i);
                        if (colWidth < 2000)
                            sheet.SetColumnWidth(i, 2000);
                        else
                            sheet.SetColumnWidth(i, colWidth + 200);
                    }

                    // Set Merge Cells Header Report_Banak
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 4));

                    var headerRow = sheet.GetRow(5);
                    headerRow.HeightInPoints = 30;

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

        public DataTable GetReportData(ReportCriteriaModel data)
        {
            var dt = new DataTable();
            apiReport.ReportData.BISREVREPOReport(data, p =>
            {
                if (p.Success) dt = p.Data.BISREVREPOReportResultModel.ToDataTable();
            });

            return dt;
        }

        public ActionResult FillCurrency(string datastr)
        {
            var res = new List<DDLItemModel>();
            apiStatic.Currency.GetDDLCurrency(datastr, p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillExecuteType(string datastr)
        {
            var res = new List<DDLItemModel>();
            res.Add(new DDLItemModel { Text = "Query", Value = "QUERY" });
            res.Add(new DDLItemModel { Text = "Rerun", Value = "EXEC" });
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}using GM.Application.Web.Areas.Report.Models;
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
    public class ReportBISREVREPOController : BaseController
    {
        private readonly ReportEntities apiReport = new ReportEntities();
        private readonly StaticEntities apiStatic = new StaticEntities();
        public string Report_Bank = "SiGG Financial Bank";
        public string Report_Code = string.Empty;
        public string Report_DateFromTo = string.Empty;
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
                Report_Name = "BISREVREPOReport.xls";
                if (reportname_list.Count == 0)
                {
                    ViewBag.ErrorMessage =
                        "Can not Get Data Report ID from Service Static Method Config/getReportID and key = " +
                        controller_name + " in table gm_report !!!!";
                    return View();
                }

                reportid = reportname_list[0].Value.ToString();
                Report_Header = "BISREVREPO Report";

                if (string.IsNullOrEmpty(reportCriteriaModel.asofdate_string))
                {
                    ViewBag.ErrorMessage = "Please select As Of Date From";
                    return View();
                }

                reportCriteriaModel.asofdate = string.IsNullOrEmpty(reportCriteriaModel.asofdate_string)
                    ? reportCriteriaModel.asofdate
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.asofdate_string);

                Report_DateFromTo = (reportCriteriaModel.asofdate != null ? "As Of Date " : "") +
                                    (reportCriteriaModel.asofdate == null
                                        ? ""
                                        : reportCriteriaModel.asofdate.Value.ToString("dd/MM/yyyy"));

                Report_Header += $" (Report No.{reportid})";

                var businessdate = reportentity.Getbusinessdate();

                if (collection["PDF"] != null) return View();

                if (collection["Excel"] != null)
                {
                    dt = GetReportData(reportCriteriaModel);

                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("BISREVREPO");

                    var excelTemplate = new ExcelTemplate(workbook);

                    var transNo = "";

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

                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 0, "CIF");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 1, "Deal No.");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 2, "Customer Code");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 3, "ชื่อลูกหนี้");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 4, "Exposure Group ของลูกหนี้");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 5, "สกุลเงินของยอดหนี้");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 6, "ยอดหนี้ (ยอดทำ Reverse Repo)");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 7, "ยอด Cash Margin");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 8, "ยอดดอกเบี้ยค้างรับ");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 9, "ยอดดอกเบี้ยค้างรับของ Cash Margin");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 10, "Banking Book หรือ Trading Book");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 11, "ประเทศลูกหนี้");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 12, "สกุลเงินของประเทศลูกหนี้");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 13, "Rating ของประเทศลูกหนี้");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 14, "Trade Date");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 15, "Maturity Date");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 16, "ชื่อย่อของตราสารที่ลูกหนี้นำมาวางเป็นประกัน");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 17, "ชื่อเต็มของตราสารที่ลูกหนี้นำมาวางเป็นประกัน");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 18, "สกุลเงินของตราสาร");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 19, "ยอด Outstanding ของตราสาร Face Value");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 20, "มูลค่าของตราสารที่ลูกหนี้ได้นำมาวางเป็นประกัน");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 21, "Exposure Group ของตราสารที่ลูกหนี้ได้นำมาวางเป็นประกัน");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 22, "Rating ของตราสาร");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 23, "สถาบันจัดอันดับ Rating ของตราสาร");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 24, "Rating ของผู้ออกตราสาร");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 25, "สถาบันจัดอันดับ Rating ของผู้ออกตราสาร");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 26, "ประเทศของผู้ออกตราสาร");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 27, "Rating ของประเทศผู้ออกตราสาร");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 28, "Core Market Participant");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 29, "BOT’s Criteria");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 30, "DATA DATE");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 31, "Bond Rating Assessment Date");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 32, "Issuer Rating Assessment Date");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 33, "Credit Mitigant Adjusted Flag");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 34, "Principal Only Flag");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 35, "Bond Type");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 36, "Bond Issuer");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 37, "Effective Maturity Day Number");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 38, "GL Account");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 39, "Internal Organization");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 40, "Portfolio");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 41, "Netting Set ID");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 42, "In Default Flag");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 43, "High Risk Category Flag");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 44, "Collateral Type");

                    // Add Data Rows
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);

                        excelTemplate.CreateCellColLeft(excelRow, 0, dt.Rows[i]["CNTR_SWIFT_CODE"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 1, dt.Rows[i]["TRANS_NO"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 2, dt.Rows[i]["CNTR_CODE"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 3, dt.Rows[i]["CNTR_NAME"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 4, dt.Rows[i]["CNTR_EXP_GRP"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 5, dt.Rows[i]["CUR"].ToString());

                        double PurchaseAmt = 0;
                        if (dt.Rows[i]["BOND_CASH_AMT"].ToString() != string.Empty)
                            PurchaseAmt = double.Parse(dt.Rows[i]["BOND_CASH_AMT"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 6, PurchaseAmt);

                        double CashMargin = 0;
                        if (dt.Rows[i]["CASH_MARGIN"].ToString() != string.Empty)
                            CashMargin = double.Parse(dt.Rows[i]["CASH_MARGIN"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 7, CashMargin);

                        if (transNo != dt.Rows[i]["TRANS_NO"].ToString())
                        {
                            double InterestAmt = 0;
                            if (dt.Rows[i]["INT_AMT"].ToString() != string.Empty)
                                InterestAmt = double.Parse(dt.Rows[i]["INT_AMT"].ToString());
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 8, InterestAmt);

                            transNo = dt.Rows[i]["TRANS_NO"].ToString();
                        }
                        else 
                        {
                            excelTemplate.CreateCellCol2RedDecimal(excelRow, 8, 0);
                        }

                        double InterestAmtMargin = 0;
                        if (dt.Rows[i]["INT_CASH_MARGIN"].ToString() != string.Empty)
                            InterestAmtMargin = double.Parse(dt.Rows[i]["INT_CASH_MARGIN"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 9, InterestAmtMargin);

                        excelTemplate.CreateCellColCenter(excelRow, 10, dt.Rows[i]["PORT_CODE"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 11, dt.Rows[i]["CNTR_COUNTRY"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 12, dt.Rows[i]["CNTR_COUNTRY_CUR"].ToString());

                        excelTemplate.CreateCellColCenter(excelRow, 13, dt.Rows[i]["CNTR_COUNTRY_RATING"].ToString());

                        if (dt.Rows[i]["TRADE_DATE"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 14, DateTime.Parse(dt.Rows[i]["TRADE_DATE"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 14, new DateTime());
                        }

                        if (dt.Rows[i]["MATURITY_DATE"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 15, DateTime.Parse(dt.Rows[i]["MATURITY_DATE"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 15, new DateTime());
                        }

                        excelTemplate.CreateCellColLeft(excelRow, 16, dt.Rows[i]["BOND_CODE"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 17, dt.Rows[i]["BOND_ISIN_CODE"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 18, dt.Rows[i]["BOND_CUR"].ToString());

                        double FaceValue = 0;
                        if (dt.Rows[i]["BOND_FACE_VALUE"].ToString() != string.Empty)
                            FaceValue = double.Parse(dt.Rows[i]["BOND_FACE_VALUE"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 19, FaceValue);

                        double InstrumentPurchase = 0;
                        if (dt.Rows[i]["BOND_MTM_AMT"].ToString() != string.Empty)
                            InstrumentPurchase = double.Parse(dt.Rows[i]["BOND_MTM_AMT"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 20, InstrumentPurchase);

                        double InstrumentExposure = 0;
                        if (dt.Rows[i]["BOND_EXP_GRP"].ToString() != string.Empty)
                            InstrumentExposure = double.Parse(dt.Rows[i]["BOND_EXP_GRP"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 21, InstrumentExposure);

                   
                        excelTemplate.CreateCellColCenter(excelRow, 22, dt.Rows[i]["BOND_AGENCY_RATING"].ToString());

                        excelTemplate.CreateCellColLeft(excelRow, 23, dt.Rows[i]["BOND_NAME"].ToString());

                        excelTemplate.CreateCellColCenter(excelRow, 24, dt.Rows[i]["ISSUER_AGENCY_RATING"].ToString());

                        excelTemplate.CreateCellColLeft(excelRow, 25, dt.Rows[i]["ISSUER_RATING"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 26, dt.Rows[i]["ISSUER_COUNTRY"].ToString());

                        excelTemplate.CreateCellColCenter(excelRow, 27, dt.Rows[i]["ISSUER_COUNTRY_RATING"].ToString());

                        excelTemplate.CreateCellColCenter(excelRow, 28, dt.Rows[i]["CORE_MARKET"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 29, dt.Rows[i]["BOT_CRITERIA"].ToString());

                        if (dt.Rows[i]["ASOF_DATE"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 30, DateTime.Parse(dt.Rows[i]["ASOF_DATE"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 30, new DateTime());
                        }

                        if (dt.Rows[i]["BOND_RATING_ASS_DATE"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 31, DateTime.Parse(dt.Rows[i]["BOND_RATING_ASS_DATE"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 31, new DateTime());
                        }

                        if (dt.Rows[i]["ISSUER_RATING_ASS_DATE"].ToString() != string.Empty)
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 32, DateTime.Parse(dt.Rows[i]["ISSUER_RATING_ASS_DATE"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 32, new DateTime());
                        }

                        excelTemplate.CreateCellColCenter(excelRow, 33, dt.Rows[i]["CR_MIT_ADJ_FLAG"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 34, dt.Rows[i]["PRINCIPAL_ONLY_FLAG"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 35, dt.Rows[i]["BOND_TYPE"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 36, dt.Rows[i]["ISSUER_CODE"].ToString());

                        double EffectiveMat = 0;
                        if (dt.Rows[i]["PERIOD_DAY"].ToString() != string.Empty)
                            EffectiveMat = double.Parse(dt.Rows[i]["PERIOD_DAY"].ToString());
                        excelTemplate.CreateCellColNumber(excelRow, 37, EffectiveMat);

                        excelTemplate.CreateCellColLeft(excelRow, 38, dt.Rows[i]["GL_ACC_NO"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 39, dt.Rows[i]["INTERNAL_ORG"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 40, dt.Rows[i]["BOND_PORT"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 41, dt.Rows[i]["NETTING_ID"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 42, dt.Rows[i]["IN_DEFAULT_FLAG"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 43, dt.Rows[i]["HIGH_RISK_FLAG"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 44, dt.Rows[i]["COLL_TYPE"].ToString());
                    }

                    for (var i = 1; i <= 44; i++)
                    {
                        sheet.AutoSizeColumn(i);

                        var colWidth = sheet.GetColumnWidth(i);
                        if (colWidth < 2000)
                            sheet.SetColumnWidth(i, 2000);
                        else
                            sheet.SetColumnWidth(i, colWidth + 200);
                    }

                    // Set Merge Cells Header Report_Banak
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 4));

                    var headerRow = sheet.GetRow(5);
                    headerRow.HeightInPoints = 30;

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

        public DataTable GetReportData(ReportCriteriaModel data)
        {
            var dt = new DataTable();
            apiReport.ReportData.BISREVREPOReport(data, p =>
            {
                if (p.Success) dt = p.Data.BISREVREPOReportResultModel.ToDataTable();
            });

            return dt;
        }

        public ActionResult FillCurrency(string datastr)
        {
            var res = new List<DDLItemModel>();
            apiStatic.Currency.GetDDLCurrency(datastr, p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillExecuteType(string datastr)
        {
            var res = new List<DDLItemModel>();
            res.Add(new DDLItemModel { Text = "Query", Value = "QUERY" });
            res.Add(new DDLItemModel { Text = "Rerun", Value = "EXEC" });
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}