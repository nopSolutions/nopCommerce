using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.DateTimeFormat
{
    public class DateTimeFormatSettings : ISettings
    {
        public string FormatString { get; set; }
        public bool IncludeCustomerId { get; set; }
    }
}