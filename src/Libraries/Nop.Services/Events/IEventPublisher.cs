
namespace Nop.Services.Events
{
    public interface IEventPublisher
    {
        void Publish<T>(T eventMessage);
    }
}
