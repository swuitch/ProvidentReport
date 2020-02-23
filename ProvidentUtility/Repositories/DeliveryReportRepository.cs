using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using Dapper;
using Ingres.Client;
using ProvidentUtility.Models;

namespace ProvidentUtility.Repositories
{
    public class DeliveryReportRepository
    {
        private static ObjectCache _cache = MemoryCache.Default;
        private object _lock = new object();



        public static List<Employer> ReportHQPSLF134(string batchno)
        {


            using (IngresConnection db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                List<Employer> data = new List<Employer>();
                db.Open();
                try
                {
                    //if (data == null && search_status == true)
                    //{
                    db.Query("set lockmode session where readlock=nolock");

                    if (batchno.Contains("ER"))
                    {
                        var output = db.Query<Employer>("select e.branch_name,f.hub_name,a.trackno, " +
                                                        "a.cutdate,a.num_envelope as num,a.batchno, " +
                                                        "a.unit_price,a.penalty,a.status_code,a.area_code," +
                                                        "a.days_delay " +
                                                        "from lo_stl_billing_employer a " +
                                                        "inner join hdmf_branches e on e.branch_code=a.branch_code " +
                                                        "inner join hdmf_hub_master f on f.hub_code=a.hub_code " +
                                                        "where batchno=@batchno",
                                                        new { batchno = batchno }).ToList();

                        foreach (var item in output.OrderBy(a => a.status_code).ToList())
                        {
                            var abc = data.ToList();
                            var c = data.Where(b => b.branch_name == item.branch_name && b.area_code == item.area_code).SingleOrDefault();
                            if (c == null)
                            {
                                data.Add(new Employer()
                                {

                                    hub_name = item.hub_name,
                                    trackno = item.trackno,
                                    branch_name = item.branch_name,
                                    area_code = item.area_code,
                                    cutdate = item.cutdate,
                                    days_delay = item.days_delay,
                                    unit_price = item.unit_price,
                                    penalty = item.penalty,
                                    num = (item.status_code == 9) ? item.num : 0,
                                    rts = (item.status_code == 10) ? item.num : 0
                                });
                            }
                            else
                            {
                                c.num = (item.status_code == 9) ? item.num + c.num : c.num;
                                c.rts = (item.status_code == 10) ? c.rts + item.num : 0;
                            }
                        }

                    }
                    else
                    {
                        var output = db.Query<Employer>("select a.penalty,a.trackno,a.batchno,a.status_code," +
                                                        "a.area_code,e.branch_name,a.cutdate," +
                                                        "a.unit_price,f.hub_name,count(*) as num " +
                                                        "from lo_stl_billing_members a " +
                                                        "inner join hdmf_branches e on e.branch_code=a.branch_code " +
                                                        "inner join hdmf_hub_master f on f.hub_code=a.hub_code " +
                                                        "where indiv_payor=1  and batchno=@batchno " +
                                                        "group by a.penalty,a.trackno,a.status_code," +
                                                        "a.area_code,e.branch_name,a.cutdate," +
                                                        "a.unit_price,f.hub_name,a.batchno order by a.status_code", new { batchno = batchno }).ToList();


                        foreach (var item in output)
                        {
                            var c = data.Where(b => b.branch_name == item.branch_name && b.area_code == item.area_code).SingleOrDefault();
                            if (c == null)
                            {
                                data.Add(new Employer()
                                {

                                    hub_name = item.hub_name,
                                    trackno = item.trackno,
                                    branch_name = item.branch_name,
                                    area_code = item.area_code,
                                    cutdate = item.cutdate,
                                    days_delay = item.days_delay,
                                    unit_price = item.unit_price,
                                    penalty = item.penalty,
                                    num = (item.status_code == 9) ? item.num : 0,
                                    rts = (item.status_code == 10) ? item.num : 0
                                });
                            }
                            else
                            {
                                c.num = (item.status_code == 9) ? item.num : c.num;
                                c.rts = (item.status_code == 10) ? item.num : 0;
                            }
                        }

                    }




                    db.Close();
                    db.Dispose();
                }
                catch (Exception ex)
                {

                }
                return data;
            }
        }
        public static List<Employer> ReportHQPSLF133(string batchno)
        {


            using (IngresConnection db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                List<Employer> data = new List<Employer>();
                db.Open();
                try
                {
                    //if (data == null && search_status == true)
                    //{
                    db.Query("set lockmode session where readlock=nolock");

                    if (batchno.Contains("ER"))
                    {
                        var output = db.Query<Employer>("select a.batchno,a.status_code,a.area_code,a.trackno,e.branch_name," +
                                                        "a.cutdate,a.unit_price,a.penalty," +
                                                        "a.days_delay,f.hub_name,a.num_envelope as num " +
                                                        "from lo_stl_billing_employer a " +
                                                        "inner join hdmf_branches e on e.branch_code=a.branch_code " +
                                                        "inner join hdmf_hub_master f on f.hub_code=a.hub_code " +
                                                        "where batchno=@batchno" +
                                                        "order by a.status_code", 
                                                        new {  batchno = batchno }).ToList();

                        foreach (var item in output.OrderBy(a => a.status_code).ToList())
                        {
                            var abc = data.ToList();
                            var c = data.Where(b => b.branch_name == item.branch_name && b.area_code == item.area_code).SingleOrDefault();
                            if (c == null)
                            {
                                data.Add(new Employer()
                                {

                                    hub_name = item.hub_name,
                                    trackno = item.trackno,
                                    branch_name = item.branch_name,
                                    area_code = item.area_code,
                                    cutdate = item.cutdate,
                                    days_delay = item.days_delay,
                                    unit_price = item.unit_price,
                                    penalty = item.penalty,
                                    num = (item.status_code == 9) ? item.num : 0,
                                    rts = (item.status_code == 10) ? item.num : 0
                                });
                            }
                            else
                            {
                                c.num = (item.status_code == 9) ? item.num + c.num : c.num;
                                c.rts = (item.status_code == 10) ? c.rts + item.num : 0;
                            }
                        }

                    }
                    else
                    {
                        var output = db.Query<Employer>("select a.penalty,a.trackno,a.batchno,a.status_code," +
                                                        "a.area_code,e.branch_name,a.cutdate," +
                                                        "a.unit_price,f.hub_name,count(*) as num " +
                                                        "from lo_stl_billing_members a " +
                                                        "inner join hdmf_branches e on e.branch_code=a.branch_code " +
                                                        "inner join hdmf_hub_master f on f.hub_code=a.hub_code " +
                                                        "where indiv_payor=1  and batchno=@batchno " +
                                                        "group by a.penalty,a.trackno,a.status_code," +
                                                        "a.area_code,e.branch_name,a.cutdate," +
                                                        "a.unit_price,f.hub_name,a.batchno order by a.status_code", new { batchno = batchno }).ToList();


                        foreach (var item in output)
                        {
                            var c = data.Where(b => b.branch_name == item.branch_name && b.area_code == item.area_code).SingleOrDefault();
                            if (c == null)
                            {
                                data.Add(new Employer()
                                {

                                    hub_name = item.hub_name,
                                    trackno = item.trackno,
                                    branch_name = item.branch_name,
                                    area_code = item.area_code,
                                    cutdate = item.cutdate,
                                    days_delay = item.days_delay,
                                    unit_price = item.unit_price,
                                    penalty = item.penalty,
                                    num = (item.status_code == 9) ? item.num : 0,
                                    rts = (item.status_code == 10) ? item.num : 0
                                });
                            }
                            else
                            {
                                c.num = (item.status_code == 9) ? item.num : c.num;
                                c.rts = (item.status_code == 10) ? item.num : 0;
                            }
                        }

                    }

   


                    db.Close();
                    db.Dispose();
                }
                catch (Exception ex)
                {

                }
                return data;
            }
        }

