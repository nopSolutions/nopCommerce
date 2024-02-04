using System.Threading.Tasks;

namespace Nop.Plugin.Api.Factories
{
    public interface IFactory<T>
    {
        Task<T> InitializeAsync();
    }
}
