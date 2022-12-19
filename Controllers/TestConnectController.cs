using System;
using GM.CommonLibs.ClassLibs;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Model.Static;
using GM.Data.Result.Static;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web.Mvc;
using GM.CommonLibs;

namespace GM.Application.Web.Controllers
{
    [AllowAnonymous]
    public class TestConnectController : Controller
    {
        // GET: TestConnect
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DatabaseInfo()
        {
            StaticEntities api_static = new StaticEntities();
            ResultWithModel<TestConnectResult> result = new ResultWithModel<TestConnectResult>();
            TestConnectModel model = new TestConnectModel();
            model.create_by = "1578";//fix

            api_static.TestConnect.GetTestDatabaseInfoList(model, p =>
            {
                result = p;
            });

            string status = "Offline", serverName = "", machineName = "", instanceName = "", databaseName = "";

            if (result.Success)
            {
                status = "Online";

                if (result.Data != null && result.Data.TestConnectResultModel != null)
                {
                    var resServerName = result.Data.TestConnectResultModel.FirstOrDefault(a => a.propertyname.Contains("ServerName"));
                    if (resServerName != null)
                    {
                        serverName = resServerName.value;
                    }

                    var resMachineName = result.Data.TestConnectResultModel.FirstOrDefault(a => a.propertyname.Contains("MachineName"));
                    if (resMachineName != null)
                    {
                        machineName = resMachineName.value;
                    }

                    var resInstanceName = result.Data.TestConnectResultModel.FirstOrDefault(a => a.propertyname.Contains("InstanceName"));
                    if (resInstanceName != null)
                    {
                        instanceName = resInstanceName.value;
                    }

                    var resDbName = result.Data.TestConnectResultModel.FirstOrDefault(a => a.propertyname.ToString().Contains("DB_NAME"));
                    if (resDbName != null)
                    {
                        databaseName = resDbName.value;
                    }
                }
            }

            return Json(new
            {
                Status = status,
                ServerName = serverName,
                MachineName = machineName,
                InstanceName = instanceName,
                DatabaseName = databaseName
            });
        }

        [HttpPost]
        public JsonResult InternalService()
        {
            List<Dictionary<string, object>> dataRows = new List<Dictionary<string, object>>();
            Dictionary<string, string> serviceSettings = new Dictionary<string, string>()
            {
                { "AgencyAndRating", PublicConfig.GetValue("AgencyAndRating")},
                { "CounterParty", PublicConfig.GetValue("CounterParty")},
                { "ExchangeRate", PublicConfig.GetValue("ExchangeRate")},
                { "MarketRate", PublicConfig.GetValue("MarketRate")},
                { "Master", PublicConfig.GetValue("Master")},
                { "PaymentProcess", PublicConfig.GetValue("PaymentProcess")},
                { "RPTrans", PublicConfig.GetValue("RPTrans")},
                { "Security", PublicConfig.GetValue("Security")},
                { "Static", PublicConfig.GetValue("Static")},
                { "UserAndScreen", PublicConfig.GetValue("UserAndScreen")},
                { "ExternalInterface", PublicConfig.GetValue("ExternalInterface")},
                { "GLProcess", PublicConfig.GetValue("GLProcess")},
                { "Report", PublicConfig.GetValue("Report")}
                ,{ "Gateway", PublicConfig.GetValue("Gateway") + "Home/Index"}
                ,{ "Provider", PublicConfig.GetValue("Provider") + "Home/Index"}
            };

            foreach (var serviceSetting in serviceSettings)
            {
                Dictionary<string, object> dRow = new Dictionary<string, object>();

                dRow.Add("name", serviceSetting.Key);
                dRow.Add("site", serviceSetting.Value);
                dRow.Add("status", CheckService(serviceSetting.Value));
                dataRows.Add(dRow);
            }

            return Json(new { data = dataRows });
        }

