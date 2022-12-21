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
using System.Data;
using System.IO;
using System.Web.Mvc;

namespace GM.Application.Web.Areas.Report.Controllers
{
    
    [Authorize]   
    [Audit]  
    public class ReportMaturityController : BaseController
    {
        private readonly CounterPartyEntities api_counterparty = new CounterPartyEntities();   
        private readonly RPTransEntity api_deal = new RPTransEntity();  
        private readonly SecurityEntities api_security = new SecurityEntities();  
        private readonly StaticEntities api_static = new StaticEntities();   
        private readonly ReportEntities apiReport = new ReportEntities();   
        public string Report_Bank = "SiGG Financial Bank";  
        public string Report_Code = string.Empty;  
        public string Report_DateFromTo = string.Empty;  
        public string Report_File_Name = string.Empty;   
        public string Report_Header = string.Empty;     

        public string Report_Name = string.Empty;   
        private readonly ReportEntitiesModel reportentity = new ReportEntitiesModel();    

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
            Report_Header = "Maturity Report : Transaction " + (reportCriteriaModel.repo_deal_type == null   
                                ? "Bilateral Repo & Private Repo"
                                : reportCriteriaModel.repo_deal_type_name);  
            Report_Header += $" (Report No.{reportid})";
            Report_File_Name = reportname_list[0].Text;  
            Report_Name = "Maturity Report";   

            Report_DateFromTo = 
                (reportCriteriaModel.asofDate != null  
                   ? "As of Date "
                   : "") + 
                (reportCriteriaModel.asofDate?.ToString("dd/MM/yyyy") ?? "");   
            Report_DateFromTo +=   
                (reportCriteriaModel.trade_date != null
                   ? " Trade Date " 
                   : "") +    
                 (reportCriteriaModel.trade_date?.ToString("dd/MM/yyyy") ?? "");   
            Report_DateFromTo +=   
                 (reportCriteriaModel.settlement_date != null   
                   ? " Settlement Date 
                   : "") +   
                 (reportCriteriaModel.settlement_date?.ToString("dd/MM/yyyy") ?? "");   
            Report_DateFromTo += 
                 (reportCriteriaModel.maturity_date != null  
                   ? " Maturity Date "
                   : "") + 
                 (reportCriteriaModel.maturity_date?.ToString("dd/MM/yyyy") ?? "");      

