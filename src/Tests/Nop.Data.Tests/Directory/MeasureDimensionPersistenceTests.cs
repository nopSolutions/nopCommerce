using Nop.Core.Domain.Directory;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Directory
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
                Ratio = 1.12345678M,
                DisplayOrder = 2,
            };

            var fromDb = SaveAndLoadEntity(measureDimension);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("inch(es)");
            fromDb.SystemKeyword.ShouldEqual("inches");
            fromDb.Ratio.ShouldEqual(1.12345678M);
            fromDb.DisplayOrder.ShouldEqual(2);
        }
    }
}
