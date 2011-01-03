using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class MeasureDimensionPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_measureDimension()
        {
            var measureDimension = new MeasureDimension
            {
                Name = "inch(es)",
                SystemKeyword = "inches",
                Ratio = 1,
                DisplayOrder = 2,
            };

            var fromDb = SaveAndLoadEntity(measureDimension);
            fromDb.Name.ShouldEqual("inch(es)");
            fromDb.SystemKeyword.ShouldEqual("inches");
            fromDb.Ratio.ShouldEqual(1);
            fromDb.DisplayOrder.ShouldEqual(2);
        }
    }
}
