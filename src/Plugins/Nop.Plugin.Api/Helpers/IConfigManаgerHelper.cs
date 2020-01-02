using Nop.Core.Data;

namespace Nop.Plugin.Api.Helpers
{
    public interface IConfigManagerHelper
    {
        void AddBindingRedirects();
        void AddConnectionString();
        DataSettings DataSettings { get; }
    }
}