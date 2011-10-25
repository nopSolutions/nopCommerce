namespace Nop.Core.Infrastructure
{
    public interface IStartupTask 
    {
        void Execute();

        int Order { get; }
    }
}
