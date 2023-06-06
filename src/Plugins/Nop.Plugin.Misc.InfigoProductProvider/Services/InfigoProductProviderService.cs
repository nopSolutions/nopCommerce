using System;
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

    public virtual void Set(InfigoProductProviderConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }
        _infigoProductProviderRepository.Insert(configuration);
    }
}