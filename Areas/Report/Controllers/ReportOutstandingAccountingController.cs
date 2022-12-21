using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GM.Application.Web.Areas.Report.Models;
using GM.CommonLibs;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.Report;
using GM.Data.Model.Static;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Areas.Report.Controllers
{
    public class ReportOutstandingAccountingController : Controller
    {
        private static readonly LogFile Log = new LogFile();
        private CounterPartyEntities api_counterparty = new CounterPartyEntities();
        private RPTransEntity api_deal = new RPTransEntity();
        private readonly MarketRateEntities api_MarketRate = new MarketRateEntities();

        private readonly StaticEntities api_static = new StaticEntities();
        private UserAndScreenEntities api_userandscreen = new UserAndScreenEntities();
        private readonly ReportEntities apiReport = new ReportEntities();
        private readonly ReportEntitiesModel reportentity = new ReportEntitiesModel();
        private readonly Utility utility = new Utility();

        public string port = string.Empty;

        //public int postingdate ;
        public string account_no;
        public string Report_Bank = "SiGG Financial Bank";
        public string Report_Code = string.Empty;
        public string Report_Date = string.Empty;

        public string Report_DateFrom = string.Empty;
        public string Report_DateFromTo = string.Empty;
        public string Report_DateTo = string.Empty;
        public string Report_File_Name = string.Empty;
        public string Report_Header = string.Empty;

        public string Report_Name = string.Empty;
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

        // GET: Report/ReportAccountingOutstanding
        public ActionResult Index()
        {
            ReportCriteriaModel model = new ReportCriteriaModel();
            model.type_date = "T";
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ReportCriteriaModel model, FormCollection collection)
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
                Report_File_Name = reportname_list[0].Text;
                Report_Header = "Outstanding Accounting Report‎";
                Report_Header += $" (Report No.{reportid})";

                model.asofdate = string.IsNullOrEmpty(model.asofdate_string)
                 ? model.asofdate
                 : utility.ConvertStringToDatetimeFormatDDMMYYYY(model.asofdate_string);

                Report_DateFromTo = (model.asofdate != null || model.trade_date_to != null ? "As of Date : " : "") +
                                    (model.asofdate == null ? "" : model.asofdate.Value.ToString("dd/MM/yyyy"));

                model.maturity_date_from =
                      string.IsNullOrEmpty(model.maturity_date_from_string)
                          ? model.maturity_date_from
                          : utility.ConvertStringToDatetimeFormatDDMMYYYY(model.maturity_date_from_string);
                model.maturity_date_to = string.IsNullOrEmpty(model.maturity_date_to_string)
                    ? model.maturity_date_to
                    : utility.ConvertStringToDatetimeFormatDDMMYYYY(model.maturity_date_to_string);

                Report_DateFromTo +=
                    (model.maturity_date_from != null || model.maturity_date_to != null
                        ? ", Maturity Date "
                        : "") +
                    (model.maturity_date_from == null
                        ? ""
                        : model.maturity_date_from.Value.ToString("dd/MM/yyyy")) +
                    (model.maturity_date_from != null && model.maturity_date_to != null
                        ? " to "
                        : "") + (model.maturity_date_to == null
                        ? ""
                        : model.maturity_date_to.Value.ToString("dd/MM/yyyy"));


                Report_DateFromTo +=
                   (model.account_code_from != null || model.account_code_from != null
                       ? ", Acct No "
                       : "") +
                   (model.account_code_from == null
                       ? ""
                       : model.account_code_from) +
                   (model.account_code_from != null && model.account_code_to != null
                       ? " to "
                       : "") + (model.account_code_to == null
                       ? ""
                       : model.account_code_to);

                var businessdate = reportentity.Getbusinessdate();
                var portfolio = string.IsNullOrEmpty(model.port_name) ? "Port : ALL" : "Port : " + model.port_name;
                var counter_party = string.IsNullOrEmpty(model.counterparty_code_name) ? "Counter Party : ALL" : "Counter Party : " + model.counterparty_code_name;
                dt = GetAccountingOutstanding(model);
                if (collection["PDF"] != null)
                {
                    var rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"), "OutstandingAccountingReport.rpt"));
                    rd.SetDataSource(dt);
                    rd.SetParameterValue("BusinessDate", businessdate);
                    rd.SetParameterValue("asOfDate", $"As of Date: {model.asofdate.Value.ToString("dd/MM/yyyy")}");
                    rd.SetParameterValue("maturityDate", (model.maturity_date_from != null || model.maturity_date_to != null
                        ? "Maturity Date "
                        : "") +
                    (model.maturity_date_from == null
                        ? ""
                        : model.maturity_date_from.Value.ToString("dd/MM/yyyy")) +
                    (model.maturity_date_from != null && model.maturity_date_to != null
                        ? " to "
                        : "") + (model.maturity_date_to == null
                        ? ""
                        : model.maturity_date_to.Value.ToString("dd/MM/yyyy")));
                    rd.SetParameterValue("acctNo", (model.account_code_from != null || model.account_code_from != null
                       ? "Acct No "
                       : "") +
                   (model.account_code_from == null
                       ? ""
                       : model.account_code_from) +
                   (model.account_code_from != null && model.account_code_to != null
                       ? " to "
                       : "") + (model.account_code_to == null
                       ? ""
                       : model.account_code_to));
                    rd.SetParameterValue("countParty", counter_party);
                    rd.SetParameterValue("report_id", reportid);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    var stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    return new FileStreamResult(stream, "application/pdf");
                }

                if (collection["Excel"] != null)
                {
                    var workbook = new HSSFWorkbook();
                    var sheet = workbook.CreateSheet("Outstanding Accounting");

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

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, counter_party);
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
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, portfolio);
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellHeaderLeft(excelRow, 0, $"System : Repo (Run date and time {DateTime.Now:dd/MM/yyyy HH:mm})");
                    rowIndex++;

                    excelRow = sheet.CreateRow(rowIndex);

                    // Add Header Table
                    excelTemplate.CreateCellColHead(excelRow, 0, "Ser. No");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Transaction No");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Contract No");
                    excelTemplate.CreateCellColHead(excelRow, 3, "MP");

                    excelTemplate.CreateCellColHead(excelRow, 4, "DATE");
                    excelTemplate.CreateCellColHead(excelRow, 5, "DATE");
                    excelTemplate.CreateCellColHead(excelRow, 6, "DATE");

                    excelTemplate.CreateCellColHead(excelRow, 7, "Counterparty");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Counterparty");

                    excelTemplate.CreateCellColHead(excelRow, 9, "Inst.");
                    excelTemplate.CreateCellColHead(excelRow, 10, "Inst Type‎");
                    excelTemplate.CreateCellColHead(excelRow, 11, "Deal CCY");
                    excelTemplate.CreateCellColHead(excelRow, 12, "Original Currency Amount");
                    excelTemplate.CreateCellColHead(excelRow, 13, "Accounting Rate");
                    excelTemplate.CreateCellColHead(excelRow, 14, "Base Amount (THB)");
                    excelTemplate.CreateCellColHead(excelRow, 15, "Rem. Tenor");
                    excelTemplate.CreateCellColHead(excelRow, 16, "Reference");
                    excelTemplate.CreateCellColHead(excelRow, 17, "Port");

                    rowIndex++;
                    excelRow = sheet.CreateRow(rowIndex);

                    excelTemplate.CreateCellColHead(excelRow, 0, "Ser. No");
                    excelTemplate.CreateCellColHead(excelRow, 1, "Transaction No");
                    excelTemplate.CreateCellColHead(excelRow, 2, "Contract No");
                    excelTemplate.CreateCellColHead(excelRow, 3, "MP");

                    excelTemplate.CreateCellColHead(excelRow, 4, "Deal");
                    excelTemplate.CreateCellColHead(excelRow, 5, "Value");
                    excelTemplate.CreateCellColHead(excelRow, 6, "Maturity");

                    excelTemplate.CreateCellColHead(excelRow, 7, "Code");
                    excelTemplate.CreateCellColHead(excelRow, 8, "Name");

                    excelTemplate.CreateCellColHead(excelRow, 9, "Inst.");
                    excelTemplate.CreateCellColHead(excelRow, 10, "Inst Type‎");
                    excelTemplate.CreateCellColHead(excelRow, 11, "Deal CCY");
                    excelTemplate.CreateCellColHead(excelRow, 12, "Original Currency Amount");
                    excelTemplate.CreateCellColHead(excelRow, 13, "Accounting Rate");
                    excelTemplate.CreateCellColHead(excelRow, 14, "Base Amount (THB)");
                    excelTemplate.CreateCellColHead(excelRow, 15, "Rem. Tenor");
                    excelTemplate.CreateCellColHead(excelRow, 16, "Reference");
                    excelTemplate.CreateCellColHead(excelRow, 17, "Port");

                    // Add Data Rows
                    try
                    {
                        int colIndex = 0;
                        string account_no = "";
                        double sum = 0;
                        string cur = "";
                        int rowNumber = 1;
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            if (account_no != "" && account_no != dt.Rows[i]["account_no"].ToString())
                            {
                                rowIndex++;
                                excelRow = sheet.CreateRow(rowIndex);

                                excelTemplate.CreateCellFooterCenter(excelRow, 9, "TOTAL");
                                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 9, 10));

                                excelTemplate.CreateCellFooterCenter(excelRow, 11, cur);

                                excelTemplate.CreateCellFooter2Decimal(excelRow, 12, sum);

                                sum = 0;
                            }
                            else if (i != 0 && (cur == "" || cur != dt.Rows[i]["currency_code"].ToString()))
                            {
                                rowIndex++;
                                excelRow = sheet.CreateRow(rowIndex);

                                excelTemplate.CreateCellFooterCenter(excelRow, 9, "TOTAL");
                                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 9, 10));

                                excelTemplate.CreateCellFooterCenter(excelRow, 11, cur);

                                excelTemplate.CreateCellFooter2Decimal(excelRow, 12, sum);

                                sum = 0;
                            }

                            if (account_no == "" || account_no != dt.Rows[i]["account_no"].ToString())
                            {
                                rowIndex++;
                                excelRow = sheet.CreateRow(rowIndex);

                                account_no = dt.Rows[i]["account_no"].ToString();

                                excelTemplate.CreateCellColLeft(excelRow, 0, dt.Rows[i]["account_no"].ToString() + " " + dt.Rows[i]["exp_acct_name"].ToString());
                                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 17));

                                rowNumber = 1;
                            }

                            rowIndex++;
                            colIndex = 0;
                            excelRow = sheet.CreateRow(rowIndex);

                            excelTemplate.CreateCellColCenter(excelRow, colIndex++, rowNumber++.ToString());
                            excelTemplate.CreateCellColCenter(excelRow, colIndex++, dt.Rows[i]["trans_no"].ToString());
                            excelTemplate.CreateCellColCenter(excelRow, colIndex++, dt.Rows[i]["bilateralcontractno"].ToString());
                            excelTemplate.CreateCellColCenter(excelRow, colIndex++, dt.Rows[i]["mp"].ToString());

                            if (dt.Rows[i]["trade_date"] != null && dt.Rows[i]["trade_date"].ToString() != string.Empty)
                            {
                                excelTemplate.CreateCellColCenter(excelRow, colIndex, DateTime.Parse(dt.Rows[i]["trade_date"].ToString()));
                            }
                            colIndex++;

                            if (dt.Rows[i]["settlement_date"] != null && dt.Rows[i]["settlement_date"].ToString() != string.Empty)
                            {
                                excelTemplate.CreateCellColCenter(excelRow, colIndex, DateTime.Parse(dt.Rows[i]["settlement_date"].ToString()));
                            }
                            colIndex++;

                            if (dt.Rows[i]["maturity_date"] != null && dt.Rows[i]["maturity_date"].ToString() != string.Empty)
                            {
                                excelTemplate.CreateCellColCenter(excelRow, colIndex, DateTime.Parse(dt.Rows[i]["maturity_date"].ToString()));
                            }
                            colIndex++;

                            excelTemplate.CreateCellColCenter(excelRow, colIndex++, dt.Rows[i]["custcode"].ToString());
                            excelTemplate.CreateCellColCenter(excelRow, colIndex++, dt.Rows[i]["engname"].ToString());

                            excelTemplate.CreateCellColCenter(excelRow, colIndex++, dt.Rows[i]["inst"].ToString());
                            excelTemplate.CreateCellColCenter(excelRow, colIndex++, dt.Rows[i]["inst_type"].ToString());

                            cur = dt.Rows[i]["currency_code"].ToString();
                            excelTemplate.CreateCellColCenter(excelRow, colIndex++, dt.Rows[i]["currency_code"].ToString());

                            double entry_amount = 0;
                            if (!string.IsNullOrEmpty(dt.Rows[i]["entry_amount"].ToString()) && !string.IsNullOrWhiteSpace(dt.Rows[i]["entry_amount"].ToString()))
                                entry_amount = double.Parse(dt.Rows[i]["entry_amount"].ToString());

                            sum += entry_amount;
                            excelTemplate.CreateCellCol2Decimal(excelRow, colIndex++, entry_amount);

                            double conv_rate = 0;
                            if (!string.IsNullOrEmpty(dt.Rows[i]["conv_rate"].ToString()) && !string.IsNullOrWhiteSpace(dt.Rows[i]["conv_rate"].ToString()))
                                conv_rate = double.Parse(dt.Rows[i]["conv_rate"].ToString());

                            excelTemplate.CreateCellCol2Decimal(excelRow, colIndex++, conv_rate);

                            double base_amount = 0;
                            if (!string.IsNullOrEmpty(dt.Rows[i]["base_amount"].ToString()) && !string.IsNullOrWhiteSpace(dt.Rows[i]["base_amount"].ToString()))
                                base_amount = double.Parse(dt.Rows[i]["base_amount"].ToString());

                            excelTemplate.CreateCellCol2Decimal(excelRow, colIndex++, base_amount);

                            excelTemplate.CreateCellColCenter(excelRow, colIndex++, dt.Rows[i]["tennor"].ToString());
                            excelTemplate.CreateCellColCenter(excelRow, colIndex++, dt.Rows[i]["deal_reference"].ToString());
                            excelTemplate.CreateCellColCenter(excelRow, colIndex++, dt.Rows[i]["portfolio"].ToString());


                            if ((i + 1) == dt.Rows.Count)
                            {
                                rowIndex++;
                                excelRow = sheet.CreateRow(rowIndex);

                                excelTemplate.CreateCellFooterCenter(excelRow, 9, "TOTAL");
                                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 9, 10));

                                excelTemplate.CreateCellFooterCenter(excelRow, 11, cur);

                                excelTemplate.CreateCellFooter2Decimal(excelRow, 12, sum);

                                sum = 0;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    for (var i = 0; i <= 17; i++)
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

                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 15));
                    sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(5, 5, 0, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(6, 6, 0, 7));

                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 0, 0));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 1, 1));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 2, 2));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 3, 3));

                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 4, 6));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 7, 7, 8));

                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 9, 9));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 10, 10));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 11, 11));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 12, 12));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 13, 13));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 14, 14));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 15, 15));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 16, 16));
                    sheet.AddMergedRegion(new CellRangeAddress(7, 8, 17, 17));

                    sheet.AddMergedRegion(new CellRangeAddress(8, 8, 4, 4));
                    sheet.AddMergedRegion(new CellRangeAddress(8, 8, 5, 5));
                    sheet.AddMergedRegion(new CellRangeAddress(8, 8, 6, 6));
                    sheet.AddMergedRegion(new CellRangeAddress(8, 8, 7, 7));
                    sheet.AddMergedRegion(new CellRangeAddress(8, 8, 8, 8));

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

        public DataTable GetAccountingOutstanding(ReportCriteriaModel data)
        {
            var dt = new DataTable();

            apiReport.ReportData.OutstandingAccountingReport(data, p =>
            {
                if (p.Success && p.Data != null)
                {
                    dt = p.Data.OutstandingAccountingReportResultModel
                    .OrderBy(x => x.account_no).ThenBy(x => OrderCur(x.currency_code)).ThenBy(x => x.trans_no).ToList().ToDataTable();
                }
            });

            return dt;
        }

        private int OrderCur(string cur)
        {
            int order = 999;
            switch (cur)
            {
                case "THB": { order = 0; } break;
                case "USD": { order = 1; } break;
                case "GBP": { order = 2; } break;
                case "JPY": { order = 4; } break;
                case "SGD": { order = 5; } break;
                case "HKD": { order = 6; } break;
            }
            return order;
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

        public ActionResult FillCounterParty(string datastr)
        {
            var res = new List<DDLItemModel>();
            api_counterparty.CounterPartyFund.GetDDLCounterParty(datastr, p =>
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
    }
}