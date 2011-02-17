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
// Contributor(s): Umbraco (http://umbraco.org/)_______. 
//------------------------------------------------------------------------------
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

        static PluginManager()
        {
            //UNDONE HostingEnvironment doesn't support mocking (required for unit testing)
            PluginFolder = new DirectoryInfo(HostingEnvironment.MapPath(PluginsPath));
            ShadowCopyFolder = new DirectoryInfo(HostingEnvironment.MapPath(ShadowCopyPath));
        }

        /// <summary>
        /// The source plugin folder from which to shadow copy from
        /// </summary>
        /// <remarks>
        /// This folder can contain sub folderst to organize plugin types
        /// </remarks>
        private static readonly DirectoryInfo PluginFolder;

        /// <summary>
        /// The folder to shadow copy the plugin DLLs to use for running the app
        /// </summary>
        private static readonly DirectoryInfo ShadowCopyFolder;

        /// <summary>
        /// Returns a collection of all referenced plugin assemblies that have been shadow copied
        /// </summary>
        public static IEnumerable<Assembly> ReferencedPlugins { get; private set; }

        public static void Initialize()
        {
            lock (_locker)
            {
                var referencedAssemblies = new List<Assembly>();

                Directory.CreateDirectory(ShadowCopyFolder.FullName);

                //UNDONE how do we prevent assemblies from locking?
                //see here http://stackoverflow.com/questions/1646049/how-to-load-an-assembly-without-using-assembly-load
                
                //clear out plugins
                foreach (var f in ShadowCopyFolder.GetFiles("*.dll", SearchOption.AllDirectories))
                {
                    File.Delete(f.FullName);
                }

                //shadow copy files
                foreach (var plug in PluginFolder.GetFiles("*.dll", SearchOption.AllDirectories))
                {
                    var di = Directory.CreateDirectory(ShadowCopyFolder.FullName);
                    File.Copy(plug.FullName, Path.Combine(di.FullName, plug.Name), true);
                }

                foreach (var file in ShadowCopyFolder.GetFiles("*.dll", SearchOption.AllDirectories))
                {
                    var an = AssemblyName.GetAssemblyName(file.FullName);
                    var a = Assembly.Load(an.FullName);
                    BuildManager.AddReferencedAssembly(a);
                    referencedAssemblies.Add(a);
                }

                ReferencedPlugins = referencedAssemblies;
            }
        }
    }
}
