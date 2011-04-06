
using Nop.Core.Configuration;

namespace Nop.Plugin.Tax.StrikeIron
{
    public class StrikeIronTaxSettings : ISettings
    {
        /// <summary>
        /// StrikeIron User ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// StrikeIron password
        /// </summary>
        public string Password { get; set; }
    }
}