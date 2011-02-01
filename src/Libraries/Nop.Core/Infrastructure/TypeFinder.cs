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
using System.Text;
using System.Reflection;
using System.Web;
using System.IO;
using System.Security;
using System.Diagnostics;
using System.Security.Permissions;
using System.Diagnostics.Contracts;
using System.Linq;


namespace Nop.Core.Infrastructure
{
    public class TypeFinder
    {
        /// <summary>
        /// Internal class to resolve types
        /// </summary>
        private readonly TypeResolver _typeResolver;

        public TypeFinder()
        {
            _typeResolver = new TypeResolver();
        }
        
        /// <summary>
        /// Searches all assemblies specified for classes of the type passed in.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyFiles"></param>
        /// <param name="onlyConcreteClasses">Only resolve concrete classes that can be instantiated</param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType<T>(string[] assemblyFiles, bool onlyConcreteClasses = true)
        {
            Contract.Requires<NullReferenceException>(assemblyFiles != null);

            var typeAndAssembly = _typeResolver.GetAssignablesFromType<T>(assemblyFiles, onlyConcreteClasses);
            return GetTypesFromResult(typeAndAssembly);
        }

        /// <summary>
        /// Searches all loaded assemblies for classes of the type passed in.
        /// </summary>
        /// <typeparam name="T">The type of object to search for</typeparam>
        /// <param name="onlyConcreteClasses">True to only return classes that can be constructed</param>
        /// <returns>A list of found types</returns>
        public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            //UNDONE doesn't work when implementing class resides in the same assembly
            //because some executing assemblies are loaded into temp ASP.NET directories (distinct assembly fully qualified name)
            //var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            //var uri = new UriBuilder(codeBase);
            //var path = Uri.UnescapeDataString(uri.Path);
            //var binFolder = Path.GetDirectoryName(path);
            
            //var typeAndAssembly = _typeResolver.GetAssignablesFromType<T>(binFolder, "*.dll", onlyConcreteClasses);
            //return GetTypesFromResult(typeAndAssembly);


            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = _typeResolver.GetAssignablesFromType<T>(loadedAssemblies, onlyConcreteClasses);
            return types;
        }

        private static IEnumerable<Type> GetTypesFromResult(Dictionary<string, string> result)
        {
            return (from type in result
                    let ass = Assembly.Load(type.Value)
                    where ass != null
                    select ass.GetType(type.Key, false)).ToList();
        }
        
        #region Internal Type Resolver Class

        [Serializable]
        private class TypeResolver : MarshalByRefObject
        {
            /// <summary>
            /// Gets a collection of assignables of type T from a collection of a specific file type from a specified path.
            /// </summary>
            /// <typeparam name="T">The Type</typeparam>
            /// <param name="path">The path.</param>
            /// <param name="filePattern">The file pattern.</param>
            /// <param name="onlyConcreteClasses"></param>
            /// <returns></returns>
            internal Dictionary<string, string> GetAssignablesFromType<T>(string path, string filePattern, bool onlyConcreteClasses)
            {
                var fis = Array.ConvertAll<string, FileInfo>(
                    Directory.GetFiles(path, filePattern),
                    s => new FileInfo(s));
                var absoluteFiles = Array.ConvertAll<FileInfo, string>(
                    fis, fi => fi.FullName);
                return GetAssignablesFromType<T>(absoluteFiles, onlyConcreteClasses);
            }

            /// <summary>
            /// Gets a collection of assignables of type T from a collection of files
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="files">The files.</param>
            /// <param name="onlyConcreteClasses"></param>
            /// <returns></returns>
            internal Dictionary<string, string> GetAssignablesFromType<T>(IEnumerable<string> files, bool onlyConcreteClasses)
            {
                var domain = GetAppDomain();

                var typeResolver = (TypeResolver)domain.CreateInstanceAndUnwrap(
                            typeof(TypeResolver).Assembly.GetName().Name,
                            typeof(TypeResolver).FullName);
                return typeResolver.GetTypes(typeof(T), files, onlyConcreteClasses);
            }

            /// <summary>
            /// Gets a collection of assignables of type T from a collection of assemblies
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="assemblies"></param>
            /// <param name="onlyConcreteClasses"></param>
            /// <returns></returns>
            internal IEnumerable<Type> GetAssignablesFromType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses)
            {
                var domain = GetAppDomain();

                var typeResolver = (TypeResolver)domain.CreateInstanceAndUnwrap(
                       typeof(TypeResolver).Assembly.GetName().Name,
                       typeof(TypeResolver).FullName);

                return typeResolver.GetTypes(typeof(T), assemblies, onlyConcreteClasses);
            }

            /// <summary>
            /// Return the app domain to process the search
            /// </summary>
            /// <returns></returns>
            private AppDomain GetAppDomain()
            {
                //TODO: Is this the best way to be doing this? if it's in medium trust, there might be a whole lot of memory eaten by type searching without being able to unload an the app domain.
                AppDomain domain = AppDomain.CurrentDomain;
                return domain;
            }

            private IEnumerable<Type> GetTypes(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses)
            {
                return (from a in assemblies
                        from t in a.GetTypes()
                        where !t.IsInterface && assignTypeFrom.IsAssignableFrom(t) && (onlyConcreteClasses ? (t.IsClass && !t.IsAbstract) : true)
                        select t).ToList();
            }

            /// <summary>
            /// Returns of a collection of type names from a collection of assembky files.
            /// </summary>
            /// <param name="assignTypeFrom">The assign type.</param>
            /// <param name="assemblyFiles">The assembly files.</param>
            /// <param name="onlyConcreteClasses"></param>
            /// <returns></returns>
            private Dictionary<string, string> GetTypes(Type assignTypeFrom, IEnumerable<string> assemblyFiles, bool onlyConcreteClasses)
            {
                var result = new Dictionary<string, string>();
                var assemblies = assemblyFiles.Where(File.Exists).Select(Assembly.LoadFile).Where(assembly => assembly != null);
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        foreach (Type t in assembly.GetTypes())
                        {
                            if (!t.IsInterface && assignTypeFrom.IsAssignableFrom(t) && (onlyConcreteClasses ? (t.IsClass && !t.IsAbstract) : true))
                            {
                                //add the full type name and full assembly name                                  
                                result.Add(t.FullName, t.Assembly.FullName);
                            }
                        }
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        Debug.WriteLine("Error reading assembly " + assembly.FullName + ": " + ex.Message);
                    }
                }
                return result;
            }
        }

        #endregion

    }
}
