using System;
using System.Collections.Generic;
using System.Data.Entity.Database;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Data;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests
{
    [TestFixture]
    public abstract class ServiceTest
    {
        protected NopObjectContext context;

        [SetUp]
        public void SetUp()
        {
            DbDatabase.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
            context = new NopObjectContext(GetTestDbName());
            context.Database.Delete();
            context.Database.Create();
        }

        private string GetTestDbName()
        {
            string testDbName = "Data Source=" + (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)) + @"\\Nop.Service.Tests.Db.sdf;Persist Security Info=False";
            return testDbName;
        }
    }
}
