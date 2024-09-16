using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TechBooks.WebUI.Controllers
{
    public class ManageBooksController : Controller
    {
        // GET: ManageBooks
        public ActionResult Index()
        {
            return View();
        }
    }
}