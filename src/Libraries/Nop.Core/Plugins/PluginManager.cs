using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Newtonsoft.Json;
using Nop.Core.ComponentModel;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

//Contributor: Umbraco (http://www.umbraco.com). Thanks a lot! 
//SEE THIS POST for full details of what this does - http://shazwazza.com/post/Developing-a-plugin-framework-in-ASPNET-with-medium-trust.aspx

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Sets the application up for the plugin referencing
    /// </summary>
    public class PluginManager
    {
        #region Constants

        private const string RESERVE_SHADOW_COPY_FOLDER_NAME = "reserve_bin_";
        private const string RESERVE_SHADOW_COPY_FOLDER_NAME_PATTERN = "reserve_bin_*";

        #endregion

        #region Fields

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
        private static string _shadowCopyFolder;
        private static readonly List<string> _baseAppLibraries;
        private static string _reserveShadowCopyFolder;
        private static readonly INopFileProvider _fileProvider;

        #endregion

        #region Ctor

        static PluginManager()
        {
            //we use the default file provider, since the DI isn't initialized yet
            _fileProvider = CommonHelper.DefaultFileProvider;

            //get all libraries from /bin/{version}/ directory
            _baseAppLibraries =_fileProvider.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll").Select(fi => _fileProvider.GetFileName(fi)).ToList();

            //get all libraries from base site directory
            if(!AppDomain.CurrentDomain.BaseDirectory.Equals(Environment.CurrentDirectory, StringComparison.InvariantCultureIgnoreCase))
                _baseAppLibraries.AddRange(_fileProvider.GetFiles(Environment.CurrentDirectory, "*.dll").Select(fi => _fileProvider.GetFileName(fi)));

            //get all libraries from refs directory
            var refsPathName = _fileProvider.Combine(Environment.CurrentDirectory, RefsPathName);
            if(_fileProvider.DirectoryExists(refsPathName))
                _baseAppLibraries.AddRange(_fileProvider.GetFiles(refsPathName, "*.dll").Select(fi => _fileProvider.GetFileName(fi)));
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get description files
        /// </summary>
        /// <param name="pluginFolder">Plugin directory info</param>
        /// <returns>Original and parsed description files</returns>
        private static IEnumerable<KeyValuePair<string, PluginDescriptor>> GetDescriptionFilesAndDescriptors(string pluginFolder)
        {
            if (pluginFolder == null)
                throw new ArgumentNullException(nameof(pluginFolder));

            //create list (<file info, parsed plugin descritor>)
            var result = new List<KeyValuePair<string, PluginDescriptor>>();

            //add display order and path to list
            foreach (var descriptionFile in _fileProvider.GetFiles(pluginFolder, PluginDescriptionFileName, false))
            {
                if (!IsPackagePluginFolder(_fileProvider.GetDirectoryName(descriptionFile)))
                    continue;

                //parse file
                var pluginDescriptor = GetPluginDescriptorFromFile(descriptionFile);

                //populate list
                result.Add(new KeyValuePair<string, PluginDescriptor>(descriptionFile, pluginDescriptor));
            }

            //sort list by display order. NOTE: Lowest DisplayOrder will be first i.e 0 , 1, 1, 1, 5, 10
            //it's required: https://www.nopcommerce.com/boards/t/17455/load-plugins-based-on-their-displayorder-on-startup.aspx
            result.Sort((firstPair, nextPair) => firstPair.Value.DisplayOrder.CompareTo(nextPair.Value.DisplayOrder));
            return result;
        }

        /// <summary>
        /// Get system names of installed plugins
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns>List of plugin system names</returns>
        private static IList<string> GetInstalledPluginNames(string filePath)
        {
            //check whether file exists
            if (!_fileProvider.FileExists(filePath))
            {
                //if not, try to parse the file that was used in previous nopCommerce versions
                filePath = _fileProvider.MapPath(ObsoleteInstalledPluginsFilePath);
                if (!_fileProvider.FileExists(filePath))
                    return new List<string>();

                //get plugin system names from the old txt file
                var pluginSystemNames = new List<string>();
                using (var reader = new StringReader(_fileProvider.ReadAllText(filePath, Encoding.UTF8)))
                {
                    string pluginName;
                    while ((pluginName = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(pluginName))
                            pluginSystemNames.Add(pluginName.Trim());
                    }
                }

                //save system names of installed plugins to the new file
                SaveInstalledPluginNames(pluginSystemNames, _fileProvider.MapPath(InstalledPluginsFilePath));

                //and delete the old one
                _fileProvider.DeleteFile(filePath);

                return pluginSystemNames;
            }

            var text = _fileProvider.ReadAllText(filePath, Encoding.UTF8);
            if (string.IsNullOrEmpty(text))
                return new List<string>();

            //get plugin system names from the JSON file
            return JsonConvert.DeserializeObject<IList<string>>(text);
        }

        /// <summary>
        /// Save system names of installed plugins to the file
        /// </summary>
        /// <param name="pluginSystemNames">List of plugin system names</param>
        /// <param name="filePath">Path to the file</param>
        private static void SaveInstalledPluginNames(IList<string> pluginSystemNames, string filePath)
        {
            //save the file
            var text = JsonConvert.SerializeObject(pluginSystemNames, Formatting.Indented);
            _fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
        }

        /// <summary>
        /// Indicates whether assembly file is already loaded
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>True if assembly file is already loaded; otherwise false</returns>
        private static bool IsAlreadyLoaded(string filePath)
        {
            //search library file name in base directory to ignore already existing (loaded) libraries
            //(we do it because not all libraries are loaded immediately after application start)
            if (_baseAppLibraries.Any(sli => sli.Equals(_fileProvider.GetFileName(filePath), StringComparison.InvariantCultureIgnoreCase)))
                return true;

            //compare full assembly name
            //var fileAssemblyName = AssemblyName.GetAssemblyName(filePath);
            //foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    if (a.FullName.Equals(fileAssemblyName.FullName, StringComparison.InvariantCultureIgnoreCase))
            //        return true;
            //}
            //return false;

            //do not compare the full assembly name, just filename
            try
            {
                var fileNameWithoutExt = _fileProvider.GetFileNameWithoutExtension(filePath);
                if (string.IsNullOrEmpty(fileNameWithoutExt))
                    throw new Exception($"Cannot get file extension for {_fileProvider.GetFileName(filePath)}");

                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var assemblyName = a.FullName.Split(',').FirstOrDefault();
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
        /// Perform file deploy
        /// </summary>
        /// <param name="plug">Plugin file info</param>
        /// <param name="applicationPartManager">Application part manager</param>
        /// <param name="config">Config</param>
        /// <param name="shadowCopyPath">Shadow copy path</param>
        /// <returns>Assembly</returns>
        private static Assembly PerformFileDeploy(string plug, ApplicationPartManager applicationPartManager, NopConfig config, string shadowCopyPath="")
        {
            var parent = string.IsNullOrEmpty(plug) ? string.Empty : _fileProvider.GetParentDirectory(plug);

            if (string.IsNullOrEmpty(parent))
                throw new InvalidOperationException($"The plugin directory for the {_fileProvider.GetFileName(plug)} file exists in a folder outside of the allowed nopCommerce folder hierarchy");

            if (!config.UsePluginsShadowCopy)
                return RegisterPluginDefinition(config, applicationPartManager, plug);

            //in order to avoid possible issues we still copy libraries into ~/Plugins/bin/ directory
            if (string.IsNullOrEmpty(shadowCopyPath))
                shadowCopyPath = _shadowCopyFolder;

            _fileProvider.CreateDirectory(shadowCopyPath);
            var shadowCopiedPlug = ShadowCopyFile(plug, shadowCopyPath);

            Assembly shadowCopiedAssembly = null;

            try
            {
                shadowCopiedAssembly = RegisterPluginDefinition(config, applicationPartManager, shadowCopiedPlug);
            }
            catch (FileLoadException)
            {
                if (!config.CopyLockedPluginAssembilesToSubdirectoriesOnStartup || !shadowCopyPath.Equals(_shadowCopyFolder))
                    throw;
            }

            return shadowCopiedAssembly ?? PerformFileDeploy(plug, applicationPartManager, config, _reserveShadowCopyFolder);
        }

        /// <summary>
        /// Register the plugin definition
        /// </summary>
        /// <param name="config">Config</param>
        /// <param name="applicationPartManager">Application part manager</param>
        /// <param name="plug">Plugin file info</param>
        /// <returns>Assembly</returns>
        private static Assembly RegisterPluginDefinition(NopConfig config, ApplicationPartManager applicationPartManager, string plug)
        {
            //we can now register the plugin definition
            var assemblyName = AssemblyName.GetAssemblyName(plug);
            Assembly pluginAssembly;
            try
            {
                pluginAssembly = Assembly.Load(assemblyName);
            }
            catch (FileLoadException)
            {
                if (config.UseUnsafeLoadAssembly)
                {
                    //if an application has been copied from the web, it is flagged by Windows as being a web application,
                    //even if it resides on the local computer.You can change that designation by changing the file properties,
                    //or you can use the<loadFromRemoteSources> element to grant the assembly full trust.As an alternative,
                    //you can use the UnsafeLoadFrom method to load a local assembly that the operating system has flagged as
                    //having been loaded from the web.
                    //see http://go.microsoft.com/fwlink/?LinkId=155569 for more information.
                    pluginAssembly = Assembly.UnsafeLoadFrom(plug);
                }
                else
                {
                    throw;
                }
            }

            Debug.WriteLine("Adding to ApplicationParts: '{0}'", pluginAssembly.FullName);
            applicationPartManager.ApplicationParts.Add(new AssemblyPart(pluginAssembly));

            return pluginAssembly;
        }

        /// <summary>
        /// Copy the plugin file to shadow copy directory
        /// </summary>
        /// <param name="pluginFilePath">Plugin file path</param>
        /// <param name="shadowCopyPlugFolder">Path to shadow copy folder</param>
        /// <returns>File path to shadow copy of plugin file</returns>
        private static string ShadowCopyFile(string pluginFilePath, string shadowCopyPlugFolder)
        {
            var shouldCopy = true;
            var shadowCopiedPlug = _fileProvider.Combine(shadowCopyPlugFolder, _fileProvider.GetFileName(pluginFilePath));

            //check if a shadow copied file already exists and if it does, check if it's updated, if not don't copy
            if (_fileProvider.FileExists(shadowCopiedPlug))
            {
                //it's better to use LastWriteTimeUTC, but not all file systems have this property
                //maybe it is better to compare file hash?
                var areFilesIdentical = _fileProvider.GetCreationTime(shadowCopiedPlug).ToUniversalTime().Ticks >= _fileProvider.GetCreationTime(pluginFilePath).ToUniversalTime().Ticks;
                if (areFilesIdentical)
                {
                    Debug.WriteLine("Not copying; files appear identical: '{0}'", _fileProvider.GetFileName(shadowCopiedPlug));
                    shouldCopy = false;
                }
                else
                {
                    //delete an existing file

                    //More info: https://www.nopcommerce.com/boards/t/11511/access-error-nopplugindiscountrulesbillingcountrydll.aspx?p=4#60838
                    Debug.WriteLine("New plugin found; Deleting the old file: '{0}'", _fileProvider.GetFileName(shadowCopiedPlug));
                    _fileProvider.DeleteFile(shadowCopiedPlug);
                }
            }

            if (!shouldCopy)
                return shadowCopiedPlug;

            try
            {
                _fileProvider.FileCopy(pluginFilePath, shadowCopiedPlug, true);
            }
            catch (IOException)
            {
                Debug.WriteLine(shadowCopiedPlug + " is locked, attempting to rename");
                //this occurs when the files are locked,
                //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                try
                {
                    var oldFile = shadowCopiedPlug + Guid.NewGuid().ToString("N") + ".old";
                    _fileProvider.FileMove(shadowCopiedPlug, oldFile);
                }
                catch (IOException exc)
                {
                    throw new IOException(shadowCopiedPlug + " rename failed, cannot initialize plugin", exc);
                }
                //OK, we've made it this far, now retry the shadow copy
                _fileProvider.FileCopy(pluginFilePath, shadowCopiedPlug, true);
            }

            return shadowCopiedPlug;
        }

        /// <summary>
        /// Determines if the folder is a bin plugin folder for a package
        /// </summary>
        /// <param name="folder">Folder</param>
        /// <returns>True if the folder is a bin plugin folder for a package; otherwise false</returns>
        private static bool IsPackagePluginFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder))
                return false;

            var parent = _fileProvider.GetParentDirectory(folder);

            if (string.IsNullOrEmpty(parent))
                return false;

            if (!_fileProvider.GetDirectoryNameOnly(parent).Equals(PluginsPathName, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="applicationPartManager">Application part manager</param>
        /// <param name="config">Config</param>
        public static void Initialize(ApplicationPartManager applicationPartManager, NopConfig config)
        {
            if (applicationPartManager == null)
                throw new ArgumentNullException(nameof(applicationPartManager));

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            using (new WriteLockDisposable(Locker))
            {
                // TODO: Add verbose exception handling / raising here since this is happening on app startup and could
                // prevent app from starting altogether
                var pluginFolder =  _fileProvider.MapPath(PluginsPath);
                _shadowCopyFolder =  _fileProvider.MapPath(ShadowCopyPath);
                _reserveShadowCopyFolder = _fileProvider.Combine(_fileProvider.MapPath(ShadowCopyPath), $"{RESERVE_SHADOW_COPY_FOLDER_NAME}{DateTime.Now.ToFileTimeUtc()}");
                
                var referencedPlugins = new List<PluginDescriptor>();
                var incompatiblePlugins = new List<string>();

                try
                {
                    var installedPluginSystemNames = GetInstalledPluginNames(_fileProvider.MapPath(InstalledPluginsFilePath));

                    Debug.WriteLine("Creating shadow copy folder and querying for DLLs");
                    //ensure folders are created
                    _fileProvider.CreateDirectory(pluginFolder);
                    _fileProvider.CreateDirectory(_shadowCopyFolder);
                    
                    //get list of all files in bin
                    var binFiles = _fileProvider.GetFiles(_shadowCopyFolder, "*", false);
                    if (config.ClearPluginShadowDirectoryOnStartup)
                    {
                        //clear out shadow copied plugins
                        foreach (var f in binFiles)
                        {
                            if(_fileProvider.GetFileName(f).Equals("placeholder.txt", StringComparison.InvariantCultureIgnoreCase))
                                continue;

                            Debug.WriteLine("Deleting " + f);
                            try
                            {
                                //ignore index.htm
                                var fileName = _fileProvider.GetFileName(f);
                                if (fileName.Equals("index.htm", StringComparison.InvariantCultureIgnoreCase))
                                    continue;

                                _fileProvider.DeleteFile(f);
                            }
                            catch (Exception exc)
                            {
                                Debug.WriteLine("Error deleting file " + f + ". Exception: " + exc);
                            }
                        }

                        //delete all reserve folders
                        foreach (var directory in _fileProvider.GetDirectories(_shadowCopyFolder, RESERVE_SHADOW_COPY_FOLDER_NAME_PATTERN))
                        {
                            try
                            {
                                _fileProvider.DeleteDirectory(directory);
                            }
                            catch
                            {
                                //do nothing
                            }
                        }
                    }
                   
                    //load description files
                    foreach (var dfd in GetDescriptionFilesAndDescriptors(pluginFolder))
                    {
                        var descriptionFile = dfd.Key;
                        var pluginDescriptor = dfd.Value;

                        //ensure that version of plugin is valid
                        if (!pluginDescriptor.SupportedVersions.Contains(NopVersion.CurrentVersion, StringComparer.InvariantCultureIgnoreCase))
                        {
                            incompatiblePlugins.Add(pluginDescriptor.SystemName);
                            continue;
                        }

                        //some validation
                        if (string.IsNullOrWhiteSpace(pluginDescriptor.SystemName))
                            throw new Exception($"A plugin '{descriptionFile}' has no system name. Try assigning the plugin a unique name and recompiling.");
                        if (referencedPlugins.Contains(pluginDescriptor))
                            throw new Exception($"A plugin with '{pluginDescriptor.SystemName}' system name is already defined");

                        //set 'Installed' property
                        pluginDescriptor.Installed = installedPluginSystemNames
                            .FirstOrDefault(x => x.Equals(pluginDescriptor.SystemName, StringComparison.InvariantCultureIgnoreCase)) != null;

                        try
                        {
                            var directoryName = _fileProvider.GetDirectoryName(descriptionFile);
                            if (string.IsNullOrEmpty(directoryName))
                                throw new Exception($"Directory cannot be resolved for '{_fileProvider.GetFileName(descriptionFile)}' description file");

                            //get list of all DLLs in plugins (not in bin!)
                            var pluginFiles = _fileProvider.GetFiles(directoryName, "*.dll", false)
                                //just make sure we're not registering shadow copied plugins
                                .Where(x => !binFiles.Select(q => q).Contains(x))
                                .Where(x => IsPackagePluginFolder(_fileProvider.GetDirectoryName(x)))
                                .ToList();

                            //other plugin description info
                            var mainPluginFile = pluginFiles
                                .FirstOrDefault(x => _fileProvider.GetFileName(x).Equals(pluginDescriptor.AssemblyFileName, StringComparison.InvariantCultureIgnoreCase));
                            
                            //plugin have wrong directory
                            if (mainPluginFile == null)
                            {
                                incompatiblePlugins.Add(pluginDescriptor.SystemName);
                                continue;
                            }

                            pluginDescriptor.OriginalAssemblyFile = mainPluginFile;

                            //shadow copy main plugin file
                            pluginDescriptor.ReferencedAssembly = PerformFileDeploy(mainPluginFile, applicationPartManager, config);

                            //load all other referenced assemblies now
                            foreach (var plugin in pluginFiles
                                .Where(x => !_fileProvider.GetFileName(x).Equals(_fileProvider.GetFileName(mainPluginFile), StringComparison.InvariantCultureIgnoreCase))
                                .Where(x => !IsAlreadyLoaded(x)))
                                    PerformFileDeploy(plugin, applicationPartManager, config);
                            
                            //init plugin type (only one plugin per assembly is allowed)
                            foreach (var t in pluginDescriptor.ReferencedAssembly.GetTypes())
                                if (typeof(IPlugin).IsAssignableFrom(t))
                                    if (!t.IsInterface)
                                        if (t.IsClass && !t.IsAbstract)
                                        {
                                            pluginDescriptor.PluginType = t;
                                            break;
                                        }

                            referencedPlugins.Add(pluginDescriptor);
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            //add a plugin name. this way we can easily identify a problematic plugin
                            var msg = $"Plugin '{pluginDescriptor.FriendlyName}'. ";
                            foreach (var e in ex.LoaderExceptions)
                                msg += e.Message + Environment.NewLine;

                            var fail = new Exception(msg, ex);
                            throw fail;
                        }
                        catch (Exception ex)
                        {
                            //add a plugin name. this way we can easily identify a problematic plugin
                            var msg = $"Plugin '{pluginDescriptor.FriendlyName}'. {ex.Message}";

                            var fail = new Exception(msg, ex);
                            throw fail;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var msg = string.Empty;
                    for (var e = ex; e != null; e = e.InnerException)
                        msg += e.Message + Environment.NewLine;

                    var fail = new Exception(msg, ex);
                    throw fail;
                }

                ReferencedPlugins = referencedPlugins;
                IncompatiblePlugins = incompatiblePlugins;
            }
        }

        /// <summary>
        /// Mark plugin as installed
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        public static void MarkPluginAsInstalled(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                throw new ArgumentNullException(nameof(systemName));

            var filePath = _fileProvider.MapPath(InstalledPluginsFilePath);

            //create file if not exists
            _fileProvider.CreateFile(filePath);

            //get installed plugin names
            var installedPluginSystemNames = GetInstalledPluginNames(filePath);

            //add plugin system name to the list if doesn't already exist
            var alreadyMarkedAsInstalled = installedPluginSystemNames.Any(pluginName => pluginName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
            if (!alreadyMarkedAsInstalled)
                installedPluginSystemNames.Add(systemName);

            //save installed plugin names to the file
            SaveInstalledPluginNames(installedPluginSystemNames,filePath);
        }

        /// <summary>
        /// Mark plugin as uninstalled
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        public static void MarkPluginAsUninstalled(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                throw new ArgumentNullException(nameof(systemName));

            var filePath = _fileProvider.MapPath(InstalledPluginsFilePath);

            //create file if not exists
            _fileProvider.CreateFile(filePath);

            //get installed plugin names
            var installedPluginSystemNames = GetInstalledPluginNames(filePath);

            //remove plugin system name from the list if exists
            var alreadyMarkedAsInstalled = installedPluginSystemNames.Any(pluginName => pluginName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
            if (alreadyMarkedAsInstalled)
                installedPluginSystemNames.Remove(systemName);

            //save installed plugin names to the file
            SaveInstalledPluginNames(installedPluginSystemNames,filePath);
        }

        /// <summary>
        /// Mark plugin as uninstalled
        /// </summary>
        public static void MarkAllPluginsAsUninstalled()
        {
            var filePath = _fileProvider.MapPath(InstalledPluginsFilePath);
            if (_fileProvider.FileExists(filePath))
                _fileProvider.DeleteFile(filePath);
        }

        /// <summary>
        /// Find a plugin descriptor by some type which is located into the same assembly as plugin
        /// </summary>
        /// <param name="typeInAssembly">Type</param>
        /// <returns>Plugin descriptor if exists; otherwise null</returns>
        public static PluginDescriptor FindPlugin(Type typeInAssembly)
        {
            if (typeInAssembly == null)
                throw new ArgumentNullException(nameof(typeInAssembly));

            return ReferencedPlugins?.FirstOrDefault(plugin =>
                plugin.ReferencedAssembly != null &&
                plugin.ReferencedAssembly.FullName.Equals(typeInAssembly.Assembly.FullName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Get plugin descriptor from the plugin description file
        /// </summary>
        /// <param name="filePath">Path to the description file</param>
        /// <returns>Plugin descriptor</returns>
        public static PluginDescriptor GetPluginDescriptorFromFile(string filePath)
        {
            var text = _fileProvider.ReadAllText(filePath, Encoding.UTF8);

            return GetPluginDescriptorFromText(text);
        }

        /// <summary>
        /// Get plugin descriptor from the description text
        /// </summary>
        /// <param name="text">Description text</param>
        /// <returns>Plugin descriptor</returns>
        public static PluginDescriptor GetPluginDescriptorFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new PluginDescriptor();

            //get plugin descriptor from the JSON file
            var descriptor = JsonConvert.DeserializeObject<PluginDescriptor>(text);

            //nopCommerce 2.00 didn't have 'SupportedVersions' parameter, so let's set it to "2.00"
            if (!descriptor.SupportedVersions.Any())
                descriptor.SupportedVersions.Add("2.00");

            return descriptor;
        }

        /// <summary>
        /// Save plugin descriptor to the plugin description file
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor</param>
        public static void SavePluginDescriptor(PluginDescriptor pluginDescriptor)
        {
            if (pluginDescriptor == null)
                throw new ArgumentException(nameof(pluginDescriptor));

            //get the description file path
            if (pluginDescriptor.OriginalAssemblyFile == null)
                throw new Exception($"Cannot load original assembly path for {pluginDescriptor.SystemName} plugin.");

            var filePath = _fileProvider.Combine(_fileProvider.GetDirectoryName(pluginDescriptor.OriginalAssemblyFile), PluginDescriptionFileName);
            if (!_fileProvider.FileExists(filePath))
                throw new Exception($"Description file for {pluginDescriptor.SystemName} plugin does not exist. {filePath}");

            //save the file
            var text = JsonConvert.SerializeObject(pluginDescriptor, Formatting.Indented);
            _fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
        }

        /// <summary>
        /// Delete plugin directory from disk storage
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor</param>
        /// <returns>True if plugin directory is deleted, false if not</returns>
        public static bool DeletePlugin(PluginDescriptor pluginDescriptor)
        {
            //no plugin descriptor set
            if (pluginDescriptor == null)
                return false;

            //check whether plugin is installed
            if (pluginDescriptor.Installed)
                return false;

            var directoryName = _fileProvider.GetDirectoryName(pluginDescriptor.OriginalAssemblyFile);

            if ( _fileProvider.DirectoryExists(directoryName))
                _fileProvider.DeleteDirectory(directoryName);

            return true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the path to file that contained (in previous versions) installed plugin system names
        /// </summary>
        public static string ObsoleteInstalledPluginsFilePath => "~/App_Data/InstalledPlugins.txt";

        /// <summary>
        /// Gets the path to file that contains installed plugin system names
        /// </summary>
        public static string InstalledPluginsFilePath => "~/App_Data/installedPlugins.json";

        /// <summary>
        /// Gets the path to plugins folder
        /// </summary>
        public static string PluginsPath => "~/Plugins";

        /// <summary>
        /// Gets the plugins folder name
        /// </summary>
        public static string PluginsPathName => "Plugins";

        /// <summary>
        /// Gets the path to plugins shadow copies folder
        /// </summary>
        public static string ShadowCopyPath => "~/Plugins/bin";

        /// <summary>
        /// Gets the path to plugins refs folder
        /// </summary>
        public static string RefsPathName => "refs";

        /// <summary>
        /// Gets the name of the plugin description file
        /// </summary>
        public static string PluginDescriptionFileName => "plugin.json";

        /// <summary>
        /// Returns a collection of all referenced plugin assemblies that have been shadow copied
        /// </summary>
        public static IEnumerable<PluginDescriptor> ReferencedPlugins { get; set; }

        /// <summary>
        /// Returns a collection of all plugin which are not compatible with the current version
        /// </summary>
        public static IEnumerable<string> IncompatiblePlugins { get; set; }

        #endregion
    }
}