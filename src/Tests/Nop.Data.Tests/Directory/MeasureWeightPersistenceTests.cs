using Nop.Core.Domain.Directory;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Directory
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
                Ratio = 1.12345678M,
                DisplayOrder = 2,
            };

            var fromDb = SaveAndLoadEntity(measureWeight);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("ounce(s)");
            fromDb.SystemKeyword.ShouldEqual("ounce");
            fromDb.Ratio.ShouldEqual(1.12345678M);
            fromDb.DisplayOrder.ShouldEqual(2);
        }
    }
}
