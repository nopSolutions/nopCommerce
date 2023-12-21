using Nop.Core.Domain.Topics;
using Nop.Services.Caching;

namespace Nop.Services.Topics.Caching;

/// <summary>
/// Represents a topic template cache event consumer
/// </summary>
public partial class TopicTemplateCacheEventConsumer : CacheEventConsumer<TopicTemplate>
{
}