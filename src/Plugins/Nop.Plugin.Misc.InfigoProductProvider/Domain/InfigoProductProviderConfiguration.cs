using Nop.Core;

namespace Nop.Plugin.Misc.InfigoProductProvider.Domain;

public class InfigoProductProviderConfiguration : BaseEntity
{
    public string UserName { get; set; }
    public string ApiBase { get; set; }
    public string ProductListUrl { get; set; }
    public string ProductDetailsUrl { get; set; }
}