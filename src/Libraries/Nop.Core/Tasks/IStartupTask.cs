namespace Nop.Core.Tasks 
{
    public interface IStartupTask 
    {
        void Execute();

        int Order { get; }
    }
}
