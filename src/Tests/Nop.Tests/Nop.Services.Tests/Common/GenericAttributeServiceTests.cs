using Nop.Services.Common;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Common
{
    [TestFixture]
    public class GenericAttributeServiceTests : ServiceTest
    {
        private IGenericAttributeService _genericAttributeService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _genericAttributeService = GetService<IGenericAttributeService>();
        }

        [Test]
        public async Task ShouldSetCreatedOrUpdatedDateUtcInInsertAttribute()
        {
            var attribute = new global::Nop.Core.Domain.Common.GenericAttribute
            {
                Key = "test",
                KeyGroup = "test",
                Value = "test",
                CreatedOrUpdatedDateUTC = null
            };

            await _genericAttributeService.InsertAttributeAsync(attribute);

            var createdOrUpdatedDate = attribute.CreatedOrUpdatedDateUTC;

            await _genericAttributeService.DeleteAttributeAsync(attribute);

            Assert.That(createdOrUpdatedDate,
                Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }

        [Test]
        public async Task ShouldUpdateCreatedOrUpdatedDateUtcInUpdateAttribute()
        {
            var attribute = new global::Nop.Core.Domain.Common.GenericAttribute { Key = "test", KeyGroup = "test", Value = "test" };

            await _genericAttributeService.InsertAttributeAsync(attribute);
            attribute.CreatedOrUpdatedDateUTC = DateTime.UtcNow.AddDays(-30);
            await _genericAttributeService.UpdateAttributeAsync(attribute);

            var createdOrUpdatedDate = attribute.CreatedOrUpdatedDateUTC;

            await _genericAttributeService.DeleteAttributeAsync(attribute);

            Assert.That(createdOrUpdatedDate,
                Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }
    }
}
