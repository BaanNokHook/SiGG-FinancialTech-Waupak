using GM.CommonLibs;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.Static;
using GM.Data.View.Static;
using GM.Filters;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers

   [Authorize]  
   [Audit]  
   public class DMSController : BaseController  
   
      Utility utility = new Utility();   

      public ActionResult Index()   
      {
            if (!IsCheckPermission())   
            {
                  return RedirectToAction("Index", "Home");
            }

            return View();  
      }

      [HttpPost]  
      public JsonResult Search(DMSViewModel model)  
      {
            try 
            {
                  if (string.IsNullOrEmpty(model.SearchDate))  
                  {
                        return Json(new { Status = "Error", Message = "require date" });   
                  }  
                  if (string.IsNullOrEmpty(model.DmsType))  
                  {
                        return Json(new { Status = "Error", Message = "requires DMS Type" });  
                  }

                  DateTime dateConvert = Utility.ConvertStringToDatetimeFormatDDMMYYYY(model.SearchDate);  
                  ResultWithModel<DataSet> result = GetDMSList(dateConvert, model.DmsType);   

                  if (result.Success)  
                  {
                        if (result.Data != null && result.Data.Tables.Count > 0)  
                        {
                              List<Dictionary<string, object>> dataRows = new List<Dictionary<string, object>>();  
                              List<Dictionary<string, object>> columns = new List<Dictionary<string, object>>();  

                              foreach (DataRow row in result.Data.Tables[0].Rows)   
                              {
                                    Dictionary<string, object> dRow = new Dictionary<string, object>();   
                                    foreach (DataColumn col in result.Data.Tables[0].Columns)
                                    {
                                          //DateTime dateConvert;
                                          //if (DateTime.TryParse(row[col].ToString().Trim(),
                                          //    System.Globalization.CultureInfo.InvariantCulture,
                                          //    System.Globalization.DateTimeStyles.None, out dateConvert))
                                          //{
                                          //    dRow.Add(col.ColumnName, dateConvert.ToString("yyyy-MM-dd HH:mm:ss"));
                                          //}
                                          //else
                                          //{
                                          dRow.Add(col.ColumnName, row[col]);
                                          //}  
                                    }
                                    dataRows.Add(dRow);  
                              }
                              return Json(new { Status = "Success", Message = string.Format("Success. Total Data: {0} rows. ", dataRows.Count), Data = dataRows, Columns = columns },  
                                    "application/json",  
                                    Encoding.UTF8,  
                                    JsonRequestBehavior.AllowGet);   
                        }

                        return Json(new { Status = "Success", Message = "Success. Total Data: {0} rows.", Data = new object[] { }, Columns = new object[] { } }, 
                                    "application/json",  
                                    Encoding.UTF8, 
                                    JsonRequestBehavior.AllowGet);  
                  }
                  else
                  {
                        return Json(new { Status = "Error", Message = "Error!!! " + result.Message, Data = new Object[] { }, Columns = new object[] { } },  
                                    "application/json" 
                                    Encoding.UTF8,  
                                    JsonRequestBehavior.AllowGet);  
                  }
            }

            [HttpPost]  
            public ActionResult ExportExcel(DMSViewModel model)   
            {
                  try
                  {
                        if (string.IsNullOrEmpty(model.SearchDate))  
                        {
                              return Content("require date", "text/html");   
                        }

                        DateTime dateConvert = utility.ConvertStringToDatetimeFormatDDMMYYYY(model.SearchDate);   

                        ResultWithModel<DataSet> result = GetDMSList(dateConvert, model.DmsType);    

                        if (result.Success)   
                        {
                              if (result.Data != null)   
                              {
                                    HSSFWorkbook workbook = new HSSFWorkbook();   
                                    ISheet sheet = workbook.CreateSheet("Result");   
                                    ExcelTemplate excelTemplate = new ExcelTemplate(workbook);  
                                    // Add Header
                                    int rowIndex = 0;  
                                    int colIndex = 0;

                                    IRow excelRow = sheet.CreateRow(rowIndex);   

                                    foreach (DataColumn col in result.Data.Tables[0].Columns)  
                                    {
                                          excelTemplate.CreateCellColHead(excelRow, colIndex, col.ColumnName);   
                                          colIndex++;   
                                    }

                                    foreach (DataRow row in result.Data.Table[0].Rows)
                                    {
                                          rowIndex++;  
                                          excelRow = sheet.CreateRow(rowIndex);   
                                          colindex = 0;   
                                          foreach (DataColumn col in result.Data.Tables[0].Columns)   
                                          {
                                                //DateTime date;
                                                //if (DateTime.TryParse(row[col].ToString().Trim(),
                                                //    System.Globalization.CultureInfo.InvariantCulture,
                                                //    System.Globalization.DateTimeStyles.None, out date))
                                                //{
                                                //    excelTemplate.CreateCellColLeft(excelRow, colIndex, date.ToString("yyyy-MM-dd HH:mm:ss"));
                                                //}
                                                //else
                                                //{
                                          }  
                                          excelTemplate.CreateCellColLeft(excelRow, colindex, row[col].ToString());    
                                          //}  
                                          colIndex++;    
                                    }  
                              }  

                              for (int i = 0; i < colIndex; i++)   
                              {
                                    sheet.AuthorizeColumn(i);   

                                    int colWidth = sheet.GetColumnWidth(i);  
                                    if (colWidth < 5000)  
                                    {
                                          sheet.SetColumnWidth(i, 5000);  
                                    }
                                    else
                                    {
                                          sheet.SetColumnWidth(i, colWidth + 200);   
                                    }  
                              }  

                              MemoryStream exportfile = new MemoryStream();   
                              workbook.Write(exportfile);  
                              Response.Clear();
                              Response.ClearContent();
                              Response.ClearHeaders();   
                              Response.Headers.Add("Content-Type", "application/vnd.ms-excel");    
                              Response.Headers.Add("Cache-Control", "must-revalidate, post-check=0, pre-check=0");   
                              Response.Headers.Add("Pragma", "public");   
                              Response.Headers.Add("Content-disposition", "attachment; filename=DMS_" + dateConvert.ToString("yyyyMMdd") + ".xls");    

                              Response.BinaryWrite(exportfile.GetBuffer());  
                              Response.End();  
                              return View("Index");  
                        }

                        return Content("no data", "text/html");   
                  }
                  else  
                  {
                        return Content("error: " + result.Message, "text/html");  
                  }   

            }
            catch (Exception ex)  
            {
                  return Content("error: " + ex.message, "text/html");  
            }

      private ResultWithModel<DataSet> GetDMSList(DateTime date, string dmsType)  
      {
           StaticEntities api_static = new StaticEntities();   
           ResultWithModel<DataSet> result = new ResultWithModel<DataSet>();   
           DMSModel model = new DMSModel()   
           {
                  asof_date = date,  
                  dms_name = dmsType
           };  
           api_static.DMS.GetDMSList(model, p => {
                  result = p;   
           });   

           return result;   
      }

      private bool IsCheckPermission()   
      {
            if (User.RoleName != "Administrator")   
            {
                  return false;   
            } 

            return true;   
      }
}
