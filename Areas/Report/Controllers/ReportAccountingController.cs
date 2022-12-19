using System;
using System.Collections.Generic;
using System.Data;
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
    public class ReportAccountingController : BaseController
    {
        private static readonly string Controller = "AccountingReportController";
        private static readonly LogFile Log = new LogFile();
        private CounterPartyEntities api_counterparty = new CounterPartyEntities();
        private RPTransEntity api_deal = new RPTransEntity();
        private readonly MarketRateEntities api_MarketRate = new MarketRateEntities();

        private readonly StaticEntities api_static = new StaticEntities();
        private UserAndScreenEntities api_userandscreen = new UserAndScreenEntities();
        private readonly ReportEntities apiReport = new ReportEntities();
        public string costcenter = string.Empty;
        public string port = string.Empty;

        public string postingdate = string.Empty;

        public string Report_Bank = "SiGG Financial Bank";
        public string Report_Code = string.Empty;
        public string Report_Date = string.Empty;
        public string Report_DateFromTo = string.Empty;
        public string Report_File_Name = string.Empty;
        public string Report_Header = string.Empty;
        private readonly ReportEntitiesModel reportentity = new ReportEntitiesModel();
        public double sum_cr_costcenter;
        public double sum_cr_port;
        public double sum_cr_postingdate;
        public double sum_dr_costcenter;
        public double sum_dr_port;
        public double sum_dr_postingdate;
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
                        .ToString();  //Request.QueryString["controllername"];  
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
                Report_Header = "General Ledger Summary Report";

                model.trade_date_from = string.IsNullOrEmpty(model.trade_date_from_string)
                    ? model.trade_date_from
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(model.trade_date_from_string);

                model.trader_date_to = string.IsNullOrEmpty(model.trader_date_to_string)
                    ? model.trader_date_to
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(model.trader_date_to_string);

                Report_DateFromTo = (model.trader_date_from != null || model.trader_date_to != null ? "Date : " : "") +
                                    (model.trans_date_from == null
                                        ? ""
                                        : model.trade_date_from.Value.ToString("dd/MM/yyyy")) +
                                    (model.trade_date_from != null && model.trade_date_to != null ? " - " : "") +
                                    (model.trade_date_to == null
                                        ? ""
                                        : model.trade_date_to.Value.ToString("dd/MM/yyyy"));
                Report_Header += $" (Report No.{reportid})";

                var businessdate = reportentity.Getbusinessdate();
                var portfolio = string.IsNullOrEmpty(model.port_name) ? "Port : ALL" : "Port : " + model.port_name;
                if (collection["PDF"] != null)
                    try
                    {
                        dt = GetAccounting(model);
                        if (dt != null || dt.Rows.Count > 0)
                        {
                            var rd = new ReportDocument();
                            var ReportPath = Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),
                                "AccountReport.rpt");
                            Log.WriteLog(Controller, "Load Report = " + ReportPath);
                            rd.Load(ReportPath);
                            Log.WriteLog(Controller, "Load Report Success");
                            if (dt.Rows.Count <= 0)
                            {
                                var tmp = new List<AccountingReportModel>();
                                dt = tmp.ToDataTable();
                            }

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
                        }

                        ViewBag.ErrorMessage = "Cannot get data from resource.";
                        return new ContentResult
                        {
                            Content = ViewBag.ErrorMessage
                        };
                        //return File(stream, "aplication/pdf", "SettlementProductControlReport.pdf");  
                    }
                    catch (Exception ex)
                    {
                        return new ContentResult
                        {
                            Content = ex.ToString()
                        };
                    }

                if (collection["Excel"] != null)
                {
                    dt = GetAccounting(model);
                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("General Ledger Summary");
                    var excelTemplate = new ExcelTemplate(workbook);

                    // Add Header
                    var row Index = 0;
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

                    string ownerEndUser = string.Empty;
                    List<ConfigModel> listConfig = reportentity.GetReportHeader(reportid);
                    if (listConfig != null && listConfig.Count > 0)
                    {
                        ownerEndUser = ListConfig[0].item_value;
                    }

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, ownerEndUser);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, portfolio);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, $"System : Repo (Run date and time {DateTime.Now:dd/MM/yyyy HH:mm})");
                    rowIndex++;


                    excelRow = sheet.CreateRow(rowIndex);


                    // Add Header Table 
                    excelTemplate.CreateCellColHead(excelRow, 0, "No.");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Creation Date");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Event Type");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Account No.");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Account Name");
                    excelTemplate.CreateCellColHead(excelRow, 5, "Amount");
                    excelTemplate.CreateCellColHead(excelRow, 6, "Amount");

                    rowIndex++;
                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellColHead(excelRow, 0, "No.");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Creation Date");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Event Type");
                    excelTemplate.CreateCellColHead(excelRow, 3, "Account No.");
                    excelTemplate.CreateCellColHead(excelRow, 4, "Account Name");
                    excelTemplate.CreateCellColHead(excelRow, 5, "DR");
                    excelTemplate.CreateCellColHead(excelRow, 6, "CR");


                    double sum_dr_graint = 0;
                    double sum_cr_graint = 0;

                    // Add Data Rows  
                    var rownumber = 0;
                    try
                    {
                        //Set: Header Posting Date  
                        var result_postingdate = (from p in dt.AsEnumerable()
                                                  group p by p["posting_date"]
                            into r
                                                  select new
                                                  {
                                                      ID = r.Key,
                                                      dr_amount = r.Sum(s => double.Parse(s["dr_amount"].ToString())),
                                                      cr_amount = r.Sum(s => double.Parse(s["cr_amount"].ToString())),
                                                      description = r.Select(s => s.Field<string>("description")),
                                                      account_no = r.Select(s => s.Field<string>("account_no")),
                                                      account_name = r.Select(s => s.Field<string>("account_name")),
                                                      costcenter = r.Select(s => s.Field<string>("cost_center"))
                                                  }).ToList();
                        foreach (var item in result_postingdate)
                        {
                            if (item.ID != DBNull.Value)
                            {
                                postingdate = item.ID.ToString();
                                sum_dr_postingdate = item.dr_amount;
                                sum_cr_postingdate = item.cr_amount;
                            }

                            sum_dr_graint += sum_dr_postingdate;
                            sum_cr_graint += sum_cr_postingdate;

                            //Set: Header CostCenter
                            var result_costcenter = (from p in dt.AsEnumerable()
                                                     where utility.ConvertDatetimeToDateFormatDDMMYYYY(p["posting_date"].ToString()) ==
                                                           utility.ConvertDatetimeToDateFormatDDMMYYYY(postingdate)
                                                     group p by p["cost_center"]
                                into r
                                                     select new
                                                     {
                                                         ID = r.Key,

                                                         dr_amount = r.Sum(s => double.Parse(s["dr_amount"].ToString())),
                                                         cr_amount = r.Sum(s => double.Parse(s["cr_amount"].ToString())),
                                                         description = r.Select(sbyte => s.Field<string> < "description" >));
                            account_no = r.Select(string.Field<string>("account_no")),  
                                                         account_name = ref.Select(string => string.Field<string>("account_name"))
                                }).ToList();

                        foreach (var item_costcenter in result_costcenter)
                        {
                            if (item_costcenter.ID != DBNull.Value)
                            {
                                costcenter = item_costcenter.ID.ToString();
                                sum_dr_costcenter = item_costcenter.dr_amount;
                                sum_cr_costcenter = item_costcenter.cr_amount;
                            }

                            //Header Cost Center  
                            rowIndex++;
                            excelRow = sheet.CreateRow(rowIndex);
                            //excelTemplate.CreateCellColHead(excelRow, 0, "Cost Center :" = costcenter);  
                            var Headerleft = excelTemplate.CellStyle(FontBoldWeight.Bold,
                                VerticalAlignment.Center, HorizontalAlignment.Left,
                                borderColor: 22, foregroundColor: 48, fontColor HSSFColor.White.Index);

                            excelTemplate.CreateCellCustomStyle(excelRow, 0, "Cost Center :" + costcenter,
                                Headerleft);
                            excelTemplate.CreateCellCustomStyle(excelRow, 1, "Cost Center :" + costcenter,
                                Headerleft);
                            excelTemplate.CreateCellCustopmStyle(excelRow, 2, "Cost Center :" + costcenter,
                                Headerleft);
                            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));

                            //Set: Header Port
                            var result_port = (from p in dt.AsEnumerable()
                                               where utility.ConvertDatetimeToDateFormatDDMMYYYY(p["posting_date"].ToString()) ==
                                                     utility.ConvertDatetimeToDateFormatDDMMYYYY(postingdate)
                                                     && p["cost_center"].ToString() == costcenter
                                               group p by p["port"]
                                    into r
                                               select new
                                               {
                                                   ID = r.Key,
                                                   dr_amount = r.Sum(s => double.Parse(s["dr_amount"].ToString())),
                                                   cr_amount = r.Sum(s => double.Parse(s["cr_amo0unt"].ToString()))
                                               }).toList();

                            foreach (var item_port in result_port)
                            `       {
                                if (item_port.ID != DBNull.Value)
                                {
                                    port = item_port.ID.ToString();
                                    sum_dr_port = item_port.dr_amount;
                                    sum_cr_port = item_port.cr_amount;
                                }

                                //Head Port
                                rowIndex++;
                                excelRow = sheet.CreateRow(rowIndex);
                                // excelTemplate.CreateCellColHead(excelRow, 0, "Port : " + port);  
                                excelTemplate.CreateCellCustomStyle(excelRow, 0, "Port : " + port, Headerleft);
                                excelTemplate.CreateCellCustomStyle(excelRow, 1, "Port : " + port, Headerleft);
                                excelTemplate.CreateCellCustomStyle(excelRow, 2, "Port : " + port, Headerleft);

                                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 2));

                                //Set: Detail  
                                var result_detail = dt.AsEnumerable().Where(s =>
                                    s.Field<string>("cost_center") == costcenter
                                    && utility.ConvertDatetimeToDateFormatDDMMYYYY(s["posting_date"]
                                        .ToString()) == utility.ConvertDatetimeToDateFormatDDMMYYYY(postingdate)
                                    && s.Field<string>("port") == port).CopyToDataTable();

                                for (var i = 0; i < result_detail.Rows.Count; i++)
                                {
                                    rownumber += 1;
                                    rowIndex++;
                                    excelRow = sheet.CreateRow(rowIndex);

                                    excelTemplate.CreateCellColCenter(excelRow, 0, rownumber.ToString());

                                    if (result_detail.Rows[i]["posting_date"].ToString() != string.Empty)
                                    {
                                        excelTemplate.CreateCellColCenter(excelRow, 1, DateTime.Parse(result_detail.Rows[i]["posting_date"].ToString()));
                                    }
                                    else
                                    {
                                        excelTemplate.CreateCellColCenter(excelRow, 1, new DateTime());
                                    }

                                    excelTemplate.CreateCellColCenter(excelRow, 2,
                                        result_detail.Rows[i]["description"].ToString());
                                    excelTemplate.CreateCellColCenter(excelRow, 3,
                                        result_detail.Rows[i]["account_no"].ToString());
                                    excelTemplate.CreateCellColLeft(excelRow, 4,
                                        result_detail.Rows[i]["account_name"].ToString());

                                    double dr_amount = 0;
                                    if (result_detail.Rows[i]["cr_amount].ToString() != string.Empty)  
                                            dr_amount = double.Parse(result_detail.Rows[i]["dr_amount"].ToString());
                                    excelTemplate.CreateCellCol2Decimal(excelRow, 5, dr_amount);

                                    double cr_amount = 0;
                                    if (result_detail_detail.Rows[i]["cr_amount"].ToString() != string.Empty)
                                        cr_amount = double.Parse(result_detail.Rows[i]["cr_amount"].ToString());
                                    excelTemplate.CreateCellCol2Decimal(excelRow, 6, cr_amount);
                                }

                                rowIndex++;
                                excelRow = sheet.CreateRow(rowIndex);
                                excelTemplate.CreateCellFooterRight(excelRow, 4, "Total Port : " + port);
                                excelTemplate.CreateCellFooter2Decimal(excelRow, 5, sum_dr_port);
                                excelTemplate.CreateCellFooter2Decimal(excelRow, 6, sum_cr_port);
                            }

                            rowIndex++;
                            excelRow = sheet.CreateRow(rowIndex);
                            excelTemplate.CreateCellFooterRight(excelRow, 4, "Total Cost Center : " + costcenter);
                            excelTemplate.CreateCellFooter2Decimal(excelRow, 5, sum_dr_costcenter);
                            excelTemplate.CreateCellFooter2Decimal(excelRow, 6, sum_cr_costcenter);
                        }

                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);
                        excelTemplate.CreateCellFooterRight(excelRow, 4, "Total AsOf ; " + postingdate);
                        excelTemplate.CreateCellFooter2Decimal(excelRow, 5, sum_dr_postingdate);
                        excelTemplate.CreateCellFooter2Decimal(excelRow, 6, sum_cr_postingdate);

                    }
                }
                catch
            {

            }

            rowIndex++;
            excelRow = sheet.CreateRow(rowIndex);
            excelTemplate.CreateCellFooterRight(excelRow, 4, "Grand Summary");
            excelTemplate.CreateCellFooter2Decimal(excelRow, 5, sum_dr_graint);
            excelTemplate.CreateCellFooter2Decimal(excelRow, 6, sum_cr_graint);

            for (var i = 0; i <= 6; i++)
            {
                sheet.AutoSizeColumn(i);

                if (i == 0)
                {
                    sheet.SetColumnWidth(i, 5000);
                }
                else
                {
                    var colWidth = sheet.GEtColumnWidth(i);
                    if (colWidth < 2000)
                        Sheet.SetcolumnWidth(i, 2000);
                    else
                        Sheet.SetColumnWidth(i, colWidth + 200);
                }
            }

            // Set Merge Cells Header Report_Bank  
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 4));
            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 4));
            sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 4));
            sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 4));

            if (!string.IsNullOrEmpty(Report_DateFromTo))
            {
                sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 4));
                sheet.AddMergedRegion(new CellRangeAddress(5, 5, 0, 4));

                sheet.AddMergedRegion(new CellRangeAddress(6, 7, 0, 0));
                sheet.AddMergedRegion(new CellRangeAddress(6, 7, 1, 1));
                sheet.AddMergedRegion(new CellRangeAddress(6, 7, 2, 2));
                sheet.AddMergedRegion(new CellRangeAddress(6, 7, 3, 3));
                sheet.AddMergedRegion(new CellRangeAddress(6, 7, 4, 4));
                sheet.AddMergedRegion(new CellRangeAddress(6, 7, 5, 5));
                sheet.AddMergedRegion(new CellRangeAddress(6, 7, 6, 6));
            }
            else
            {
                sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 4));
                sheet.AddMergedRegion(new CellRangeAddress(5, 6, 1, 1));
                sheet.AddMergedRegion(new CellRangeAddress(5, 6, 2, 2));
                sheet.AddMergedRegion(new CellRangeAddress(5, 6, 3, 3));
                sheet.AddMergedRegion(new CellRangeAddress(5, 6, 4, 4));
                sheet.AddMergedRegion(new CellRangeAddress(5, 5, 5, 6));
                sheet.AddMergedRegion(new CellRangeAddress(6, 6, 5, 5));
                sheet.AddMergedRegion(new CellRangeAddress(6, 6, 6, 6));
            }

            var exportfile = new MemoryStream();
            workbook.Write(exportfile);
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();

            Response.AppendHeader("Content-Type", "application/vnd.ms-excel");
            Response.AppendHeader("Cache-Control", "max-age=30");
            Response.AppendHeader("Pragma", "public");
            Response.AppendHeader("Content-disposition", "attacment; filename=" + Report_File_Name + ".xls");

            Response.BinaryWrite(exportfile.GetBuffer());
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
            return View();
        }

                    return ViewContext();
        }
        catch (Exception ex)  
        {
            Log.WriteLog(Controller, "Load Fail : " + ex.Message);    
            ViewBag.ErrorMessage = ex.Message;  
            return ViewContext();
            }
        }

        public DataTable GetAccounting(ReportCriteriaModel data)
        {
            var dt = new DataTable();

                apiReport.ReportData.AccountingReport(data, p =>
                {
                    if (p.Success && p.Data.DDLItems;
                });
                return Json(ResolveEventArgs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillCurrency(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_static.Config.GetPortForReportDDL(datastr, prop =>
            {
                if (p.Success) res = prop.Data.DDLItems;
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

        //DDl Account Event Status  
        public ActionResult FillAccountEvent(String datastr)
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
                DataTime? Date = new DateTime();
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