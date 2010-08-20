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
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class BestSellersControl : BaseNopUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            BindData();
            base.OnInit(e);
        }

        private void BindData()
        {
            if (SettingManager.GetSettingValueBoolean("Display.ShowBestsellersOnMainPage"))
            {
                int number = SettingManager.GetSettingValueInteger("Display.ShowBestsellersOnMainPageNumber");
                var report = OrderManager.BestSellersReport(720, number, 1);
                if (report.Count > 0)
                {
                    List<Product> productList = new List<Product>();
                    foreach (BestSellersReportLine line in report)
                    {
                        var productVariant = ProductManager.GetProductVariantById(line.ProductVariantId);
                        if (productVariant != null)
                        {
                            var product = productVariant.Product;
                            if (product != null)
                            {
                                bool contains = false;
                                foreach (Product p in productList)
                                {
                                    if (p.ProductId == product.ProductId)
                                    {
                                        contains = true;
                                        break;
                                    }
                                }
                                if (!contains)
                                {
                                    productList.Add(product);
                                }
                            }
                        }
                    }
                    dlCatalog.DataSource = productList;
                    dlCatalog.DataBind();
                }
                else
                {
                    this.Visible = false;
                }
            }
            else
            {
                this.Visible = false;
            }
        }

        protected void dlCatalog_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var product = e.Item.DataItem as Product;
                if(product != null)
                {
                    string productURL = SEOHelper.GetProductUrl(product);

                    var hlImageLink = e.Item.FindControl("hlImageLink") as HyperLink;
                    if(hlImageLink != null)
                    {
                        var picture = product.DefaultPicture;
                        if (picture != null)
                        {
                            hlImageLink.ImageUrl = PictureManager.GetPictureUrl(picture, SettingManager.GetSettingValueInteger("Media.Product.ThumbnailImageSize", 125), true);
                        }
                        else
                        {
                            hlImageLink.ImageUrl = PictureManager.GetDefaultPictureUrl(SettingManager.GetSettingValueInteger("Media.Product.ThumbnailImageSize", 125));
                        }

                        hlImageLink.NavigateUrl = productURL;
                        hlImageLink.ToolTip = String.Format(GetLocaleResourceString("Media.Product.ImageLinkTitleFormat"), product.LocalizedName);
                        hlImageLink.Text = String.Format(GetLocaleResourceString("Media.Product.ImageAlternateTextFormat"), product.LocalizedName);
                    }

                    var hlProduct = e.Item.FindControl("hlProduct") as HyperLink;
                    if(hlProduct != null)
                    {
                        hlProduct.NavigateUrl = productURL;
                        hlProduct.Text = Server.HtmlEncode(product.LocalizedName);
                    }
                }
            }
        }
    }
}