using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.InfigoProductProvider.Models;

public class ConfigurationModel
{
    [NopResourceDisplayName("Plugins.Misc.InfigoProductProvider.ApiUserName")]
    public string ApiUserName { get; set; }
    
    [NopResourceDisplayName("Plugins.Misc.InfigoProductProvider.ApiBase")]
    public string ApiBase { get; set; }
    
    [NopResourceDisplayName("Plugins.Misc.InfigoProductProvider.ProductListUrl")]
    public string ProductListUrl { get; set; }
    
    [NopResourceDisplayName("Plugins.Misc.InfigoProductProvider.ProductDetailsUrl")]
    public string ProductDetailsUrl { get; set; }
}