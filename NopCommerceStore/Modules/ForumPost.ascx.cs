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
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ForumPostControl : BaseNopUserControl
    {
        ForumPost forumPost = null;

        public override void DataBind()
        {
            base.DataBind();
            this.BindData();
        }

        public void BindData()
        {
            if (forumPost != null)
            {
                lAnchor.Text = string.Format("<a name=\"{0}\"></a>", forumPost.ForumPostId);

                btnEdit.Visible = IoC.Resolve<IForumService>().IsUserAllowedToEditPost(NopContext.Current.User, forumPost);
                btnDelete.Visible = IoC.Resolve<IForumService>().IsUserAllowedToDeletePost(NopContext.Current.User, forumPost);
                btnDelete.OnClientClick = string.Format("return confirm('{0}')", GetLocaleResourceString("Common.AreYouSure"));

                //post date
                string dateStr = string.Empty;
                if (IoC.Resolve<IForumService>().RelativeDateTimeFormattingEnabled)
                    dateStr = forumPost.CreatedOn.RelativeFormat(true, "f");
                else
                    dateStr = DateTimeHelper.ConvertToUserTime(forumPost.CreatedOn, DateTimeKind.Utc).ToString("f");
                lblDate.Text = dateStr;

                //forum text
                lText.Text = forumPost.FormatPostText();
                lblForumPostId.Text = forumPost.ForumPostId.ToString();

                var customer = forumPost.User;
                if (customer != null)
                {
                    if (IoC.Resolve<ICustomerService>().AllowViewingProfiles && !customer.IsGuest)
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

                    if (IoC.Resolve<ICustomerService>().AllowCustomersToUploadAvatars)
                    {
                        var customerAvatar = customer.Avatar;
                        int avatarSize = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Media.Customer.AvatarSize", 85);
                        if (customerAvatar != null)
                        {
                            string pictureUrl = IoC.Resolve<IPictureService>().GetPictureUrl(customerAvatar, avatarSize, false);
                            this.imgAvatar.ImageUrl = pictureUrl;
                        }
                        else
                        {
                            if (IoC.Resolve<ICustomerService>().DefaultAvatarEnabled)
                            {
                                string pictureUrl = IoC.Resolve<IPictureService>().GetDefaultPictureUrl(PictureTypeEnum.Avatar, avatarSize);
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

                    if (customer.IsForumModerator)
                    {
                        lblStatus.Text = GetLocaleResourceString("Forum.Moderator");
                    }
                    else
                    {
                        phStatus.Visible = false;
                    }

                    if(IoC.Resolve<IForumService>().ShowCustomersPostCount && !customer.IsGuest)
                    {
                        lblTotalPosts.Text = customer.TotalForumPosts.ToString();
                    }
                    else
                    {
                        phTotalPosts.Visible = false;
                    }

                    if(IoC.Resolve<ICustomerService>().ShowCustomersJoinDate && !customer.IsGuest)
                    {
                        lblJoined.Text = DateTimeHelper.ConvertToUserTime(customer.RegistrationDate, DateTimeKind.Utc).ToString("d");
                    }
                    else
                    {
                        phJoined.Visible = false;
                    }

                    if(IoC.Resolve<ICustomerService>().ShowCustomersLocation && !customer.IsGuest)
                    {
                        var country = IoC.Resolve<ICountryService>().GetCountryById(customer.CountryId);
                        if (country != null)
                        {
                            lblLocation.Text = Server.HtmlEncode(country.Name);
                        }
                        else
                        {
                            phLocation.Visible = false;
                        }
                    }
                    else
                    {
                        phLocation.Visible = false;
                    }

                    if(IoC.Resolve<IForumService>().AllowPrivateMessages && !customer.IsGuest)
                    {
                        btnSendPM.CustomerId = customer.CustomerId;
                        phPM.Visible = true;
                    }
                    else
                    {
                        phPM.Visible = false;
                    }

                    if (IoC.Resolve<IForumService>().SignaturesEnabled && !String.IsNullOrEmpty(customer.Signature))
                    {
                        lblSignature.Text = customer.Signature.FormatForumSignatureText();
                    }
                    else
                    {
                        pnlSignature.Visible = false;
                    }
                }
                else
                {
                    //error, cannot be
                }
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            int forumPostId = 0;
            int.TryParse(lblForumPostId.Text, out forumPostId);
            var forumPost = IoC.Resolve<IForumService>().GetPostById(forumPostId);
            if (forumPost != null)
            {
                if (!IoC.Resolve<IForumService>().IsUserAllowedToEditPost(NopContext.Current.User, forumPost))
                {
                    string loginURL = SEOHelper.GetLoginPageUrl(true);
                    Response.Redirect(loginURL);
                }

                string url = SEOHelper.GetEditForumPostUrl(forumPost.ForumPostId);
                Response.Redirect(url);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            int forumPostId = 0;
            int.TryParse(lblForumPostId.Text, out forumPostId);
            var forumPost = IoC.Resolve<IForumService>().GetPostById(forumPostId);
            if (forumPost != null)
            {
                var forumTopic = forumPost.Topic;
                if (!IoC.Resolve<IForumService>().IsUserAllowedToDeletePost(NopContext.Current.User, forumPost))
                {
                    string loginURL = SEOHelper.GetLoginPageUrl(true);
                    Response.Redirect(loginURL);
                }

                IoC.Resolve<IForumService>().DeletePost(forumPost.ForumPostId);

                string url = string.Empty;
                //get topic one more time because it can be deleted
                forumTopic = IoC.Resolve<IForumService>().GetTopicById(forumPost.TopicId);
                if (forumTopic != null)
                {
                    url = SEOHelper.GetForumTopicUrl(forumTopic);
                }
                else
                {
                    url = SEOHelper.GetForumMainUrl();
                }
                Response.Redirect(url);
            }
        }

        protected void BtnQuote_OnClick(object sender, EventArgs e)
        {
            int forumPostId = 0;
            int.TryParse(lblForumPostId.Text, out forumPostId);
            var forumPost = IoC.Resolve<IForumService>().GetPostById(forumPostId);
            if(forumPost != null)
            {
                Response.Redirect(SEOHelper.GetNewForumPostUrl(forumPost.TopicId, forumPost.ForumPostId));
            }
        }

        public ForumPost ForumPost
        {
            get
            {
                return forumPost;
            }
            set
            {
                forumPost = value;
            }
        }

    }
}
