using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Tests;

using NUnit.Framework;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class LogPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_log()
        {
            var log = new Log
            {
                LogLevel = LogLevel.Error,
                ShortMessage = "ShortMessage1",
                FullMessage = "FullMessage1",
                IpAddress = "127.0.0.1",
                CustomerId = 1,
                PageUrl = "http://www.someUrl1.com",
                ReferrerUrl = "http://www.someUrl2.com",
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };

            var fromDb = SaveAndLoadEntity(log);
            fromDb.ShouldNotBeNull();
            fromDb.LogLevel.ShouldEqual(LogLevel.Error);
            fromDb.ShortMessage.ShouldEqual("ShortMessage1");
            fromDb.FullMessage.ShouldEqual("FullMessage1");
            fromDb.IpAddress.ShouldEqual("127.0.0.1");
            fromDb.CustomerId.ShouldEqual(1);
            fromDb.PageUrl.ShouldEqual("http://www.someUrl1.com");
            fromDb.ReferrerUrl.ShouldEqual("http://www.someUrl2.com");
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }
    }
}
