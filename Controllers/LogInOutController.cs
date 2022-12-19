using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.Static;
using GM.Data.Result.Static;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class LogInOutController : BaseController
    {
        StaticEntities api_static = new StaticEntities();
        Utility utility = new Utility();

        // GET: LogInOut
        public ActionResult Index()
        {
            if (!IsCheckPermission())
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            ResultWithModel<LogInOutResult> result = new ResultWithModel<LogInOutResult>();
            LogInOutModel logInOutModel = new LogInOutModel();
            try
            {
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = model.pageno;
                paging.RecordPerPage = model.length;
                logInOutModel.paging = paging;

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

                logInOutModel.ordersby = orders;

                var columns = model.columns.Where(o => o.search.value != null).ToList();
                columns.ForEach(column =>
                {
                    switch (column.data)
                    {
                        case "module_name":
                            logInOutModel.module_name = column.search.value;
                            break;
                        case "action_name":
                            logInOutModel.action_name = column.search.value;
                            break;
                        case "status":
                            logInOutModel.status = column.search.value;
                            break;
                        case "create_date":
                            logInOutModel.create_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(column.search.value);
                            break;
                    }
                });

                logInOutModel.create_by = User.UserId;

                api_static.LogInOut.GetLogInOutList(logInOutModel, p => { result = p; });

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return Json(new
            {
                draw = model.draw,
                recordsTotal = result.HowManyRecord,
                recordsFiltered = result.HowManyRecord,
                Message = result.Message,
                data = result.Data != null ? result.Data.LogInOutResultModel : new List<LogInOutModel>()
            });
        }

        [HttpPost]
        public ActionResult Download(int svcId, string type)
        {
            try
            {
                string sql;
                if (type == "req")
                {
                    sql = string.Format("select svc_req from GM_service_in_out_req where svc_id={0}", svcId);
                }
                else if (type == "res")
                {
                    sql = string.Format("select svc_res from GM_service_in_out_req where svc_id={0}", svcId);
                }
                else
                {
                    return Content("no found", "text/html");
                }

                ResultWithModel<DataSet> result = new ResultWithModel<DataSet>();
                TestPageModel model = new TestPageModel();
                model.text = sql;
                model.create_by = User.UserId;

                api_static.TestPage.GetTestPageList(model, p =>
                {
                    result = p;
                });

                if (result.Success)
                {
                    if (result.Data != null)
                    {
                        MemoryStream ms = new MemoryStream();
                        TextWriter tw = new StreamWriter(ms);
                        if (result.Data.Tables[0].Rows.Count > 0)
                        {
                            tw.WriteLine(result.Data.Tables[0].Rows[0][0].ToString());
                        }
                        tw.Flush();
                        byte[] bytes = ms.ToArray();
                        ms.Close();

                        Response.Clear();
                        Response.ContentType = "application/force-download";
                        Response.AddHeader("content-disposition", "attachment; filename=result_" + type + "_" + svcId + ".txt");
                        Response.BinaryWrite(bytes);
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