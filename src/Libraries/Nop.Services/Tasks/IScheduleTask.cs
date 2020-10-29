namespace Nop.Services.Tasks
{
    /// <summary>
    /// Interface that should be implemented by each task
    /// </summary>
    public partial interface IScheduleTask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        System.Threading.Tasks.Task ExecuteAsync();
    }
}
