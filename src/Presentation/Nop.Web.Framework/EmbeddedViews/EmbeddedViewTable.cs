using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Web.Framework.EmbeddedViews
{
    [Serializable]
    public class EmbeddedViewTable
    {
        private static readonly object _lock = new object();
        private readonly Dictionary<string, EmbeddedViewMetadata> _viewCache;

        public EmbeddedViewTable()
        {
            _viewCache = new Dictionary<string, EmbeddedViewMetadata>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void AddView(string viewName, string assemblyName)
        {
            lock (_lock)
            {
                _viewCache[viewName] = new EmbeddedViewMetadata { Name = viewName, AssemblyFullName = assemblyName };
            }
        }

        public IList<EmbeddedViewMetadata> Views
        {
            get
            {
                return _viewCache.Values.ToList();
            }
        }

        public bool ContainsEmbeddedView(string fullyQualifiedViewName)
        {
            var foundView = FindEmbeddedView(fullyQualifiedViewName);
            return (foundView != null);
        }

        public EmbeddedViewMetadata FindEmbeddedView(string fullyQualifiedViewName)
        {
            //var name = GetNameFromPath(viewPath);
            var name = fullyQualifiedViewName;
            if (string.IsNullOrEmpty(name)) return null;

            return Views
                //.SingleOrDefault(view => view.Name.ToLowerInvariant().Contains(name.ToLowerInvariant()))
                .SingleOrDefault(view => view.Name.ToLowerInvariant().Equals(name.ToLowerInvariant()));
        }

        protected string GetNameFromPath(string viewPath)
        {
            if (string.IsNullOrEmpty(viewPath)) return null;
            var name = viewPath.Replace("/", ".");
            return name.Replace("~", "");
        }
    }
}