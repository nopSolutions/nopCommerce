using System;
using System.Data.Entity;
using NUnit.Framework;
using Nop.Tests;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class SchemaTests
    {
        [Test]
        public void Can_generate_schema()
        {
            Database.SetInitializer<NopObjectContext>(null);
            var ctx = new NopObjectContext("Test");
            string result = ctx.CreateDatabaseScript();
            result.ShouldNotBeNull();
            Console.Write(result);
        }
    }
}
