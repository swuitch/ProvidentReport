using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProvidentUtility.Models
{
    public class Users
    {
        public int id { get; set; }
        public string fullname { get; set; }
        public string firstname { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string hub_code { get; set; }
        public string branch_code { get; set; }
        public string email { get; set; }
        public string hub_name { get; set; }
        public string branch_name { get; set; }
        public DateTime registration_date { get; set; }
    }
}