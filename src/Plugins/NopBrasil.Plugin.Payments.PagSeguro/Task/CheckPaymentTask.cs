using Nop.Core.Domain.Tasks;
using Nop.Services.Tasks;
using NopBrasil.Plugin.Payments.PagSeguro.Services;

namespace NopBrasil.Plugin.Payments.PagSeguro.Task
{
    public class CheckPaymentTask : IScheduleTask
    {
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IPagSeguroService _pagSeguroService;

        public CheckPaymentTask(IScheduleTaskService scheduleTaskService, IPagSeguroService pagSeguroService)
        {
            this._pagSeguroService = pagSeguroService;
            this._scheduleTaskService = scheduleTaskService;
        }

        public void Execute() => _pagSeguroService.CheckPayments();

        private ScheduleTask GetScheduleTask()
        {
            ScheduleTask task = _scheduleTaskService.GetTaskByType(GetTaskType());
            if (task == null)
            {
                task = new ScheduleTask();
                task.Type = GetTaskType();
                task.Name = "PagSeguro Check Payments";
                task.Enabled = true;
                task.StopOnError = false;
                task.Seconds = 600;
            }
            return task;
        }

        protected string GetTaskType() => "NopBrasil.Plugin.Payments.PagSeguro.Task.CheckPaymentTask";

        public void InstallTask()
        {
            ScheduleTask scheduleTask = GetScheduleTask();
            if (scheduleTask.Id > 0)
                _scheduleTaskService.UpdateTask(scheduleTask);
            else
                _scheduleTaskService.InsertTask(scheduleTask);
            RestartAllTasks();
        }

        public void UninstallTask()
        {
            ScheduleTask scheduleTask = GetScheduleTask();
            if (scheduleTask.Id > 0)
            {
                _scheduleTaskService.DeleteTask(scheduleTask);
                RestartAllTasks();
            }
        }

        private void RestartAllTasks()
        {
            TaskManager.Instance.Stop();
            TaskManager.Instance.Initialize();
            TaskManager.Instance.Start();
        }
    }
}
