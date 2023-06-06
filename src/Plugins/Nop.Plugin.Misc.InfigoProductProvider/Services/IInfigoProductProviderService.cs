using Nop.Plugin.Misc.InfigoProductProvider.Domain;

namespace Nop.Plugin.Misc.InfigoProductProvider.Services;

public interface IInfigoProductProviderService
{
    void Set(InfigoProductProviderConfiguration configuration);
}