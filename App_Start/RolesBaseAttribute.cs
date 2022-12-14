using GM.Data;
using GM.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace GM.WaTuPak.Web.App_Start
{
    public class RolesBaseAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //throw new NotImplementedException();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpCookie authCookie = filterContext.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                var action     = filterContext.ActionDescriptor.ActionName + " (Logged By: MyNewCustomActionFilter)";
                var remoteip   = filterContext.HttpContext.Request.UserHostAddress;
                var cdate      = filterContext.HttpContext.Timestamp;
                var user       = filterContext.HttpContext.User.Identity.Name;
                var token      = ((UserPrincipal)HttpContext.Current.User).Token;

                //TokenValidationHandler TokenValidation = new TokenValidationHandler();
                //TokenValidation.TokenHandler(token);

                //Authentication aut  = new Authentication();
                //var mUser           = aut.GetUser(Thread.CurrentPrincipal);


          
                //if(1 != 1)
                //{
                //    RedirectToRoute(filterContext,
                //        new { controller = "Error", action = "ERROR500" }
                //    );
                //}
            }
        }


        void RedirectToRoute(ActionExecutingContext context, object routeValues)
        {
            var rc = new RequestContext(context.HttpContext, context.RouteData);
            string url = RouteTable.Routes.GetVirtualPath(rc, new RouteValueDictionary(routeValues)).VirtualPath;
            context.HttpContext.Response.Redirect(url, true);
        }
    }
}