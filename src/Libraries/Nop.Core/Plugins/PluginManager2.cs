using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading;
using System.Web;
using System.IO;
using System.Web.Hosting;
using System.Web.Compilation;
using System.Reflection;
using Nop.Core.ComponentModel;
using Nop.Core.Plugins;

//Contributor: Umbraco (http://www.umbraco.com)
// SEE THIS POST for full details of what this does
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

        /// <summary>
        /// Returns a collection of all referenced plugin assemblies that have been shadow copied
        /// </summary>
        public static IEnumerable<PluginDefinition> ReferencedPlugins { get; private set; }

        public static void Initialize()
        {
            using (new WriteLockDisposable(Locker))
            {
                var pluginsPath = "~/Plugins";
                var shadowCopyPath = "~/Plugins/bin";

                // TODO: Add verbose exception handling / raising here since this is happening on app startup and could
                // prevent app from starting altogether
                var pluginFolder = new DirectoryInfo(HostingEnvironment.MapPath(pluginsPath));
                _shadowCopyFolder = new DirectoryInfo(HostingEnvironment.MapPath(shadowCopyPath));

                var referencedPlugins = new List<PluginDefinition>();

                var pluginFiles = Enumerable.Empty<FileInfo>();

                try
                {
                    Debug.WriteLine("Creating shadow copy folder and querying for dlls");

                    //ensure folders are created
                    Directory.CreateDirectory(pluginFolder.FullName);
                    Directory.CreateDirectory(_shadowCopyFolder.FullName);

                    //get list of all DLLs in bin
                    var binFiles = _shadowCopyFolder.GetFiles("*.dll", SearchOption.AllDirectories);

                    //get list of all DLLs in plugins (not in bin!)
                    //this will match the plugin folder pattern, either 'Core' or 'PackageName'
                    pluginFiles = pluginFolder.GetFiles("*.dll", SearchOption.AllDirectories)
                        //just make sure we're not registering shadow copied plugins
                        .Where(x => !binFiles.Select(q => q.FullName).Contains(x.FullName))
                        .Where(x => x.Directory.Parent != null && (IsPackagePluginBinFolder(x.Directory) || x.Directory.Parent.Name == "Core"))
                        .ToList();

                    //clear out shadow copied plugins
                    foreach (var f in binFiles)
                    {
                        Debug.WriteLine("Deleting " + f.Name);

                        File.Delete(f.FullName);
                    }

                }
                catch (Exception ex)
                {
                    var fail = new Exception("Could not initialise plugin folder", ex);
                    Debug.WriteLine(fail.Message, fail);
                    throw fail;
                }

                try
                {
                    Debug.WriteLine("Shadow copying assemblies");

                    //shadow copy files
                    referencedPlugins
                        .AddRange(pluginFiles.Select(plug =>
                            new PluginDefinition(plug,
                                PerformFileDeploy(plug))));
                }
                catch (Exception ex)
                {
                    var fail = new Exception("Could not initialise plugin folder", ex);
                    Debug.WriteLine(fail.Message, fail);
                    throw fail;

                }

                ReferencedPlugins = referencedPlugins;

            }
        }

        private static Assembly PerformFileDeploy(FileInfo plug)
        {
            if (plug.Directory.Parent == null)
                throw new InvalidOperationException("The plugin directory for the " + plug.Name +
                                                    " file exists in a folder outside of the allowed Umbraco folder heirarchy");

            FileInfo shadowCopiedPlug;

            if (CommonHelper.GetTrustLevel() != AspNetHostingPermissionLevel.Unrestricted)
            {
                //all plugins will need to be copied to ~/Plugins/bin/Core OR ~/Plugins/bin/Packages
                //this is aboslutely required because all of this relies on probingPaths being set statically in the web.config
                var shadowCopyPlugFolderName = plug.Directory.Parent.Name == "Core"
                                                   ? "Core"
                                                   : "Packages";

                Debug.WriteLine(plug.FullName + " to " + shadowCopyPlugFolderName);

                //were running in med trust, so copy to custom bin folder
                var shadowCopyPlugFolder = Directory.CreateDirectory(Path.Combine(_shadowCopyFolder.FullName, shadowCopyPlugFolderName));
                shadowCopiedPlug = InitializeMediumTrust(plug, shadowCopyPlugFolder);
            }
            else
            {
                Debug.WriteLine(plug.FullName + " to " + AppDomain.CurrentDomain.DynamicDirectory);
                //were running in full trust so copy to standard dynamic folder
                shadowCopiedPlug = InitializeFullTrust(plug, new DirectoryInfo(AppDomain.CurrentDomain.DynamicDirectory));
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
                catch (IOException)
                {
                    Debug.WriteLine(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin");
                    throw;
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
                    catch (IOException)
                    {
                        Debug.WriteLine(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin");
                        throw;
                    }
                    //ok, we've made it this far, now retry the shadow copy
                    File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
                }
            }

            return shadowCopiedPlug;
        }

        /// <summary>
        /// Returns the package folder for the plugin DLL passed in
        /// </summary>
        /// <param name="pluginDll"></param>
        /// <returns></returns>
        internal static DirectoryInfo GetPackageFolderFromPluginDll(FileInfo pluginDll)
        {
            if (!IsPackagePluginBinFolder(pluginDll.Directory))
            {
                throw new DirectoryNotFoundException("The file specified does not exist in the bin folder for a package");
            }
            //we know this folder structure is correct now so return the directory. parent  \bin\..\{PackageName}
            return pluginDll.Directory.Parent;
        }

        /// <summary>
        /// Determines if the folder is a bin plugin folder for a package
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        internal static bool IsPackagePluginFolder(DirectoryInfo folder)
        {
            if (folder.Parent == null) return false;
            if (folder.Parent.Name != "Packages") return false;
            return true;
        }

        /// <summary>
        /// Determines if the folder is a bin plugin folder for a package
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        internal static bool IsPackagePluginBinFolder(DirectoryInfo folder)
        {
            if (folder.Name != "lib") return false;
            if (folder.Parent == null) return false;
            return IsPackagePluginFolder(folder.Parent);
        }
    }

    /// <summary>
    /// Defines a plugin
    /// </summary>
    public class PluginDefinition
    {
        //TODO move this file to a new file
        public PluginDefinition(FileInfo originalAssembly, Assembly shadowCopied)
        {
            ReferencedAssembly = shadowCopied;
            OriginalAssemblyFile = originalAssembly;
        }

        /// <summary>
        /// The assembly that has been shadow copied that is active in the application
        /// </summary>
        public Assembly ReferencedAssembly { get; internal set; }

        /// <summary>
        /// The original assembly file that a shadow copy was made from it
        /// </summary>
        public FileInfo OriginalAssemblyFile { get; private set; }
    }
}
