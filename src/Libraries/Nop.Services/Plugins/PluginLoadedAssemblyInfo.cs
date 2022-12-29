using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nop.Services.Plugins
{
    /// <summary>
    /// Represents an information about assembly which loaded by plugins
    /// </summary>
    public partial class PluginLoadedAssemblyInfo
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="shortName">Assembly short name</param>
        /// <param name="assemblyInMemory">Assembly</param>
        public PluginLoadedAssemblyInfo(string shortName, Assembly assemblyInMemory)
        {
            ShortName = shortName;
            References = new List<(string PluginName, string AssemblyName)>();
            AssemblyInMemory = assemblyInMemory;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Assembly in memory
        /// </summary>
        public Assembly AssemblyInMemory { get; }

        /// <summary>
        /// Gets the short assembly name
        /// </summary>
        public string ShortName { get; }

        /// <summary>
        /// Gets the full assembly name loaded in memory
        /// </summary>
        public string AssemblyFullNameInMemory {
            get
            {
                return AssemblyInMemory.FullName;
            }
        }

        /// <summary>
        /// Gets a list of all mentioned plugin-assembly pairs
        /// </summary>
        public List<(string PluginName, string AssemblyName)> References { get; }

        /// <summary>
        /// Gets a list of plugins that conflict with the loaded assembly version
        /// </summary>
        public IList<(string PluginName, string AssemblyName)> Collisions =>
            References.Where(reference => !reference.AssemblyName.Equals(AssemblyFullNameInMemory, StringComparison.CurrentCultureIgnoreCase)).ToList();

        #endregion
    }
}