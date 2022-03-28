using System;
using System.Collections.Generic;
using System.Linq;
using EllaSoftware.Plugin.Misc.CronTasks.Domain;
using EllaSoftware.Plugin.Misc.CronTasks.Infrastructure;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Tasks;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using Quartz;
using Task = System.Threading.Tasks.Task;

namespace EllaSoftware.Plugin.Misc.CronTasks.Services
{
    internal class CronTaskService : ICronTaskService
    {
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ILogger _logger;
        private readonly IScheduler _scheduler;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;

        public CronTaskService(
            IScheduleTaskService scheduleTaskService,
            IGenericAttributeService genericAttributeService,
            IStaticCacheManager staticCacheManager,
            ILogger logger,
            IScheduler scheduler,
            IRepository<GenericAttribute> genericAttributeRepository)
        {
            _scheduleTaskService = scheduleTaskService;
            _genericAttributeService = genericAttributeService;
            _staticCacheManager = staticCacheManager;
            _logger = logger;
            _scheduler = scheduler;
            _genericAttributeRepository = genericAttributeRepository;
        }

        public async System.Threading.Tasks.Task<IDictionary<int, string>> GetCronTasksAsync()
        {
            return await _staticCacheManager.GetAsync(new CacheKey(CronTasksCacheKey.CRON_TASKS_ALL_KEY), async () =>
            {
                var result = new Dictionary<int, string>();

                var cronExpressionGas = _genericAttributeRepository.Table
                        .Where(ga => ga.KeyGroup == nameof(ScheduleTask) && !string.IsNullOrEmpty(ga.Value) && ga.Key == CronTasksDefaults.CronExpressionGenericAttributeKey)
                        .ToList();

                var scheduleTasks = (await _scheduleTaskService.GetAllTasksAsync(true)).Where(t => cronExpressionGas.Select(ga => ga.EntityId).Contains(t.Id)).ToList();
                foreach (var scheduleTask in scheduleTasks)
                {
                    string cronExpression = cronExpressionGas.First(ga => ga.EntityId == scheduleTask.Id).Value;
                    result.Add(scheduleTask.Id, cronExpression);
                }

                return result;
            });
        }

        public async Task InsertCronTaskAsync(int scheduleTaskId, string cronExpression)
        {
            var ga = _genericAttributeRepository.Table
                    .FirstOrDefault(gar => gar.KeyGroup == nameof(ScheduleTask) && gar.Key == CronTasksDefaults.CronExpressionGenericAttributeKey && gar.EntityId == scheduleTaskId);

            if (ga == null)
            {
                ga = new GenericAttribute()
                {
                    StoreId = 0,
                    KeyGroup = nameof(ScheduleTask),
                    Key = CronTasksDefaults.CronExpressionGenericAttributeKey,
                    EntityId = scheduleTaskId,
                    Value = cronExpression
                };
                await _genericAttributeService.InsertAttributeAsync(ga);
            }
            else
            {
                ga.Value = cronExpression;
                await _genericAttributeService.UpdateAttributeAsync(ga);
            }

            var scheduledTask = await _scheduleTaskService.GetTaskByIdAsync(scheduleTaskId);
            if (scheduledTask != null)
            {
                scheduledTask.Enabled = false;
                await _scheduleTaskService.UpdateTaskAsync(scheduledTask);
            }

            await RescheduleQuartzJobAsync(scheduleTaskId, cronExpression);
        }

        public async System.Threading.Tasks.Task UpdateCronTaskAsync(int scheduleTaskId, string cronExpression)
        {
            var ga = _genericAttributeRepository.Table
                .FirstOrDefault(gar => gar.KeyGroup == nameof(ScheduleTask) && gar.Key == CronTasksDefaults.CronExpressionGenericAttributeKey && gar.EntityId == scheduleTaskId);

            if (ga == null)
                throw new NullReferenceException("Cron task record not found");

            ga.Value = cronExpression;
            await _genericAttributeService.UpdateAttributeAsync(ga);

            await RescheduleQuartzJobAsync(scheduleTaskId, cronExpression);
        }

