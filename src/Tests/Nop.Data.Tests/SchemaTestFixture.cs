using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Database;
using NUnit.Framework;
using Nop.Tests;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class SchemaTestFixture
    {
        [Test]
        public void Can_generate_schema()
        {
            DbDatabase.SetInitializer<NopObjectContext>(null);
            var ctx = new NopObjectContext("Test");
            string result = ctx.CreateDatabaseScript();
            result.ShouldNotBeNull();
            Console.Write(result);
        }
    }
}
