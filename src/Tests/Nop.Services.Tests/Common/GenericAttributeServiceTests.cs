using System;
using FluentAssertions;
using Moq;
using Nop.Core.Domain.Catalog;
using Nop.Services.Common;
using NUnit.Framework;

namespace Nop.Services.Tests.Common
{
    [TestFixture]
    public class GenericAttributeServiceTests
    {
        private IGenericAttributeService _genericAttributeService;
        private Mock<Core.Caching.ICacheManager> _cacheManager;
        private Mock<Events.IEventPublisher> _eventPublisher;
        private Mock<Core.Data.IRepository<Core.Domain.Common.GenericAttribute>> _repository;

        [SetUp]
        public void SetUp()
        {
            _cacheManager = new Mock<Core.Caching.ICacheManager>();
            _eventPublisher = new Mock<Events.IEventPublisher>();
            _repository = new Mock<Core.Data.IRepository<Core.Domain.Common.GenericAttribute>>();

            _genericAttributeService = new GenericAttributeService(
                _cacheManager.Object,
                _eventPublisher.Object,
                _repository.Object
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
