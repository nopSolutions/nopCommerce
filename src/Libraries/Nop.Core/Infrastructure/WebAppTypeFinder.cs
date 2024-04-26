using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Nop.Core.Infrastructure;

/// <summary>
/// Provides information about types in the current web application. 
/// Optionally this class can look at all assemblies in the bin folder.
/// </summary>
public partial class WebAppTypeFinder : ITypeFinder
{
    #region Constants

    /// <summary>Gets the pattern for DLLs that we know don't need to be investigated.</summary>
    protected const string ASSEMBLY_SKIP_LOADING_PATTERN = "^System|^mscorlib|^Microsoft|^AjaxControlToolkit|^Antlr3|^Autofac|^AutoMapper|^Castle|^ComponentArt|^CppCodeProvider|^DotNetOpenAuth|^EntityFramework|^EPPlus|^FluentValidation|^ImageResizer|^itextsharp|^log4net|^MaxMind|^MbUnit|^MiniProfiler|^Mono.Math|^MvcContrib|^Newtonsoft|^NHibernate|^nunit|^Org.Mentalis|^PerlRegex|^QuickGraph|^Recaptcha|^Remotion|^RestSharp|^Rhino|^Telerik|^Iesi|^TestDriven|^TestFu|^UserAgentStringLibrary|^VJSharpCodeProvider|^WebActivator|^WebDev|^WebGrease";

    #endregion

    #region Fields

    protected static readonly Dictionary<string, Assembly> _assemblies = new(StringComparer.InvariantCultureIgnoreCase);
        
    protected static bool _loaded;
    protected static readonly object _locker = new();

    private readonly INopFileProvider _fileProvider;

    #endregion

    #region Ctor

    public WebAppTypeFinder()
    {
        _fileProvider = CommonHelper.DefaultFileProvider;
    }

    static WebAppTypeFinder()
    {
        AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
    }

    #endregion

    #region Utilities

    private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
    {
        var assembly = args.LoadedAssembly;

        if (assembly.FullName == null)
            return;

        if (_assemblies.ContainsKey(assembly.FullName))
            return;

        if (!Matches(assembly.FullName))
            return;

        _assemblies.TryAdd(assembly.FullName, assembly);
    }

