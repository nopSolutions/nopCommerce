using System.Threading.Tasks;
using AutoMapper;
using Nop.Plugin.Misc.InfigoProductProvider.Models;

namespace Nop.Plugin.Misc.InfigoProductProvider.Factories;

public class ConfigurationModelFactory : IConfigurationModelFactory
{
    private readonly IMapper _mapper;

    public ConfigurationModelFactory(IMapper mapper)
    {
        _mapper = mapper;
    }

    public ConfigurationModel PrepareConfigurationModel(ConfigurationModel model,
        InfigoProductProviderConfiguration configuration)
    {
        if (configuration != null)
        {
            if (model == null)
            {
                model = _mapper.Map<InfigoProductProviderConfiguration, ConfigurationModel>(configuration);
            }
        }

        return model;
    }
}