        public async System.Threading.Tasks.Task DeleteCronTaskAsync(int scheduleTaskId)
        {
            var ga = _genericAttributeRepository.Table
                .FirstOrDefault(gar => gar.KeyGroup == nameof(ScheduleTask) && gar.Key == CronTasksDefaults.CronExpressionGenericAttributeKey && gar.EntityId == scheduleTaskId);

            if (ga == null)
                throw new NullReferenceException("Cron task record not found");

            await _genericAttributeService.DeleteAttributeAsync(ga);

            await RescheduleQuartzJobAsync(scheduleTaskId, null);
        }

        public async System.Threading.Tasks.Task ExecuteCronTaskAsync(ScheduleTask scheduleTask)
        {
            try
            {
                var type = Type.GetType(scheduleTask.Type) ??
                   //ensure that it works fine when only the type name is specified (do not require fully qualified names)
                   AppDomain.CurrentDomain.GetAssemblies()
                   .Select(a => a.GetType(scheduleTask.Type))
                   .FirstOrDefault(t => t != null);
                if (type == null)
                {
                    await _logger.ErrorAsync($"Type for schedule task ({scheduleTask.Type}) cannot be found");
                    return;
                }

                object instance = null;
                try
                {
                    instance = EngineContext.Current.Resolve(type);
                }
                catch
                {
                    //try resolve
                }

                if (instance == null)
                {
                    //not resolved
                    instance = EngineContext.Current.ResolveUnregistered(type);
                }

                if (!(instance is IScheduleTask task))
                    return;

                scheduleTask.LastStartUtc = DateTime.UtcNow;
                await _scheduleTaskService.UpdateTaskAsync(scheduleTask);

                await task.ExecuteAsync();

                scheduleTask.LastEndUtc = scheduleTask.LastSuccessUtc = DateTime.UtcNow;
                await _scheduleTaskService.UpdateTaskAsync(scheduleTask);
            }
            catch (Exception ex)
            {
                scheduleTask.LastEndUtc = DateTime.UtcNow;
                await _scheduleTaskService.UpdateTaskAsync(scheduleTask);

                await _logger.ErrorAsync($"Error trying to execute a CRON task \"{scheduleTask.Name}\". {ex.Message}", ex);
            }
        }

        public async System.Threading.Tasks.Task RescheduleQuartzJobAsync(int scheduleTaskId, string cronExpression)
        {
            if (!_scheduler.IsStarted)
                return;

            var jobKey = new JobKey(scheduleTaskId.ToString(), nameof(CronTasksPlugin));
            var exists = await _scheduler.CheckExists(jobKey);

            if (exists && cronExpression == null) //remove from scheduler
            {
                await _scheduler.DeleteJob(jobKey);
                return;
            }

            var triggerKey = new TriggerKey(scheduleTaskId.ToString(), nameof(CronTasksPlugin));
            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .WithCronSchedule(cronExpression, cron => { cron.InTimeZone(TimeZoneInfo.Utc); })
                .ForJob(jobKey)
                .Build();

            if (exists)
            {
                //reschedule
                await _scheduler.RescheduleJob(triggerKey, trigger);
            }
            else
            {
                //schedule new 
                var job = JobBuilder.Create<CronTaskQuartzJob>()
                    .WithIdentity(scheduleTaskId.ToString(), nameof(CronTasksPlugin))
                    .Build();

                await _scheduler.ScheduleJob(job, trigger);
            }
        }

        public DateTime? GetQuartzJobNextOccurrence(int scheduleTaskId)
        {
            var jobKey = new JobKey(scheduleTaskId.ToString(), nameof(CronTasksPlugin));
            if (!_scheduler.CheckExists(jobKey).Result)
                return null;

            var trigger = _scheduler.GetTriggersOfJob(jobKey).Result.FirstOrDefault();
            if (trigger == null)
                return null;

            var nextOccurence = trigger.GetNextFireTimeUtc();

            return nextOccurence.HasValue ? (DateTime?)nextOccurence.Value.UtcDateTime : null;
        }

        public CronTaskExecutionStatus GetQuartzJobExecutionStatus(int scheduleTaskId)
        {
            var jobKey = new JobKey(scheduleTaskId.ToString(), nameof(CronTasksPlugin));
            if (!_scheduler.CheckExists(jobKey).Result)
                return CronTaskExecutionStatus.NotFound;

            bool running = _scheduler.GetCurrentlyExecutingJobs().Result.Any(context => context.JobDetail.Key == jobKey);
            if (running)
                return CronTaskExecutionStatus.Running;
            else
                return CronTaskExecutionStatus.Waiting;
        }
    }
}