    /// <summary>
    /// Check if a dll is one of the shipped dlls that we know don't need to be investigated.
    /// </summary>
    /// <param name="assemblyFullName">
    /// The name of the assembly to check.
    /// </param>
    /// <returns>
    /// True if the assembly should be loaded into Nop.
    /// </returns>
    protected static bool Matches(string assemblyFullName)
    {
        return !Regex.IsMatch(assemblyFullName, ASSEMBLY_SKIP_LOADING_PATTERN, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    /// <summary>
    /// Does type implement generic?
    /// </summary>
    /// <param name="type"></param>
    /// <param name="openGeneric"></param>
    /// <returns></returns>
    protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
    {
        try
        {
            var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
            return type.FindInterfaces((_, _) => true, null)
                .Where(implementedInterface => implementedInterface.IsGenericType).Any(implementedInterface =>
                    genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition()));
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Find classes of type
    /// </summary>
    /// <param name="assignTypeFrom">Assign type from</param>
    /// <param name="assemblies">Assemblies</param>
    /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
    /// <returns>Result</returns>
    protected virtual IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
    {
        var result = new List<Type>();

        try
        {
            foreach (var a in assemblies)
            {
                Type[] types = null;
                try
                {
                    types = a.GetTypes();
                }
                catch
                {
                    //ignore
                }

                if (types == null)
                    continue;

                foreach (var t in types)
                {
                    if (!assignTypeFrom.IsAssignableFrom(t) && (!assignTypeFrom.IsGenericTypeDefinition || !DoesTypeImplementOpenGeneric(t, assignTypeFrom)))
                        continue;

                    if (t.IsInterface)
                        continue;

                    if (onlyConcreteClasses)
                    {
                        if (t.IsClass && !t.IsAbstract)
                            result.Add(t);
                    }
                    else
                        result.Add(t);
                }
            }
        }
        catch (ReflectionTypeLoadException ex)
        {
            var msg = string.Empty;

            if (ex.LoaderExceptions.Any()) 
                msg = ex.LoaderExceptions.Where(e => e != null)
                    .Aggregate(msg, (current, e) => $"{current}{e.Message + Environment.NewLine}");

            var fail = new Exception(msg, ex);
            Debug.WriteLine(fail.Message, fail);

            throw fail;
        }

        return result;
    }

    protected virtual void InitData()
    {
        //data already loaded
        if (_loaded)
            return;

        //prevent multi loading data
        lock (_locker)
        {
            //data can be loaded while we waited
            if (_loaded)
                return;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName == null)
                    continue;

                if (!Matches(assembly.FullName))
                    continue;

                _assemblies.TryAdd(assembly.FullName, assembly);
            }

            foreach (var directoriesToLoadAssembly in DirectoriesToLoadAssemblies)
            {
                if (!_fileProvider.DirectoryExists(directoriesToLoadAssembly))
                    continue;

                foreach (var dllPath in _fileProvider.GetFiles(directoriesToLoadAssembly, "*.dll"))
                    try
                    {
                        var an = AssemblyName.GetAssemblyName(dllPath);

                        if (_assemblies.ContainsKey(an.FullName))
                            continue;

                        if (!Matches(an.FullName))
                            continue;

                        Assembly assembly;

                        try
                        {
                            assembly = AppDomain.CurrentDomain.Load(an);
                        }
                        catch
                        {
                            assembly = Assembly.LoadFrom(dllPath);
                        }

                        _assemblies.TryAdd(assembly.FullName, assembly);
                    }
                    catch (BadImageFormatException ex)
                    {
                        Trace.TraceError(ex.ToString());
                    }
            }

            _loaded = true;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the assemblies related to the current implementation.
    /// </summary>
    /// <returns>A list of assemblies</returns>
    public virtual IList<Assembly> GetAssemblies()
    {
        if (!_loaded)
            InitData();
            
        return _assemblies.Values.ToList();
    }

    /// <summary>
    /// Find classes of type
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
    /// <returns>Result</returns>
    public virtual IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
    {
        return FindClassesOfType(typeof(T), onlyConcreteClasses);
    }

    /// <summary>
    /// Find classes of type
    /// </summary>
    /// <param name="assignTypeFrom">Assign type from</param>
    /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
    /// <returns>Result</returns>
    /// <returns></returns>
    public virtual IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
    {
        return FindClassesOfType(assignTypeFrom, GetAssemblies(), onlyConcreteClasses);
    }

    /// <summary>
    /// Gets the assembly by it full name
    /// </summary>
    /// <returns>A list of assemblies</returns>
    public virtual Assembly GetAssemblyByName(string assemblyFullName)
    {
        if (!_loaded)
            InitData();

        _assemblies.TryGetValue(assemblyFullName, out var assembly);

        if (assembly != null)
            return assembly;

        var assemblyName = new AssemblyName(assemblyFullName);
        var key = _assemblies.Keys.FirstOrDefault(k => k.StartsWith(assemblyName.Name ?? assemblyFullName.Split(' ')[0], StringComparison.InvariantCultureIgnoreCase));

        return string.IsNullOrEmpty(key) ? null : _assemblies[key];
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the list of directories to load assemblies on initialize 
    /// </summary>
    /// <remarks>
    /// For example, the web application's bin folder should be specifically checked for being loaded on the application load. This is needed in situations where plugins need to be loaded in the AppDomain after the application has been reloaded
    /// </remarks>
    public virtual List<string> DirectoriesToLoadAssemblies { get; set; } = new ()
    {
        AppContext.BaseDirectory
    };

    #endregion
}