using Nop.Core;
using System;
using System.Linq;
using Nop.Data;

namespace Nop.Plugin.Misc.AbcCore.Domain
{
    /// <summary>
    /// A mapping from products to a cart price. these should be unique to one product
    /// </summary>
    public partial class ProductCartPrice : BaseEntity
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public const string PRODUCTCARTPRICE_BY_PRODID_KEY = "Abc.cartprice.prodid-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        public const string PRODUCTCARTPRICE_PATTERN_KEY = "Abc.cartprice.";

        public static Func<ProductCartPrice> GetByProductIdFunc(IRepository<ProductCartPrice> repo, int productId)
        {
            return () => { return repo.Table.Where(pcp => pcp.Product_Id == productId).FirstOrDefault(); };
        }

        public virtual int Product_Id { get; set; }

        public virtual decimal CartPrice { get; set; }


    }
}