            if (collection["PDF"] != null)   
            {
                  dt = GetReportData(reportCriteriaModel);   
                  var rd = new ReportDocument();   
                  rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),
                       "MaturityReport.rpt"));     
                  rd.SetDataSource(dt);  
                  rd.SetParameterValue("asofdate", DateTime.Now);  
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
                  dt = GetReportData(reportCriteriaModel);   

                  var workbook = new HSSFWorkbook();  
                  var sheet = workbook.CreateSheet("Maturity");   

                  var excelTemplate = new ExcelTemplate(workbook);  

                  // Add Header  
                  var rowIndex = 0;
                  var excelRow = sheet.CreateRow(rowIndex);  
                  excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_Bank);   
                  rowIndex++;  

                  excelRow = sheet.CreateRow(rowIndex);
                  excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_Header);   
                  rowIndex++;  
                  if (Report_DateFromTo != "")  
                  {
                     excelRow = sheet.CreateRow(rowIndex);  
                     excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_DateFromTo.Trim());   
                     rowIndex++;   
                  }

                  string ownerEndUer = string.Empty;  
                  List<ConfigModel> listConfig = reportentity.GetReportHeader(reportid);    
                  if (listConfig != null && listConfig.Count > 0)
                  {
                        ownerEndUer = listConfig[0].item_value;   
                  }  

                  excelRow = sheet.CreateRow(rowIndex);  
                  excelTemplate.CreateCellHeaderLeft(excelRow, 0, ownerEndUser);   
                  rowIndex++;   

                  excelRow = sheet.CreateRow(rowIndex);    
                  excelTemplate.CreateCellHeaderLeft(excelRow, 0, $"System : Repo (Run date and time {DateTime.Now:dd/MM/yyyy HH:mm})");  
                  rowIndex++;  

                  excelRow = sheet.CreateRow(rowIndex);  

                  // Add Header Table
                  excelTemplate.CreateCellColHead(excelRow, 0, "No.");  
                  excelTemplate.CreateCellColHead(excelRow, 1, "Trade Date");  
                  excelTemplate.CreateCellColHead(excelRow, 2, "Settlement Date");
                  excelTemplate.CreateCellColHead(excelRow, 3, "Maturity Date");
                  excelTemplate.CreateCellColHead(excelRow, 4, "Period");
                  excelTemplate.CreateCellColHead(excelRow, 5, "Inst.");
                  excelTemplate.CreateCellColHead(excelRow, 6, "Inst. Type");
                  excelTemplate.CreateCellColHead(excelRow, 7, "Tran. No.");
                  excelTemplate.CreateCellColHead(excelRow, 8, "Contract No.");
                  excelTemplate.CreateCellColHead(excelRow, 9, "Counterparty Name");
                  excelTemplate.CreateCellColHead(excelRow, 10, "Cash Amount");
                  excelTemplate.CreateCellColHead(excelRow, 11, "Cur");
                  excelTemplate.CreateCellColHead(excelRow, 12, "Reference Rate");
                  excelTemplate.CreateCellColHead(excelRow, 13, "Spread");
                  excelTemplate.CreateCellColHead(excelRow, 14, "Repo Interest Rate");
                  excelTemplate.CreateCellColHead(excelRow, 15, "Portfolio");
                  excelTemplate.CreateCellColHead(excelRow, 16, "Collateral Details");
                  excelTemplate.CreateCellColHead(excelRow, 17, "Collateral Details");
                  excelTemplate.CreateCellColHead(excelRow, 18, "Collateral Details");
                  excelTemplate.CreateCellColHead(excelRow, 19, "Collateral Details");
                  excelTemplate.CreateCellColHead(excelRow, 20, "Collateral Details");
                  excelTemplate.CreateCellColHead(excelRow, 21, "Collateral Details");
                  excelTemplate.CreateCellColHead(excelRow, 22, "Collateral Details");
                  excelTemplate.CreateCellColHead(excelRow, 23, "Limit Status");
                  excelTemplate.CreateCellColHead(excelRow, 24, "Tran. Status");

                  rowIndex++;
                  excelRow = sheet.CreateRow(rowIndex);
                  excelTemplate.CreateCellColHead(excelRow, 0, "No.");
                  excelTemplate.CreateCellColHead(excelRow, 1, "Trade Date");
                  excelTemplate.CreateCellColHead(excelRow, 2, "Settlement Date");
                  excelTemplate.CreateCellColHead(excelRow, 3, "Maturity Date");
                  excelTemplate.CreateCellColHead(excelRow, 4, "Period");
                  excelTemplate.CreateCellColHead(excelRow, 5, "Inst.");
                  excelTemplate.CreateCellColHead(excelRow, 6, "Inst. Type");
                  excelTemplate.CreateCellColHead(excelRow, 7, "Tran. No.");
                  excelTemplate.CreateCellColHead(excelRow, 8, "Contract No.");
                  excelTemplate.CreateCellColHead(excelRow, 9, "Counterparty Name");
                  excelTemplate.CreateCellColHead(excelRow, 10, "Cash Amount");
                  excelTemplate.CreateCellColHead(excelRow, 11, "Cur");
                  excelTemplate.CreateCellColHead(excelRow, 12, "Reference Rate");
                  excelTemplate.CreateCellColHead(excelRow, 13, "Spread");
                  excelTemplate.CreateCellColHead(excelRow, 14, "Repo Interest Rate");
                  excelTemplate.CreateCellColHead(excelRow, 15, "Portfolio");
                  excelTemplate.CreateCellColHead(excelRow, 16, "Sec.ID");
                  excelTemplate.CreateCellColHead(excelRow, 17, "ISIN Code");
                  excelTemplate.CreateCellColHead(excelRow, 18, "Cur.");
                  excelTemplate.CreateCellColHead(excelRow, 19, "Purchase Units");
                  excelTemplate.CreateCellColHead(excelRow, 20, "Par/Unit");
                  excelTemplate.CreateCellColHead(excelRow, 21, "Par/Value");
                  excelTemplate.CreateCellColHead(excelRow, 22, "Portfolio");
                  excelTemplate.CreateCellColHead(excelRow, 23, "Limit Status");
                  excelTemplate.CreateCellColHead(excelRow, 24, "Tran. Status");

            // Add Data Rows   
            for (var i = 0; i < dt.Rows.Count; i++)   
            {
                rowIndex++;  
                excelRow = sheet.CreateRow(rowIndex);  

                excelTemplate.CreateCellColCenter(excelRow, 0, (i + 1).ToString());  

                if (dt.Rows[i]["trade_date"].ToString() != string.Empty)  
                {
                    excelTemplate.CreateCellColCenter(excelRow, 1, DateTime.Parse(dt.Rows[i]["trade_date"].ToString()));  
                }
                else
                {
                    excelTemplate.CreateCellColCenter(excelRow, 1, new DateTime());   
                }  

                 if (dt.Rows[i]["trade_date"].ToString() != string.Empty)
                    {
                        excelTemplate.CreateCellColCenter(excelRow, 1, DateTime.Parse(dt.Rows[i]["trade_date"].ToString()));
                    }
                    else
                    {
                        excelTemplate.CreateCellColCenter(excelRow, 1, new DateTime());
                    }

                    if (dt.Rows[i]["settlement_date"].ToString() != string.Empty)
                    {
                        excelTemplate.CreateCellColCenter(excelRow, 2, DateTime.Parse(dt.Rows[i]["settlement_date"].ToString()));
                    }
                    else
                    {
                        excelTemplate.CreateCellColCenter(excelRow, 2, new DateTime());
                    }

                    if (dt.Rows[i]["maturity_date"].ToString() != string.Empty)
                    {
                        excelTemplate.CreateCellColCenter(excelRow, 3, DateTime.Parse(dt.Rows[i]["maturity_date"].ToString()));
                    }
                    else
                    {
                        excelTemplate.CreateCellColCenter(excelRow, 3, new DateTime());
                    }

                    excelTemplate.CreateCellColCenter(excelRow, 4, dt.Rows[i]["period"].ToString());
                    excelTemplate.CreateCellColCenter(excelRow, 5, dt.Rows[i]["instrument_code"].ToString());
                    excelTemplate.CreateCellColCenter(excelRow, 6, dt.Rows[i]["instrument_type"].ToString());
                    excelTemplate.CreateCellColCenter(excelRow, 7, dt.Rows[i]["trans_no"].ToString());

                    excelTemplate.CreateCellColLeft(excelRow, 8, dt.Rows[i]["contract_no"].ToString());
                    excelTemplate.CreateCellColLeft(excelRow, 9, dt.Rows[i]["counterparty_name"].ToString());

                    excelTemplate.CreateCellCol2Decimal(excelRow, 10, dt.Rows[i]["cash_amount"].ToString());

                    excelTemplate.CreateCellColCenter(excelRow, 11, dt.Rows[i]["cur"].ToString());
                    excelTemplate.CreateCellColCenter(excelRow, 12, dt.Rows[i]["reference_rate"].ToString());


                    double spread = 0;
                    if (dt.Rows[i]["spread"].ToString() != string.Empty)
                        spread = double.Parse(dt.Rows[i]["spread"].ToString());

                    if (dt.Rows[i]["reference_rate"].ToString() == "FIXED")
                    {
                        excelTemplate.CreateCellColLeft(excelRow, 13, "");
                    }
                    else
                    {
                        excelTemplate.CreateCellCol6Decimal(excelRow, 13, spread);
                    }

                    excelTemplate.CreateCellCol6Decimal(excelRow, 14, dt.Rows[i]["repo_interest_rate"].ToString());

                    excelTemplate.CreateCellColCenter(excelRow, 15, dt.Rows[i]["portfolio"].ToString());

                    excelTemplate.CreateCellColLeft(excelRow, 16, dt.Rows[i]["security_id_col"].ToString());
                    excelTemplate.CreateCellColLeft(excelRow, 17, dt.Rows[i]["isin_code_col"].ToString());

                    excelTemplate.CreateCellColCenter(excelRow, 18, dt.Rows[i]["cur_col"].ToString());

                    excelTemplate.CreateCellColNumber(excelRow, 19, dt.Rows[i]["purchase_unit_col"].ToString());

                    excelTemplate.CreateCellCol2Decimal(excelRow, 20, dt.Rows[i]["par_unit_col"].ToString());
                    excelTemplate.CreateCellCol2Decimal(excelRow, 21, dt.Rows[i]["par_value_col"].ToString());

                    excelTemplate.CreateCellColCenter(excelRow, 22, dt.Rows[i]["port_col"].ToString());
                    excelTemplate.CreateCellColCenter(excelRow, 23, dt.Rows[i]["limit_status"].ToString());
                    excelTemplate.CreateCellColCenter(excelRow, 24, dt.Rows[i]["tran_status"].ToString());
                }

                for (var i = 1; i <= 24; i++)   
                {

                    sheet.AutoSizeColumn(i);  

                    var colWidth = sheet.GetColumnWidth(i);  
                    if (colWidth < 2000)   
                        sheet.SetColumnWidth(i, 2000);  
                    else
                        sheet.SetColumnWidth(i, colWidth + 200);   
                }

                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));  
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 10));  
                sheet.AddMergedRegion(new CellRangeAddress(2, 2, 0, 10));  
                sheet.AddMergedRegion(new CellRangeAddress(3, 3, 0, 10));   

                int x = 0;  
                int y = 0;  

                if (Report_DateFromTo != "")   
                {
                    sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 10));   
                    x = 5;  
                    y = 6;    
                }  
                else
                {
                    x = 4;  
                    y = 5;  
                }  

                sheet.AddMergedRegion(new CellRangeAddress(x, y, 0, 0));    
                sheet.AddMergedRegion(new CellRangeAddress(x, y, 2, 2));  
                sheet.AddMergedRegion(new CellRangeAddress(x, y, 3, 3));  
                sheet.AddMergedRegion(new CellRangeAddress(x, y, 4, 4));
                sheet.AddMergedRegion(new CellRangeAddress(x, y, 5, 5));

                sheet.AddMergedRegion(new CellRangeAddress(x, y, 6, 6));
                sheet.AddMergedRegion(new CellRangeAddress(x, y, 7, 7));
                sheet.AddMergedRegion(new CellRangeAddress(x, y, 8, 8));
                sheet.AddMergedRegion(new CellRangeAddress(x, y, 9, 9));
                sheet.AddMergedRegion(new CellRangeAddress(x, y, 10, 10));
                sheet.AddMergedRegion(new CellRangeAddress(x, y, 11, 11));
                sheet.AddMergedRegion(new CellRangeAddress(x, y, 12, 12));
                sheet.AddMergedRegion(new CellRangeAddress(x, y, 13, 13));
                sheet.AddMergedRegion(new CellRangeAddress(x, y, 14, 14));
                sheet.AddMergedRegion(new CellRangeAddress(x, y, 15, 15));

                sheet.AddMergedRegion(new CellRangeAddress(x, x, 16, 22));

                sheet.AddMergedRegion(new CellRangeAddress(y, y, 16, 16));
                sheet.AddMergedRegion(new CellRangeAddress(y, y, 17, 17));
                sheet.AddMergedRegion(new CellRangeAddress(y, y, 18, 18));
                sheet.AddMergedRegion(new CellRangeAddress(y, y, 19, 19));
                sheet.AddMergedRegion(new CellRangeAddress(y, y, 20, 20));
                sheet.AddMergedRegion(new CellRangeAddress(y, y, 21, 21));
                sheet.AddMergedRegion(new CellRangeAddress(y, y, 22, 22));

                sheet.AddMergedRegion(new CellRangeAddress(x, y, 23, 23));
                sheet.AddMergedRegion(new CellRangeAddress(x, y, 24, 24));

                var exportfile = new MemoryStream();   
                workbook.Write(exportfile);   
                Response.Clear();  
                Response.ClearContent();  
                Response.ClearHeaders();   

                Response.AppendHeader("Content-Type", "application/vnd.ms-excel");  
                Response.AppendHeader("Cache-Control", "must-revalidate, post-check=0, pre-check=0");   
                Response.AppendHeader("Cache=Control", "max-age=30");   
                Response.AppendHeader("Pragma", "public");   
                Response.AppendHeader("Content-disposition", "attachment; filename=" + Report_Name + ".xls");   

                Response.BinaryWrite(exportfile.GetBuffer());  
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();  

                return View();  
            }

            return View();  

        }

        public DataTable GetReportData(ReportCriteriaModel data)   
        {
            var dt = new DataTable();   
            apiReport.ReportData.MaturityReport(data, p =>   
            {
                if (p.Success) dt = p.Data.MaturityReportResultModel.ToDataTable();  
            });  

            return dt;  
        }  

        public ActionResult FillInstrumentType(string datastr)  
        {
            var res = new List<DDLItemModel>();  
            api_deal.RPDealEntry.GetDDLTnstrumnetType(datastr, p =>  
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

        public ActionResult FillCurrency(string datastr)   
        {
            var res = new List<DDLItemModel>();     
            api_counterparty.CounterPartyFund.GetDDLCounterParty(datastr, p => 
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

    }
}