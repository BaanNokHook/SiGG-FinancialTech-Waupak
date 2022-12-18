using ElFinder;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace GM.Application.Web.Controllers
{
    public class FileController : Controller
    {
        public virtual ActionResult Index()
        {
            var driver = new FileSystemDriver();
            
            var root = new Root(new DirectoryInfo(Server.MapPath("~")).Parent,
                "http://" + Request.Url.Authority)
            {
                IsReadOnly = false,
                Alias = "Root",
                MaxUploadSizeInMb = 500,
                LockedFolders = new List<string>()
            };
            
            driver.AddRoot(root);
            var connector = new Connector(driver);
            return connector.Process(HttpContext.Request);
        }
    }
}