using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Classes implementing this interface provide information about types 
    /// to various services in the N2 engine.
    /// </summary>
    public interface ITypeFinder
    {
        /// <summary>Finds types assignable from of a certain type in the app domain.</summary>
        /// <param name="requestedType">The type to find.</param>
        /// <returns>A list of types found in the app domain.</returns>
        IList<Type> Find(Type requestedType);

        /// <summary>Gets tne assemblies related to the current implementation.</summary>
        /// <returns>A list of assemblies that should be loaded by the N2 factory.</returns>
        IList<Assembly> GetAssemblies();
    }
}
