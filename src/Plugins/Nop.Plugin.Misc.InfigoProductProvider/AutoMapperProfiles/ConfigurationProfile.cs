using AutoMapper;
using Nop.Plugin.Misc.InfigoProductProvider.Models;

namespace Nop.Plugin.Misc.InfigoProductProvider.AutoMapperProfiles;

public class ConfigurationProfile : Profile
{
    public ConfigurationProfile()
    {
        CreateMap<InfigoProductProviderConfiguration, ConfigurationModel>();
        CreateMap<ConfigurationModel, InfigoProductProviderConfiguration>();
    }
}