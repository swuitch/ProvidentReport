using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ProvidentUtility.Migrations;

namespace ProvidentUtility.Models
{
    public class AppContext:DbContext
    {
        public AppContext(): base("AppContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppContext,Configuration>());
        }

        public DbSet<Branches> Branches { get; set; }
        public DbSet<Hub> Hub { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Employer> Employers { get; set; }
    }
}