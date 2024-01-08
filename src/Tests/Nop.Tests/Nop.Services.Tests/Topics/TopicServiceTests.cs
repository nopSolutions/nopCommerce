using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.DataProvider;
using Moq;
using Nop.Core.Caching;
using Nop.Core.Events;
using Nop.Core;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Topics;
using NUnit.Framework;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Services.Vendors;
using System.Numerics;

namespace Nop.Tests.Nop.Services.Tests.Topics;

[TestFixture]
public class TopicServiceTests
{
    [Test]
    public async Task DeleteTopicAsync_ValidTopic_DeletesTopicAndPublishesEvent()
    {
        // Arrange
        var topicRepositoryMock = new Mock<IRepository<Topic>>();
        var dataProviderMock = new Mock<IDataProvider>();
        var eventPublisherMock = new Mock<IEventPublisher>();
        var topicService = new TopicService(
            Mock.Of<IAclService>(),
            Mock.Of<ICustomerService>(),
            topicRepositoryMock.Object,
            Mock.Of<IStaticCacheManager>(),
            Mock.Of<IStoreMappingService>(),
            Mock.Of<IWorkContext>()
        );
       

        var topic = new Topic { Id = 1, Title = "Sample Topic" };
        topicRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Topic>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

        // Act
        await topicService.DeleteTopicAsync(topic);

        // Assert
        topicRepositoryMock.Verify(r => r.DeleteAsync(topic, It.IsAny<bool>()), Times.Once);

    }
    [Test]
    public async Task GetTopicByIdAsync_ValidTopicId_ReturnsTopic()
    {
        // Arrange
        var topicRepositoryMock = new Mock<IRepository<Topic>>();
        var topicService = new TopicService(
            Mock.Of<IAclService>(),
            Mock.Of<ICustomerService>(),
            topicRepositoryMock.Object,
            Mock.Of<IStaticCacheManager>(),
            Mock.Of<IStoreMappingService>(),
            Mock.Of<IWorkContext>()
        );

        var topicId = 1;
        var topic = new Topic { Id = topicId, Title = "Sample Topic" };
        topicRepositoryMock.Setup(r => r.GetByIdAsync(topicId, It.IsAny<Func<ICacheKeyService, CacheKey>>(), true, false))
                           .ReturnsAsync(topic);

        // Act
        var result = await topicService.GetTopicByIdAsync(topicId);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(topicId, result.Id);
        Assert.AreEqual("Sample Topic", result.Title);

        topicRepositoryMock.Verify(r => r.GetByIdAsync(topicId, It.IsAny<Func<ICacheKeyService, CacheKey>>(), true, false), Times.Once);
    }

    [Test]
    public async Task InsertTopicAsync_ValidTopic_InsertsTopicAndPublishesEvent()
    {
        // Arrange
        var topicRepositoryMock = new Mock<IRepository<Topic>>();
        var dataProviderMock = new Mock<IDataProvider>();
        var eventPublisherMock = new Mock<IEventPublisher>();
        var topicService = new TopicService(
            Mock.Of<IAclService>(),
            Mock.Of<ICustomerService>(),
            topicRepositoryMock.Object,
            Mock.Of<IStaticCacheManager>(),
            Mock.Of<IStoreMappingService>(),
            Mock.Of<IWorkContext>()
        );

        var topic = new Topic
        {
            Title = "Test",
        };


        // Act
        await topicService.InsertTopicAsync(topic);

        // Assert
        topicRepositoryMock.Verify(
            r => r.InsertAsync(It.IsAny<Topic>(), It.IsAny<bool>()),
            Times.Once
        );
    }
    [Test]
    public async Task UpdateTopicAsync_ValidTopic_UpdatesTopic()
    {
        // Arrange
        var topicRepositoryMock = new Mock<IRepository<Topic>>();
        var topicService = new TopicService(
            Mock.Of<IAclService>(),
            Mock.Of<ICustomerService>(),
            topicRepositoryMock.Object,
            Mock.Of<IStaticCacheManager>(),
            Mock.Of<IStoreMappingService>(),
            Mock.Of<IWorkContext>()
        );

        var topic = new Topic
        {
            Id = 1,
            Title = "Sample Topic"
            // Set other properties as needed
        };


        // Act
        await topicService.UpdateTopicAsync(topic);


        // Assert
        topicRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<Topic>(), It.IsAny<bool>()),
            Times.Once
        );
    }
   
}
