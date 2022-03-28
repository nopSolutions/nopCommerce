using Nop.Core;
using Nop.Data;
using System;
using System.Linq;

namespace Nop.Plugin.Misc.AbcCore.Domain
{
    public partial class ProductDocuments : BaseEntity
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public const string DOCUMENT_BY_PRODID_KEY = "Abc.document.prodid-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        public const string DOCUMENT_PATTERN_KEY = "Abc.document.";

        public static Func<string> GetByProductIdFunc(IRepository<ProductDocuments> repo, int prodId)
        {
            return () =>
            {
                var curDate = DateTime.UtcNow;
                return repo.Table.Where(pd => pd.ProductId == prodId).Select(pd => pd.Documents).FirstOrDefault();
            };
        }

        public virtual int ProductId { get; set; }
        public virtual string Documents { get; set; }
    }
}