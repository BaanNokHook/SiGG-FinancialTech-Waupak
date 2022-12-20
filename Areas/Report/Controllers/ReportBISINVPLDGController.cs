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
   public class ReportBISINVPLDGController : BaseController 
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
                  Report_Name = "BISINVPLDGReport.xls";   
                  if (reportname_list.Count == 0)  
                  {
                        ViewBag.ErrorMessage = 
                            "Can not Get Data Report ID from Service Static Method Config/getReportID amd key = " + 
                            controller_name + " in table gm_report !!!!";  
                        return View();   
                  }  

                  reportid = reportname_list[0].Value.ToString();   
                  Report_Header = "BISINVPLDG Report";   

                  if (string.IsNullOrEmpty(reportCriteriaModel.asofdate_string))   
                  {
                        ViewBag.ErrorMessage = "Please select As of Date From";   
                        return View();  
                  }  

                  reportCriteriaModel.asofdate = string.IsNullOrEmpty(reportCriteriaModel.asofdate_string)   
                     ? reportCriteriaModel.asofdate
                     : utility.ConvertStringToDatetimeFormatDDMMYYYY(reportCriteriaModel.asofdate_string);   
                  
                  Report_DateFromTo = (reportCriteriaModel.asofdate != null ? "As of Date " : "") +  
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
                    var sheet = workbook.CreateSheet("BISINVPLDG");  

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

                    // Add Header Table

                    excelRow = sheet.CreateRow(rowIndex);
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 0, "Code ของผู้ให้กู้ยืม");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 1, "Deal No.");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 2, "Customer Code");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 3, "ชื่อผู้ให้กู้ยืม");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 4, "Exposure Group ของผู้ให้กู้ยืม");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 5, "ประเทศผู้ให้กู้");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 6, "สกุลเงินของประเทศผู้ให้กู้");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 7, "Rating ของประเทศของผู้ให้กู้ยืม");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 8, "Trade Date");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 9, "Maturity Date");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 10, "สกุลเงินของยอดเงินกู้");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 11, "ยอดเงินกู้ (ยอดทำ Repo)");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 12, "ยอด Cash Margin");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 13, "ยอดดอกเบี้ยค้างจ่ายของเงินกู้");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 14, "ยอดดอกเบี้ยค้างจ่ายของ Cash Margin");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 15, "Banking Book หรือ Trading Book");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 16, "ชื่อย่อของตราสารที่นำไปวางเป็นประกัน");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 17, "ชื่อเต็มของตราสารที่นำไปวางเป็นประกัน");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 18, "สกุลเงินของตราสาร");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 19, "ยอด Outstandings ของตราสารที่นำไปวางเป็นประกัน");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 20, "Exposure Group ของตราสารที่นำไปวางเป็นประกัน");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 21, "Core Market Participant");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 22, "BOT’s Criteria");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 23, "DATA DATE");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 24, "ยอด Outstandings ของตราสาร Face Value");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 25, "ประเทศผู้ออกตราสาร");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 26, "Collteral Type");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 27, "Repo รายการภาระนอกงบดุล");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 28, "ISIC Code");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 29, "MIN FEE RATE");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 30, "MIN FEE UNIT");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 31, "MAX FEE RATE");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 32, "MAX FEE UNIT");
                    excelTemplate.CreateCellColHeadAndWarpText(excelRow, 33, "CONTINGENT ACCOUNT");

                    // Add Data Rows
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);
                        excelTemplate.CreateCellColCenter(excelRow, 0, dt.Rows[i]["CNTR_SWIFT_CODE"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 1, dt.Rows[i]["TRANS_NO"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 2, dt.Rows[i]["CNTR_CODE"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 3, dt.Rows[i]["CNTR_NAME"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 4, dt.Rows[i]["CNTR_EXP_GRP"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 5, dt.Rows[i]["CNTR_COUNTRY"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 6, dt.Rows[i]["CNTR_COUNTRY_CUR"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 7, dt.Rows[i]["CNTR_COUNTRY_RATING"].ToString());

                        if (!string.IsNullOrEmpty(dt.Rows[i]["TRADE_DATE"].ToString()) && !string.IsNullOrWhiteSpace(dt.Rows[i]["TRADE_DATE"].ToString()))
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 8, DateTime.Parse(dt.Rows[i]["TRADE_DATE"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 8, new DateTime?());
                        }

                        if (!string.IsNullOrEmpty(dt.Rows[i]["MATURITY_DATE"].ToString()) && !string.IsNullOrWhiteSpace(dt.Rows[i]["MATURITY_DATE"].ToString()))
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 9, DateTime.Parse(dt.Rows[i]["MATURITY_DATE"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 9, new DateTime());
                        }

                        excelTemplate.CreateCellColCenter(excelRow, 10, dt.Rows[i]["CUR"].ToString());

                        double PurchaseAmt = 0;
                        if (!string.IsNullOrEmpty(dt.Rows[i]["BOND_CASH_AMT"].ToString()) && !string.IsNullOrWhiteSpace(dt.Rows[i]["BOND_CASH_AMT"].ToString()))
                            PurchaseAmt = double.Parse(dt.Rows[i]["BOND_CASH_AMT"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 11, PurchaseAmt);

                        double CashMargin = 0;
                        if (!string.IsNullOrEmpty(dt.Rows[i]["CASH_MARGIN"].ToString()) && !string.IsNullOrWhiteSpace(dt.Rows[i]["CASH_MARGIN"].ToString()))
                            CashMargin = double.Parse(dt.Rows[i]["CASH_MARGIN"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 12, CashMargin);

                        double InterestAmt = 0;
                        if (!string.IsNullOrEmpty(dt.Rows[i]["INT_AMT"].ToString()) && !string.IsNullOrWhiteSpace(dt.Rows[i]["INT_AMT"].ToString()))
                            InterestAmt = double.Parse(dt.Rows[i]["INT_AMT"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 13, InterestAmt);

                        double InterestAmtMargin = 0;
                        if (!string.IsNullOrEmpty(dt.Rows[i]["INT_CASH_MARGIN"].ToString()) && !string.IsNullOrWhiteSpace(dt.Rows[i]["INT_CASH_MARGIN"].ToString()))
                            InterestAmtMargin = double.Parse(dt.Rows[i]["INT_CASH_MARGIN"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 14, InterestAmtMargin);

                        excelTemplate.CreateCellColCenter(excelRow, 15, dt.Rows[i]["PORT_CODE"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 16, dt.Rows[i]["BOND_CODE"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 17, dt.Rows[i]["BOND_NAME"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 18, dt.Rows[i]["BOND_CUR"].ToString());

                        double InstrumentPurchase = 0;
                        if (!string.IsNullOrEmpty(dt.Rows[i]["BOND_OUTS_AMT"].ToString()) && !string.IsNullOrWhiteSpace(dt.Rows[i]["BOND_OUTS_AMT"].ToString()))
                            InstrumentPurchase = double.Parse(dt.Rows[i]["BOND_OUTS_AMT"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 19, InstrumentPurchase);

                        double InstrumentExposure = 0;
                        if (!string.IsNullOrEmpty(dt.Rows[i]["BOND_EXP_GRP"].ToString()) && !string.IsNullOrWhiteSpace(dt.Rows[i]["BOND_EXP_GRP"].ToString()))
                            InstrumentExposure = double.Parse(dt.Rows[i]["BOND_EXP_GRP"].ToString());
                        excelTemplate.CreateCellColNumber(excelRow, 20, InstrumentExposure);

                        excelTemplate.CreateCellColCenter(excelRow, 21, dt.Rows[i]["CORE_MARKET"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 22, dt.Rows[i]["BOT_CRITERIA"].ToString());

                        if (!string.IsNullOrEmpty(dt.Rows[i]["ASOF_DATE"].ToString()) && !string.IsNullOrWhiteSpace(dt.Rows[i]["ASOF_DATE"].ToString()))
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 23, DateTime.Parse(dt.Rows[i]["ASOF_DATE"].ToString()));
                        }
                        else
                        {
                            excelTemplate.CreateCellColCenter(excelRow, 23, new DateTime());
                        }

                        double FaceValue = 0;
                        if (!string.IsNullOrEmpty(dt.Rows[i]["BOND_FACE_VALUE"].ToString()) && !string.IsNullOrWhiteSpace(dt.Rows[i]["BOND_FACE_VALUE"].ToString()))
                            FaceValue = double.Parse(dt.Rows[i]["BOND_FACE_VALUE"].ToString());
                        excelTemplate.CreateCellCol2RedDecimal(excelRow, 24, FaceValue);

                        excelTemplate.CreateCellColLeft(excelRow, 25, dt.Rows[i]["ISSUER_COUNTRY"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 26, dt.Rows[i]["COLL_TYPE"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 27, dt.Rows[i]["BOND_OWNER_FLAG"].ToString());
                        excelTemplate.CreateCellColLeft(excelRow, 28, dt.Rows[i]["BOT_ISIC_CODE"].ToString());

                        double MinFeeRate = 0;
                        if (!string.IsNullOrEmpty(dt.Rows[i]["MIN_FEE_RATE"].ToString()) && !string.IsNullOrWhiteSpace(dt.Rows[i]["MIN_FEE_RATE"].ToString()))
                            MinFeeRate = double.Parse(dt.Rows[i]["MIN_FEE_RATE"].ToString());
                        excelTemplate.CreateCellCol4RedDecimal(excelRow, 29, MinFeeRate);

                        string minFeeUnit = string.Empty;
                        if (dt.Rows[i]["MIN_FEE_UNIT"] != null)
                        {
                            var x = dt.Rows[i]["MIN_FEE_UNIT"].ToString().Split('.');
                            if (x.Length > 1)
                            {
                                minFeeUnit = x[0];
                            }
                        }
                        excelTemplate.CreateCellColRight(excelRow, 30, minFeeUnit);

                        double MaxFeeRate = 0;
                        if (!string.IsNullOrEmpty(dt.Rows[i]["MAX_FEE_RATE"].ToString()) && !string.IsNullOrWhiteSpace(dt.Rows[i]["MAX_FEE_RATE"].ToString()))
                            MaxFeeRate = double.Parse(dt.Rows[i]["MAX_FEE_RATE"].ToString());
                        excelTemplate.CreateCellCol4RedDecimal(excelRow, 31, MaxFeeRate);

                        string maxFeeUnit = string.Empty;
                        if (dt.Rows[i]["MAX_FEE_UNIT"] != null)
                        {
                            var x = dt.Rows[i]["MAX_FEE_UNIT"].ToString().Split('.');
                            if (x.Length > 1)
                            {
                                maxFeeUnit = x[0];
                            }
                        }
                        excelTemplate.CreateCellColRight(excelRow, 32, maxFeeUnit);
                        excelTemplate.CreateCellColRight(excelRow, 33, dt.Rows[i]["CONTINGENT_ACCOUNT"].ToString());
                    }

                    for (var i = 0; i <= 33; i++)
                    {
                        sheet.AutoSizeColumn(i);

                        if (i == 0)
                        {
                            sheet.SetColumnWidth(i, 5000);
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

                    var headerRow = sheet.GetRow(5);
                    headerRow.HeightInPoints = 40;

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
            apiReport.ReportData.BISINVPLDGReport(data, p =>
            {
                if (p.Success) dt = p.Data.BISINVPLDGReportResultModel.ToDataTable();
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