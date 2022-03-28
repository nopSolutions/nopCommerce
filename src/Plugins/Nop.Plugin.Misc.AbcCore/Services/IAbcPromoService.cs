using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Domain;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public interface IAbcPromoService
    {
        Task<IList<AbcPromo>> GetAllPromosAsync();
        Task<IList<AbcPromo>> GetActivePromosAsync();
        Task<IList<AbcPromo>> GetExpiredPromosAsync();
        Task<AbcPromo> GetPromoByIdAsync(int promoId);
        Task<IList<AbcPromo>> GetActivePromosByProductIdAsync(int productId);
        Task<IList<AbcPromo>> GetAllPromosByProductIdAsync(int productId);
        Task<IList<Product>> GetProductsByPromoIdAsync(int abcPromoId);
        Task<IList<Product>> GetPublishedProductsByPromoIdAsync(int abcPromoId);
        void UpdatePromo(AbcPromo promo);
    }
}