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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ForumInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            Forum forum = ForumManager.GetForumById(this.ForumId);
            if (forum != null)
            {
                CommonHelper.SelectListItem(this.ddlForumGroup, forum.ForumGroupId);

                this.txtName.Text = forum.Name;
                this.txtDescription.Text = forum.Description;
                this.txtDisplayOrder.Value = forum.DisplayOrder;
                
                this.pnlCreatedOn.Visible = true;
                this.lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(forum.CreatedOn, DateTimeKind.Utc).ToString();
                this.pnlUpdatedOn.Visible = true;
                this.lblUpdatedOn.Text = DateTimeHelper.ConvertToUserTime(forum.UpdatedOn, DateTimeKind.Utc).ToString();

            }
            else
            {
                this.pnlCreatedOn.Visible = false;
                this.pnlUpdatedOn.Visible = false;
            }
        }

        private void FillDropDowns()
        {
            this.ddlForumGroup.Items.Clear();
            var forumGroups = ForumManager.GetAllForumGroups();
            foreach (ForumGroup forumGroup in forumGroups)
            {
                ListItem item2 = new ListItem(forumGroup.Name, forumGroup.ForumGroupId.ToString());
                this.ddlForumGroup.Items.Add(item2);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.FillDropDowns();
                this.BindData();
            }
        }

        public Forum SaveInfo()
        {
            Forum forum = ForumManager.GetForumById(this.ForumId);
            DateTime nowDT = DateTime.UtcNow;

            if (forum != null)
            {
                forum = ForumManager.UpdateForum(forum.ForumId, int.Parse(this.ddlForumGroup.SelectedItem.Value),
                    txtName.Text, txtDescription.Text, forum.NumTopics,
                    forum.NumPosts, forum.LastTopicId, forum.LastPostId,
                    forum.LastPostUserId, forum.LastPostTime,
                    txtDisplayOrder.Value, forum.CreatedOn, nowDT);
            }
            else
            {
                forum = ForumManager.InsertForum(int.Parse(this.ddlForumGroup.SelectedItem.Value), txtName.Text,
                    txtDescription.Text, 0, 0, 0, 0, 0, null, txtDisplayOrder.Value, nowDT, nowDT);
            }

            return forum;
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                ForumManager.DeleteForum(this.ForumId);
                Response.Redirect("Forums.aspx");
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        public int ForumId
        {
            get
            {
                return CommonHelper.QueryStringInt("ForumId");
            }
        }
    }
}