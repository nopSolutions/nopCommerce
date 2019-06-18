using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Tasks;
using Nop.Plugin.Payments.MercadoPago.FuraFila;
using Nop.Services.Tasks;

namespace Nop.Plugin.Payments.MercadoPago.Tasks
{
    public class CheckPaymentMPTask : IScheduleTask
    {
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IMPPaymentService _mpService;

        public CheckPaymentMPTask(IScheduleTaskService scheduleTaskService, IMPPaymentService mpService)
        {
            _scheduleTaskService = scheduleTaskService ?? throw new ArgumentNullException(nameof(scheduleTaskService));
            _mpService = mpService ?? throw new ArgumentNullException(nameof(mpService));
        }

        public void Execute() => _mpService.CheckPayments().GetAwaiter().GetResult();

        private ScheduleTask GetScheduleTask()
        {
            var task = _scheduleTaskService.GetTaskByType(GetTaskType());
            if (task == null)
            {
                task = new ScheduleTask();
                task.Type = GetTaskType();
                task.Name = "Mercado Pago Check Payments";
                task.Enabled = true;
                task.StopOnError = false;
                task.Seconds = 600;
            }
            return task;
        }

        protected string GetTaskType() => "Nop.Plugin.Payments.MercadoPago.Tasks.CheckPaymentMPTask";

        public void InstallTask()
        {
            var scheduleTask = GetScheduleTask();
            if (scheduleTask.Id > 0)
                _scheduleTaskService.UpdateTask(scheduleTask);
            else
                _scheduleTaskService.InsertTask(scheduleTask);
            RestartAllTasks();
        }

        public void UninstallTask()
        {
            var scheduleTask = GetScheduleTask();
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
