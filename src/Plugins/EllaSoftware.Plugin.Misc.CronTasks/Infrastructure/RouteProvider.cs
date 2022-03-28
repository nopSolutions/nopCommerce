using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace EllaSoftware.Plugin.Misc.CronTasks.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //add route for the access token callback
            endpointRouteBuilder.MapControllerRoute("EllaSoftware.Plugin.Misc.CronTasks.RunCronTask", CronTasksDefaults.CronTaskPath,
                new { controller = "RunCronTask", action = "Index" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}
