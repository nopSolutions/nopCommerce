using Nop.Plugin.Misc.InfigoProductProvider.Models;

namespace Nop.Plugin.Misc.InfigoProductProvider.Factories;

public interface IConfigurationModelFactory
{
    public ConfigurationModel PrepareConfigurationModel(ConfigurationModel model,
        InfigoProductProviderConfiguration configuration);
}