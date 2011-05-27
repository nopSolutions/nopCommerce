
using Nop.Core.Configuration;

namespace Nop.Plugin.SMS.Verizon
{
    public class VerizonSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the Verizon email
        /// </summary>
        public string Email { get; set; }
    }
}