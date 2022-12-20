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
    public class ReportMarginCalResultController : BaseController
    {
        private readonly CounterPartyEntities api_counterparty = new CounterPartyEntities();
        private readonly StaticEntities api_static = new StaticEntities();
        private readonly ReportEntities apiReport = new ReportEntities();
        private readonly ReportEntitiesModel reportentity = new ReportEntitiesModel();
        private string reportid;
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
                var reportname_list = new List<DDLItemModel>();
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
                if (reportCriteriaModel.type_date == "CALL_DATE")
                {
                    reportCriteriaModel.call_date_from = string.IsNullOrEmpty(reportCriteriaModel.from_date_string)
                        ? reportCriteriaModel.call_date_from
                        : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.from_date_string);
                    reportCriteriaModel.call_date_to = string.IsNullOrEmpty(reportCriteriaModel.to_date_string)
                        ? reportCriteriaModel.call_date_to
                        : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.to_date_string);
                }
                else
                {
                    //AS_DATE
                    reportCriteriaModel.asofdate_from = string.IsNullOrEmpty(reportCriteriaModel.from_date_string)
                        ? reportCriteriaModel.asofdate_from
                        : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.from_date_string);
                    reportCriteriaModel.asofdate_to = string.IsNullOrEmpty(reportCriteriaModel.to_date_string)
                        ? reportCriteriaModel.asofdate_to
                        : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.to_date_string);
                }

                if (reportCriteriaModel.type_report == "SUM")
                    ExportExcelSummary(reportCriteriaModel);
                else
                    ExportExcelDetail(reportCriteriaModel);

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(ex.ToString());
            }
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

        public DataTable GetTransPRPData(ReportCriteriaModel data)
        {
            var dt = new DataTable();

            apiReport.ReportData.MarginTransPRPReport(data, p =>
            {
                if (p.Success) dt = p.Data.MarginTransPRPReportResultModel.ToDataTable();
            });

            return dt;
        }

        public DataTable GetTransBRPData(ReportCriteriaModel data)
        {
            var dt = new DataTable();

            apiReport.ReportData.MarginTransBRPReport(data, p =>
            {
                if (p.Success) dt = p.Data.MarginTransBRPReportResultModel.ToDataTable();
            });

            return dt;
        }

        public DataTable GetDetailData(ReportCriteriaModel model)
        {
            var dt = new DataTable();

            apiReport.ReportData.MarginTransDetailReport(model, p =>
            {
                if (p.Success) dt = p.Data.MarginTransDetailReportResultModel.ToDataTable();
            });

            return dt;
        }

        private void ExportExcelSummary(ReportCriteriaModel model)
        {
            var dt = GetTransPRPData(model);
            var Report_File_Name = "MarginCalResult";

            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("MarginCalResult_Summary");

            var excelTemplate = new ExcelTemplate(workbook);

            string reportDate;
            if (model.type_date == "CALL_DATE")
            {
                reportDate = "Call date : " + model.from_date_string;
                if (!string.IsNullOrEmpty(model.to_date_string))
                    reportDate += " - " + model.to_date_string;
            }
            else
            {
                reportDate = "As of date : " + model.from_date_string;
                if (!string.IsNullOrEmpty(model.to_date_string))
                    reportDate += " - " + model.to_date_string;
            }

            // Add Header
            var rowIndex = 0;
            var excelRow = sheet.CreateRow(rowIndex);
            excelTemplate.CreateCellHeaderLeft(excelRow, 0, "SiGG Financial Bank");
            rowIndex++;

            excelRow = sheet.CreateRow(rowIndex);
            excelTemplate.CreateCellHeaderLeft(excelRow, 0, $"Margin Calculation Result : Summary Report (Report No.{reportid})");
            rowIndex++;

            excelRow = sheet.CreateRow(rowIndex);
            excelTemplate.CreateCellHeaderLeft(excelRow, 0, reportDate);
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

            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 3));
            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 3));
            sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 3));
            sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 3));
            sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 5));
            sheet.AddMergedRegion(new CellRangeAddress(5, 5, 0, 3));

            rowIndex++;

            excelRow = sheet.CreateRow(rowIndex);
            excelTemplate.CreateCellHeaderLeft(excelRow, 0, "Instrument : PRP");
            rowIndex++;

            // Add Header Table
            var colIndex = 0;
            excelRow = sheet.CreateRow(rowIndex);
            excelTemplate.CreateCellColHead(excelRow, colIndex, "Counter Party");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Counter Party Fund");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Threshold");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "CCY");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Exposure");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Position Yesterday");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Margin");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Accrue Int. Yesterday");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Net Exposure");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Margin Call\nRec= +,Pay= -");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Close Margin");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Interest Receive");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "WH Tax 1%");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Interest Paid");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Margin Balance");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Int. Rate");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Int. Per Day");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Net Accrue Int. Today");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Net Accrue Int. Yesterday");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Remark");

            excelRow.GetCell(colIndex).CellStyle.WrapText = true;
            excelRow.HeightInPoints = 45;

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                colIndex = 0;
                rowIndex++;
                excelRow = sheet.CreateRow(rowIndex);

                excelTemplate.CreateCellColCenter(excelRow, colIndex, dt.Rows[i]["counter_party_code"].ToString());
                colIndex++;

                excelTemplate.CreateCellColCenter(excelRow, colIndex, dt.Rows[i]["fundname"].ToString());
                colIndex++;

                double.TryParse(dt.Rows[i]["threshold"].ToString(), out var threshold);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, threshold);
                colIndex++;

                excelTemplate.CreateCellColCenter(excelRow, colIndex, dt.Rows[i]["cur"].ToString());
                colIndex++;

                double.TryParse(dt.Rows[i]["exposure"].ToString(), out var exposure);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, exposure);
                colIndex++;

                double.TryParse(dt.Rows[i]["position_yesterday"].ToString(), out var positionYesterday);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, positionYesterday);
                colIndex++;

                if (dt.Rows[i]["margin"] != null)
                {
                    double.TryParse(dt.Rows[i]["margin"].ToString(), out var margin);
                    excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, margin);
                }
                colIndex++;

                double.TryParse(dt.Rows[i]["accrue_int_yesterday"].ToString(), out var accrueIntYesterday);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, accrueIntYesterday);
                colIndex++;

                double.TryParse(dt.Rows[i]["net_exposure"].ToString(), out var netExposure);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, netExposure);
                colIndex++;

                double.TryParse(dt.Rows[i]["call_margin"].ToString(), out var callMargin);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, callMargin);
                colIndex++;

                double.TryParse(dt.Rows[i]["close_margin"].ToString(), out var closeMargin);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, closeMargin);
                colIndex++;

                double.TryParse(dt.Rows[i]["Int_Recv_Disp"].ToString(), out var intRecvDisp);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, intRecvDisp);
                colIndex++;

                double.TryParse(dt.Rows[i]["Int_Tax_Disp"].ToString(), out var intTaxDisp);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, intTaxDisp);
                colIndex++;

                double.TryParse(dt.Rows[i]["Int_Paid_Disp"].ToString(), out var intPaidDisp2);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, intPaidDisp2);
                colIndex++;

                double.TryParse(dt.Rows[i]["margin_balance"].ToString(), out var marginBalance);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, marginBalance);
                colIndex++;

                double.TryParse(dt.Rows[i]["IntRateToday"].ToString(), out var intRateToday);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, intRateToday);
                colIndex++;

                double.TryParse(dt.Rows[i]["IntPerDay"].ToString(), out var intPerDay);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, intPerDay);
                colIndex++;

                double.TryParse(dt.Rows[i]["net_accrue_int_today"].ToString(), out var netAccrueToday);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, netAccrueToday);
                colIndex++;

                double.TryParse(dt.Rows[i]["net_accrue_int_yesterday"].ToString(), out var netAccrueYesterday);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, netAccrueYesterday);
                colIndex++;

                excelTemplate.CreateCellColLeft(excelRow, colIndex, dt.Rows[i]["remark"].ToString());
            }

            var endColIndex = colIndex;

            if (dt.Rows.Count > 0)
            {
                rowIndex++;
                excelRow = sheet.CreateRow(rowIndex);
                for (var i = 0; i <= endColIndex; i++) excelTemplate.CreateCellFooter(excelRow, i, "");

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 9, 0, 2);
                excelRow.GetCell(9).SetCellFormula(string.Format("SUM(J9:J{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 14, 0, 2);
                excelRow.GetCell(14).SetCellFormula(string.Format("SUM(O9:O{0})", rowIndex));
            }

            for (var i = 0; i <= endColIndex; i++)
            {
                sheet.AutoSizeColumn(i);

                var colWidth = sheet.GetColumnWidth(i);
                if (colWidth < 2000)
                    sheet.SetColumnWidth(i, 2000);
                else
                    sheet.SetColumnWidth(i, colWidth + 200);
            }

            //Instrument : BRP

            dt = GetTransBRPData(model);

            rowIndex += 3;


            excelRow = sheet.CreateRow(rowIndex);
            excelTemplate.CreateCellHeaderLeft(excelRow, 0, "Instrument : BRP");
            rowIndex++;

            // Add Header Table
            excelRow = sheet.CreateRow(rowIndex);
            colIndex = 0;
            excelTemplate.CreateCellColHead(excelRow, colIndex, "Contract No.");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "CCY");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Exposure");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Position Yesterday");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Margin Call\nRec= +,Pay= -");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Close Margin");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Interest Receive");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "WH Tax 1%");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Interest Paid");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Margin");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Margin Balance");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Int. Rate");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Int. Per Day");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Int. Recv Today");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Int. Recv Yesterday");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Int. Pay Today");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Int. Pay Yesterday");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Remark");

            excelRow.GetCell(colIndex).CellStyle.WrapText = true;
            excelRow.HeightInPoints = 45;

            var rowIndexBRP = rowIndex + 1;

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                colIndex = 0;
                rowIndex++;

                excelRow = sheet.CreateRow(rowIndex);

                excelTemplate.CreateCellColLeft(excelRow, colIndex, dt.Rows[i]["contract_no"].ToString());
                colIndex++;

                excelTemplate.CreateCellColCenter(excelRow, colIndex, dt.Rows[i]["cur"].ToString());
                colIndex++;

                double.TryParse(dt.Rows[i]["exposure"].ToString(), out var exposure);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, exposure);
                colIndex++;

                double.TryParse(dt.Rows[i]["position_yesterday"].ToString(), out var positionYesterday);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, positionYesterday);
                colIndex++;

                if (dt.Rows[i]["margin"] != null)
                {
                    double.TryParse(dt.Rows[i]["margin"].ToString(), out var margin);
                    excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, margin);
                }
                colIndex++;

                double.TryParse(dt.Rows[i]["close_margin"].ToString(), out var closeMargin);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, closeMargin);
                colIndex++;

                double.TryParse(dt.Rows[i]["Int_Recv_Disp"].ToString(), out var intRecvDisp);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, intRecvDisp);
                colIndex++;

                double.TryParse(dt.Rows[i]["Int_Tax_Disp"].ToString(), out var intTaxDisp);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, intTaxDisp);
                colIndex++;

                double.TryParse(dt.Rows[i]["Int_Paid_Disp"].ToString(), out var intPaidDisp2);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, intPaidDisp2);
                colIndex++;

                double.TryParse(dt.Rows[i]["call_margin"].ToString(), out var callMargin);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, callMargin);
                colIndex++;

                double.TryParse(dt.Rows[i]["margin_balance"].ToString(), out var marginBalance);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, marginBalance);
                colIndex++;

                double.TryParse(dt.Rows[i]["INT_RATE"].ToString(), out var INT_RATE);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, INT_RATE);
                colIndex++;

                double.TryParse(dt.Rows[i]["IntPerDay"].ToString(), out var intPerDay);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, intPerDay);
                colIndex++;

                double.TryParse(dt.Rows[i]["IntRecToday"].ToString(), out var IntRecToday);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, IntRecToday);
                colIndex++;

                double.TryParse(dt.Rows[i]["IntRecYesterday"].ToString(), out var IntRecYesterday);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, IntRecYesterday);
                colIndex++;

                double.TryParse(dt.Rows[i]["IntPayToday"].ToString(), out var IntPayToday);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, IntPayToday);
                colIndex++;

                double.TryParse(dt.Rows[i]["IntPayYesterday"].ToString(), out var IntPayYesterday);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, IntPayYesterday);
                colIndex++;

                excelTemplate.CreateCellColLeft(excelRow, colIndex, dt.Rows[i]["BRP_REMARK"].ToString());

            }

            endColIndex = colIndex;

            if (dt.Rows.Count > 0)
            {
                rowIndex++;
                excelRow = sheet.CreateRow(rowIndex);
                for (var i = 0; i <= endColIndex; i++) excelTemplate.CreateCellFooter(excelRow, i, "");

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 4, 0, 2);
                excelRow.GetCell(4).SetCellFormula(string.Format("SUM(E" + rowIndexBRP + ":E{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 5, 0, 2);
                excelRow.GetCell(5).SetCellFormula(string.Format("SUM(F" + rowIndexBRP + ":F{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 10, 0, 2);
                excelRow.GetCell(10).SetCellFormula(string.Format("SUM(K" + rowIndexBRP + ":K{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 13, 0, 2);
                excelRow.GetCell(13).SetCellFormula(string.Format("SUM(N" + rowIndexBRP + ":N{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 15, 0, 2);
                excelRow.GetCell(15).SetCellFormula(string.Format("SUM(P" + rowIndexBRP + ":P{0})", rowIndex));
            }

            for (var i = 0; i <= endColIndex; i++)
            {
                sheet.AutoSizeColumn(i);

                var colWidth = sheet.GetColumnWidth(i);
                if (colWidth < 2000)
                    sheet.SetColumnWidth(i, 2000);
                else
                    sheet.SetColumnWidth(i, colWidth + 200);
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
        }

        private void ExportExcelDetail(ReportCriteriaModel model)
        {
            var dt = GetDetailData(model);

            var Report_File_Name = "MarginCalResult_Detail";

            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("MarginCalResult");

            var excelTemplate = new ExcelTemplate(workbook);

            // Add Header
            var rowIndex = 0;
            string reportDate;
            if (model.type_date == "CALL_DATE")
            {
                reportDate = "Call date : " + model.from_date_string;
                if (!string.IsNullOrEmpty(model.to_date_string))
                    reportDate += " - " + model.to_date_string;
            }
            else
            {
                reportDate = "As of date : " + model.from_date_string;
                if (!string.IsNullOrEmpty(model.to_date_string))
                    reportDate += " - " + model.to_date_string;
            }

            // Add Header
            var excelRow = sheet.CreateRow(rowIndex);
            excelTemplate.CreateCellHeaderLeft(excelRow, 0, "SiGG Financial Bank");
            rowIndex++;

            excelRow = sheet.CreateRow(rowIndex);
            excelTemplate.CreateCellHeaderLeft(excelRow, 0, $"Margin Calculation Result : Detail Report (Report No.{reportid})");
            rowIndex++;

            excelRow = sheet.CreateRow(rowIndex);
            excelTemplate.CreateCellHeaderLeft(excelRow, 0, reportDate);
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

            if (model.counterparty_code_name != null)
            {
                string counter_party_code = model.counterparty_code_name.Split(':')[0];
                excelRow = sheet.CreateRow(rowIndex);
                excelTemplate.CreateCellHeaderLeft(excelRow, 0, "Counter Party : " + counter_party_code);
            }

            rowIndex++;

            // Add Header Table
            var colIndex = 0;
            excelRow = sheet.CreateRow(rowIndex);
            excelTemplate.CreateCellColHead(excelRow, colIndex, "Calc. Date");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Counter Party");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Counter Party Fund");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Trans No.");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Contract No.");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Int. Day");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Lend/Borrow");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Type");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Trade Date");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Purchase Date");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "RePurchase Date");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Repo Rate(%)");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Purchase Price");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Interest Amount");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Symbol");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "ISIN");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Unit");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Current Par");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "H(%)");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "VM(%)");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Dirty Price");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Market Price");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Uncollaterize Ratio");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Sec.Market Value");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Repo Int Return");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Cash Amount");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Aset Value");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Req Sec Value");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Collateral Exposure");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Transaction Exposure");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Call Margin Receive");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Call Margin Pay");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Close Margin");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Margin Position Yesterday");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Margin Balance");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Int. Rate(%)");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Interest on Cash Margin Receive");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Interest on Cash Margin Tax (Amt.)");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Interest on Cash Margin Paid");
            colIndex++;

            excelTemplate.CreateCellColHead(excelRow, colIndex, "Trans Status");
            excelRow.GetCell(colIndex).CellStyle.WrapText = true;
            excelRow.HeightInPoints = 45;
            var endColIndex = colIndex;

            // Add Data Rows
            //var endRowIndex = rowIndex;
            string calDate = string.Empty;
            string transNo = string.Empty;
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                colIndex = 0;
                rowIndex++;
                excelRow = sheet.CreateRow(rowIndex);

                if (dt.Rows[i]["calDate"] != null && dt.Rows[i]["calDate"].ToString() != string.Empty)
                {
                    excelTemplate.CreateCellColCenter(excelRow, colIndex, DateTime.Parse(dt.Rows[i]["calDate"].ToString()));
                }
                colIndex++;

                excelTemplate.CreateCellColLeft(excelRow, colIndex, dt.Rows[i]["counter_party_code"].ToString());
                colIndex++;

                excelTemplate.CreateCellColLeft(excelRow, colIndex, dt.Rows[i]["fundname"].ToString());
                colIndex++;

                excelTemplate.CreateCellColCenter(excelRow, colIndex, dt.Rows[i]["trans_no"].ToString());
                colIndex++;

                excelTemplate.CreateCellColLeft(excelRow, colIndex, dt.Rows[i]["BRPContractNo"].ToString());
                colIndex++;

                excelTemplate.CreateCellColCenter(excelRow, colIndex, dt.Rows[i]["DAY"].ToString());
                colIndex++;

                excelTemplate.CreateCellColCenter(excelRow, colIndex, dt.Rows[i]["lend_borrow"].ToString());
                colIndex++;

                excelTemplate.CreateCellColCenter(excelRow, colIndex, dt.Rows[i]["trade_type"].ToString());
                colIndex++;

                if (dt.Rows[i]["trade_date"] != null && dt.Rows[i]["trade_date"].ToString() != string.Empty)
                {
                    excelTemplate.CreateCellColCenter(excelRow, colIndex, DateTime.Parse(dt.Rows[i]["trade_date"].ToString()));
                }
                colIndex++;

                if (dt.Rows[i]["purchase_date"] != null && dt.Rows[i]["purchase_date"].ToString() != string.Empty)
                {
                    excelTemplate.CreateCellColCenter(excelRow, colIndex, DateTime.Parse(dt.Rows[i]["purchase_date"].ToString()));
                }
                colIndex++;

                if (dt.Rows[i]["repurchase_date"] != null && dt.Rows[i]["repurchase_date"].ToString() != string.Empty)
                {
                    excelTemplate.CreateCellColCenter(excelRow, colIndex, DateTime.Parse(dt.Rows[i]["repurchase_date"].ToString()));
                }
                colIndex++;

                double.TryParse(dt.Rows[i]["int_rate"].ToString(), out var intRate);
                excelTemplate.CreateCellColDecimalBucket(excelRow, colIndex, intRate, 6);
                colIndex++;

                if (calDate != dt.Rows[i]["calDate"].ToString() || transNo != dt.Rows[i]["trans_no"].ToString())
                {
                    double.TryParse(dt.Rows[i]["purchase_price"].ToString(), out var purchasePrice);
                    excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, purchasePrice);

                    calDate = dt.Rows[i]["calDate"].ToString();
                    transNo = dt.Rows[i]["trans_no"].ToString();
                }

                colIndex++;

                double.TryParse(dt.Rows[i]["InterestAmount"].ToString(), out var interestAmt);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, interestAmt);
                colIndex++;

                excelTemplate.CreateCellColLeft(excelRow, colIndex, dt.Rows[i]["bond_id"].ToString());
                colIndex++;

                excelTemplate.CreateCellColLeft(excelRow, colIndex, dt.Rows[i]["isincode"].ToString());
                colIndex++;

                double.TryParse(dt.Rows[i]["unit"].ToString(), out var unit);
                excelTemplate.CreateCellColNumber(excelRow, colIndex, unit);
                colIndex++;

                double.TryParse(dt.Rows[i]["current_par"].ToString(), out var currentPar);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, currentPar);
                colIndex++;

                double.TryParse(dt.Rows[i]["variationRatio"].ToString(), out var variationMargin);
                excelTemplate.CreateCellColDecimalBucket(excelRow, colIndex, variationMargin, 6);
                colIndex++;

                double.TryParse(dt.Rows[i]["variation_margin"].ToString(), out var hMargin);
                excelTemplate.CreateCellColDecimalBucket(excelRow, colIndex, hMargin, 6);
                colIndex++;

                double.TryParse(dt.Rows[i]["gross_price"].ToString(), out var grossPrice);
                excelTemplate.CreateCellColDecimalBucket(excelRow, colIndex, grossPrice, 6);
                colIndex++;

                double.TryParse(dt.Rows[i]["market_price"].ToString(), out var marketPrice);
                excelTemplate.CreateCellColDecimalBucket(excelRow, colIndex, marketPrice, 6);
                colIndex++;

                double.TryParse(dt.Rows[i]["Uncollaterize_Ratio"].ToString(), out var uncollaterizeRatio);
                excelTemplate.CreateCellColDecimalBucket(excelRow, colIndex, uncollaterizeRatio, 6);
                colIndex++;

                double.TryParse(dt.Rows[i]["sec_mtm_value"].ToString(), out var secMtmValue);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, secMtmValue);
                colIndex++;

                double.TryParse(dt.Rows[i]["accrue_int"].ToString(), out var accrueInt);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, accrueInt);
                colIndex++;

                double.TryParse(dt.Rows[i]["coll_cash_amt"].ToString(), out var collCashAmt);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, collCashAmt);
                colIndex++;

                double.TryParse(dt.Rows[i]["asset_value"].ToString(), out var assetValue);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, assetValue);
                colIndex++;

                double.TryParse(dt.Rows[i]["req_sec_value"].ToString(), out var reqSecValue);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, reqSecValue);
                colIndex++;

                double.TryParse(dt.Rows[i]["coll_exposure"].ToString(), out var collExposure);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, collExposure);
                colIndex++;

                double.TryParse(dt.Rows[i]["trans_exposure"].ToString(), out var transExposure);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, transExposure);
                colIndex++;

                double.TryParse(dt.Rows[i]["trans_margin_call_rec"].ToString(), out var transMarginCall);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, transMarginCall);
                colIndex++;

                double.TryParse(dt.Rows[i]["trans_margin_call_pay"].ToString(), out var transMarginPos);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, transMarginPos);
                colIndex++;

                double.TryParse(dt.Rows[i]["trans_close_margin"].ToString(), out var transCloseMargin);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, transCloseMargin);
                colIndex++;

                double.TryParse(dt.Rows[i]["trans_margin_pos_prev"].ToString(), out var transMarginPosPrev);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, transMarginPosPrev);
                colIndex++;

                double.TryParse(dt.Rows[i]["trans_margin_pos"].ToString(), out var marginBalance);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, marginBalance);
                colIndex++;

                double.TryParse(dt.Rows[i]["margin_int_rate"].ToString(), out var initMargin);
                excelTemplate.CreateCellColDecimalBucket(excelRow, colIndex, initMargin, 6);
                colIndex++;

                double.TryParse(dt.Rows[i]["trans_margin_int_rec"].ToString(), out var transMarginIntRec);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, transMarginIntRec);
                colIndex++;

                double.TryParse(dt.Rows[i]["trans_margin_int_tax"].ToString(), out var transMarginIntTax);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, transMarginIntTax);
                colIndex++;

                double.TryParse(dt.Rows[i]["trans_margin_int_pay"].ToString(), out var transMarginIntPay);
                excelTemplate.CreateCellCol2RedDecimal(excelRow, colIndex, transMarginIntPay);
                colIndex++;

                excelTemplate.CreateCellColCenter(excelRow, colIndex, dt.Rows[i]["status"].ToString());

                //endRowIndex = rowIndex;
            }

            if (dt.Rows.Count > 0)
            {
                rowIndex++;
                excelRow = sheet.CreateRow(rowIndex);
                for (var i = 0; i <= endColIndex; i++) excelTemplate.CreateCellFooter(excelRow, i, "");

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 12, 0, 2);
                excelRow.GetCell(12).SetCellFormula(string.Format("SUM(M7:M{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 13, 0, 2);
                excelRow.GetCell(13).SetCellFormula(string.Format("SUM(N7:N{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 23, 0, 2);
                excelRow.GetCell(23).SetCellFormula(string.Format("SUM(X7:X{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 24, 0, 2);
                excelRow.GetCell(24).SetCellFormula(string.Format("SUM(Y7:Y{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 25, 0, 2);
                excelRow.GetCell(25).SetCellFormula(string.Format("SUM(Z7:Z{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 26, 0, 2);
                excelRow.GetCell(26).SetCellFormula(string.Format("SUM(AA7:AA{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 27, 0, 2);
                excelRow.GetCell(27).SetCellFormula(string.Format("SUM(AB7:AB{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 28, 0, 2);
                excelRow.GetCell(28).SetCellFormula(string.Format("SUM(AC7:AC{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 29, 0, 2);
                excelRow.GetCell(29).SetCellFormula(string.Format("SUM(AD7:AD{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 30, 0, 2);
                excelRow.GetCell(30).SetCellFormula(string.Format("SUM(AE7:AE{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 31, 0, 2);
                excelRow.GetCell(31).SetCellFormula(string.Format("SUM(AF7:AF{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 32, 0, 2);
                excelRow.GetCell(32).SetCellFormula(string.Format("SUM(AG7:AG{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 33, 0, 2);
                excelRow.GetCell(33).SetCellFormula(string.Format("SUM(AH7:AH{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 34, 0, 2);
                excelRow.GetCell(34).SetCellFormula(string.Format("SUM(AI7:AI{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 36, 0, 2);
                excelRow.GetCell(36).SetCellFormula(string.Format("SUM(AK7:AK{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 37, 0, 2);
                excelRow.GetCell(37).SetCellFormula(string.Format("SUM(AL7:AL{0})", rowIndex));

                excelTemplate.CreateCellFooterDecimalBucket(excelRow, 38, 0, 2);
                excelRow.GetCell(38).SetCellFormula(string.Format("SUM(AM7:AM{0})", rowIndex));
            }

            for (var i = 0; i <= endColIndex; i++)
            {
                sheet.AutoSizeColumn(i);

                var colWidth = sheet.GetColumnWidth(i);
                if (colWidth < 2000)
                    sheet.SetColumnWidth(i, 2000);
                else
                    sheet.SetColumnWidth(i, 5000);
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
        }
    }
}