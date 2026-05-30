using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.Polls;

public class PollSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether poll plugin is enabled
    /// </summary>
    public bool Enabled { get; set; }
}
