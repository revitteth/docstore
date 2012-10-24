using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StorageServer;

namespace docstore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            FileArchiver dave = new FileArchiver();
            // line above is proof of working reference to StorageServer project
            return View();
        }
    }
}
