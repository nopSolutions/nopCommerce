using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Services.Custom
{
    public interface ICustomManufacturerService : IManufacturerService
    {
        Task<IList<Manufacturer>> GetManufacturersByNameAsync(string name);
    }
}