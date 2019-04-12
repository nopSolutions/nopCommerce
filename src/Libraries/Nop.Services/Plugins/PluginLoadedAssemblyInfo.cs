using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="assemblyInMemory">Assembly full name</param>
        public PluginLoadedAssemblyInfo(string shortName, string assemblyInMemory)
        {
            ShortName = shortName;
            References = new List<(string PluginName, string AssemblyName)>();
            AssemblyFullNameInMemory = assemblyInMemory;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the short assembly name
        /// </summary>
        public string ShortName { get; }

        /// <summary>
        /// Gets the full assembly name loaded in memory
        /// </summary>
        public string AssemblyFullNameInMemory { get; }

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