using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reyx.Web.ViewGenerator.Models
{
    public class DbConnection
    {
        public DbConnection()
        {
            Databases = new List<Database>();
            Tables = new List<Table>();
        }

        public string DataSource { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }

        public List<Database> Databases { get; set; }
        public List<Table> Tables { get; set; }
    }
}