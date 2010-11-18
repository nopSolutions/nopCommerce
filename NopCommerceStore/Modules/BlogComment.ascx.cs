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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.BusinessLogic.Content.Blog;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class BlogCommentControl : BaseNopFrontendUserControl
    {
        BlogComment blogComment = null;
        
        public override void DataBind()
        {
            base.DataBind();
            this.BindData();
        }

        public void BindData()
        {
            if (blogComment != null)
            {
                lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(blogComment.CreatedOn, DateTimeKind.Utc).ToString("g");
                lblComment.Text = blogComment.FormatCommentText();
                lblBlogCommentId.Text = blogComment.BlogCommentId.ToString();

                var customer = blogComment.Customer;
                if (customer != null)
                {
                    if (this.CustomerService.AllowViewingProfiles)
                    {
                        hlUser.Text = Server.HtmlEncode(customer.FormatUserName(true));
                        hlUser.NavigateUrl = SEOHelper.GetUserProfileUrl(customer.CustomerId);
                        lblUser.Visible = false;
                    }
                    else
                    {
                        lblUser.Text = Server.HtmlEncode(customer.FormatUserName(true));
                        hlUser.Visible = false;
                    }

                    if (this.CustomerService.AllowCustomersToUploadAvatars)
                    {
                        var customerAvatar = customer.Avatar;
                        int avatarSize = this.SettingManager.GetSettingValueInteger("Media.Customer.AvatarSize", 85);
                        if (customerAvatar != null)
                        {
                            string pictureUrl = this.PictureService.GetPictureUrl(customerAvatar, avatarSize, false);
                            this.imgAvatar.ImageUrl = pictureUrl;
                        }
                        else
                        {
                            if (this.CustomerService.DefaultAvatarEnabled)
                            {
                                string pictureUrl = this.PictureService.GetDefaultPictureUrl(PictureTypeEnum.Avatar, avatarSize);
                                this.imgAvatar.ImageUrl = pictureUrl;
                            }
                            else
                            {
                                imgAvatar.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        imgAvatar.Visible = false;
                    }
                }
                else
                {
                    lblUser.Text = GetLocaleResourceString("Customer.NotRegistered");
                    hlUser.Visible = false;
                    if (this.CustomerService.AllowCustomersToUploadAvatars && this.CustomerService.DefaultAvatarEnabled)
                    {
                        int avatarSize = this.SettingManager.GetSettingValueInteger("Media.Customer.AvatarSize", 85);
                        string pictureUrl = this.PictureService.GetDefaultPictureUrl(PictureTypeEnum.Avatar, avatarSize);
                        this.imgAvatar.ImageUrl = pictureUrl;
                    }
                    else
                    {
                        imgAvatar.Visible = false;
                    }
                }
            }
        }

        public BlogComment BlogComment
        {
            get
            {
                return blogComment;
            }
            set
            {
                blogComment = value;
            }
        }

    }
}
