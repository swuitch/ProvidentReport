using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using ProvidentUtility.DomainService;
using ProvidentUtility.Models;
using ProvidentUtility.Repositories;

namespace ProvidentUtility.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
     

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Users model)
        {
            ServiceSoapClient client = new ServiceSoapClient();

            //if (client.AuthenticateUser(model.username, model.password) == "True")
            if (true)
            {
                var users = UserRepository.GetUser(model.username);
                if (users != null)
                {
                   // UserNameDetails details = client.GetUserNameDetailsViaLoginName(model.username);


                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name,"Anthony Carl R. Meniado" +"," +"Anthony Carl"+","+users.username+","+ users.hub_code +","+users.branch_code), 
                        //new Claim(ClaimTypes.Name,
                        //    client.GetDisplayName(model.username) + "," + details.FirstName + "," + users.username + "," +
                        //    users.hub_code + "," + users.branch_code),
                        new Claim(ClaimTypes.Email, model.username + "@pagibigfund.gov.ph"),

                    }, "ApplicationCookie");


                    var ctx = Request.GetOwinContext();
                    var authManager = ctx.Authentication;
                    authManager.SignIn(identity);
                    return RedirectToAction("index", "home");
                }
                else
                {
                    ViewBag.message = "You are not registered to the system. Please contact your System Administrator.";
                }

            }
            else
            {
                ViewBag.message = "The Username and password you entered don't match.";    
            }
            
            return View();
        }


        [HttpPost]
        public ActionResult Logout()
        {
            
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            //authManager.User.Claims.ToList().ForEach(claim => Context.Entry(claim).State =
            //                 System.Data.Entity.EntityState.Deleted);
            
            System.Web.HttpContext.Current.GetOwinContext().Authentication.SignOut(
            CookieAuthenticationDefaults.AuthenticationType,
            OpenIdConnectAuthenticationDefaults.AuthenticationType);
            authManager.SignOut("ApplicationCookie");

            Request.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

    }
}