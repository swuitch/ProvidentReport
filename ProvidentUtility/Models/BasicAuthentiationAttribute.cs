using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Microsoft.SqlServer.Server;

namespace ProvidentUtility.Models
{
    public class BasicAuthentiationAttribute:AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext context)
        {
            if (context.Request.Headers.Authorization == null)
            {
                context.Response = context.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                string token = context.Request.Headers.Authorization.Parameter;
                string decodeToken = Encoding.UTF8.GetString(Convert.FromBase64String(token));

                string username = decodeToken.Split(':')[0];
                string password = decodeToken.Split(':')[1];
                if(StudentSecurity.Login(username,password))
                {
                    Thread.CurrentPrincipal=new GenericPrincipal(new GenericIdentity(username),null );
                }
                else
                {
                    context.Response = context.Request.CreateResponse(HttpStatusCode.Unauthorized); 
                }
            }
        }
    }
}