using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// A class that finds types needed by Nop by looping assemblies in the 
    /// currently executing AppDomain. Only assemblies whose names matches
    /// certain patterns are investigated and an optional list of assemblies
    /// referenced by <see cref="AssemblyNames"/> are always investigated.
    /// </summary>
    public class AppDomainTypeFinder : ITypeFinder
    {
        #region Private Fields

        private bool loadAppDomainAssemblies = true;

        private string assemblySkipLoadingPattern = "^System|^mscorlib|^Microsoft|^CppCodeProvider|^VJSharpCodeProvider|^WebDev|^Castle|^Iesi|^log4net|^NHibernate|^nunit|^TestDriven|^MbUnit|^Rhino|^QuickGraph|^TestFu|^Telerik|^ComponentArt|^MvcContrib|^AjaxControlToolkit|^Antlr3|^Remotion|^Recaptcha";

        private string assemblyRestrictToLoadingPattern = ".*";
        private IList<string> assemblyNames = new List<string>();

        #endregion

        #region Constructors

        /// <summary>Creates a new instance of the AppDomainTypeFinder.</summary>
        public AppDomainTypeFinder()
        {
        }

        #endregion

        #region Properties

        /// <summary>The app domain to look for types in.</summary>
        public virtual AppDomain App
        {
            get { return AppDomain.CurrentDomain; }
        }

        /// <summary>Gets or sets wether Nop should iterate assemblies in the app domain when loading Nop types. Loading patterns are applied when loading these assemblies.</summary>
        public bool LoadAppDomainAssemblies
        {
            get { return loadAppDomainAssemblies; }
            set { loadAppDomainAssemblies = value; }
        }

        /// <summary>Gets or sets assemblies loaded a startup in addition to those loaded in the AppDomain.</summary>
        public IList<string> AssemblyNames
        {
            get { return assemblyNames; }
            set { assemblyNames = value; }
        }

        /// <summary>Gets the pattern for dlls that we know don't need to be investigated.</summary>
        public string AssemblySkipLoadingPattern
        {
            get { return assemblySkipLoadingPattern; }
            set { assemblySkipLoadingPattern = value; }
        }

        /// <summary>Gets or sets the pattern for dll that will be investigated. For ease of use this defaults to match all but to increase performance you might want to configure a pattern that includes assemblies and your own.</summary>
        /// <remarks>If you change this so that Nop assemblies arn't investigated (e.g. by not including something like "^Nop|..." you may break core functionality.</remarks>
        public string AssemblyRestrictToLoadingPattern
        {
            get { return assemblyRestrictToLoadingPattern; }
            set { assemblyRestrictToLoadingPattern = value; }
        }

        #endregion

        #region Internal Attributed Assembly class

        private class AttributedAssembly
        {
            internal Assembly Assembly { get; set; }
            internal Type PluginAttributeType { get; set; }
        }

        #endregion

        #region ITypeFinder

        public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(assignTypeFrom, GetAssemblies(), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof (T), assemblies, onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            var result = new List<Type>();
            try
            {
                foreach (var a in assemblies)
                {
                    foreach (var t in a.GetTypes())
                    {
                        if (assignTypeFrom.IsAssignableFrom(t) || (assignTypeFrom.IsGenericTypeDefinition && DoesTypeImplementOpenGeneric(t, assignTypeFrom)))
                        {
                            if (!t.IsInterface)
                            {
                                if (onlyConcreteClasses)
                                {
                                    if (t.IsClass && !t.IsAbstract)
                                    {
                                        result.Add(t);
                                    }
                                }
                                else
                                {
                                    result.Add(t);
                                }
                            }
                        }
                    }

                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                var msg = string.Empty;
                foreach (var e in ex.LoaderExceptions)
                    msg += e.Message + Environment.NewLine;

                var fail = new Exception(msg, ex);
                Debug.WriteLine(fail.Message, fail);

                throw fail;
            }
            return result;
        }

        public IEnumerable<Type> FindClassesOfType<T, TAssemblyAttribute>(bool onlyConcreteClasses = true) where TAssemblyAttribute : Attribute
        {
            var found = FindAssembliesWithAttribute<TAssemblyAttribute>();
            return FindClassesOfType<T>(found, onlyConcreteClasses);
        }

        public IEnumerable<Assembly> FindAssembliesWithAttribute<T>()
        {
            return FindAssembliesWithAttribute<T>(GetAssemblies());
        }

        /// <summary>
        /// Caches attributed assembly information so they don't have to be re-read
        /// </summary>
        private readonly List<AttributedAssembly> _attributedAssemblies = new List<AttributedAssembly>();

        /// <summary>
        /// Caches the assembly attributes that have been searched for
        /// </summary>
        private readonly List<Type> _assemblyAttributesSearched = new List<Type>();

        public IEnumerable<Assembly> FindAssembliesWithAttribute<T>(IEnumerable<Assembly> assemblies)
        {
            //check if we've already searched this assembly);)
            if (!_assemblyAttributesSearched.Contains(typeof(T)))
            {
                var foundAssemblies = (from assembly in assemblies
                                      let customAttributes = assembly.GetCustomAttributes(typeof(T), false)
                                      where customAttributes.Any()
                                      select assembly).ToList();
                //now update the cache
                _assemblyAttributesSearched.Add(typeof(T));
                foreach (var a in foundAssemblies)
                {
                    _attributedAssemblies.Add(new AttributedAssembly { Assembly = a, PluginAttributeType = typeof(T) });
                }
            }

            //We must do a ToList() here because it is required to be serializable when using other app domains.
            return _attributedAssemblies
                .Where(x => x.PluginAttributeType.Equals(typeof(T)))
                .Select(x => x.Assembly)
                .ToList();
        }

        public IEnumerable<Assembly> FindAssembliesWithAttribute<T>(DirectoryInfo assemblyPath)
        {
            var assemblies = (from f in Directory.GetFiles(assemblyPath.FullName, "*.dll")
                              select Assembly.LoadFrom(f)
                                  into assembly
                                  let customAttributes = assembly.GetCustomAttributes(typeof(T), false)
                                  where customAttributes.Any()
                                  select assembly).ToList();
            return FindAssembliesWithAttribute<T>(assemblies);
        }

        /// <summary>Gets tne assemblies related to the current implementation.</summary>
        /// <returns>A list of assemblies that should be loaded by the Nop factory.</returns>
        public virtual IList<Assembly> GetAssemblies()
        {
            var addedAssemblyNames = new List<string>();
            var assemblies = new List<Assembly>();

            if (LoadAppDomainAssemblies)
                AddAssembliesInAppDomain(addedAssemblyNames, assemblies);
            AddConfiguredAssemblies(addedAssemblyNames, assemblies);

            return assemblies;
        }

        #endregion

        /// <summary>Iterates all assemblies in the AppDomain and if it's name matches the configured patterns add it to our list.</summary>
        /// <param name="addedAssemblyNames"></param>
        /// <param name="assemblies"></param>
        private void AddAssembliesInAppDomain(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (Matches(assembly.FullName))
                {
                    if (!addedAssemblyNames.Contains(assembly.FullName))
                    {
                        assemblies.Add(assembly);
                        addedAssemblyNames.Add(assembly.FullName);
                    }
                }
            }
        }

        /// <summary>Adds specificly configured assemblies.</summary>
        protected virtual void AddConfiguredAssemblies(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (string assemblyName in AssemblyNames)
            {
                Assembly assembly = Assembly.Load(assemblyName);
                if (!addedAssemblyNames.Contains(assembly.FullName))
                {
                    assemblies.Add(assembly);
                    addedAssemblyNames.Add(assembly.FullName);
                }
            }
        }

        /// <summary>Check if a dll is one of the shipped dlls that we know don't need to be investigated.</summary>
        /// <param name="assemblyFullName">The name of the assembly to check.</param>
        /// <returns>True if the assembly should be loaded into Nop.</returns>
        public virtual bool Matches(string assemblyFullName)
        {
            return !Matches(assemblyFullName, AssemblySkipLoadingPattern)
                   && Matches(assemblyFullName, AssemblyRestrictToLoadingPattern);
        }

        /// <summary>Check if a dll is one of the shipped dlls that we know don't need to be investigated.</summary>
        /// <param name="assemblyFullName">The assembly name to match.</param>
        /// <param name="pattern">The regular expression pattern to match against the assembly name.</param>
        /// <returns>True if the pattern matches the assembly name.</returns>
        protected virtual bool Matches(string assemblyFullName, string pattern)
        {
            return Regex.IsMatch(assemblyFullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>Makes sure matching assemblies in the supplied folder are loaded in the app domain.</summary>
        /// <param name="directoryPath">The physical path to a directory containing dlls to load in the app domain.</param>
        protected virtual void LoadMatchingAssemblies(string directoryPath)
        {
            List<string> loadedAssemblyNames = new List<string>();
            foreach (Assembly a in GetAssemblies())
            {
                loadedAssemblyNames.Add(a.FullName);
            }

            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            foreach (string dllPath in Directory.GetFiles(directoryPath, "*.dll"))
            {
                try
                {
                    var an = AssemblyName.GetAssemblyName(dllPath);
                    if (Matches(an.FullName) && !loadedAssemblyNames.Contains(an.FullName))
                    {
                        App.Load(an);
                    }
                    
                    //old loading stuff
                    //Assembly a = Assembly.ReflectionOnlyLoadFrom(dllPath);
                    //if (Matches(a.FullName) && !loadedAssemblyNames.Contains(a.FullName))
                    //{
                    //    App.Load(a.FullName);
                    //}
                }
                catch (BadImageFormatException ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

        protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                foreach (var implementedInterface in type.FindInterfaces((objType, objCriteria) => true, null))
                {
                    if (!implementedInterface.IsGenericType)
                        continue;

                    var isMatch = genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition());
                    return isMatch;
                }
                return false;
            }catch
            {
                return false;
            }
        }
        
    }
}
