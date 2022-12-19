using Antlr.Runtime.Misc;
using CsvHelper;
using GM.CommonLibs;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.ExternalInterface;
using GM.Data.Model.StockReconcile;
using GM.Data.Result.ExternalInterface;
using GM.Data.Result.StockReconcile;
using GM.Data.View.ExternalInterface;
using GM.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class ImportExportDataController : BaseController
    {
        private readonly string _dateStr;
        private readonly string _pathUpload;
        private readonly string _folderUpload;
        private string _alertMsg = "";
        private string _alertStatus = "";
        private Utility _utility;

        public ImportExportDataController()
        {
            _dateStr = DateTime.Now.ToString("yyyyMMdd");
            _folderUpload = "ImportData";
        }

        #region MarketPrice
        [HttpGet]
        public ActionResult MarketPrice()
        {
            ImportExportDataViewModel model = new ImportExportDataViewModel();
            ViewBag.AlertStatus = "";
            ViewBag.AlertMsg = "";
            return View(model);
        }

        //import
        [HttpPost]
        public ActionResult MarketPrice(ImportExportDataViewModel model)
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var fileContent = Request.Files[0];

                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(fileContent.FileName);
                        var extension = Path.GetExtension(fileContent.FileName);
                        //var contentType = fileContent.ContentType;

                        //validate
                        if (model.ImportTo == "MARKET_PRICE_T0")
                        {
                            var validateFilename = fileName != null && fileName.Contains("ReferenceYield_");
                            if (!validateFilename)
                            {
                                throw new Exception("incorrect file name (ex.ReferenceYield_xxxxxxxx)");
                            }
                        }
                        else if (model.ImportTo == "MARKET_PRICE_T1")
                        {
                            var validateFilename = fileName != null && fileName.Contains("ReferenceYield(T+2)_");
                            if (!validateFilename)
                            {
                                throw new Exception("incorrect file name (ex.ReferenceYield(T+2)_xxxxxxxx)");
                            }
                        }
                        else if (model.ImportTo == "MARKET_PRICE_T2")
                        {
                            var validateFilename = fileName != null && fileName.Contains("ReferenceYield(T+3)_");
                            if (!validateFilename)
                            {
                                throw new Exception("incorrect file name (ex.ReferenceYield(T+3)_xxxxxxxx)");
                            }
                        }

                        fileName = fileName + DateTime.Now.ToString("_HHmmss") + extension;

                        //write file
                        var path = Path.Combine(GetDirectory(), fileName);
                        using (FileStream fileStream = new FileStream(path, FileMode.Create))
                        {
                            fileContent.InputStream.Seek(0, SeekOrigin.Begin);
                            fileContent.InputStream.CopyTo(fileStream);
                            
                        }

                        //process import
                        ImportToMarketPrice(model, extension, path);
                    }
                }
            }
            catch (Exception ex)
            {
                _alertStatus = "Error";
                _alertMsg = ex.Message;
            }

            ViewBag.AlertStatus = _alertStatus;
            ViewBag.AlertMsg = _alertMsg;
            return View();
        }

        private void ImportToMarketPrice(ImportExportDataViewModel model, string extension, string path)
        {
            InterfaceRpReferenceModel interfaceRpModel = new InterfaceRpReferenceModel();
            interfaceRpModel.success = true;
            interfaceRpModel.message = "Run Successfully";
            interfaceRpModel.serverity = "";
            interfaceRpModel.rdfcode = 0;
            interfaceRpModel.total = 1;

            string asof = model.ImportDate.Value.ToString("dd/MM/yyyy");
            string settlementDay = "";
            if (model.ImportTo == "MARKET_PRICE_T0")
            {
                settlementDay = "1";
            }
            else if (model.ImportTo == "MARKET_PRICE_T1")
            {
                settlementDay = "2";
            }
            else if (model.ImportTo == "MARKET_PRICE_T2")
            {
                settlementDay = "3";
            }

            int rowRp = 0;
            int readRow = 0;
            int bondTypeCount = 0;
            InterfaceRpReferenceModel.Datas data = new InterfaceRpReferenceModel.Datas();
            List<InterfaceRpReferenceModel.Rp> rps = new ListStack<InterfaceRpReferenceModel.Rp>();

            
            if (extension == ".csv")//csv
            {
                #region CSV
                CultureInfo culture = new CultureInfo("en-GB");
                using (CsvReader csv = new CsvReader(new StreamReader(path), true))
                {
                    string bondType = "";
                    string stepCol = "";
                    while (csv.Read())
                    {
                        stepCol = "";

                        try
                        {
                            ReadingContext context = csv.Context;
                            if (csv.Context.Row == 1)
                            {
                                DateTime tempDate = System.Convert.ToDateTime(context.Record[0], culture);
                                if (DateTime.Compare(tempDate.Date, model.ImportDate.Value.Date) != 0)
                                {
                                    throw new Exception("Date select not match date in file.");
                                }
                            }

                            if (csv.Context.Row < 3) continue;//skip 2 row

                            if (context.Record.Count() == 1)
                            {
                                if (context.Record[0] == "Foreign Bonds") break;

                                bondTypeCount++;
                                bondType = bondTypeCount + "." + context.Record[0].Trim();
                                continue;
                            }

                            //skip 15 col
                            if (context.Record.Count() < 15) continue;

                            rowRp++;
                            InterfaceRpReferenceModel.Rp rp = new InterfaceRpReferenceModel.Rp();
                            stepCol = "[Symbol]";
                            rp.symbol = context.Record[0].Trim();
                            stepCol = "[Maturity]";
                            rp.maturity_date = System.Convert.ToDateTime(context.Record[1], culture).ToString("dd/MM/yyyy");
                            stepCol = "[Avg.Bidding]";
                            rp.avg_bidding = (context.Record[2] == "-") ? "" : context.Record[2];
                            stepCol = "[Govt.Interpolated Yield]";
                            rp.govt_interpolated_yield = (context.Record[3] == "-") ? "" : context.Record[3].Trim();
                            stepCol = "[TTM (Yrs.)]";
                            rp.ttm = (context.Record[4] == "-") ? "" : context.Record[4].Trim();
                            stepCol = "[Spread]";
                            rp.spread = (context.Record[6] == "-") ? "" : context.Record[6];
                            stepCol = "[Reference Yield]";
                            rp.reference_yield = (context.Record[7] == "-") ? "" : context.Record[7].Trim();
                            stepCol = "[Settlement Date]";
                            rp.settlement_date = System.Convert.ToDateTime(context.Record[8], culture).ToString("dd/MM/yyyy");
                            stepCol = "[AI %]";
                            rp.ai = (context.Record[9] == "-") ? "" : context.Record[9].Trim();
                            stepCol = "[Gross Price %]";
                            rp.gross_price_percent = (context.Record[10] == "-") ? "" : context.Record[10].Trim();
                            stepCol = "[Clean Price %]";
                            rp.clean_price_percent = (context.Record[11] == "-") ? "" : context.Record[11].Trim();
                            stepCol = "[Modified Duration*]";
                            rp.modified_duration = (context.Record[12] == "-") ? "" : context.Record[12].Trim();
                            stepCol = "[Convexity]";
                            rp.convexity = (context.Record[13] == "-") ? "" : context.Record[13].Trim();
                            stepCol = "[Index Ratio]";
                            rp.index_ratio = (context.Record[14] == "-") ? "" : context.Record[14];
                            rp.bond_type = bondType;
                            rp.settlement_day = settlementDay;
                            rp.asof = asof;
                            rp.row_number = rowRp.ToString();
                            rp.data_Id = "0";
                            rps.Add(rp);
                        }
                        catch (Exception ex)
                        {
                            _alertStatus = "Error";
                            _alertMsg = stepCol + ex.Message;
                            return;
                        }
                    }
                }

                #endregion
            }
            else if (extension == ".xlsx" || extension == ".xls")//xlsx
            {
                #region Excel
                DataTable dt = ExcelReader.GetDataTableFromExcel(path);
                if (dt.Rows.Count > 0)
                {
                    string bondType = "";
                    string stepCol = "";
                    CultureInfo culture = new CultureInfo("en-GB");
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        stepCol = "";

                        try
                        {
                            readRow++;
                            if (readRow == 1)
                            {
                                DateTime tempDate = System.Convert.ToDateTime(dataRow[0].ToString(), culture);
                                if (DateTime.Compare(tempDate.Date, model.ImportDate.Value.Date) != 0)
                                {
                                    throw new Exception("Date select not match date in file.");
                                }
                            }

                            if (readRow < 3) continue;//skip 2 row

                            if (dataRow[1].ToString() == "")
                            {
                                if (dataRow[0].ToString() == "Foreign Bonds") break;

                                bondTypeCount++;
                                bondType = bondTypeCount + "." + dataRow[0].ToString();
                                continue;
                            }

                            //skip 15 col
                            if (dataRow.ItemArray.Count() < 15) continue;

                            rowRp++;
                            InterfaceRpReferenceModel.Rp rp = new InterfaceRpReferenceModel.Rp();
                            stepCol = "[Symbol]";
                            rp.symbol = dataRow[0].ToString();
                            stepCol = "[Maturity]";
                            rp.maturity_date = System.Convert.ToDateTime(dataRow[1].ToString(), culture).ToString("dd/MM/yyyy");
                            stepCol = "[Avg.Bidding]";
                            rp.avg_bidding = (dataRow[2].ToString() == "-") ? "" : dataRow[2].ToString();
                            stepCol = "[Govt.Interpolated Yield]";
                            rp.govt_interpolated_yield = (dataRow[3].ToString() == "-") ? "" : dataRow[3].ToString();
                            stepCol = "[TTM (Yrs.)]";
                            rp.ttm = (dataRow[4].ToString() == "-") ? "" : dataRow[4].ToString();
                            stepCol = "[Spread]";
                            rp.spread = (dataRow[6].ToString() == "-") ? "" : dataRow[6].ToString();
                            stepCol = "[Reference Yield]";
                            rp.reference_yield = (dataRow[7].ToString() == "-") ? "" : dataRow[7].ToString();
                            stepCol = "[Settlement Date]";
                            rp.settlement_date = System.Convert.ToDateTime(dataRow[8].ToString(), culture).ToString("dd/MM/yyyy");
                            stepCol = "[AI %]";
                            rp.ai = (dataRow[9].ToString() == "-") ? "" : dataRow[9].ToString();
                            stepCol = "[Gross Price %]";
                            rp.gross_price_percent = (dataRow[10].ToString() == "-") ? "" : dataRow[10].ToString();
                            stepCol = "[Clean Price %]";
                            rp.clean_price_percent = (dataRow[11].ToString() == "-") ? "" : dataRow[11].ToString();
                            stepCol = "[Modified Duration*]";
                            rp.modified_duration = (dataRow[12].ToString() == "-") ? "" : dataRow[12].ToString();
                            stepCol = "[Convexity]";
                            rp.convexity = (dataRow[13].ToString() == "-") ? "" : dataRow[13].ToString();
                            stepCol = "[Index Ratio]";
                            rp.index_ratio = (dataRow[14].ToString() == "-") ? "" : dataRow[14].ToString();
                            rp.bond_type = bondType;
                            rp.settlement_day = settlementDay;
                            rp.asof = asof;
                            rp.row_number = rowRp.ToString();
                            rp.data_Id = "0";
                            rps.Add(rp);

                        }
                        catch (Exception ex)
                        {
                            _alertStatus = "Error";
                            _alertMsg = stepCol + ex.Message;
                            return;
                        }
                    }
                }
                #endregion
            }

            data.asof_date = model.ImportDate.Value.ToString("yyyyMMdd") + "_T" + settlementDay;
            data.channel_id = "ThaiBMA";
            data.asof_time = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");
            data.total_rec = rowRp;
            data.ref_id = Guid.NewGuid().ToString().ToUpper();
            data.rp = rps;

            if (rps.Count == 0)
            {
                _alertStatus = "Error";
                _alertMsg = "No Data";
                return;
            }

            interfaceRpModel.datas = data;

            #region connect WS_RepoImportRpRefSoapClient

            //string strJson = JsonConvert.SerializeObject(interfaceRpModel);

            //WS_RepoImportRpRefSoapClient repoImportRpRefService = new WS_RepoImportRpRefSoapClient();
            //string resultStr = repoImportRpRefService.Import_RpReference(strJson, asof, settlementDay);
            //var objResult = JsonConvert.DeserializeObject<dynamic>(resultStr);
            //if (objResult.response_code == "0")
            //{
            //    _alertStatus = "Success";
            //    _alertMsg = objResult.response_message;
            //}
            //else
            //{
            //    _alertStatus = "Error";
            //    _alertMsg = objResult.response_message;
            //}
            #endregion

            #region req ws in .net core

            ResultWithModel<InterfaceRpReferenceResult> result = new ResultWithModel<InterfaceRpReferenceResult>();
            ExternalInterfaceEntities api_ext = new ExternalInterfaceEntities();
            api_ext.InterfaceRpReference.ImportRpReference(asof, settlementDay, interfaceRpModel, p =>
            {
                result = p;
            });

            if (result.Data != null)
            {
                if (result.Data.response_code == "0")
                {
                    _alertStatus = "Success";
                    _alertMsg = result.Data.response_message;
                }
                else
                {
                    _alertStatus = "Error";
                    _alertMsg = result.Data.response_message;
                }
            }

            #endregion

        }
        #endregion

        #region NavPrice
        [HttpGet]
        public ActionResult NavPrice()
        {
            ImportExportDataViewModel model = new ImportExportDataViewModel();
            ViewBag.AlertStatus = "";
            ViewBag.AlertMsg = "";
            return View(model);
        }

        [HttpPost]
        public ActionResult NavPrice(ImportExportDataViewModel model)
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var fileContent = Request.Files[0];

                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(fileContent.FileName);
                        var extension = Path.GetExtension(fileContent.FileName);

                        if (model.ImportTo == "NAV_PRICE")
                        {
                            var validateFilename = fileName != null && fileName.Contains("Nav Center_");
                            if (!validateFilename)
                            {
                                throw new Exception("incorrect file name (ex.Nav Center_ddMMyyyy.csv)");
                            }

                            string importDate = model.ImportDate.ToString();
                            string validateImportDate = System.Convert.ToDateTime(importDate).ToString("ddMMyyyy");
                            if (!fileName.Contains(validateImportDate))
                            {
                                throw new Exception("incorrect file name not match import date");
                            }

                            fileName = fileName + DateTime.Now.ToString("_HHmmss") + extension;

                            //write file
                            var path = Path.Combine(GetDirectory(), fileName);
                            using (FileStream fileStream = new FileStream(path, FileMode.Create))
                            {
                                fileContent.InputStream.Seek(0, SeekOrigin.Begin);
                                fileContent.InputStream.CopyTo(fileStream);

                            }

                            //process import
                            ImportToNavPrice(model, extension, path);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _alertStatus = "Error";
                _alertMsg = ex.Message;
            }

            ViewBag.AlertStatus = _alertStatus;
            ViewBag.AlertMsg = _alertMsg;
            return View(model);
        }

        private void ImportToNavPrice(ImportExportDataViewModel model, string extension, string path)
        {
            InterfaceNavPriceModel interfaceNavPriceModel = new InterfaceNavPriceModel();
            interfaceNavPriceModel.success = true;
            interfaceNavPriceModel.message = "Run Successfully";
            interfaceNavPriceModel.serverity = "";
            interfaceNavPriceModel.rdfcode = 0;
            interfaceNavPriceModel.total = 1;

            int rowNav = 0;
            string ref_no = Guid.NewGuid().ToString().ToUpper();
            InterfaceNavPriceModel.Datas data = new InterfaceNavPriceModel.Datas();
            List<InterfaceNavPriceModel.NavPrice> listNavPrice = new ListStack<InterfaceNavPriceModel.NavPrice>();

            if (extension == ".csv")//csv
            {
                #region CSV
                CultureInfo culture = new CultureInfo("en-GB");
                using (CsvReader csv = new CsvReader(new StreamReader(path, Encoding.GetEncoding("TIS-620"), true), true))
                {
                    string stepCol = "";

                    while (csv.Read())
                    {
                        stepCol = "";

                        try
                        {
                            ReadingContext context = csv.Context;

                            //skip 15 col
                            //if (context.Record.Count() < 15) continue;

                            rowNav++;
                            InterfaceNavPriceModel.NavPrice NavPrice = new InterfaceNavPriceModel.NavPrice();

                            stepCol = "[ref_no]";
                            NavPrice.ref_no = ref_no;
                      
                            stepCol = "[source_name]";
                            NavPrice.source_name = "SET";

                            stepCol = "[asofdate]";
                            NavPrice.asofdate = System.Convert.ToDateTime(context.Record[0], culture).ToString("yyyyMMdd");

                            stepCol = "[fund_type]";
                            NavPrice.fund_type = context.Record[1].Trim();

                            stepCol = "[investment_type]";
                            NavPrice.investment_type = context.Record[2].Trim();

                            stepCol = "[issuer_code]";
                            NavPrice.issuer_code = context.Record[3].Trim();

                            stepCol = "[custodian_code]";
                            NavPrice.custodian_code = context.Record[4].Trim();

                            stepCol = "[fund_name_th]";
                            NavPrice.fund_name_th = context.Record[5].Trim();

                            stepCol = "[fund_name_eng]";
                            NavPrice.fund_name_eng = context.Record[6].Trim();

                            stepCol = "[symbol]";
                            NavPrice.symbol = context.Record[7].Trim();

                            stepCol = "[nav]";
                            NavPrice.nav = context.Record[8].Trim();

                            stepCol = "[nav_per_unit]";
                            NavPrice.nav_per_unit = context.Record[9].Trim();

                            stepCol = "[nav_per_unit_change]";
                            NavPrice.nav_per_unit_change = context.Record[10].Trim();

                            stepCol = "[xd_date]";
                            if (context.Record[11].Trim() != "")
                            {
                                NavPrice.xd_date = System.Convert.ToDateTime(context.Record[11], culture).ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                NavPrice.xd_date = "";
                            }
                            
                            stepCol = "[sell_price]";
                            NavPrice.sell_price = context.Record[12].Trim();

                            stepCol = "[buy_price]";
                            NavPrice.buy_price = context.Record[13].Trim();

                            stepCol = "[import_by]";
                            NavPrice.import_by = HttpContext.User.Identity.Name;

                            listNavPrice.Add(NavPrice);
                        }
                        catch (Exception ex)
                        {
                            _alertStatus = "Error";
                            _alertMsg = stepCol + ex.Message;
                            return;
                        }
                    }
                }

                #endregion
            }

            data.asof_date = model.ImportDate.Value.ToString("yyyyMMdd");
            data.channel_id = "REPO";
            data.asof_time = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");
            data.total_rec = rowNav;
            data.ref_no = ref_no;
            data.NavPrice = listNavPrice;

            if (listNavPrice.Count == 0)
            {
                _alertStatus = "Error";
                _alertMsg = "No Data";
                return;
            }

            interfaceNavPriceModel.datas = data;

        
            #region req ws in .net core

            ResultWithModel<InterfaceReqNavPricResult> result = new ResultWithModel<InterfaceReqNavPricResult>();
            ExternalInterfaceEntities api_ext = new ExternalInterfaceEntities();
            api_ext.InterfaceNavPrice.ImportNavPrice(interfaceNavPriceModel, p =>
            {
                result = p;
            });

            if (result.Data != null)
            {
                if (result.Data.response_code == "0")
                {
                    _alertStatus = "Success";
                    _alertMsg = result.Data.response_message;
                }
                else
                {
                    _alertStatus = "Error";
                    _alertMsg = result.Data.response_message;
                }
            }

            #endregion

        }

        #endregion

        #region EOD BO Reconcile
        [HttpGet]
        public ActionResult EodBoReconcile()
        {
            ImportExportDataViewModel model = new ImportExportDataViewModel();
            ViewBag.AlertStatus = "";
            ViewBag.AlertMsg = "";
            return View(model);
        }

        //import
        [HttpPost]
        public ActionResult EodBoReconcile(ImportExportDataViewModel model)
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var fileContent = Request.Files[0];

                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(fileContent.FileName);
                        var extension = Path.GetExtension(fileContent.FileName);

                        //var contentType = fileContent.ContentType;
                        
                        fileName = fileName + DateTime.Now.ToString("_HHmmss") + extension;

                        //write file
                        var path = Path.Combine(GetDirectory(), fileName);
                        using (FileStream fileStream = new FileStream(path, FileMode.Create))
                        {
                            fileContent.InputStream.Seek(0, SeekOrigin.Begin);
                            fileContent.InputStream.CopyTo(fileStream);

                        }

                        //process import
                        if (model.ImportTo == "EOD_RECONCILE_PTI")
                        {
                            ImportToEodBoReconcilePTI(model, path, fileName);
                        }
                        else if (model.ImportTo == "EOD_RECONCILE_BAHTNET")
                        {
                            ImportToEodBoReconcileBahtnet(model, path, fileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _alertStatus = "Error";
                _alertMsg = ex.Message;
            }

            ViewBag.AlertStatus = _alertStatus;
            ViewBag.AlertMsg = _alertMsg;
            return View();
        }
        
        private void ImportToEodBoReconcilePTI(ImportExportDataViewModel model, string path, string filename)
        {
            InterfaceReqEodReconcilePtiModel modelPti = new InterfaceReqEodReconcilePtiModel();
            List<InterfaceReqEodReconcilePtiModel.ListData> listData = new ListStack<InterfaceReqEodReconcilePtiModel.ListData>();

            modelPti.asOfDate = model.ImportDate.Value;
            modelPti.filename = filename;
            modelPti.recordedBy = User.UserId;

            //string asof = model.ImportDate.Value.ToString("dd/MM/yyyy");

            #region CSV

            CultureInfo culture = new CultureInfo("en-GB");
            using (CsvReader csv = new CsvReader(new StreamReader(path), true))
            {
                string stepCol = "";
                while (csv.Read())
                {
                    stepCol = "";

                    try
                    {
                        ReadingContext context = csv.Context;

                        if (csv.Context.Row == 1) continue;//skip 1 row

                        //skip 23 col
                        if (context.Record.Count() < 23) continue;

                        if (string.IsNullOrEmpty(context.Record[12])) continue;

                        stepCol = "[Settle. Date]";
                        DateTime settleDate = System.Convert.ToDateTime(context.Record[12], culture);
                        if (DateTime.Compare(settleDate.Date, model.ImportDate.Value.Date) != 0)
                        {
                            throw new Exception("Date select not match settle date in file.");
                        }

                        InterfaceReqEodReconcilePtiModel.ListData data = new InterfaceReqEodReconcilePtiModel.ListData();
                        stepCol = "[Trans Date]";
                        data.trans_date = context.Record[0];
                        stepCol = "[Matched Id.]";
                        data.matched_id = context.Record[1];
                        stepCol = "[MT]";
                        data.mt = context.Record[2];
                        stepCol = "[Status]";
                        data.status = context.Record[3];
                        stepCol = "[CounterParty BIC]";
                        data.counter_party_bic = context.Record[4];
                        stepCol = "[Delv. Acct.]";
                        data.delv_acct = context.Record[5];
                        stepCol = "[Security Symbol]";
                        data.security_symbol = context.Record[6];
                        stepCol = "[ISIN Code]";
                        data.isin_code = context.Record[7];
                        stepCol = "[Face Amt.]";
                        data.face_amt = context.Record[8];
                        stepCol = "[Currency]";
                        data.currency = context.Record[9];
                        stepCol = "[Volume]";
                        data.volume = context.Record[10];
                        stepCol = "[Create Time]";
                        data.create_time = context.Record[11];
                        stepCol = "[Settle. Date]";
                        data.settle_date = context.Record[12];
                        stepCol = "[Sender's Ref.]";
                        data.sender_ref = context.Record[13];
                        stepCol = "[BT]";
                        data.bt = context.Record[14];
                        stepCol = "[Error]";
                        data.error = context.Record[15];
                        stepCol = "[Counter Sender's Ref]";
                        data.count_sender_ref = context.Record[16];
                        stepCol = "[Recv. Acct.]";
                        data.recv_acct = context.Record[17];
                        stepCol = "[Cash Acct.]";
                        data.cash_acct = context.Record[18];
                        stepCol = "[Settle. Amt.]";
                        data.settle_amt = context.Record[19];
                        stepCol = "[currency2]";
                        data.currency2 = context.Record[20];
                        stepCol = "[Channel]";
                        data.channel = context.Record[21];
                        stepCol = "[Time]";
                        data.time = context.Record[22];

                        listData.Add(data);
                    }
                    catch (Exception ex)
                    {
                        _alertStatus = "Error";
                        _alertMsg = stepCol + ex.Message;
                        return;
                    }
                }
            }

            #endregion

            if (listData.Count == 0)
            {
                _alertStatus = "Error";
                _alertMsg = "No Data";
                return;
            }

            modelPti.listData = listData;

            ResultWithModel<InterfaceResEodReconcileResult> result = new ResultWithModel<InterfaceResEodReconcileResult>();
            ExternalInterfaceEntities api_ext = new ExternalInterfaceEntities();
            api_ext.InterfaceEodReconcile.ImportPti(modelPti, p =>
            {
                result = p;
            });

            if (result.Success)
            {
                _alertStatus = "Success";
                _alertMsg = result.Message;
            }
            else
            {
                _alertStatus = "Error";
                _alertMsg = result.Message;
            }

        }

        private void ImportToEodBoReconcileBahtnet(ImportExportDataViewModel model, string path, string filename)
        {
            InterfaceReqEodReconcileBahtnetModel modelBahtnet = new InterfaceReqEodReconcileBahtnetModel();
            List<InterfaceReqEodReconcileBahtnetModel.ListData> listData = new ListStack<InterfaceReqEodReconcileBahtnetModel.ListData>();

            modelBahtnet.asOfDate = model.ImportDate.Value;
            modelBahtnet.filename = filename;
            modelBahtnet.recordedBy = User.UserId;

            //string asof = model.ImportDate.Value.ToString("dd/MM/yyyy");

            #region CSV
            CultureInfo culture = new CultureInfo("en-GB");
            using (CsvReader csv = new CsvReader(new StreamReader(path), true))
            {
                string stepCol = "";
                string valueDate = string.Empty;
                while (csv.Read())
                {
                    stepCol = "";

                    try
                    {
                        ReadingContext context = csv.Context;
                        
                        //skip 1-17 row
                        if (csv.Context.Row == 6)
                        {
                            valueDate = context.Record[1];
                            
                            //validate
                            stepCol = "[value Date]";
                            DateTime convertValueDate = System.Convert.ToDateTime(valueDate, culture);
                            if (DateTime.Compare(convertValueDate.Date, model.ImportDate.Value.Date) != 0)
                            {
                                throw new Exception("Date select not match value date in file.");
                            }
                            continue;
                        }
                        else if (csv.Context.Row < 17)
                        {
                            continue;
                        }

                        //skip 15 col
                        if (context.Record.Count() < 15) continue;

                        InterfaceReqEodReconcileBahtnetModel.ListData data = new InterfaceReqEodReconcileBahtnetModel.ListData();
                        stepCol = "[BN Trans ID]";
                        data.bn_trans_id = context.Record[0];
                        stepCol = "[Sender's Ref]";
                        data.sender_ref = context.Record[1];
                        stepCol = "[MT]";
                        data.mt = context.Record[2];
                        stepCol = "[BT]";
                        data.bt = context.Record[3];
                        stepCol = "[Dr BIC]";
                        data.dr_bic = context.Record[4];
                        stepCol = "[Dr Acc]";
                        data.dr_acc = context.Record[5];
                        stepCol = "[Cr BIC]";
                        data.cr_bic = context.Record[6];
                        stepCol = "[Cr Acc]";
                        data.cr_acc = context.Record[7];
                        stepCol = "[Dr Amt]";
                        data.dr_amt = context.Record[8];
                        stepCol = "[Cr Amt]";
                        data.cr_amt = context.Record[9];
                        stepCol = "[Status]";
                        data.status = context.Record[10];
                        stepCol = "[Error]";
                        data.error = context.Record[11];
                        stepCol = "[Time]";
                        data.time = context.Record[12];
                        stepCol = "[CH]";
                        data.ch = context.Record[13];
                        stepCol = "[Transmission Type]";
                        data.transmission_type = context.Record[14];
                        data.value_date = valueDate;

                        listData.Add(data);
                    }
                    catch (Exception ex)
                    {
                        _alertStatus = "Error";
                        _alertMsg = stepCol + ex.Message;
                        return;
                    }
                }
            }

            #endregion

            if (listData.Count == 0)
            {
                _alertStatus = "Error";
                _alertMsg = "No Data";
                return;
            }

            modelBahtnet.listData = listData;

            ResultWithModel<InterfaceResEodReconcileResult> result = new ResultWithModel<InterfaceResEodReconcileResult>();
            ExternalInterfaceEntities api_ext = new ExternalInterfaceEntities();
            api_ext.InterfaceEodReconcile.ImportBahtnet(modelBahtnet, p =>
            {
                result = p;
            });

            if (result.Success)
            {
                _alertStatus = "Success";
                _alertMsg = result.Message;
            }
            else
            {
                _alertStatus = "Error";
                _alertMsg = result.Message;
            }
        }

        #endregion

        #region Stock Reconcile
        [HttpGet]
        public ActionResult StockReconcile()
        {
            ImportExportDataViewModel model = new ImportExportDataViewModel();
            ViewBag.AlertStatus = "";
            ViewBag.AlertMsg = "";
            return View(model);
        }

        //import
        [HttpPost]
        public ActionResult StockReconcile(ImportExportDataViewModel model)
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var fileContent = Request.Files[0];

                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(fileContent.FileName);
                        var extension = Path.GetExtension(fileContent.FileName);

                        fileName = fileName + DateTime.Now.ToString("_yyyyMMdd_HHmmss") + extension;

                        //write file
                        var path = Path.Combine(GetDirectory(), fileName);
                        using (FileStream fileStream = new FileStream(path, FileMode.Create))
                        {
                            fileContent.InputStream.Seek(0, SeekOrigin.Begin);
                            fileContent.InputStream.CopyTo(fileStream);

                        }

                        //process import
                        ImportStockReconcile(model, path, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                _alertStatus = "Error";
                _alertMsg = ex.Message;
            }

            ViewBag.AlertStatus = _alertStatus;
            ViewBag.AlertMsg = _alertMsg;
            return View();
        }


        public ActionResult CheckRemark(string asofDate)
        {
            ResultWithModel<bool> result = new ResultWithModel<bool>();
            RPTransEntity api_trans = new RPTransEntity();
            api_trans.StockReconcile.CheckRemark(asofDate, p =>
            {
                result = p;
            });

            if (result.Success)
            {
                return Json(new { Status = "Success" , IsCheck = result.Data }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Status = "Error", IsCheck = result.Data, Message = result.Message }, JsonRequestBehavior.AllowGet);
        }

        private void ImportStockReconcile(ImportExportDataViewModel model, string path, string filename)
        {
            List<StockReconcileImportModel> listImportModel = new List<StockReconcileImportModel>();
            string guid = Guid.NewGuid().ToString();

            #region 305

            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    string stepCol = "";
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        try
                        {
                            string[] splitData = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                            if (splitData.Length < 3)
                            {
                                throw new Exception("Format incorrect in file.");
                            }

                            StockReconcileImportModel importModel = new StockReconcileImportModel();
                            stepCol = "[participant]";
                            importModel.participant = splitData[0].Substring(0, 3);
                            stepCol = "[accno]";
                            importModel.accno = splitData[0].Substring(3, 10);
                            if (!(string.Equals(importModel.accno, "0000000117") || string.Equals(importModel.accno, "0000000122")))
                            {
                                continue;
                            }

                            stepCol = "[instrumentCode]";
                            importModel.instrumentCode = splitData[0].Substring(13, splitData[0].Length - 13);
                            stepCol = "[isincode]";
                            importModel.isincode = splitData[1];
                            stepCol = "[unit]";
                            importModel.unit = decimal.Parse(splitData[2]);
                            stepCol = "[pending_withdrawal]";
                            importModel.pending_withdrawal = decimal.Parse(splitData[3]);
                            stepCol = "[pending_deposit]";
                            importModel.pending_deposit = decimal.Parse(splitData[4]);
                            stepCol = "[pending_sec]";
                            importModel.pending_sec = decimal.Parse(splitData[5]);
                            stepCol = "[broker]";
                            if (splitData[6] != null)
                            {
                                importModel.broker = splitData[6];
                            }
                            
                            importModel.asOfDate = model.ImportDate.Value;
                            importModel.filename = filename;
                            importModel.import_id = guid;
                            importModel.create_by = User.UserId;
                            listImportModel.Add(importModel);
                        }
                        catch (Exception ex)
                        {
                            _alertStatus = "Error";
                            _alertMsg = stepCol + ex.Message;
                            return;
                        }
                    }
                }
            }

            #endregion

            if (listImportModel.Count == 0)
            {
                _alertStatus = "Error";
                _alertMsg = "No Data";
                return;
            }


            ResultWithModel<StockReconcileResult> result = new ResultWithModel<StockReconcileResult>();
            RPTransEntity api_trans = new RPTransEntity();
            api_trans.StockReconcile.Import(listImportModel, p =>
            {
                result = p;
            });

            if (result.Success)
            {
                _alertStatus = "Success";
                _alertMsg = "Import Success";
            }
            else
            {
                _alertStatus = "Error";
                _alertMsg = result.Message;
            }
        }

        #endregion

        #region EquitySymbol
        [HttpGet]
        public ActionResult EquitySymbol()
        {
            ImportExportDataViewModel model = new ImportExportDataViewModel();
            ViewBag.AlertStatus = "";
            ViewBag.AlertMsg = "";
            return View(model);
        }

        public ActionResult EquitySymbol(ImportExportDataViewModel model)
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var fileContent = Request.Files[0];

                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(fileContent.FileName);
                        var extension = Path.GetExtension(fileContent.FileName);
                        //var contentType = fileContent.ContentType;

                        //validate
                        if (model.ImportTo == "EquitySymbol")
                        {
                            var validateFilename = fileName != null && fileName.Contains("Equity_Def_");
                            if (!validateFilename)
                            {
                                throw new Exception("incorrect file name (ex.Equity_Def_yyyymmdd)");
                            }

                            string importDate = model.ImportDate.ToString();
                            string validateImportDate = System.Convert.ToDateTime(importDate).ToString("yyyyMMdd");
                            if (!fileName.Contains(validateImportDate))
                            {
                                throw new Exception("incorrect file name not match import date");
                            }
                        }

                        fileName = fileName + DateTime.Now.ToString("_HHmmss") + extension;

                        //write file
                        var path = Path.Combine(GetDirectory(), fileName);
                        using (FileStream fileStream = new FileStream(path, FileMode.Create))
                        {
                            fileContent.InputStream.Seek(0, SeekOrigin.Begin);
                            fileContent.InputStream.CopyTo(fileStream);

                        }

                        //process import
                        ImportToEquitySymbol(model, extension, path);
                    }
                }
            }
            catch (Exception ex)
            {
                _alertStatus = "Error";
                _alertMsg = ex.Message;
            }

            ViewBag.AlertStatus = _alertStatus;
            ViewBag.AlertMsg = _alertMsg;
            return View(model);
        }

        private void ImportToEquitySymbol(ImportExportDataViewModel model, string extension, string path)
        {
            InterfaceEquitySymbolModel interfaceEquitySymbolModel = new InterfaceEquitySymbolModel();
            interfaceEquitySymbolModel.success = true;
            interfaceEquitySymbolModel.message = "Run Successfully";
            interfaceEquitySymbolModel.serverity = "";
            interfaceEquitySymbolModel.rdfcode = 0;
            interfaceEquitySymbolModel.total = 1;

            string asof = model.ImportDate.Value.ToString("dd/MM/yyyy");
            string ref_no = Guid.NewGuid().ToString().ToUpper();

            int rowEq = 0;
            int readRow = 0;

            InterfaceEquitySymbolModel.Datas data = new InterfaceEquitySymbolModel.Datas();
            List<InterfaceEquitySymbolModel.EquitySymbol> listEquitySymbol = new ListStack<InterfaceEquitySymbolModel.EquitySymbol>();

            if (extension == ".xlsx" || extension == ".xls")//xlsx
            {
                #region Excel
                DataTable dt = ExcelReader.GetDataTableFromExcel(path);
                if (dt.Rows.Count > 0)
                {
                    string bondType = "";
                    string stepCol = "";
                    CultureInfo culture = new CultureInfo("en-GB");
        
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        stepCol = "";

                        try
                        {
                            if (rowEq == 0)
                            {
                                readRow++;
                                rowEq++;
                                continue;
                            }

                            InterfaceEquitySymbolModel.EquitySymbol eq = new InterfaceEquitySymbolModel.EquitySymbol();
                            readRow++;
                            rowEq++;

                            stepCol = "[ref_no]";
                            eq.ref_no = ref_no;
                            stepCol = "[asof_date]";
                            eq.asof_date = model.ImportDate.Value.ToString("yyyyMMdd");
                            stepCol = "[instrument_id]";
                            eq.instrument_id = dataRow[0].ToString().Trim();
                            stepCol = "[instrument_code]";
                            eq.instrument_code = dataRow[1].ToString().Trim();
                            stepCol = "[instrument_desc]";
                            eq.instrument_desc = dataRow[2].ToString().Trim();
                            stepCol = "[market_id]";
                            eq.market_id = dataRow[3].ToString().Trim();
                            stepCol = "[product_code]";
                            eq.product_code = dataRow[4].ToString().Trim();
                            stepCol = "[instrumenttype]";
                            eq.instrumenttype = dataRow[5].ToString().Trim();
                            stepCol = "[sub_product_code]";
                            eq.sub_product_code = dataRow[6].ToString().Trim();
                            stepCol = "[sub_producsecond_instrumenttypet_code]";
                            eq.second_instrumenttype = dataRow[7].ToString().Trim();
                            stepCol = "[issue_date]";
                            eq.issue_date = dataRow[8].ToString().Trim();
                            stepCol = "[maturity_date]";
                            eq.maturity_date = dataRow[9].ToString().Trim();
                            stepCol = "[issuer_id]";
                            eq.issuer_id = dataRow[10].ToString().Trim();
                            stepCol = "[register_id]";
                            eq.register_id = dataRow[11].ToString().Trim();
                            stepCol = "[cur]";
                            eq.cur = dataRow[12].ToString().Trim();
                            stepCol = "[begining_par]";
                            eq.begining_par = dataRow[13].ToString().Trim();
                            stepCol = "[ending_par]";
                            eq.ending_par = dataRow[14].ToString().Trim();
                            stepCol = "[min_unit]";
                            eq.min_unit = dataRow[15].ToString().Trim();
                            stepCol = "[max_unit]";
                            eq.max_unit = dataRow[16].ToString().Trim();
                            stepCol = "[incremental_unit]";
                            eq.incremental_unit = dataRow[17].ToString().Trim();
                            stepCol = "[spread]";
                            eq.spread = dataRow[18].ToString().Trim();
                            stepCol = "[ISIN_code_TH]";
                            eq.ISIN_code_TH = dataRow[19].ToString().Trim();
                            stepCol = "[ISIN_CODE_FR]";
                            eq.ISIN_CODE_FR = dataRow[20].ToString().Trim();
                            stepCol = "[ISIN_CODE_NVDR]";
                            eq.ISIN_CODE_NVDR = dataRow[21].ToString().Trim();
                            stepCol = "[listed_flag]";
                            eq.listed_flag = dataRow[22].ToString().Trim();
                            stepCol = "[active_flag]";
                            eq.active_flag = dataRow[23].ToString().Trim();
                            stepCol = "[create_date]";
                            eq.create_date = dataRow[24].ToString().Trim();
                            stepCol = "[create_by]";
                            eq.create_by = dataRow[25].ToString().Trim();
                            stepCol = "[update_date]";
                            eq.update_date = dataRow[26].ToString().Trim();
                            stepCol = "[update_by]";
                            eq.update_by = dataRow[27].ToString().Trim();
                            stepCol = "[address]";
                            eq.address = dataRow[28].ToString().Trim();
                            stepCol = "[tel_no]";
                            eq.tel_no = dataRow[29].ToString().Trim();
                            stepCol = "[fax_no]";
                            eq.fax_no = dataRow[30].ToString().Trim();
                            stepCol = "[web_site]";
                            eq.web_site = dataRow[31].ToString().Trim();
                            stepCol = "[authorized_share_capital]";
                            eq.authorized_share_capital = dataRow[32].ToString().Trim();
                            stepCol = "[paid_up_capital]";
                            eq.paid_up_capital = dataRow[33].ToString().Trim();
                            stepCol = "[authorized_share]";
                            eq.authorized_share = dataRow[34].ToString().Trim();
                            stepCol = "[paid_up_share]";
                            eq.paid_up_share = dataRow[35].ToString().Trim();
                            stepCol = "[remark]";
                            eq.remark = dataRow[36].ToString().Trim();
                            stepCol = "[industry_group_id]";
                            eq.industry_group_id = dataRow[37].ToString().Trim();
                            stepCol = "[industry_sector_id]";
                            eq.industry_sector_id = dataRow[38].ToString().Trim();
                            stepCol = "[ktb_flag]";
                            eq.ktb_flag = dataRow[39].ToString().Trim();
                            stepCol = "[ktb_holding_unit]";
                            eq.ktb_holding_unit = dataRow[40].ToString().Trim();
                            stepCol = "[ktb_holding_percent]";
                            eq.ktb_holding_percent = dataRow[41].ToString().Trim();
                            stepCol = "[ktb_holding_amount]";
                            eq.ktb_holding_amount = dataRow[42].ToString().Trim();
                            stepCol = "[domestic_flag]";
                            eq.domestic_flag = dataRow[43].ToString().Trim();
                            stepCol = "[p_e]";
                            eq.p_e = dataRow[44].ToString().Trim();
                            stepCol = "[p_pv]";
                            eq.p_pv = dataRow[45].ToString().Trim();
                            stepCol = "[market_cap]";
                            eq.market_cap = dataRow[46].ToString().Trim();
                            stepCol = "[dividen_policy]";
                            eq.dividen_policy = dataRow[47].ToString().Trim();
                            stepCol = "[listed_share]";
                            eq.listed_share = dataRow[48].ToString().Trim();
                            stepCol = "[dividend_yeild]";
                            eq.dividend_yeild = dataRow[49].ToString().Trim();
                            stepCol = "[p_nav]";
                            eq.p_nav = dataRow[50].ToString();
                            stepCol = "[redemption_method_id]";
                            eq.redemption_method_id = dataRow[51].ToString().Trim();
                            stepCol = "[redemption_value]";
                            eq.redemption_value = dataRow[52].ToString().Trim();
                            stepCol = "[redemption_percent]";
                            eq.redemption_percent = dataRow[53].ToString().Trim();
                            stepCol = "[InvestmentGroup]";
                            eq.InvestmentGroup = dataRow[54].ToString().Trim();
                            stepCol = "[cfo_garantee_flag]";
                            eq.cfo_garantee_flag = dataRow[55].ToString().Trim();
                            stepCol = "[set50_flag]";
                            eq.set50_flag = dataRow[56].ToString().Trim();
                            stepCol = "[set100_flag]";
                            eq.set100_flag = dataRow[57].ToString().Trim();
                            stepCol = "[devalue]";
                            eq.devalue = dataRow[58].ToString().Trim();
                            stepCol = "[bloomberg_code]";
                            eq.bloomberg_code = dataRow[59].ToString().Trim();
                            stepCol = "[risk_weight]";
                            eq.risk_weight = dataRow[60].ToString().Trim();
                            stepCol = "[rwa_code]";
                            eq.rwa_code = dataRow[61].ToString().Trim();
                            stepCol = "[new_instrument_code]";
                            eq.new_instrument_code = dataRow[62].ToString().Trim();
                            stepCol = "[parent_id]";
                            eq.parent_id = dataRow[63].ToString().Trim();
                            stepCol = "[custodian_id]";
                            eq.custodian_id = dataRow[64].ToString().Trim();
                            stepCol = "[regista_id]";
                            eq.regista_id = dataRow[65].ToString().Trim();
                            stepCol = "[underlying]";
                            eq.underlying = dataRow[66].ToString().Trim();
                            stepCol = "[exercise_price]";
                            eq.exercise_price = dataRow[67].ToString().Trim();
                            stepCol = "[Company_Investment_Type_Code]";
                            eq.Company_Investment_Type_Code = dataRow[68].ToString().Trim();
                            stepCol = "[trade_dt]";
                            eq.trade_dt = dataRow[69].ToString().Trim();
                            stepCol = "[settlement_dt]";
                            eq.settlement_dt = dataRow[70].ToString().Trim();
                            stepCol = "[fo_verify_flag]";
                            eq.fo_verify_flag = dataRow[71].ToString().Trim();
                            stepCol = "[bo_verify_flag]";
                            eq.bo_verify_flag = dataRow[72].ToString().Trim();
                            stepCol = "[trader_costcenter]";
                            eq.trader_costcenter = dataRow[73].ToString().Trim();
                            stepCol = "[ifrs_instrument_type]";
                            eq.ifrs_instrument_type = dataRow[74].ToString().Trim();
                            stepCol = "[ifrs_measurement_type]";
                            eq.ifrs_measurement_type = dataRow[75].ToString().Trim();
                            stepCol = "[ifrs_readonly_flag]";
                            eq.ifrs_readonly_flag = dataRow[76].ToString().Trim();
                            stepCol = "[xe_old_share]";
                            eq.xe_old_share = dataRow[77].ToString().Trim();
                            stepCol = "[xe_new_share]";
                            eq.xe_new_share = dataRow[78].ToString().Trim();
                            stepCol = "[tax_type_id]";
                            eq.tax_type_id = dataRow[79].ToString().Trim();
                            stepCol = "[Market_Price_Multiplie]";
                            eq.Market_Price_Multiplier = dataRow[80].ToString().Trim();
                            stepCol = "[GL_Backdate_Flag]";
                            eq.GL_Backdate_Flag = dataRow[81].ToString().Trim();
                            stepCol = "[SP_Flag]";
                            eq.SP_Flag = dataRow[82].ToString().Trim();
                            stepCol = "[limit_percent]";
                            eq.limit_percent = dataRow[83].ToString().Trim();
                            stepCol = "[import_by]";
                            eq.import_by = HttpContext.User.Identity.Name;

                            listEquitySymbol.Add(eq);

                        }
                        catch (Exception ex)
                        {
                            _alertStatus = "Error";
                            _alertMsg = stepCol + ex.Message;
                            return;
                        }
                    }
                }
                #endregion
            }

            data.asof_date = model.ImportDate.Value.ToString("yyyyMMdd");
            data.channel_id = "REPO";
            data.asof_time = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");
            data.total_rec = rowEq;
            data.ref_no = ref_no;
            data.EquitySymbol = listEquitySymbol;
            data.create_by = HttpContext.User.Identity.Name;

            if (listEquitySymbol.Count == 0)
            {
                _alertStatus = "Error";
                _alertMsg = "No Data";
                return;
            }

            interfaceEquitySymbolModel.datas = data;
            ResultWithModel<InterfaceEquitySymbolResult> result = new ResultWithModel<InterfaceEquitySymbolResult>();
            ExternalInterfaceEntities api_ext = new ExternalInterfaceEntities();
            api_ext.InterfaceEquitySymbol.ImportEquitSymbol(interfaceEquitySymbolModel, p =>
            {
                result = p;
            });

            if (result.Data != null)
            {
                if (result.Data.response_code == "0")
                {
                    _alertStatus = "Success";
                    _alertMsg = result.Data.response_message;
                }
                else
                {
                    _alertStatus = "Error";
                    _alertMsg = result.Data.response_message;
                }
            }

        }

        #endregion

        private string GetDirectory()
        {
            string path = Server.MapPath("~/UploadFiles") + "/" + _folderUpload + "/" + _dateStr;

            if (Directory.Exists(path))
            {
                return path;
            }
            // Try to create the directory.
            Directory.CreateDirectory(path);
            return path;
        }
    }
}