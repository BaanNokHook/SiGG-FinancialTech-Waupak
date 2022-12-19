using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GM.CommonLibs.ClassLibs;
using GM.Data.Entity;
using GM.Data.Model.Common;
using GM.Data.Result.Master;
using GM.Data.View.Master;

namespace GM.Application.Web.Controllers
{
    [AllowAnonymous]
    public class LoginController : BaseController
    {
        private readonly UserAndScreenEntities _apiUserandscreen = new UserAndScreenEntities();

        private string GetIp
        {
            get
            {
                var ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip))
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                return ip;
            }
        }

        // GET: Login
        public ActionResult Index(string ReturnUrl)
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                if (!string.IsNullOrEmpty(ReturnUrl))
                    Response.Redirect(ReturnUrl);
                else
                    Response.Redirect("/");
            }
            
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel login, string ReturnUrl)
        {
            if (ModelState.ContainsKey("isKick")) ModelState["isKick"].Errors.Clear();

            if (ModelState.IsValid)
                if (login.Username != string.Empty && login.Password != string.Empty)
                    _apiUserandscreen.User.Login(login, res =>
                    {
                        if (res.Success)
                        {
                            if (res.Data != null && res.Data.LoginViewModels != null &&
                                res.Data.LoginViewModels.Count > 0 && res.Data.LoginViewModels[0].isOnline == false)
                            {
                                login.sessionID = HttpContext.Session.SessionID;
                                login.IPaddress = GetIp;
                                _apiUserandscreen.User.Online(login, p =>
                                {
                                    if (!p.Success) ModelState.AddModelError("ErrorMsg", p.Message);
                                });

                                //initial user
                                FormsAuthentication.SetAuthCookie(login.Username, login.RememberMe);
                                //string userData = (new Authentication()).CreateToken(login.Username);
                                var userData = HttpContext.Session.SessionID;

                                var ticket = new FormsAuthenticationTicket(1,
                                    login.Username,
                                    DateTime.Now,
                                    DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
                                    login.RememberMe,
                                    userData);
                                var encTicket = FormsAuthentication.Encrypt(ticket);
                                var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                                faCookie.Expires = DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes);
                                Response.Cookies.Add(faCookie);

                                Response.Redirect(!string.IsNullOrEmpty(ReturnUrl) ? ReturnUrl : "/");
                            }
                            else
                            {
                                login.isOnline = true;
                                ViewBag.isOnline = true;
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("ErrorMsg", res.Message);
                        }
                    });
            return View("Index");
        }


        [HttpPost]
        public ActionResult LoginAndKick(LoginViewModel login)
        {
            //ViewBag.version = Utility.AssemblyVersion;
            var result = new ResultWithModel<LoginResult>();

            login.sessionID = HttpContext.Session.SessionID;
            login.IPaddress = GetIp;
            login.isKick = true;
            Session["loginModel"] = login;
            _apiUserandscreen.User.Online(login, p => { result = p; });
            //initial user
            FormsAuthentication.SetAuthCookie(login.Username, login.RememberMe);
            //string userData = (new Authentication()).CreateToken(login.Username);
            var userData = HttpContext.Session.SessionID;

            var ticket = new FormsAuthenticationTicket(1,
                login.Username,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                login.RememberMe,
                userData);
            var encTicket = FormsAuthentication.Encrypt(ticket);
            var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);

            Response.Cookies.Add(faCookie);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult StayLogged(LoginViewModel login)
        {
            var result = new ResultWithModel<LoginResult>();

            login.sessionID = HttpContext.Session.SessionID;
            login.IPaddress = GetIp;
            Session["loginModel"] = login;
            _apiUserandscreen.User.Online(login, p => { result = p; });

            FormsAuthentication.SetAuthCookie(login.Username, login.RememberMe);
            var userData = HttpContext.Session.SessionID;

            var ticket = new FormsAuthenticationTicket(1,
                login.Username,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                login.RememberMe,
                userData);
            var encTicket = FormsAuthentication.Encrypt(ticket);
            var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);

            Response.Cookies.Add(faCookie);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult IsSessionTimeout()
        {
            var IsTimeOut = false;
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (string.IsNullOrEmpty(authTicket.UserData)) IsTimeOut = true;
            }
            else
            {
                IsTimeOut = true;
            }

            return Json(new {IsTimeOut}, JsonRequestBehavior.AllowGet);
        }
    }
}