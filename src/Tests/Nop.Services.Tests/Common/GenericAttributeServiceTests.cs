using System;
using Moq;
using Nop.Core.Events;
using Nop.Services.Common;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Common
{
    [TestFixture]
    public class GenericAttributeServiceTests
    {
        private IGenericAttributeService _genericAttributeService;
        private Mock<IEventPublisher> _eventPublisher;
        private Mock<Data.IRepository<Core.Domain.Common.GenericAttribute>> _repository;

        [SetUp]
        public void SetUp()
        {
            _eventPublisher = new Mock<IEventPublisher>();
            _repository = new Mock<Data.IRepository<Core.Domain.Common.GenericAttribute>>();

            _genericAttributeService = new GenericAttributeService(_repository.Object,
                new TestCacheManager()
            );
        }

        [Test]
        public void Should_Set_CreatedOrUpdatedDateUTC_In_InsertAttribute()
        {
            var attribute = new Core.Domain.Common.GenericAttribute
            {
                Key = "test",
                CreatedOrUpdatedDateUTC = null
            };

            _genericAttributeService.InsertAttribute(attribute);

            Assert.That(attribute.CreatedOrUpdatedDateUTC, 
                Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }

        [Test]
        public void Should_Update_CreatedOrUpdatedDateUTC_In_UpdateAttribute()
        {
            var attribute = new Core.Domain.Common.GenericAttribute
            {
                Key = "test",
                CreatedOrUpdatedDateUTC = DateTime.UtcNow.AddDays(-30)
            };

            _genericAttributeService.UpdateAttribute(attribute);

            Assert.That(attribute.CreatedOrUpdatedDateUTC,
                Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        }
    }
}
