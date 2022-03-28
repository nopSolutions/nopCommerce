using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcFrontend.Models
{
    public record CheckoutWarrantySelectionModel : BaseNopModel
    {
        IList<IDictionary<Product, IList<ProductAttributeValue>>> warranties { get; set; }
    }
}
