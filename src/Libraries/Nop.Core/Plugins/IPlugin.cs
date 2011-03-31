using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Interface denoting plug-in attributes that are displayed throughout 
    /// the editing interface.
    /// </summary>
    public interface IPlugin : IComparable<IPlugin>
    {
        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        string SystemName { get; }

        /// <summary>
        /// Gets or sets the version
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets or sets the author
        /// </summary>
        string Author { get; }
        
        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        int DisplayOrder { get; }

        bool IsAuthorized(IPrincipal user);
    }
}
