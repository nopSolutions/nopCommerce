using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

public partial record FooterModel : BaseNopModel
{
    public bool DisplayTaxShippingInfoFooter { get; set; }
    public bool HidePoweredByNopCommerce { get; set; }
    public bool IsHomePage { get; set; }
    public string StoreName { get; set; }

    public List<FooterLinkModel> LinksColumn1 { get; set; } = new();
    public List<FooterLinkModel> LinksColumn2 { get; set; } = new();
    public List<FooterLinkModel> LinksColumn3 { get; set; } = new();

    #region Nested classes

    public partial record FooterLinkModel : BaseNopEntityModel
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public int DisplayOrder { get; set; }
    }

    #endregion
}