using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ProvidentUtility.Models;
using ProvidentUtility.Repositories;

namespace ProvidentUtility.Controllers
{
    public class UserAccountsController : Controller
    {
        // GET: UserAccounts
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UserData()
        {

            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 100 };
            return new ContentResult()
            {
                Content = serializer.Serialize(new { data = UserRepository.GetAllUsers() }),
                ContentType = "json",
            };

        }
    }
}