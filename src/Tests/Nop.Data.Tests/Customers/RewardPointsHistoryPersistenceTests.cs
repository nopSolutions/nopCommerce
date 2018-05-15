using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Customers
{
    [TestFixture]
    public class RewardPointsHistoryPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_rewardPointsHistory()
        {
            var rewardPointsHistory = this.GetTestRewardPointsHistory();

            var fromDb = SaveAndLoadEntity(this.GetTestRewardPointsHistory());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(rewardPointsHistory);

            fromDb.Customer.ShouldNotBeNull();
            fromDb.Customer.PropertiesShouldEqual(rewardPointsHistory.Customer);
        }

        [Test]
        public void Can_save_and_load_rewardPointsHistory_with_order()
        {
            var rewardPointsHistory = this.GetTestRewardPointsHistory();

            var fromDb = SaveAndLoadEntity(this.GetTestRewardPointsHistory());
            fromDb.ShouldNotBeNull();
            fromDb.PropertiesShouldEqual(rewardPointsHistory);

            fromDb.UsedWithOrder.ShouldNotBeNull();
            fromDb.UsedWithOrder.PropertiesShouldEqual(rewardPointsHistory.UsedWithOrder);
        }
    }
}