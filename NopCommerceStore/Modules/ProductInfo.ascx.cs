//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ProductInfoControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                BindData();
        }

        protected void BindData()
        {
            var product = ProductManager.GetProductById(this.ProductId);
            if (product != null)
            {
                lProductName.Text = Server.HtmlEncode(product.LocalizedName);
                lShortDescription.Text = product.LocalizedShortDescription;
                lFullDescription.Text = product.LocalizedFullDescription;

                //manufacturers
                List<Manufacturer> manufacturers = new List<Manufacturer>();
                foreach (var pm in product.ProductManufacturers)
                {
                    var manufacturer = pm.Manufacturer;
                    if (manufacturer != null)
                        manufacturers.Add(manufacturer);
                }
                if (manufacturers.Count > 0)
                {
                    if (manufacturers.Count == 1)
                    {
                        lManufacturersTitle.Text = GetLocaleResourceString("Products.Manufacturer");
                    }
                    else
                    {
                        lManufacturersTitle.Text = GetLocaleResourceString("Products.Manufacturers");
                    }
                    rptrManufacturers.DataSource = manufacturers;
                    rptrManufacturers.DataBind();
                }
                else
                {
                    phManufacturers.Visible = false;
                }

                //pictures
                var pictures = PictureManager.GetPicturesByProductId(product.ProductId);
                if (pictures.Count > 1)
                {
                    defaultImage.ImageUrl = PictureManager.GetPictureUrl(pictures[0], SettingManager.GetSettingValueInteger("Media.Product.DetailImageSize", 300));
                    defaultImage.ToolTip = String.Format(GetLocaleResourceString("Media.Product.ImageAlternateTextFormat"), product.LocalizedName);
                    defaultImage.AlternateText = String.Format(GetLocaleResourceString("Media.Product.ImageAlternateTextFormat"), product.LocalizedName);
                    lvProductPictures.DataSource = pictures;
                    lvProductPictures.DataBind();
                }
                else if (pictures.Count == 1)
                {
                    defaultImage.ImageUrl = PictureManager.GetPictureUrl(pictures[0], SettingManager.GetSettingValueInteger("Media.Product.DetailImageSize", 300));
                    defaultImage.ToolTip = String.Format(GetLocaleResourceString("Media.Product.ImageAlternateTextFormat"), product.LocalizedName);
                    defaultImage.AlternateText = String.Format(GetLocaleResourceString("Media.Product.ImageAlternateTextFormat"), product.LocalizedName);
                    lvProductPictures.Visible = false;
                }
                else
                {
                    defaultImage.ImageUrl = PictureManager.GetDefaultPictureUrl(SettingManager.GetSettingValueInteger("Media.Product.DetailImageSize", 300));
                    defaultImage.ToolTip = String.Format(GetLocaleResourceString("Media.Product.ImageAlternateTextFormat"), product.LocalizedName);
                    defaultImage.AlternateText = String.Format(GetLocaleResourceString("Media.Product.ImageAlternateTextFormat"), product.LocalizedName);
                    lvProductPictures.Visible = false;
                }
                if(SettingManager.GetSettingValueBoolean("Media.Product.DefaultPictureZoomEnabled", false))
                {
                    var picture = product.DefaultPicture;
                    if (picture != null)
                    {
                        lnkMainLightbox.Attributes["href"] = PictureManager.GetPictureUrl(picture);
                        lnkMainLightbox.Attributes["rel"] = "lightbox-pd";
                    }
                }
            }
            else
                this.Visible = false;
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();

            string slimBox = CommonHelper.GetStoreLocation() + "Scripts/slimbox2.js";
            Page.ClientScript.RegisterClientScriptInclude(slimBox, slimBox);

            base.OnPreRender(e);
        }

        public int ProductId
        {
            get
            {
                return CommonHelper.QueryStringInt("ProductId");
            }
        }
    }
}