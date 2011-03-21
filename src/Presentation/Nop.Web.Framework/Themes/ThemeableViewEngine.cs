using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Themes
{
    public class ThemableViewEngine : RazorViewEngine
    {
        public ThemableViewEngine()
        {
            ViewLocationFormats = Append(ViewLocationFormats, new[] {
                                                "~/Themes/{2}/Views/{1}/{0}.cshtml",
                                                "~/Themes/{2}/Views/{1}/{0}.vbhtml",
                                                "~/Themes/{2}/Views/Shared/{0}.cshtml",
                                                "~/Themes/{2}/Views/Shared/{0}.vbhtml",
                                            });

            MasterLocationFormats = Append(MasterLocationFormats, new[] {
                                                "~/Themes/{2}/Views/{1}/{0}.cshtml",
                                                "~/Themes/{2}/Views/{1}/{0}.vbhtml",
                                                "~/Themes/{2}/Views/Shared/{0}.cshtml",
                                                "~/Themes/{2}/Views/Shared/{0}.vbhtml",
                                            });

            PartialViewLocationFormats = Append(PartialViewLocationFormats, new[] {
                                                "~/Themes/{2}/Views/{1}/{0}.cshtml",
                                                "~/Themes/{2}/Views/{1}/{0}.vbhtml",
                                                "~/Themes/{2}/Views/Shared/{0}.cshtml",
                                                "~/Themes/{2}/Views/Shared/{0}.vbhtml",
                                            });

            // Search parts for the partial views
            // The search parts for the partial views are the same as the regular views
            base.PartialViewLocationFormats = base.ViewLocationFormats;
        }

        #region Helper Methods

        private string[] Append(string[] first, string[] second)
        {
            var list = new List<string>();
            list.AddRange(second);
            list.AddRange(first);
            return list.ToArray();
        }

        private static string GetTheme(ControllerContext controllerContext)
        {
            return EngineContext.Current.Resolve<IWorkContext>().WorkingTheme;
        }

        private string GetPath(ControllerContext controllerContext, string[] locations, string name,
                                string theme, string controller, string cacheKeyPrefix, bool useCache, out string[] searchedLocations)
        {
            searchedLocations = new string[] { };

            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            if ((locations == null) || (locations.Length == 0))
            {
                throw new InvalidOperationException("locations must not be null or emtpy.");
            }

            bool flag = IsSpecificPath(name);
            string key = this.CreateCacheKey(cacheKeyPrefix, name, flag ? string.Empty : controller, theme);
            if (useCache)
            {
                string viewLocation = this.ViewLocationCache.GetViewLocation(controllerContext.HttpContext, key);
                if (viewLocation != null)
                {
                    return viewLocation;
                }
            }
            if (!flag)
            {
                string path = this.GetPathFromGeneralName(controllerContext, locations, name, controller, theme, key, ref searchedLocations);
                if (String.IsNullOrEmpty(path))
                {
                    path = this.GetPathFromGeneralName(controllerContext, locations, name, controller, "Default", key, ref searchedLocations);
                }
                return path;
            }
            return this.GetPathFromSpecificName(controllerContext, name, key, ref searchedLocations);
        }

        private static bool IsSpecificPath(string name)
        {
            char firstCharacter = name[0];
            if (firstCharacter != '~')
            {
                return (firstCharacter == '/');
            }
            return true;
        }

        private string CreateCacheKey(string prefix, string name, string controllerName, string theme)
        {
            return string.Format(CultureInfo.InvariantCulture, ":ViewCacheEntry:{0}:{1}:{2}:{3}:{4}",
                new object[] { base.GetType().AssemblyQualifiedName, prefix, name, controllerName, theme });
        }

        private string GetPathFromGeneralName(ControllerContext controllerContext, string[] locations, string name,
                                               string controller, string theme, string cacheKey, ref string[] searchedLocations)
        {
            string virtualPath = string.Empty;
            searchedLocations = new string[locations.Length];
            for (int i = 0; i < locations.Length; i++)
            {
                string path = string.Format(CultureInfo.InvariantCulture, locations[i], new object[] { name, controller, theme });

                if (this.FileExists(controllerContext, path))
                {
                    searchedLocations = new string[] { };
                    virtualPath = path;
                    this.ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, virtualPath);
                    return virtualPath;
                }
                searchedLocations[i] = path;
            }
            return virtualPath;
        }

        private string GetPathFromSpecificName(ControllerContext controllerContext, string name, string cacheKey, ref string[] searchedLocations)
        {
            string virtualPath = name;

            if (!FileExists(controllerContext, name))
            {
                virtualPath = String.Empty;
                searchedLocations = new[] { name };
            }

            this.ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, virtualPath);
            return virtualPath;
        }
        #endregion

        #region Override Default Behavior
        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            try
            {
                return System.IO.File.Exists(controllerContext.HttpContext.Server.MapPath(virtualPath));
            }
            catch (HttpException exception)
            {
                if (exception.GetHttpCode() != 0x194)
                    throw;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (string.IsNullOrEmpty(partialViewName))
            {
                throw new ArgumentException("partialViewName");
            }

            string[] partialViewLocationsSearched;
            string theme = GetTheme(controllerContext);

            string controllerName = controllerContext.RouteData.GetRequiredString("controller");
            string partialViewPath = this.GetPath(controllerContext, this.PartialViewLocationFormats, partialViewName, theme, controllerName, "Partial", useCache, out partialViewLocationsSearched); ;

            if (string.IsNullOrEmpty(partialViewPath))
            {
                return new ViewEngineResult(partialViewLocationsSearched);
            }
            return new ViewEngineResult(this.CreatePartialView(controllerContext, partialViewPath), this);
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException("viewName");
            }

            string[] viewLocationsSearched;
            string[] masterLocationsSearched;

            string theme = GetTheme(controllerContext);

            string controllerName = controllerContext.RouteData.GetRequiredString("controller");
            string viewPath = this.GetPath(controllerContext, this.ViewLocationFormats, viewName,
                                       theme, controllerName, "View", useCache, out viewLocationsSearched);

            //if (String.IsNullOrEmpty(masterName))
            //{
            //    masterName = "Theme";
            //}

            if (controllerName == "Widget" && viewName == "View") //hackety hack
                masterName = "";

            string masterPath = GetPath(controllerContext, MasterLocationFormats, masterName, theme, controllerName, "Master", useCache, out masterLocationsSearched);

            if (!string.IsNullOrEmpty(viewPath) && (!string.IsNullOrEmpty(masterPath) || string.IsNullOrEmpty(masterName)))
            {
                return new ViewEngineResult(this.CreateView(controllerContext, viewPath, masterPath), this);
            }

            return new ViewEngineResult(viewLocationsSearched.Union(masterLocationsSearched));
        }
        #endregion
    }



    //public class ThemableRazorViewEngine : RazorViewEngine
    //{
    //    private static readonly string[] EmptyLocations = new string[0];

    //    public ThemableRazorViewEngine()
    //    {
    //        ViewLocationFormats = Append(ViewLocationFormats, new[] {
    //                                            "~/Themes/{2}/Views/{1}/{0}.cshtml",
    //                                            "~/Themes/{2}/Views/{1}/{0}.vbhtml",
    //                                            "~/Themes/{2}/Views/Shared/{0}.cshtml",
    //                                            "~/Themes/{2}/Views/Shared/{0}.vbhtml",
    //                                        });

    //        MasterLocationFormats = Append(MasterLocationFormats, new[] {
    //                                            "~/Themes/{2}/Views/{1}/{0}.cshtml",
    //                                            "~/Themes/{2}/Views/{1}/{0}.vbhtml",
    //                                            "~/Themes/{2}/Views/Shared/{0}.cshtml",
    //                                            "~/Themes/{2}/Views/Shared/{0}.vbhtml",
    //                                        });

    //        PartialViewLocationFormats = Append(PartialViewLocationFormats, new[] {
    //                                            "~/Themes/{2}/Views/{1}/{0}.cshtml",
    //                                            "~/Themes/{2}/Views/{1}/{0}.vbhtml",
    //                                            "~/Themes/{2}/Views/Shared/{0}.cshtml",
    //                                            "~/Themes/{2}/Views/Shared/{0}.vbhtml",
    //                                        });
    //    }

    //    private string[] Append(string[] first, string[] second)
    //    {
    //        var list = new List<string>();
    //        list.AddRange(first);
    //        list.AddRange(second);
    //        return list.ToArray();
    //    }

    //    public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
    //    {
    //        string[] strArray;
    //        string[] strArray2;
    //        if (controllerContext == null)
    //        {
    //            throw new ArgumentNullException("controllerContext");
    //        }
    //        if (string.IsNullOrEmpty(viewName))
    //        {
    //            throw new ArgumentException(MvcResources.Common_NullOrEmpty, "viewName");
    //        }
    //        string requiredString = controllerContext.RouteData.GetRequiredString("controller");
    //        string str2 = this.GetPath(controllerContext, this.ViewLocationFormats, this.AreaViewLocationFormats, "ViewLocationFormats", viewName, requiredString, "View", useCache, out strArray);
    //        string str3 = this.GetPath(controllerContext, this.MasterLocationFormats, this.AreaMasterLocationFormats, "MasterLocationFormats", masterName, requiredString, "Master", useCache, out strArray2);
    //        if (!string.IsNullOrEmpty(str2) && (!string.IsNullOrEmpty(str3) || string.IsNullOrEmpty(masterName)))
    //        {
    //            return new ViewEngineResult(this.CreateView(controllerContext, str2, str3), this);
    //        }
    //        return new ViewEngineResult(strArray.Union<string>(strArray2));

    //    }
       
    //}


    //public class ThemeableViewEngine : IViewEngine
    //{
    //    // format is ":ViewCacheEntry:{cacheType}:{theme}:{prefix}:{name}:{controllerName}:{areaName}:"
    //    private const string _cacheKeyFormat = ":ViewCacheEntry:{0}:{1}:{2}:{3}:{4}:{5}:";
    //    private const string _cacheKeyPrefix_Master = "Master";
    //    private const string _cacheKeyPrefix_Partial = "Partial";
    //    private const string _cacheKeyPrefix_View = "View";
    //    private static readonly string[] _emptyLocations = new string[0];

    //    private VirtualPathProvider _vpp;

    //    public ThemeableViewEngine()
    //    {
    //        if (HttpContext.Current == null || HttpContext.Current.IsDebuggingEnabled)
    //        {
    //            ViewLocationCache = DefaultViewLocationCache.Null;
    //        }
    //        else
    //        {
    //            ViewLocationCache = new DefaultViewLocationCache();
    //        }

    //        AreaViewLocationFormats = new[] {
    //                                            "~/Areas/{2}/Views/themes/{3}/{1}/{0}.cshtml",
    //                                            "~/Areas/{2}/Views/themes/{3}/{1}/{0}.vbhtml",
    //                                            "~/Areas/{2}/Views/themes/{3}/Shared/{0}.cshtml",
    //                                            "~/Areas/{2}/Views/themes/{3}/Shared/{0}.vbhtml",

    //                                            "~/Areas/{2}/Views/{1}/{0}.cshtml",
    //                                            "~/Areas/{2}/Views/{1}/{0}.vbhtml",
    //                                            "~/Areas/{2}/Views/Shared/{0}.cshtml",
    //                                            "~/Areas/{2}/Views/Shared/{0}.vbhtml"
    //                                        };

    //        AreaMasterLocationFormats = new[] {
    //                                              "~/Areas/{2}/Views/themes/{3}/{1}/{0}.cshtml",
    //                                              "~/Areas/{2}/Views/themes/{3}/{1}/{0}.vbhtml",
    //                                              "~/Areas/{2}/Views/themes/{3}/Shared/{0}.cshtml",
    //                                              "~/Areas/{2}/Views/themes/{3}/Shared/{0}.vbhtml",

    //                                              "~/Areas/{2}/Views/{1}/{0}.cshtml",
    //                                              "~/Areas/{2}/Views/{1}/{0}.vbhtml",
    //                                              "~/Areas/{2}/Views/Shared/{0}.cshtml",
    //                                              "~/Areas/{2}/Views/Shared/{0}.vbhtml"
    //                                          };

    //        AreaPartialViewLocationFormats = new[] {
    //                                                   "~/Areas/{2}/Views/themes/{3}/{1}/{0}.cshtml",
    //                                                   "~/Areas/{2}/Views/themes/{3}/{1}/{0}.vbhtml",
    //                                                   "~/Areas/{2}/Views/themes/{3}/Shared/{0}.cshtml",
    //                                                   "~/Areas/{2}/Views/themes/{3}/Shared/{0}.vbhtml",

    //                                                   "~/Areas/{2}/Views/{1}/{0}.cshtml",
    //                                                   "~/Areas/{2}/Views/{1}/{0}.vbhtml",
    //                                                   "~/Areas/{2}/Views/Shared/{0}.cshtml",
    //                                                   "~/Areas/{2}/Views/Shared/{0}.vbhtml"
    //                                               };

    //        // format is ":ViewCacheEntry:0{cacheType}:1{theme}:2{prefix}:3{name}:4{controllerName}:5{areaName}:"
    //        //private const string _cacheKeyFormat = ":ViewCacheEntry:{0}:{1}:{2}:{3}:{4}:{5}:";

    //        ViewLocationFormats = new[] {
    //                                        "~/Themes/{2}/Views/{1}/{0}.cshtml",
    //                                        "~/Themes/{2}/Views/{1}/{0}.vbhtml",
    //                                        "~/Themes/{2}/Views/Shared/{0}.cshtml",
    //                                        "~/Themes/{2}/Views/Shared/{0}.vbhtml",

    //                                        "~/Views/{1}/{0}.cshtml",
    //                                        "~/Views/{1}/{0}.vbhtml",
    //                                        "~/Views/Shared/{0}.cshtml",
    //                                        "~/Views/Shared/{0}.vbhtml"
    //                                    };

    //        MasterLocationFormats = new[] {
    //                                        "~/Themes/{2}/Views/{1}/{0}.cshtml",
    //                                        "~/Themes/{2}/Views/{1}/{0}.vbhtml",
    //                                         "~/Themes/{2}/Views/Shared/{0}.cshtml",
    //                                        "~/Themes/{2}/Views/Shared/{0}.vbhtml",

    //                                          "~/Views/{1}/{0}.cshtml",
    //                                          "~/Views/{1}/{0}.vbhtml",
    //                                          "~/Views/Shared/{0}.cshtml",
    //                                          "~/Views/Shared/{0}.vbhtml"
    //                                      };

    //        PartialViewLocationFormats = new[] {
    //                                        "~/Themes/{2}/Views/{1}/{0}.cshtml",
    //                                        "~/Themes/{2}/Views/{1}/{0}.vbhtml",
    //                                         "~/Themes/{2}/Views/Shared/{0}.cshtml",
    //                                        "~/Themes/{2}/Views/Shared/{0}.vbhtml",

    //                                               "~/Views/{1}/{0}.cshtml",
    //                                               "~/Views/{1}/{0}.vbhtml",
    //                                               "~/Views/Shared/{0}.cshtml",
    //                                               "~/Views/Shared/{0}.vbhtml"
    //                                           };

    //        ViewStartFileExtensions = new[] { "cshtml", "vbhtml", };
    //    }

    //    public Func<HttpContextBase, string> CurrentTheme { get; set; }

    //    public string[] ViewStartFileExtensions { get; set; }

    //    public string[] AreaMasterLocationFormats { get; set; }

    //    public string[] AreaPartialViewLocationFormats { get; set; }

    //    public string[] AreaViewLocationFormats { get; set; }

    //    public string[] MasterLocationFormats { get; set; }

    //    public string[] PartialViewLocationFormats { get; set; }

    //    public string[] ViewLocationFormats { get; set; }

    //    public IViewLocationCache ViewLocationCache { get; set; }

    //    protected VirtualPathProvider VirtualPathProvider
    //    {
    //        get
    //        {
    //            return _vpp ?? (_vpp = HostingEnvironment.VirtualPathProvider);
    //        }

    //        set
    //        {
    //            _vpp = value;
    //        }
    //    }

    //    public virtual ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
    //    {
    //        if (controllerContext == null)
    //        {
    //            throw new ArgumentNullException("controllerContext");
    //        }

    //        if (string.IsNullOrEmpty(viewName))
    //        {
    //            throw new ArgumentException("Value cannot be null or empty.", "viewName");
    //        }

    //        string[] viewLocationsSearched;
    //        string[] masterLocationsSearched;
    //        bool incompleteMatch = false;

    //        string controllerName = controllerContext.RouteData.GetRequiredString("controller");

    //        string viewPath = GetPath(controllerContext, ViewLocationFormats, AreaViewLocationFormats,
    //                                  "ViewLocationFormats", viewName, controllerName, _cacheKeyPrefix_View, useCache,
    //            /* checkPathValidity */ true, ref incompleteMatch, out viewLocationsSearched);

    //        string masterPath = GetPath(controllerContext, MasterLocationFormats, AreaMasterLocationFormats,
    //                                    "MasterLocationFormats", masterName, controllerName, _cacheKeyPrefix_Master,
    //                                    useCache, /* checkPathValidity */ false, ref incompleteMatch,
    //                                    out masterLocationsSearched);

    //        if (string.IsNullOrEmpty(viewPath) || (string.IsNullOrEmpty(masterPath) && !string.IsNullOrEmpty(masterName)))
    //        {
    //            return new ViewEngineResult(viewLocationsSearched.Union(masterLocationsSearched));
    //        }

    //        return new ViewEngineResult(CreateView(controllerContext, viewPath, masterPath), this);
    //    }

    //    public virtual ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
    //    {
    //        if (controllerContext == null)
    //        {
    //            throw new ArgumentNullException("controllerContext");
    //        }

    //        if (string.IsNullOrEmpty(partialViewName))
    //        {
    //            throw new ArgumentException("Value cannot be null or empty.", "partialViewName");
    //        }

    //        string[] searched;
    //        bool incompleteMatch = false;
    //        string controllerName = controllerContext.RouteData.GetRequiredString("controller");

    //        string partialPath = GetPath(controllerContext, PartialViewLocationFormats, AreaPartialViewLocationFormats,
    //                                     "PartialViewLocationFormats", partialViewName, controllerName,
    //                                     _cacheKeyPrefix_Partial, useCache, /* checkBaseType */ true,
    //                                     ref incompleteMatch, out searched);

    //        if (string.IsNullOrEmpty(partialPath))
    //        {
    //            return new ViewEngineResult(searched);
    //        }

    //        return new ViewEngineResult(CreatePartialView(controllerContext, partialPath), this);
    //    }

    //    public virtual void ReleaseView(ControllerContext controllerContext, IView view)
    //    {
    //        var disposable = view as IDisposable;

    //        if (disposable != null)
    //        {
    //            disposable.Dispose();
    //        }
    //    }

    //    protected virtual bool FileExists(ControllerContext controllerContext, string virtualPath)
    //    {
    //        return BuildManager.GetObjectFactory(virtualPath, false) != null;
    //    }

    //    protected virtual bool? IsValidPath(ControllerContext controllerContext, string virtualPath)
    //    {
    //        Type compiledType = BuildManager.GetCompiledType(virtualPath);

    //        return compiledType == null ? (bool?)null : IsValidCompiledType(controllerContext, virtualPath, compiledType);
    //    }

    //    protected bool IsValidCompiledType(ControllerContext controllerContext, string virtualPath, Type compiledType)
    //    {
    //        return typeof(WebViewPage).IsAssignableFrom(compiledType);
    //    }

    //    protected IView CreatePartialView(ControllerContext controllerContext, string partialPath)
    //    {
    //        return new RazorView(controllerContext, partialPath, null, false, ViewStartFileExtensions);
    //    }

    //    protected IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
    //    {
    //        return new RazorView(controllerContext, viewPath, masterPath, true, ViewStartFileExtensions);
    //    }

    //    private static List<ViewLocation> GetViewLocations(IEnumerable<string> viewLocationFormats, IEnumerable<string> areaViewLocationFormats)
    //    {
    //        var allLocations = new List<ViewLocation>();

    //        if (areaViewLocationFormats != null)
    //        {
    //            allLocations.AddRange(areaViewLocationFormats.Select(areaViewLocationFormat => new AreaAwareViewLocation(areaViewLocationFormat)));
    //        }

    //        if (viewLocationFormats != null)
    //        {
    //            allLocations.AddRange(viewLocationFormats.Select(viewLocationFormat => new ViewLocation(viewLocationFormat)));
    //        }

    //        return allLocations;
    //    }

    //    private static bool IsSpecificPath(string name)
    //    {
    //        char c = name[0];

    //        return (c == '~' || c == '/');
    //    }

    //    private static string GetAreaName(RouteData routeData)
    //    {
    //        object area;

    //        if (routeData.DataTokens.TryGetValue("area", out area))
    //        {
    //            return area as string;
    //        }

    //        return GetAreaName(routeData.Route);
    //    }

    //    private static string GetAreaName(RouteBase route)
    //    {
    //        IRouteWithArea routeWithArea = route as IRouteWithArea;

    //        if (routeWithArea != null)
    //        {
    //            return routeWithArea.Area;
    //        }

    //        Route castRoute = route as Route;

    //        if (castRoute != null && castRoute.DataTokens != null)
    //        {
    //            return castRoute.DataTokens["area"] as string;
    //        }

    //        return null;
    //    }

    //    private string CreateCacheKey(string theme, string prefix, string name, string controllerName, string areaName)
    //    {
    //        return string.Format(CultureInfo.InvariantCulture, _cacheKeyFormat, GetType().AssemblyQualifiedName, theme, prefix, name, controllerName, areaName);
    //    }

    //    private string GetPath(ControllerContext controllerContext, IEnumerable<string> locations, IEnumerable<string> areaLocations, string locationsPropertyName, string name, string controllerName, string cacheKeyPrefix, bool useCache, bool checkPathValidity, ref bool incompleteMatch, out string[] searchedLocations)
    //    {
    //        searchedLocations = _emptyLocations;

    //        if (string.IsNullOrEmpty(name))
    //        {
    //            return string.Empty;
    //        }

    //        string areaName = GetAreaName(controllerContext.RouteData);
    //        bool usingAreas = !string.IsNullOrEmpty(areaName);

    //        string theme = CurrentTheme(controllerContext.HttpContext);

    //        List<ViewLocation> viewLocations = GetViewLocations(locations, (usingAreas) ? areaLocations : null);

    //        if (viewLocations.Count == 0)
    //        {
    //            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The property '{0}' cannot be null or empty.", locationsPropertyName));
    //        }

    //        bool nameRepresentsPath = IsSpecificPath(name);

    //        string cacheKey = CreateCacheKey(theme, cacheKeyPrefix, name, (nameRepresentsPath) ? string.Empty : controllerName, areaName);

    //        if (useCache)
    //        {
    //            return ViewLocationCache.GetViewLocation(controllerContext.HttpContext, cacheKey);
    //        }

    //        return nameRepresentsPath ?
    //               GetPathFromSpecificName(controllerContext, name, cacheKey, checkPathValidity, ref searchedLocations, ref incompleteMatch) :
    //               GetPathFromGeneralName(controllerContext, viewLocations, name, controllerName, areaName, theme, cacheKey, ref searchedLocations);
    //    }

    //    private string GetPathFromGeneralName(ControllerContext controllerContext, IList<ViewLocation> locations, string name, string controllerName, string areaName, string theme, string cacheKey, ref string[] searchedLocations)
    //    {
    //        string result = string.Empty;
    //        searchedLocations = new string[locations.Count];

    //        for (int i = 0; i < locations.Count; i++)
    //        {
    //            ViewLocation location = locations[i];
    //            string virtualPath = location.Format(name, controllerName, areaName, theme);

    //            if (FileExists(controllerContext, virtualPath))
    //            {
    //                searchedLocations = _emptyLocations;
    //                result = virtualPath;
    //                ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, result);
    //                break;
    //            }

    //            searchedLocations[i] = virtualPath;
    //        }

    //        return result;
    //    }

    //    private string GetPathFromSpecificName(ControllerContext controllerContext, string name, string cacheKey, bool checkPathValidity, ref string[] searchedLocations, ref bool incompleteMatch)
    //    {
    //        string result = name;
    //        bool fileExists = FileExists(controllerContext, name);

    //        if (checkPathValidity && fileExists)
    //        {
    //            bool? validPath = IsValidPath(controllerContext, name);

    //            if (validPath == false)
    //            {
    //                fileExists = false;
    //            }
    //            else if (validPath == null)
    //            {
    //                incompleteMatch = true;
    //            }
    //        }

    //        if (!fileExists)
    //        {
    //            result = string.Empty;
    //            searchedLocations = new[] { name };
    //        }

    //        if (!incompleteMatch)
    //        {
    //            ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, result);
    //        }

    //        return result;
    //    }

    //    private class AreaAwareViewLocation : ViewLocation
    //    {
    //        public AreaAwareViewLocation(string virtualPathFormatString)
    //            : base(virtualPathFormatString)
    //        {
    //        }

    //        public override string Format(string viewName, string controllerName, string areaName, string theme)
    //        {
    //            return string.Format(CultureInfo.InvariantCulture, _virtualPathFormatString, viewName, controllerName, areaName, theme);
    //        }
    //    }

    //    private class ViewLocation
    //    {
    //        protected readonly string _virtualPathFormatString;

    //        public ViewLocation(string virtualPathFormatString)
    //        {
    //            _virtualPathFormatString = virtualPathFormatString;
    //        }

    //        public virtual string Format(string viewName, string controllerName, string areaName, string theme)
    //        {
    //            return string.Format(CultureInfo.InvariantCulture, _virtualPathFormatString, viewName, controllerName, theme);
    //        }
    //    }
    //}
}
