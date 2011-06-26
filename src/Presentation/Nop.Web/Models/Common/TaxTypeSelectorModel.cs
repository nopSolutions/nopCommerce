using Nop.Core.Domain.Tax;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Common
{
    public class TaxTypeSelectorModel : BaseNopModel
    {
        public bool Enabled { get; set; }

        public TaxDisplayType CurrentTaxType { get; set; }
    }
}