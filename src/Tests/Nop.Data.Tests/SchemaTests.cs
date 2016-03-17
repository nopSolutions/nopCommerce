using System.Data.Entity;
using Nop.Tests;
using NUnit.Framework;

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
        }
    }
}
