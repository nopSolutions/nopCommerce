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
    public partial class ForumGroupInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            ForumGroup forumGroup = ForumManager.GetForumGroupById(this.ForumGroupId);
            if (forumGroup != null)
            {
                this.txtName.Text = forumGroup.Name;
                this.txtDescription.Text = forumGroup.Description;
                this.txtDisplayOrder.Value = forumGroup.DisplayOrder;
                
                this.pnlCreatedOn.Visible = true;
                this.lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(forumGroup.CreatedOn, DateTimeKind.Utc).ToString();
                this.pnlUpdatedOn.Visible = true;
                this.lblUpdatedOn.Text = DateTimeHelper.ConvertToUserTime(forumGroup.UpdatedOn, DateTimeKind.Utc).ToString();

            }
            else
            {
                this.pnlCreatedOn.Visible = false;
                this.pnlUpdatedOn.Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        public ForumGroup SaveInfo()
        {
            ForumGroup forumGroup = ForumManager.GetForumGroupById(this.ForumGroupId);
            DateTime nowDT = DateTime.UtcNow;
            if (forumGroup != null)
            {
                forumGroup = ForumManager.UpdateForumGroup(forumGroup.ForumGroupId,
                    txtName.Text, txtDescription.Text, txtDisplayOrder.Value, forumGroup.CreatedOn, nowDT);
            }
            else
            {
                forumGroup = ForumManager.InsertForumGroup(txtName.Text,
                    txtDescription.Text, txtDisplayOrder.Value, nowDT, nowDT);
            }

            return forumGroup;
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                ForumManager.DeleteForumGroup(this.ForumGroupId);
                Response.Redirect("Forums.aspx");
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        public int ForumGroupId
        {
            get
            {
                return CommonHelper.QueryStringInt("ForumGroupId");
            }
        }
    }
}