using GM.CommonLibs;
using GM.CommonLibs.Constants;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.ExternalInterface.InterfaceThorIndex;
using GM.Data.Result.ExternalInterface;
using GM.Data.View.ExternalInterface;
using GM.Filters;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]

    public class ThorIndexController : BaseController
    {
        ExternalInterfaceEntities api = new ExternalInterfaceEntities();
        SecurityEntities api_security = new SecurityEntities();
        GM.Data.Helper.Utility utility = new GM.Data.Helper.Utility();
        private static LogFile Log = new LogFile();
        private static string controller = "ThorIndex";

        // Action Index
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Index()
        {
            ThorIndexViewModel model = new ThorIndexViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            ResultWithModel<ThorIndexResult> Result = new ResultWithModel<ThorIndexResult>();
            ThorIndexModel reqModel = new ThorIndexModel();

            //Add Paging
            PagingModel paging = new PagingModel();
            paging.PageNumber = model.pageno;
            paging.RecordPerPage = model.length;
            reqModel.paging = paging;

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

            reqModel.ordersby = orders;

            var columns = model.columns.Where(o => o.search.value != null).ToList();
            columns.ForEach(column =>
            {
                switch (column.data)
                {
                    case "asof_date_from":
                        reqModel.asof_date_from = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                        break;
                    case "asof_date_to":
                        reqModel.asof_date_to = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                        break;
                    case "instrument_id":
                        if (!string.IsNullOrEmpty(column.search.value) && !string.IsNullOrWhiteSpace(column.search.value))
                        {
                            reqModel.instrument_id = Convert.ToInt32(column.search.value);
                        }
                      
                        break;
                }
            });

            api.InterfaceThorIndexFits.GetThorIndex(reqModel, p =>
            {
                Result = p;
            });

            return Json(new
            {
                draw = model.draw,
                recordsTotal = Result.HowManyRecord,
                recordsFiltered = Result.HowManyRecord,
                data = Result.Data != null ? Result.Data.ThorIndexResultModel : new List<ThorIndexModel>()
            });
        }

        public ActionResult FillInstrument(string text)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_security.Security.GetDDLInstrumentCode(text, p => {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult ExportData(ThorIndexModel model)
        {
            string strMsg = string.Empty;
            string filename = "";
            try
            {
                // Export Excel XLS
                DataTable dt = new DataTable();
                ExcelEntity ExcelEnt = new ExcelEntity();
                model.mode = "EXPORT";
                dt = GetReportData(model);
                if (dt == null)
                {
                    throw new Exception("No Data.");
                }

                ExcelEnt.FileName = "ThorIndex.xls";
                ExcelEnt.SheetName = "ThorIndex";
                filename = "ThorIndex.xls";

                //save the file to server temp folder
                string importexportpath = ConfigurationManager.AppSettings["ImportExportPath"].ToString();
                string fullPath = Path.Combine(Server.MapPath(importexportpath), ExcelEnt.FileName);

                HSSFWorkbook workbook = new HSSFWorkbook();
                var sheet = workbook.CreateSheet(ExcelEnt.SheetName);

                var excelTemplate = new ExcelTemplate(workbook);

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

                for (var i = 0; i <= colIndex; i++)
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

                FileStream FileData = new FileStream(fullPath, FileMode.Create);
                workbook.Write(FileData);
                FileData.Close();
            }
            catch (Exception ex)
            {
                strMsg = ex.Message;
                Log.WriteLog(controller, "Error : " + ex.Message);
            }
            return Json(new { fileName = filename, errorMessage = strMsg }, JsonRequestBehavior.AllowGet);
        }

        public DataTable GetReportData(ThorIndexModel model)
        {
            ResultWithModel<ThorRateResult> result = new ResultWithModel<ThorRateResult>();

            //Add Paging
            PagingModel paging = new PagingModel();
            model.paging = paging;

            var dt = new DataTable();

            api.InterfaceThorIndexFits.GetThorIndexDataTable(model, p =>
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
        public ActionResult Download(string filename)
        {
            //get the temp folder and file path in server
            string importexportpath = ConfigurationManager.AppSettings["ImportExportPath"].ToString();
            string fullPath = Path.Combine(Server.MapPath(importexportpath), filename);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/vnd.ms-excel", filename);
        }
    }
}