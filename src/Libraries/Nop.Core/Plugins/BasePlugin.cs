using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Nop.Core.Plugins
{
    public abstract class BasePlugin : IPlugin
    {
        protected BasePlugin(string name, int sortOrder = 0)
        {
            Name = name;
            SortOrder = sortOrder;
        }

        #region IPlugin Members

        public string Name { get; set; }
        public int SortOrder { get; set; }

        public bool IsAuthorized(IPrincipal user)
        {
            return true;
        }

        #endregion

        #region IComparable<IPlugin> Members

        public int CompareTo(IPlugin other)
        {
            return SortOrder - other.SortOrder;
        }

        #endregion

        public override bool Equals(object obj)
        {
            var other = obj as IPlugin;
            return other != null && Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
