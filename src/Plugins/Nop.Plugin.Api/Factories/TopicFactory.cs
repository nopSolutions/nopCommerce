using System.Threading.Tasks;
using Nop.Core.Domain.Topics;

namespace Nop.Plugin.Api.Factories
{
    public class TopicFactory : IFactory<Topic>
    {
        public Task<Topic> InitializeAsync()
        {
            var topic = new Topic();
            return Task.FromResult(topic);
        }
    }
}
