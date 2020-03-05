using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Media;
using Dapper;
using Ingres.Client;
using ProvidentUtility.Models;

namespace ProvidentUtility.Repositories
{
    public class UserRepository
    {

        public static List<Users>GetAllUsers()
        {
            using (IngresConnection db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                db.Query("set lockmode session where readlock=nolock");
                return db.Query<Users>("SELECT a.*,b.hub_name,c.branch_name FROM lo_stl_billing_users a " +
                                       "inner join hdmf_hub_master b on b.hub_code=a.hub_code " +
                                       "inner join hdmf_branches c on c.branch_code=a.branch_code ").ToList();
            }
        }


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


        public static Users GetUserEdit(int id)
        {
            using (IngresConnection db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                db.Query("set lockmode session where readlock=nolock");
                return db.Query<Users>("SELECT * FROM lo_stl_billing_users a " +
                                       "where id=@id", new { id = id }).SingleOrDefault();
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

        public static string AddUpdate(Users usr)
        {
            using (IngresConnection db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                db.Query("set lockmode session where readlock=nolock");
                string insert = "INSERT INTO lo_stl_billing_users(username,hub_code,branch_code,registration_date) " +
                                "VALUES (@username,@hub_code,@branch_code,@registration_date)";

                string update = "update lo_stl_billing_users " +
                                "set hub_code=@hub_code,branch_code=@branch_code " +
                                "where id=@id";
                if (usr.id == 0)
                {
                    db.Execute(insert, usr);
                }
                else
                {
                    db.Execute(update, usr);
                }
                
               
            }
            return "Ok";
        }

        public static string Remove(int id)
        {
            using (IngresConnection db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                db.Query("set lockmode session where readlock=nolock");
                db.Query("delete from lo_stl_billing_users where id=@id", new {id = id});

            }
            return "Ok";
        }
        public static void Update(Users usr)
        {
            using (IngresConnection db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                db.Query("set lockmode session where readlock=nolock");
                string update = "update lo_stl_billing_users " +
                                "set hub_code=@hub_code,branch_code=@branch_code " +
                                "where id=@id";


                db.Execute(update, usr);

            }
        }
    }
}