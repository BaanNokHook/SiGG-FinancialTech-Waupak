using GM.Application.Web.Models;
using GM.CommonLibs;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.ExternalInterface;
using GM.Data.Model.ExternalInterface.InterfaceThorIndex;
using GM.Data.Model.Static;
using GM.Data.Result.ExternalInterface;
using GM.Data.Result.Static;
using GM.Filters;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
      [Authorize]
      [Audit]
      public class AdminController : BaseController
      {
            Utility utility = new Utility();  
            private static Console_Entity ConsoleEnt = new Console_Entity();   
            private StaticEntities StaticEnt = new StaticEntities();  
            private ExternalInterfaceEntities InterfaceEnt = new ExternalInterfaceEntities();   
            private static string Controller = "AdminController";  
            private static LogFile Log = new logFile();    

            private class Console_Entity  
            {
                  private int _page_number = 1;  
                  private int _record_per_page = 999999;   
                  private string _create_by = "Web";  

                  public string CreateBy  
                  {
                        get { return _create_by; }   
                        set { -create_by = value; }  
                  }  

                  public int PageNumber  
                  {
                        get { return _page_number; }  
                        set { _page_number = value; }  
                  }   

                  public int RecordPerPage  
                  {
                        get { return _record_per_page; }    
                        set { _record_per_page = value; }   
                  }   
            }  

            public AdminController()  
            {
                  StaticEnt.SetTimeOut(30);   
                  InterfaceEnt.SetTimeOut(30);   
            }   

            public ActionResult Dashboard()  
            {
                  if (!IsCheckPermission())   
                  {
                        return RedirectToAction("Index", "Home");  
                  }  

                  return View();  
            }  

            public ActionResult WizardQuery()  
            {
                  if (IsCheckPermission() && User.WizardPage == "Y")   
                  {
                        return View();   
                  }

                  return RedirectToAction("Index", "Home");     
            }   

            public ActionResult TestFile()
            {
                  if (!IsCheckPermission())  
                  {
                        return RedirectToAction("Index", "home");   
                  }  

                  return View();   
            }  

            #region Function : Export DWH To SFTP  

            public void ExportDWH_FCY_ToSFTP(ref string strMsg, ref int countFail, ref int countSuccess, ref int countTotal, AdminViewModel model)    
            {
                  //Step 1 : Get Config    
                  ResultWithModel<RpConfigResult> resultRpConfig = new ResultWithModel<RpConfigResult>();   
                  List<RpConfigModel> rpConfigModel = new List<RpConfigModel>();   

                  StaticEnt.Rp.GetRpConfig("RP_DWH_SFTP", string.Empty, p => 
                  {
                        resultRpconfig = p;   
                  });   

                  if (!resultRpConfig.Success)  
                  {
                        throw new Exception("GetRpConfig() => [" + resultRPConfig.RefCode.ToString() + "] " + resultRPConfig.message);  
                  }  

                  rpConfigModel = resultRpConfig.Data.RpConfigResultModel;  
                  if (!Set_ConfigDWHIAS39ToSFTP(ref strMsg, rpConfigModel))   
                  {
                        throw new Excrption("Set_ConfigDWHIAS39ToSFTP() => " + strMsg);
                  }

                  Log.WriteLog(Controller, "Get Config Success");

                  //Step 2 : Search DWH List  
                  ResultWithModel<InterfaceDwhSftpResult> resultDwhSftp = new ResultWithModel<InterfaceBBGMarketPrice>  
                  resultDwhSftp = new ResultWithModel<InterfaceDwhSftpResult>();
                  InterfaceDwhSftpModel interfaceDwhSftpModel = new InterfaceDwhSftpModel();   

                  //Add Paging
                  PagingModel paging = new PagingModel();  
                  paging.PageNumber = ConsoleEnt.PageNumber;  
                  paging.RecordPerPage = ConsoleEnt.RecordPerPage;   
                  interfaceDwhSftpModel.paging = paging;   

                  //Add orderby  
                  var orders = new List<OrderByModel>();  
                  interfaceDwhSftpModel.ordersby = orders;  
                  //Add condition  
                  interfaceDwhSftpModel.type = model.type;  
                  interfaceDwhSftpModel.cur_type = "FCY";  
                  interfaceDwhSftpModel.enable_flag = true;  

                  InterfaceEnt.InterfaceDwh.GetInterfaceDwhSftpList(interfaceDwhSftpModel, p => 
            {
                  resultDwhSftp = p;  
            });   

            if (!resultDwhSftp.Success)  
                  
            {
                throw new Exception("GetInterfaceDwhSftpList() => [" + resultDwhSftp.RefCode.ToString() + "] " + resultDwhSftp.Message);
            }

            List<ResultWithModel<InterfaceDwhSftpResult>> List_Result = new List<ResultWithModel<InterfaceDwhSftpResult>>();
            List<InterfaceDwhSftpModel> DwhList = new List<InterfaceDwhSftpModel>();
            DwhList = resultDwhSftp.Data.InterfaceDwhSftpResultModel;

            //Step 3 : Gen DWH List
            Log.WriteLog(Controller, "Get DwhList Success");
            Log.WriteLog(Controller, "DwhList = [" + DwhList.Count + "] List");
            Log.WriteLog(Controller, "");
            List<Task> taskList = new List<Task>();
            //for (int i = 0; i < DwhList.Count; i++)
            foreach (var Row in DwhList)
            {
                Task lastTask = new Task(() =>
                {
                    resultDwhSftp = new ResultWithModel<InterfaceDwhSftpResult>();
                    Row.create_by = ConsoleEnt.CreateBy;
                    Row.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(model.asof_date);
                    Row.RpConfigModel = rpConfigModel;

                    InterfaceEnt.InterfaceDwh.ExportInterfaceDwh(Row, p =>
                    {
                        resultDwhSftp = p;
                    });

                    List_Result.Add(resultDwhSftp);
                });

                lastTask.Start();
                taskList.Add(lastTask);
            }

            Task.WaitAll(taskList.ToArray());

            //Step 4 : Check Result
            countTotal = List_Result.Count;
            foreach (var Row in List_Result)
            {
                Log.WriteLog(Controller, "Name = " + Row.Data.InterfaceDwhSftpResultModel[0].dwh_name);
                Log.WriteLog(Controller, "File = " + Row.Data.InterfaceDwhSftpResultModel[0].file_title);
                Log.WriteLog(Controller, "ReturnCode = [" + Row.RefCode + "] " + Row.Message);
                Log.WriteLog(Controller, "");
                if (!Row.Success)
                {
                    countFail += 1;
                }
                else
                {
                    countSuccess += 1;
                }
            }

            strMsg = "Total = [" + countTotal.ToString() + "] File. ";
            strMsg += "Success = [" + countSuccess.ToString() + " ] File. ";

            if (countFail > 0)
            {
                strMsg += "Fail = [" + countFail.ToString() + " ] File.";
                throw new Exception(strMsg);
            }
        }

        public void ExportDWH_THB_ToSFTP(ref string strMsg, ref int countFail, ref int countSuccess, ref int countTotal, AdminViewModel model)
        {
            //Step 1 : Get Config
            ResultWithModel<RpConfigResult> resultRpconfig = new ResultWithModel<RpConfigResult>();
            List<RpConfigModel> rpConfigModel = new List<RpConfigModel>();

            StaticEnt.RpConfig.GetRpConfig("RP_DWH_SFTP", string.Empty, p =>
            {
                resultRpconfig = p;
            });

            if (!resultRpconfig.Success)
            {
                throw new Exception("GetRpConfig() => [" + resultRpconfig.RefCode.ToString() + "] " + resultRpconfig.Message);
            }

            rpConfigModel = resultRpconfig.Data.RpConfigResultModel;
            if (!Set_ConfigDWHIAS39ToSFTP(ref strMsg, rpConfigModel))
            {
                throw new Exception("Set_ConfigDWH_THB_ToSFTP() => " + strMsg);
            }

            Log.WriteLog(Controller, "Get Config Success");

            //Step 2 : Search DWH List
            ResultWithModel<InterfaceDwhSftpResult> resultDwhSftp = new ResultWithModel<InterfaceDwhSftpResult>();
            resultDwhSftp = new ResultWithModel<InterfaceDwhSftpResult>();
            InterfaceDwhSftpModel interfaceDwhSftpModel = new InterfaceDwhSftpModel();

            //Add Paging
            PagingModel paging = new PagingModel();
            paging.PageNumber = ConsoleEnt.PageNumber;
            paging.RecordPerPage = ConsoleEnt.RecordPerPage;
            interfaceDwhSftpModel.paging = paging;

            //Add Orderby
            var orders = new List<OrderByModel>();
            interfaceDwhSftpModel.ordersby = orders;
            // Add condition
            interfaceDwhSftpModel.type = model.type;
            interfaceDwhSftpModel.cur_type = "THB";
            interfaceDwhSftpModel.enable_flag = true;

            InterfaceEnt.InterfaceDwh.GetInterfaceDwhSftpList(interfaceDwhSftpModel, p =>
            {
                resultDwhSftp = p;
            });

            if (!resultDwhSftp.Success)
            {
                throw new Exception("GetInterfaceDwhSftpList() => [" + resultDwhSftp.RefCode.ToString() + "] " + resultDwhSftp.Message);
            }

            List<ResultWithModel<InterfaceDwhSftpResult>> List_Result = new List<ResultWithModel<InterfaceDwhSftpResult>>();
            List<InterfaceDwhSftpModel> DwhList = new List<InterfaceDwhSftpModel>();
            DwhList = resultDwhSftp.Data.InterfaceDwhSftpResultModel;

            //Step 3 : Gen DWH List
            Log.WriteLog(Controller, "Get DwhList Success");
            Log.WriteLog(Controller, "DwhList = [" + DwhList.Count + "] List");
            Log.WriteLog(Controller, "");
            List<Task> taskList = new List<Task>();
            //for (int i = 0; i < DwhList.Count; i++)
            foreach (var Row in DwhList)
            {
                Task lastTask = new Task(() =>
                {
                    resultDwhSftp = new ResultWithModel<InterfaceDwhSftpResult>();
                    Row.create_by = ConsoleEnt.CreateBy;
                    Row.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(model.asof_date);
                    Row.RpConfigModel = rpConfigModel;

                    InterfaceEnt.InterfaceDwh.ExportInterfaceDwh(Row, p =>
                    {
                        resultDwhSftp = p;
                    });

                    List_Result.Add(resultDwhSftp);
                });

                lastTask.Start();
                taskList.Add(lastTask);
            }

            Task.WaitAll(taskList.ToArray());

            //Step 4 : Check Result
            countTotal = List_Result.Count;
            foreach (var Row in List_Result)
            {
                Log.WriteLog(Controller, "Name = " + Row.Data.InterfaceDwhSftpResultModel[0].dwh_name);
                Log.WriteLog(Controller, "File = " + Row.Data.InterfaceDwhSftpResultModel[0].file_title);
                Log.WriteLog(Controller, "ReturnCode = [" + Row.RefCode + "] " + Row.Message);
                Log.WriteLog(Controller, "");
                if (!Row.Success)
                {
                    countFail += 1;
                }
                else
                {
                    countSuccess += 1;
                }
            }

            strMsg = "Total = [" + countTotal.ToString() + "] File. ";
            strMsg += "Success = [" + countSuccess.ToString() + " ] File. ";

            if (countFail > 0)
            {
                strMsg += "Fail = [" + countFail.ToString() + " ] File.";
                throw new Exception(strMsg);
            }
        }

        private static bool Set_ConfigDWHIAS39ToSFTP(ref string ReturnMsg, List<RpConfigModel> List_RpConfigModel)
        {
            try
            {
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "SFTP IP = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "IP")?.item_value);
                Log.WriteLog(Controller, "SFTP PORT = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PORT")?.item_value);
                Log.WriteLog(Controller, "SFTP USER = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "USER")?.item_value);
                Log.WriteLog(Controller, "SFTP PATH_SFTP = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SFTP")?.item_value);
                Log.WriteLog(Controller, "SFTP PATH_SERVICE = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SERVICE")?.item_value);
                Log.WriteLog(Controller, "SFTP SSHHOSTKEYFINGERPRINT = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "SSHHOSTKEYFINGERPRINT")?.item_value);
                Log.WriteLog(Controller, "SFTP SSHPRIVATEKEYPATH = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "SSHPRIVATEKEYPATH")?.item_value);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        [HttpPost]
        public JsonResult ExportDWH_FCY_DailyToSFTP(AdminViewModel model)
        {
            string strMsg = string.Empty;
            int countFail = 0;
            int countSuccess = 0;
            int countTotal = 0;
            model.type = "Daily";
            try
            {
                UpdateCheckingEod("ExportDWH_FCY_DailyToSFTP");

                Log.WriteLog(Controller, "Start ExportDWH_FCY_DailyToSFTP ==========");

                ExportDWH_FCY_ToSFTP(ref strMsg, ref countFail, ref countSuccess, ref countTotal, model);

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "ExportDWH_FCY_DailyToSFTP() " +
                                  model.asof_date +
                                  " => Fail : " + ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End ExportDWH_FCY_DailyToSFTP ==========");
            }

            return Json(new { Status = "Success", Message = "ExportDWH_FCY_DailyToSFTP() " + model.asof_date + " : " + strMsg });
        }

        [HttpPost]
        public JsonResult ExportDWH_FCY_MonthlyToSFTP(AdminViewModel model)
        {
            string strMsg = string.Empty;
            int countFail = 0;
            int countSuccess = 0;
            int countTotal = 0;
            model.type = "Monthly";
            try
            {
                UpdateCheckingEod("ExportDWH_FCY_MonthlyToSFTP");

                Log.WriteLog(Controller, "Start ExportDWH_FCY_MonthlyToSFTP ==========");

                ExportDWH_FCY_ToSFTP(ref strMsg, ref countFail, ref countSuccess, ref countTotal, model);

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "ExportDWH_FCY_MonthlyToSFTP() " +
                                  model.asof_date +
                                  " => Fail : " + ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End ExportDWH_FCY_MonthlyToSFTP ==========");
            }

            return Json(new { Status = "Success", Message = "ExportDWH_FCY_MonthlyToSFTP() " + model.asof_date + " : " + strMsg });
        }

        [HttpPost]
        public JsonResult ExportDWH_THB_DailyToSFTP(AdminViewModel model)
        {
            string strMsg = string.Empty;
            int countFail = 0;
            int countSuccess = 0;
            int countTotal = 0;
            model.type = "Daily";
            try
            {
                UpdateCheckingEod("ExportDWH_THB_DailyToSFTP");

                Log.WriteLog(Controller, "Start ExportDWH_THB_DailyToSFTP ==========");

                ExportDWH_THB_ToSFTP(ref strMsg, ref countFail, ref countSuccess, ref countTotal, model);

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "ExportDWH_THB_DailyToSFTP() " +
                                  model.asof_date +
                                  " => Fail : " + ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End ExportDWH_THB_DailyToSFTP ==========");
            }

            return Json(new { Status = "Success", Message = "ExportDWH_THB_DailyToSFTP() " + model.asof_date + " : " + strMsg });
        }

        [HttpPost]
        public JsonResult ExportDWH_THB_MonthlyToSFTP(AdminViewModel model)
        {
            string strMsg = string.Empty;
            int countFail = 0;
            int countSuccess = 0;
            int countTotal = 0;
            model.type = "Monthly";
            try
            {
                UpdateCheckingEod("ExportDWH_THB_MonthlyToSFTP");

                Log.WriteLog(Controller, "Start ExportDWH_THB_MonthlyToSFTP ==========");

                ExportDWH_THB_ToSFTP(ref strMsg, ref countFail, ref countSuccess, ref countTotal, model);

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "ExportDWH_THB_MonthlyToSFTP() " +
                                  model.asof_date +
                                  " => Fail : " + ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End ExportDWH_THB_MonthlyToSFTP ==========");
            }

            return Json(new { Status = "Success", Message = "ExportDWH_THB_MonthlyToSFTP() " + model.asof_date + " : " + strMsg });
        }

        #endregion

        #region Function : Export DMS Daily To SFTP

        [HttpPost]
        public JsonResult ExportDMSDataSetToSFTP(AdminViewModel Model)
        {
            string StrMsg = string.Empty;
            int CountFail = 0;
            int CountSuccess = 0;
            int CountTotal;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("ExportDMSDataSetToSFTP");

                Log.WriteLog(Controller, "Start ExportDMSDataSetToSFTP ==========");

                //Step 1 : Get Config
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_DMS_DATASET_SFTP", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
                }

                //Step 2 : Set Config
                List<InterfaceDmsSftpModel> DmsList = new List<InterfaceDmsSftpModel>();
                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                if (!Set_ConfigDMSDataSetToSFTP(ref StrMsg, ref DmsList, RpConfigModel, Model))
                {
                    throw new Exception("Set_ConfigDMSDataSetToSFTP() => " + StrMsg);
                }

                Log.WriteLog(Controller, "Get Config Success");

                ResultWithModel<InterfaceDmsSftpResult> ResultDmsSftp = new ResultWithModel<InterfaceDmsSftpResult>();
                List<ResultWithModel<InterfaceDmsSftpResult>> List_Result = new List<ResultWithModel<InterfaceDmsSftpResult>>();

                //Step 3 : Gen DMS DataSet List
                List<Task> taskList = new List<Task>();
                foreach (var Row in DmsList)
                {
                    Log.WriteLog(Controller, "Gen = " + Row.dms_name);
                    Task lastTask = new Task(() =>
                    {
                        ResultDmsSftp = new ResultWithModel<InterfaceDmsSftpResult>();
                        Row.create_by = ConsoleEnt.CreateBy;
                        Row.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date);
                        Row.RpConfigModel = RpConfigModel;

                        InterfaceEnt.InterfaceDms.ExportInterfaceDms(Row, p =>
                        {
                            ResultDmsSftp = p;
                        });

                        List_Result.Add(ResultDmsSftp);
                    });

                    lastTask.Start();
                    taskList.Add(lastTask);
                }

                Task.WaitAll(taskList.ToArray());

                //Step 4 : Check Result
                CountTotal = List_Result.Count;
                foreach (var Row in List_Result)
                {
                    Log.WriteLog(Controller, "Name = " + Row.Data.InterfaceDmsSftpResultModel[0].dms_name);
                    Log.WriteLog(Controller, "File = " + Row.Data.InterfaceDmsSftpResultModel[0].file_name);
                    Log.WriteLog(Controller, "ReturnCode = [" + Row.RefCode + "] " + Row.Message);
                    Log.WriteLog(Controller, "");
                    if (!Row.Success)
                    {
                        CountFail += 1;
                    }
                    else
                    {
                        CountSuccess += 1;
                    }
                }

                StrMsg = "Total = [" + CountTotal.ToString() + "] File. ";
                StrMsg += "Success = [" + CountSuccess.ToString() + " ] File. ";

                if (CountFail > 0)
                {
                    StrMsg += "Fail = [" + CountFail.ToString() + " ] File.";
                    throw new Exception(StrMsg);
                }
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = " ExportDMSDataSetToSFTP() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End ExportDMSDataSetToSFTP ==========");
            }

            return Json(new { Status = "Success", Message = "ExportDMSDataSetToSFTP() " + Model.asof_date + " : " + StrMsg });
        }
        private static bool Set_ConfigDMSDataSetToSFTP(ref string ReturnMsg, ref List<InterfaceDmsSftpModel> DmsList, List<RpConfigModel> List_RpConfigModel, AdminViewModel Model)
        {
            try
            {
                Utility utility = new Utility();
                Log.WriteLog(Controller, "");

                for (int i = 0; i < List_RpConfigModel.Count; i++)
                {
                    if (List_RpConfigModel[i].item_code.Contains("FILE_NAME"))
                    {
                        InterfaceDmsSftpModel DmsSftpModel = new InterfaceDmsSftpModel();
                        string[] item_code = List_RpConfigModel[i].item_code.Split('_');
                        string dms_name = item_code[2];

                        DmsSftpModel.RowNumber = i;
                        DmsSftpModel.dms_name = dms_name;
                        DmsSftpModel.file_name = List_RpConfigModel[i].item_value.Replace("yyyyMMdd", utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date).ToString("yyyyMMdd"));
                        DmsSftpModel.file_path = List_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SERVICE")?.item_value;
                        DmsList.Add(DmsSftpModel);
                        Log.WriteLog(Controller, "List File = " + DmsSftpModel.file_name);
                    }
                }

                Log.WriteLog(Controller, "SFTP IP = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "IP")?.item_value);
                Log.WriteLog(Controller, "SFTP PORT = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PORT")?.item_value);
                Log.WriteLog(Controller, "SFTP USER = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "USER")?.item_value);
                Log.WriteLog(Controller, "SFTP PATH = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SFTP")?.item_value);
                Log.WriteLog(Controller, "SFTP PATH_SERVICE = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SERVICE")?.item_value);
                Log.WriteLog(Controller, "SFTP SSHHOSTKEYFINGERPRINT = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "SSHHOSTKEYFINGERPRINT")?.item_value);
                Log.WriteLog(Controller, "SFTP SSHPRIVATEKEYPATH = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "SSHPRIVATEKEYPATH")?.item_value);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Export DMS Monthly To SFTP

        [HttpPost]
        public JsonResult ExportDMSDataSetMonthlyToSFTP(AdminViewModel Model)
        {
            string StrMsg = string.Empty;
            int CountFail = 0;
            int CountSuccess = 0;
            int CountTotal;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("ExportDMSDataSetMonthlyToSFTP");

                Log.WriteLog(Controller, "Start ExportDMSDataSetMonthlyToSFTP ==========");

                //Step 1 : Get Config
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_DMS_DATASET_MONTHLY_SFTP", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
                }

                //Step 2 : Set Config
                List<InterfaceDmsSftpModel> DmsList = new List<InterfaceDmsSftpModel>();
                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                if (!Set_ConfigDMSDataSetMonthlyToSFTP(ref StrMsg, ref DmsList, RpConfigModel, Model))
                {
                    throw new Exception("Set_ConfigDMSDataSetMonthlyToSFTP() => " + StrMsg);
                }

                Log.WriteLog(Controller, "Get Config Success");

                ResultWithModel<InterfaceDmsSftpResult> ResultDmsSftp = new ResultWithModel<InterfaceDmsSftpResult>();
                List<ResultWithModel<InterfaceDmsSftpResult>> List_Result = new List<ResultWithModel<InterfaceDmsSftpResult>>();

                //Step 3 : Gen DMS DataSet List
                List<Task> taskList = new List<Task>();
                foreach (var Row in DmsList)
                {
                    Log.WriteLog(Controller, "Gen = " + Row.dms_name);
                    Task lastTask = new Task(() =>
                    {
                        ResultDmsSftp = new ResultWithModel<InterfaceDmsSftpResult>();
                        Row.create_by = ConsoleEnt.CreateBy;
                        Row.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date);
                        Row.RpConfigModel = RpConfigModel;

                        InterfaceEnt.InterfaceDms.ExportInterfaceDmsMonthly(Row, p =>
                        {
                            ResultDmsSftp = p;
                        });

                        List_Result.Add(ResultDmsSftp);
                    });

                    lastTask.Start();
                    taskList.Add(lastTask);
                }

                Task.WaitAll(taskList.ToArray());

                //Step 4 : Check Result
                CountTotal = List_Result.Count;
                foreach (var Row in List_Result)
                {
                    Log.WriteLog(Controller, "Name = " + Row.Data.InterfaceDmsSftpResultModel[0].dms_name);
                    Log.WriteLog(Controller, "File = " + Row.Data.InterfaceDmsSftpResultModel[0].file_name);
                    Log.WriteLog(Controller, "ReturnCode = [" + Row.RefCode + "] " + Row.Message);
                    Log.WriteLog(Controller, "");
                    if (!Row.Success)
                    {
                        CountFail += 1;
                    }
                    else
                    {
                        CountSuccess += 1;
                    }
                }

                StrMsg = "Total = [" + CountTotal.ToString() + "] File. ";
                StrMsg += "Success = [" + CountSuccess.ToString() + " ] File. ";

                if (CountFail > 0)
                {
                    StrMsg += "Fail = [" + CountFail.ToString() + " ] File.";
                    throw new Exception(StrMsg);
                }
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = " ExportDMSDataSetMonthlyToSFTP() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End ExportDMSDataSetMonthlyToSFTP ==========");
            }

            return Json(new { Status = "Success", Message = "ExportDMSDataSetMonthlyToSFTP() " + Model.asof_date + " : " + StrMsg });
        }
        private static bool Set_ConfigDMSDataSetMonthlyToSFTP(ref string ReturnMsg, ref List<InterfaceDmsSftpModel> DmsList, List<RpConfigModel> List_RpConfigModel, AdminViewModel Model)
        {
            try
            {
                Utility utility = new Utility();
                Log.WriteLog(Controller, "");

                for (int i = 0; i < List_RpConfigModel.Count; i++)
                {
                    if (List_RpConfigModel[i].item_code.Contains("FILE_NAME"))
                    {
                        InterfaceDmsSftpModel DmsSftpModel = new InterfaceDmsSftpModel();
                        string[] item_code = List_RpConfigModel[i].item_code.Split('_');
                        string dms_name = item_code[2];

                        DmsSftpModel.RowNumber = i;
                        DmsSftpModel.dms_name = dms_name;
                        DmsSftpModel.file_name = List_RpConfigModel[i].item_value.Replace("yyyyMMdd", utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date).ToString("yyyyMMdd"));
                        DmsSftpModel.file_path = List_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SERVICE")?.item_value;
                        DmsList.Add(DmsSftpModel);
                        Log.WriteLog(Controller, "List File = " + DmsSftpModel.file_name);
                    }
                }

                Log.WriteLog(Controller, "SFTP IP = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "IP")?.item_value);
                Log.WriteLog(Controller, "SFTP PORT = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PORT")?.item_value);
                Log.WriteLog(Controller, "SFTP USER = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "USER")?.item_value);
                Log.WriteLog(Controller, "SFTP PATH = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SFTP")?.item_value);
                Log.WriteLog(Controller, "SFTP PATH_SERVICE = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SERVICE")?.item_value);
                Log.WriteLog(Controller, "SFTP SSHHOSTKEYFINGERPRINT = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "SSHHOSTKEYFINGERPRINT")?.item_value);
                Log.WriteLog(Controller, "SFTP SSHPRIVATEKEYPATH = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "SSHPRIVATEKEYPATH")?.item_value);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Export FxReconcile To SFTP

        [HttpPost]
        public JsonResult ExportFxReconcileToSFTP(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("ExportFxReconcileToSFTP");

                Log.WriteLog(Controller, "Start ExportFxReconcileToSFTP ==========");
                //Step 1 : Get Config
                InterfaceFxReconcileSftpModel FxReconcileSftpModel = new InterfaceFxReconcileSftpModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_FX_RECONCILE_SFTP", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
                }

                //Step 2 : Set Config
                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                FxReconcileSftpModel.RpConfigModel = RpConfigModel;
                if (!Set_ConfigFxReconcileToSFTP(ref StrMsg, ref FxReconcileSftpModel, RpConfigModel, Model))
                {
                    throw new Exception("Set_ConfigFxReconcileToSFTP() => " + StrMsg);
                }

                //Step 3 : Interface FX Reconcile
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ExportInterfaceFxReconcile");
                ResultWithModel<InterfaceFxReconcileSftpResult> ResultFxReconcileSftp = new ResultWithModel<InterfaceFxReconcileSftpResult>();

                InterfaceEnt.InterfaceFx.ExportInterfaceFxReconcile(FxReconcileSftpModel, p =>
                {
                    ResultFxReconcileSftp = p;
                });

                if (!ResultFxReconcileSftp.Success)
                {
                    throw new Exception(ResultFxReconcileSftp.Message);
                }

                InterfaceFxReconcileSftpModel ResultModel = new InterfaceFxReconcileSftpModel();
                ResultModel = ResultFxReconcileSftp.Data.InterfaceFxReconcileSftpResultModel[0];

                foreach (var ListFile in ResultModel.FileSuccess)
                {
                    Log.WriteLog(Controller, "SFTP " + ListFile + " Success.");
                }

                foreach (var ListFile in ResultModel.FileFail)
                {
                    Log.WriteLog(Controller, "SFTP " + ListFile);
                    StrMsg += ListFile + "<br>";
                }

                if (StrMsg != string.Empty)
                {
                    throw new Exception(StrMsg);
                }

            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "ExportFxReconcileToSFTP() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End ExportFxReconcileToSFTP ==========");
            }

            return Json(new { Status = "Success", Message = "ExportFxReconcileToSFTP() " + Model.asof_date + " : Success." });
        }
        private static bool Set_ConfigFxReconcileToSFTP(ref string ReturnMsg, ref InterfaceFxReconcileSftpModel FxReconcileSftpModel, List<RpConfigModel> List_RpConfigModel, AdminViewModel Model)
        {
            try
            {
                Utility utility = new Utility();
                Log.WriteLog(Controller, "FileTransaction = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "FILE_NAME_TRANSACTION")?.item_value.Replace("yyyyMMdd", Model.asof_date));
                Log.WriteLog(Controller, "FilePosition = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "FILE_NAME_POSITION")?.item_value.Replace("yyyyMMdd", Model.asof_date));
                Log.WriteLog(Controller, "FilePostingEvent = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "FILE_NAME_POSTINGEVENT")?.item_value.Replace("yyyyMMdd", Model.asof_date));

                FxReconcileSftpModel.AsofDate = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date).Date;
                FxReconcileSftpModel.FilePath = List_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SERVICE")?.item_value;

                Log.WriteLog(Controller, "SFTP IP = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "IP")?.item_value);
                Log.WriteLog(Controller, "SFTP PORT = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PORT")?.item_value);
                Log.WriteLog(Controller, "SFTP USER = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "USER")?.item_value);
                Log.WriteLog(Controller, "SFTP PATH = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SFTP")?.item_value);
                Log.WriteLog(Controller, "SFTP PATH_SERVICE = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SERVICE")?.item_value);
                Log.WriteLog(Controller, "SFTP SSHHOSTKEYFINGERPRINT = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "SSHHOSTKEYFINGERPRINT")?.item_value);
                Log.WriteLog(Controller, "SFTP SSHPRIVATEKEYPATH = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "SSHPRIVATEKEYPATH")?.item_value);

            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Export NES To SFTP
        [HttpPost]
        public JsonResult ExportNesToSFTP(AdminViewModel Model)
        {
            string StrMsg = string.Empty;
            try
            {
                UpdateCheckingEod("ExportNesToSFTP");

                Log.WriteLog(Controller, "Start ExportNesToSFTP ==========");
                //Step 1 : Get Config
                InterfaceNesSftpModel NesSftpModel = new InterfaceNesSftpModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                StaticEntities StaticEnt = new StaticEntities();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_NES_SFTP", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode + "] " + ResultRpconfig.Message);
                }

                //Step 2 : Set Config
                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                NesSftpModel.RpConfigModel = RpConfigModel;
                if (!Set_ConfigNesToSFTP(ref StrMsg, ref NesSftpModel, RpConfigModel, Model))
                {
                    throw new Exception("Set_ConfigNesToSFTP() => " + StrMsg);
                }

                //Step 3 : Interface FX Reconcile
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ExportInterfaceNes");
                ExternalInterfaceEntities InterfaceEnt = new ExternalInterfaceEntities();
                ResultWithModel<InterfaceNesSftpResult> ResultNesSftp = new ResultWithModel<InterfaceNesSftpResult>();

                InterfaceEnt.InterfaceNes.ExportInterfaceNes(NesSftpModel, p =>
                {
                    ResultNesSftp = p;
                });

                if (!ResultNesSftp.Success)
                {
                    throw new Exception(ResultNesSftp.Message);
                }

                InterfaceNesSftpModel ResultModel = new InterfaceNesSftpModel();
                ResultModel = ResultNesSftp.Data.InterfaceNesSftpResultModel[0];
                if (ResultModel.FileSuccess != null)
                {
                    foreach (var ListFile in ResultModel.FileSuccess)
                    {
                        Log.WriteLog(Controller, "SFTP " + ListFile.ToString() + " Success.");
                    }
                }

                if (ResultModel.FileFail != null)
                {
                    foreach (var ListFile in ResultModel.FileFail)
                    {
                        Log.WriteLog(Controller, "SFTP " + ListFile.ToString());
                        StrMsg += ListFile.ToString() + "<br>";
                    }
                }

                if (StrMsg != string.Empty)
                {
                    throw new Exception(StrMsg);
                }

            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "ExportNesToSFTP() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message.ToString()
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End ExportNesToSFTP ==========");
            }

            return Json(new { Status = "Success", Message = "ExportNesToSFTP() " + Model.asof_date + " : Success." });
        }
        private static bool Set_ConfigNesToSFTP(ref string ReturnMsg, ref InterfaceNesSftpModel NesSftpModel, List<RpConfigModel> List_RpConfigModel, AdminViewModel Model)
        {
            try
            {
                Utility utility = new Utility();
                Log.WriteLog(Controller, "FileName = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "FILE_NAME")?.item_value.Replace("yyyyMMdd", Model.asof_date));

                NesSftpModel.AsofDate = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date).Date;
                NesSftpModel.FilePath = List_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SERVICE")?.item_value;

                Log.WriteLog(Controller, "SFTP IP = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "IP")?.item_value);
                Log.WriteLog(Controller, "SFTP PORT = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PORT")?.item_value);
                Log.WriteLog(Controller, "SFTP USER = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "USER")?.item_value);
                Log.WriteLog(Controller, "SFTP PATH = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SFTP")?.item_value);
                Log.WriteLog(Controller, "SFTP PATH_SERVICE = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SERVICE")?.item_value);
                Log.WriteLog(Controller, "SFTP SSHHOSTKEYFINGERPRINT = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "SSHHOSTKEYFINGERPRINT")?.item_value);
                Log.WriteLog(Controller, "SFTP SSHPRIVATEKEYPATH = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "SSHPRIVATEKEYPATH")?.item_value);

            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Export GL To SFTP

        [HttpPost]
        public JsonResult ExportGLToSFTP(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("ExportGlToSFTP");

                Log.WriteLog(Controller, "Start ExportGLToSFTP ==========");
                //Step 1 : Get Config
                InterfaceGlSftpModel GlSftpModel = new InterfaceGlSftpModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_GL_SFTP", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
                }

                //Step 2 : Set Config
                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                GlSftpModel.RpConfigModel = RpConfigModel;
                GlSftpModel.create_by = ConsoleEnt.CreateBy;
                if (!Set_ConfigGlToSFTP(ref StrMsg, ref GlSftpModel, RpConfigModel, Model))
                {
                    throw new Exception("Set_ConfigGlToSFTP() => " + StrMsg);
                }

                //Step 3 : Interface  Gl SFTP
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ExportInterfaceGl");
                ResultWithModel<InterfaceGlSftpResult> ResultGlSftp = new ResultWithModel<InterfaceGlSftpResult>();

                InterfaceEnt.InterfaceGl.ExportInterfaceGl(GlSftpModel, p =>
                {
                    ResultGlSftp = p;
                });

                if (!ResultGlSftp.Success)
                {
                    throw new Exception(ResultGlSftp.Message);
                }

                InterfaceGlSftpModel ResultModel = new InterfaceGlSftpModel();
                if (ResultGlSftp.Data != null && ResultGlSftp.Data.InterfaceGlSftpResultModel.Count > 0)
                {
                    ResultModel = ResultGlSftp.Data.InterfaceGlSftpResultModel[0];
                    var enableSftp = GlSftpModel.RpConfigModel.FirstOrDefault(a => a.item_code == "ENABLE_SFTP").item_value;
                    if (enableSftp == "Y")
                    {
                        foreach (var ListFile in ResultModel.FileSuccess)
                        {
                            Log.WriteLog(Controller, "SFTP " + ListFile + " Success.");
                        }

                        foreach (var ListFile in ResultModel.FileFail)
                        {
                            Log.WriteLog(Controller, "SFTP " + ListFile);
                            StrMsg += ListFile + "<br>";
                        }
                    }
                }

                if (StrMsg != string.Empty)
                {
                    throw new Exception(StrMsg);
                }
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "ExportGLToSFTP() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End ExportGLToSFTP ==========");
            }

            return Json(new { Status = "Success", Message = "ExportGLToSFTP() " + Model.asof_date + " : Success." });
        }
        private static bool Set_ConfigGlToSFTP(ref string ReturnMsg, ref InterfaceGlSftpModel GlSftpModel, List<RpConfigModel> List_RpConfigModel, AdminViewModel Model)
        {
            try
            {
                Utility utility = new Utility();
                Log.WriteLog(Controller, "");
                GlSftpModel.AsofDate = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date).Date;
                Log.WriteLog(Controller, "AsofDate = " + GlSftpModel.AsofDate.ToString("yyyyMMdd"));
                Log.WriteLog(Controller, "File Name GLREPO = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "FILE_NAME_GLREPO")?.item_value);
                Log.WriteLog(Controller, "File Name SWREPO = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "FILE_NAME_SWREPO")?.item_value);
                Log.WriteLog(Controller, "SFTP IP = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "IP")?.item_value);
                Log.WriteLog(Controller, "SFTP PORT = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PORT")?.item_value);
                Log.WriteLog(Controller, "SFTP USER = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "USER")?.item_value);
                Log.WriteLog(Controller, "SFTP PATH = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SFTP")?.item_value);
                Log.WriteLog(Controller, "SFTP PATH_SERVICE = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SERVICE")?.item_value);
                Log.WriteLog(Controller, "SFTP SSHHOSTKEYFINGERPRINT = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "SSHHOSTKEYFINGERPRINT")?.item_value);
                Log.WriteLog(Controller, "SFTP SSHPRIVATEKEYPATH = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "SSHPRIVATEKEYPATH")?.item_value);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Export FITS Trp To SFTP

        [HttpPost]
        public JsonResult ExportFITSTrpToSFTP(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("ExportFITSTrpToSFTP");
                Log.WriteLog(Controller, "Start ExportFITSTrpToSFTP ==========");

                //Step 1 : Get Config
                InterfaceTrpFitsSftpModel trpFitsSftpModel = new InterfaceTrpFitsSftpModel();
                ResultWithModel<RpConfigResult> resultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> rpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_FITS_TRP_SFTP", string.Empty, p =>
                {
                    resultRpconfig = p;
                });

                if (!resultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + resultRpconfig.RefCode + "] " + resultRpconfig.Message);
                }

                //Step 2 : Set Config
                rpConfigModel = resultRpconfig.Data.RpConfigResultModel;
                trpFitsSftpModel.RpConfigModel = rpConfigModel;
                if (!Set_ConfigFITSTrp(ref StrMsg, ref trpFitsSftpModel, rpConfigModel, Model))
                {
                    throw new Exception("Set_ConfigFITSTrp() => " + StrMsg);
                }

                //Step 2 : ExportInterfaceTrpFits 
                ResultWithModel<InterfaceTrpFitsSftpResult> resultFitsSftp = new ResultWithModel<InterfaceTrpFitsSftpResult>();

                InterfaceEnt.InterfaceTrpFits.ExportInterfaceTrpFits(trpFitsSftpModel, p =>
                {
                    resultFitsSftp = p;
                });

                if (!resultFitsSftp.Success)
                {
                    throw new Exception(resultFitsSftp.Message);
                }

            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "ExportGLToSFTP() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End ExportFITSTrpToSFTP ==========");
            }

            return Json(new { Status = "Success", Message = "ExportGLToSFTP() " + Model.asof_date + " : Success." });
        }
        private static bool Set_ConfigFITSTrp(ref string StrMsg, ref InterfaceTrpFitsSftpModel TrpFitsSftpModel, List<RpConfigModel> list_RpConfigModel, AdminViewModel model)
        {
            try
            {
                Utility utility = new Utility();
                Log.WriteLog(Controller, "");
                TrpFitsSftpModel.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(model.asof_date).Date;
                Log.WriteLog(Controller, "AsOf_date = " + TrpFitsSftpModel.asof_date.ToString("yyyyMMdd"));
                Log.WriteLog(Controller, "SFTP IP = " + list_RpConfigModel.FirstOrDefault(a => a.item_code == "IP")?.item_value);
                Log.WriteLog(Controller, "SFTP PORT = " + list_RpConfigModel.FirstOrDefault(a => a.item_code == "PORT")?.item_value);
                Log.WriteLog(Controller, "SFTP USER = " + list_RpConfigModel.FirstOrDefault(a => a.item_code == "USER")?.item_value);
                Log.WriteLog(Controller, "SFTP PATH = " + list_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SFTP")?.item_value);
                Log.WriteLog(Controller, "SFTP PATH_SERVICE = " + list_RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SERVICE")?.item_value);
                Log.WriteLog(Controller, "SFTP SSHHOSTKEYFINGERPRINT = " + list_RpConfigModel.FirstOrDefault(a => a.item_code == "SSHHOSTKEYFINGERPRINT")?.item_value);
                Log.WriteLog(Controller, "SFTP SSHPRIVATEKEYPATH = " + list_RpConfigModel.FirstOrDefault(a => a.item_code == "SSHPRIVATEKEYPATH")?.item_value);

            }
            catch (Exception Ex)
            {
                StrMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Interface CR TransLimit Daily & Eod

        [HttpPost]
        public JsonResult InterfaceCRTransLimitDaily(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("InterfaceCRTransLimit");

                Log.WriteLog(Controller, "Start InterfaceCRTransLimitDaily ==========");

                // Step 2 : Get Config
                InterfaceTransLimitCrModel TransLimitCrModel = new InterfaceTransLimitCrModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_CR_INTERFACE_TRANS_LIMIT", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
                }

                // Step 3 : Set Config
                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                if (!Set_ConfigInterfaceCRTransLimit(ref StrMsg, ref TransLimitCrModel, RpConfigModel, Model))
                {
                    throw new Exception("Set_ConfigInterfaceCRTransLimit() => " + StrMsg);
                }

                // Step 4 : Interface CR TransLimit
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ExportTransLimitCr");
                ResultWithModel<InterfaceTransLimitCrResult> ResultTransLimitCr = new ResultWithModel<InterfaceTransLimitCrResult>();
                List<ResponsTransLimitCr> List_RespTrans = new List<ResponsTransLimitCr>();

                InterfaceEnt.InterfaceTransLimitCr.ExportTransLimitCr(TransLimitCrModel, p =>
                {
                    ResultTransLimitCr = p;
                });

                if (!ResultTransLimitCr.Success)
                {
                    throw new Exception(ResultTransLimitCr.Message);
                }

                InterfaceTransLimitCrModel ResultTransLimitCrModel = new InterfaceTransLimitCrModel();
                ResultTransLimitCrModel = ResultTransLimitCr.Data.InterfaceTransLimitCrResultModel[0];

                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "ReturnCode = [" + ResultTransLimitCr.RefCode + "] " + ResultTransLimitCr.Message);
                Log.WriteLog(Controller, "Trans Total = [" + ResultTransLimitCrModel.TransTotal.ToString() + "] Item.");
                Log.WriteLog(Controller, "TransCancel Total = [" + ResultTransLimitCrModel.TransCancelTotal.ToString() + "] Item.");
                Log.WriteLog(Controller, "Trans Success = [" + ResultTransLimitCrModel.TransSuccess.ToString() + "] Item.");
                Log.WriteLog(Controller, "Trans Fail = [" + ResultTransLimitCrModel.TransFail.ToString() + "] Item.");

                List_RespTrans = ResultTransLimitCrModel.List_RespTrans;
                foreach (var Row in List_RespTrans)
                {
                    Log.WriteLog(Controller, "");
                    Log.WriteLog(Controller, "TransNo = " + Row.TransNo);
                    Log.WriteLog(Controller, "TotalColl = [" + Row.TotalColl + "] Item.");
                    Log.WriteLog(Controller, "Action = " + Row.Action);
                    Log.WriteLog(Controller, "ReturnCode = " + Row.ReturnCode);
                    Log.WriteLog(Controller, "ReturnMsg = " + Row.ReturnMsg);
                }

                if (ResultTransLimitCrModel.TransFail > 0)
                {
                    throw new Exception("Trans [" + ResultTransLimitCrModel.TransFail.ToString() + "] Item.");
                }

            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "InterfaceCRTransLimitDaily() " +
                                  Model.asof_date +
                                  " => Fail  " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InterfaceCRTransLimitDaily ==========");
            }

            return Json(new { Status = "Success", Message = "InterfaceCRTransLimitDaily() " + Model.asof_date + " : Success." });
        }
        [HttpPost]
        public JsonResult InterfaceCRTransLimitEod(AdminViewModel Model)
        {
            string StrMsg = string.Empty;
            try
            {
                UpdateCheckingEod("InterfaceCRTransLimitEod");

                Log.WriteLog(Controller, "Start InterfaceCRTransLimitEod ==========");

                // Step 2 : Get Config
                InterfaceTransLimitCrModel TransLimitCrModel = new InterfaceTransLimitCrModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_CR_INTERFACE_TRANS_LIMIT", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
                }

                // Step 3 : Set Config
                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                if (!Set_ConfigInterfaceCRTransLimit(ref StrMsg, ref TransLimitCrModel, RpConfigModel, Model, "EOD"))
                {
                    throw new Exception("Set_ConfigInterfaceCRTransLimit() => " + StrMsg);
                }

                // Step 4 : Interface CR TransLimit Eod
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ExportTransLimitCrEod");
                ResultWithModel<InterfaceTransLimitCrResult> ResultTransLimitCr = new ResultWithModel<InterfaceTransLimitCrResult>();
                List<ResponsTransLimitCr> List_RespTrans = new List<ResponsTransLimitCr>();

                InterfaceEnt.InterfaceTransLimitCr.ExportTransLimitCrEod(TransLimitCrModel, p =>
                {
                    ResultTransLimitCr = p;
                });

                if (!ResultTransLimitCr.Success)
                {
                    throw new Exception(ResultTransLimitCr.Message);
                }

                InterfaceTransLimitCrModel ResultTransLimitCrModel = new InterfaceTransLimitCrModel();
                ResultTransLimitCrModel = ResultTransLimitCr.Data.InterfaceTransLimitCrResultModel[0];

                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "ReturnCode = [" + ResultTransLimitCr.RefCode + "] " + ResultTransLimitCr.Message);
                Log.WriteLog(Controller, "Trans Total = [" + ResultTransLimitCrModel.TransTotal.ToString() + "] Item.");
                Log.WriteLog(Controller, "TransCancel Total = [" + ResultTransLimitCrModel.TransCancelTotal.ToString() + "] Item.");
                Log.WriteLog(Controller, "Trans Success = [" + ResultTransLimitCrModel.TransSuccess.ToString() + "] Item.");
                Log.WriteLog(Controller, "Trans Fail = [" + ResultTransLimitCrModel.TransFail.ToString() + "] Item.");

                List_RespTrans = ResultTransLimitCrModel.List_RespTrans;
                foreach (var Row in List_RespTrans)
                {
                    Log.WriteLog(Controller, "");
                    Log.WriteLog(Controller, "TransNo = " + Row.TransNo);
                    Log.WriteLog(Controller, "TotalColl = [" + Row.TotalColl + "] Item.");
                    Log.WriteLog(Controller, "Action = " + Row.Action);
                    Log.WriteLog(Controller, "ReturnCode = " + Row.ReturnCode);
                    Log.WriteLog(Controller, "ReturnMsg = " + Row.ReturnMsg);
                }

                if (ResultTransLimitCrModel.TransFail > 0)
                {
                    throw new Exception(" Trans [" + ResultTransLimitCrModel.TransFail.ToString() + "] Item.");
                }
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "InterfaceCRTransLimitEod() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InterfaceCRTransLimitEod ==========");
            }

            return Json(new { Status = "Success", Message = "InterfaceCRTransLimitEod() " + Model.asof_date + " : Success." });
        }
        private static bool Set_ConfigInterfaceCRTransLimit(ref string ReturnMsg, ref InterfaceTransLimitCrModel TransLimitCrModel, List<RpConfigModel> List_RpConfigModel, AdminViewModel Model, string Type = "")
        {
            try
            {
                Utility utility = new Utility();
                TransLimitCrModel.ServiceUrl = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_URL")?.item_value;
                if (!String.IsNullOrEmpty(List_RpConfigModel.FirstOrDefault(a => a.item_code == "TIMEOUT_SERVICE")?.item_value))
                {
                    TransLimitCrModel.ServiceTimeOut = System.Convert.ToInt32(List_RpConfigModel.FirstOrDefault(a => a.item_code == "TIMEOUT_SERVICE")?.item_value);
                }

                TransLimitCrModel.ChannelId = List_RpConfigModel.FirstOrDefault(a => a.item_code == "CHANNEL_ID")?.item_value;
                TransLimitCrModel.RegisterCode = List_RpConfigModel.FirstOrDefault(a => a.item_code == "REGISTER_CODE")?.item_value;
                TransLimitCrModel.AsOfDate = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date).Date;
                TransLimitCrModel.create_by = "Console";

                Log.WriteLog(Controller, "ServiceUrl       = " + TransLimitCrModel.ServiceUrl);
                Log.WriteLog(Controller, "ServiceTimeOut   = " + TransLimitCrModel.ServiceTimeOut.ToString());
                Log.WriteLog(Controller, "ChannelId        = " + TransLimitCrModel.ChannelId);
                Log.WriteLog(Controller, "RegisterCode     = " + TransLimitCrModel.RegisterCode);
                Log.WriteLog(Controller, "AsOfDate         = " + TransLimitCrModel.AsOfDate.ToString("yyyyMMdd"));
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Interface FITS BondPledge

        [HttpPost]
        public JsonResult InterfaceFITSBondPledge(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("InterfaceFITSBondPledge");

                Log.WriteLog(Controller, "Start InterfaceFITSBondPledge ==========");
                // Step 2 : Get Config
                InterfaceBondPledgeFitsModel BondPledgeFitsModel = new InterfaceBondPledgeFitsModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_FITS_INTERFACE_BONDPLEDGE", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
                }

                // Step 3 : Set Config
                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                if (!Set_ConfigInterfaceFITSBondPledge(ref StrMsg, ref BondPledgeFitsModel, RpConfigModel, Model))
                {
                    throw new Exception("Set_ConfigInterfaceFITSBondPledge() => " + StrMsg);
                }

                // Step 4 : Interface FITS BondPledge
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ExportBondPledgeFits");
                ResultWithModel<InterfaceBondPledgeFitsResult> ResultBondPledgeFits = new ResultWithModel<InterfaceBondPledgeFitsResult>();

                InterfaceEnt.InterfaceBondPledgeFits.ExportBondPledgeFits(BondPledgeFitsModel, p =>
                {
                    ResultBondPledgeFits = p;
                });

                if (!ResultBondPledgeFits.Success)
                {
                    throw new Exception(ResultBondPledgeFits.Message);
                }

            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "InterfaceFITSBondPledge() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InterfaceFITSBondPledge ==========");
            }

            return Json(new { Status = "Success", Message = "InterfaceFITSBondPledge() " + Model.asof_date + " : Success." });
        }
        private static bool Set_ConfigInterfaceFITSBondPledge(ref string StrMsg, ref InterfaceBondPledgeFitsModel BondPledgeFitsModel, List<RpConfigModel> List_RpConfigModel, AdminViewModel Model)
        {
            try
            {
                Utility utility = new Utility();
                BondPledgeFitsModel.RpConfigModel = List_RpConfigModel;
                BondPledgeFitsModel.ServiceUrl = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_URL")?.item_value;
                if (!String.IsNullOrEmpty(List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_TIMEOUT")?.item_value))
                {
                    BondPledgeFitsModel.ServiceTimeOut = System.Convert.ToInt32(List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_TIMEOUT")?.item_value);
                }

                BondPledgeFitsModel.AsOfDate = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date).Date;
                BondPledgeFitsModel.create_by = "Console";
            }
            catch (Exception Ex)
            {
                StrMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Interface CCM

        [HttpPost]
        public JsonResult InterfaceCCM(AdminViewModel Model)
        {
            string StrMsg = string.Empty;
            int CountFail = 0;
            int CountSuccess = 0;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("InterfaceCCM");

                Log.WriteLog(Controller, "Start InterfaceCCM ==========");

                ResultWithModel<RpConfigResult> resultRpconfig = new ResultWithModel<RpConfigResult>();
                StaticEnt.RpConfig.GetRpConfig("RP_CCM_INTERFACE", string.Empty, p =>
                {
                    resultRpconfig = p;
                });

                if (!resultRpconfig.Success)
                {
                    throw new Exception(resultRpconfig.Message);
                }

                if (resultRpconfig.Data.RpConfigResultModel == null)
                {
                    throw new Exception("Config is null");
                }

                //select RP Confirmation
                DateTime now = DateTime.Now;
                Utility utility = new Utility();
                ResultWithModel<InterfaceConfirmationResult> rwmCCM = new ResultWithModel<InterfaceConfirmationResult>();
                InterfaceCCMSearch ccmSearch = new InterfaceCCMSearch();
                //ccmSearch.search_date = DateTime.Today;
                ccmSearch.search_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date).Date;

                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = ConsoleEnt.PageNumber;
                paging.RecordPerPage = ConsoleEnt.RecordPerPage;
                ccmSearch.paging = paging;

                //Add Orderby
                var orders = new List<OrderByModel>();
                ccmSearch.ordersby = orders;

                InterfaceEnt.InterfaceConfirmation.GetInterfaceCCMList(ccmSearch, p =>
                {
                    rwmCCM = p;
                });

                if (rwmCCM.Success)
                {
                    string guid = Guid.NewGuid().ToString();
                    List<ResultCCM> resultCcms = new List<ResultCCM>();
                    foreach (var row in rwmCCM.Data.InterfaceConfirmationResultModel)
                    {
                        ResultWithModel<InterfaceConfirmationResult> res = new ResultWithModel<InterfaceConfirmationResult>();
                        InterfaceConfirmationModel model = row;
                        model.TransDate = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date).Date.ToString("yyyyMMdd");
                        model.TransTime = now.ToString("HH:mm:ss");
                        model.guid = guid;
                        model.create_by = ConsoleEnt.CreateBy;
                        model.RpConfigModel = resultRpconfig.Data.RpConfigResultModel;

                        InterfaceEnt.InterfaceConfirmation.SendConfirmation(model, p =>
                        {
                            res = p;
                        });

                        resultCcms.Add(new ResultCCM
                        {
                            Success = res.Success,
                            Message = res.Message
                        });

                        Log.WriteLog(Controller, "ReturnCode = [" + res.RefCode + "] " + res.Message);

                        if (res.Success == false)
                        {
                            CountFail += 1;
                        }
                        else
                        {
                            CountSuccess += 1;
                        }
                    }

                    StrMsg += "Success = [" + CountSuccess.ToString() + " ] File. ";

                    if (CountFail > 0)
                    {
                        StrMsg += "Fail = [" + CountFail.ToString() + " ] File.";
                        throw new Exception(StrMsg);
                    }
                }
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "InterfaceCCM() " +
                                 Model.asof_date +
                                 " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InterfaceCCM ==========");
            }


            return Json(new { Status = "Success", Message = "InterfaceCCM() " + Model.asof_date + " : Success." });
        }
        private class ResultCCM
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        #endregion

        #region Function : Interface BBG MarketPrice(Mark To Market)

        [HttpPost]
        public JsonResult InterfaceMarketPriceBBG(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("InterfaceBBGMarketPrice");

                Log.WriteLog(Controller, "Start InterfaceMarketPriceBBG ==========");

                //Step 1 : Get Config
                InterfaceMarketPriceModel interfaceMarketPrice = new InterfaceMarketPriceModel();
                ResultWithModel<RpConfigResult> resultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> rpConfigModelList = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_FITS_INTERFACE_MARKET_PRICE_BBG", string.Empty, p =>
                {
                    resultRpconfig = p;
                });

                if (resultRpconfig.RefCode != 0)
                {
                    throw new Exception("GetMarketPriceBBGConfig() => [" + resultRpconfig.RefCode.ToString() + "]" + resultRpconfig.Message);
                }

                rpConfigModelList = resultRpconfig.Data.RpConfigResultModel;
                if (!Set_ConfigInterfaceMarketPrice(ref StrMsg, rpConfigModelList, ref interfaceMarketPrice, Model))
                {
                    throw new Exception("Set_ConfigInterfaceMarketPriceBBG() => " + StrMsg);
                }

                //select RP Confirmation
                ResultWithModel<InterfaceMarketPriceBBGResult> rwm = new ResultWithModel<InterfaceMarketPriceBBGResult>();

                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = ConsoleEnt.PageNumber;
                paging.RecordPerPage = ConsoleEnt.RecordPerPage;
                interfaceMarketPrice.paging = paging;

                //Add Orderby
                var orders = new List<OrderByModel>();
                interfaceMarketPrice.ordersby = orders;
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ImportMarketPriceBBG");

                InterfaceEnt.InterfaceMarketPriceBBG.ImportMarketPriceBBG(interfaceMarketPrice, p =>
                {
                    rwm = p;
                });

                if (!rwm.Success)
                {
                    throw new Exception(rwm.Message);
                }
            }
            catch (Exception Ex)
            {
                Log.WriteLog(Controller, "");
                return Json(new
                {
                    Status = "Error",
                    Message = "InterfaceMarketPriceBBG() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InterfaceMarketPriceBBG ==========");
            }

            return Json(new { Status = "Success", Message = "InterfaceMarketPriceBBG() " + Model.asof_date + " : Success." });
        }
        private static bool Set_ConfigInterfaceMarketPrice(ref string ReturnMsg, List<RpConfigModel> List_RpConfigModel, ref InterfaceMarketPriceModel interfaceMarketPrice, AdminViewModel Model)
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
                interfaceMarketPrice.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date).ToString("yyyyMMdd"); //20170720
                interfaceMarketPrice.source_type = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SOURCE_TYPE")?.item_value;
                interfaceMarketPrice.security_code = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SECURITY_CODE")?.item_value;
                interfaceMarketPrice.urlservice = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_URL")?.item_value;

                Log.WriteLog(Controller, "channel       = " + interfaceMarketPrice.channel);
                Log.WriteLog(Controller, "ref_no        = " + interfaceMarketPrice.ref_no);
                Log.WriteLog(Controller, "request_date  = " + interfaceMarketPrice.request_date);
                Log.WriteLog(Controller, "request_time  = " + interfaceMarketPrice.request_time);
                Log.WriteLog(Controller, "mode          = " + interfaceMarketPrice.mode);
                Log.WriteLog(Controller, "asof_date     = " + interfaceMarketPrice.asof_date);
                Log.WriteLog(Controller, "source_type   = " + interfaceMarketPrice.source_type);
                Log.WriteLog(Controller, "security_code = " + interfaceMarketPrice.security_code);
                Log.WriteLog(Controller, "urlservice    = " + interfaceMarketPrice.urlservice);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Interface MarketPrice TBMA Fits

        [HttpPost]
        public JsonResult InterfaceMarketPriceTbmaT1(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("InterfaceMarketPriceTbmaT1");

                Log.WriteLog(Controller, "Start InterfaceMarketPriceTbmaT1 ==========");

                //Step 1 : Get Config
                InterfaceMarketPriceTbmaModel interfaceMarketPriceTbma = new InterfaceMarketPriceTbmaModel();
                ResultWithModel<RpConfigResult> resultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> rpConfigModelList = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_FITS_INTERFACE_MARKET_PRICE_TBMA", string.Empty, p =>
                {
                    resultRpconfig = p;
                });

                if (resultRpconfig.RefCode != 0)
                {
                    throw new Exception("GetMarketPriceTbmaConfig() => [" + resultRpconfig.RefCode.ToString() + "]" + resultRpconfig.Message);
                }

                rpConfigModelList = resultRpconfig.Data.RpConfigResultModel;
                if (!Set_ConfigInterfaceMarketPriceTbma(ref StrMsg, rpConfigModelList, ref interfaceMarketPriceTbma, Model, "T1"))
                {
                    throw new Exception("Set_ConfigInterfaceMarketPriceTbma() => " + StrMsg);
                }

                //select RP Confirmation
                ResultWithModel<InterfaceMarketPriceTbmaResult> rwm = new ResultWithModel<InterfaceMarketPriceTbmaResult>();

                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = ConsoleEnt.PageNumber;
                paging.RecordPerPage = ConsoleEnt.RecordPerPage;
                interfaceMarketPriceTbma.paging = paging;

                //Add Orderby
                var orders = new List<OrderByModel>();
                interfaceMarketPriceTbma.ordersby = orders;
                interfaceMarketPriceTbma.create_by = "web";

                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ImportMarketPriceTbma");
                Log.WriteLog(Controller, " - create_by : " + interfaceMarketPriceTbma.create_by);

                InterfaceEnt.InterfaceMarketPriceTbma.ImportMarketPriceTbma(interfaceMarketPriceTbma, p =>
                {
                    rwm = p;
                });

                if (!rwm.Success)
                {
                    throw new Exception(rwm.Message);
                }
            }
            catch (Exception Ex)
            {
                Log.WriteLog(Controller, "");
                return Json(new
                {
                    Status = "Error",
                    Message = "InterfaceMarketPriceTbma() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InterfaceMarketPriceTbmaT1 ==========");
            }

            return Json(new { Status = "Success", Message = "InterfaceMarketPriceTbmaT1() " + Model.asof_date + " : Success." });
        }

        public JsonResult InterfaceMarketPriceTbmaT2(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("InterfaceMarketPriceTbmaT2");

                Log.WriteLog(Controller, "Start InterfaceMarketPriceTbmaT2 ==========");

                //Step 1 : Get Config
                InterfaceMarketPriceTbmaModel interfaceMarketPriceTbma = new InterfaceMarketPriceTbmaModel();
                ResultWithModel<RpConfigResult> resultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> rpConfigModelList = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_FITS_INTERFACE_MARKET_PRICE_TBMA", string.Empty, p =>
                {
                    resultRpconfig = p;
                });

                if (resultRpconfig.RefCode != 0)
                {
                    throw new Exception("GetMarketPriceTbmaConfig() => [" + resultRpconfig.RefCode.ToString() + "]" + resultRpconfig.Message);
                }

                rpConfigModelList = resultRpconfig.Data.RpConfigResultModel;
                if (!Set_ConfigInterfaceMarketPriceTbma(ref StrMsg, rpConfigModelList, ref interfaceMarketPriceTbma, Model, "T2"))
                {
                    throw new Exception("Set_ConfigInterfaceMarketPriceTbma() => " + StrMsg);
                }

                //select RP Confirmation
                ResultWithModel<InterfaceMarketPriceTbmaResult> rwm = new ResultWithModel<InterfaceMarketPriceTbmaResult>();

                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = ConsoleEnt.PageNumber;
                paging.RecordPerPage = ConsoleEnt.RecordPerPage;
                interfaceMarketPriceTbma.paging = paging;

                //Add Orderby
                var orders = new List<OrderByModel>();
                interfaceMarketPriceTbma.ordersby = orders;
                interfaceMarketPriceTbma.create_by = "web";

                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ImportMarketPriceTbma");
                Log.WriteLog(Controller, " - create_by : " + interfaceMarketPriceTbma.create_by);

                InterfaceEnt.InterfaceMarketPriceTbma.ImportMarketPriceTbma(interfaceMarketPriceTbma, p =>
                {
                    rwm = p;
                });

                if (!rwm.Success)
                {
                    throw new Exception(rwm.Message);
                }
            }
            catch (Exception Ex)
            {
                Log.WriteLog(Controller, "");
                return Json(new
                {
                    Status = "Error",
                    Message = "InterfaceMarketPriceTbma() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InterfaceMarketPriceTbmaT2 ==========");
            }

            return Json(new { Status = "Success", Message = "InterfaceMarketPriceTbmaT2() " + Model.asof_date + " : Success." });
        }
        private static bool Set_ConfigInterfaceMarketPriceTbma(ref string ReturnMsg, List<RpConfigModel> List_RpConfigModel, ref InterfaceMarketPriceTbmaModel interfaceMarketPriceTbma, AdminViewModel Model, string market_date_t)
        {
            try
            {
                Utility utility = new Utility();
                //Set Req Data InterfaceMarketPriceModel form config
                DateTime date = DateTime.Now;
                interfaceMarketPriceTbma.channel = List_RpConfigModel.FirstOrDefault(a => a.item_code == "CHANNEL")?.item_value;
                interfaceMarketPriceTbma.ref_no = date.ToString("yyyyMMddHHMMss");
                interfaceMarketPriceTbma.request_date = date.ToString("yyyyMMdd");
                interfaceMarketPriceTbma.request_time = date.ToString("HH:MM:ss");
                interfaceMarketPriceTbma.mode = int.Parse(List_RpConfigModel.FirstOrDefault(a => a.item_code == "MODE")?.item_value);
                interfaceMarketPriceTbma.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date).ToString("yyyyMMdd"); //20170720
                if (market_date_t == "T1")
                {
                    interfaceMarketPriceTbma.market_date_t = "1";
                    interfaceMarketPriceTbma.source_type = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SOURCE_TYPE_T1")?.item_value;
                }
                else
                {
                    interfaceMarketPriceTbma.market_date_t = "2";
                    interfaceMarketPriceTbma.source_type = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SOURCE_TYPE_T2")?.item_value;
                }
                interfaceMarketPriceTbma.security_code = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SECURITY_CODE")?.item_value;
                interfaceMarketPriceTbma.urlservice = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_URL")?.item_value;

                Log.WriteLog(Controller, "channel       = " + interfaceMarketPriceTbma.channel);
                Log.WriteLog(Controller, "ref_no        = " + interfaceMarketPriceTbma.ref_no);
                Log.WriteLog(Controller, "request_date  = " + interfaceMarketPriceTbma.request_date);
                Log.WriteLog(Controller, "request_time  = " + interfaceMarketPriceTbma.request_time);
                Log.WriteLog(Controller, "mode          = " + interfaceMarketPriceTbma.mode);
                Log.WriteLog(Controller, "asof_date     = " + interfaceMarketPriceTbma.asof_date);
                Log.WriteLog(Controller, "source_type   = " + interfaceMarketPriceTbma.source_type);
                Log.WriteLog(Controller, "security_code = " + interfaceMarketPriceTbma.security_code);
                Log.WriteLog(Controller, "urlservice    = " + interfaceMarketPriceTbma.urlservice);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Interface EQUITY NavPrice
        [HttpPost]
        public JsonResult InterfaceEQUITYNavPrice(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("InterfaceEQUITYNavPrice");

                Log.WriteLog(Controller, "Start InterfaceEQUITYNavPrice ==========");

                //Step 1 : Get Config
                InterfaceReqNavPriceModel reqNavPriceModel = new InterfaceReqNavPriceModel();
                ResultWithModel<RpConfigResult> resultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> rpConfigModelList = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_EQUITY_INTERFACE_NAV_PRICE", string.Empty, p =>
                {
                    resultRpconfig = p;
                });

                if (resultRpconfig.RefCode != 0)
                {
                    throw new Exception("GetRpConfig() => [" + resultRpconfig.RefCode.ToString() + "]" + resultRpconfig.Message);
                }

                rpConfigModelList = resultRpconfig.Data.RpConfigResultModel;
                if (!Set_ConfigInterfaceEQUITYNavPrice(ref StrMsg, rpConfigModelList, ref reqNavPriceModel, Model))
                {
                    throw new Exception("Set_ConfigInterfaceEQUITYNavPrice() => " + StrMsg);
                }

                ResultWithModel<InterfaceReqNavPriceResult> rwm = new ResultWithModel<InterfaceReqNavPriceResult>();

                //Add Paging
                PagingModel paging = new PagingModel();
                paging.PageNumber = ConsoleEnt.PageNumber;
                paging.RecordPerPage = ConsoleEnt.RecordPerPage;
                reqNavPriceModel.paging = paging;

                //Add Orderby
                var orders = new List<OrderByModel>();
                reqNavPriceModel.ordersby = orders;
                reqNavPriceModel.create_by = "web";

                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run InterfaceNavPriceEquity");
                Log.WriteLog(Controller, " - create_by : " + reqNavPriceModel.create_by);

                InterfaceEnt.InterfaceNavPrice.InterfaceNavPriceEquity(reqNavPriceModel, p =>
                {
                    rwm = p;
                });

                if (!rwm.Success)
                {
                    throw new Exception(rwm.Message);
                }
            }
            catch (Exception Ex)
            {
                Log.WriteLog(Controller, "");
                return Json(new
                {
                    Status = "Error",
                    Message = "InterfaceNavPriceEquity() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InterfaceNavPriceEquity ==========");
            }


            return Json(new { Status = "Success", Message = "InterfaceEQUITYNavPrice() " + Model.asof_date + " : Success." });
        }

        private static bool Set_ConfigInterfaceEQUITYNavPrice(ref string ReturnMsg, List<RpConfigModel> List_RpConfigModel, ref InterfaceReqNavPriceModel interfaceMarketPrice, AdminViewModel Model)
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
                interfaceMarketPrice.asof_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date).ToString("yyyyMMdd"); //20170720
                interfaceMarketPrice.url_service = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_URL")?.item_value;

                Log.WriteLog(Controller, "channel       = " + interfaceMarketPrice.channel);
                Log.WriteLog(Controller, "ref_no        = " + interfaceMarketPrice.ref_no);
                Log.WriteLog(Controller, "request_date  = " + interfaceMarketPrice.request_date);
                Log.WriteLog(Controller, "request_time  = " + interfaceMarketPrice.request_time);
                Log.WriteLog(Controller, "asof_date     = " + interfaceMarketPrice.asof_date); ;
                Log.WriteLog(Controller, "urlservice    = " + interfaceMarketPrice.url_service);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Interface SSMD ExchangeRate(Spot Rate)

        [HttpPost]
        public JsonResult InterfaceSSMDExchangeRate(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("InterfaceSSMDExchangeRate");

                Log.WriteLog(Controller, "Start InterfaceSSMDExchangeRate ==========");

                //Step 1 : Get Config
                InterfaceReqExchRateHeaderSummitModel reqExchRateHeaderSummitModel = new InterfaceReqExchRateHeaderSummitModel();
                ResultWithModel<RpConfigResult> resultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> rpConfigModelList = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_SSMD_INTERFACE", string.Empty, p =>
                {
                    resultRpconfig = p;
                });

                if (resultRpconfig.RefCode != 0)
                {
                    throw new Exception("Get_RP_SSMD_INTERFACE_Config() => [" + resultRpconfig.RefCode.ToString() + "]" + resultRpconfig.Message);
                }

                rpConfigModelList = resultRpconfig.Data.RpConfigResultModel;
                if (!Set_ConfigInterfaceExchangeRateSummit(ref StrMsg, rpConfigModelList, ref reqExchRateHeaderSummitModel, Model))
                {
                    throw new Exception("Set_ConfigInterfaceExchangeRateSummit() => " + StrMsg);
                }

                //select RP Confirmation
                ResultWithModel<InterfaceReqExchRateHeaderSummitResult> rwm = new ResultWithModel<InterfaceReqExchRateHeaderSummitResult>();
                //ccmSearch.search_date = DateTime.Today;

                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ImportExchangeRateSSMD");
                InterfaceEnt.InterfaceSSMDExchangeRateSummit.ImportExchangeRateSSMD(reqExchRateHeaderSummitModel, p =>
                {
                    rwm = p;
                });

                if (!rwm.Success)
                {
                    throw new Exception(rwm.Message);
                }

            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "InterfaceSSMDExchangeRate() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InterfaceSSMDExchangeRate ==========");
            }

            return Json(new { Status = "Success", Message = "InterfaceSSMDExchangeRate() " + Model.asof_date + " : Success." });
        }
        private static bool Set_ConfigInterfaceExchangeRateSummit(ref string ReturnMsg, List<RpConfigModel> List_RpConfigModel, ref InterfaceReqExchRateHeaderSummitModel reqExchRateHeaderSummitModel, AdminViewModel Model)
        {
            try
            {
                Utility utility = new Utility();
                //Set Req Data InterfaceMarketPriceModel form config
                DateTime date = DateTime.Now;
                reqExchRateHeaderSummitModel.authorization = List_RpConfigModel.FirstOrDefault(a => a.item_code == "AUTHORIZATION")?.item_value;
                reqExchRateHeaderSummitModel.url_ticket = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_URL_TICKET")?.item_value;
                reqExchRateHeaderSummitModel.url_rate = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_URL_RATE")?.item_value;
                reqExchRateHeaderSummitModel.reqbody = new InterfaceReqExchRateBodySummitModel();
                reqExchRateHeaderSummitModel.reqbody.as_of_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date).ToString("yyyyMMdd"); //20170720
                reqExchRateHeaderSummitModel.reqbody.curve_id = List_RpConfigModel.FirstOrDefault(a => a.item_code == "EXCHANGE_RATE_CURVE_ID")?.item_value;
                reqExchRateHeaderSummitModel.reqbody.data_type = List_RpConfigModel.FirstOrDefault(a => a.item_code == "EXCHANGE_RATE_DATA_TYPE")?.item_value;
                reqExchRateHeaderSummitModel.reqbody.ccy1 = List_RpConfigModel.FirstOrDefault(a => a.item_code == "EXCHANGE_RATE_CCY_1")?.item_value;
                reqExchRateHeaderSummitModel.reqbody.ccy2 = List_RpConfigModel.FirstOrDefault(a => a.item_code == "EXCHANGE_RATE_CCY_2")?.item_value;
                //End Set

                Log.WriteLog(Controller, "Authorization = " + reqExchRateHeaderSummitModel.authorization);
                Log.WriteLog(Controller, "Url Ticket    = " + reqExchRateHeaderSummitModel.url_ticket);
                Log.WriteLog(Controller, "Url Rate      = " + reqExchRateHeaderSummitModel.url_rate);
                Log.WriteLog(Controller, "AsOfDate      = " + reqExchRateHeaderSummitModel.reqbody.as_of_date);
                Log.WriteLog(Controller, "CurveId       = " + reqExchRateHeaderSummitModel.reqbody.curve_id);
                Log.WriteLog(Controller, "DataType      = " + reqExchRateHeaderSummitModel.reqbody.data_type);
                Log.WriteLog(Controller, "ccy1          = " + reqExchRateHeaderSummitModel.reqbody.ccy1);
                Log.WriteLog(Controller, "ccy2          = " + reqExchRateHeaderSummitModel.reqbody.ccy2);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Interface SSMD ExchangeRate(FloatingIndex)

        [HttpPost]
        public JsonResult InterfaceSSMDFloatingIndex(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("InterfaceSSMDFloatingIndex");

                Log.WriteLog(Controller, "Start InterfaceSSMDFloatingIndex ==========");

                //Step 1 : Get Config
                InterfaceFloatingIndexSummitModel FloatingIndexModel = new InterfaceFloatingIndexSummitModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_SSMD_INTERFACE", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
                }

                //Step 2 : Set Config
                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                if (!Set_ConfigInterfaceSSMDFloatingIndex(ref StrMsg, ref FloatingIndexModel, RpConfigModel, Model))
                {
                    throw new Exception("Set_ConfigInterfaceSSMDFloatingIndex() => " + StrMsg);
                }

                //Step 3 : Interface FloatingIndex
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ImportFloatingIndexSSMD");
                ResultWithModel<InterfaceFloatingIndexSummitResult> ResultFloatingIndex = new ResultWithModel<InterfaceFloatingIndexSummitResult>();

                InterfaceEnt.InterfaceFloatingIndexSummit.ImportFloatingIndexSSMD(FloatingIndexModel, p =>
                {
                    ResultFloatingIndex = p;
                });

                if (!ResultFloatingIndex.Success)
                {
                    throw new Exception(ResultFloatingIndex.Message);
                }
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "InterfaceSSMDFloatingIndex() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InterfaceSSMDFloatingIndex ==========");
            }

            return Json(new { Status = "Success", Message = "InterfaceSSMDFloatingIndex() " + Model.asof_date + " : Success." });
        }
        private static bool Set_ConfigInterfaceSSMDFloatingIndex(ref string ReturnMsg, ref InterfaceFloatingIndexSummitModel FloatingIndexModel, List<RpConfigModel> List_RpConfigModel, AdminViewModel Model)
        {
            try
            {
                Utility utility = new Utility();
                FloatingIndexModel.url_ticket = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_URL_TICKET")?.item_value;
                FloatingIndexModel.url_rate = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_URL_RATE")?.item_value;
                FloatingIndexModel.mode = List_RpConfigModel.FirstOrDefault(a => a.item_code == "FLOATING_INDEX_MODE")?.item_value;
                FloatingIndexModel.authorization = List_RpConfigModel.FirstOrDefault(a => a.item_code == "AUTHORIZATION")?.item_value;
                FloatingIndexModel.as_of_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date).ToString("yyyyMMdd"); //20180103
                FloatingIndexModel.curve_id = List_RpConfigModel.FirstOrDefault(a => a.item_code == "FLOATING_INDEX_CURVE_ID")?.item_value;
                FloatingIndexModel.data_type = List_RpConfigModel.FirstOrDefault(a => a.item_code == "FLOATING_INDEX_DATA_TYPE")?.item_value;
                FloatingIndexModel.ccy = List_RpConfigModel.FirstOrDefault(a => a.item_code == "FLOATING_INDEX_CCY")?.item_value;
                FloatingIndexModel.index = string.Empty;

                Log.WriteLog(Controller, "Authorization = " + FloatingIndexModel.authorization);
                Log.WriteLog(Controller, "Url Ticket    = " + FloatingIndexModel.url_ticket);
                Log.WriteLog(Controller, "Url Rate      = " + FloatingIndexModel.url_rate);
                Log.WriteLog(Controller, "Mode          = " + FloatingIndexModel.mode);
                Log.WriteLog(Controller, "AsOfDate      = " + FloatingIndexModel.as_of_date);
                Log.WriteLog(Controller, "CurveId       = " + FloatingIndexModel.curve_id);
                Log.WriteLog(Controller, "DataType      = " + FloatingIndexModel.data_type);
                Log.WriteLog(Controller, "Ccy           = " + FloatingIndexModel.ccy);
                Log.WriteLog(Controller, "Index         = " + FloatingIndexModel.index);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Interface EXRATE ExchangeRate(Counter Rate)

        [HttpPost]
        public JsonResult InterfaceEXRATECounterRate(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("InterfaceEXRATECounterRate");

                Utility utility = new Utility();
                DateTime asOfDate = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date);
                Log.WriteLog(Controller, "Start InterfaceEXRATECounterRate ==========");

                //Step 1 : Get Config
                InterfaceCounterRateExRateModel CounterRateExRateModel = new InterfaceCounterRateExRateModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_EXRATE_INTERFACE_COUNTER_RATE", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
                }

                //Step 2 : Set Config
                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                if (!Set_ConfigInterfaceEXRATECounterRate(ref StrMsg, ref CounterRateExRateModel, RpConfigModel, Model))
                {
                    throw new Exception("Set_ConfigInterfaceEXRATECounterRate() => " + StrMsg);
                }

                //Step 3 : Interface Counter Rate
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ImportCounterRateExRate");
                int CountRound = 0;
                int EndRound = 5;
                int ExRound = 0;
                ResultWithModel<InterfaceCounterRateExRateResult> ResultCounterRateExRate = new ResultWithModel<InterfaceCounterRateExRateResult>();
                do
                {
                    Log.WriteLog(Controller, "ExRound = [" + ExRound.ToString() + "]");
                    Log.WriteLog(Controller, "exDate = [" + CounterRateExRateModel.exDate + "]");
                    if (EndRound == ExRound || EndRound == CountRound)
                    {
                        throw new Exception("CounterRate Not Found And EndRound = [" + EndRound.ToString() + "]");
                    }

                    ResultCounterRateExRate = new ResultWithModel<InterfaceCounterRateExRateResult>();

                    InterfaceEnt.InterfaceCounterRateExRate.ImportCounterRateExRate(CounterRateExRateModel, p =>
                    {
                        ResultCounterRateExRate = p;
                    });

                    if (!ResultCounterRateExRate.Success)
                    {
                        throw new Exception(ResultCounterRateExRate.Message);
                    }

                    ExRound = ResultCounterRateExRate.Data.InterfaceCounterRateExRateResultModel[0].exRound;
                    Log.WriteLog(Controller, "Result ExRound = [" + ExRound.ToString() + "]");
                    Log.WriteLog(Controller, "Result Msg = " + ResultCounterRateExRate.Message + "");
                    Log.WriteLog(Controller, "");
                    CounterRateExRateModel.asof_date = CounterRateExRateModel.asof_date.AddMinutes(-10);
                    CounterRateExRateModel.exDate = CounterRateExRateModel.asof_date.ToString("yyyyMMdd HH:mm");
                    CountRound++;
                } while (ExRound > 1);
            }
            catch (Exception Ex)
            {
                Log.WriteLog(Controller, "Fail " + Ex.Message);
                return Json(new
                {
                    Status = "Error",
                    Message = "InterfaceEXRATECounterRate() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InterfaceEXRATECounterRate ==========");
            }

            return Json(new { Status = "Success", Message = "InterfaceEXRATECounterRate() " + Model.asof_date + " : Success." });
        }
        private static bool Set_ConfigInterfaceEXRATECounterRate(ref string ReturnMsg, ref InterfaceCounterRateExRateModel CounterRateExRateModel, List<RpConfigModel> List_RpConfigModel, AdminViewModel Model)
        {
            try
            {
                Utility utility = new Utility();
                DateTime asOfDate = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date);

                int.TryParse(List_RpConfigModel.FirstOrDefault(a => a.item_code == "EX_ROUND")?.item_value, out var exRound);
                CounterRateExRateModel.exRound = exRound;
                CounterRateExRateModel.exDate = asOfDate.ToString("yyyyMMdd");
                CounterRateExRateModel.exTime = List_RpConfigModel.FirstOrDefault(a => a.item_code == "EX_TIME")?.item_value;

                asOfDate = DateTime.ParseExact(CounterRateExRateModel.exDate + " " + CounterRateExRateModel.exTime, "yyyyMMdd HH:mm", CultureInfo.InvariantCulture);
                CounterRateExRateModel.asof_date = asOfDate;
                CounterRateExRateModel.channel = List_RpConfigModel.FirstOrDefault(a => a.item_code == "CHANNEL")?.item_value;
                CounterRateExRateModel.exCurrency = List_RpConfigModel.FirstOrDefault(a => a.item_code == "CURRENCY")?.item_value;
                CounterRateExRateModel.serviceID = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_ID")?.item_value;
                CounterRateExRateModel.ServiceUrl = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_URL")?.item_value;
                CounterRateExRateModel.ServiceType = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_TYPE")?.item_value;
                CounterRateExRateModel.ApiAuthenUrl = List_RpConfigModel.FirstOrDefault(a => a.item_code == "API_AUTHEN_URL")?.item_value;
                CounterRateExRateModel.ApiRateUrl = List_RpConfigModel.FirstOrDefault(a => a.item_code == "API_RATE_URL")?.item_value;
                CounterRateExRateModel.ApiUsername = List_RpConfigModel.FirstOrDefault(a => a.item_code == "API_USERNAME")?.item_value;
                CounterRateExRateModel.ApiPassword = List_RpConfigModel.FirstOrDefault(a => a.item_code == "API_PASSWORD")?.item_value;

                if (!String.IsNullOrEmpty(List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_TIMEOUT")?.item_value))
                {
                    CounterRateExRateModel.ServiceTimeOut = System.Convert.ToInt32(List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_TIMEOUT")?.item_value);
                }
                else
                {
                    CounterRateExRateModel.ServiceTimeOut = 600000;
                }

                Log.WriteLog(Controller, "ExRound       = " + CounterRateExRateModel.exRound);
                Log.WriteLog(Controller, "ExDate       = " + CounterRateExRateModel.exDate);
                Log.WriteLog(Controller, "ExTime       = " + CounterRateExRateModel.exTime);
                Log.WriteLog(Controller, "Channel      = " + CounterRateExRateModel.channel);
                Log.WriteLog(Controller, "ExCurrency   = " + CounterRateExRateModel.exCurrency);
                Log.WriteLog(Controller, "ServiceID    = " + CounterRateExRateModel.serviceID);
                Log.WriteLog(Controller, "ServiceUrl   = " + CounterRateExRateModel.ServiceUrl);
                Log.WriteLog(Controller, "ServiceType   = " + CounterRateExRateModel.ServiceType);
                Log.WriteLog(Controller, "ApiAuthenUrl   = " + CounterRateExRateModel.ApiAuthenUrl);
                Log.WriteLog(Controller, "ApiRateUrl   = " + CounterRateExRateModel.ApiRateUrl);

            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Interface EXRATE ExchangeRate(Mid & Valuation Rate)

        [HttpPost]
        public JsonResult InterfaceEXRATEMidRate(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("InterfaceEXRATEMidRate");

                Log.WriteLog(Controller, "Start InterfaceEXRATECounterRate ==========");

                InterfaceMidValuationRateExRateModel MidValuationExRateModel = new InterfaceMidValuationRateExRateModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_EXRATE_INTERFACE_MID_VALUATION_RATE", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
                }

                //Step 2 : Set Config
                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;

                if (!Set_ConfigInterfaceEXRATEMidValuation(ref StrMsg, ref MidValuationExRateModel, RpConfigModel, Model, "MIDRATE"))
                {
                    throw new Exception("Set_ConfigInterfaceEXRATEMidValuation() => " + StrMsg);
                }

                //Step 3 : Interface MidValuation Rate
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ImportMidValuationExRate");
                ResultWithModel<InterfaceMidValuationRateExRateResult> ResultMidValExRate = new ResultWithModel<InterfaceMidValuationRateExRateResult>();

                InterfaceEnt.InterfaceMidValuationExRate.ImportMidValuationExRate(MidValuationExRateModel, p =>
                {
                    ResultMidValExRate = p;
                });

                if (!ResultMidValExRate.Success)
                {
                    throw new Exception(ResultMidValExRate.Message);
                }
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "InterfaceEXRATEMidRate() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InterfaceEXRATECounterRate ==========");
            }

            return Json(new { Status = "Success", Message = "InterfaceEXRATEMidRate() " + Model.asof_date + " : Success." });
        }
        [HttpPost]
        public JsonResult InterfaceValuationRate(AdminViewModel Model)
        {
            string StrMsg = string.Empty;
            try
            {
                UpdateCheckingEod("InterfaceEXRATEValuationRate");

                Log.WriteLog(Controller, "Start InterfaceValuationRate ==========");

                //Step 1 : Get Config
                InterfaceMidValuationRateExRateModel MidValuationExRateModel = new InterfaceMidValuationRateExRateModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_EXRATE_INTERFACE_MID_VALUATION_RATE", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
                }

                //Step 2 : Set Config
                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;

                if (!Set_ConfigInterfaceEXRATEMidValuation(ref StrMsg, ref MidValuationExRateModel, RpConfigModel, Model, "VALUATIONRATE"))
                {
                    throw new Exception("Set_ConfigInterfaceEXRATEMidValuation() => " + StrMsg);
                }

                //Step 3 : Interface MidValuation Rate
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ImportMidValuationExRate");
                ResultWithModel<InterfaceMidValuationRateExRateResult> ResultMidValExRate = new ResultWithModel<InterfaceMidValuationRateExRateResult>();

                InterfaceEnt.InterfaceMidValuationExRate.ImportMidValuationExRate(MidValuationExRateModel, p =>
                {
                    ResultMidValExRate = p;
                });

                if (!ResultMidValExRate.Success)
                {
                    throw new Exception(ResultMidValExRate.Message);
                }
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "InterfaceValuationRate() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InterfaceValuationRate ==========");
            }

            return Json(new { Status = "Success", Message = "InterfaceValuationRate() " + Model.asof_date + " : Success." });
        }
        private static bool Set_ConfigInterfaceEXRATEMidValuation(ref string ReturnMsg, ref InterfaceMidValuationRateExRateModel MidValuationExRateModel, List<RpConfigModel> List_RpConfigModel, AdminViewModel Model, string Type)
        {
            try
            {
                Utility utility = new Utility();
                DateTime asOfDate = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date);

                MidValuationExRateModel.asof_date = asOfDate.Date;
                MidValuationExRateModel.exDate = asOfDate.ToString("yyyyMMdd");
                MidValuationExRateModel.channel = List_RpConfigModel.FirstOrDefault(a => a.item_code == "CHANNEL")?.item_value;
                MidValuationExRateModel.exCurrency = List_RpConfigModel.FirstOrDefault(a => a.item_code == "CURRENCY")?.item_value;
                MidValuationExRateModel.serviceID = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_ID")?.item_value;
                MidValuationExRateModel.ServiceUrl = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_URL")?.item_value;
                MidValuationExRateModel.ServiceType = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_TYPE")?.item_value;
                MidValuationExRateModel.ApiAuthenUrl = List_RpConfigModel.FirstOrDefault(a => a.item_code == "API_AUTHEN_URL")?.item_value;
                MidValuationExRateModel.ApiRateUrl = List_RpConfigModel.FirstOrDefault(a => a.item_code == "API_RATE_URL")?.item_value;
                MidValuationExRateModel.ApiUsername = List_RpConfigModel.FirstOrDefault(a => a.item_code == "API_USERNAME")?.item_value;
                MidValuationExRateModel.ApiPassword = List_RpConfigModel.FirstOrDefault(a => a.item_code == "API_PASSWORD")?.item_value;
                MidValuationExRateModel.type = Type;

                if (!String.IsNullOrEmpty(List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_TIMEOUT")?.item_value))
                {
                    MidValuationExRateModel.ServiceTimeOut = System.Convert.ToInt32(List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_TIMEOUT")?.item_value);
                }
                else
                {
                    MidValuationExRateModel.ServiceTimeOut = 600000;
                }

                Log.WriteLog(Controller, "ExDate       = " + MidValuationExRateModel.exDate);
                Log.WriteLog(Controller, "Channel      = " + MidValuationExRateModel.channel);
                Log.WriteLog(Controller, "ExCurrency   = " + MidValuationExRateModel.exCurrency);
                Log.WriteLog(Controller, "ServiceID    = " + MidValuationExRateModel.serviceID);
                Log.WriteLog(Controller, "ServiceUrl   = " + MidValuationExRateModel.ServiceUrl);
                Log.WriteLog(Controller, "ServiceType   = " + MidValuationExRateModel.ServiceType);
                Log.WriteLog(Controller, "ApiAuthenUrl   = " + MidValuationExRateModel.ApiAuthenUrl);
                Log.WriteLog(Controller, "ApiRateUrl   = " + MidValuationExRateModel.ApiRateUrl);
                Log.WriteLog(Controller, "Type         = " + MidValuationExRateModel.type);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Interface EQUITY Pledge

        [HttpPost]
        public JsonResult InterfaceEQUITYPledge(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("InterfaceEQUITYPledge");

                Log.WriteLog(Controller, "Start InterfaceEQUITYPledge ==========");
                // Step 2 : Get Config
                InterfaceEquityPledgeModel equityPledgeModel = new InterfaceEquityPledgeModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_EQUITY_INTERFACE_PLEDGE", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
                }

                // Step 3 : Set Config
                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                if (!Set_ConfigInterfaceEQUITYPledge(ref StrMsg, ref equityPledgeModel, RpConfigModel, Model))
                {
                    throw new Exception("Set_ConfigInterfaceEQUITYPledge() => " + StrMsg);
                }

                // Step 4 : Interface FITS BondPledge
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run InterfaceEQUITYPledge");
                ResultWithModel<InterfaceEquityPledgeResult> resultEquityPledge = new ResultWithModel<InterfaceEquityPledgeResult>();

                InterfaceEnt.InterfaceEquityPledge.InterfaceEquityPledge(equityPledgeModel, p =>
                {
                    resultEquityPledge = p;
                });

                if (!resultEquityPledge.Success)
                {
                    throw new Exception(resultEquityPledge.Message);
                }

            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "InterfaceEQUITYPledge() " +
                                  Model.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InterfaceEQUITYPledge ==========");
            }

            return Json(new { Status = "Success", Message = "InterfaceEQUITYPledge() " + Model.asof_date + " : Success." });
        }
        private static bool Set_ConfigInterfaceEQUITYPledge(ref string StrMsg, ref InterfaceEquityPledgeModel equityPledgeModel, List<RpConfigModel> List_RpConfigModel, AdminViewModel Model)
        {
            try
            {
                Utility utility = new Utility();
                equityPledgeModel.RpConfigModel = List_RpConfigModel;
                equityPledgeModel.ServiceUrl = List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_URL")?.item_value;
                if (!String.IsNullOrEmpty(List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_TIMEOUT")?.item_value))
                {
                    equityPledgeModel.ServiceTimeOut = System.Convert.ToInt32(List_RpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_TIMEOUT")?.item_value);
                }

                equityPledgeModel.AsOfDate = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date).Date;
                equityPledgeModel.create_by = "Console";
            }
            catch (Exception Ex)
            {
                StrMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : InterfaceThorRate

        [HttpPost]
        public JsonResult InterfaceThorRate(AdminViewModel adminModel)
        {
            string strMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("InterfaceThorRate");

                Log.WriteLog(Controller, "Start InterfaceThorRate ==========");

                //Step 1 : Get Config
                InterfaceReqThorRateModel model = new InterfaceReqThorRateModel();
                ResultWithModel<RpConfigResult> resultRPConfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> rpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_SSMD_INTERFACE_THOR", string.Empty, p =>
                {
                    resultRPConfig = p;
                });

                if (!resultRPConfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + resultRPConfig.RefCode.ToString() + "] " + resultRPConfig.Message);
                }

                //Step 2 : Set Config
                rpConfigModel = resultRPConfig.Data.RpConfigResultModel;
                if (!Set_ConfigInterfaceThorRate(ref strMsg, ref model, rpConfigModel, adminModel))
                {
                    throw new Exception("Set_ConfigInterfaceThorRate() => " + strMsg);
                }

                //Step 3 : Interface ImportThorIndex
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ImportThorIndexSSMD");
                ResultWithModel<InterfaceThorRateResult> res = new ResultWithModel<InterfaceThorRateResult>();

                InterfaceEnt.InterfaceThorRate.ImportThorRateSSMD(model, p =>
                {
                    res = p;
                });

                if (!res.Success)
                {
                    throw new Exception(res.Message);
                }
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "InterfaceThorRate() " +
                                  adminModel.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InterfaceThorRate ==========");
            }

            return Json(new { Status = "Success", Message = "InterfaceThorRate() " + adminModel.asof_date + " : Success." });
        }
        private static bool Set_ConfigInterfaceThorRate(ref string ReturnMsg, ref InterfaceReqThorRateModel model, List<RpConfigModel> lstRpConfigModel, AdminViewModel adminModel)
        {
            try
            {
                Utility utility = new Utility();
                model.url_ticket = lstRpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_URL_TICKET")?.item_value;
                model.url_rate = lstRpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_URL_RATE")?.item_value;
                model.mode = lstRpConfigModel.FirstOrDefault(a => a.item_code == "MODE")?.item_value;
                model.authorization = lstRpConfigModel.FirstOrDefault(a => a.item_code == "AUTHORIZATION")?.item_value;
                if (lstRpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_TIMEOUT") != null)
                {
                    model.time_out = System.Convert.ToInt32(lstRpConfigModel.FirstOrDefault(a => a.item_code == "SERVICE_TIMEOUT").item_value);
                }

                model.reqBody.as_of_date = utility.ConvertStringToDatetimeFormatDDMMYYYY(adminModel.asof_date).ToString("yyyyMMdd");
                model.reqBody.curve_id = lstRpConfigModel.FirstOrDefault(a => a.item_code == "CURVE_ID")?.item_value;
                model.reqBody.data_type = lstRpConfigModel.FirstOrDefault(a => a.item_code == "DATA_TYPE")?.item_value;
                model.reqBody.ccy = lstRpConfigModel.FirstOrDefault(a => a.item_code == "CCY")?.item_value;
                model.reqBody.index = lstRpConfigModel.FirstOrDefault(a => a.item_code == "INDEX")?.item_value;

                Log.WriteLog(Controller, "Authorization = " + model.authorization);
                Log.WriteLog(Controller, "Url Ticket    = " + model.url_ticket);
                Log.WriteLog(Controller, "Url Rate      = " + model.url_rate);
                Log.WriteLog(Controller, "Mode          = " + model.mode);
                Log.WriteLog(Controller, "AsOfDate      = " + model.reqBody.as_of_date);
                Log.WriteLog(Controller, "CurveId       = " + model.reqBody.curve_id);
                Log.WriteLog(Controller, "DataType      = " + model.reqBody.data_type);
                Log.WriteLog(Controller, "Ccy           = " + model.reqBody.ccy);
                Log.WriteLog(Controller, "Index         = " + model.reqBody.index);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : InterfaceThorIndex

        [HttpPost]
        public JsonResult InterfaceThorIndex(AdminViewModel adminModel)
        {
            string strMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("InterfaceThorIndex");

                Log.WriteLog(Controller, "Start InterfaceThorIndex ==========");

                //Step 1 : Get Config
                InterfaceReqThorIndexFitsModel model = new InterfaceReqThorIndexFitsModel();
                ResultWithModel<RpConfigResult> resultRPConfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> rpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_FITS_INTERFACE_THOR_INDEX", string.Empty, p =>
                {
                    resultRPConfig = p;
                });

                if (!resultRPConfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + resultRPConfig.RefCode.ToString() + "] " + resultRPConfig.Message);
                }

                //Step 2 : Set Config
                rpConfigModel = resultRPConfig.Data.RpConfigResultModel;
                if (!Set_ConfigInterfaceThorIndex(ref strMsg, ref model, rpConfigModel, adminModel))
                {
                    throw new Exception("Set_ConfigInterfaceThorIndex() => " + strMsg);
                }

                //Step 3 : Interface ImportThorIndex
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ImportThorIndexFits");
                ResultWithModel<InterfaceThorIndexResult> res = new ResultWithModel<InterfaceThorIndexResult>();

                InterfaceEnt.InterfaceThorIndexFits.ImportThorIndexFits(model, p =>
                {
                    res = p;
                });

                if (!res.Success)
                {
                    throw new Exception(res.Message);
                }
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "InterfaceThorIndex() " +
                                  adminModel.asof_date +
                                  " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InterfaceThorIndex ==========");
            }

            return Json(new { Status = "Success", Message = "InterfaceThorIndex() " + adminModel.asof_date + " : Success." });
        }
         
        private static bool Set_ConfigInterfaceThorIndex(ref string returnMsg, ref InterfaceReqThorIndexFitsModel model, List<RpConfigModel> lstRpConfigModel, AdminViewModel adminModel)
        {
            try
            {
                Utility utility = new Utility();
                model.RPConfigModel = lstRpConfigModel;
                model.AsOfDate = utility.ConvertStringToDatetimeFormatDDMMYYYY(adminModel.asof_date);
                model.create_by = "Rerun";
                Log.WriteLog(Controller, "RPConfigModel = " + JsonConvert.SerializeObject(model.RPConfigModel));
            }
            catch (Exception Ex)
            {
                returnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion


        #region Function : Export AmendCancelDaily Mail

        [HttpPost]
        public JsonResult ExportAmendCancelDaily(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("ExportAmendCancelDailyToEmail");

                Log.WriteLog(Controller, "Start ExportAmendCancelDaily ==========");

                //Step 1 : Get Config
                ExportAmendCancelDailyMailModel AmendCancelDailyMailModel = new ExportAmendCancelDailyMailModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_AMEND_CANCEL_DAILY_MAIL", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
                }

                //Step 2 : Set Config
                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                AmendCancelDailyMailModel.RpConfigModel = RpConfigModel;
                AmendCancelDailyMailModel.create_by = ConsoleEnt.CreateBy;
                if (!Set_ConfigAmendCancelDailyToEmail(ref StrMsg, ref AmendCancelDailyMailModel, RpConfigModel, Model))
                {
                    throw new Exception("Set_ConfigAmendCancelDailyToEmail() => " + StrMsg);
                }

                //Step 3 : Export AmendCancel Daily To Email
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ExportAmendCancelDailyMail");
                ResultWithModel<ExportAmendCancelDailyMailResult> ResultAmendCancelDailyMail = new ResultWithModel<ExportAmendCancelDailyMailResult>();

                InterfaceEnt.ExportAmendCancel.ExportAmendCancelDailyMail(AmendCancelDailyMailModel, p =>
                {
                    ResultAmendCancelDailyMail = p;
                });

                if (!ResultAmendCancelDailyMail.Success)
                {
                    throw new Exception(ResultAmendCancelDailyMail.Message);
                }
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "ExportAmendCancelDaily() " +
                    Model.asof_date +
                    " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End ExportAmendCancelDaily ==========");
            }

            return Json(new { Status = "Success", Message = "ExportAmendCancelDaily() " + Model.asof_date + " : Success." });
        }
        private static bool Set_ConfigAmendCancelDailyToEmail(ref string ReturnMsg, ref ExportAmendCancelDailyMailModel AmendCancelDailyModel, List<RpConfigModel> List_RpConfigModel, AdminViewModel Model)
        {
            try
            {
                Utility utility = new Utility();
                DateTime asOfDate = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date);

                AmendCancelDailyModel.AsofDate = asOfDate.Date;
                Log.WriteLog(Controller, "AsofDate = " + AmendCancelDailyModel.AsofDate.ToString("yyyyMMdd"));
                Log.WriteLog(Controller, "File Name = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "FILE_NAME")?.item_value);
                Log.WriteLog(Controller, "MAIL_SERVER  = " + AmendCancelDailyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_SERVER")?.item_value);
                Log.WriteLog(Controller, "MAIL_PORT    = " + AmendCancelDailyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_PORT")?.item_value);
                Log.WriteLog(Controller, "MAIL_SENDER  = " + AmendCancelDailyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_SENDER")?.item_value);
                Log.WriteLog(Controller, "MAIL_TO      = " + AmendCancelDailyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_TO")?.item_value);
                Log.WriteLog(Controller, "MAIL_CC      = " + AmendCancelDailyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_CC")?.item_value);
                Log.WriteLog(Controller, "MAIL_SUBJECT = " + AmendCancelDailyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_SUBJECT")?.item_value);
                Log.WriteLog(Controller, "MAIL_BODY    = " + AmendCancelDailyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_BODY")?.item_value);
                Log.WriteLog(Controller, "MAIL_ADMIN   = " + AmendCancelDailyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_ADMIN")?.item_value);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Export AmendCancelMonthly Mail

        [HttpPost]
        public JsonResult ExportAmendCancelMonthly(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("ExportAmendCancelMonthlyToEmail");

                Log.WriteLog(Controller, "Start ExportAmendCancelMonthly ==========");

                //Step 1 : Get Config
                ExportAmendCancelMonthlyMailModel AmendCancelMonthlyMailModel = new ExportAmendCancelMonthlyMailModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_AMEND_CANCEL_MONTHLY_MAIL", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
                }

                //Step 2 : Set Config
                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                AmendCancelMonthlyMailModel.RpConfigModel = RpConfigModel;
                AmendCancelMonthlyMailModel.create_by = ConsoleEnt.CreateBy;
                if (!Set_ConfigAmendCancelMonthlyToEmail(ref StrMsg, ref AmendCancelMonthlyMailModel, RpConfigModel, Model))
                {
                    throw new Exception("Set_ConfigAmendCancelMonthlyToEmail() => " + StrMsg);
                }

                //Step 3 : Export AmendCancel Daily To Email
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ExportAmendCancelDailyMail");
                ResultWithModel<ExportAmendCancelMonthlyMailResult> ResultAmendCancelMonthlyMail = new ResultWithModel<ExportAmendCancelMonthlyMailResult>();

                InterfaceEnt.ExportAmendCancel.ExportAmendCancelMonthlyMail(AmendCancelMonthlyMailModel, p =>
                {
                    ResultAmendCancelMonthlyMail = p;
                });

                if (!ResultAmendCancelMonthlyMail.Success)
                {
                    throw new Exception(ResultAmendCancelMonthlyMail.Message);
                }

            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "ExportAmendCancelMonthly() " +
                    Model.asof_date +
                    " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End ExportAmendCancelMonthly ==========");
            }

            return Json(new { Status = "Success", Message = "ExportAmendCancelMonthly() " + Model.asof_date + " : Success." });
        }
        private static bool Set_ConfigAmendCancelMonthlyToEmail(ref string ReturnMsg, ref ExportAmendCancelMonthlyMailModel AmendCancelMonthlyModel, List<RpConfigModel> List_RpConfigModel, AdminViewModel Model)
        {
            try
            {
                Utility utility = new Utility();
                DateTime asOfDate = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date);

                AmendCancelMonthlyModel.AsofDate = asOfDate.Date;
                AmendCancelMonthlyModel.Monthly = asOfDate.ToString("yyyyMM");

                Log.WriteLog(Controller, "Monthly = " + AmendCancelMonthlyModel.Monthly);
                Log.WriteLog(Controller, "File Name = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "FILE_NAME")?.item_value);
                Log.WriteLog(Controller, "MAIL_SERVER  = " + AmendCancelMonthlyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_SERVER")?.item_value);
                Log.WriteLog(Controller, "MAIL_PORT    = " + AmendCancelMonthlyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_PORT")?.item_value);
                Log.WriteLog(Controller, "MAIL_SENDER  = " + AmendCancelMonthlyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_SENDER")?.item_value);
                Log.WriteLog(Controller, "MAIL_TO      = " + AmendCancelMonthlyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_TO")?.item_value);
                Log.WriteLog(Controller, "MAIL_CC      = " + AmendCancelMonthlyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_CC")?.item_value);
                Log.WriteLog(Controller, "MAIL_SUBJECT = " + AmendCancelMonthlyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_SUBJECT")?.item_value);
                Log.WriteLog(Controller, "MAIL_BODY    = " + AmendCancelMonthlyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_BODY")?.item_value);
                Log.WriteLog(Controller, "MAIL_ADMIN   = " + AmendCancelMonthlyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_ADMIN")?.item_value);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Export UserProfileMonthly Mail

        [HttpPost]
        public JsonResult ExportUserProfileMonthly(AdminViewModel Model)
        {
            string StrMsg = string.Empty;

            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("ExportUserProfileMonthlyToEmail");

                Log.WriteLog(Controller, "Start ExportUserProfileMonthly ==========");

                //Step 1 : Get Config
                ExportUserProfileMonthlyMailModel UserProfileMonthlyMailModel = new ExportUserProfileMonthlyMailModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_USER_PROFILE_MONTHLY_MAIL", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
                }

                //Step 2 : Set Config
                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                UserProfileMonthlyMailModel.RpConfigModel = RpConfigModel;
                UserProfileMonthlyMailModel.create_by = ConsoleEnt.CreateBy;
                if (!Set_ConfigUserProfileMonthlyToEmail(ref StrMsg, ref UserProfileMonthlyMailModel, RpConfigModel, Model))
                {
                    throw new Exception("Set_ConfigUserProfileMonthlyToEmail() => " + StrMsg);
                }

                //Step 3 : Export UserProfile Monthly To Email
                Log.WriteLog(Controller, "");
                Log.WriteLog(Controller, "Run ExportUserProfileMonthlyMail");
                ResultWithModel<ExportUserProfileMonthlyMailResult> ResultUserProfileMonthlyMail = new ResultWithModel<ExportUserProfileMonthlyMailResult>();

                InterfaceEnt.ExportUserProfile.ExportUserProfileMonthlyMail(UserProfileMonthlyMailModel, p =>
                {
                    ResultUserProfileMonthlyMail = p;
                });

                if (!ResultUserProfileMonthlyMail.Success)
                {
                    throw new Exception(ResultUserProfileMonthlyMail.Message);
                }

            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "ExportUserProfileMonthly() " +
                    Model.asof_date +
                    " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End ExportUserProfileMonthly ==========");
            }

            return Json(new { Status = "Success", Message = "ExportUserProfileMonthly() " + Model.asof_date + " : Success." });
        }
        private static bool Set_ConfigUserProfileMonthlyToEmail(ref string ReturnMsg, ref ExportUserProfileMonthlyMailModel UserProfileMonthlyModel, List<RpConfigModel> List_RpConfigModel, AdminViewModel Model)
        {
            try
            {
                Utility utility = new Utility();
                DateTime asOfDate = utility.ConvertStringToDatetimeFormatDDMMYYYY(Model.asof_date);

                UserProfileMonthlyModel.AsofDate = asOfDate.Date;
                UserProfileMonthlyModel.Monthly = asOfDate.ToString("yyyyMM");

                Log.WriteLog(Controller, "Monthly = " + UserProfileMonthlyModel.Monthly);
                Log.WriteLog(Controller, "File Name = " + List_RpConfigModel.FirstOrDefault(a => a.item_code == "FILE_NAME")?.item_value);
                Log.WriteLog(Controller, "MAIL_SERVER  = " + UserProfileMonthlyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_SERVER")?.item_value);
                Log.WriteLog(Controller, "MAIL_PORT    = " + UserProfileMonthlyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_PORT")?.item_value);
                Log.WriteLog(Controller, "MAIL_SENDER  = " + UserProfileMonthlyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_SENDER")?.item_value);
                Log.WriteLog(Controller, "MAIL_TO      = " + UserProfileMonthlyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_TO")?.item_value);
                Log.WriteLog(Controller, "MAIL_CC      = " + UserProfileMonthlyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_CC")?.item_value);
                Log.WriteLog(Controller, "MAIL_SUBJECT = " + UserProfileMonthlyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_SUBJECT")?.item_value);
                Log.WriteLog(Controller, "MAIL_BODY    = " + UserProfileMonthlyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_BODY")?.item_value);
                Log.WriteLog(Controller, "MAIL_ADMIN   = " + UserProfileMonthlyModel.RpConfigModel.FirstOrDefault(a => a.item_code == "MAIL_ADMIN")?.item_value);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Function : Export CheckingEOD Mail

        [HttpPost]
        public JsonResult CheckingEOD()
        {
            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RP_CHECKING_EOD_MAIL", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
                }

                ResultWithModel<InterfaceCheckingEodResult> result = new ResultWithModel<InterfaceCheckingEodResult>();
                InterfaceCheckingEodModel CheckingEodModel = new InterfaceCheckingEodModel();

                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                CheckingEodModel.RpConfigModel = RpConfigModel;
                InterfaceEnt.InterfaceCheckingEod.CheckingEodList(CheckingEodModel, p =>
                {
                    result = p;

                });

                if (result.Success)
                {
                    return Json(new { Status = "Success", Message = " Success." });
                }
                else
                {
                    return Json(new { Status = "Error", Message = " Error!!! " + result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error", Message = " Error!!! " + ex.Message });
            }
        }

        #endregion

        #region Function : Internal BatchJobEod(Gen GL)

        [HttpPost]
        public JsonResult InternalBatchJobEod(AdminViewModel Model)
        {
            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("InternalBatchJobEod");

                Log.WriteLog(Controller, "Start InternalBatchJobEod ==========");


                InternalJobModel InternalJobModel = new InternalJobModel();
                InternalJobModel.AsofDate = Model.asof_date;

                ResultWithModel<InternalJobResult> ResultInternalJob = new ResultWithModel<InternalJobResult>();
                InterfaceEnt.InternalJob.InternalBatchJobEod(InternalJobModel, p =>
                {
                    ResultInternalJob = p;
                });

                if (!ResultInternalJob.Success)
                {
                    throw new Exception("InternalBatchJobEod() => [" + ResultInternalJob.RefCode + "] " + ResultInternalJob.Message);
                }

            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "InternalBatchJobEod() " +
                    Model.asof_date +
                    " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InternalBatchJobEod ==========");
            }

            return Json(new { Status = "Success", Message = "InternalBatchJobEod() : Success." });
        }

        #endregion

        #region Function : Internal BatchJobEndOfDay(Next Business Date)

        [HttpPost]
        public JsonResult InternalBatchJobEndOfDay(AdminViewModel Model)
        {
            if (!IsCheckPermission())
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                UpdateCheckingEod("InternalBatchJobEndOfDay");

                Log.WriteLog(Controller, "Start InternalBatchJobEndOfDay ==========");

                InternalJobModel InternalJobModel = new InternalJobModel();
                InternalJobModel.AsofDate = Model.asof_date;

                ResultWithModel<InternalJobResult> ResultInternalJob = new ResultWithModel<InternalJobResult>();
                InterfaceEnt.InternalJob.InternalBatchJobEndOfDay(InternalJobModel, p =>
                {
                    ResultInternalJob = p;
                });

                if (!ResultInternalJob.Success)
                {
                    throw new Exception(ResultInternalJob.Message);
                }
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    Status = "Error",
                    Message = "InternalBatchJobEndOfDay() " +
                    Model.asof_date +
                    " => Fail " + Ex.Message
                });
            }
            finally
            {
                Log.WriteLog(Controller, "End InternalBatchJobEndOfDay ==========");
            }

            return Json(new { Status = "Success", Message = "InternalBatchJobEod() : Next Business Date Success." });
        }

        #endregion

        private void UpdateCheckingEod(string taskName)
        {
            InterfaceEnt.InterfaceCheckingEod.UpdateCheckingEod(new InterfaceCheckingEodModel() { task_name = taskName }, p =>
            {
            });
        }

        #region TestPage
        [HttpPost]
        public JsonResult ExecuteQuery(string TextQuery)
        {
            if (!IsCheckPermission() && User.WizardPage != "Y")
            {
                return Json(new { Status = "Error", Message = "no permission" });
            }

            try
            {
                DateTime startDate = DateTime.Now;

                if (string.IsNullOrEmpty(TextQuery))
                {
                    return Json(new { Status = "Error", Message = "no query" });
                }

                ResultWithModel<DataSet> result = ExecuteTestPage(TextQuery);

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

                        return Json(new { Status = "Success", Message = string.Format("Success. Total Data: {0} rows ({1} s)", dataRows.Count, DiffInSeconds(startDate, DateTime.Now)), Data = dataRows, Columns = columns },
                            "application/json",
                            Encoding.UTF8,
                            JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { Status = "Success", Message = string.Format("Success. ({0} s)", DiffInSeconds(startDate, DateTime.Now)), Data = new object[] { }, Columns = new object[] { } },
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
        public ActionResult ExportQueryExcel(string TextQuery)
        {
            if (!IsCheckPermission() && User.WizardPage != "Y")
            {
                return Content("no permission", "text/html");
            }

            try
            {
                if (string.IsNullOrEmpty(TextQuery))
                {
                    return Content("no query", "text/html");
                }

                ResultWithModel<DataSet> result = ExecuteTestPage(TextQuery);

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
                                if (col.DataType == typeof(DateTime))
                                {
                                    DateTime date;
                                    bool isConvert = DateTime.TryParse(row[col].ToString().Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                                    if (isConvert)
                                    {
                                        excelTemplate.CreateCellColLeft(excelRow, colIndex, date.ToString("yyyy-MM-dd HH:mm:ss"));
                                    }
                                    else
                                    {
                                        excelTemplate.CreateCellColLeft(excelRow, colIndex, string.Empty);
                                    }
                                }
                                else
                                {
                                    excelTemplate.CreateCellColLeft(excelRow, colIndex, row[col].ToString());
                                }
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
                            else if (colWidth > 24800)
                            {
                                sheet.SetColumnWidth(i, 25000);
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
                        Response.Headers.Add("Content-disposition", "attachment; filename=result_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");

                        Response.BinaryWrite(exportfile.GetBuffer());
                        Response.End();
                        return View("WizardQuery");
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

        [HttpPost]
        public ActionResult ExportQuerySQL(string TextQuery)
        {
            if (!IsCheckPermission() && User.WizardPage != "Y")
            {
                return Content("no permission", "text/html");
            }

            try
            {
                if (string.IsNullOrEmpty(TextQuery))
                {
                    return Content("no query", "text/html");
                }

                ResultWithModel<DataSet> result = ExecuteTestPage(TextQuery);

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
                        Response.AddHeader("content-disposition", "attachment;    filename=result.sql");
                        Response.BinaryWrite(bytes);
                        Response.End();
                        return View("WizardQuery");
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

        private ResultWithModel<DataSet> ExecuteTestPage(string text)
        {
            ResultWithModel<DataSet> result = new ResultWithModel<DataSet>();
            TestPageModel model = new TestPageModel();
            model.text = text;
            model.create_by = User.UserId;

            StaticEnt.TestPage.GetTestPageList(model, p =>
            {
                result = p;
            });

            return result;
        }

        #endregion

        private bool IsCheckPermission()
        {
            if (User.RoleName != "Administrator")
            {
                return false;
            }

            return true;
        }

        private double DiffInSeconds(DateTime startTime, DateTime endTime)
        {
            return (endTime - startTime).TotalSeconds;
        }
    }
}