using Nop.Web.Models.Topics;

namespace Nop.Web.Factories
{
    public partial interface ITopicModelFactory
    {
        TopicModel PrepareTopicModelById(int topicId);

        TopicModel PrepareTopicModelBySystemName(string systemName);

        string PrepareTemplateViewPath(int topicTemplateId);
    }
}
