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
    public class ReportDailyRepoOutstandingController : BaseController
    {
        private readonly CounterPartyEntities api_counterparty = new CounterPartyEntities();
        private readonly RPTransEntity api_deal = new RPTransEntity();

        private readonly SecurityEntities api_security = new SecurityEntities();
        private readonly StaticEntities api_static = new StaticEntities();
        private readonly ReportEntities apiReport = new ReportEntities();
        public string Report_Bank = "SiGG Financial Bank";
        public string Report_Code = string.Empty;
        public string Report_CounterParty = string.Empty;
        public string Report_DateFromTo = string.Empty;
        public string Report_End_Users = string.Empty;
        public string Report_File_Name = string.Empty;
        public string Report_Header = string.Empty;
        public string Report_Name = string.Empty;
        public string Report_Owner = string.Empty;
        public string Report_Port = string.Empty;
        public string Report_Repo_Deal_Type = string.Empty;
        public string Report_Type = string.Empty;
        private readonly ReportEntitiesModel reportentity = new ReportEntitiesModel();
        private readonly Utility utility = new Utility();

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(ReportCriteriaModel model, FormCollection collection)
        {
            var Error_desc = string.Empty;
            var Error_code = 0;
            try
            {
                #region :: Validate Date Range Not Over 1 Year ::

                model.asofdate_from = string.IsNullOrEmpty(model.asofdate_from_string)
                    ? model.asofdate_from
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(model.asofdate_from_string);
                model.asofdate_to = string.IsNullOrEmpty(model.asofdate_to_string)
                    ? model.asofdate_to
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(model.asofdate_to_string);

                #endregion


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
                Report_File_Name = reportname_list[0].Text;
                Report_Header = "Daily Repo Outstanding Report ";

                model.asofdate_from = string.IsNullOrEmpty(model.asofdate_from_string)
                    ? model.asofdate_from
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(model.asofdate_from_string);
                model.asofdate_to = string.IsNullOrEmpty(model.asofdate_to_string)
                    ? model.asofdate_to
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(model.asofdate_to_string);

                Report_DateFromTo = (model.asofdate_from != null || model.asofdate_to != null ? "As of Date " : "") +
                                    (model.asofdate_from == null
                                        ? ""
                                        : model.asofdate_from.Value.ToString("dd/MM/yyyy")) +
                                    (model.asofdate_from != null && model.asofdate_to != null ? " - " : "") +
                                    (model.asofdate_to == null ? "" : model.asofdate_to.Value.ToString("dd/MM/yyyy"));
                Report_Port = string.IsNullOrEmpty(model.port) ? "Port : ALL" : $"Port : {model.port_name}";
                Report_DateFromTo += string.IsNullOrEmpty(model.counterparty_code)
                    ? ", Counter Party : ALL"
                    : $", Counter Party : {model.counterparty_code_name}";
                Report_DateFromTo += string.IsNullOrEmpty(model.instrument_type_name)
                    ? ", Trans Deal Type : ALL"
                    : $", Trans Deal Type : {model.instrument_type_name}";
                model.report_type = model.report_type == "All" ? "ALL" : model.report_type;
                Report_DateFromTo += string.IsNullOrEmpty(model.report_type)
                    ? ", Report Type : ALL"
                    : $", Report Type : {model.report_type}";

                Report_DateFromTo += string.IsNullOrEmpty(model.currency) ? "" : ", Currency : " + model.currency;
                Report_Header += $" (Report No.{reportid})";

                var businessdate = reportentity.Getbusinessdate();

                #region :: EXPORT EXCEL ::

                if (collection["Excel"] != null)
                {
                    List<DailyRepoOutstandingReportModel> resList = new List<DailyRepoOutstandingReportModel>();
                    apiReport.ReportData.OutstandingDailyRepoReport(model, p =>
                    {
                        resList = p.Data.DailyRepoOutstandingResultModel;

                        Error_code = p.RefCode;
                        Error_desc = p.Message;
                    });

                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("DailyRepoOutstanding");

                    var excelTemplate = new ExcelTemplate(workbook);

                    var colGroup = excelTemplate.CellStyle(verticalAlignment: VerticalAlignment.Center, fontBoldWeight: FontBoldWeight.Bold,
                        horizontalAlignment: HorizontalAlignment.Center, borderColor: 22);

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

                    excelRow = sheet.CreateRow(rowIndex);

                    // Add Header Table
                    excelTemplate.CreateCellColHead(excelRow, 0, "No.");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Date");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Counterparty Name");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Cpty. Code");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Portfolio");
                    excelTemplate.CreateCellColHead(excelRow, 5, "Inst. Type");
                    excelTemplate.CreateCellColHead(excelRow, 6, "Notional Cash Amount");

                    // Add Data Rows
                    int countNo = 0;

                    IDataFormat dataFormatCustom = workbook.CreateDataFormat();

                    foreach (var row in resList)
                    {
                        countNo++;
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);

                        double nation_cash_amount = 0;
                        if (!string.IsNullOrEmpty(row.Notional_Cash_Amount))
                            nation_cash_amount = double.Parse(row.Notional_Cash_Amount);

                        if (!string.IsNullOrEmpty(row.Instrument_Type))
                        {
                            if (row.Instrument_Type == Constant.TRANS_DEAL_TYPE_BORROWING)
                                row.Instrument_Type = "Borrowing";
                            else if (row.Instrument_Type == Constant.TRANS_DEAL_TYPE_LENDING)
                                row.Instrument_Type = "Lending";
                        }

                        excelTemplate.CreateCellColRight(excelRow, 0, countNo.ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 1, utility.ConvertStringToDatetimeFormatDDMMYYYY(row.Report_Date));

                        excelTemplate.CreateCellColLeft(excelRow, 2, row.CounterParty_Name);
                        excelTemplate.CreateCellColLeft(excelRow, 3, row.CounterParty_Code);
                        excelTemplate.CreateCellColLeft(excelRow, 4, row.Portfolio);
                        excelTemplate.CreateCellColLeft(excelRow, 5, row.Instrument_Type);

                        excelTemplate.CreateCellColDecimalBucket(excelRow, 6, nation_cash_amount, 2);
                    }

                    //    rowIndex++;
                    //    excelRow = sheet.CreateRow(rowIndex);
                    //    excelTemplate.CreateCellFooterRight(excelRow, 0, "Total");
                    //    excelTemplate.CreateCellFooterRight(excelRow, 1, "Total");
                    //    excelTemplate.CreateCellFooterRight(excelRow, 2, "Total");
                    //    excelTemplate.CreateCellFooterRight(excelRow, 3, "Total");
                    //    excelTemplate.CreateCellFooterRight(excelRow, 4, "Total");
                    //    excelTemplate.CreateCellFooterRight(excelRow, 5, "Total");

                    //    excelTemplate.CreateCellFooter2Decimal(excelRow, 6, 0);
                    //    excelRow.GetCell(6)
                    //        .SetCellFormula(string.Format("SUM(G{0}:G{1})", startRow+2, rowIndex));
                    //    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 5));
                    //}

                    for (var i = 1; i <= 34; i++)
                    {
                        sheet.AutoSizeColumn(i);

                        var colWidth = sheet.GetColumnWidth(i);
                        if (colWidth < 2000)
                            sheet.SetColumnWidth(i, 2000);
                        else
                            sheet.SetColumnWidth(i, colWidth + 200);
                    }

                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 13));
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 13));
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 13));
                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 13));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 13));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 0, 13));

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

                #endregion

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                //throw ex;
            }

            return View();
        }

        public DataTable GetDailyRepoOutstanding(ReportCriteriaModel data, ref int ErrorCode, ref string ErrorDesc)
        {
            var dt = new DataTable();

            var tmpErrorCode = 0;
            var tmpErrorDesc = string.Empty;
            apiReport.ReportData.OutstandingDailyRepoReport(data, p =>
            {
                if (p.Success) dt = p.Data.DailyRepoOutstandingResultModel.ToDataTable();

                tmpErrorCode = p.RefCode;
                tmpErrorDesc = p.Message;
            });

            ErrorCode = tmpErrorCode;
            ErrorDesc = tmpErrorDesc;
            return dt;
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

        public ActionResult FillRepoDealType(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_deal.RPDealEntry.GetDDLRepoDealType(p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillReportType()
        {
            var controller = ControllerContext.RouteData.Values["controller"].ToString();
            var res = new List<DDLItemModel>();
            api_static.Config.GetConfigReport(controller, "report_type", p =>
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

        private List<DDLItemModel> GetReportHeaderConfiguration(string controller)
        {
            var res = new List<DDLItemModel>();
            api_static.Config.GetConfigReport(controller, "report_header", p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return res;
        }
    }
}