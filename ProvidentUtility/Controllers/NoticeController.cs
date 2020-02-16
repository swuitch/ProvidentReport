using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Ionic.Zip;
using ProvidentUtility.Models;
using ProvidentUtility.Repositories;

namespace ProvidentUtility.Controllers
{
    [Authorize]
    public class NoticeController : Controller
    {
        // GET: Notice
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult CuttOffData()
        {
            Users usr = UserRepository.Details(User.Identity.Name);
            return Json(NoticeRepository.GetCutOffs(usr.hub_code), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult EmployerData(DateTime cutdate)
        {
            
            Users usr = UserRepository.Details(User.Identity.Name);
            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 100 };
            return new ContentResult()
            {
                Content = serializer.Serialize(new { data = NoticeRepository.GetEmployer(cutdate.Date,usr.hub_code) }),
                ContentType = "json",
            };
            
        }

        public FileResult DownloadFile()
        {
            Users usr = UserRepository.Details(User.Identity.Name);
            Stream s = new FileStream(Path.Combine(Server.MapPath("~/Downloads/"), usr.username + "/" + usr.username + ".zip"),
                   FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.None);
            s.Seek(0, SeekOrigin.Begin);

            //return File(s, "application/xlsx", "RSMI_AND_RECAP_" + period + ".xlsx");
            return File(Path.Combine(Server.MapPath("~/Downloads/"), usr.username + "/" + usr.username + ".zip"), "application/zip", "test.zip");
        }
        [HttpPost]
        public void DownloadData(DateTime cutdate)
        {

            
            Users usr = UserRepository.Details(User.Identity.Name);
            
            Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Downloads/"),usr.username));
            //Report Initial Notice
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "HQP-SLF-034.rpt"));

            ReportDocument rd1 = new ReportDocument();
            rd1.Load(Path.Combine(Server.MapPath("~/Reports"), "HQP-SLF-049.rpt"));
            

            var er = NoticeRepository.GetEmployer(cutdate.Date, usr.hub_code);
            var em = NoticeRepository.GetMembers(cutdate.Date, er);

            var grp = (from x in em
                       group x by new { x.employerid, x.loan_program } into g
                       select new
                       {
                           employerid = g.Key.employerid,
                           loan_program = g.Key.loan_program
                       }).ToList();
            
