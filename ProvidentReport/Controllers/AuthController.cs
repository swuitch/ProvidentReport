using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using ProvidentReport.Models;
using ProvidentReport.Repositories;
using ProvidentUtility.DomainService;

namespace ProvidentReport.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Users model)
        {
            ServiceSoapClient client = new ServiceSoapClient();
            
            //if (client.AuthenticateUser(model.username,model.password) == "True")
            if (true)
            {
                var users = UserRepository.GetUser(model.username);
                if (users!=null)
                {
                    //UserNameDetails details = client.GetUserNameDetailsViaLoginName(model.username);


                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name,"Anthony Carl R. Meniado" +"," +"Anthony Carl"+","+users.username+","+ users.hub_code +","+users.branch_code), 
                        //new Claim(ClaimTypes.Name,client.GetDisplayName(model.username) +"," +details.FirstName+","+users.username+","+ users.hub_code +","+users.branch_code), 
                        new Claim(ClaimTypes.Email,model.username + "@pagibigfund.gov.ph"),
                    
                    }, "ApplicationCookie");


                    var ctx = Request.GetOwinContext();
                    var authManager = ctx.Authentication;
                    authManager.SignIn(identity);
                    return RedirectToAction("index", "home");
                }
                else
                {
                    ModelState.AddModelError("", "You are not registered to the system. Please contact your System Administrator.");
                }
                
            }
            ModelState.AddModelError("", "The Username and password you entered don't match.");
            return View();
        }


        [HttpPost]
        public ActionResult Logout()
        {
            
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            //authManager.User.Claims.ToList().ForEach(claim => Context.Entry(claim).State =
            //                 System.Data.Entity.EntityState.Deleted);

            //Context.SaveChanges();
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            authManager.SignOut("ApplicationCookie");
            

            return RedirectToAction("Index", "Home");
        }

    }
}