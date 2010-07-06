using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;

namespace NopSolutions.NopCommerce.Web
{
    public partial class ManufacturersPage : BaseNopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SEOHelper.RenderTitle(this, GetLocaleResourceString("PageTitle.Manufactures"), true);
            if(!IsPostBack)
            {
                dlManufacturers.DataSource = ManufacturerManager.GetAllManufacturers();
                dlManufacturers.DataBind();
            }
        }

        protected void DlManufacturers_OnItemDataBound(object sender, DataListItemEventArgs e)
        {
            if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var manufacturer = e.Item.DataItem as Manufacturer;
                string manufacturerURL = SEOHelper.GetManufacturerUrl(manufacturer);

                var hlImageLink = e.Item.FindControl("hlImageLink") as HyperLink;
                if(hlImageLink != null)
                {
                    hlImageLink.ImageUrl = PictureManager.GetPictureUrl(manufacturer.PictureId, SettingManager.GetSettingValueInteger("Media.Manufacturer.ThumbnailImageSize", 125), true);
                    hlImageLink.NavigateUrl = manufacturerURL;
                    hlImageLink.ToolTip = String.Format(GetLocaleResourceString("Media.Manufacturer.ImageLinkTitleFormat"), manufacturer.LocalizedName);
                    hlImageLink.Text = String.Format(GetLocaleResourceString("Media.Manufacturer.ImageAlternateTextFormat"), manufacturer.LocalizedName);
                }

                var hlManufacturer = e.Item.FindControl("hlManufacturer") as HyperLink;
                if(hlManufacturer != null)
                {
                    hlManufacturer.NavigateUrl = manufacturerURL;
                    hlManufacturer.ToolTip = String.Format(GetLocaleResourceString("Media.Manufacturer.ImageLinkTitleFormat"), manufacturer.LocalizedName);
                    hlManufacturer.Text = Server.HtmlEncode(manufacturer.LocalizedName);
                }
            }
        }
    }
}
