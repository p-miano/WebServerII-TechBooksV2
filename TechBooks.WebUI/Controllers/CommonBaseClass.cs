using System.Configuration;
using System.Web.Mvc;

namespace TechBooks.WebUI.Controllers
{
    public class CommonBaseClass : Controller
    {
        public string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["TechBooksCString"].ConnectionString;
            }
        }
    }
}
