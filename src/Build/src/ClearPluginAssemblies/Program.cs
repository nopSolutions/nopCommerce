using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClearPluginAssemblies
{
    public class Program
    {
        protected const string FILES_TO_DELETE = "dotnet-bundle.exe;Nop.Web.pdb;Nop.Web.exe;Nop.Web.exe.config";

        protected static void Clear(string paths, IList<string> fileNames, bool saveLocalesFolders)
        {
            foreach (var pluginPath in paths.Split(';'))
            {
                try
                {
                    var pluginDirectoryInfo = new DirectoryInfo(pluginPath);
                    var allDirectoryInfo = new List<DirectoryInfo> { pluginDirectoryInfo };

                    if (!saveLocalesFolders)
                        allDirectoryInfo.AddRange(pluginDirectoryInfo.GetDirectories());

                    foreach (var directoryInfo in allDirectoryInfo)
                    {
                        foreach (var fileName in fileNames)
                        {
                            //delete dll file if it exists in current path
                            var dllfilePath = Path.Combine(directoryInfo.FullName, fileName + ".dll");
                            if (File.Exists(dllfilePath))
                                File.Delete(dllfilePath);
                            //delete pdb file if it exists in current path
                            var pdbfilePath = Path.Combine(directoryInfo.FullName, fileName + ".pdb");
                            if (File.Exists(pdbfilePath))
                                File.Delete(pdbfilePath);
                        }

                        foreach (var fileName in FILES_TO_DELETE.Split(';'))
                        {
                            //delete file if it exists in current path
                            var pdbfilePath = Path.Combine(directoryInfo.FullName, fileName);
                            if (File.Exists(pdbfilePath))
                                File.Delete(pdbfilePath);
                        }

                        if (!directoryInfo.GetFiles().Any() && !directoryInfo.GetDirectories().Any() && !saveLocalesFolders)
                            directoryInfo.Delete(true);
                    }
                }
                catch
                {
                    //do nothing
                }
            }
        }

        private static void Main(string[] args)
        {
            var outputPath = string.Empty;
            var pluginPaths = string.Empty;
            var saveLocalesFolders = true;

            var settings = args.FirstOrDefault(a => a.Contains("|")) ?? string.Empty;
            if(string.IsNullOrEmpty(settings))
                return;

            foreach (var arg in settings.Split('|'))
            {
                var data = arg.Split("=").Select(p => p.Trim()).ToList();

                var name = data[0];
                var value = data.Count > 1 ? data[1] : string.Empty;

                switch (name)
                {
                    case "OutputPath":
                        outputPath = value;
                        break;
                    case "PluginPath":
                        pluginPaths = value;
                        break;
                    case "SaveLocalesFolders":
                        bool.TryParse(value, out saveLocalesFolders);
                        break;
                }
            }
            
            if(!Directory.Exists(outputPath))
                return;

            var di = new DirectoryInfo(outputPath);
            var separator = Path.DirectorySeparatorChar;
            var folderToIgnore = string.Concat(separator, "Plugins", separator);
            var fileNames = di.GetFiles("*.dll", SearchOption.AllDirectories)
                .Where(fi => !fi.FullName.Contains(folderToIgnore))
                .Select(fi => fi.Name.Replace(fi.Extension, "")).ToList();
           
            if (string.IsNullOrEmpty(pluginPaths) || !fileNames.Any())
            {
                return;
            }

            Clear(pluginPaths, fileNames, saveLocalesFolders);
        }
    }
}
