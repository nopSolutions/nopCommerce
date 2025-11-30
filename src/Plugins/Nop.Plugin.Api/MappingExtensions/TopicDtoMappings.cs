using Nop.Core.Domain.Topics;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTOs.Topics;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class TopicDtoMappings
    {
        public static TopicDto ToDto(this Topic topic)
        {
            return topic.MapTo<Topic, TopicDto>();
        }
    }
}
