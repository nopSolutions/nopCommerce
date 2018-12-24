using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Represents an information about assembly which loaded by plugins
    /// </summary>
    public class PluginLoadedAssemblyInfo
    {
        public PluginLoadedAssemblyInfo(string shortName, string assemblyInMemory)
        {
            this.ShortName = shortName;
            this.OtherReferences = new List<KeyValuePair<string, string>>();
            this.AssemblyFullNameInMemory = assemblyInMemory;
        }

        /// <summary>
        /// The short assembly name
        /// </summary>
        public string ShortName { get; }

        /// <summary>
        /// The full assembly name
        /// </summary>
        public string AssemblyFullNameInMemory { get; }

        /// <summary>
        /// Other assembly versions needed for plugins
        /// </summary>
        public List<KeyValuePair<string, string>> OtherReferences { get; }

        /// <summary>
        /// Returns a list of plugins that conflict with the loaded assembly version.
        /// </summary>
        public IList<KeyValuePair<string, string>> GetCollision
        {
            get
            {
                return OtherReferences.Where(assemblyFullName =>
                    !assemblyFullName.Value.Equals(AssemblyFullNameInMemory,
                        StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
        }

        /// <summary>
        /// Checks if a collision exists
        /// </summary>
        public bool IsCollisionExists => GetCollision.Any();
    }
}