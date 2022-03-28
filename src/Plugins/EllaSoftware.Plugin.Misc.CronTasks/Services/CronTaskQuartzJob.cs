using System.Collections.Generic;
using System.Net.Http;
using Nop.Core;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Quartz;

namespace EllaSoftware.Plugin.Misc.CronTasks.Services
{
    [DisallowConcurrentExecution]
    public class CronTaskQuartzJob : IJob
    {
        public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            //send post data to run a schedule task
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("id", context.JobDetail.Key.Name)
            });

            HttpClient client = null;
            try
            {
                client = EngineContext.Current.Resolve<IHttpClientFactory>().CreateClient(NopHttpDefaults.DefaultHttpClient);
                var storeUrl = (await EngineContext.Current.Resolve<IStoreContext>().GetCurrentStoreAsync()).Url;
                await client.PostAsync($"{storeUrl}{CronTasksDefaults.CronTaskPath}", data);
            }
            catch { }
            finally
            {
                if (client != null)
                    client.Dispose();
            }
        }
    }
}
