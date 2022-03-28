using Nop.Core;
using Nop.Data;
using System;
using System.Linq;

namespace Nop.Plugin.Misc.AbcCore.Domain
{
    /// <summary>
    /// A collection of products that require login to add to cart
    /// </summary>
    public partial class ProductRequiresLogin : BaseEntity
    {
        public virtual int Product_Id { get; set; }

        public static Func<ProductRequiresLogin> GetByProductIdFunc(IRepository<ProductRequiresLogin> repo, int productId)
        {
            return () =>
            {
                return repo.Table.Where(pd => pd.Product_Id == productId).FirstOrDefault();
            };
        }
    }
}