using Nop.Plugin.Demo.Database.Domains;

namespace Nop.Plugin.Demo.Database.Services
{
    internal interface ILocationService
    {
        void Log(Location record);
    }
}