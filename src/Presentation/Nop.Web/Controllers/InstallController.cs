using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web.Hosting;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Installation;
using Nop.Web.Models.Install;

namespace Nop.Web.Controllers
{
    public class InstallController : BaseNopController
    {
        #region Utilities
        /// <summary>
        /// Checks if the specified database exists, returns true if database exists
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Returns true if the database exists.</returns>
        private bool SqlServerDatabaseExists(string connectionString)
        {
            try
            {
                //just try to connect
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }
        private bool CheckPermissions(string path, bool checkRead, bool checkWrite, bool checkModify, bool checkDelete)
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

        public ActionResult Index()
        {
            if (DataProviderHelper.DatabaseIsInstalled())
                return RedirectToAction("Index", "Home");

            var model = new InstallModel()
            {
                AdminEmail = "admin@yourStore.com",
                AdminPassword = "admin",
                ConfirmPassword = "admin",
                DatabaseConnectionString = "",
                DataProvider = "sqlce",
                InstallSampleData= true,
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(InstallModel model)
        {
            if (DataProviderHelper.DatabaseIsInstalled())
                return RedirectToAction("Index", "Home");

            if (model.DatabaseConnectionString != null)
                model.DatabaseConnectionString = model.DatabaseConnectionString.Trim();

            //validate conncetion string
            if (model.DataProvider.Equals("sqlserver", StringComparison.InvariantCultureIgnoreCase))
            {
                //SQL Server
                if (string.IsNullOrEmpty(model.DatabaseConnectionString))
                {
                    ModelState.AddModelError("", "A SQL connection string is required");
                }
                else
                {
                    try
                    {
                        //try to create connection string
                        new SqlConnectionStringBuilder(model.DatabaseConnectionString);


                        //check whether database exists
                        if (!SqlServerDatabaseExists(model.DatabaseConnectionString))
                        {
                            ModelState.AddModelError("", "Database does not exist or you don't have permissions to connect to it");
                        }
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError("", "Wrong SQL connection string format");
                    }
                }
            }

            //validate permissions
            string rootDir = Server.MapPath("~/");
            var dirsToCheck = new List<string>();
            dirsToCheck.Add(rootDir);
            dirsToCheck.Add(rootDir + "bin");
            dirsToCheck.Add(rootDir + "content");
            dirsToCheck.Add(rootDir + "content\\images");
            dirsToCheck.Add(rootDir + "content\\images\\thumbs");
            dirsToCheck.Add(rootDir + "content\files\\exportimport");
            //TODO Add Google directory after 'Google payment module' is added
            foreach (string dir in dirsToCheck)
                if (!CheckPermissions(dir, false, true, true, true))
                    ModelState.AddModelError("", string.Format("The '{0}' account is not granted with Modify permission on folder '{1}'. Please configure these permissions.", WindowsIdentity.GetCurrent().Name, dir));
                

            var filesToCheck = new List<string>();
            filesToCheck.Add(rootDir + "web.config");
            filesToCheck.Add(rootDir + "bin\\" + "Settings.txt");
            foreach (string file in filesToCheck)
                if (!CheckPermissions(file, false, true, true, true))
                    ModelState.AddModelError("", string.Format("The '{0}' account is not granted with Modify permission on file '{1}'. Please configure these permissions.", WindowsIdentity.GetCurrent().Name, file));
            
            if (ModelState.IsValid)
            {
                var dataProviderManager = new DataProviderManager();
                try
                {
                    string connectionString = null;
                    if (model.DataProvider.Equals("sqlserver", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //SQL Server
                        connectionString = model.DatabaseConnectionString;
                    }
                    else
                    {
                        //SQL CE
                        //little hack here (SQL CE 4 bug - http://www.hanselman.com/blog/PDC10BuildingABlogWithMicrosoftUnnamedPackageOfWebLove.aspx)
                        string databasePath = HostingEnvironment.MapPath("~/App_Data/") + @"Nop.Db.sdf";
                        connectionString = "Data Source=" + databasePath + ";Persist Security Info=False";
                        
                        //drop database if exists
                        if (System.IO.File.Exists(databasePath))
                        {
                            System.IO.File.Delete(databasePath);
                        }
                    }

                    //save settings
                    var dataProvider = model.DataProvider;
                    dataProviderManager.SaveSettings(new DataProviderSettings()
                        {
                            DataProvider = dataProvider,
                            DataConnectionString = connectionString
                        });

                    //init data provider
                    var dataProviderInstance = dataProviderManager.LoadDataProvider(dataProvider);
                    dataProviderInstance.InitConnectionFactory();
                    dataProviderInstance.SetDatabaseInitializer();
                    


                    //set page timeout to 5 minutes
                    this.Server.ScriptTimeout = 300;

                    //only now resolve installation service
                    var installationService = EngineContext.Current.Resolve<IInstallationService>();
                    installationService.InstallData(model.AdminEmail, model.AdminPassword, model.InstallSampleData);

                    //reset cache
                    DataProviderHelper.ResetCache();

                    //TODO install plugins

                    //restart application
                    var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                    webHelper.RestartAppDomain();

                    //Redirect to home page
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception exception)
                {
                    //clear provider settings if something got wrong
                    dataProviderManager.SaveSettings(new DataProviderSettings()
                    {
                        DataProvider = null,
                        DataConnectionString = null
                    });
                    
                    ModelState.AddModelError("", "Setup failed: " + exception);
                }
            }
            return View(model);
        }

        #endregion
    }
}
