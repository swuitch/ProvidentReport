using System.Configuration;
using System.Linq;
using Dapper;
using Ingres.Client;
using ProvidentUtility.Models;

namespace ProvidentUtility.Repositories
{
    public class UserRepository
    {
        public static Users GetUser(string username)
        {
            using (IngresConnection db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                db.Query("set lockmode session where readlock=nolock");
                return db.Query<Users>("SELECT a.*,b.hub_name FROM lo_stl_billing_users a " +
                                       "inner join hdmf_hub_master b on b.hub_code=a.hub_code " +
                                       "where a.username=@username", new { username = username }).SingleOrDefault();
            }
        }

        public static Users Details(string phrase)
        {
            //new Claim(ClaimTypes.Name,client.GetDisplayName(model.username) +"," +details.FirstName+","+users.username+","+ users.hub_code +","+users.branch_code), 

            string[] result = phrase.Split(',');
            Users usr=new Users();
            usr.fullname = result[0];
            usr.firstname = result[1];
            usr.username = result[2];
            usr.hub_code = result[3];
            usr.branch_code = result[4];
            return usr;
        }
    }
}