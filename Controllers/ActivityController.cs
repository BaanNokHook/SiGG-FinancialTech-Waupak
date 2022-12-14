using GM.CommonLibs;
using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.UserAndScreen;
using GM.Data.Result.UserAndScreen;
using GM.Filters;
using NPOI.HSSF.UserModel;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
      [Authorize]
      [Audit] 
      public class ActivityController : BaseController  
      {
            private static string Controller = "ActivityController";   
            private static LogFile Log = new LogFile();  
            UserAndScreenEntities api = new UserAndScreenEntities();   

            [RoleScreen(RoleScreen.EDIT)]  
            public ActionResult Index()  
            {
                  return View();  
            }   

            [HttpPost]  
            [RoleScreen(RoleScreen.EDIT)]  
            public ActionResult ExportActivity(ActivityLogModel model)  
            {
                  string strMsg = string.Empty;    
                  string filename = "";   
                  try
                  {
                        // Export Excel XLS   
                        DataTable dt = new DataTable();   
                        ExcelEntity ExcelEnt = new ExcelEntity();   

                        dt = GetReportData(model);  
                        if (dt == null)  
                        {
                              throw new Exception("No Data.");  
                        }

                        ExcelEnt.FileName = "ActivityLog.xls";   
                        ExcelEnt.SheetName = "ActivityLog";  
                        filename = "ActivityLog.xls";  

                        //save the file to server temp folder  
                        string importexportpath = ConfigurationManger.AppSettings["ImportExportPath"].ToString();   
                        string fullPath = Path.Combine(Server.MapPath(importexportpath), ExcelEnt.FileName);   

                        HSSFWorkbook workbook = new HSSFWorkbook();  
                        var sheet = workbook.CreateSheet(ExcelEnt.SheetName);  

                        var ExcelTemplate = new ExcelTemplate(workbook);   

                        // Add Header  
                        var rowIndex = 0;  
                        var excelRow = sheet.CreateRow(rowIndex);  

                        int colIndex = 0;  
                        foreach (var item in dt.Columns.Cast<DataColumn>())  
                        {
                            excelTemplate.CreateCellColHead(excelRow, colIndex, item.ColumnName.ToUpper());  
                            colIndex++;
                        }  

                        int row = 0;  
                        foreach (var rows in dt.Rows)   
                        {
                            rowIndex++;  
                            excelRow = sheet.CreateRow(rowIndex);   
                            
                            for (int i = 0; i < colIndex; i++)  
                            {
                                excelTemplate.CreateCellColLeft(excelRow, i, dt.Rows[row][i].ToString());   
                            }  
                            row++;   
                        }  

                        for (var i = 0; i < colIndex; i++)  
                        {
                            sheet.AutoSizeColumn(i);  

                            var colWidth = sheet.GetColumnWidth(i);  
                            if (colWidth < 2000)  
                                sheet.SetColumnWidth(i, 2000);  
                            else
                                sheet.SetColumnWidth(i, colWidth + 200);   
                        }

                    if (System.IO.File.Exists(fullPath))  
                        System.IO.File.Delete(fullPath);   

                    FileStream FileData - new FileStream(fullPath, FileMode.Create);
                    workbook.Write(FileData);
                    FileData.Close();  
                } 
                catch (Exception ex)   
                {
                    strMsg = ex.Message;  
                    Log.WriteLog(Controller, "Error : " + ex.Message);   
                }  
                return Json(new { fileName = filename, errorMessage = strMsg }, JsonRequestBehavior.AllowGet);  
            }  

            public DataTable GetReportData(ActivityLogModel model)    
            {
                ResultWithModel<UserResult> result = new ResultWithModel<UserResult>();   

                //Add Paging   
                PagingModel paging = new PagingModel();   

                model.paging = paging;  
                model.user_id = HttpContext.User.Identity.Name;   

                var dt = new DataTable();  
                api.User.GetUserActivityLog(model, p => 
                {
                    if (p.Success)   
                    {
                        if (p.Data.Tables.Count > 0 && p.Data.Tables[0].Rows.Count > 0)   
                        {
                            dt = p.Data.Tables[0];   
                        }  
                        else
                        {
                            dt = null;   
                        }  
                    }
                });  

                return dt;  
            }  

            [HttpGet]
            [RoleScreen(RoleScreen.VIEW)]   
            public Actionresult Download(string filename)
            {
                //get the temp folder and file path in server
                string importexportpath = ConfigurationManager.AppSettings["ImportExportPath"].ToString();  
                string fullPath = Path.Combine(Server.MapPath(importexportpath), filename);     

                //return the filep for download, this is an Excel
                //so I set the file content type to "application/vnd.ms-excel"   
                return File(fullPath, "application/vnd.ms-excel", filename);    
            }  
      }   
}   