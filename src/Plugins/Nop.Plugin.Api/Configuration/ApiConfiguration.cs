using Nop.Core.Configuration;

namespace Nop.Plugin.Api.Configuration
{
    public class ApiConfiguration : IConfig
    {
        public int AllowedClockSkewInMinutes { get; set; } = 5;

        public string SecurityKey { get; set; } = "NowIsTheTimeForAllGoodMenToComeToTheAideOfTheirCountry";
    }
}
