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
using System.Web.UI.MobileControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class WishlistEmailAFriendControl: BaseNopFrontendUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.SettingManager.GetSettingValueBoolean("Common.EnableWishlist") || 
                !this.SettingManager.GetSettingValueBoolean("Common.EmailWishlist"))
                Response.Redirect(CommonHelper.GetStoreLocation());

            if (!Page.IsPostBack)
                BindData();
        }

        protected void BindData()
        {
            //load wishlist
            if (NopContext.Current.Session == null)
            {
                Response.Redirect(CommonHelper.GetStoreLocation());
            }
            var cart = this.ShoppingCartService.GetShoppingCartByCustomerSessionGuid(ShoppingCartTypeEnum.Wishlist, NopContext.Current.Session.CustomerSessionGuid);
            if (cart.Count > 0)
            {
                lblDescription.Text = string.Format(GetLocaleResourceString("EmailWishlist.Description"), cart.TotalProducts);

                if(NopContext.Current.User == null || NopContext.Current.User.IsGuest)
                {
                    lblEmailAFriend.Text = GetLocaleResourceString("EmailWishlist.OnlyRegisteredUsersCanEmailAFriend");
                    pnlFriendsEmail.Visible = false;
                    pnlFrom.Visible = false;
                    pnlPersonalMessage.Visible = false;
                    btnEmail.Enabled = false;
                }
                else
                {
                    lblEmailAFriend.Text = string.Empty;
                    pnlFriendsEmail.Visible = true;
                    pnlFrom.Visible = true;
                    lblFrom.Text = NopContext.Current.User.Email;
                    pnlPersonalMessage.Visible = true;
                    btnEmail.Enabled = true;
                }
            }
            else
                this.Visible = false;
        }

        protected void btnEmail_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    //load wishlist
                    if (NopContext.Current.Session == null)
                    {
                        Response.Redirect(CommonHelper.GetStoreLocation());
                    }
                    var cart = this.ShoppingCartService.GetShoppingCartByCustomerSessionGuid(ShoppingCartTypeEnum.Wishlist, NopContext.Current.Session.CustomerSessionGuid);
                    if (cart.Count == 0)
                    {
                        Response.Redirect(CommonHelper.GetStoreLocation());
                    }

                    //email it
                    if (NopContext.Current.User == null || NopContext.Current.User.IsGuest)
                    {
                        lblEmailAFriend.Text = GetLocaleResourceString("EmailWishlist.OnlyRegisteredUsersCanEmailAFriend");
                        return;
                    }

                    string friendsEmail = txtFriendsEmail.Text.Trim();
                    string personalMessage = txtPersonalMessage.Text.Trim();
                    personalMessage = personalMessage.FormatEmailAFriendText();

                    this.MessageService.SendWishlistEmailAFriendMessage(NopContext.Current.User,
                        cart, NopContext.Current.WorkingLanguage.LanguageId,
                        friendsEmail, personalMessage);

                    txtFriendsEmail.Text = string.Empty;
                    txtPersonalMessage.Text = string.Empty;
                    lblEmailAFriend.Text = GetLocaleResourceString("EmailWishlist.YourMessageHasBeenSent");
                }
                catch (Exception exc)
                {
                    lblEmailAFriend.Text = exc.Message;
                }
            }
        }
    }
}