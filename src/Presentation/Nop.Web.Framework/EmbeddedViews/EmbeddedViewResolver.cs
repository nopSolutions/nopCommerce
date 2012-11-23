using System.Reflection;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.EmbeddedViews
{
    public class EmbeddedViewResolver : IEmbeddedViewResolver
    {
        ITypeFinder _typeFinder;
        public EmbeddedViewResolver(ITypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }

        public EmbeddedViewTable GetEmbeddedViews()
        {
            var assemblies = _typeFinder.GetAssemblies();
            if (assemblies == null || assemblies.Count == 0) return null;

            var table = new EmbeddedViewTable();

            foreach (var assembly in assemblies)
            {
                var names = GetNamesOfAssemblyResources(assembly);
                if (names == null || names.Length == 0) continue;

                foreach (var name in names)
                {
                    var key = name.ToLowerInvariant();
                    if (!key.Contains(".views.")) continue;

                    table.AddView(name, assembly.FullName);
                }
            }

            return table;
        }
        
        private static string[] GetNamesOfAssemblyResources(Assembly assembly)
        {
            //GetManifestResourceNames will throw a NotSupportedException when run on a dynamic assembly
            try
            {
                if (!assembly.IsDynamic)
                    return assembly.GetManifestResourceNames();
            }
            catch
            {
                // Any exception we fall back to returning an empty array.
            }
            
            return new string[] { };
        }
    }
}