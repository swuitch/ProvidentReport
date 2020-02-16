using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace ProvidentUtility.Models
{
    public class Employer
    {

        public string employerid { get; set; }
        public string pagibig_erid { get; set; }
        public string eyername { get; set; }
        public string eyeraddr { get; set; }
        public string zipcode { get; set; }
        public string branch_code { get; set; }
        public string hub_code { get; set; }
        public string trackno { get; set; }
        public DateTime pick_date { get; set; }
        public string batchno { get; set; }
        public DateTime rec_date { get; set; }
        public DateTime pod_date { get; set; }
        public int status_code { get; set; }
        public int area_code { get; set; }
        public int days_delay { get; set; }
        public int num_envelope { get; set; }
        public decimal unit_price { get; set; }
        public decimal penalty { get; set; }
        public DateTime cutdate { get; set; }
        public string branch_address { get; set; }
        public string branch_contactno { get; set; }
        public DateTime statement_date { get; set; }
        public string mpl { get; set; }
        public string cal { get; set; }
        public string loan_program { get; set; }

        public string branch_name { get; set; }
        public string hub_name { get; set; }
        public int num { get; set; }
        public int rts { get; set; }
        public int numbers { get; set; }
        
    }
}