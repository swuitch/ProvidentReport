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
    public class NoticeRepository
    {
        public static List<Employer> GetEmployer(DateTime cutdate,string hub_code)
        {
            using (IngresConnection db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                db.Query("set lockmode session where readlock=nolock");
                return db.Query<Employer>("SELECT distinct a.*,b.address as branch_address,b.contact_no as branch_contactno FROM lo_stl_billing_employer a " +
                                          "inner join  hdmf_branches b on b.branch_code=a.branch_code " +
                                          "where a.cutdate=@cutdate and a.hub_code=@hub_code", new { cutdate = cutdate.ToString("MM/dd/yyyy"), hub_code = hub_code }).ToList();
            }
        }

        public static List<Members> GetMembers(DateTime cutdate, List<Employer> lst )
        {
            using (IngresConnection db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                List<string> er = lst.Select(a => a.employerid).ToList();
                db.Query("set lockmode session where readlock=nolock");
                var result= db.Query<Members>("SELECT distinct a.* FROM lo_stl_billing_members a " +
                                         "inner join  lo_stl_billing_employer b on b.employerid=a.employerid " +
                                         "where a.cutdate=@cutdate and a.employerid in @employerid", new { cutdate = cutdate.ToString("MM/dd/yyyy"), employerid = er }).ToList();

                return result;
            }
        }
        public static List<CutOff> GetCutOffs(string hub_code)
        {

            using (IngresConnection db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                db.Query("set lockmode session where readlock=nolock");
                return db.Query<CutOff>("select cutdate,(date_format(cutdate,'%M %Y')) as desc from lo_stl_billing_employer " +
                                          "where hub_code=@hub_code " +
                                          "group by cutdate order by cutdate desc", new {hub_code=hub_code }).ToList();
            }
            

        } 
    }
}