using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using Dapper;
using ProvidentUtility.Models;

namespace ProvidentUtility.Controllers.Api
{
    public class AuthController : ApiController
    {

        [Authorize]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            string username = Thread.CurrentPrincipal.Identity.Name;
            return Request.CreateResponse(HttpStatusCode.OK, username);
        }

       [HttpPost]
        public IHttpActionResult Validate(Users _users)
        {
            using (SqlConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["AppContext"].ConnectionString))
            {
                var result=db.Query<Users>("SELECT * FROM Users where username=@username",new{username=_users.username}).FirstOrDefault();
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
           
        }  

        
    }
}
