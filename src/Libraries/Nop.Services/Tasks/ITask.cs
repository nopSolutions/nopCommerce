namespace Nop.Services.Tasks
{
    /// <summary>
    /// Interface that should be implemented by each task
    /// </summary>
    public partial interface ITask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        void Execute();
    }
}
