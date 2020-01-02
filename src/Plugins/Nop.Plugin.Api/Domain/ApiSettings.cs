using Nop.Core.Configuration;

namespace Nop.Plugin.Api.Domain
{
    public class ApiSettings : ISettings
    {
        public bool EnableApi { get; set; }
        public bool AllowRequestsFromSwagger { get; set; }
        public bool EnableLogging { get; set; }
    }
}