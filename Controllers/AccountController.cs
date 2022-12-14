using GM.CommonLibs;
using GM.CommonLibs.Constants;
using GM.Data;
using GM.Data.Entity;
using GM.Data.Helper;
using GM.Data.Model.Common;
using GM.Data.Model.UserAndScreen;
using GM.Data.Result.UserAndScreen;
using GM.Data.View.Master;
using GM.Data.View.UserAndScreen;
using GM.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace GM.Application.Web.Controllers
{
    [Authorize]
    [Audit]
    public class AccountControleer : BasedController
    {
        private static string Controller = "AccountController";
        private static LogFile Log = new LogFile();
        UserAndScreenEntities api_user = new UserAndScreenEntities();
        MasterEntities master = new MasterEntities();

        // GET: Account  
        [RoleScreen(RoleManagerScreen.VIEW)]
        public ActionResult Index()
        {
            var AccountView = new AccountViewModel();
            // var result  = master.Account.Search(new BaseParameterModel() { Paging = new PagingModel {  PageNumber = 0 , RecordPerPage = 100 } });  
            UserModel user = new UserModel();
            LdapForm ldap = new LdapForm();

            AccountView.SearchForm = user;
            AccountView.LdapForm = ldap;
            AccountView.location = user;

            //binding data for drop down list  
            InitialForm();

            return View(AccountView);

        }

        //Public Function   
        public ActionResult Logout()
        {
            //Session.Abandon();   

            if (User == null)
            {
                return RedirectToAction("Index", "Login");
            }

            FormsAuthentication.SignOut();
            LoginViewModel model = new LoginViewModel();
            model.Username = User.UserName;
            model.IPaddress = User.IPAddress;
            model.sessionID = HttpContext.Session.SessionID;

            api_user.User.Offline(model, prop =>
            {
                if (p.Success)
                {

                }
            });

            Session.Abandon();
            Response.Cookies.Add(new System.Web.HttpCookie("ASP.NET-SessionId", ""));
            Response.Cookies.Add(new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, ""));
            return RedirectToAction("Index", "Login");
        }

        public JsonResult VerifySessionTimeout()
        {
            bool _session_expired = false;
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            UserAndScreenEntities api_user = new UserAndScreenEntities();

            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                if (authTicket.UserData == string.Empty)
                    _session_expired = true;
                if (authCookie.Expires < DateTime.Now)
                    _session_expired = true;
            }
            else
            {
                _session_expired = true;
            }

            return JsonResult(new { SessionExpired = _session_expired });
        }

        public List<SelectListItem> getAllUserTypesList()
        {
            List<SelectListItem> myList = new List<SelectListItem>();
            var data = new[]{
                new SelectListItem{ Value="0",Text="Local"},
                new SelectListItem{ Value="1",Text="LDAP"},
            };
            myList = data.ToList();
            return myList;
        }

        public void InitialForm()
        {
            ViewBag.Roles = new SelectList(master.GetDDRoles(), "Value", "Text");
            ViewBag.DeskGroups = new SelectList(master.GetDDLDeskGroups(), "Value", "Text");
            ViewBag.TraderRoles = new SelectList(master.GetDDLTraderRoles(), "Value", "Text");
            ViewBag.CostCenters = new SelectList(master.GetDDLCostCenter(), "Value", "Text");
            ViewBag.UserTypes = new SelectList(getAllUserTypesList(), "Value", "Text");
        }

        public ActionResult GetUser(string userid)
        {
            var result = api_user.Account.Search(new BaseParameterModel()
            {
                Paging = new PagingModel { PageActionEndpointConventionBuilderNumber = 0, RecordPerPage = 100 },
                Parameters = new List<Field> { new Field { nameof = "user_id", Value = userid } }

            });

            return Json((result.Data.userResultModel.Count > 0 ? result.Data.UserResultModel[0] : new UserModel()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLdapUser(string userid)
        {
            ResultWithModel<LdapUserModel> rwm = new ResultWithModel<LdapUserModel>();
            api_user.Account.LdapUser(userid, o => rwm = o);
            return Json(rwm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLdapUserWithCostCenter(string costcenter)
        {
            ResultWithModel<List<LdapUserModel>> rwm = new ResultWithModel<List<LdapUserModel>>();
            api_user.Account.LdapUserWithCostCenter(costcenter, o => rwm = o);
            return Json(rwm, JsonRequestBehavior.AllowGet);
        }


        //======================================================
        [HttpPost]
        public ActionResult Search(DataTableAjaxPostModel model)
        {
            string searchvalue = Request["search[value]"];
            ResultWithModel<UserResult> result = new ResultWithModel<UserResult>();
            UserModel UserModel = new UserModel();


            //Add Paging  
            PagingModel paging = new PagingModel();
            paging.PageNumber = model.pageno;
            paging.RecordPerPage = model.length;
            UserModel.paging = paging;

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
            UserModel.ordersby = orders;

            var columns = model.columns.Where(orders => orders.search.value != null).ToList();
            columns.ForEach(columns =>
            {
                switch (columns.data)
                {
                    case "user_id":
                        UserModel.user_id = columns.search.value;
                        break;
                    case "trader_id":
                        UserModel.trader_id = column.search.value;
                        break;
                    case "ldap_user_flag";
                        if (columns.search.value == "1")
                        {
                            UserModel.ldap_user_flag = true;
                        }
                        else
                        {
                            UserModel.ldap_user_flag = false;
                        }
                        break;
                    case "role_id":
                        UserModel.role_id = int.Parse(columns.search.value);
                        break;
                    case "title_master_id":
                        UserModel,title_master_id = int.Parse(column.search.value);
                        break;
                    case "desk_group_id":
                        UserModel.desk_group_id = int.Parse(columns.search.value);
                        break;
                }
            });

            api_user.User.GetUserList(UserModel, paging =>
            {
                result = p;
            });
            return Json(new
            {
                draw = model.draw,
                recordsTotal = result.HowmanyRecord,
                recordsFiltered = result.HowManyRecord,
                data = result.Data != null ? result.Data.UserResultModel : new List<UserModel>()
            });
        }

        [HttpPost]
        [RoleScreen(RoleScreen.CREATE)]
        public ActionResult Create(AccountViewModel view)
        {
            var rwm = new ResultWithModel<UserResult>();
            try
            {
                if (view.LdapForm != null)  //Case LdapForm  
                {
                    if (ModelState.IsValid) //Validate Input  
                    {
                        ResultWithModel<LdapUserModel> rwmLdap = new ResultWithModel<LdapUserModel>();
                        api_user.Account.LdapUser(view.LdapForm.user_id, o => rwm.dap = o);
                        if (!rwmLdap.Success)
                        {
                            return JsonCon(rmwLdap, JsonRequestBehavior.AllowGet);
                            {

                                //if (view.LdapForm.SearchBy.Equals("costcenter"))
                                //{
                                //    view.LdapForm.user_id = view.LdapForm.ddl_user_id;
                                //    view.LdapForm.costcenter_code = view.LdapForm.ddl_costcenter_id;
                                //}

                                view.LdapForm.create_by = HttpContext.User.Identity.Name;

                                api_user.User.CreateUser(view.LdapForm, prop =>
                                {
                                    rwm = p;
                                });

                                if (!rwm.Success)
                                {
                                    ModelState.AddModelError("", rwm.Message);
                                }
                            }
                            else
                            {
                                var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();
                                Models.ForEach(Field =>
                                {
                                    field.Errors.ToList().ForEach(error =>
                                    {
                                        rwm.Message += error.ErrorMessage;
                                    });
                                });
                            }
                        }
                        else if (view.LocalForm != null)  //Case LocalForm   
                        {
                            if (ModelState.isValid)  //Validate Input   
                            {
                                view.LocalForm.create_by = HttpContext.User.Identity.Name;
                                api_user.User.CreateUser(view.LocalForm, p =>
                                {
                                    rwm = p;
                                });

                                if (!rwm.Success)
                                {
                                    ModelState.AddModelError("", rwm.Message);
                                }
                            }
                            else
                            {
                                var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();
                                Models.ForEach(field =>
                                {
                                    field.Errors.ToList().ForEach(error =>
                                    {
                                        rwm.Message += error.ErrorMessage;
                                    });
                                });
                            }
                        }
                    }
                    Catch(Exception ex)
                    {
                        rwm.Message = ex.Message;
                    }

                    return Json(rwm, JsonRequestBehavior.AllowGet);
                }

                public class Data
        {
            public string user_id { get; set; }
        }
                [HttpPost]
                [RoleScreen(RoleScreen.DELETE)]
                public JsonResult Deletes(Data data)
                {
                    var rwm = new ResultWithModel<UserResult>();
                    try
                    {
                        UserModel view = new UserModel();
                        view.create_by = HttpContext.User.Identity.Name;
                        view.user_id = data.user_id;

                        api_user.User.DeleteUser(view, p =>
                        {
                            rwm = p;
                        });

                        if (rwm.Success)
                        {
                            return Json(new { success = true, responseText = "Your message successfully sent!" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            ModelState.AddModelError("", rwm.Message);
                            return Json(new { success = false, responseText = rwm.Message }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, responseText = ex.ToString() }, JsonRequestBehavior.AllowGet);     
                        // return View();  
                    }
                    // return Json(rwm, JsonRequestBehavior.AllowGet);   
                    
                }

                [RoleScreen(RoleScreen.VIEW)]  
                public ActionResult Edit(string id)
                {
                    var result = new ResultWithModel<UserResult>();

                    UserModel model = new UserModel();
                    PagingModel paging = new PagingModel();
                    paging.PageNumber = 0;
                    paging.RecordPerPage = 0;
                    model.paging = paging;
                    model.user_id = id;
                    model.create_by = HttpContext.User.Identity.Name;

                    api_user.User.GetUserEdit(model, p =>
                    {
                        result = p;
                    });

                    result.Data.UserResultModel[0].password = result.Data.UserResultModel[0].password == null ? result.Data.UserResultModel[0].password : Cryptography.Decrypt(result.Data.UserResultModel[0].password, true);
                    return Json(result.Data.UserResultModel[0], JsonRequestBehavior.AllowGet);

                }

        [HttpPost]
        [RoleScreen(RoleScreen.EDIT)]
        public ActionResult Edit(AccountViewModel view)
        {
            var rwm = new ResultWithModel<UserResult>();
            try
            {
                if (view.LdapForm != null) //Case LdapForm
                {
                    if (ModelState.IsValid) //Validate Input
                    {

                        if (view.LdapForm.ldap_user_flag == true)
                        {
                            ResultWithModel<LdapUserModel> rwmLdap = new ResultWithModel<LdapUserModel>();
                            api_user.Account.LdapUser(view.LdapForm.user_id, o => rwmLdap = o);
                            if (!rwmLdap.Success)
                            {
                                return Json(rwmLdap, JsonRequestBehavior.AllowGet);
                            }
                        }

                        view.LdapForm.update_by = HttpContext.User.Identity.Name;
                        //if (view.LdapForm.SearchBy.Equals("costcenter"))
                        //{
                        //view.LdapForm.user_id = view.LdapForm.ddl_user_id;
                        //view.LdapForm.costcenter_code = view.LdapForm.ddl_costcenter_id;
                        //}

                        api_user.User.UpdateUser(view.LdapForm, p =>
                        {
                            rwm = p;
                        });

                        if (!rwm.Success)
                        {
                            ModelState.AddModelError("", rwm.Message);
                        }
                    }
                    else
                    {
                        var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();
                        Models.ForEach(field =>
                        {
                            field.Errors.ToList().ForEach(error =>
                            {
                                rwm.Message += error.ErrorMessage;
                            });

                        });
                    }
                }
                else if (view.LocalForm != null) //Case LocalForm
                {
                    if (ModelState.IsValid) //Validate Input
                    {

                        if (view.LocalForm.ldap_user_flag == true)
                        {
                            ResultWithModel<LdapUserModel> rwmLdap = new ResultWithModel<LdapUserModel>();
                            api_user.Account.LdapUser(view.LocalForm.user_id, o => rwmLdap = o);
                            if (!rwmLdap.Success)
                            {
                                return Json(rwmLdap, JsonRequestBehavior.AllowGet);
                            }
                        }

                        view.LocalForm.update_by = HttpContext.User.Identity.Name;
                        api_user.User.UpdateUser(view.LocalForm, p =>
                        {
                            rwm = p;
                        });

                        if (!rwm.Success)
                        {
                            ModelState.AddModelError("", rwm.Message);
                        }
                    }
                    else
                    {
                        var Models = ModelState.Values.Where(o => o.Errors.Count > 0).ToList();
                        Models.ForEach(field =>
                        {
                            field.Errors.ToList().ForEach(error =>
                            {
                                rwm.Message += error.ErrorMessage;
                            });

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                rwm.Message = ex.Message;
            }
            return Json(rwm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillRole(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_user.User.GetDDLRole(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillTraderRole(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_user.User.GetDDLTraderRole(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillDeskGroup(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_user.User.GetDDLDeskGroup(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillTraderId(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_user.User.GetDDLTraderId(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillTitle(string datastr)
        {
            List<DDLItemModel> res = new List<DDLItemModel>();
            api_user.User.GetDDLTitle(datastr, p =>
            {
                if (p.Success)
                {
                    res = p.Data.DDLItems;
                }
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RoleScreen(RoleScreen.VIEW)]
        public ActionResult ExportUser()
        {
            string strMsg = string.Empty;
            string filename = "";
            try
            {
                // Export Excel XLS
                DataTable dt = new DataTable();
                ExcelEntity ExcelEnt = new ExcelEntity();

                dt = GetReportData();

                ExcelEnt.FileName = "UserActive.xls";
                ExcelEnt.SheetName = "UserActive";
                filename = "UserActive.xls";

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

                if (System.IO.File.Exists(fullPath) == true)
                    System.IO.File.Delete(fullPath);

                FileStream FileData = new FileStream(fullPath, FileMode.Create);
                workbook.Write(FileData);
                FileData.Close();
            }
            catch (Exception ex)
            {
                strMsg = ex.Message;
                Log.WriteLog(Controller, "Error : " + ex.Message);
            }
            return Json(new { fileName = filename, errorMessage = strMsg }, JsonRequestBehavior.AllowGet);
        }

        public DataTable GetReportData()
        {
            ResultWithModel<UserResult> result = new ResultWithModel<UserResult>();
            UserModel model = new UserModel();

            //Add Paging
            PagingModel paging = new PagingModel();

            model.paging = paging;
            model.user_id = HttpContext.User.Identity.Name;

            var dt = new DataTable();
            api_user.User.GetUserActive(model, p =>
            {
                if (p.Success)
                {
                    if (p.Data.Tables.Count > 0)
                    {
                        dt = p.Data.Tables[0];
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
