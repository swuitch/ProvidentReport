using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Dapper;
using Ingres.Client;
using ProvidentUtility.Models;

namespace ProvidentUtility.Repositories
{
    public class OtherRepository
    {
        public static List<Hub> GetAllHub()
        {
            using (IngresConnection db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                db.Query("set lockmode session where readlock=nolock");
                return db.Query<Hub>("SELECT * FROM hdmf_hub_master").ToList();
            }
        }

        public static List<Branches> GetAllBranches(string hub_code)
        {
            using (IngresConnection db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                db.Query("set lockmode session where readlock=nolock");
                return db.Query<Branches>("SELECT * FROM hdmf_branches where hub_code=@hub_code",new{hub_code=hub_code}).ToList();
            }
        } 
    }
}