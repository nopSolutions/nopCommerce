using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Represents the manager for uploading plugins
    /// </summary>
    public static class UploadManager
    {
        #region Properties

        /// <summary>
        /// Gets the path to temp folder with uploads
        /// </summary>
        public static string PluginsTempPath => "~/App_Data/TempUploads";

        /// <summary>
        /// Gets the name of the file containing information about the uploaded plugins
        /// </summary>
        public static string UploadedPluginsFileName => "uploadedPlugins.json";

        #endregion

        #region Utilities

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
                    if (entry.FullName.Equals($"{uploadedPluginDirectoryName}/{PluginManager.PluginDescriptionFileName}",
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        using (var unzippedEntryStream = entry.Open())
                        {
                            using (var reader = new StreamReader(unzippedEntryStream))
                            {
                                var text = reader.ReadToEnd();
                                pluginDescriptor = PluginManager.GetPluginDescriptorFromText(text);
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
                throw new Exception($"No {PluginManager.PluginDescriptionFileName} file is found. It should be in the root of the archive.");

            //new plugin path
            if (uploadedPluginDirectoryName == null)
                throw new Exception("Cannot get the plugin directory name");

            var pluginFolder = CommonHelper.MapPath(PluginManager.PluginsPath);
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
                    .FirstOrDefault(entry => entry.Name.Equals(UploadedPluginsFileName, StringComparison.InvariantCultureIgnoreCase)
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
                    var pluginDescriptorPath = $"{pluginPath}{PluginManager.PluginDescriptionFileName}";
                    var descriptorEntry = archive.Entries.FirstOrDefault(entry => entry.FullName.Equals(pluginDescriptorPath, StringComparison.InvariantCultureIgnoreCase));
                    if (descriptorEntry == null)
                        return null;

                    using (var unzippedEntryStream = descriptorEntry.Open())
                    {
                        using (var reader = new StreamReader(unzippedEntryStream))
                        {
                            var pluginDescriptor = PluginManager.GetPluginDescriptorFromText(reader.ReadToEnd());
                            return new { DirectoryName = directoryName, PluginPath = pluginPath, PluginDescriptor = pluginDescriptor };
                        }
                    }
                }).Where(plugin => plugin?.PluginDescriptor?.SupportedVersions.Contains(NopVersion.CurrentVersion) ?? false).ToList();

                //extract plugins into the plugin folder
                var pluginFolder = CommonHelper.MapPath(PluginManager.PluginsPath);
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
                        {
                            Directory.CreateDirectory(directoryPath);
                            if (directoryPath.Equals(pluginPath, StringComparison.InvariantCultureIgnoreCase))
                                entry.ExtractToFile(entryPath);
                        }
                        else
                            entry.ExtractToFile(entryPath);
                    }
                }

                return pluginsInArchive.Select(plugin => plugin.PluginDescriptor).ToList();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Upload plugins
        /// </summary>
        /// <param name="archivefile">File</param>
        /// <returns>List of plugin descriptor</returns>
        public static IList<PluginDescriptor> UploadPlugins(IFormFile archivefile)
        {
            if (archivefile == null)
                throw new ArgumentNullException(nameof(archivefile));

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
                //you can find a sample of such descriptive file in Libraries\Nop.Core\Plugins\Samples\
                var jsonFileExists = false;
                using (var archive = ZipFile.OpenRead(zipFilePath))
                {
                    jsonFileExists = archive.Entries
                        .Any(entry => entry.Name.Equals(UploadedPluginsFileName, StringComparison.InvariantCultureIgnoreCase)
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
    }
}