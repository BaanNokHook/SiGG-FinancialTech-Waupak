using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.RPTransaction;
using GM.Data.Model.Static;
using GM.Data.Result.RPTransaction;
using GM.Data.Result.Static;
using GM.Data.View.RPTransaction;
using GM.Filters;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class RPReportTBMAController : BaseController
    {
        RPTransEntity api_RPTrans = new RPTransEntity();
        Utility utility = new Utility();

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            var RPDealEntryView = new RPReportTBMAViewModel();
            return View(RPDealEntryView);
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            ResultWithModel<RPRportTBMATableResult> result = new ResultWithModel<RPRportTBMATableResult>();
            RPTransModel RPTransModel = new RPTransModel();
            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                RPTransModel.paging = paging;

                //Add Orderby
                var orders = new List<OrderByModel>();
                if (model.order != null)
                {
                    model.order.ForEach(o =>
                    {
                        var col = model.columns[o.column];
                        orders.Add(new OrderByModel { Name = col.data, SortDirection = (o.dir.Equals("desc") ? SortDirection.Descending : SortDirection.Ascending) });
                    });
                }
                RPTransModel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null).ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "from_trade_date":
                            RPTransModel.from_trade_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "to_trade_date":
                            RPTransModel.to_trade_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                        case "trans_deal_type_name":
                            RPTransModel.trans_deal_type = column.search.value;
                            break;
                        case "port":
                            RPTransModel.port = column.search.value;
                            break;
                        case "counter_party":
                            RPTransModel.counter_party_code = column.search.value;
                            break;
                    }
                });

                api_RPTrans.RPReportTBMA.GetRPReportTBMAList(RPTransModel, p =>
                {
                    result = p;
                });

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Json(new
            {
                success = false,
                draw = model.draw,
                recordsTotal = result.HowManyRecord,
                recordsFiltered = result.HowManyRecord,
                Message = result.Message,
                data = result.Data != null ? result.Data.RPReportTBMATableResultModel : new List<RPReportTBMATable>(),
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Send(string id)
        {
            string StrMsg = string.Empty;
            var Result = new List<object>();
            ResultWithModel<RPReportTBMAResult> ResultModel = new ResultWithModel<RPReportTBMAResult>();
            RPReportTBMAModel RPReportTBMAModel = new RPReportTBMAModel();

            try
            {
                // Step 1 : Select Trans
                List<string> ListTrans = new List<string>();
                ListTrans = JsonConvert.DeserializeObject<List<string>>(id);
                ListTrans.Sort();
                string TransNo = string.Empty;

                for (int i = 0; i < ListTrans.Count; i++)
                {
                    if (i == 0)
                    {
                        TransNo += ListTrans[i].ToString();
                    }
                    else
                    {
                        TransNo += "," + ListTrans[i].ToString();
                    }
                }

                // Step 2 : Search Data
                RPTransModel RPTransModel = new RPTransModel();
                RPTransModel.trans_no = TransNo;

                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = 1;
                paging.RecordPerPage = 100;
                RPTransModel.paging = paging;

                //Add Orderby
                var orders = new List<OrderByModel>();
                RPTransModel.ordersby = orders;
                RPTransModel.create_by = HttpContext.User.Identity.Name;

                api_RPTrans.RPReportTBMA.ExportRPReportTBMA(RPTransModel, p =>
                {
                    ResultModel = p;
                });

                if (ResultModel.Success == false)
                {
                    throw new Exception("ExportRPReportTBMA() => " + ResultModel.Message);
                }
            }
            catch (Exception Ex)
            {
                StrMsg = Ex.Message;
            }

            Result.Add(new { Message = StrMsg });
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        //export excel
        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult GenExcel(string id, string from_date, string to_date, 
            string trans_deal_type, string counter_party_code, string port, string exportType)
        {
            string StrMsg = string.Empty;
            var Result = new List<object>();
            ResultWithModel<RPReportTBMADetailResult> ResultModel = new ResultWithModel<RPReportTBMADetailResult>();
            List<RPReportTBMADetailModel> RPReportTBMADetailModel = new List<RPReportTBMADetailModel>();
            try
            {
                // Step 1 : Select Trans
                //List<string> ListTrans = new List<string>();
                //ListTrans = JsonConvert.DeserializeObject<List<string>>(id);
                //ListTrans.Sort();
                string TransNo = id;

                //for (int i = 0; i < ListTrans.Count; i++)
                //{
                //    if (i == 0)
                //    {
                //        TransNo += ListTrans[i].ToString();
                //    }
                //    else
                //    {
                //        TransNo += "," + ListTrans[i].ToString();
                //    }
                //}

                //get config
                ResultWithModel<RpConfigResult> resultRpconfig = new ResultWithModel<RpConfigResult>();
                StaticEntities staticEnt = new StaticEntities();
                List<RpConfigModel> rpConfigModelList = new List<RpConfigModel>();
                staticEnt.RpConfig.GetRpConfig("REPORT_TBMA", string.Empty, p =>
                {
                    resultRpconfig = p;
                });

                if (resultRpconfig.RefCode != 0)
                {
                    throw new Exception("GetRpConfig() => [" + resultRpconfig.RefCode.ToString() + "]" + resultRpconfig.Message);
                }

                rpConfigModelList = resultRpconfig.Data.RpConfigResultModel;

                string filename = rpConfigModelList.FirstOrDefault(a => a.item_code == "FILE_NAME").item_value;
                string sheetName = rpConfigModelList.FirstOrDefault(a => a.item_code == "SHEET_NAME").item_value;

                // Step 2 : Search Data
                RPTransModel RPTransModel = new RPTransModel();
                RPTransModel.trans_no = TransNo;
                RPTransModel.from_trade_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(from_date);
                RPTransModel.to_trade_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(to_date);
                RPTransModel.trans_deal_type = trans_deal_type;
                RPTransModel.counter_party_code = counter_party_code;
                RPTransModel.port = port;
                RPTransModel.create_by = User.UserId;

                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = 1;
                paging.RecordPerPage = 100;
                RPTransModel.paging = paging;

                //Add Orderby
                var orders = new List<OrderByModel>();
                RPTransModel.ordersby = orders;

                api_RPTrans.RPReportTBMA.GetRPReportTBMADetail(RPTransModel, exportType, p =>
                {
                    ResultModel = p;
                });

                if (ResultModel.Success == false)
                {
                    throw new Exception("ExportRPReportTBMA() => " + ResultModel.Message);
                }

                RPReportTBMADetailModel = ResultModel.Data.RPReportTBMADetailResultModel;

                // Step 2 : Export Excel XLS
                DataTable Dt_Export = new DataTable();
                //ExcelEntity ExcelEnt = new ExcelEntity();
                Dt_Export = ToDataTable(RPReportTBMADetailModel);
                filename = filename.Replace("{yyyyMMdd-HHmmssfff}", DateTime.Now.ToString("yyyyMMdd-HHmmssfff"));

                //save the file to server temp folder
                //string importexportpath = ConfigurationManager.AppSettings["ImportExportPath"].ToString();
                //string fullPath = Path.Combine(Server.MapPath(importexportpath), ExcelEnt.FileName);

                HSSFWorkbook HssWorkbook = new HSSFWorkbook();
                var sheet = HssWorkbook.CreateSheet(sheetName);

                //===== Create Column Name Style
                ICellStyle Column_Name_Style = HssWorkbook.CreateCellStyle();
                IFont FontStyle = HssWorkbook.CreateFont();
                FontStyle.Boldweight = (short)FontBoldWeight.Bold;
                Column_Name_Style.SetFont(FontStyle);
                Column_Name_Style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                Column_Name_Style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightGreen.Index;
                Column_Name_Style.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;

                //===== Create Detail Style
                ICellStyle Detail_CellStyle = HssWorkbook.CreateCellStyle();
                FontStyle = HssWorkbook.CreateFont();
                FontStyle.FontHeightInPoints = 8;
                Detail_CellStyle.SetFont(FontStyle);

                ICellStyle Detail_Number_CellStyle = HssWorkbook.CreateCellStyle();
                FontStyle = HssWorkbook.CreateFont();
                FontStyle.FontHeightInPoints = 8;
                Detail_Number_CellStyle.SetFont(FontStyle);
                Detail_Number_CellStyle.DataFormat = HssWorkbook.CreateDataFormat().GetFormat("#,##0");

                ICellStyle Detail_2Decimal_CellStyle = HssWorkbook.CreateCellStyle();
                FontStyle = HssWorkbook.CreateFont();
                FontStyle.FontHeightInPoints = 8;
                Detail_2Decimal_CellStyle.SetFont(FontStyle);
                Detail_2Decimal_CellStyle.DataFormat = HssWorkbook.CreateDataFormat().GetFormat("#,##0.00");

                ICellStyle Detail_6Decimal_CellStyle = HssWorkbook.CreateCellStyle();
                FontStyle = HssWorkbook.CreateFont();
                FontStyle.FontHeightInPoints = 8;
                Detail_6Decimal_CellStyle.SetFont(FontStyle);
                Detail_6Decimal_CellStyle.DataFormat = HssWorkbook.CreateDataFormat().GetFormat("#,##0.000000");

                ICellStyle Detail_Date_CellStyle = HssWorkbook.CreateCellStyle();
                FontStyle = HssWorkbook.CreateFont();
                FontStyle.FontHeightInPoints = 8;
                Detail_Date_CellStyle.SetFont(FontStyle);
                Detail_Date_CellStyle.DataFormat = HssWorkbook.CreateDataFormat().GetFormat("dd/mm/yyyy");

                ICellStyle Detail_Time_CellStyle = HssWorkbook.CreateCellStyle();
                FontStyle = HssWorkbook.CreateFont();
                FontStyle.FontHeightInPoints = 8;
                Detail_Time_CellStyle.SetFont(FontStyle);
                Detail_Time_CellStyle.DataFormat = HssWorkbook.CreateDataFormat().GetFormat("HH:mm");

                //===== Add Header Columns
                int rowIndex = 0;
                IRow ExcelRow = sheet.CreateRow(rowIndex);

                int iCol = 0;
                foreach (DataColumn Column in Dt_Export.Columns)
                {
                    ExcelRow.CreateCell(iCol).SetCellValue(Column.ColumnName);
                    ExcelRow.GetCell(iCol).CellStyle = Column_Name_Style;
                    sheet.SetColumnWidth(iCol, 3000);
                    iCol += 1;
                }
                rowIndex = rowIndex + 1;
                ExcelRow = sheet.CreateRow(rowIndex);

                //===== Add Detail
                for (int i = 0; i < Dt_Export.Rows.Count; i++)
                {
                    int ORDER_NUM;
                    if (Dt_Export.Rows[i]["ORDER_NUM"].ToString() != string.Empty)
                    {
                        ORDER_NUM = int.Parse(Dt_Export.Rows[i]["ORDER_NUM"].ToString());
                        ExcelRow.CreateCell(0).SetCellValue(ORDER_NUM);
                        ExcelRow.GetCell(0).CellStyle = Detail_Number_CellStyle;
                    }

                    ExcelRow.CreateCell(1).SetCellValue(Dt_Export.Rows[i]["TRADER_ID"].ToString());
                    ExcelRow.GetCell(1).CellStyle = Detail_CellStyle;

                    ExcelRow.CreateCell(2).SetCellValue(Dt_Export.Rows[i]["PURPOSE"].ToString());
                    ExcelRow.GetCell(2).CellStyle = Detail_CellStyle;

                    DateTime TRADE_TIME;
                    if (Dt_Export.Rows[i]["TRADE_TIME"].ToString() != string.Empty)
                    {
                        TRADE_TIME = DateTime.Parse(Dt_Export.Rows[i]["TRADE_TIME"].ToString());
                        ExcelRow.CreateCell(3).SetCellValue(TRADE_TIME);
                        ExcelRow.GetCell(3).CellStyle = Detail_Time_CellStyle;
                    }

                    DateTime TRADE_DATE;
                    if (Dt_Export.Rows[i]["TRADE_DATE"].ToString() != string.Empty)
                    {
                        TRADE_DATE = DateTime.Parse(Dt_Export.Rows[i]["TRADE_DATE"].ToString());
                        ExcelRow.CreateCell(4).SetCellValue(TRADE_DATE);
                        ExcelRow.GetCell(4).CellStyle = Detail_Date_CellStyle;
                    }

                    DateTime SETTLEMENT_DATE;
                    if (Dt_Export.Rows[i]["SETTLEMENT_DATE"].ToString() != string.Empty)
                    {
                        SETTLEMENT_DATE = DateTime.Parse(Dt_Export.Rows[i]["SETTLEMENT_DATE"].ToString());
                        ExcelRow.CreateCell(5).SetCellValue(SETTLEMENT_DATE);
                        ExcelRow.GetCell(5).CellStyle = Detail_Date_CellStyle;
                    }

                    ExcelRow.CreateCell(6).SetCellValue(Dt_Export.Rows[i]["TYPE"].ToString());
                    ExcelRow.GetCell(6).CellStyle = Detail_CellStyle;

                    ExcelRow.CreateCell(7).SetCellValue(Dt_Export.Rows[i]["ISSUE_SYMBOL"].ToString());
                    ExcelRow.GetCell(7).CellStyle = Detail_CellStyle;

                    double YIELD;
                    if (Dt_Export.Rows[i]["YIELD"].ToString() != string.Empty)
                    {
                        YIELD = double.Parse(Dt_Export.Rows[i]["YIELD"].ToString());
                        ExcelRow.CreateCell(8).SetCellValue(YIELD);
                        ExcelRow.GetCell(8).CellStyle = Detail_6Decimal_CellStyle;
                    }

                    ExcelRow.CreateCell(9).SetCellValue(Dt_Export.Rows[i]["YIELD_TYPE"].ToString());
                    ExcelRow.GetCell(9).CellStyle = Detail_CellStyle;

                    double PRICE;
                    if (Dt_Export.Rows[i]["PRICE"].ToString() != string.Empty)
                    {
                        PRICE = double.Parse(Dt_Export.Rows[i]["PRICE"].ToString());
                        ExcelRow.CreateCell(10).SetCellValue(PRICE);
                        ExcelRow.GetCell(10).CellStyle = Detail_6Decimal_CellStyle;
                    }

                    int VOLUME;
                    if (Dt_Export.Rows[i]["VOLUME"].ToString() != string.Empty)
                    {
                        VOLUME = int.Parse(Dt_Export.Rows[i]["VOLUME"].ToString());
                        ExcelRow.CreateCell(11).SetCellValue(VOLUME);
                        ExcelRow.GetCell(11).CellStyle = Detail_Number_CellStyle;
                    }

                    ExcelRow.CreateCell(12).SetCellValue(Dt_Export.Rows[i]["COUNTER_PARTY"].ToString());
                    ExcelRow.GetCell(12).CellStyle = Detail_CellStyle;

                    ExcelRow.CreateCell(13).SetCellValue(Dt_Export.Rows[i]["TERM"].ToString());
                    ExcelRow.GetCell(13).CellStyle = Detail_CellStyle;

                    double RATE;
                    if (Dt_Export.Rows[i]["RATE"].ToString() != string.Empty)
                    {
                        RATE = double.Parse(Dt_Export.Rows[i]["RATE"].ToString());
                        ExcelRow.CreateCell(14).SetCellValue(RATE);
                        ExcelRow.GetCell(14).CellStyle = Detail_6Decimal_CellStyle;
                    }

                    ExcelRow.CreateCell(15).SetCellValue(Dt_Export.Rows[i]["REMARK"].ToString());
                    ExcelRow.GetCell(15).CellStyle = Detail_CellStyle;

                    rowIndex = rowIndex + 1;
                    ExcelRow = sheet.CreateRow(rowIndex);
                }

                //if (System.IO.File.Exists(fullPath) == true)
                //    System.IO.File.Delete(fullPath);
                //FileStream FileData = new FileStream(fullPath, FileMode.Create);
                //HssWorkbook.Write(FileData);
                //FileData.Close();

                MemoryStream exportfile = new MemoryStream();
                HssWorkbook.Write(exportfile);
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();

                Response.AppendHeader("Content-Type", "application/vnd.ms-excel");
                Response.AppendHeader("Cache-Control", "must-revalidate, post-check=0, pre-check=0");
                Response.AppendHeader("Cache-Control", "max-age=30");
                Response.AppendHeader("Pragma", "public");
                Response.AppendHeader("Content-disposition", "attachment; filename=" + filename + ".xls");

                Response.BinaryWrite(exportfile.GetBuffer());
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View("Index");
        }

        [HttpGet]
        public ActionResult Download(string filename)
        {
            //get the temp folder and file path in server
            string importexportpath = ConfigurationManager.AppSettings["ImportExportPath"].ToString();
            string fullPath = Path.Combine(Server.MapPath(importexportpath), filename); 

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/vnd.ms-excel", filename);
        }

        private DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

    }
}