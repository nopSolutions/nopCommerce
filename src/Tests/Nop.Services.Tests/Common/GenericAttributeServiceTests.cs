using System;
using Nop.Services.Common;
using NUnit.Framework;

namespace Nop.Services.Tests.Common
{
    [TestFixture]
    public class GenericAttributeServiceTests : ServiceTest
    {
        private IGenericAttributeService _genericAttributeService;

        [SetUp]
        public void SetUp()
        {
            _genericAttributeService = GetService<IGenericAttributeService>();
        }

        [Test]
        public void ShouldSetCreatedOrUpdatedDateUtcInInsertAttribute()
        {
            var attribute = new Core.Domain.Common.GenericAttribute
            {
                Key = "test", KeyGroup = "test", Value = "test", CreatedOrUpdatedDateUTC = null
            };

            _genericAttributeService.InsertAttribute(attribute);

            var createdOrUpdatedDate = attribute.CreatedOrUpdatedDateUTC;

            _genericAttributeService.DeleteAttribute(attribute);

            Assert.That(createdOrUpdatedDate,
                Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }

        [Test]
        public void ShouldUpdateCreatedOrUpdatedDateUtcInUpdateAttribute()
        {
            var attribute = new Core.Domain.Common.GenericAttribute { Key = "test", KeyGroup = "test", Value = "test" };

            _genericAttributeService.InsertAttribute(attribute);
            attribute.CreatedOrUpdatedDateUTC = DateTime.UtcNow.AddDays(-30);
            _genericAttributeService.UpdateAttribute(attribute);

            var createdOrUpdatedDate = attribute.CreatedOrUpdatedDateUTC;

            _genericAttributeService.DeleteAttribute(attribute);

            Assert.That(createdOrUpdatedDate,
                Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }
    }
}
