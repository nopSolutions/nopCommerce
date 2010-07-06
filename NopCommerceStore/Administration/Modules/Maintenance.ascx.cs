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
using NopSolutions.NopCommerce.BusinessLogic.Maintenance;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class MaintenanceControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                btnBackupPictures.Visible = !PictureManager.StoreInDB;
                BindGrid();
            }
        }

        protected void BindGrid()
        {
            BackupFileCollection collection = MaintenanceManager.GetAllBackupFiles();
            gvBackups.DataSource = collection;
            gvBackups.DataBind();
        }

        protected void gvBackups_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBackups.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void BackupButton_Click(object sender, EventArgs e)
        {
            try
            {
                MaintenanceManager.Backup();

                CustomerActivityManager.InsertActivity(
                    "CreateBackup",
                    GetLocaleResourceString("ActivityLog.CreateBackup"));

                ShowMessage(GetLocaleResourceString("Admin.Maintenance.BackupCreated"));

                BindGrid();
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected string GetFileSizeInfo(long byteCount)
        {
            return string.Format("{0:F2} Mb", byteCount / 1024f / 1024f);  
        }

        protected void RestoreButton_OnCommand(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "RestoreBackup")
            {
                try
                {
                    MaintenanceManager.RestoreBackup(e.CommandArgument.ToString());

                    CustomerActivityManager.InsertActivity(
                        "RestoreBackup",
                        GetLocaleResourceString("ActivityLog.RestoreBackup"));

                    BindGrid();
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }
        protected void DeleteButton_OnCommand(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "DeleteBackup")
            {
                try
                {
                    MaintenanceManager.DeleteBackup(e.CommandArgument.ToString());
                    BindGrid();
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void BtnBackupPictures_OnClick(object sender, EventArgs e)
        {
            try
            {
                MaintenanceManager.BackupPictures();
                BindGrid();
            }
            catch(Exception exc)
            {
                ProcessException(exc);
            }
        }
    }
}