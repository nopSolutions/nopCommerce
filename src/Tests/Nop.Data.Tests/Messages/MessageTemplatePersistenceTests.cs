using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Messages
{
    [TestFixture]
    public class MessageTemplatePersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_messageTemplate()
        {
            var mt = this.GetTestMessageTemplate();

            var fromDb = SaveAndLoadEntity(this.GetTestMessageTemplate());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(mt);
        }
    }
}