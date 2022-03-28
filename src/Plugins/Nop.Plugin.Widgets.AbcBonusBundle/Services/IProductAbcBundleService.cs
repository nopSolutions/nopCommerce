using Nop.Plugin.Widgets.AbcBonusBundle.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.AbcBonusBundle.Services
{
    public interface IProductAbcBundleService
    {
        IList<ProductAbcBundle> GetBundles(string sku);
    }
}
