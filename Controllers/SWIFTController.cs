using GM.CommonLibs;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.Static;
using GM.Data.Result.Static;
using GM.Data.View.Static;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using GM.CommonLibs.Constants;
using GM.Data.Model.ExternalInterface;
using GM.Data.Result.ExternalInterface;
using GM.Filters;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class SWIFTController : BaseController
    {
        private static string Controller = "SWIFTController";
        private static LogFile Log = new LogFile();

        Utility utility = new Utility();

        // GET: SWIFT
        public ActionResult Index()
        {
            string StrJson = string.Empty;

            if (!IsCheckPermission())
            {

                return RedirectToAction("Index", "Home");
            }

            ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
            StaticEntities StaticEnt = new StaticEntities();
            List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

            StaticEnt.RpConfig.GetRpConfig("RELEASE_MSG", string.Empty, p =>
            {
                ResultRpconfig = p;
            });

            if (!ResultRpconfig.Success)
            {
                throw new Exception("GetRpConfig() => [" + ResultRpconfig.RefCode.ToString() + "] " + ResultRpconfig.Message);
            }

            RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
            StringBuilder sb = new StringBuilder();

            sb.Append("IP = " + RpConfigModel.FirstOrDefault(a => a.item_code == "IP")?.item_value);
            sb.AppendLine();
            sb.Append("PORT = " + RpConfigModel.FirstOrDefault(a => a.item_code == "PORT")?.item_value);
            sb.AppendLine();
            sb.Append("USER = " + RpConfigModel.FirstOrDefault(a => a.item_code == "USER")?.item_value);
            sb.AppendLine();
            sb.Append("PASSWORD = " + RpConfigModel.FirstOrDefault(a => a.item_code == "PASSWORD")?.item_value);
            sb.AppendLine();
            sb.Append("PATH_SFTP_REPO_OUT = " + RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SFTP_REPO_OUT")?.item_value);
            sb.AppendLine();
            sb.Append("PATH_SFTP_REPO_BACKOUT = " + RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SFTP_REPO_BACKOUT")?.item_value);
            sb.AppendLine();
            sb.Append("PATH_WEB = " + RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_WEB")?.item_value);
            sb.AppendLine();
            sb.Append("SSHPRIVATEKEYPATH = " + RpConfigModel.FirstOrDefault(a => a.item_code == "SSHPRIVATEKEYPATH")?.item_value);
            sb.AppendLine();
            sb.Append("SSHHOSTKEYFINGERPRINT = " + RpConfigModel.FirstOrDefault(a => a.item_code == "SSHHOSTKEYFINGERPRINT")?.item_value);
            sb.AppendLine();
            sb.Append("FAIL_RETRY = " + RpConfigModel.FirstOrDefault(a => a.item_code == "FAIL_RETRY")?.item_value);
            sb.AppendLine();

            StrJson = sb.ToString();

            SWIFTViewModel model = new SWIFTViewModel();
            model.config_swift = StrJson;

            return View(model);
        }

        [HttpPost]
        public JsonResult Index(SWIFTViewModel model)
        {
            string StrMsg = string.Empty;
            string StrJson = string.Empty;
            bool Success = true;
            StringBuilder sb = new StringBuilder();

            try
            {
                Log.WriteLog(Controller, "Start SWIFT SFTP");

                SftpEntity SftpOutEnt = new SftpEntity();
                SftpEntity SftpBackOutEnt = new SftpEntity();
                SftpUtility ObjectSftp = new SftpUtility();

                if (Search_ConfigReleaseMsg(ref StrMsg, ref SftpOutEnt, ref SftpBackOutEnt) == false)
                {
                    throw new Exception("Search_ConfigReleaseMsg() => " + StrMsg);
                }

                sb.Append("Info : Read Config Success.");
                sb.AppendLine();

                ArrayList ListFile = new ArrayList();
                ArrayList ListFileSuccess = new ArrayList();
                ArrayList ListFileError = new ArrayList();

                if (model.active_path == "repo_out")
                {
                    // Step 1 : Sftp REPO_OUT
                    ListFile.Add(model.file_name);

                    if (!ObjectSftp.UploadSFTPList(ref StrMsg, SftpOutEnt, ListFile, ref ListFileError, ref ListFileSuccess))
                    {
                        throw new Exception("UploadSFTPList() => " + StrMsg);
                    }

                    if (ListFileError.Count > 0)
                    {
                        Log.WriteLog(Controller, "- SFTP Fail" + SftpBackOutEnt.RemoteServerPath + " " + ListFileError[0].ToString());
                        sb.Append("Info : SFTP Fail " + ListFileError[0].ToString());
                        sb.AppendLine();
                    }
                    else
                    {
                        foreach (var FileSuccess in ListFileSuccess)
                        {
                            Log.WriteLog(Controller, "- SFTP " + SftpOutEnt.RemoteServerPath + " " + FileSuccess.ToString() + " Success.");
                            sb.Append("Info : SFTP " + FileSuccess.ToString() + " To " + SftpOutEnt.RemoteServerPath + " Success.");
                            sb.AppendLine();
                        }
                    }
                }
                else if (model.active_path == "repo_backout")
                {
                    // Step 1 : Sftp REPO_BACKOUT
                    ListFile = new ArrayList();
                    ListFileSuccess = new ArrayList();
                    ListFileError = new ArrayList();

                    ListFile.Add(model.file_name);

                    ObjectSftp = new SftpUtility();
                    if (!ObjectSftp.UploadSFTPList(ref StrMsg, SftpBackOutEnt, ListFile, ref ListFileError, ref ListFileSuccess))
                    {
                        throw new Exception("UploadSFTPList() => " + StrMsg);
                    }

                    if (ListFileError.Count > 0)
                    {
                        Log.WriteLog(Controller, "- SFTP Fail" + SftpBackOutEnt.RemoteServerPath + " " + ListFileError[0].ToString());
                        sb.Append("Info : SFTP Fail " + ListFileError[0].ToString());
                        sb.AppendLine();
                    }
                    else
                    {
                        foreach (var FileSuccess in ListFileSuccess)
                        {
                            Log.WriteLog(Controller, "- SFTP " + SftpBackOutEnt.RemoteServerPath + " " + FileSuccess.ToString() + " Success.");
                            sb.Append("Info : SFTP " + FileSuccess.ToString() + " To " + SftpOutEnt.RemoteServerPath + " Success.");
                            sb.AppendLine();
                        }
                    }
                }
                else
                {
                    // Step 1 : Sftp REPO_OUT
                    ListFile.Add(model.file_name);

                    if (!ObjectSftp.UploadSFTPList(ref StrMsg, SftpOutEnt, ListFile, ref ListFileError, ref ListFileSuccess))
                    {
                        throw new Exception("UploadSFTPList() => " + StrMsg);
                    }

                    if (ListFileError.Count > 0)
                    {
                        Log.WriteLog(Controller, "- SFTP Fail" + SftpBackOutEnt.RemoteServerPath + " " + ListFileError[0].ToString());
                        sb.Append("Info : SFTP Fail " + ListFileError[0].ToString());
                        sb.AppendLine();
                    }
                    else
                    {
                        foreach (var FileSuccess in ListFileSuccess)
                        {
                            Log.WriteLog(Controller, "- SFTP " + SftpOutEnt.RemoteServerPath + " " + FileSuccess.ToString() + " Success.");
                            sb.Append("Info : SFTP " + FileSuccess.ToString() + " To " + SftpOutEnt.RemoteServerPath + " Success.");
                            sb.AppendLine();
                        }
                    }

                    // Step 2 : Sftp REPO_BACKOUT
                    ListFile = new ArrayList();
                    ListFileSuccess = new ArrayList();
                    ListFileError = new ArrayList();

                    ListFile.Add(model.file_name);

                    ObjectSftp = new SftpUtility();
                    if (!ObjectSftp.UploadSFTPList(ref StrMsg, SftpBackOutEnt, ListFile, ref ListFileError, ref ListFileSuccess))
                    {
                        throw new Exception("UploadSFTPList() => " + StrMsg);
                    }

                    if (ListFileError.Count > 0)
                    {
                        Log.WriteLog(Controller, "- SFTP Fail" + SftpBackOutEnt.RemoteServerPath + " " + ListFileError[0].ToString());
                        sb.Append("Info : SFTP Fail " + ListFileError[0].ToString());
                        sb.AppendLine();
                    }
                    else
                    {
                        foreach (var FileSuccess in ListFileSuccess)
                        {
                            Log.WriteLog(Controller, "- SFTP " + SftpBackOutEnt.RemoteServerPath + " " + FileSuccess.ToString() + " Success.");
                            sb.Append("Info : SFTP " + FileSuccess.ToString() + " To " + SftpOutEnt.RemoteServerPath + " Success.");
                            sb.AppendLine();
                        }
                    }
                }

                StrMsg = "SFTP Success";
                StrJson = sb.ToString();
            }
            catch (Exception Ex)
            {
                Success = false;
                StrMsg = Ex.Message;
            }
            finally
            {
                Log.WriteLog(Controller, "End SWIFT SFTP");
            }

            return Json(new { Success = Success, Message = StrMsg, Result = StrJson });
        }

        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult Manual()
        {
            return View();
        }

        [HttpPost]
        [RoleScreen(RoleScreen.VIEW)]
        public JsonResult Manual(SWIFTViewModel viewModel)
        {
            string strMsg = string.Empty;
            try
            {
                //Step 1 : Get Config
                InterfaceSwiftSftpModel swiftModel = new InterfaceSwiftSftpModel();
                ResultWithModel<RpConfigResult> resultRpconfig = new ResultWithModel<RpConfigResult>();
                StaticEntities staticEnt = new StaticEntities();
                List<RpConfigModel> rpConfigModel = new List<RpConfigModel>();

                swiftModel.FileName = viewModel.file_name;
                swiftModel.Text = viewModel.result;
                swiftModel.create_by = User.UserId;

                staticEnt.RpConfig.GetRpConfig("RELEASE_MSG", string.Empty, p =>
                {
                    resultRpconfig = p;
                });

                if (!resultRpconfig.Success)
                {
                    throw new Exception("GetRpConfig() => [" + resultRpconfig.RefCode + "] " + resultRpconfig.Message);
                }

                //Step 2 : Set Config
                rpConfigModel = resultRpconfig.Data.RpConfigResultModel;
                swiftModel.RpConfigModel = rpConfigModel;

                //Step 3 : Interface Swift Manual
                ExternalInterfaceEntities interfaceEnt = new ExternalInterfaceEntities();
                ResultWithModel<InterfaceSwiftSftpResult> resultSwiftSftp = new ResultWithModel<InterfaceSwiftSftpResult>();

                interfaceEnt.InterfaceSwiftSftp.SwiftManual(swiftModel, p =>
                {
                    resultSwiftSftp = p;
                });

                if (!resultSwiftSftp.Success)
                {
                    throw new Exception(resultSwiftSftp.Message);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = "Error", Message = ex.Message });
            }
            return Json(new { Success = "Success", Message = strMsg });
        }

        [HttpPost]
        public JsonResult GenFileName(SWIFTViewModel model)
        {
            string strMsg = string.Empty;
            string mtCode = model.mt_code.Replace("MT", "");
            string userId = User.UserId;
            string payment = "SWIFT";
            string system = "REPO";
            string date = DateTime.Now.ToString("ddMMyyhhmmss");
            string fileName = $"{mtCode}_{userId}_{payment}{system}{date}.out";
            return Json(new { Success = "Success", Message = strMsg, FileName = fileName });
        }

        public ActionResult FillMtCode(string search)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            PaymentProcessEntities apiRPReleaseMsg = new PaymentProcessEntities();
            apiRPReleaseMsg.RPReleaseMessage.GetDDLSwiftManual(search, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        private bool Search_ConfigReleaseMsg(ref string ReturnMsg, ref SftpEntity SftpOutEnt, ref SftpEntity SftpBackOutEnt)
        {
            try
            {
                RpSftpModel SftpModel = new RpSftpModel();
                ResultWithModel<RpConfigResult> ResultRpconfig = new ResultWithModel<RpConfigResult>();
                StaticEntities StaticEnt = new StaticEntities();
                List<RpConfigModel> RpConfigModel = new List<RpConfigModel>();

                StaticEnt.RpConfig.GetRpConfig("RELEASE_MSG", string.Empty, p =>
                {
                    ResultRpconfig = p;
                });

                if (!ResultRpconfig.Success)
                {
                    throw new Exception(ResultRpconfig.Message);
                }

                RpConfigModel = ResultRpconfig.Data.RpConfigResultModel;
                SftpOutEnt.RemoteServerName = RpConfigModel.FirstOrDefault(a => a.item_code == "IP")?.item_value;
                SftpOutEnt.RemotePort = RpConfigModel.FirstOrDefault(a => a.item_code == "PORT")?.item_value;
                SftpOutEnt.RemoteUserName = RpConfigModel.FirstOrDefault(a => a.item_code == "USER")?.item_value;
                SftpOutEnt.RemotePassword = RpConfigModel.FirstOrDefault(a => a.item_code == "PASSWORD")?.item_value;
                SftpOutEnt.RemoteServerPath = RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SFTP_REPO_OUT")?.item_value;
                SftpOutEnt.RemoteSshHostKeyFingerprint = RpConfigModel.FirstOrDefault(a => a.item_code == "SSHHOSTKEYFINGERPRINT")?.item_value;
                SftpOutEnt.RemoteSshPrivateKeyPath = RpConfigModel.FirstOrDefault(a => a.item_code == "SSHPRIVATEKEYPATH")?.item_value;
                SftpOutEnt.NoOfFailRetry = System.Convert.ToInt32(RpConfigModel.FirstOrDefault(a => a.item_code == "FAIL_RETRY")?.item_value);
                SftpOutEnt.LocalPath = HostingEnvironment.MapPath(RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_WEB")?.item_value);

                Log.WriteLog(Controller, "SFTP OutEnt");
                Log.WriteLog(Controller, "RemoteServerName = " + SftpOutEnt.RemoteServerName);
                Log.WriteLog(Controller, "RemotePort = " + SftpOutEnt.RemotePort);
                Log.WriteLog(Controller, "RemoteServerPath = " + SftpOutEnt.RemoteServerPath);
                Log.WriteLog(Controller, "RemoteSshHostKeyFingerprint = " + SftpOutEnt.RemoteSshHostKeyFingerprint);
                Log.WriteLog(Controller, "RemoteSshPrivateKeyPath = " + SftpOutEnt.RemoteSshPrivateKeyPath);
                Log.WriteLog(Controller, "NoOfFailRetry File = " + SftpOutEnt.NoOfFailRetry);
                Log.WriteLog(Controller, "PATH_WEB = " + SftpOutEnt.LocalPath);

                SftpBackOutEnt.RemoteServerName = RpConfigModel.FirstOrDefault(a => a.item_code == "IP")?.item_value;
                SftpBackOutEnt.RemotePort = RpConfigModel.FirstOrDefault(a => a.item_code == "PORT")?.item_value;
                SftpBackOutEnt.RemoteUserName = RpConfigModel.FirstOrDefault(a => a.item_code == "USER")?.item_value;
                SftpBackOutEnt.RemotePassword = RpConfigModel.FirstOrDefault(a => a.item_code == "PASSWORD")?.item_value;
                SftpBackOutEnt.RemoteServerPath = RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_SFTP_REPO_BACKOUT")?.item_value;
                SftpBackOutEnt.RemoteSshHostKeyFingerprint = RpConfigModel.FirstOrDefault(a => a.item_code == "SSHHOSTKEYFINGERPRINT")?.item_value;
                SftpBackOutEnt.RemoteSshPrivateKeyPath = RpConfigModel.FirstOrDefault(a => a.item_code == "SSHPRIVATEKEYPATH")?.item_value;
                SftpBackOutEnt.NoOfFailRetry = System.Convert.ToInt32(RpConfigModel.FirstOrDefault(a => a.item_code == "FAIL_RETRY")?.item_value);
                SftpBackOutEnt.LocalPath = HostingEnvironment.MapPath(RpConfigModel.FirstOrDefault(a => a.item_code == "PATH_WEB")?.item_value);

                Log.WriteLog(Controller, "SFTP BackOutEnt");
                Log.WriteLog(Controller, "RemoteServerName = " + SftpBackOutEnt.RemoteServerName);
                Log.WriteLog(Controller, "RemotePort = " + SftpBackOutEnt.RemotePort);
                Log.WriteLog(Controller, "RemoteServerPath = " + SftpBackOutEnt.RemoteServerPath);
                Log.WriteLog(Controller, "RemoteSshHostKeyFingerprint = " + SftpBackOutEnt.RemoteSshHostKeyFingerprint);
                Log.WriteLog(Controller, "RemoteSshPrivateKeyPath = " + SftpBackOutEnt.RemoteSshPrivateKeyPath);
                Log.WriteLog(Controller, "NoOfFailRetry File = " + SftpBackOutEnt.NoOfFailRetry);
                Log.WriteLog(Controller, "PATH_WEB = " + SftpBackOutEnt.LocalPath);
            }
            catch (Exception Ex)
            {
                ReturnMsg = Ex.Message;
                return false;
            }

            return true;
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