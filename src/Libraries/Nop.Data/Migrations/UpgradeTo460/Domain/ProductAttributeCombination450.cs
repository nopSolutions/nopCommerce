using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.UpgradeTo460.Domain
{
    public class ProductAttributeCombination450 : ProductAttributeCombination
    {
        public int PictureId { get; set; }
    }
}
