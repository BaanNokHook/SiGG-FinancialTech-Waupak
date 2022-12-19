using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.ExternalInterface;
using GM.Data.Model.Static;
using GM.Data.Result.ExternalInterface;
using GM.Data.Result.Static;
using GM.Data.View.Static;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    public class MarketPriceBBGController : Controller
    {
        // GET: MarketPriceBBG
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FillCur(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            StaticEntities api = new StaticEntities();
            api.Currency.GetDDLCurrency(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        private ResultWithModel<DataSet> MarketPriceBBGList(InterfaceMarketPriceModel model)
        {
            ExternalInterfaceEntities api = new ExternalInterfaceEntities();
            ResultWithModel<DataSet> result = new ResultWithModel<DataSet>();
            api.InterfaceMarketPriceBBG.ImportMarketPriceBBGList(model, p =>
            {
                result = p;
            });

            return result;
        }

        [HttpPost]
        public JsonResult Search(MarketPriceBBGViewModel modelView)
        {
            try
            {
                InterfaceMarketPriceModel model = new InterfaceMarketPriceModel();
                model.security_code = modelView.security_code;
                if (modelView.as_of_date != null)
                {
                    model.asof_date = modelView.as_of_date.Value.ToString("yyyyMMdd");
                }
                else
                {
                    model.asof_date = null;
                }
                model.cur = model.cur;
                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = 1;
                paging.RecordPerPage = 9999999;
                model.paging = paging;

                ResultWithModel<DataSet> result = MarketPriceBBGList(model);

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
                                dRow.Add(col.ColumnName, row[col]);
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
        public JsonResult RequestData(MarketPriceBBGViewModel modelView)
        {
            try
            {
                //Step 1 : Get Config
                InterfaceMarketPriceModel interfaceMarketPrice = new InterfaceMarketPriceModel();
                interfaceMarketPrice.security_code = modelView.security_code;

                if (modelView.as_of_date != null)
                {
                    interfaceMarketPrice.asof_date = modelView.as_of_date.Value.ToString("yyyyMMdd");
                }
                else
                {
                    interfaceMarketPrice.asof_date = null;
                }
                interfaceMarketPrice.cur = modelView.cur;

                ResultWithModel<RpConfigResult> resultRpconfig = new ResultWithModel<RpConfigResult>();
                StaticEntities staticEnt = new StaticEntities();
                List<RpConfigModel> rpConfigModelList = new List<RpConfigModel>();
                string StrMsg = string.Empty;
                staticEnt.RpConfig.GetRpConfig("RP_FITS_INTERFACE_MARKET_PRICE_BBG", string.Empty, p =>
                        {
                            resultRpconfig = p;
                        });

                if (resultRpconfig.RefCode != 0)
                {
                    throw new Exception("GetMarketPriceBBGConfig() => [" + resultRpconfig.RefCode.ToString() + "]" + resultRpconfig.Message);
                }

                rpConfigModelList = resultRpconfig.Data.RpConfigResultModel;
                if (!Set_ConfigInterfaceMarketPrice(ref StrMsg, rpConfigModelList, ref interfaceMarketPrice, modelView))
                {
                    return Json(new { Status = "Error", Message = "Error!!! " + StrMsg, Data = new object[] { }, Columns = new object[] { } },
                      "application/json",
                      Encoding.UTF8,
                      JsonRequestBehavior.AllowGet);
                }

                ExternalInterfaceEntities interfaceEnt = new ExternalInterfaceEntities();

                //select RP Confirmation
                ResultWithModel<InterfaceMarketPriceBBGResult> rwm = new ResultWithModel<InterfaceMarketPriceBBGResult>();

                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = 1;
                paging.RecordPerPage = int.MaxValue;
                interfaceMarketPrice.paging = paging;

                //Add Orderby
                var orders = new List<OrderByModel>();
                interfaceMarketPrice.ordersby = orders;

                interfaceEnt.InterfaceMarketPriceBBG.ImportMarketPriceBBG(interfaceMarketPrice, p =>
                {
                    rwm = p;
                });

                return Json(new { Status = "Success", Message = "Success.", Data = new object[] { }, Columns = new object[] { } },
                       "application/json",
                       Encoding.UTF8,
                       JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error", Message = "Error!!! " + ex.Message, Data = new object[] { }, Columns = new object[] { } },
                   "application/json",
                   Encoding.UTF8,
                   JsonRequestBehavior.AllowGet);
            }
        }

        private static bool Set_ConfigInterfaceMarketPrice(ref string ReturnMsg, List<RpConfigModel> List_RpConfigModel, ref InterfaceMarketPriceModel interfaceMarketPrice, MarketPriceBBGViewModel Model)
        {
            try
            {
                Utility utility = new Utility();
                //Set Req Data InterfaceMarketPriceModel form config
                DateTime date = DateTime.Now;
                interfaceMarketPrice.channel = List_RpConfigModel.FirstOrDefault(a => a.item_code == "CHANNEL")?.item_value;
                interfaceMarketPrice.ref_no = date.ToString("yyyyMMddHHMMss");
                interfaceMarketPrice.request_date = date.ToString("yyyyMMdd");
                interfaceMarketPrice.request_time = date.ToString("HH:MM:ss");
                interfaceMarketPrice.mode = int.Parse(List_RpConfigModel.FirstOrDefault(a => a.item_code == "MODE")?.item_value);
                if (Model.as_of_date != null)
                {
                    interfaceMarketPrice.asof_date = Model.as_of_date.Value.ToString("yyyyMMdd"); //20170720
                }
                interfaceMarketPrice.source_type = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SOURCE_TYPE")?.item_value;
                if (interfaceMarketPrice.security_code == string.Empty)
                {
                    interfaceMarketPrice.security_code = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SECURITY_CODE")?.item_value;
                }
                interfaceMarketPrice.urlservice = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_URL")?.item_value;
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }
    }
}