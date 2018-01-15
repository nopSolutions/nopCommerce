using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClearPluginAssemblies
{
    public class Program
    {
        protected const string FILES_TO_DELETE = "dotnet-bundle.exe;Nop.Web.pdb;Nop.Web.exe;Nop.Web.exe.config";

        protected static void Clear(string paths, string fileNames, bool saveLocalesFolders)
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
                        foreach (var fileName in fileNames.Split(';'))
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
            var paths = string.Empty;
            var fileNames = string.Empty;
            var basePluginPath = string.Empty;
            var saveLocalesFolders = true;

            foreach (var arg in args)
            {
                var data = arg.Split("=").Select(p => p.Trim()).ToList();

                var name = data[0];
                var value = data.Count > 1 ? data[1] : string.Empty;

                switch (name)
                {
                    case "Paths":
                        paths = value;
                        break;
                    case "FileNames":
                        fileNames = value;
                        break;
                    case "PluginPath":
                        basePluginPath = value;
                        break;
                    case "SaveLocalesFolders":
                        bool.TryParse(value, out saveLocalesFolders);
                        break;
                }
            }
            
            if (string.IsNullOrEmpty(paths) || string.IsNullOrEmpty(fileNames))
            {
                return;
            }
            
            if (!string.IsNullOrEmpty(basePluginPath))
            {
                paths = basePluginPath;
            }

            Clear(paths, fileNames, saveLocalesFolders);
        }
    }
}
