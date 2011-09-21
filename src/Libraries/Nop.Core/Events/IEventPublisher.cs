
namespace Nop.Core.Events
{
    public interface IEventPublisher
    {
        void Publish<T>(T eventMessage);
    }
}
