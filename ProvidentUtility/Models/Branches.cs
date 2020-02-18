using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace ProvidentUtility.Models
{
    public class Branches
    {
        
        public string branch_code { get; set; }
        public string branch_name { get; set; }
        public string hub_code { get; set; }
   
    }
}