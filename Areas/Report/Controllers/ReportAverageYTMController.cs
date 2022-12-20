using System.Collections.Generic;
using System.Data;
using System.IO;
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
using GM.Filters;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;

namespace GM.Application.Web.Areas.Report.Controllers
{
     [Authorize]  
     [Audit]
     public class ReportAverageYTMController : BaseController  
     {
        private readonly RPTransEntity api_deal = new RPTransEntity();  

        private SecurityEntities api_security = new SecurityEntities();  
        private readonly StaticEntities api_static = new StaticEntities();
        private readonly ReportEntities apiReport = new ReportEntities(); 
        private readonly string Report_Bank = "SiGG Financial Bank";  
        private string Report_Code = string.Empty;  
        private string Report_DateFromTo = string.Empty;  
        private string Report_File_Name = string.Empty;  
        private string Report_Header = string.Empty;  
        private string Report_Name = string.Empty;  
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
            var dt = new DataTable();   
            var reportname_list = new List<DDLItemModel>();  
            string reportid;  
            var controller_name = 
                  ControllerContext.RouteData.Values["controller"].ToString();  //Request.QueryString["controllername"];
            reportname_list = reportentity.Getreportname(controller_name);   
            if (reportname_list.Count == 0)   
            {
                  ViewBag.ErrorMessage = 
                        "Can not Get Data report ID from Service StATIC Method Config/getReportID and key = " +    
                        controller_name + " in table gm_report !!!!";   
                  return View();   
            }   

            reportid = reportname_list[0].Value.ToString();   
            Report_File_Name = reportname_list[0].Text;   
            Report_Name = "AverageYTMReport.xls";  
            Report_Header = "Average YTM Report";   

            model.asofdate_from = string.IsNullOrEmpty(model.asofdate_from_string)   
                  ? model.asofdate_from   
                  : utility.ConvertStringToDatetimeFormatDDMMYYYY(model.asofdate_from_string);   
            model.asofdate_to = string.IsNullOrEmpty(model.asofdate_to_string)   
                  ? model.asofdate-to  
                  : utility.ConvertStringToDatetimeFormatDDMMYYYY(model.asofdate_to_string);   

            Report_DateFromTo = (model.asofdate_from != null || model.asofdate_to != null ? "As of Date " : "") +  
                                (model.asofdate_from == null ? "" : model.asofdate_from.Value.ToString("dd/MM/yyyy")) +   
                                (model.asofdate_from != null && model.asofdate_to != null ? " - " : "") + 
                                (model.asofdate-to == null ? "" : model.asofdate_to.Value.ToString("dd/MM/yyyy"));  

            if (collection["PDF"] != null)  
            {
                  dt = GetAverageYTM(model);  

                  var rd = new ReportDocument();  
                  rd.Load(Path.Combine(Server.MapPath("~/Areas/Report/Reports/CrystalReportsFile"),   
                      "AverageYTMReport.rpt")); 
                  rd.SetDataSource(dt);  
                  rd.SetParameterValue("asofdate", Report_DateFromTo);  
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
                  dt = GetAverageYTM(model);   

                  var workbook = new HSSFWorkbook();   
                  var sheet = workbook.CreateSheet("Outstanding");   

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
                  excelTemplate.CreateCellHeaderLeft(excelRow, 0, Report_DateFromTo);   
                  rowIndex++;  
  
                  excelRow = sheet.CreateRow(rowIndex);
                  excelTemplate.CreateCellHeaderLeft(excelRow, 0, "System : Repo");
                  rowIndex++;

                  excelRow = sheet.CreateRow(rowIndex);
                  excelTemplate.CreateCellHeaderLeft(excelRow, 0, "Report No." + reportid);
                  rowIndex++;

                  excelRow = sheet.CreateRow(rowIndex);

                  // Add Header Table
                  excelTemplate.CreateCellColHead(excelRow, 0, "Run Date");
                  excelTemplate.CreateCellColHead(excelRow, 1, "Lending");
                  excelTemplate.CreateCellColHead(excelRow, 2, "WTD. Avg Rate");
                  excelTemplate.CreateCellColHead(excelRow, 3, "Borrowing");
                  excelTemplate.CreateCellColHead(excelRow, 4, "WTD. Avg Rate");

                  // Add Data Rows
                  for (var i = 0; i < dt.Rows.Count; i++)
                  {
                        rowIndex++;
                        excelRow = sheet.CreateRow(rowIndex);
                        excelTemplate.CreateCellColCenter(excelRow, 0, dt.Rows[i]["run_date"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 1, dt.Rows[i]["lending"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 2, dt.Rows[i]["wtd_avg_rate_lending"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 3, dt.Rows[i]["borrowing"].ToString());
                        excelTemplate.CreateCellColCenter(excelRow, 4, dt.Rows[i]["wtd_avg_rate_borrowing"].ToString());
                  }

                  for (var i = 1; i <= 4; i++)
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
                  sheet.AddMergedRegion(new CellRangeAddress(4, 4, 0, 10));
                  sheet.AddMergedRegion(new CellRangeAddress(5, 5, 0, 0));
                  sheet.AddMergedRegion(new CellRangeAddress(5, 5, 1, 1));
                  sheet.AddMergedRegion(new CellRangeAddress(5, 5, 2, 2));
                  sheet.AddMergedRegion(new CellRangeAddress(5, 5, 3, 3));
                  sheet.AddMergedRegion(new CellRangeAddress(5, 5, 4, 4));

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

            public DataTable GetAverageYTM(ReportCriteriaModel data)
            {
                  var dt = new DataTable();

                  //SqlParameter outRefCode = new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                  //SqlParameter outMessage = new SqlParameter("@Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };
                  //SqlParameter outServerity = new SqlParameter("@Serverity", SqlDbType.VarChar, 15) { Direction = ParameterDirection.Output };
                  //SqlParameter outHowManyRecord = new SqlParameter("@HowManyRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                  //dt = (new DbHelper()).GetTableRow("RP_Report_AverageYTM_Proc", new[] { outRefCode, outMessage, outServerity, outHowManyRecord,
                  //        new SqlParameter("@as_of_date_from", data.asofdate_from),
                  //        new SqlParameter("@as_of_date_to",  data.asofdate_to),
                  //        new SqlParameter("@repo_deal_type",  data.instrument_type),
                  //        new SqlParameter("@port",  data.port)
                  //});


                  apiReport.ReportData.AverageYTMReport(data, p =>
                  {
                  if (p.Success) dt = p.Data.AverageYTMReportResultModel.ToDataTable();
                  });

                  return dt;
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

            public ActionResult FillInstrumentType(string datastr)
            {
                  var res = new List<DDLItemModel>();
                  api_deal.RPDealEntry.GetDDLInstrumentType(datastr, p =>
                  {
                  if (p.Success) res = p.Data.DDLItems;
                  });

                  return Json(res, JsonRequestBehavior.AllowGet);
            }
      }
}