            List<Employer> mpl_cal = (from x in grp
                                      group x by new { x.employerid } into g
                                      where g.Count() > 1
                                      select new Employer()
                                      {
                                          employerid = g.Key.employerid,
                                      }).ToList();
            var mpl = em.Where(a => a.loan_program == "MPL").Select(b => b.start_term).Max().ToString("MMMM dd, yyyy");
            var cal = em.Where(a => a.loan_program == "CAL").Select(b => b.start_term).Max().ToString("MMMM dd, yyyy");
            var query =
                    from c in grp
                    where !(from o in mpl_cal
                            select o.employerid)
                           .Contains(c.employerid)
                    select c;
            foreach (var i in mpl_cal)
            {
                er.Where(a => a.employerid == i.employerid).ToList().ForEach(b =>
                {
                    b.mpl = mpl;
                    b.cal = cal;
                });
            }
            foreach (var j in query)
            {
                if (j.loan_program == "CAL")
                {
                    er.Where(a => a.employerid == j.employerid).ToList().ForEach(b =>
                    {
                        b.mpl = null;
                        b.cal = cal;
                    });
                }
                else
                {
                    er.Where(a => a.employerid == j.employerid).ToList().ForEach(b =>
                    {
                        b.mpl = mpl;
                        b.cal = null;
                    });
                }
            }
            using (ZipFile z = new ZipFile())
            {
                z.UseZip64WhenSaving = Zip64Option.AsNecessary;
                z.ParallelDeflateThreshold = -1;
                //Employer Notice
                foreach (var ier in er)
                {
                    
                    var employerList = new List<Employer> { ier };
                    employerList.ToList().ForEach(i => i.statement_date = cutdate.AddDays(1));
                    rd.SetDataSource(employerList);

                    //rd.ExportToDisk(ExportFormatType.PortableDocFormat, Path.Combine(Server.MapPath("~/Reports"), item.pagibig_erid+".pdf"));
                    Stream stream_employer = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    string output_employer = Regex.Replace(ier.eyername, @"[^0-9a-zA-Z]+", "_");
                    output_employer = output_employer + "_" + ier.pagibig_erid + "_ER";

                    //rd.ExportToDisk(ExportFormatType.PortableDocFormat, Path.Combine(Server.MapPath("~/Downloads/"),  usr.username +"/" + output_employer +".pdf"));
                    z.AddEntry(output_employer + ".pdf", stream_employer);
                    //System.IO.File.Delete(output_employer + ".pdf");
                    stream_employer.Seek(0, SeekOrigin.Begin);

                    //List<Members> memberList = em.Where(a => a.employerid == ier.employerid).ToList();
                    List<Members> memberList = (from c in em
                                                where c.employerid == employerList.FirstOrDefault().employerid
                                                select c).ToList();

                    //Test(rd1,employerList,memberList,z);
                    rd1.Database.Tables[0].SetDataSource(er);
                    rd1.Database.Tables[1].SetDataSource(memberList);
                    
                    Stream stream_member = rd1.ExportToStream(ExportFormatType.PortableDocFormat);
                    string output_member = Regex.Replace(ier.eyername, @"[^0-9a-zA-Z]+", "_");
                    output_member = output_member + "_" + ier.pagibig_erid + "_BS";
                    //rd1.ExportToDisk(ExportFormatType.PortableDocFormat, Path.Combine(Server.MapPath("~/Downloads/"),usr.username + "/" + output_member + ".pdf"));
                    //z.AddItem(Path.Combine(Server.MapPath("~/Downloads/"), usr.username + "/" + output_member + ".pdf"));
                    z.AddEntry(output_member + ".pdf",stream_member);
                    //System.IO.File.Delete(output_member + ".pdf");
                    stream_member.Seek(0, SeekOrigin.Begin);
                }
                z.Save(Path.Combine(Server.MapPath("~/Downloads/"), usr.username + "/" + usr.username + ".zip"));
            }
            
         
         
            
            rd.Close();
            rd.Dispose();
            rd1.Close();
            rd1.Dispose();
            
            //Session["ip"] = filename;
           
            //return File(Path.Combine(Server.MapPath("~/Downloads"),"test.zip"),"application/zip",  "test.zip");
           
            




            //rd.SetParameterValue("billing_date", DateTime.Now.ToString("MMMM dd, yyyy"));
            //rd.SetParameterValue("subject", "Initial Billing Statement & Notice for the period of " + Convert.ToDateTime(TempData["datefrom"]).ToString("MMMM dd, yyyy") + " to " + Convert.ToDateTime(TempData["dateto"]).ToString("MMMM dd, yyyy"));
            //rd.SetParameterValue("mailing_date", DateTime.Now.ToString("MMMM dd, yyyy"));
            
            //rd.SetParameterValue("signatory", TempData["signatory"].ToString());
            //rd.SetParameterValue("date_from_to", Convert.ToDateTime(TempData["datefrom"]).ToString("MMMM dd") + "-" + Convert.ToDateTime(TempData["dateto"]).ToString("dd yyyy"));


            
            //return new FileStreamResult(stream, "application/pdf");
            //Stream stream = rd.ExportToDisk(ExportFormatType.PortableDocFormat,"tst.pdf");
            //stream.Seek(0, SeekOrigin.Begin);
            //rd.Close();
            //rd.Dispose();
            //GC.Collect();
            //return new FileStreamResult(stream, "application/pdf");
        }

        public void Test(ReportDocument rd1, List<Employer> er,List<Members> em,ZipFile z )
        {
            //Billing Statement

            rd1.Database.Tables[0].SetDataSource(er);
            rd1.Database.Tables[1].SetDataSource(em);

            Stream stream_member = rd1.ExportToStream(ExportFormatType.PortableDocFormat);
            string output_member = Regex.Replace(er.FirstOrDefault().eyername, @"[^0-9a-zA-Z]+", "_");
            output_member = output_member + " " + er.FirstOrDefault().pagibig_erid + " BS";
            z.AddEntry(output_member + ".pdf", stream_member);
            rd1.Close();
            rd1.Dispose();
            GC.Collect();
        }
    }
}