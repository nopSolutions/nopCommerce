using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Nop.Core.Plugins
{
    public abstract class BasePlugin : IPlugin
    {
        protected BasePlugin()
        {
        }

        protected BasePlugin(string friendlyName, string systemName,
            string version = "", string author = "", int displayOrder = 0)
        {
            this.FriendlyName = friendlyName;
            this.SystemName = systemName;
            this.Version = version;
            this.Author = author;
            this.DisplayOrder = displayOrder;
        }

        #region IPlugin Members

        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        public virtual string FriendlyName { get; protected set; }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        public virtual string SystemName { get; protected set; }

        /// <summary>
        /// Gets or sets the version
        /// </summary>
        public virtual string Version { get; protected set; }

        /// <summary>
        /// Gets or sets the author
        /// </summary>
        public virtual string Author { get; protected set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }

        public virtual bool IsAuthorized(IPrincipal user)
        {
            return true;
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public virtual void Install() { }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public virtual void Uninstall() { }

        #endregion

        public int CompareTo(IPlugin other)
        {
            return DisplayOrder - other.DisplayOrder;
        }

        public override bool Equals(object obj)
        {
            var other = obj as IPlugin;
            return other != null && SystemName.Equals(other.SystemName);
        }

        public override int GetHashCode()
        {
            return SystemName.GetHashCode();
        }
    }
}
