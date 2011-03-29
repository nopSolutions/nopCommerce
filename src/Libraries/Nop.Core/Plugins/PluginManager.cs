
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Hosting;
using System.Web.Compilation;
using System.Reflection;
using Nop.Core.Plugins;

// SEE THIS POST for full details of what this does
//http://shazwazza.com/post/Developing-a-plugin-framework-in-ASPNET-with-medium-trust.aspx
//contributor is Umbraco (umbraco.org)

[assembly: PreApplicationStartMethod(typeof(PluginManager), "Initialize")]

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Plugin manager
    /// </summary>
    public class PluginManager
    {

        internal const string PluginsPath = "~/Plugins";
        internal const string ShadowCopyPath = "~/Plugins/bin";

        private static readonly object _locker = new object();
        private volatile static bool _isInit = false;

        /// <summary>
        /// Returns a collection of all referenced plugin assemblies that have been shadow copied
        /// </summary>
        public static IList<Assembly> ReferencedPlugins { get; protected set; }

        private static bool IsAssemblyAlreadyLoaded(AssemblyName assemblyName)
        {
            if (assemblyName == null)
                throw new ArgumentNullException("assemblyName");

            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                if (a.FullName == assemblyName.FullName)
                    return true;

            return false;
        }

        private static Assembly LoadAssembly(string assemblyPath, bool loadWithoutLocking = false)
        {
            Assembly assembly = null;
            if (loadWithoutLocking)
            {
                //dynamically load assembly (no file locking)
                byte[] assemStream = null;
                using (var fs = new FileStream(assemblyPath, FileMode.Open))
                {
                    assemStream = new byte[fs.Length];
                    fs.Read(assemStream, 0, (int)fs.Length);
                }

                var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
                if (!IsAssemblyAlreadyLoaded(assemblyName))
                    assembly = Assembly.Load(assemStream);
            }
            else
            {
                var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
                if (!IsAssemblyAlreadyLoaded(assemblyName))
                    assembly = Assembly.Load(assemblyName.FullName);
            }

            if (assembly != null)
            {
                BuildManager.AddReferencedAssembly(assembly);
                ReferencedPlugins.Add(assembly);
            }
            return assembly;
        }

        public static void Initialize()
        {
            if (!_isInit)
            {
                lock (_locker)
                {
                    //double check
                    if (!_isInit)
                    {
                        ReferencedPlugins = new List<Assembly>();

                        var pluginFolder = new DirectoryInfo(HostingEnvironment.MapPath(PluginsPath));
                        var shadowCopyFolder = new DirectoryInfo(HostingEnvironment.MapPath(ShadowCopyPath));

                        if (!shadowCopyFolder.Exists)
                            Directory.CreateDirectory(shadowCopyFolder.FullName);
                        //get list of all DLLs in bin
                        var binFiles = shadowCopyFolder.GetFiles("*.dll", SearchOption.AllDirectories);
                        //get list of all DLLs in plugins (not in bin!)
                        var pluginFiles = pluginFolder.GetFiles("*.dll", SearchOption.AllDirectories)
                            .Where(x => !binFiles.Select(q => q.FullName).Contains(x.FullName));


#if RELEASE
                        //Visual studio maintains a lock on shadow copied files sometimes which is really annoying while developing
                        //so just to be slightly less annoying while developing, we won't clear them out and simply overwrite them below
                        //if they've chagned.

                        //clear out shadow copied plugins
                        foreach (var f in binFiles)
                        {                            
                            if (shouldDelete)
                            {
                                File.Delete(f.FullName);    
                            }
                        }
#endif

                        //shadow copy files
                        foreach (var plug in pluginFiles)
                        {
                            //var shadowCopyPlugFolder = Directory.CreateDirectory(Path.Combine(shadowCopyFolder.FullName, plug.Directory.Name));
                            var shadowCopyPlugFolder = new DirectoryInfo(shadowCopyFolder.FullName);
                            var shouldCopy = true;
                            var shadowCopyPlugFileName = Path.Combine(shadowCopyPlugFolder.FullName, plug.Name);

#if DEBUG
                            //check if a shadow copied file already exists and if it does, check if its updated, if not don't copy
                            if (File.Exists(shadowCopyPlugFileName))
                            {
                                if (File.GetCreationTimeUtc(shadowCopyPlugFileName).Ticks == plug.CreationTimeUtc.Ticks)
                                {
                                    shouldCopy = false;
                                }
                            }
#endif

                            if (shouldCopy)
                            {
                                File.Copy(plug.FullName, Path.Combine(shadowCopyPlugFolder.FullName, plug.Name), true);
                            }
                        }

                        foreach (var file in shadowCopyFolder
                            .GetFiles("*.dll", SearchOption.AllDirectories))
                        {
                            //load assembly without locking
                            //replace "false" parameter with "true" to load an assembly without locking
                            var a = LoadAssembly(file.FullName, true);
                        }

                        _isInit = true;
                    }
                }
            }
        }
    }
}