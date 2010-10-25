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
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Maintenance;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class MaintenanceControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                btnBackupPictures.Visible = !IoCFactory.Resolve<IPictureManager>().StoreInDB;
                BindGrid();
            }
        }

        protected void BindGrid()
        {
            var backups = IoCFactory.Resolve<IMaintenanceManager>().GetAllBackupFiles();
            gvBackups.DataSource = backups;
            gvBackups.DataBind();
        }

        protected void gvBackups_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBackups.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvBackups_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                BackupFile backupFile = (BackupFile)e.Row.DataItem;

                //download
                HyperLink hlDownload = e.Row.FindControl("hlDownload") as HyperLink;
                if (hlDownload != null)
                {
                    string url = string.Format("{0}backups/{1}", CommonHelper.GetStoreAdminLocation(), backupFile.FileName);
                    hlDownload.NavigateUrl = url;
                }
            }
        }

        protected string GetFileSizeInfo(long byteCount)
        {
            return string.Format("{0:F2} Mb", byteCount / 1024f / 1024f);
        }

        protected void btnBackupButton_Click(object sender, EventArgs e)
        {
            try
            {
                IoCFactory.Resolve<IMaintenanceManager>().Backup();

                IoCFactory.Resolve<ICustomerActivityManager>().InsertActivity(
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

        protected void btnBackupPictures_Click(object sender, EventArgs e)
        {
            try
            {
                IoCFactory.Resolve<IMaintenanceManager>().BackupPictures();
                BindGrid();
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected void btnDeleteOldExportedFiles_Click(object sender, EventArgs e)
        {
            try
            {
                //delete old files for the last 1 hour
                int hours = 1;
                int num = IoCFactory.Resolve<IMaintenanceManager>().DeleteOldExportImportFiles(hours);
                if (num > 0)
                {
                    ShowMessage(string.Format(GetLocaleResourceString("Admin.Maintenance.DeleteOldExportedFiles.Success"), num));
                }
                else
                {
                    ShowMessage(string.Format(GetLocaleResourceString("Admin.Maintenance.DeleteOldExportedFiles.NoFilesToDelete"), num));
                }
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        protected void RestoreButton_OnCommand(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "RestoreBackup")
            {
                try
                {
                    IoCFactory.Resolve<IMaintenanceManager>().RestoreBackup(e.CommandArgument.ToString());

                    IoCFactory.Resolve<ICustomerActivityManager>().InsertActivity(
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
                    IoCFactory.Resolve<IMaintenanceManager>().DeleteBackup(e.CommandArgument.ToString());
                    BindGrid();
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }
    }
}