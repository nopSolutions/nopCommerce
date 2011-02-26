using System;

using Nop.Core.Domain.Messages;
using Nop.Tests;

using NUnit.Framework;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class NewsLetterSubscriptionPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_nls()
        {
            var newGuid = Guid.NewGuid();
            var now = new DateTime(2010, 01, 01);

            var nls = new NewsLetterSubscription
            {
                Email = "me@yourstore.com",
                NewsLetterSubscriptionGuid = newGuid,
                CreatedOn = now,
                Active = true
            };

            var fromDb = SaveAndLoadEntity(nls);
            fromDb.ShouldNotBeNull();
            fromDb.Email.ShouldEqual("me@yourstore.com");
            fromDb.NewsLetterSubscriptionGuid.ShouldEqual(newGuid);
            fromDb.CreatedOn.ShouldEqual(now);
            fromDb.Active.ShouldBeTrue();
        }
    }
}

