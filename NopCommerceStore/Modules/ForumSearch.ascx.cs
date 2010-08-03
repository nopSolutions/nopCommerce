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
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ForumSearchControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!String.IsNullOrEmpty(this.SearchTerms))
                    txtSearchTerm.Text = this.SearchTerms;

                this.cbAdvancedSearch.Checked = this.AdvSearch;
                this.ctrlForumSelector.SelectedForumId = this.SearchInForumId;

                FillDropDowns();

                CommonHelper.SelectListItem(this.ddlSearchWithin, this.SearchWithinId);
                CommonHelper.SelectListItem(this.ddlLimitResultsToPrevious, this.LimitResultsToPreviousId);

                BindData();
            }
        }

        protected void FillDropDowns()
        {
            ctrlForumSelector.BindData();
        }

        protected void BindData()
        {
            try
            {
                string keywords = txtSearchTerm.Text.Trim();

                if (!String.IsNullOrEmpty(keywords))
                {
                    //can be removed
                    if (String.IsNullOrEmpty(keywords))
                        throw new NopException(LocalizationManager.GetLocaleResourceString("Forum.SearchTermCouldNotBeEmpty"));

                    int searchTermMinimumLength = SettingManager.GetSettingValueInteger("Search.ForumSearchTermMinimumLength", 3);
                    if (keywords.Length < searchTermMinimumLength)
                        throw new NopException(string.Format(LocalizationManager.GetLocaleResourceString("Forum.SearchTermMinimumLengthIsNCharacters"), searchTermMinimumLength));

                    int forumId = 0;
                    ForumSearchTypeEnum searchWithin = 0;
                    int limitResultsToPrevious = 0;
                    if (cbAdvancedSearch.Checked)
                    {
                        //adv search
                        forumId = ctrlForumSelector.SelectedForumId;
                        searchWithin = (ForumSearchTypeEnum)Convert.ToInt32(ddlSearchWithin.SelectedValue);
                        limitResultsToPrevious = Convert.ToInt32(ddlLimitResultsToPrevious.SelectedValue);
                    }

                    int totalRecords = 0;
                    int pageSize = 10;
                    if (ForumManager.SearchResultsPageSize > 0)
                    {
                        pageSize = ForumManager.SearchResultsPageSize;
                    }

                    var forumTopics = ForumManager.GetAllTopics(forumId, 0, keywords, searchWithin,
                        limitResultsToPrevious, pageSize, this.CurrentPageIndex, out totalRecords);
                    if (forumTopics.Count > 0)
                    {
                        this.searchPager1.PageSize = pageSize;
                        this.searchPager1.TotalRecords = totalRecords;
                        this.searchPager1.PageIndex = this.CurrentPageIndex;

                        this.searchPager2.PageSize = pageSize;
                        this.searchPager2.TotalRecords = totalRecords;
                        this.searchPager2.PageIndex = this.CurrentPageIndex;

                        rptrSearchResults.DataSource = forumTopics;
                        rptrSearchResults.DataBind();
                    }

                    rptrSearchResults.Visible = (forumTopics.Count > 0);
                    lblNoResults.Visible = !(rptrSearchResults.Visible);
                }
                else
                {
                    rptrSearchResults.Visible = false;
                }
            }
            catch (Exception exc)
            {
                rptrSearchResults.Visible = false;
                lblError.Text = Server.HtmlEncode(exc.Message);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string url = SEOHelper.GetForumSearchUrl(txtSearchTerm.Text);
            url = CommonHelper.ModifyQueryString(url, "adv=" + cbAdvancedSearch.Checked.ToString(), null);
            url = CommonHelper.ModifyQueryString(url, "fid=" + ctrlForumSelector.SelectedForumId.ToString(), null);
            url = CommonHelper.ModifyQueryString(url, "w=" + ddlSearchWithin.SelectedValue.ToString(), null);
            url = CommonHelper.ModifyQueryString(url, "l=" + ddlLimitResultsToPrevious.SelectedValue.ToString(), null);

            Response.Redirect(url);
        }

        protected void rptrSearchResults_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var forumTopic = e.Item.DataItem as ForumTopic;
                var customer = forumTopic.User;

                var pnlTopicImage = e.Item.FindControl("pnlTopicImage") as Panel;
                if (pnlTopicImage != null)
                {
                    switch (forumTopic.TopicType)
                    {
                        case ForumTopicTypeEnum.Normal:
                            pnlTopicImage.CssClass = "post";
                            break;
                        case ForumTopicTypeEnum.Sticky:
                            pnlTopicImage.CssClass = "poststicky";
                            break;
                        case ForumTopicTypeEnum.Announcement:
                            pnlTopicImage.CssClass = "postannoucement";
                            break;
                        default:
                            pnlTopicImage.CssClass = "post";
                            break;
                    }
                }

                var lblTopicType = e.Item.FindControl("lblTopicType") as Label;
                if (lblTopicType != null)
                {
                    switch (forumTopic.TopicType)
                    {
                        case ForumTopicTypeEnum.Sticky:
                            lblTopicType.Text = string.Format("[{0}]", GetLocaleResourceString("Forum.Sticky"));
                            break;
                        case ForumTopicTypeEnum.Announcement:
                            lblTopicType.Text = string.Format("[{0}]", GetLocaleResourceString("Forum.Announcement"));
                            break;
                        default:
                            lblTopicType.Visible = false;
                            break;
                    }
                }

                var hlTopic = e.Item.FindControl("hlTopic") as HyperLink;
                if (hlTopic != null)
                {
                    hlTopic.NavigateUrl = SEOHelper.GetForumTopicUrl(forumTopic);
                    hlTopic.Text = Server.HtmlEncode(forumTopic.Subject);
                }

                var hlTopicStarter = e.Item.FindControl("hlTopicStarter") as HyperLink;
                if (hlTopicStarter != null)
                {
                    if (customer != null && CustomerManager.AllowViewingProfiles && !customer.IsGuest)
                    {
                        hlTopicStarter.Text = Server.HtmlEncode(CustomerManager.FormatUserName(customer, true));
                        hlTopicStarter.NavigateUrl = SEOHelper.GetUserProfileUrl(customer.CustomerId);
                    }
                    else
                    {
                        hlTopicStarter.Visible = false;
                    }
                }

                var lblTopicStarter = e.Item.FindControl("lblTopicStarter") as Label;
                if (lblTopicStarter != null)
                {
                    if (customer != null && (!CustomerManager.AllowViewingProfiles || customer.IsGuest))
                    {
                        lblTopicStarter.Text = Server.HtmlEncode(CustomerManager.FormatUserName(customer, true));
                    }
                    else
                    {
                        lblTopicStarter.Visible = false;
                    }
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();

            this.cbAdvancedSearch.Attributes.Add("onclick", "toggleAdvancedSearch();");

            txtSearchTerm.Attributes.Add("onkeydown", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + btnSearch.ClientID + "').click();return false;}} else {return true}; ");
            base.OnPreRender(e);
        }

        public int CurrentPageIndex
        {
            get
            {
                int _pageIndex = CommonHelper.QueryStringInt(searchPager1.QueryStringProperty);
                _pageIndex--;
                if (_pageIndex < 0)
                    _pageIndex = 0;
                return _pageIndex;
            }
        }

        public string SearchTerms
        {
            get
            {
                return CommonHelper.QueryString("SearchTerms");
            }
        }

        public bool AdvSearch
        {
            get
            {
                return CommonHelper.QueryStringBool("adv");
            }
        }

        public int SearchInForumId
        {
            get
            {
                return CommonHelper.QueryStringInt("fid");
            }
        }

        public int SearchWithinId
        {
            get
            {
                return CommonHelper.QueryStringInt("w");
            }
        }

        public int LimitResultsToPreviousId
        {
            get
            {
                return CommonHelper.QueryStringInt("l");
            }
        }
    }
}
