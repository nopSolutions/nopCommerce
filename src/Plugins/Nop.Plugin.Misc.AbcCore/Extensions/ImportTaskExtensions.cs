using Microsoft.AspNetCore.Hosting;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using System;
using System.IO;

namespace Nop.Plugin.Misc.AbcCore.Extensions
{
    public static class ImportTaskExtensions
    {
        public static string GetSqlScript(string name, params string[] paramList)
        {
            var env = EngineContext.Current.Resolve<IWebHostEnvironment>();
            string script = File.ReadAllText($"{env.ContentRootPath}/Plugins/Misc.AbcSync/{name}.sql");
            if (paramList == null || paramList.Length == 0)
            {
                return script;
            }

            var i = 0;
            foreach (var param in paramList)
            {
                script = script.Replace($"[param_{i}]", param);
                ++i;
            }

            return script;
        }

        public static void Skipped(this IScheduleTask task)
        {
            EngineContext.Current.Resolve<ILogger>().WarningAsync($"Skipped Task: {task.GetType().Name}");
        }

        public static void DropIndexes()
        {
            try
            {
                EngineContext.Current.Resolve<INopDataProvider>().ExecuteNonQueryAsync(GetSqlScript("Nopcommerce_Drop_IXs"));
            }
            catch (Exception ex)
            {
                EngineContext.Current.Resolve<ILogger>().WarningAsync("Error when dropping indexes", ex);
            }
        }

        public static void CreateIndexes()
        {
            EngineContext.Current.Resolve<INopDataProvider>().ExecuteNonQueryAsync(GetSqlScript("Nopcommerce_Create_IXs"));
        }
    }
}