using System.Threading.Tasks;
using Nop.Plugin.Misc.InfigoProductProvider.Domain;

namespace Nop.Plugin.Misc.InfigoProductProvider.Services;

public interface IInfigoProductProviderService
{
    Task Set(InfigoProductProviderConfiguration configuration);
    public Task<InfigoProductProviderConfiguration> GetByIdAsync(int id);
}