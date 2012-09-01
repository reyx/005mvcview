using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Reyx.Web.ViewGenerator.Models
{
    public class DataEntities : DbContext
    {
        public DbSet<DbConnection> DbConnections { get; set; }
    }
}