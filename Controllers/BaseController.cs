using GM.CommonLibs;
using GM.Data;
using System;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;

namespace GM.Application.Web.Controllers
{
    public class BaseController : Controller
    {
        public static readonly Stopwatch _stopwatch = new Stopwatch();
        public static readonly LogFile _log = new LogFile();
        protected new virtual UserPrincipal User => HttpContext.User as UserPrincipal;

        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    _stopwatch.Reset();
        //    _stopwatch.Start();
        //    Log(filterContext.RouteData, _stopwatch.Elapsed, "Begin", false);
        //}

        //protected override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    _stopwatch.Stop();
        //    Log(filterContext.RouteData, _stopwatch.Elapsed,"End",true);
        //}

        //private static void Log(RouteData routeData, TimeSpan executionTime, string title , bool isExecuted)
        //{
        //    var _controllerName = routeData.Values["controller"].ToString();
        //    var _actionName = routeData.Values["action"].ToString();
        //    var _datetime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff");

        //    _log.WriteLog(_controllerName,
        //        isExecuted
        //            ? $"{title} Action: [{_actionName}] , Tracking Time: [{_datetime}] , Executed Time: {executionTime.TotalSeconds} seconds."
        //            : $"{title} Action: [{_actionName}] , Tracking Time: [{_datetime}]");
        //}
    }
}