        public static List<Members> ReportHQPSLF132(string batchno)
        {


            using (IngresConnection db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                List<Members> data = new List<Members>();
                db.Open();
                try
                {
                    db.Query("set lockmode session where readlock=nolock");
                    if (batchno.Contains("ER"))
                    {
                        data = db.Query<Members>("select a.status_code,a.batchno,a.eyername as fname," +
                                                  "b.branch_name, int2(0) as indiv_payor,a.eyeraddr as home_address,a.zipcode from lo_stl_billing_employer a " +
                                                  "inner join hdmf_branches b on b.branch_code=a.branch_code " +
                                                  "where batchno=@batchno and a.status_code=10",new {  batchno = batchno }).ToList();
                    }
                    else
                    {
                        data = db.Query<Members>("select a.lname,a.fname,a.mid,a.name_ext,e.branch_name,a.indiv_payor, " +
                                                 " home_address,zipcode from lo_stl_billing_members a inner join hdmf_branches e on " +
                                                 "e.branch_code=a.branch_code where  and a.status_code=10 and a.indiv_payor=1 and batchno=@batchno", 
                                                 new {  batchno = batchno }).ToList();
                    }
                    db.Close();
                    db.Dispose();
                
                }
                catch (Exception ex)
                {

                }
                return data;
            }


        }

