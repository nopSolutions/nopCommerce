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
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Configuration;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Installation;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Install
{
    public partial class InstallPage : Page
    {
        #region Fields
        private List<string> upgradeableVersions = new List<string>() { "1.40", "1.50", "1.60", "1.70", "1.80" };
        private string newVersion = "1.80";
        #endregion

        #region Utilities
        protected bool createDatabase()
        {
            addResult(string.Format("Creating a new database {0}", this.txtNewDatabaseName.Text));
            string connectionString = InstallerHelper.CreateConnectionString(this.TrustedConnection, this.txtServerName.Text.Trim(), "master", this.txtUsername.Text.Trim(), Convert.ToString(ViewState["install.password"]), 120);
            string error = InstallerHelper.CreateDatabase(this.txtNewDatabaseName.Text.Trim(), connectionString);
            if (!String.IsNullOrEmpty(error))
            {
                this.pnlLog.Visible = true;
                addResult(string.Format("An error occured when creating database: {0}", error));
                return false;
            }
            return true;
        }

        protected bool installDatabase(string connectionString, bool createSampleData)
        {
            //uncomment this line to support transactions
            //using (var scope = new System.Transactions.TransactionScope())
            {
                string scriptsFolder = Server.MapPath("~/install/Scripts");

                string createDatabaseFile = string.Format(@"{0}\{1}", scriptsFolder, "nopCommerce_createDatabase.sql");
                string error = proceedSQLScripts(createDatabaseFile, connectionString);
                if (!String.IsNullOrEmpty(error))
                {
                    this.pnlLog.Visible = true;
                    addResult(string.Format("An error occured: {0}", error));
                    return false;
                }

                string createDataFile = string.Format(@"{0}\{1}", scriptsFolder, "nopCommerce_createData.sql");
                error = proceedSQLScripts(createDataFile, connectionString);
                if (!String.IsNullOrEmpty(error))
                {
                    this.pnlLog.Visible = true;
                    addResult(string.Format("An error occured: {0}", error));
                    return false;
                }
                if (createSampleData)
                {
                    string createSampleDataFile = string.Format(@"{0}\{1}", scriptsFolder, "nopCommerce_createSampleData.sql");
                    error = proceedSQLScripts(createSampleDataFile, connectionString);
                    if (!String.IsNullOrEmpty(error))
                    {
                        this.pnlLog.Visible = true;
                        addResult(string.Format("An error occured: {0}", error));
                        return false;
                    }
                }

                //uncomment this line to support transactions
                //scope.Complete();
            }
            return true;
        }

        protected bool upgradeDatabase(string newVersion, string connectionString)
        {
            //uncomment this line to support transactions
            //using (var scope = new System.Transactions.TransactionScope())
            {
                string scriptsFolder = Server.MapPath("~/install/Scripts");

                string upgradeFile = string.Format(@"{0}\{1}\{2}", scriptsFolder, newVersion, "nopCommerce_upgrade.sql");
                string error = proceedSQLScripts(upgradeFile, connectionString);
                if (!String.IsNullOrEmpty(error))
                {
                    this.pnlLog.Visible = true;
                    addResult(string.Format("An error occured: {0}", error));
                    return false;
                }

                //uncomment this line to support transactions
                //scope.Complete();
            }
            return true;
        }

        protected string proceedSQLScripts(string pathToScriptFile, string connectionString)
        {
            addResult(string.Format("Running scripts from file: {0}", pathToScriptFile));
            List<string> statements = new List<string>();

            using (Stream stream = File.OpenRead(pathToScriptFile))
            using (StreamReader reader = new StreamReader(stream))
            {
                string statement = string.Empty;
                while ((statement = readNextStatementFromStream(reader)) != null)
                {
                    statements.Add(statement);
                }
            }
            try
            {
                foreach (string stmt in statements)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand(stmt, conn);
                        command.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return string.Empty;
        }
     
        protected string readNextStatementFromStream(StreamReader reader)
        {
            StringBuilder sb = new StringBuilder();

            string lineOfText;

            while (true)
            {
                lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();
                    else
                        return null;
                }

                if (lineOfText.TrimEnd().ToUpper() == "GO")
                    break;

                sb.Append(lineOfText + Environment.NewLine);
            }

            return sb.ToString();
        }

        protected void addResult(string result)
        {
            mResult = string.Format("{0}\n{1}", result, mResult);
        }

        protected void handleError(string message)
        {
            this.lblError.Text = message;
        }

        private bool checkPermissions(string path, bool checkRead, bool checkWrite, bool checkModify, bool checkDelete)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            bool flag7 = false;
            bool flag8 = false;
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            System.Security.AccessControl.AuthorizationRuleCollection rules = null;
            try
            {
                rules = Directory.GetAccessControl(path).GetAccessRules(true, true, typeof(SecurityIdentifier));
            }
            catch
            {
                return true;
            }
            try
            {
                foreach (FileSystemAccessRule rule in rules)
                {
                    if (!current.User.Equals(rule.IdentityReference))
                    {
                        continue;
                    }
                    if (AccessControlType.Deny.Equals(rule.AccessControlType))
                    {
                        if ((FileSystemRights.Delete & rule.FileSystemRights) == FileSystemRights.Delete)
                            flag4 = true;
                        if ((FileSystemRights.Modify & rule.FileSystemRights) == FileSystemRights.Modify)
                            flag3 = true;

                        if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
                            flag = true;

                        if ((FileSystemRights.Write & rule.FileSystemRights) == FileSystemRights.Write)
                            flag2 = true;

                        continue;
                    }
                    if (AccessControlType.Allow.Equals(rule.AccessControlType))
                    {
                        if ((FileSystemRights.Delete & rule.FileSystemRights) == FileSystemRights.Delete)
                        {
                            flag8 = true;
                        }
                        if ((FileSystemRights.Modify & rule.FileSystemRights) == FileSystemRights.Modify)
                        {
                            flag7 = true;
                        }
                        if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
                        {
                            flag5 = true;
                        }
                        if ((FileSystemRights.Write & rule.FileSystemRights) == FileSystemRights.Write)
                        {
                            flag6 = true;
                        }
                    }
                }
                foreach (IdentityReference reference in current.Groups)
                {
                    foreach (FileSystemAccessRule rule2 in rules)
                    {
                        if (!reference.Equals(rule2.IdentityReference))
                        {
                            continue;
                        }
                        if (AccessControlType.Deny.Equals(rule2.AccessControlType))
                        {
                            if ((FileSystemRights.Delete & rule2.FileSystemRights) == FileSystemRights.Delete)
                                flag4 = true;
                            if ((FileSystemRights.Modify & rule2.FileSystemRights) == FileSystemRights.Modify)
                                flag3 = true;
                            if ((FileSystemRights.Read & rule2.FileSystemRights) == FileSystemRights.Read)
                                flag = true;
                            if ((FileSystemRights.Write & rule2.FileSystemRights) == FileSystemRights.Write)
                                flag2 = true;
                            continue;
                        }
                        if (AccessControlType.Allow.Equals(rule2.AccessControlType))
                        {
                            if ((FileSystemRights.Delete & rule2.FileSystemRights) == FileSystemRights.Delete)
                                flag8 = true;
                            if ((FileSystemRights.Modify & rule2.FileSystemRights) == FileSystemRights.Modify)
                                flag7 = true;
                            if ((FileSystemRights.Read & rule2.FileSystemRights) == FileSystemRights.Read)
                                flag5 = true;
                            if ((FileSystemRights.Write & rule2.FileSystemRights) == FileSystemRights.Write)
                                flag6 = true;
                        }
                    }
                }
                bool flag9 = !flag4 && flag8;
                bool flag10 = !flag3 && flag7;
                bool flag11 = !flag && flag5;
                bool flag12 = !flag2 && flag6;
                bool flag13 = true;
                if (checkRead)
                {
                    flag13 = flag13 && flag11;
                }
                if (checkWrite)
                {
                    flag13 = flag13 && flag12;
                }
                if (checkModify)
                {
                    flag13 = flag13 && flag10;
                }
                if (checkDelete)
                {
                    flag13 = flag13 && flag9;
                }
                return flag13;
            }
            catch (IOException)
            {
            }
            return false;
        }

        #endregion

        #region Methods
        public string GetNewVersion()
        {
            return newVersion;
        }
        #endregion

        #region Handlers
        protected void Page_Load(Object sender, EventArgs e)
        {
            CommonHelper.SetResponseNoCache(Response);

            lblVersion.Text = string.Format("nopCommerce {0}", GetNewVersion());

            if (!this.IsPostBack)
            {
                if (InstallerHelper.ConnectionStringIsSet())
                    Response.Redirect(CommonHelper.GetStoreLocation());

                bool checkPermission = Convert.ToBoolean(CommonHelper.QueryStringInt("checkpermission", 1));
                bool testAgain = Convert.ToBoolean(CommonHelper.QueryStringInt("testagain", 0));

                string rootDir = HttpContext.Current.Server.MapPath("~/");

                List<string> dirsToCheck = new List<string>();
                dirsToCheck.Add(rootDir);
                dirsToCheck.Add(rootDir + "Administration\\backups");
                dirsToCheck.Add(rootDir + "files");
                dirsToCheck.Add(rootDir + "files\\become");
                dirsToCheck.Add(rootDir + "files\\ExportImport");
                dirsToCheck.Add(rootDir + "files\\froogle");
                dirsToCheck.Add(rootDir + "files\\pricegrabber");
                dirsToCheck.Add(rootDir + "Google");
                dirsToCheck.Add(rootDir + "images");
                dirsToCheck.Add(rootDir + "images\\thumbs");
                foreach (string dir in dirsToCheck)
                {
                    if (!checkPermissions(dir, false, true, true, true) && checkPermission)
                    {
                        pnlWizard.Visible = false;
                        imgHeader.Visible = false;
                        pnlPermission.Visible = true;
                        pnlButtons.Visible = true;
                        lblPermission.Text = string.Format("The <b>{0}</b> account is not granted with Modify permission on folder <b>{1}</b>. Although this is not an error, it's highly recommended that you configure these permissions.", System.Security.Principal.WindowsIdentity.GetCurrent().Name, dir);
                        return;
                    }
                }

                List<string> filesToCheck = new List<string>();
                filesToCheck.Add(rootDir + "ConnectionStrings.config");
                filesToCheck.Add(rootDir + "web.config");
                foreach (string file in filesToCheck)
                {
                    if (!checkPermissions(file, false, true, true, true) && checkPermission)
                    {
                        pnlWizard.Visible = false;
                        imgHeader.Visible = false;
                        pnlPermission.Visible = true;
                        pnlButtons.Visible = true;
                        lblPermission.Text = string.Format("The <b>{0}</b> account is not granted with Modify permission on file <b>{1}</b>. Although this is not an error, it's highly recommended that you configure these permissions.", System.Security.Principal.WindowsIdentity.GetCurrent().Name, file);
                        return;
                    }
                }

                if (testAgain)
                {
                    pnlWizard.Visible = false;
                    pnlPermission.Visible = false;
                    pnlButtons.Visible = false;
                    pnlPermissionSuccess.Visible = true;
                    lblPermissionSuccess.Text = "The permissions are configured correctly now.";
                    return;
                }
            }

            pnlWizard.Visible = true;
            pnlPermission.Visible = false;
            pnlButtons.Visible = false;

            if (!this.IsPostBack)
            {
                if (HttpContext.Current != null)
                    txtServerName.Text = HttpContext.Current.Server.MachineName;
                this.TrustedConnection = false;
                wzdInstaller.ActiveStepIndex = 0;
            }
            else
            {
                if (ViewState["install.password"] == null)
                    ViewState["install.password"] = this.txtPassword.Text;
            }

            this.pnlLog.Visible = false;
            this.lblError.Text = string.Empty;
            this.rbWindowsAuthentication.Text = string.Format("Use integrated Windows authentication (ASP.NET account: {0})", System.Security.Principal.WindowsIdentity.GetCurrent().Name);

            mResult = string.Empty;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (this.Install)
            {
                this.rbCreateNew.Enabled = true;
                this.rbUseExisting.Enabled = true;
                this.chkCreateSampleData.Enabled = true;
                this.pnlChangeAdminCredentials.Visible = true;
            }
            else
            {
                this.rbCreateNew.Enabled = false;
                this.rbCreateNew.Checked = false;
                this.rbUseExisting.Enabled = true;
                this.rbUseExisting.Checked = true;
                this.chkCreateSampleData.Enabled = false;
                this.chkCreateSampleData.Checked = false;
                this.pnlChangeAdminCredentials.Visible = false;
            }
            this.txtPassword.Enabled = this.rbSQLAuthentication.Checked;
            this.txtUsername.Enabled = this.rbSQLAuthentication.Checked;
            this.txtExistingDatabaseName.Enabled = this.rbUseExisting.Checked;
            this.txtNewDatabaseName.Enabled = this.rbCreateNew.Checked;
            this.chkDontCheckDatabase.Enabled = this.rbUseExisting.Checked;

            if (this.pnlLog.Visible)
                this.txtLog.Text = mResult;

            string imgName = null;

           if (this.wzdInstaller.ActiveStepIndex == this.wzdInstaller.WizardSteps.IndexOf(this.stpWelcome))
                imgName = "header_1.gif";
            else if (this.wzdInstaller.ActiveStepIndex == this.wzdInstaller.WizardSteps.IndexOf(this.stpUserServer))
                imgName = "header_2.gif";
            else if (this.wzdInstaller.ActiveStepIndex == this.wzdInstaller.WizardSteps.IndexOf(this.stpDatabase))
                imgName = "header_3.gif";
            else if (this.wzdInstaller.ActiveStepIndex == this.wzdInstaller.WizardSteps.IndexOf(this.stpConnectionString))
                imgName = "header_3.gif";
            else if (this.wzdInstaller.ActiveStepIndex == this.wzdInstaller.WizardSteps.IndexOf(this.stpFinish))
                imgName = "header_4.gif";

            imgHeader.ImageUrl = string.Format("~/App_Themes/{0}/Images/{1}", Page.Theme, imgName);
        }

        protected void btnPermissionContinue_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.GetLeftPart(UriPartial.Path));
        }

        protected void btnPermissionSkip_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.GetLeftPart(UriPartial.Path) + "?checkpermission=0");
        }

        protected void btnPermissionTest_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.GetLeftPart(UriPartial.Path) + "?testagain=1");
        }

        protected void btnGoToSite_Click(object sender, EventArgs e)
        {
            Response.Redirect(CommonHelper.GetStoreLocation());
        }

        protected void wzdInstaller_OnActiveStepChanged(object sender, EventArgs e)
        {
        }

        protected void btnSaveAdmin_OnClick(object sender, EventArgs e)
        {
            try
            {
                lblSaveAdminResult.Text = string.Empty;

                string email = txtAdminEmail.Text.Trim();
                if (!CommonHelper.IsValidEmail(email))
                {
                    handleError("Email is not valid.");
                    return;
                }
                if (!SettingManager.GetSettingValueBoolean("InstallationWizard.AdminAccountChanged", false))
                {
                    Customer admin = CustomerManager.GetCustomerByEmail(this.AdminUserEmail);

                    if (admin == null || !admin.IsAdmin)
                    {
                        throw new NopException("Admin record does not exist.");
                    }

                    admin = CustomerManager.UpdateCustomer(admin.CustomerId,
                        admin.CustomerGuid, email, email,
                        admin.PasswordHash, admin.SaltKey, admin.AffiliateId,
                        admin.BillingAddressId, admin.ShippingAddressId, admin.LastPaymentMethodId,
                        admin.LastAppliedCouponCode, admin.GiftCardCouponCodes,
                        admin.CheckoutAttributes, admin.LanguageId,
                        admin.CurrencyId, admin.TaxDisplayType, admin.IsTaxExempt,
                        admin.IsAdmin, admin.IsGuest, admin.IsForumModerator,
                        admin.TotalForumPosts, admin.Signature, admin.AdminComment,
                        admin.Active, admin.Deleted, admin.RegistrationDate,
                        admin.TimeZoneId, admin.AvatarId, admin.DateOfBirth);

                    this.AdminUserEmail = admin.Email;

                    CustomerManager.ModifyPassword(admin.CustomerId, txtAdminPassword.Text);

                    SettingManager.SetParam("InstallationWizard.AdminAccountChanged", "true");

                    lblSaveAdminResult.Visible = true;
                    lblSaveAdminResult.Text = "Admin account has been changed";
                    txtAdminEmail.Enabled = false;
                    txtAdminPassword.Enabled = false;
                    btnSaveAdmin.Visible = false;
                }
                else
                {
                    throw new Exception("Admin account has already been changed");
                }
            }
            catch (Exception ex)
            {
                handleError(ex.Message);
            }
        }

        protected void wzdInstaller_OnNextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (this.wzdInstaller.ActiveStepIndex == this.wzdInstaller.WizardSteps.IndexOf(this.stpWelcome))
            {
                #region Welcome screen
                this.Install = this.rbInstall.Checked;
                #endregion
            }
            else if (this.wzdInstaller.ActiveStepIndex == this.wzdInstaller.WizardSteps.IndexOf(this.stpUserServer))
            {
                #region Server
                ViewState["install.password"] = this.txtPassword.Text;

                this.TrustedConnection = this.rbWindowsAuthentication.Checked;

                if (String.IsNullOrEmpty(this.txtServerName.Text.Trim()))
                {
                    handleError("Please enter the SQL Server name or IP address");
                    e.Cancel = true;
                    return;
                }
                string error = InstallerHelper.TestConnection(this.TrustedConnection, this.txtServerName.Text.Trim(), string.Empty, this.txtUsername.Text.Trim(), Convert.ToString(ViewState["install.password"]));
                if (!String.IsNullOrEmpty(error))
                {
                    handleError(error);
                    e.Cancel = true;
                    return;
                }
                #endregion
            }
            else if (this.wzdInstaller.ActiveStepIndex == this.wzdInstaller.WizardSteps.IndexOf(this.stpDatabase))
            {
                #region Database
                string database = string.Empty;
                if (rbCreateNew.Checked)
                    database = txtNewDatabaseName.Text.Trim();
                else
                    database = txtExistingDatabaseName.Text.Trim();
                this.ConnectionString = InstallerHelper.CreateConnectionString(this.TrustedConnection, this.txtServerName.Text.Trim(), database, this.txtUsername.Text.Trim(), Convert.ToString(ViewState["install.password"]), 120);

                if (this.rbUseExisting.Checked)
                {
                    // Use existing database
                    if (String.IsNullOrEmpty(database))
                    {
                        handleError("Please enter the database name");
                        e.Cancel = true;
                        return;
                    }
                    else
                    {
                        if (!chkDontCheckDatabase.Checked)
                        {
                            if (!InstallerHelper.DatabaseExists(this.TrustedConnection, this.txtServerName.Text.Trim(), database, this.txtUsername.Text.Trim(), Convert.ToString(ViewState["install.password"])))
                            {
                                handleError(string.Format("The database '{0}' doesn't exist!", database));
                                e.Cancel = true;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    // Create a new database
                    if (!createDatabase())
                    {
                        handleError(string.Format("Error creating the Database {0}", this.txtNewDatabaseName.Text));
                        e.Cancel = true;
                        return;
                    }
                    else
                    {
                        this.txtExistingDatabaseName.Text = this.txtNewDatabaseName.Text;
                        this.rbCreateNew.Checked = false;
                        this.rbUseExisting.Checked = true;
                    }
                }

                if (this.Install)
                {
                    bool createSampleData = chkCreateSampleData.Checked;
                    if (!installDatabase(ConnectionString, createSampleData))
                    {
                        handleError("An error occured during the database setup");
                        e.Cancel = true;
                        return;
                    }
                }
                else
                {
                    string currentVersion = InstallerHelper.GetCurrentVersion(this.ConnectionString);


                    if (String.IsNullOrEmpty(currentVersion))
                    {
                        handleError("Upgrade script from your version is not available");
                        e.Cancel = true;
                        return;
                    }

                    if (currentVersion == newVersion)
                    {
                        handleError(string.Format("You already have version '{0}'", currentVersion));
                        e.Cancel = true;
                        return;
                    }

                    if (!upgradeableVersions.Contains(currentVersion))
                    {
                        handleError(string.Format("Upgrade script from your version '{0}' to '{1}' is not available", currentVersion, newVersion));
                        e.Cancel = true;
                        return;
                    }

                    bool flag1 = false;
                    foreach (string version in upgradeableVersions)
                    {
                        if (currentVersion == version)
                        {
                            flag1 = true;
                            continue;
                        }

                        if (flag1)
                        {
                            if (!upgradeDatabase(version, ConnectionString))
                            {
                                handleError("An error occured during the database upgrade");
                                e.Cancel = true;
                                return;
                            }
                        }
                    }

                }

                string setCurrentVersionError = InstallerHelper.SetCurrentVersion(this.ConnectionString, newVersion);
                if (!String.IsNullOrEmpty(setCurrentVersionError))
                {
                    this.pnlLog.Visible = true;
                    addResult(string.Format("An error occured during setting new version: {0}", setCurrentVersionError));
                    return;
                }

                this.pnlLog.Visible = false;

                if (InstallerHelper.SaveConnectionString("NopSqlConnection", ConnectionString))
                    wzdInstaller.ActiveStepIndex = this.wzdInstaller.WizardSteps.IndexOf(this.stpFinish);
                else
                {
                    string connStringDisplay = InstallerHelper.CreateConnectionString(this.TrustedConnection, this.txtServerName.Text.Trim(), database, this.txtUsername.Text.Trim(), Convert.ToString(ViewState["install.password"]), 120);
                    wzdInstaller.ActiveStepIndex = this.wzdInstaller.WizardSteps.IndexOf(this.stpConnectionString);
                    string message = "The installer couldn't update the ConnectionStrings.config file on your server. This may be caused by limited file system permissions. Please open your ConnectionStrings.config file manually in Notepad and add the following line inside the &lt;connectionStrings&gt;&lt;/connectionStrings&gt;: <br/><br/><b>&lt;add name=\"NopSqlConnection\" connectionString=\"" + connStringDisplay + "\"/&gt;</b><br/><br/>";
                    lblErrorConnMessage.Text = message;

                }
                #endregion
            }
            else if (this.wzdInstaller.ActiveStepIndex == this.wzdInstaller.WizardSteps.IndexOf(this.stpConnectionString))
            {
                #region Connection String
                if (NopConfig.ConnectionString != ConnectionString)
                {
                    handleError("The connection string you added doesn't match. Make sure, the connection string is correct.");
                    e.Cancel = true;
                    return;
                }
                else
                {
                    wzdInstaller.ActiveStepIndex = this.wzdInstaller.WizardSteps.IndexOf(this.stpFinish);
                }
                #endregion
            }
        }
        #endregion

        #region Properties
        private bool Install
        {
            get
            {
                if (ViewState["Install"] != null)
                    return (bool)ViewState["Install"];
                return true;
            }
            set
            {
                ViewState["Install"] = value;
            }
        }
        
        private string AdminUserEmail
        {
            get
            {
                if (ViewState["AdminUserEmail"] == null)
                    ViewState["AdminUserEmail"] = "admin@yourStore.com";
                return (string)ViewState["AdminUserEmail"];
            }
            set
            {
                ViewState["AdminUserEmail"] = value;
            }
        }

        private bool TrustedConnection
        {
            get
            {
                if (ViewState["TrustedConnection"] != null)
                    return (bool)ViewState["TrustedConnection"];
                return false;
            }
            set
            {
                ViewState["TrustedConnection"] = value;
            }
        }

        private string ConnectionString
        {
            get
            {
                if (ViewState["connString"] == null)
                    ViewState["connString"] = string.Empty;
                return (string)ViewState["connString"];
            }
            set
            {
                ViewState["connString"] = value;
            }
        }

        private string mResult
        {
            get
            {
                return (string)ViewState["result"];
            }
            set
            {
                ViewState["result"] = value;
            }
        }
        #endregion
    }
}
