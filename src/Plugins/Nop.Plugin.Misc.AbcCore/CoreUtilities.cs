using Microsoft.AspNetCore.Hosting;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Misc.AbcCore
{
    public static class CoreUtilities
    {
        public static string AppPath()
        {
            var env = EngineContext.Current.Resolve<IWebHostEnvironment>();
            return env.ContentRootPath;
        }

        public static string WebRootPath()
        {
            var env = EngineContext.Current.Resolve<IWebHostEnvironment>();
            return env.WebRootPath;
        }
    }
}