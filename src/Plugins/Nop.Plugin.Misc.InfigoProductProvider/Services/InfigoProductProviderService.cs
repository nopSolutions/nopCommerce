using System;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Misc.InfigoProductProvider.Domain;

namespace Nop.Plugin.Misc.InfigoProductProvider.Services;

public class InfigoProductProviderService : IInfigoProductProviderService
{
    private readonly IRepository<InfigoProductProviderConfiguration> _infigoProductProviderRepository;

    public InfigoProductProviderService(IRepository<InfigoProductProviderConfiguration> infigoProductProviderRepository)
    {
        _infigoProductProviderRepository = infigoProductProviderRepository;
    }

    public virtual async Task Set(InfigoProductProviderConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var configurationEntity = await _infigoProductProviderRepository.GetByIdAsync(1);

        configurationEntity.ApiUserName = configuration.ApiUserName;
        configurationEntity.ApiBase = configuration.ApiBase;
        configurationEntity.ProductListUrl = configuration.ProductListUrl;
        configurationEntity.ProductDetailsUrl = configuration.ProductDetailsUrl;

        await _infigoProductProviderRepository.UpdateAsync(configurationEntity);
    }

    public async Task<InfigoProductProviderConfiguration> GetApiConfigurationAsync()
    {
        var configuration = await _infigoProductProviderRepository.GetByIdAsync(1);

        return configuration;
    }
}