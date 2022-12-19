using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GM.CommonLibs;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.ExternalInterface;
using GM.Data.Model.Static;
using GM.Data.Result.ExternalInterface;
using GM.Data.Result.Static;
using GM.Data.View.ExternalInterface;
using GM.Filters;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class InternalPLController : BaseController
    {
        private Utility _utility = new Utility();
        private static LogFile _log = new LogFile();
        private ExternalInterfaceEntities _interfaceEnt = new ExternalInterfaceEntities();
        private static string _controller = "InternalPLController";

        public ActionResult Index()
        {
            if (!IsCheckPermission())
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public JsonResult Search(InternalPLViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.asof_date_from) || string.IsNullOrEmpty(model.asof_date_to))
                {
                    return Json(new { Status = "Error", Message = "require date" });
                }

                ResultWithModel<DataSet> res = new ResultWithModel<DataSet>();

                DateTime asOfDateFrom = _utility.ConvertStringToDatetimeFormatDDMMYYYY(model.asof_date_from);
                DateTime asOfDateTo = _utility.ConvertStringToDatetimeFormatDDMMYYYY(model.asof_date_to);

                _log.WriteLog(_controller, " - asof_date_from: " + asOfDateFrom.Date);
                _log.WriteLog(_controller, " - asof_date_to: " + asOfDateTo.Date);

                _interfaceEnt.InternalPl.GetList(
                    new InternalPLModel()
                    {
                        asof_date_from = asOfDateFrom,
                        asof_date_to = asOfDateTo
                    }, p =>
                    {
                        res = p;
                    });

                if (res.Success)
                {
                    if (res.Data != null && res.Data.Tables.Count > 0)
                    {
                        List<Dictionary<string, object>> dataRows = new List<Dictionary<string, object>>();
                        List<Dictionary<string, object>> columns = new List<Dictionary<string, object>>();

                        foreach (DataColumn col in res.Data.Tables[0].Columns)
                        {
                            Dictionary<string, object> dCol = new Dictionary<string, object>();
                            dCol.Add("title", col.ColumnName);
                            dCol.Add("data", col.ColumnName);
                            columns.Add(dCol);
                        }
                        foreach (DataRow row in res.Data.Tables[0].Rows)
                        {
                            Dictionary<string, object> dRow = new Dictionary<string, object>();
                            foreach (DataColumn col in res.Data.Tables[0].Columns)
                            {
                                if (col.DataType == typeof(DateTime))
                                {
                                    DateTime date;
                                    bool isConvert = DateTime.TryParse(row[col].ToString().Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                                    if (isConvert)
                                    {
                                        dRow.Add(col.ColumnName, date.ToString("yyyy-MM-dd HH:mm:ss"));
                                    }
                                    else
                                    {
                                        dRow.Add(col.ColumnName, string.Empty);
                                    }
                                }
                                else
                                {
                                    dRow.Add(col.ColumnName, row[col]);
                                }
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
                    return Json(new { Status = "Error", Message = "Error!!! " + res.Message, Data = new object[] { }, Columns = new object[] { } },
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
        public JsonResult ReRun(InternalPLViewModel model)
        {
            ResultWithModel<DataSet> res = new ResultWithModel<DataSet>();

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                _log.WriteLog(_controller, "Start ReRun PL ==========");

                DateTime asOfDateFrom = _utility.ConvertStringToDatetimeFormatDDMMYYYY(model.asof_date_from);
                DateTime asOfDateTo = _utility.ConvertStringToDatetimeFormatDDMMYYYY(model.asof_date_to);

                _log.WriteLog(_controller, " - asof_date_from: " + asOfDateFrom.Date);
                _log.WriteLog(_controller, " - asof_date_to: " + asOfDateTo.Date);

                _interfaceEnt.InternalPl.ReRunPL(
                    new InternalPLModel()
                    {
                        asof_date_from = asOfDateFrom,
                        asof_date_to = asOfDateTo
                    }, p =>
                {
                    res = p;
                });

                if (!res.Success)
                {
                    throw new Exception(res.Message);
                }

                _log.WriteLog(_controller, "Success");
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = ex.Message;
                _log.WriteLog(_controller, "Error: "+ ex.Message);
            }
            finally
            {
                _log.WriteLog(_controller, "End ReRun PL ==========");
            }

            return Json(res);
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