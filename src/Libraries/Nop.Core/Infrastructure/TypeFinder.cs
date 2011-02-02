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
        /// Searches all loaded assemblies for classes of the type passed in.
        /// </summary>
        /// <typeparam name="T">The type of object to search for</typeparam>
        /// <param name="onlyConcreteClasses">True to only return classes that can be constructed</param>
        /// <returns>A list of found types</returns>
        public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = GetAssignablesFromType<T>(loadedAssemblies, onlyConcreteClasses);
            return types;
        }

        /// <summary>
        /// Searches all assemblies specified for classes of the type passed in.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyFiles"></param>
        /// <param name="onlyConcreteClasses">Only resolve concrete classes that can be instantiated</param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<string> assemblyFiles, bool onlyConcreteClasses = true)
        {
            Contract.Requires<NullReferenceException>(assemblyFiles != null);

            var typeAndAssembly = GetAssignablesFromType<T>(assemblyFiles, onlyConcreteClasses);
            return GetTypesFromResult(typeAndAssembly);
        }

        /// <summary>
        /// Returns all types found of in the assemblies specified of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblies"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            Contract.Requires<NullReferenceException>(assemblies != null);

            return GetAssignablesFromType<T>(assemblies, onlyConcreteClasses);
        }

        /// <summary>
        /// Return all types found in the assembly specified of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType<T>(Assembly assembly, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType<T>(new[] { assembly }, onlyConcreteClasses);
        }

        #region Private methods

        private static IEnumerable<Type> GetTypesFromResult(Dictionary<string, string> result)
        {
            return (from type in result
                    let ass = Assembly.Load(type.Value)
                    where ass != null
                    select ass.GetType(type.Key, false)).ToList();
        }

        /// <summary>
        /// Gets a collection of assignables of type T from a collection of files
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="files">The files.</param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetAssignablesFromType<T>(IEnumerable<string> files, bool onlyConcreteClasses)
        {
            return GetTypes(typeof(T), files, onlyConcreteClasses);
        }

        /// <summary>
        /// Gets a collection of assignables of type T from a collection of assemblies
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblies"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
        private static IEnumerable<Type> GetAssignablesFromType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses)
        {
            return GetTypes(typeof(T), assemblies, onlyConcreteClasses);
        }

        private static IEnumerable<Type> GetTypes(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses)
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
        private static Dictionary<string, string> GetTypes(Type assignTypeFrom, IEnumerable<string> assemblyFiles, bool onlyConcreteClasses)
        {
            var result = new Dictionary<string, string>();
            foreach (var assembly in
                assemblyFiles.Where(File.Exists).Select(Assembly.LoadFile).Where(assembly => assembly != null))
            {
                try
                {
                    foreach (Type t in
                        assembly.GetTypes().Where(t => !t.IsInterface && assignTypeFrom.IsAssignableFrom(t) && (onlyConcreteClasses ? (t.IsClass && !t.IsAbstract) : true)))
                    {
                        //add the full type name and full assembly name                                  
                        result.Add(t.FullName, t.Assembly.FullName);
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Debug.WriteLine("Error reading assembly " + assembly.FullName + ": " + ex.Message);
                }
            }
            return result;
        }
        #endregion

    }
}
