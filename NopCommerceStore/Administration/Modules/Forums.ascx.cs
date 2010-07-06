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
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ForumControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        protected void btnAddNewForum_Click(object sender, EventArgs e)
        {
            Response.Redirect("ForumAdd.aspx");
        }

        void BindData()
        {
            var forumGroups = ForumManager.GetAllForumGroups();
            btnAddNewForum.Visible = forumGroups.Count > 0;

            rptrForumGroups.DataSource = forumGroups;
            rptrForumGroups.DataBind();
        }

        protected void rptrForumGroups_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Repeater rptrForums = (Repeater)e.Item.FindControl("rptrForums");

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ForumGroup forumGroup = (ForumGroup)e.Item.DataItem;

                rptrForums.DataSource = forumGroup.Forums;
                rptrForums.DataBind();
            }
        }
    }
}