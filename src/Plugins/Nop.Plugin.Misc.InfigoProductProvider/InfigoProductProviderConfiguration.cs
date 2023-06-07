using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.InfigoProductProvider;

public class InfigoProductProviderConfiguration : ISettings
{
    public string ApiUserName { get; set; }
    public string ApiBase { get; set; }
    public string ProductListUrl { get; set; }
    public string ProductDetailsUrl { get; set; }
}