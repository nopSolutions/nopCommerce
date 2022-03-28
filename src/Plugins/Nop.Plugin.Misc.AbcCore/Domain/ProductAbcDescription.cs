using Nop.Core;
using Nop.Data;
using System;
using System.Linq;

namespace Nop.Plugin.Misc.AbcCore.Domain
{
    // This entity is used in other queries external of Nop and cannot be removed.
    public partial class ProductAbcDescription : BaseEntity
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public const string PRODUCTABCDESCRIPTION_BY_PRODID_KEY = "Abc.abcdescription.prodid-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        public const string PRODUCTABCDESCRIPTION_PATTERN_KEY = "Abc.abcdescription.";

        public static Func<ProductAbcDescription> GetByProductIdFunc(IRepository<ProductAbcDescription> repo, int productId)
        {
            return () => { return repo.Table.Where(p => p.Product_Id == productId).FirstOrDefault(); };
        }

        public virtual int Product_Id { get; set; }

        public virtual string AbcDescription { get; set; }

        public virtual string AbcItemNumber { get; set; }

        public virtual bool UsesPairPricing { get; set; }

    }
}