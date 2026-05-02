using System.Reflection;

namespace Nop.Core.Infrastructure;

/// <summary>
/// Classes implementing this interface provide information about types 
/// to various services in the Nop engine.
/// </summary>
public partial interface ITypeFinder
{
    /// <summary>
    /// Find classes of type
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
    /// <returns>Result</returns>
    IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);

    /// <summary>
    /// Find classes of type
    /// </summary>
    /// <param name="assignTypeFrom">Assign type from</param>
    /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
    /// <returns>Result</returns>
    /// <returns></returns>
    IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);

    /// <summary>
    /// Gets the assemblies related to the current implementation.
    /// </summary>
    /// <returns>A list of assemblies</returns>
    IList<Assembly> GetAssemblies();

    /// <summary>
    /// Gets the assembly by it full name
    /// </summary>
    /// <returns>A list of assemblies</returns>
    Assembly GetAssemblyByName(string assemblyFullName);
}