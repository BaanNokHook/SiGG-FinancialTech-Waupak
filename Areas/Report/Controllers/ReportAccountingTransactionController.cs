using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
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
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace GM.Application.Web.Areas.Report.Controllers
{
   [Authorize]  
   [Audit]  
   public class ReportAccountingTransactionController : BaseController  
   {
        private CounterPartyEntities api_counterparty = new CounterPartyEntities();   
        private RPTransEntity api_deal = new RPTransEntity();  
        private readonly MarketRateEntities api_MarketRate =new MarketRateEntities();   

        private readonly StaticEntities api_static = new StaticEntities();  
        private UserAndScreenEntities api_userandscreen = new UserAndScreenEntities();  
        private readonly ReportEntities apiReport = new ReportEntities();   
        public string costcenter = string.Empty;  

        public string port = string.Empty;  

        //public int postingdate ;  
        public DateTime postingdate;  
        public string Report_Bank = "SiGG Financial Bank";
        public string Report_Code = string.Empty;
        public string Report_Date = string.Empty;

        public string Report_DateFrom = string.Empty;
        public string Report_DateFromTo = string.Empty;
        public string Report_DateTo = string.Empty;
        public string Report_File_Name = string.Empty;
        public string Report_Header = string.Empty;

        public string Report_Name = string.Empty;
        private readonly ReportEntitiesModel reportentity = new ReportEntitiesModel();
        public string sec_type = string.Empty;
        public double sum_cr_costcenter;
        public double sum_cr_port;
        public double sum_cr_postingdate;
        public double sum_cr_trans;
        public double sum_dr_costcenter;
        public double sum_dr_port;
        public double sum_dr_postingdate;
        public double sum_dr_trans;
        public string trans_no = string.Empty;
        private readonly Utility utility = new Utility();

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(ReportCriteriaModel model, FormCollection collection)
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
                Report_Header = "General Ledger Transaction Journal Report ‎";

                model.trade_date_from = string.IsNullOrEmpty(model.trade_date_from_string)
                    ? model.trade_date_from
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(model.trade_date_from_string);
                model.trade_date_to = string.IsNullOrEmpty(model.trade_date_to_string)
                    ? model.trade_date_to
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(model.trade_date_to_string);

                Report_DateFromTo = (model.trade_date_from != null || model.trade_date_to != null ? "Date : " : "") +
                                    (model.trade_date_from == null
                                        ? ""
                                        : model.trade_date_from.Value.ToString("dd/MM/yyyy")) +
                                    (model.trade_date_from != null && model.trade_date_to != null ? " - " : "") +
                                    (model.trade_date_to == null
                                        ? ""
                                        : model.trade_date_to.Value.ToString("dd/MM/yyyy"));

                Report_Header += $" (Report No.{reportid})";

                var businessdate = reportentity.Getbusinessdate();
                var portfolio = string.IsNullOrEmpty(model.port_name) ? "Port : ALL" : "Port : " + model.port_name;
                dt = GetAccountingTransaction(model);
                if (collection["PDF"] != null)
                {
                    var rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),
                        "GLTransJournalReport.rpt"));
                    rd.SetDataSource(dt);
                    rd.SetParameterValue("BusinessDate", businessdate);
                    rd.SetParameterValue("Date", Report_DateFromTo);
                    rd.SetParameterValue("report_id", reportid);
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
                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("General Ledger Summary");

                    var excelTemplate = new ExcelTemplate(workbook);

                    // Add Header 
                    var rowIndex = 0;
                    var excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_Bank);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_Header);
                    rowIndex++;

                    if (!string.IsNullOrEmpty(Report_DateFromTo))
                    {
                        excelRow = sheet.CreateRow(rowIndex);
                        excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_DateFromTo);
                        rowIndex++;
                    }

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
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, portfolio);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, $"System : Repo (Run date and time {DateTime.Now:dd/MM/yyyy HH:mm})");
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);

                    // Add Header Table
                    excelTemplate.CreateCellColHead(excelRow, 0, "Creation Date");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Tran No.");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Event Type");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Account Number");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Account Name");
                    excelTemplate.CreateCellColHead(excelRow, 5, "DR/CR");
                    excelTemplate.CreateCellColHead(excelRow, 6, "DR Amount");
                    excelTemplate.CreateCellColHead(excelRow, 7, "CR Amount");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Cur");
                    excelTemplate.CreateCellColHead(excelRow, 9, "Buy‎/‎Sell");
                    excelTemplate.CreateCellColHead(excelRow, 10, "Adj Num‎");
                    excelTemplate.CreateCellColHead(excelRow, 11, "Sequence‎ Number‎");
                    excelTemplate.CreateCellColHead(excelRow, 12, "Cost Center");

                    // Add Data Rows
                    var rowmergeport = 0;
                    double sum_dr_graint = 0;
                    double sum_cr_graint = 0;

                    try
                    {
                        var result_postingdate = (from p in dt.AsEnumerable()
                                                  group p by (DateTime)p["posting_date"]
                            into r
                                                  //group p by (p["posting_date"]) into r
                                                  //group p by new { week = (p["posting_date"])} into r
                                                  select new
                                                  {
                                                      ID = r.Key,
                                                      Group = r,
                                                      dr_amount = r.Sum(s => double.Parse(s["dr_amount"].ToString())),
                                                      cr_amount = r.Sum(s => double.Parse(s["cr_amount"].ToString()))
                                                  }).ToList();

                        foreach (var item in result_postingdate)
                        {
                            if (item.ID != null)
                            {
                                postingdate = item.ID;
                                sum_dr_postingdate = item.dr_amount;
                                sum_cr_postingdate = item.cr_amount;
                            }

                            sum_dr_graint += sum_dr_postingdate;
                            sum_cr_graint += sum_cr_postingdate;

                            //Set: Header Port
                            var result_port = (from p in dt.AsEnumerable()
                                               where (DateTime)p["posting_date"] == postingdate
                                               group p by p["port"]
                                into r
                                               select new
                                               {
                                                   ID = r.Key,
                                                   dr_amount = r.Sum(s => double.Parse(s["dr_amount"].ToString())),
                                                   cr_amount = r.Sum(s => double.Parse(s["cr_amount"].ToString()))
                                               }).ToList();

                            foreach (var item_port in result_port)
                            {
                                if (item_port.ID != DBNull.Value)
                                {
                                    port = item_port.ID.ToString();
                                    sum_dr_port = item_port.dr_amount;
                                    sum_cr_port = item_port.cr_amount;
                                }

                                //Header Port
                                rowIndex++;
                                excelRow = sheet.CreateRow(rowIndex);
                                var Hederleft = excelTemplate.CellStyle(FontBoldWeight.Bold,
                                    VerticalAlignment.Center, HorizontalAlignment.Left,
                                    borderColor: 22, foregroundColor: 48, fontColor: HSSFColor.White.Index);

                                excelTemplate.CreateCellCustomStyle(excelRow, 0, "Port : " + port, Hederleft);
                                // excelTemplate.CreateCellColHead(excelRow, 0, "Port : " + port);

                                // Set: Trans No
                                var result_trans = (from p in dt.AsEnumerable()
                                                        //where p["posting_date"].ToString() == postingdate
                                                    where (DateTime)p["posting_date"] == postingdate
                                                          && p["port"].ToString() == port //&& p["sec_type"].ToString() == sec_type
                                                    group p by p["trans_no"]
                                    into r
                                                    select new
                                                    {
                                                        ID = r.Key,
                                                        dr_amount = r.Sum(s => double.Parse(s["dr_amount"].ToString())),
                                                        cr_amount = r.Sum(s => double.Parse(s["cr_amount"].ToString()))
                                                    }).ToList();

                                foreach (var item_trans in result_trans)
                                {
                                    if (item_trans.ID != DBNull.Value)
                                    {
                                        trans_no = item_trans.ID.ToString();
                                        sum_dr_trans = item_trans.dr_amount;
                                        sum_cr_trans = item_trans.cr_amount;
                                    }

                                    var result_detail = dt.AsEnumerable().Where(s =>
                                        s.Field<DateTime>("posting_date") == postingdate
                                        && s.Field<string>("port") == port
                                        //&& s.Field<string>("sec_type") == sec_type
                                        && s.Field<string>("trans_no") == trans_no).CopyToDataTable();

                                    for (var i = 0; i < result_detail.Rows.Count; i++)
                                    {
                                        rowIndex++;
                                        excelRow = sheet.CreateRow(rowIndex);

                                        if (result_detail.Rows[i]["posting_date"].ToString() != string.Empty)
                                        {
                                            excelTemplate.CreateCellColCenter(excelRow, 0, DateTime.Parse(result_detail.Rows[i]["posting_date"].ToString()));
                                        }
                                        else
                                        {
                                            excelTemplate.CreateCellColCenter(excelRow, 0, new DateTime());
                                        }

                                        excelTemplate.CreateCellColCenter(excelRow, 1,
                                            result_detail.Rows[i]["trans_no"].ToString());
                                        excelTemplate.CreateCellColCenter(excelRow, 2,
                                            result_detail.Rows[i]["event_type"].ToString());
                                        excelTemplate.CreateCellColCenter(excelRow, 3,
                                            result_detail.Rows[i]["account_no"].ToString());
                                        excelTemplate.CreateCellColLeft(excelRow, 4,
                                            result_detail.Rows[i]["account_name"].ToString());
                                        excelTemplate.CreateCellColCenter(excelRow, 5,
                                            result_detail.Rows[i]["dr_cr"].ToString());

                                        double dr_amount = 0;
                                        if (result_detail.Rows[i]["dr_amount"].ToString() != string.Empty)
                                            dr_amount = double.Parse(result_detail.Rows[i]["dr_amount"].ToString());
                                        excelTemplate.CreateCellCol2Decimal(excelRow, 6, dr_amount);
                                        double cr_amount = 0;
                                        if (result_detail.Rows[i]["cr_amount"].ToString() != string.Empty)
                                            cr_amount = double.Parse(result_detail.Rows[i]["cr_amount"].ToString());
                                        excelTemplate.CreateCellCol2Decimal(excelRow, 7, cr_amount);

                                        excelTemplate.CreateCellColCenter(excelRow, 8,
                                            result_detail.Rows[i]["cur"].ToString());
                                        excelTemplate.CreateCellColCenter(excelRow, 9,
                                            result_detail.Rows[i]["buy_sell"].ToString());
                                        excelTemplate.CreateCellColCenter(excelRow, 10,
                                            result_detail.Rows[i]["adj_num"].ToString());
                                        excelTemplate.CreateCellColCenter(excelRow, 11,
                                            result_detail.Rows[i]["sequence_num"].ToString());
                                        excelTemplate.CreateCellColCenter(excelRow, 12,
                                            result_detail.Rows[i]["cost_center"].ToString());
                                    }
                                    //End Set Detail

                                    rowIndex++;
                                    excelRow = sheet.CreateRow(rowIndex);
                                    excelTemplate.CreateCellFooterCenter(excelRow, 5, "Sub Total");
                                    excelTemplate.CreateCellFooter2Decimal(excelRow, 6, sum_dr_trans);
                                    excelTemplate.CreateCellFooter2Decimal(excelRow, 7, sum_cr_trans);
                                }

                                //}

                                rowIndex++;
                                excelRow = sheet.CreateRow(rowIndex);
                                excelTemplate.CreateCellFooterCenter(excelRow, 5, "Total Amount");
                                excelTemplate.CreateCellFooter2Decimal(excelRow, 6, sum_dr_port);
                                excelTemplate.CreateCellFooter2Decimal(excelRow, 7, sum_cr_port);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var test = ex.Message;
                    }


                    for (var i = 0; i <= 12; i++)
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

                    // Set Merge Cells Header Report_Banak                    

                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 4));

                    sheet.AddMergedRegion(new CellRangeAddress(rowmergeport, rowmergeport, 0, 4));

                    if (!string.IsNullOrEmpty(Report_DateFromTo))
                        sheet.AddMergedRegion(new CellRangeAddress(5, 5, 0, 4));

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
                return View(ex);
            }
        }

        public DataTable GetAccountingTransaction(ReportCriteriaModel data)
        {
            var dt = new DataTable();

            apiReport.ReportData.AccountingTransactionReport(data, p =>
            {
                if (p.Success) dt = p.Data.AccountingTransactionReportResultModel.ToDataTable();
            });

            return dt;
        }

        public ActionResult FillInstrumentCode(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_MarketRate.RPReferece.GetDDLInstrument(datastr, p =>
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

        //DDL Account Group
        public ActionResult FillAccountGroup(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_static.Accounting.GetDDLAccountGroup(datastr, p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        //DDL Account Group
        public ActionResult FillAccountCode(string datastr)
        {
            var res = new List<DDLItemModel>();
            GLProcessEntities api = new GLProcessEntities();
            api.GLAdjust.GetDDlAdjustAccountCode(datastr, p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        //DDL Account Event Status
        public ActionResult FillAccountEvent(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_static.Accounting.GetDDLAccountEvent(datastr, p =>
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

        public ActionResult FillCounterParty(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_counterparty.CounterPartyFund.GetDDLCounterParty(datastr, p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        //DDL Event Type
        public ActionResult FillEventType(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_static.Accounting.GetDDLGlEventType(datastr, p =>
            {
                if (p.Success) res = p.Data.DDLItems;
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public int getWeek(DateTime dt)
        {
            var date = new DateTime(dt.Year, dt.Month, dt.Day);
            var norwCulture =
                CultureInfo.CreateSpecificCulture("no");
            var cal = norwCulture.Calendar;
            var weekNo = cal.GetWeekOfYear(date,
                norwCulture.DateTimeFormat.CalendarWeekRule,
                norwCulture.DateTimeFormat.FirstDayOfWeek);
            return weekNo;
        }
    }
}