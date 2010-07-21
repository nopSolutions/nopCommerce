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

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class BlogCommentControl : BaseNopUserControl
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
                lblComment.Text = BlogManager.FormatCommentText(blogComment.CommentText);
                lblBlogCommentId.Text = blogComment.BlogCommentId.ToString();

                var customer = blogComment.Customer;
                if (customer != null)
                {
                    if (CustomerManager.AllowViewingProfiles)
                    {
                        hlUser.Text = Server.HtmlEncode(CustomerManager.FormatUserName(customer, true));
                        hlUser.NavigateUrl = SEOHelper.GetUserProfileUrl(customer.CustomerId);
                        lblUser.Visible = false;
                    }
                    else
                    {
                        lblUser.Text = Server.HtmlEncode(CustomerManager.FormatUserName(customer, true));
                        hlUser.Visible = false;
                    }

                    if (CustomerManager.AllowCustomersToUploadAvatars)
                    {
                        var customerAvatar = customer.Avatar;
                        int avatarSize = SettingManager.GetSettingValueInteger("Media.Customer.AvatarSize", 85);
                        if (customerAvatar != null)
                        {
                            string pictureUrl = PictureManager.GetPictureUrl(customerAvatar, avatarSize, false);
                            this.imgAvatar.ImageUrl = pictureUrl;
                        }
                        else
                        {
                            if (CustomerManager.DefaultAvatarEnabled)
                            {
                                string pictureUrl = PictureManager.GetDefaultPictureUrl(PictureTypeEnum.Avatar, avatarSize);
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
                    if (CustomerManager.AllowCustomersToUploadAvatars && CustomerManager.DefaultAvatarEnabled)
                    {
                        int avatarSize = SettingManager.GetSettingValueInteger("Media.Customer.AvatarSize", 85);
                        string pictureUrl = PictureManager.GetDefaultPictureUrl(PictureTypeEnum.Avatar, avatarSize);
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
