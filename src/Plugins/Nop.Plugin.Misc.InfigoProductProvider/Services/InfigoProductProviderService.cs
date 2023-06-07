using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Plugin.Misc.InfigoProductProvider.Api;
using Nop.Services.Configuration;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Plugin.Misc.InfigoProductProvider.Services;

public class InfigoProductProviderService : IInfigoProductProviderService
{
    private readonly InfigoProductProviderHttpClient _infigoProductProviderHttpClient;
    private readonly ISettingService _settingService;

    public InfigoProductProviderService(InfigoProductProviderHttpClient infigoProductProviderHttpClient, ISettingService settingService)
    {
        _infigoProductProviderHttpClient = infigoProductProviderHttpClient;
        _settingService = settingService;
    }
    
    public async Task<List<int>> GetAllProductsIds()
    {
        var settings = await _settingService.LoadSettingAsync<InfigoProductProviderConfiguration>();
        var url = settings.ApiBase + settings.ProductListUrl;
        var userName = settings.ApiUserName;
        
        var data = await _infigoProductProviderHttpClient.RequestAsync(url, userName);
        
        var productIdList = JsonConvert.DeserializeObject<List<int>>(data);

        await GetProductById(productIdList[0]);
        
        return productIdList;
    }

    public async Task<ProductModel> GetProductById(int id)
    {
        var settings = await _settingService.LoadSettingAsync<InfigoProductProviderConfiguration>();
        var url = settings.ApiBase + settings.ProductDetailsUrl + $"?id={id}";
        var userName = settings.ApiUserName;

        var data = await _infigoProductProviderHttpClient.RequestAsync(url, userName);

        var product = JsonConvert.DeserializeObject<ProductModel>(data);

        return product;
    }
}