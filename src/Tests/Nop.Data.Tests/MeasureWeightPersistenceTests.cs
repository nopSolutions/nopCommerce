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
    public class MeasureWeightPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_measureWeight()
        {
            var measureWeight = new MeasureWeight
            {
                Name = "ounce(s)",
                SystemKeyword = "ounce",
                Ratio = 1,
                DisplayOrder = 2,
            };

            var fromDb = SaveAndLoadEntity(measureWeight);
            fromDb.Name.ShouldEqual("ounce(s)");
            fromDb.SystemKeyword.ShouldEqual("ounce");
            fromDb.Ratio.ShouldEqual(1);
            fromDb.DisplayOrder.ShouldEqual(2);
        }
    }
}
