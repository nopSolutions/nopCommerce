using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain.Common;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class AddressPeristenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_address()
        {
            var address = new Address { FirstName = "Test" };

            var fromDb = SaveAndLoadEntity(address);
            fromDb.ShouldNotBeNull();
        }

    }
}
