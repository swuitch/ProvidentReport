using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using OfficeOpenXml;
using ProvidentUtility.Models;
using ProvidentUtility.Reports;
using ProvidentUtility.Repositories;

namespace ProvidentUtility.Controllers
{
    [Authorize]
    public class DeliveryReportController : Controller
    {
        // GET: DeliveryReport
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetList()
        {

            //var draw = Request.Form.GetValues("draw").FirstOrDefault();
            //var start = Request.Form.GetValues("start").FirstOrDefault();
            //var length = Request.Form.GetValues("length").FirstOrDefault();
            //List<Employer> emp = new List<Employer>();


            //int pageSize = length != null ? Convert.ToInt32(length) : 0;
            //int skip = start != null ? Convert.ToInt32(start) : 0;
            //int recordsTotal = 0;

            Users usr = UserRepository.Details(User.Identity.Name);

            var rec = DeliveryReportRepository.GetDeliveryReport(usr.hub_code);
            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 100 };
            return new ContentResult()
            {
                Content = serializer.Serialize(new{data=rec}),
                ContentType = "json",
            };

            //var data = rec.Skip(skip).Take(pageSize).ToList();
            ////var a = ws.GetEmployers(from_date, to_date, Session["HubCode"].ToString());
            //recordsTotal = rec.Count();
            //var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 100 };
            //return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.OrderBy(a => a.batchno) }, JsonRequestBehavior.AllowGet);


        }

        public FileStreamResult PrintHQPSLF131(string batchno)
        {
            
            List<Members> lst = DeliveryReportRepository.ReportHQPSLF131(batchno).OrderBy(b => b.fname).ToList();
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "HQP-SLF_131.rpt"));
            rd.SetDataSource(lst);


            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);


            rd.Dispose();
            return new FileStreamResult(stream, "application/pdf");
        }


        public FileStreamResult PrintHQPSLF132(string batchno)
        {

            List<Members> lst = DeliveryReportRepository.ReportHQPSLF132(batchno);
            
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "HQP_SLF_132.rpt"));
            rd.SetDataSource(lst);


            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            rd.Dispose();
            return new FileStreamResult(stream, "application/pdf");
        }
        public FileStreamResult PrintHQPSLF133(string batchno)
        {

            List<Employer> lst = DeliveryReportRepository.ReportHQPSLF133(batchno);
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "HQPSLF133.rpt"));
            rd.SetDataSource(lst);


            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            rd.Dispose();
            return new FileStreamResult(stream, "application/pdf");
        }


        public FileStreamResult PrintHQPSLF134(string batchno)
        {

            List<Employer> lst = DeliveryReportRepository.ReportHQPSLF134(batchno).OrderBy(b => b.branch_name).ToList();
            //DbContexC:\Users\Anthony\Documents\GitHub\ReportUtility\ReportUtility\Controllers\DeliveryReportController.cst.hub_code = Session["HubCode"].ToString();
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "HQPSLF134.rpt"));
            rd.SetDataSource(lst);


            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            rd.Dispose();
            return new FileStreamResult(stream, "application/pdf");
        }

        
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            HttpFileCollectionBase files = Request.Files;
            var f = files[0];
            using (var stream = f.InputStream)
            {
                using (var package = new ExcelPackage(stream))
                {
                    List<string> ColumnNames = new List<string>();
                    foreach (ExcelWorksheet worksheet1 in package.Workbook.Worksheets)
                    {

                        for (int i = 1; i <= worksheet1.Dimension.End.Column; i++)
                        {
                            ColumnNames.Add(worksheet1.Cells[1, i].Value.ToString()); // 1 = First Row, i = Column Number
                        }

                    }
                    var a = ColumnNames.FindIndex(x => x.StartsWith("BATCHNAME"));
                    //get the first worksheet in the workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int colCount = worksheet.Dimension.End.Column;  //get Column Count
                    int rowCount = worksheet.Dimension.End.Row;     //get row count
                    List<Employer> lst = new List<Employer>();

                    for (int i = 2; i <= rowCount; i++)
                    {
                        lst.Add(new Employer
                        {
                            batchno = worksheet.Cells[i, ColumnNames.FindIndex(x => x.Equals("BATCH"))+1].Value.ToString(),//24
                            trackno = worksheet.Cells[i, ColumnNames.FindIndex(x => x.Equals("TRACKNO"))+1].Value.ToString(),
                            pick_date = Convert.ToDateTime(worksheet.Cells[i, ColumnNames.FindIndex(x => x.Equals("PICK_DATE")) + 1].Value),
                            pod_date = Convert.ToDateTime(worksheet.Cells[i, ColumnNames.FindIndex(x => x.Equals("POD_DATE")) + 1].Value),
                            status_code = Convert.ToInt32(worksheet.Cells[i, ColumnNames.FindIndex(x => x.Equals("STATUS")) + 1].Value),
                            area_code = Convert.ToInt32(worksheet.Cells[i, ColumnNames.FindIndex(x => x.Equals("DEL_AREA")) + 1].Value),
                            branch_code = worksheet.Cells[i, ColumnNames.FindIndex(x => x.Equals("BRANCH_COD")) + 1].Value.ToString(),
                            pagibig_erid = (string)worksheet.Cells[i, ColumnNames.FindIndex(x => x.Equals("PAGIBIG_ER")) + 1].Value,
                            //num_envelope = Convert.ToInt32(worksheet.Cells[i, 33].Value),
                        });


                    }
                    var t = lst.ToList();
                    List<Employer> result = (from c in lst
                                          group c by new { c.batchno, c.trackno,c.pod_date,c.status_code,c.area_code,c.pagibig_erid,c.pick_date }
                                              into g
                                              select new Employer()
                                              {
                                                  pick_date = g.Key.pick_date,
                                                  batchno = g.Key.batchno,
                                                  trackno = g.Key.trackno,
                                                  pod_date = g.Key.pod_date,
                                                  status_code = g.Key.status_code,
                                                  area_code = g.Key.area_code,
                                                  pagibig_erid = g.Key.pagibig_erid,
                                                  num_envelope = g.Count()

                                              }).ToList();
                    //DbContext.testc(lst);
                    int num =DeliveryReportRepository.UpdateEmployer(result);
                    result.ToList().ForEach(c => c.num = num);
                    var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 100 };
                    return new ContentResult()
                    {
                        Content = serializer.Serialize(result),
                        ContentType = "json",
                    };
                }

            }
           
        }

    }
}