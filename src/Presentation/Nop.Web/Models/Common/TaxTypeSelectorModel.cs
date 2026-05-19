using Nop.Core.Domain.Tax;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

public partial record TaxTypeSelectorModel : BaseNopModel
{
    public TaxDisplayType CurrentTaxType { get; set; }
}