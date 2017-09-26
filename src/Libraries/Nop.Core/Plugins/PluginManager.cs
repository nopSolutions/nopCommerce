using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Newtonsoft.Json;
using Nop.Core.ComponentModel;
using Nop.Core.Configuration;

//Contributor: Umbraco (http://www.umbraco.com). Thanks a lot! 
//SEE THIS POST for full details of what this does - http://shazwazza.com/post/Developing-a-plugin-framework-in-ASPNET-with-medium-trust.aspx

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Sets the application up for the plugin referencing
    /// </summary>
    public class PluginManager
    {
        #region Const

        private const string InstalledPluginsFilePath = "~/App_Data/InstalledPlugins.txt";
        private const string PluginsPath = "~/Plugins";
        private const string PluginsTempPath = "~/App_Data/TempUploads";
        private const string PluginsPathName = "Plugins";
        private const string ShadowCopyPath = "~/Plugins/bin";
        private const string RefsPathName = "refs";
        private const string PluginDescriptorFile = "Description.txt";
        private const string UploadedPluginsDescriptionFile = "uploadedPlugins.json";

        #endregion

        #region Fields

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
        private static DirectoryInfo _shadowCopyFolder;
        private static readonly List<string> BaseAppLibraries;

        #endregion

        #region Ctor

        static PluginManager()
        {
            //get all libraries from /bin/{version}/ directory
            BaseAppLibraries = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
                .GetFiles("*.dll", SearchOption.TopDirectoryOnly).Select(fi => fi.Name).ToList();
            //get all libraries from base site directory
            if(!AppDomain.CurrentDomain.BaseDirectory.Equals(Environment.CurrentDirectory, StringComparison.InvariantCultureIgnoreCase))
                BaseAppLibraries.AddRange(new DirectoryInfo(Environment.CurrentDirectory).GetFiles("*.dll", SearchOption.TopDirectoryOnly).Select(fi => fi.Name));
            //get all libraries from refs directory
            var refsPathName = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, RefsPathName));
            if(refsPathName.Exists)
                BaseAppLibraries.AddRange(refsPathName.GetFiles("*.dll", SearchOption.TopDirectoryOnly).Select(fi => fi.Name));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a collection of all referenced plugin assemblies that have been shadow copied
        /// </summary>
        public static IEnumerable<PluginDescriptor> ReferencedPlugins { get; set; }

        /// <summary>
        /// Returns a collection of all plugin which are not compatible with the current version
        /// </summary>
        public static IEnumerable<string> IncompatiblePlugins { get; set; }

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
                var pluginFolder = new DirectoryInfo(CommonHelper.MapPath(PluginsPath));
                _shadowCopyFolder = new DirectoryInfo(CommonHelper.MapPath(ShadowCopyPath));

                var referencedPlugins = new List<PluginDescriptor>();
                var incompatiblePlugins = new List<string>();

                try
                {
                    var installedPluginSystemNames = PluginFileParser.ParseInstalledPluginsFile(GetInstalledPluginsFilePath());

                    Debug.WriteLine("Creating shadow copy folder and querying for DLLs");
                    //ensure folders are created
                    Directory.CreateDirectory(pluginFolder.FullName);
                    Directory.CreateDirectory(_shadowCopyFolder.FullName);
                    
                    //get list of all files in bin
                    var binFiles = _shadowCopyFolder.GetFiles("*", SearchOption.AllDirectories);
                    if (config.ClearPluginShadowDirectoryOnStartup)
                    {
                        //clear out shadow copied plugins
                        foreach (var f in binFiles)
                        {
                            Debug.WriteLine("Deleting " + f.Name);
                            try
                            {
                                //ignore index.htm
                                var fileName = Path.GetFileName(f.FullName);
                                if (fileName.Equals("index.htm", StringComparison.InvariantCultureIgnoreCase))
                                    continue;

                                File.Delete(f.FullName);
                            }
                            catch (Exception exc)
                            {
                                Debug.WriteLine("Error deleting file " + f.Name + ". Exception: " + exc);
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
                        if (String.IsNullOrWhiteSpace(pluginDescriptor.SystemName))
                            throw new Exception($"A plugin '{descriptionFile.FullName}' has no system name. Try assigning the plugin a unique name and recompiling.");
                        if (referencedPlugins.Contains(pluginDescriptor))
                            throw new Exception($"A plugin with '{pluginDescriptor.SystemName}' system name is already defined");

                        //set 'Installed' property
                        pluginDescriptor.Installed = installedPluginSystemNames
                            .FirstOrDefault(x => x.Equals(pluginDescriptor.SystemName, StringComparison.InvariantCultureIgnoreCase)) != null;

                        try
                        {
                            if (descriptionFile.Directory == null)
                                throw new Exception($"Directory cannot be resolved for '{descriptionFile.Name}' description file");

                            //get list of all DLLs in plugins (not in bin!)
                            var pluginFiles = descriptionFile.Directory.GetFiles("*.dll", SearchOption.AllDirectories)
                                //just make sure we're not registering shadow copied plugins
                                .Where(x => !binFiles.Select(q => q.FullName).Contains(x.FullName))
                                .Where(x => IsPackagePluginFolder(x.Directory))
                                .ToList();

                            //other plugin description info
                            var mainPluginFile = pluginFiles
                                .FirstOrDefault(x => x.Name.Equals(pluginDescriptor.PluginFileName, StringComparison.InvariantCultureIgnoreCase));

                            //plugin have wrong directory
                            if (mainPluginFile == null)
                            {
                                incompatiblePlugins.Add(pluginDescriptor.SystemName);
                                continue;
                            }

                            pluginDescriptor.OriginalAssemblyFile = mainPluginFile;

                            //shadow copy main plugin file
                            pluginDescriptor.ReferencedAssembly = PerformFileDeploy(mainPluginFile, applicationPartManager);

                            //load all other referenced assemblies now
                            foreach (var plugin in pluginFiles
                                .Where(x => !x.Name.Equals(mainPluginFile.Name, StringComparison.InvariantCultureIgnoreCase))
                                .Where(x => !IsAlreadyLoaded(x)))
                                    PerformFileDeploy(plugin, applicationPartManager);
                            
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
            if (String.IsNullOrEmpty(systemName))
                throw new ArgumentNullException(nameof(systemName));

            var filePath = CommonHelper.MapPath(InstalledPluginsFilePath);
            if (!File.Exists(filePath))
                using (File.Create(filePath))
                {
                    //we use 'using' to close the file after it's created
                }

            var installedPluginSystemNames = PluginFileParser.ParseInstalledPluginsFile(GetInstalledPluginsFilePath());
            bool alreadyMarkedAsInstalled = installedPluginSystemNames
                                .FirstOrDefault(x => x.Equals(systemName, StringComparison.InvariantCultureIgnoreCase)) != null;
            if (!alreadyMarkedAsInstalled)
                installedPluginSystemNames.Add(systemName);
            PluginFileParser.SaveInstalledPluginsFile(installedPluginSystemNames,filePath);
        }

        /// <summary>
        /// Mark plugin as uninstalled
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        public static void MarkPluginAsUninstalled(string systemName)
        {
            if (String.IsNullOrEmpty(systemName))
                throw new ArgumentNullException(nameof(systemName));

            var filePath = CommonHelper.MapPath(InstalledPluginsFilePath);
            if (!File.Exists(filePath))
                using (File.Create(filePath))
                {
                    //we use 'using' to close the file after it's created
                }


            var installedPluginSystemNames = PluginFileParser.ParseInstalledPluginsFile(GetInstalledPluginsFilePath());
            bool alreadyMarkedAsInstalled = installedPluginSystemNames
                                .FirstOrDefault(x => x.Equals(systemName, StringComparison.InvariantCultureIgnoreCase)) != null;
            if (alreadyMarkedAsInstalled)
                installedPluginSystemNames.Remove(systemName);
            PluginFileParser.SaveInstalledPluginsFile(installedPluginSystemNames,filePath);
        }

        /// <summary>
        /// Mark plugin as uninstalled
        /// </summary>
        public static void MarkAllPluginsAsUninstalled()
        {
            var filePath = CommonHelper.MapPath(InstalledPluginsFilePath);
            if (File.Exists(filePath))
                File.Delete(filePath);
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

            if (ReferencedPlugins == null)
                return null;

            return ReferencedPlugins.FirstOrDefault(plugin => plugin.ReferencedAssembly != null
                && plugin.ReferencedAssembly.FullName.Equals(typeInAssembly.Assembly.FullName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Upload plugins
        /// </summary>
        /// <param name="archivefile">File</param>
        /// <returns>List of plugin descriptor</returns>
        public static IList<PluginDescriptor> UploadPlugins(IFormFile archivefile)
        {
            if (archivefile == null)
                throw  new ArgumentNullException(nameof(archivefile));

            string zipFilePath = null;
            var pluginDescriptors = new List<PluginDescriptor>();
            try
            {
                //only zip archives are supported
                var extension = Path.GetExtension(archivefile.FileName);
                if (extension == null || !extension.Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception("Only zip archives are supported");

                //ensure temp folder is created
                var pluginTempFolder = CommonHelper.MapPath(PluginsTempPath);
                Directory.CreateDirectory(new DirectoryInfo(pluginTempFolder).FullName);

                //copy original archive to the temp folder
                zipFilePath = Path.Combine(pluginTempFolder, archivefile.FileName);
                using (var fileStream = new FileStream(zipFilePath, FileMode.Create))
                    archivefile.CopyTo(fileStream);

                //check whether there is a descriptive JSON file in the root of the archive
                var jsonFileExists = false;
                using (var archive = ZipFile.OpenRead(zipFilePath))
                {
                    jsonFileExists = archive.Entries
                        .Any(entry => entry.Name.Equals(UploadedPluginsDescriptionFile, StringComparison.InvariantCultureIgnoreCase) 
                            && string.IsNullOrEmpty(Path.GetDirectoryName(entry.FullName)));
                }

                if (!jsonFileExists)
                {
                    //JSON file doesn't exist, so there is a single plugin in the archive, just extract it
                    pluginDescriptors.Add(UploadSinglePlugin(zipFilePath));
                }
                else
                {
                    //JSON file exists, so there are multiple plugins or plugin versions in the archive
                    pluginDescriptors.AddRange(UploadMultiplePlugins(zipFilePath));
                }
            }
            finally
            {
                //delete temporary file
                if (!string.IsNullOrEmpty(zipFilePath))
                    File.Delete(zipFilePath);
            }

            return pluginDescriptors;
        }
        
        #endregion

        #region Utilities

        /// <summary>
        /// Get description files
        /// </summary>
        /// <param name="pluginFolder">Plugin directory info</param>
        /// <returns>Original and parsed description files</returns>
        private static IEnumerable<KeyValuePair<FileInfo, PluginDescriptor>> GetDescriptionFilesAndDescriptors(DirectoryInfo pluginFolder)
        {
            if (pluginFolder == null)
                throw new ArgumentNullException(nameof(pluginFolder));

            //create list (<file info, parsed plugin descritor>)
            var result = new List<KeyValuePair<FileInfo, PluginDescriptor>>();
            
            //add display order and path to list
            foreach (var descriptionFile in pluginFolder.GetFiles(PluginDescriptorFile, SearchOption.AllDirectories))
            {
                if (!IsPackagePluginFolder(descriptionFile.Directory))
                    continue;

                //parse file
                var pluginDescriptor = PluginFileParser.ParsePluginDescriptionFile(descriptionFile.FullName);

                //populate list
                result.Add(new KeyValuePair<FileInfo, PluginDescriptor>(descriptionFile, pluginDescriptor));
            }

            //sort list by display order. NOTE: Lowest DisplayOrder will be first i.e 0 , 1, 1, 1, 5, 10
            //it's required: https://www.nopcommerce.com/boards/t/17455/load-plugins-based-on-their-displayorder-on-startup.aspx
            result.Sort((firstPair, nextPair) => firstPair.Value.DisplayOrder.CompareTo(nextPair.Value.DisplayOrder));
            return result;
        }

        /// <summary>
        /// Upload the single plugin from the archive into the plugin folder
        /// </summary>
        /// <param name="archivePath">Path to the archive</param>
        /// <returns>Plugin descriptor</returns>
        private static PluginDescriptor UploadSinglePlugin(string archivePath)
        {
            //ensure we have a valid plugin description file and the current version is supported
            PluginDescriptor pluginDescriptor = null;
            var uploadedPluginDirectoryName = string.Empty;
            using (var archive = ZipFile.OpenRead(archivePath))
            {
                //the archive should contain only one root directory (the plugin one)
                var rootDirectories = archive.Entries
                    .Where(entry => entry.FullName.Count(ch => ch == '/') == 1 && entry.FullName.EndsWith("/"))
                    .ToList();
                if (rootDirectories.Count != 1)
                    throw new Exception("The archive should contain only one root plugin directory. For example, Payments.PayPalDirect.");
                //the plugin directory name (remove the ending /)
                uploadedPluginDirectoryName = rootDirectories.First().FullName.Replace("/", "");

                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.Equals($"{uploadedPluginDirectoryName}/{PluginDescriptorFile}",
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        using (var unzippedEntryStream = entry.Open())
                        {
                            using (var reader = new StreamReader(unzippedEntryStream))
                            {
                                string text = reader.ReadToEnd();
                                pluginDescriptor = PluginFileParser.ParsePluginDescription(text);
                                if (!pluginDescriptor.SupportedVersions.Contains(NopVersion.CurrentVersion,
                                    StringComparer.InvariantCultureIgnoreCase))
                                    throw new Exception(
                                        $"This plugin doesn't support the current version - {NopVersion.CurrentVersion}");

                                break;
                            }
                        }
                    }
                }
            }

            if (pluginDescriptor == null)
                throw new Exception($"No {PluginDescriptorFile} file is found. It should be in the root of the archive.");

            //new plugin path
            if (uploadedPluginDirectoryName == null)
                throw new Exception("Cannot get the plugin directory name");

            var pluginFolder = CommonHelper.MapPath(PluginsPath);
            var uploadedPluginPath = Path.Combine(pluginFolder, uploadedPluginDirectoryName);

            //ensure it's a new directory (e.g. some old files are not required when re-uploading a plugin)
            //furthermore, zip extract functionality cannot override existing files
            //but there could deletion issues (related to file locking, etc). In such cases the directory should be deleted manually
            if (Directory.Exists(uploadedPluginPath))
                CommonHelper.DeleteDirectory(uploadedPluginPath);

            //extract to /Plugins
            ZipFile.ExtractToDirectory(archivePath, pluginFolder);

            return pluginDescriptor;
        }

        /// <summary>
        /// Upload multiple plugins from the archive into the plugin folder
        /// </summary>
        /// <param name="archivePath">Path to the archive</param>
        /// <returns>List of plugin descriptor</returns>
        private static IList<PluginDescriptor> UploadMultiplePlugins(string archivePath)
        {
            using (var archive = ZipFile.OpenRead(archivePath))
            {
                //get plugin directory names from the descriptive JSON file
                var pluginDirectories = new List<string>();
                var jsonFileEntry = archive.Entries
                    .FirstOrDefault(entry => entry.Name.Equals(UploadedPluginsDescriptionFile, StringComparison.InvariantCultureIgnoreCase)
                        && string.IsNullOrEmpty(Path.GetDirectoryName(entry.FullName)));
                using (var unzippedEntryStream = jsonFileEntry.Open())
                {
                    using (var reader = new StreamReader(unzippedEntryStream))
                    {
                        var definitionType = new[] { new { SystemName = string.Empty, Version = string.Empty, DirectoryPath = string.Empty, SourceDirectoryPath = string.Empty } };
                        pluginDirectories = JsonConvert.DeserializeAnonymousType(reader.ReadToEnd(), definitionType)
                            .Select(plugin => plugin.DirectoryPath).ToList();
                    }
                }

                //get plugins descriptors contained in the archive and ensure that the current version is supported
                var pluginsInArchive = pluginDirectories.Select(directoryPath =>
                {
                    //the plugin path should end with a slash
                    var pluginPath = $"{directoryPath.TrimEnd('/')}/";

                    //get the plugin directory name
                    var directoryName = Path.GetFileName(pluginPath.TrimEnd('/'));

                    //try to get the plugin descriptor entry
                    var pluginDescriptorPath = $"{pluginPath}{PluginDescriptorFile}";
                    var descriptorEntry = archive.Entries.FirstOrDefault(entry => entry.FullName.Equals(pluginDescriptorPath, StringComparison.InvariantCultureIgnoreCase));
                    if (descriptorEntry == null)
                        return null;
                    
                    using (var unzippedEntryStream = descriptorEntry.Open())
                    {
                        using (var reader = new StreamReader(unzippedEntryStream))
                        {
                            var pluginDescriptor = PluginFileParser.ParsePluginDescription(reader.ReadToEnd());
                            return new { DirectoryName = directoryName, PluginPath = pluginPath, PluginDescriptor = pluginDescriptor };
                        }
                    }
                }).Where(plugin => plugin?.PluginDescriptor?.SupportedVersions.Contains(NopVersion.CurrentVersion) ?? false).ToList();

                //extract plugins into the plugin folder
                var pluginFolder = CommonHelper.MapPath(PluginsPath);
                foreach (var plugin in pluginsInArchive)
                {
                    var pluginPath = Path.Combine(pluginFolder, plugin.DirectoryName);

                    //ensure it's a new directory (e.g. some old files are not required when re-uploading a plugin)
                    //furthermore, zip extract functionality cannot override existing files
                    //but there could deletion issues (related to file locking, etc). In such cases the directory should be deleted manually
                    if (Directory.Exists(pluginPath))
                        CommonHelper.DeleteDirectory(pluginPath);
                    Directory.CreateDirectory(pluginPath);

                    //extract entries into files
                    var pluginEntries = archive.Entries.Where(entry => entry.FullName.StartsWith(plugin.PluginPath, StringComparison.InvariantCultureIgnoreCase)
                        && !entry.FullName.Equals(plugin.PluginPath, StringComparison.InvariantCultureIgnoreCase));
                    foreach (var entry in pluginEntries)
                    {
                        var entryPath = Path.Combine(pluginPath, entry.FullName.Substring(plugin.PluginPath.Length));
                        var directoryPath = Path.GetDirectoryName(entryPath);
                        if (!Directory.Exists(directoryPath))
                            Directory.CreateDirectory(directoryPath);
                        else
                            entry.ExtractToFile(entryPath);
                    }
                }

                return pluginsInArchive.Select(plugin => plugin.PluginDescriptor).ToList();
            }
        }

        /// <summary>
        /// Indicates whether assembly file is already loaded
        /// </summary>
        /// <param name="fileInfo">File info</param>
        /// <returns>Result</returns>
        private static bool IsAlreadyLoaded(FileInfo fileInfo)
        {
            //search library file name in base directory to ignore already existing (loaded) libraries
            //(we do it because not all libraries are loaded immediately after application start)
            if (BaseAppLibraries.Any(sli => sli.Equals(fileInfo.Name, StringComparison.InvariantCultureIgnoreCase)))
                return true;

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
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                if (string.IsNullOrEmpty(fileNameWithoutExt))
                    throw new Exception($"Cannot get file extension for {fileInfo.Name}");

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
        /// <returns>Assembly</returns>
        private static Assembly PerformFileDeploy(FileInfo plug, ApplicationPartManager applicationPartManager)
        {
            if (plug.Directory == null || plug.Directory.Parent == null)
                throw new InvalidOperationException("The plugin directory for the " + plug.Name + " file exists in a folder outside of the allowed nopCommerce folder hierarchy");

            //but in order to avoid possible issues we still copy libraries into ~/Plugins/bin/ directory
            var shadowCopyPlugFolder = Directory.CreateDirectory(_shadowCopyFolder.FullName);
            var shadowCopiedPlug = ShadowCopyFile(plug, shadowCopyPlugFolder);

            //we can now register the plugin definition
            var shadowCopiedAssembly = Assembly.Load(AssemblyName.GetAssemblyName(shadowCopiedPlug.FullName));
            Debug.WriteLine("Adding to ApplicationParts: '{0}'", shadowCopiedAssembly.FullName);
            applicationPartManager.ApplicationParts.Add(new AssemblyPart(shadowCopiedAssembly));

            return shadowCopiedAssembly;
        }
        
        /// <summary>
        /// Copy the plugin file to shadow copy directory
        /// </summary>
        /// <param name="plug"></param>
        /// <param name="shadowCopyPlugFolder"></param>
        /// <returns></returns>
        private static FileInfo ShadowCopyFile(FileInfo plug, DirectoryInfo shadowCopyPlugFolder)
        {
            var shouldCopy = true;
            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plug.Name));

            //check if a shadow copied file already exists and if it does, check if it's updated, if not don't copy
            if (shadowCopiedPlug.Exists)
            {
                //it's better to use LastWriteTimeUTC, but not all file systems have this property
                //maybe it is better to compare file hash?
                var areFilesIdentical = shadowCopiedPlug.CreationTimeUtc.Ticks >= plug.CreationTimeUtc.Ticks;
                if (areFilesIdentical)
                {
                    Debug.WriteLine("Not copying; files appear identical: '{0}'", shadowCopiedPlug.Name);
                    shouldCopy = false;
                }
                else
                {
                    //delete an existing file

                    //More info: https://www.nopcommerce.com/boards/t/11511/access-error-nopplugindiscountrulesbillingcountrydll.aspx?p=4#60838
                    Debug.WriteLine("New plugin found; Deleting the old file: '{0}'", shadowCopiedPlug.Name);
                    File.Delete(shadowCopiedPlug.FullName);
                }
            }

            if (!shouldCopy)
                return shadowCopiedPlug;

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
                //OK, we've made it this far, now retry the shadow copy
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
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
            if (!folder.Parent.Name.Equals(PluginsPathName, StringComparison.InvariantCultureIgnoreCase)) return false;

            return true;
        }

        /// <summary>
        /// Gets the full path of InstalledPlugins.txt file
        /// </summary>
        /// <returns></returns>
        private static string GetInstalledPluginsFilePath()
        { 
            return CommonHelper.MapPath(InstalledPluginsFilePath);
        }
        
        #endregion
    }
}