        public static List<Members> ReportHQPSLF131(string batchno)
        {


            using (IngresConnection db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                List<Members> data = new List<Members>();
                db.Open();
                try
                {
                    //if (data == null && search_status == true)
                    //{
                    db.Query("set lockmode session where readlock=nolock");
                    if (batchno.Contains("ER"))
                    {
                        data = db.Query<Members>("select a.status_code,a.area_code," +
                                                  "a.pagibig_erid,a.batchno,a.pick_date," +
                                                  "a.cutdate,a.pod_date,a.days_delay," +
                                                  "a.unit_price,a.penalty,a.eyername as fname," +
                                                  "b.branch_name, int2(0) as indiv_payor from lo_stl_billing_employer a " +
                                                  "inner join hdmf_branches b on b.branch_code=a.branch_code " +
                                                  "where batchno=@batchno",new {  batchno = batchno }).ToList();
                    }
                    else
                    {
                        data = db.Query<Members>("select a.lname,a.fname,a.mid,a.name_ext, a.pagibigid,a.batchno,a.pick_date,a.cutdate,a.pod_date, " +
                                                 "a.days_delay,a.unit_price,a.penalty,e.branch_name,a.indiv_payor " +
                                                 "from lo_stl_billing_members a inner join hdmf_branches e on " +
                                                 "e.branch_code=a.branch_code where a.indiv_payor=1 and batchno=@batchno", 
                                                 new {  batchno = batchno }).ToList();
                    }
                    db.Close();
                    db.Dispose();
                }
                catch (Exception ex)
                {

                }
                return data;
            }


        }

        public static List<Employer> GetDeliveryReport(string hub_code)
        {


            using (IngresConnection db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                List<Employer> data = new List<Employer>();
                db.Open();
                try
                {
                    var key = "batchno" + hub_code;

                    // Try to get the object from the cache
                    //data = _cache[key] as List<Employer>;
                    //if (data == null)
                    //{
                        db.Query("set lockmode session where readlock=nolock");
                        data= db.Query<Employer>("select batchno,cutdate " +
                                                  "from lo_stl_billing_employer " +
                                                  "where  ifnull(batchno,'')<>'' " +
                                                  "group by batchno,cutdate").ToList();
                       // _cache.Set(key, data, DateTimeOffset.Now.AddMinutes(1));
                   // }
                    
                }
                catch (Exception ex)
                {

                }
                db.Close();
                db.Dispose();
                return data;
            }


        }


        public static int UpdateEmployer(List<Employer> ds)
        {
           
            using (var db = new IngresConnection(ConfigurationManager.ConnectionStrings["pfmdb"].ConnectionString))
            {
                db.Open();
                //if (exist.Equals(0) && ds.First().batchno.Contains("ER"))
                //{

                    string employer = "update lo_stl_billing_employer " +
                                          " set batchno=@batchno, pod_date=@pod_date,status_code=@status_code,area_code=@area_code," +
                                      "num_envelope=@num_envelope,unit_price=@unit_price,days_delay=@days_delay,penalty=@penalty " +
                                          " where trackno=@trackno and pagibig_erid=@pagibig_erid";

                List<Area> area = db.Query<Area>("select * from lo_stl_billing_delivery_area").ToList();
                var to_update = (from c in ds
                    join d in area on c.area_code equals d.area_code
                    let unit_price = d.er_price
                    let days_delay = (c.pod_date - c.pick_date).TotalDays-(+d.delivery_days + d.grace_period)
                    let penalty = unit_price*days_delay*d.penalty_rate
                    select new
                    {
                        c.num_envelope,
                        c.area_code,
                        c.batchno,
                        c.trackno,
                        c.pagibig_erid,
                        c.pod_date,
                        c.status_code,
                        unit_price,
                        days_delay,
                        penalty,
                        c.pick_date
                    }).ToList();
                db.Query("set lockmode session where readlock=nolock");
                db.Execute(employer, to_update);

                var batchno = ds.Select(a => a.batchno).FirstOrDefault();
                Employer result= db.Query<Employer>("select sum(num_envelope) as num from lo_stl_billing_employer " +
                         "where batchno=@batchno", new {batchno =batchno }).SingleOrDefault();
                
                //}
                //else
                //{

                //    string ip = "update lo_stl_billing_recap " +
                //                          " set batchno=@batchno, pod_date=@pod_date,status_code=@status_code,area_code=@area_code " +
                //                          " where trackno=@trackno and pagibigid=@pagibigid";


                //    db.Execute(ip, ds);
                //}
                db.Dispose();
                return  Convert.ToInt32(result.num);
            }
        }
    }
}