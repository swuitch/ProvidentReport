﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProvidentUtility.Models
{
    public class Members
    {
        public string applno { get; set; }
        public string dvno { get; set; }
        public DateTime dvdate { get; set; }
        public DateTime appldate { get; set; }
        public string pagibigid { get; set; }
        public string employerid { get; set; }
        public string lname { get; set; }
        public string fname { get; set; }
        public string mid { get; set; }
        public string name_ext { get; set; }
        public double loan_granted { get; set; }
        public string loan_program { get; set; }
        public double  monthly_amort { get; set; }
        public DateTime start_term { get; set; }
        public DateTime end_term { get; set; }
        public double cap_interest { get; set; }
        public double net_proceed { get; set; }
        public double outbal { get; set; }
        public string scheme_code { get; set; }
        public string loan_desc { get; set; }
        public string user_branch_code { get; set; }
        public string home_address { get; set; }
        public string mobile_no { get; set; }
        public int grace_period { get; set; }
        public int indiv_payor { get; set; }
        public string received_by { get; set; }
        public DateTime cutdate { get; set; }
    }
}