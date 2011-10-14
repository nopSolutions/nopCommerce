using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using Nop.Core.ComponentModel;
using Nop.Core.Plugins;

//Contributor: Umbraco (http://www.umbraco.com). Thanks a lot!
//SEE THIS POST for full details of what this does
//http://shazwazza.com/post/Developing-a-plugin-framework-in-ASPNET-with-medium-trust.aspx

[assembly: PreApplicationStartMethod(typeof(PluginManager), "Initialize")]

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Sets the application up for the plugin referencing
    /// </summary>
    public class PluginManager
    {
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
        private static DirectoryInfo _shadowCopyFolder;
        private static readonly string _installedPluginsFilePath = "~/App_Data/InstalledPlugins.txt";
        private static readonly string _pluginsPath = "~/Plugins";
        private static readonly string _shadowCopyPath = "~/Plugins/bin";


        /// <summary>
        /// Returns a collection of all referenced plugin assemblies that have been shadow copied
        /// </summary>
        public static IEnumerable<PluginDescriptor> ReferencedPlugins { get; set; }

        /// <summary>
        /// Initialize
        /// </summary>
        public static void Initialize()
        {
            using (new WriteLockDisposable(Locker))
            {
                // TODO: Add verbose exception handling / raising here since this is happening on app startup and could
                // prevent app from starting altogether
                var pluginFolder = new DirectoryInfo(HostingEnvironment.MapPath(_pluginsPath));
                _shadowCopyFolder = new DirectoryInfo(HostingEnvironment.MapPath(_shadowCopyPath));

                var referencedPlugins = new List<PluginDescriptor>();

                try
                {
                    var installedPluginSystemNames = ParseInstalledPluginsFile();

                    Debug.WriteLine("Creating shadow copy folder and querying for dlls");
                    //ensure folders are created
                    Directory.CreateDirectory(pluginFolder.FullName);
                    Directory.CreateDirectory(_shadowCopyFolder.FullName);

                    //get list of all files in bin
                    var binFiles = _shadowCopyFolder.GetFiles("*", SearchOption.AllDirectories);
                    //clear out shadow copied plugins
                    foreach (var f in binFiles)
                    {
                        Debug.WriteLine("Deleting " + f.Name);
                        try
                        {
                            File.Delete(f.FullName);
                        }
                        catch (Exception exc)
                        {
                            Debug.WriteLine("Error deleting file " + f.Name + ". Exception: " + exc);
                        }
                    }

                    //load description files
                    foreach (var descriptionFile in pluginFolder.GetFiles("Description.txt", SearchOption.AllDirectories))
                    {
                        try
                        {
                            //parse file
                            var description = ParsePluginDescriptionFile(descriptionFile.FullName);

                            //ensure that version of plugin is valid
                            if (!description.SupportedVersions.Contains(NopVersion.CurrentVersion, StringComparer.InvariantCultureIgnoreCase))
                                continue;

                            //some validation
                            if (String.IsNullOrWhiteSpace(description.SystemName))
                                throw new Exception(string.Format("A plugin has no system name. Try assigning the plugin a unique name and recompiling.", description.SystemName));
                            if (referencedPlugins.Contains(description))
                                throw new Exception(string.Format("A plugin with '{0}' system name is already defined", description.SystemName));

                            //set 'Installed' property
                            description.Installed = installedPluginSystemNames
                                .ToList()
                                .Where(x => x.Equals(description.SystemName, StringComparison.InvariantCultureIgnoreCase))
                                .FirstOrDefault() != null;

                            //get list of all DLLs in plugins (not in bin!)
                            var pluginFiles = descriptionFile.Directory.GetFiles("*.dll", SearchOption.AllDirectories)
                                //just make sure we're not registering shadow copied plugins
                                .Where(x => !binFiles.Select(q => q.FullName).Contains(x.FullName))
                                .Where(x => IsPackagePluginFolder(x.Directory))
                                .ToList();

                            //other plugin description info
                            var mainPluginFile = pluginFiles.Where(x => x.Name.Equals(description.PluginFileName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            description.OriginalAssemblyFile = mainPluginFile;

                            //shadow copy main pugin file
                            description.ReferencedAssembly = PerformFileDeploy(mainPluginFile);

                            //load all other referenced assemblies now
                            foreach (var plugin in pluginFiles
                                .Where(x => !x.Name.Equals(mainPluginFile.Name, StringComparison.InvariantCultureIgnoreCase))
                                .Where(x => !IsAlreadyLoaded(x)))
                                    PerformFileDeploy(plugin);


                            //init plugin type (only one plugin per assembly is allowed)
                            foreach (var t in description.ReferencedAssembly.GetTypes())
                                if (typeof(IPlugin).IsAssignableFrom(t))
                                    if (!t.IsInterface)
                                        if (t.IsClass && !t.IsAbstract)
                                        {
                                            description.PluginType = t;
                                            break;
                                        }

                            referencedPlugins.Add(description);
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            var msg = string.Empty;
                            foreach (var e in ex.LoaderExceptions)
                                msg += e.Message + Environment.NewLine;

                            var fail = new Exception(msg, ex);
                            Debug.WriteLine(fail.Message, fail);

                            throw fail;
                        }
                        //catch (Exception ex)
                        //{
                        //    var fail = new Exception("Could not initialise plugin folder", ex);
                        //    Debug.WriteLine(fail.Message);
                        //    throw fail;
                        //}
                    }
                }
                catch (Exception ex)
                {
                    var msg = string.Empty;
                    for (var e = ex; e != null; e = e.InnerException)
                        msg += e.Message + Environment.NewLine;

                    var fail = new Exception(msg, ex);
                    Debug.WriteLine(fail.Message, fail);

                    throw fail;
                }


                ReferencedPlugins = referencedPlugins;

            }
        }

        protected static bool IsAlreadyLoaded(FileInfo fileInfo)
        {
            //compare full assembly name
            //var fileAssemblyName = AssemblyName.GetAssemblyName(fileInfo.FullName);
            //foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    if (a.FullName.Equals(fileAssemblyName.FullName, StringComparison.InvariantCultureIgnoreCase))
            //        return true;
            //}
            //return false;

            //do not compare the full assembly name, just filename
            try
            {
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    string assemblyName = a.FullName.Split(new[] {','}).FirstOrDefault();
                    if (fileNameWithoutExt.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Cannot validate whether an assembly is already loaded. " + exc);
            }
            return false;
        }

        /// <summary>
        /// Mark plugin as installed
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        public static void MarkPluginAsInstalled(string systemName)
        {
            if (String.IsNullOrEmpty(systemName))
                throw new ArgumentNullException("systemName");

            var filePath = HostingEnvironment.MapPath(_installedPluginsFilePath);
            if (!File.Exists(filePath))
                using (File.Create(filePath))
                {
                    //we use 'using' to close the file after it's created
                }


            var installedPluginSystemNames = ParseInstalledPluginsFile();
            bool alreadyMarkedAsInstalled = installedPluginSystemNames
                                .ToList()
                                .Where(x => x.Equals(systemName, StringComparison.InvariantCultureIgnoreCase))
                                .FirstOrDefault() != null;
            if (!alreadyMarkedAsInstalled)
                installedPluginSystemNames.Add(systemName);
            var text = ComposeInstalledPluginsFile(installedPluginSystemNames);
            File.WriteAllText(filePath, text);
        }

        /// <summary>
        /// Mark plugin as uninstalled
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        public static void MarkPluginAsUninstalled(string systemName)
        {
            if (String.IsNullOrEmpty(systemName))
                throw new ArgumentNullException("systemName");

            var filePath = HostingEnvironment.MapPath(_installedPluginsFilePath);
            if (!File.Exists(filePath))
                using (File.Create(filePath))
                {
                    //we use 'using' to close the file after it's created
                }


            var installedPluginSystemNames = ParseInstalledPluginsFile();
            bool alreadyMarkedAsInstalled = installedPluginSystemNames
                                .ToList()
                                .Where(x => x.Equals(systemName, StringComparison.InvariantCultureIgnoreCase))
                                .FirstOrDefault() != null;
            if (alreadyMarkedAsInstalled)
                installedPluginSystemNames.Remove(systemName);
            var text = ComposeInstalledPluginsFile(installedPluginSystemNames);
            File.WriteAllText(filePath, text);
        }

        /// <summary>
        /// Mark plugin as uninstalled
        /// </summary>
        public static void MarkAllPluginsAsUninstalled()
        {
            var filePath = HostingEnvironment.MapPath(_installedPluginsFilePath);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        /// <summary>
        /// Perform file deply
        /// </summary>
        /// <param name="plug">Plugin file info</param>
        /// <returns>Assembly</returns>
        private static Assembly PerformFileDeploy(FileInfo plug)
        {
            if (plug.Directory.Parent == null)
                throw new InvalidOperationException("The plugin directory for the " + plug.Name +
                                                    " file exists in a folder outside of the allowed Umbraco folder heirarchy");

            FileInfo shadowCopiedPlug;

            if (CommonHelper.GetTrustLevel() != AspNetHostingPermissionLevel.Unrestricted)
            {
                //all plugins will need to be copied to ~/Plugins/bin/
                //this is aboslutely required because all of this relies on probingPaths being set statically in the web.config
                
                //var shadowCopyPlugFolderName = "Packages";
                //Debug.WriteLine(plug.FullName + " to " + shadowCopyPlugFolderName);

                //were running in med trust, so copy to custom bin folder
                var shadowCopyPlugFolder = Directory.CreateDirectory(_shadowCopyFolder.FullName);
                shadowCopiedPlug = InitializeMediumTrust(plug, shadowCopyPlugFolder);
            }
            else
            {
                var directory = AppDomain.CurrentDomain.DynamicDirectory;
                Debug.WriteLine(plug.FullName + " to " + directory);
                //were running in full trust so copy to standard dynamic folder
                shadowCopiedPlug = InitializeFullTrust(plug, new DirectoryInfo(directory));
            }

            //we can now register the plugin definition
            var shadowCopiedAssembly = Assembly.Load(AssemblyName.GetAssemblyName(shadowCopiedPlug.FullName));

            //add the reference to the build manager
            Debug.WriteLine("Adding to BuildManager: '{0}'", shadowCopiedAssembly.FullName);
            BuildManager.AddReferencedAssembly(shadowCopiedAssembly);

            return shadowCopiedAssembly;
        }

        /// <summary>
        /// Used to initialize plugins when running in Full Trust
        /// </summary>
        /// <param name="plug"></param>
        /// <param name="shadowCopyPlugFolder"></param>
        /// <returns></returns>
        private static FileInfo InitializeFullTrust(FileInfo plug, DirectoryInfo shadowCopyPlugFolder)
        {
            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plug.Name));
            try
            {
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            catch (IOException)
            {
                Debug.WriteLine(shadowCopiedPlug.FullName + " is locked, attempting to rename");
                //this occurs when the files are locked,
                //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                try
                {
                    var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                    File.Move(shadowCopiedPlug.FullName, oldFile);
                }
                catch (IOException exc)
                {
                    throw new IOException(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin", exc);
                }
                //ok, we've made it this far, now retry the shadow copy
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            return shadowCopiedPlug;
        }

        /// <summary>
        /// Used to initialize plugins when running in Medium Trust
        /// </summary>
        /// <param name="plug"></param>
        /// <param name="shadowCopyPlugFolder"></param>
        /// <returns></returns>
        private static FileInfo InitializeMediumTrust(FileInfo plug, DirectoryInfo shadowCopyPlugFolder)
        {
            var shouldCopy = true;
            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plug.Name));

            //check if a shadow copied file already exists and if it does, check if its updated, if not don't copy
            if (shadowCopiedPlug.Exists)
            {
                if (shadowCopiedPlug.CreationTimeUtc.Ticks == plug.CreationTimeUtc.Ticks)
                {
                    Debug.WriteLine("Not copying; files appear identical: '{0}'", shadowCopiedPlug.Name);
                    shouldCopy = false;
                }
            }

            if (shouldCopy)
            {
                try
                {
                    File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
                }
                catch (IOException)
                {
                    Debug.WriteLine(shadowCopiedPlug.FullName + " is locked, attempting to rename");
                    //this occurs when the files are locked,
                    //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                    //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                    try
                    {
                        var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                        File.Move(shadowCopiedPlug.FullName, oldFile);
                    }
                    catch (IOException exc)
                    {
                        throw new IOException(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin", exc);
                    }
                    //ok, we've made it this far, now retry the shadow copy
                    File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
                }
            }

            return shadowCopiedPlug;
        }
        
        /// <summary>
        /// Determines if the folder is a bin plugin folder for a package
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private static bool IsPackagePluginFolder(DirectoryInfo folder)
        {
            if (folder == null) return false;
            if (folder.Parent == null) return false;
            if (!folder.Parent.Name.Equals("Plugins", StringComparison.InvariantCultureIgnoreCase)) return false;
            return true;
        }
        
        private static IList<string> ParseInstalledPluginsFile()
        {
            var filePath = HostingEnvironment.MapPath(_installedPluginsFilePath);
            var systemNames = new List<string>();
            if (File.Exists(filePath))
            {
                var text = File.ReadAllText(filePath);

                if (String.IsNullOrEmpty(text))
                    return new List<string>();

                systemNames = new List<string>();
                var lines = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                    systemNames.Add(line.Trim());
            }

            return systemNames;
        }

        private static string ComposeInstalledPluginsFile(IList<String> pluginSystemNames)
        {
            if (pluginSystemNames == null || pluginSystemNames.Count == 0)
                return "";

            string result = "";
            foreach (var sn in pluginSystemNames)
                result += string.Format("{0}\r\n", sn);
            return result;
        }

        private static PluginDescriptor ParsePluginDescriptionFile(string filePath)
        {
            var text = File.ReadAllText(filePath);

            var descriptor = new PluginDescriptor();
            if (String.IsNullOrEmpty(text))
                return descriptor;

            var settings = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var setting in settings)
            {
                var separatorIndex = setting.IndexOf(':');
                if (separatorIndex == -1)
                {
                    continue;
                }
                string key = setting.Substring(0, separatorIndex).Trim();
                string value = setting.Substring(separatorIndex + 1).Trim();

                switch (key)
                {
                    case "Group":
                        descriptor.Group = value;
                        break;
                    case "FriendlyName":
                        descriptor.FriendlyName = value;
                        break;
                    case "SystemName":
                        descriptor.SystemName = value;
                        break;
                    case "Version":
                        descriptor.Version = value;
                        break;
                    case "SupportedVersions":
                        {
                            //parse supported versions
                            descriptor.SupportedVersions = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim())
                                .ToList();
                        }
                        break;
                    case "Author":
                        descriptor.Author = value;
                        break;
                    case "DisplayOrder":
                        {
                            int displayOrder;
                            int.TryParse(value, out displayOrder);
                            descriptor.DisplayOrder = displayOrder;
                        }
                        break;
                    case "FileName":
                        descriptor.PluginFileName = value;
                        break;
                }
            }

            //nopCommerce 2.00 didn't have 'SupportedVersions' parameter
            //so let's set it to "2.00"
            if (descriptor.SupportedVersions.Count == 0)
                descriptor.SupportedVersions.Add("2.00");

            return descriptor;
        }

    }
}
