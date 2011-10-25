
namespace Nop.Services.Tasks
{
    /// <summary>
    /// Interface that should be implemented by each task
    /// </summary>
    public partial interface ITask
    {
        /// <summary>
        /// Execute task
        /// </summary>
        void Execute();
    }
}
