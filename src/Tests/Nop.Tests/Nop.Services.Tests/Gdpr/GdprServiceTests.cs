using System.Threading.Tasks;
using Nop.Core.Domain.Gdpr;
using Nop.Services.Gdpr;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Gdpr
{
    [TestFixture]
    public class GdprServiceTests : BaseNopTest
    {
        private IGdprService _gdprService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _gdprService = GetService<IGdprService>();
        }

        [Test]
        public async Task TestCrud()
        {
            var insertItem = new GdprConsent
            {
                Message = "Test message"
            };

            var updateItem = new GdprConsent
            {
                Message = "Update test message"
            };

            await TestCrud(insertItem, _gdprService.InsertConsentAsync, updateItem, _gdprService.UpdateConsentAsync, _gdprService.GetConsentByIdAsync, (item, other) => item.Message.Equals(other.Message), _gdprService.DeleteConsentAsync);
        }
    }
}
