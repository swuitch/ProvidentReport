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
    [Authorize]
    public class UserAccountsController : Controller
    {
        // GET: UserAccounts
        public ActionResult Index()
        {
            ViewBag.hub_code = new SelectList(OtherRepository.GetAllHub().OrderBy(a=>a.hub_name), "hub_code", "hub_name");
            return View();
        }

        public ActionResult getBranch(string hub_code)
        {
            return Json(OtherRepository.GetAllBranches(hub_code).OrderBy(a=>a.branch_name), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddUpdate(Users usr)
        {
            if (usr.id ==0)
            {
                usr.registration_date = DateTime.Now;
            }
            return Json(UserRepository.AddUpdate(usr), JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult Remove(int id)
        {
            return Json(UserRepository.Remove(id), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Edit(int id)
        {
            return Json(UserRepository.GetUserEdit(id), JsonRequestBehavior.AllowGet);
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