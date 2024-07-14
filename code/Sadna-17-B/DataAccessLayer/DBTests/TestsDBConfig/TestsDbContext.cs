using Sadna_17_B.DomainLayer.Order;
using Sadna_17_B.DomainLayer.StoreDom;
using Sadna_17_B.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DataAccessLayer
{
    public class TestsDbContext : ApplicationDbContext
    {
        public static string TestsDbContextName = "RemoteTestsDB";
        public static bool isMemoryDB = false;
        public TestsDbContext() : base("RemoteTestsDB")
        {
        }
    }
}