using System;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    public interface IProductAvailabilityService
    {
        Task<bool> IsProductAvailabilityForDateAsync(Product product, DateTime dateUTC);
    }
}