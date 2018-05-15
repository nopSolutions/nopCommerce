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
            var measureWeight = this.GetTestMeasureWeight();

            var fromDb = SaveAndLoadEntity(this.GetTestMeasureWeight());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(measureWeight);
        }
    }
}
