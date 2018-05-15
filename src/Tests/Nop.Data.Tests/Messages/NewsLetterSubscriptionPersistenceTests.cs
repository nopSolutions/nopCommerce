using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Messages
{
    [TestFixture]
    public class NewsLetterSubscriptionPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_nls()
        {
            var nls = this.GetTestNewsLetterSubscription();

            var fromDb = SaveAndLoadEntity(this.GetTestNewsLetterSubscription());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(nls);
        }
    }
}