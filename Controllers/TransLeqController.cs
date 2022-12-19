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
{
    [Authorize]
    [Audit]
    public class TransLeqController : BaseController
    {
        Utility utility = new Utility();

        // GET: TransLeq
        public ActionResult Index()
        {
            if (!IsCheckPermission())
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public JsonResult Search(TransLeqViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.SearchDate))
                {
                    return Json(new { Status = "Error", Message = "require date" });
                }
                
                DateTime dateConvert = utility.ConvertStringToDatetimeFormatDDMMYYYY(model.SearchDate);
                ResultWithModel<DataSet> result = GetTransLeqList(dateConvert);

                if (result.Success)
                {
                    if (result.Data != null && result.Data.Tables.Count > 0)
                    {
                        List<Dictionary<string, object>> dataRows = new List<Dictionary<string, object>>();
                        List<Dictionary<string, object>> columns = new List<Dictionary<string, object>>();

                        foreach (DataColumn col in result.Data.Tables[0].Columns)
                        {
                            Dictionary<string, object> dCol = new Dictionary<string, object>();
                            dCol.Add("title", col.ColumnName);
                            dCol.Add("data", col.ColumnName);
                            columns.Add(dCol);
                        }
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

                        return Json(new { Status = "Success", Message = string.Format("Success. Total Data: {0} rows.", dataRows.Count), Data = dataRows, Columns = columns },
                            "application/json",
                            Encoding.UTF8,
                            JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { Status = "Success", Message = "Success. Total Data: 0 rows.", Data = new object[] { }, Columns = new object[] { } },
                        "application/json",
                        Encoding.UTF8,
                        JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Status = "Error", Message = "Error!!! " + result.Message, Data = new object[] { }, Columns = new object[] { } },
                        "application/json",
                        Encoding.UTF8,
                        JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error", Message = "Error!!! " + ex.Message, Data = new object[] { }, Columns = new object[] { } },
                    "application/json",
                    Encoding.UTF8,
                    JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ExportExcel(TransLeqViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.SearchDate))
                {
                    return Content("require date", "text/html");
                }

                DateTime dateConvert = utility.ConvertStringToDatetimeFormatDDMMYYYY(model.SearchDate);
                
                ResultWithModel<DataSet> result = GetTransLeqList(dateConvert);

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

                        foreach (DataRow row in result.Data.Tables[0].Rows)
                        {
                            rowIndex++;
                            excelRow = sheet.CreateRow(rowIndex);
                            colIndex = 0;
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
                                    excelTemplate.CreateCellColLeft(excelRow, colIndex, row[col].ToString());
                                //}
                                colIndex++;
                            }
                        }

                        for (int i = 0; i < colIndex; i++)
                        {
                            sheet.AutoSizeColumn(i);

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
                        Response.Headers.Add("Cache-Control", "max-age=30");
                        Response.Headers.Add("Pragma", "public");
                        Response.Headers.Add("Content-disposition", "attachment; filename=TransLEQ_" + dateConvert.ToString("yyyyMMdd") + ".xls");

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
                return Content("error: " + ex.Message, "text/html");
            }
        }

        private ResultWithModel<DataSet> GetTransLeqList(DateTime date)
        {
            StaticEntities api_static = new StaticEntities();
            ResultWithModel<DataSet> result = new ResultWithModel<DataSet>();
            TransLeqModel model = new TransLeqModel()
            {
                asof_date = date
            };
            api_static.TransLeq.GetTransLeqList(model, p => {
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
}