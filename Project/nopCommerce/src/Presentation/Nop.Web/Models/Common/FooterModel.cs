using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

public partial record FooterModel : BaseNopModel
{
    public string StoreName { get; set; }
    public bool IsHomePage { get; set; }
    public bool HidePoweredByNopCommerce { get; set; }
    public bool DisplayTaxShippingInfoFooter { get; set; }
}