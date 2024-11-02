using FluentAssertions;
using Nop.Core.Domain.Topics;
using Nop.Data;
using Nop.Web.Factories;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories;

[TestFixture]
public class TopicModelFactoryTests : BaseNopTest
{
    private readonly ITopicModelFactory _topicModelFactory;
    private readonly Topic _testTopic;

    public TopicModelFactoryTests()
    {
        _topicModelFactory = GetService<ITopicModelFactory>();
        _testTopic = GetService<IRepository<Topic>>().GetById(1);
    }

    [Test]
    public async Task CanPrepareTopicModel()
    {
        var modelByTopic = await _topicModelFactory.PrepareTopicModelAsync(_testTopic);
        modelByTopic.Should().NotBeNull();
        var modelBySystemName = await _topicModelFactory.PrepareTopicModelBySystemNameAsync(_testTopic.SystemName);
        modelBySystemName.Should().NotBeNull();
        modelByTopic.SeName.Should().NotBeNullOrEmpty();
        PropertiesShouldEqual(modelByTopic, modelBySystemName, "CustomProperties");
    }

    [Test]
    public async Task CanPrepareTemplateViewPath()
    {
        var model1 = await _topicModelFactory.PrepareTemplateViewPathAsync(1);
        var model2 = await _topicModelFactory.PrepareTemplateViewPathAsync(int.MaxValue);

        model1.Should().Be(model2);
    }
}