        [HttpPost]
        public JsonResult ExternalService()
        {
            List<Dictionary<string, object>> dataRows = new List<Dictionary<string, object>>();

            StaticEntities api_static = new StaticEntities();
            ExternalInterfaceEntities api_external = new ExternalInterfaceEntities();
            ResultWithModel<RpConfigResult> result = new ResultWithModel<RpConfigResult>();
            TestConnectModel model = new TestConnectModel();
            model.create_by = "1578";//fix

            api_static.TestConnect.GetExternalServiceList(model, p =>
            {
                result = p;
            });

            if (result.Success)
            {
                if (result.Data != null && result.Data.RpConfigResultModel != null)
                {
                    //check swift
                    var resSwift = result.Data.RpConfigResultModel.Where(a => a.category == "RELEASE_MSG").ToList();
                    if (resSwift.Any())
                    {
                        var info = resSwift.FirstOrDefault(a => a.item_code == "IP");
                        Dictionary<string, object> dRow = new Dictionary<string, object>();
                        dRow.Add("name", info.category + " (Check in Application)");
                        dRow.Add("site", info.item_value);
                        dRow.Add("status", ConnectionOpen(resSwift));
                        dataRows.Add(dRow);
                    }

                    //check open web
                    var resServiceUrls = result.Data.RpConfigResultModel.Where(a => a.item_code.Contains("SERVICE_URL"));

                    foreach (var serviceUrl in resServiceUrls)
                    {
                        Dictionary<string, object> dRow = new Dictionary<string, object>();

                        ResultWithModel<object> res = new ResultWithModel<object>();
                        api_external.CheckService.ConnectionOpen(serviceUrl, p =>
                        {
                            res = p;
                        });

                        dRow.Add("name", serviceUrl.category);
                        dRow.Add("site", serviceUrl.item_value);
                        dRow.Add("status", res.Message);
                        dataRows.Add(dRow);
                    }

                    //check open sftp
                    var resSftpGroups = result.Data.RpConfigResultModel.Where(a => a.item_code.Contains("IP"));

                    foreach (var sftpGroup in resSftpGroups)
                    {
                        ResultWithModel<object> resultSftp = new ResultWithModel<object>();
                        List<RpConfigModel> configSftp = result.Data.RpConfigResultModel.Where(a => a.category == sftpGroup.category).ToList();
                        //string r = string.Empty;
                        api_external.CheckSFTP.ConnectionOpen(configSftp, p =>
                        {
                            resultSftp = p;
                        });

                        Dictionary<string, object> dRow = new Dictionary<string, object>();

                        dRow.Add("name", sftpGroup.category);
                        dRow.Add("site", sftpGroup.item_value);
                        dRow.Add("status", resultSftp.Message);
                        dataRows.Add(dRow);
                    }

                    //check LDAP
                    var resLdapGroups = result.Data.RpConfigResultModel.Where(a => a.item_code.Contains("LDAP_server"));
                    foreach (var ldapGroup in resLdapGroups)
                    {
                        string[] arrStrings = ldapGroup.item_value.Split(':');
                        int port = int.Parse(arrStrings[1]);

                        Dictionary<string, object> dRow = new Dictionary<string, object>();

                        dRow.Add("name", ldapGroup.category);
                        dRow.Add("site", ldapGroup.item_value);
                        dRow.Add("status", CheckLdap(arrStrings[0], port));
                        dataRows.Add(dRow);
                    }
                }
            }

            return Json(new { data = dataRows });
        }

        private string CheckService(string site)
        {
            try
            {
                //get
                using (WebClient client = new WebClient())
                {
                    if (site.StartsWith("https://"))
                        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, errors) => true;

                    using (client.OpenRead(site))
                    {
                        return "Online";
                    }
                }

            }
            catch (Exception ex)
            {
                try
                {
                    //post
                    using (WebClient client = new WebClient())
                    {
                        if (site.StartsWith("https://"))
                            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, errors) => true;

                        client.UploadString(site, string.Empty);
                        return "Online";
                    }

                }
                catch (WebException we)
                {
                    if (we.Response != null && ((System.Net.HttpWebResponse)we.Response).StatusCode == HttpStatusCode.BadRequest)
                    {
                        return "Online";
                    }
                    else
                    {
                        return "Offline";
                    }
                }
                catch
                {
                    return "Offline";
                }
            }
        }

        private string CheckLdap(string address, int port)
        {
            try
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(address, port);
                return "Online";
            }
            catch
            {
                return "Offline";
            }
        }

        private string ConnectionOpen(List<RpConfigModel> model)
        {
            try
            {
                SftpUtility objectSftp = new SftpUtility();

                string strMsg = string.Empty;
                SftpEntity sftpEnt = new SftpEntity();
                sftpEnt.RemoteServerName = model.FirstOrDefault(a => a.item_code == "IP")?.item_value;
                sftpEnt.RemotePort = model.FirstOrDefault(a => a.item_code == "PORT")?.item_value;
                sftpEnt.RemoteUserName = model.FirstOrDefault(a => a.item_code == "USER")?.item_value;
                sftpEnt.RemotePassword = model.FirstOrDefault(a => a.item_code == "PASSWORD")?.item_value;
                sftpEnt.RemoteSshHostKeyFingerprint = model.FirstOrDefault(a => a.item_code == "SSHHOSTKEYFINGERPRINT")?.item_value;
                sftpEnt.RemoteSshPrivateKeyPath = model.FirstOrDefault(a => a.item_code == "SSHPRIVATEKEYPATH")?.item_value;
                sftpEnt.NoOfFailRetry = Convert.ToInt32(model.FirstOrDefault(a => a.item_code == "FAIL_RETRY")?.item_value);
                sftpEnt.RemoteServerPath = model.FirstOrDefault(a => a.item_code == "PATH_SFTP")?.item_value;

                if (objectSftp.CheckConnectionSFTP(ref strMsg, sftpEnt))
                {
                    return "Online";
                }

                return "Offline";
            }
            catch (Exception ex)
            {
                return "Offline";
            }
        }
    }
}