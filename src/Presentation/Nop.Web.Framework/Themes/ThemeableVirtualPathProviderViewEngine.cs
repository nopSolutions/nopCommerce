using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Core.Infrastructure;
using Nop.Services.Common;

namespace Nop.Web.Framework.Themes
{
    public abstract class ThemeableVirtualPathProviderViewEngine : VirtualPathProviderViewEngine
    {
        #region Fields

        internal Func<string, string> GetExtensionThunk;

        private readonly string[] _emptyLocations = null;
        private readonly string _mobileViewModifier = "Mobile";

        #endregion

        #region Ctor

        protected ThemeableVirtualPathProviderViewEngine()
        {
            GetExtensionThunk = new Func<string, string>(VirtualPathUtility.GetExtension);
        }

        #endregion

        #region Utilities

        protected virtual string GetPath(ControllerContext controllerContext, string[] locations, string[] areaLocations, string locationsPropertyName, string name, string controllerName, string theme, string cacheKeyPrefix, bool useCache, bool mobile, out string[] searchedLocations)
        {
            searchedLocations = _emptyLocations;
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            string areaName = GetAreaName(controllerContext.RouteData);

            //little hack to get nop's admin area to be in /Administration/ instead of /Nop/Admin/ or Areas/Admin/
            if (!string.IsNullOrEmpty(areaName) && areaName.Equals("admin", StringComparison.InvariantCultureIgnoreCase))
            {
                //admin area does not support mobile devices
                if (mobile)
                {
                    searchedLocations = new string[0];
                    return string.Empty;
                }
                var newLocations = areaLocations.ToList();
                newLocations.Insert(0, "~/Administration/Views/{1}/{0}.cshtml");
                newLocations.Insert(0, "~/Administration/Views/{1}/{0}.vbhtml");
                newLocations.Insert(0, "~/Administration/Views/Shared/{0}.cshtml");
                newLocations.Insert(0, "~/Administration/Views/Shared/{0}.vbhtml");
                areaLocations = newLocations.ToArray();
            }

            bool flag = !string.IsNullOrEmpty(areaName);
            List<ViewLocation> viewLocations = GetViewLocations(locations, flag ? areaLocations : null);
            if (viewLocations.Count == 0)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Properties cannot be null or empty.", new object[] { locationsPropertyName }));
            }
            bool flag2 = IsSpecificPath(name);
            string key = this.CreateCacheKey(cacheKeyPrefix, name, flag2 ? string.Empty : controllerName, areaName, theme);
            if (useCache)
            {
                var cached = this.ViewLocationCache.GetViewLocation(controllerContext.HttpContext, key);
                if (cached != null)
                {
                    return cached;
                }
            }
            if (!flag2)
            {
                return this.GetPathFromGeneralName(controllerContext, viewLocations, name, controllerName, areaName, theme, key, ref searchedLocations);
            }
            return this.GetPathFromSpecificName(controllerContext, name, key, ref searchedLocations);
        }

        protected virtual bool FilePathIsSupported(string virtualPath)
        {
            if (this.FileExtensions == null)
            {
                return true;
            }
            string str = this.GetExtensionThunk(virtualPath).TrimStart(new char[] { '.' });
            return this.FileExtensions.Contains<string>(str, StringComparer.OrdinalIgnoreCase);
        }

        protected virtual string GetPathFromSpecificName(ControllerContext controllerContext, string name, string cacheKey, ref string[] searchedLocations)
        {
            string virtualPath = name;
            if (!this.FilePathIsSupported(name) || !this.FileExists(controllerContext, name))
            {
                virtualPath = string.Empty;
                searchedLocations = new string[] { name };
            }
            this.ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, virtualPath);
            return virtualPath;
        }

        protected virtual string GetPathFromGeneralName(ControllerContext controllerContext, List<ViewLocation> locations, string name, string controllerName, string areaName, string theme, string cacheKey, ref string[] searchedLocations)
        {
            string virtualPath = string.Empty;
            searchedLocations = new string[locations.Count];
            for (int i = 0; i < locations.Count; i++)
            {
                string str2 = locations[i].Format(name, controllerName, areaName, theme);
                if (this.FileExists(controllerContext, str2))
                {
                    searchedLocations = _emptyLocations;
                    virtualPath = str2;
                    this.ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, virtualPath);
                    return virtualPath;
                }
                searchedLocations[i] = str2;
            }
            return virtualPath;
        }

        protected virtual string CreateCacheKey(string prefix, string name, string controllerName, string areaName, string theme)
        {
            return string.Format(CultureInfo.InvariantCulture, ":ViewCacheEntry:{0}:{1}:{2}:{3}:{4}:{5}", new object[] { base.GetType().AssemblyQualifiedName, prefix, name, controllerName, areaName, theme });
        }

        protected virtual List<ViewLocation> GetViewLocations(string[] viewLocationFormats, string[] areaViewLocationFormats)
        {
            var list = new List<ViewLocation>();
            if (areaViewLocationFormats != null)
            {
                list.AddRange(areaViewLocationFormats.Select(str => new AreaAwareViewLocation(str)).Cast<ViewLocation>());
            }
            if (viewLocationFormats != null)
            {
                list.AddRange(viewLocationFormats.Select(str2 => new ViewLocation(str2)));
            }
            return list;
        }

        protected virtual bool IsSpecificPath(string name)
        {
            char ch = name[0];
            if (ch != '~')
            {
                return (ch == '/');
            }
            return true;
        }

        protected virtual string GetCurrentTheme(bool mobile)
        {
            var themeContext = EngineContext.Current.Resolve<IThemeContext>();
            if (mobile)
                //mobile theme
                return themeContext.WorkingMobileTheme;
            else
                //desktop theme
                return themeContext.WorkingDesktopTheme;
        }

        protected virtual string GetAreaName(RouteData routeData)
        {
            object obj2;
            if (routeData.DataTokens.TryGetValue("area", out obj2))
            {
                return (obj2 as string);
            }
            return GetAreaName(routeData.Route);
        }

        protected virtual string GetAreaName(RouteBase route)
        {
            var area = route as IRouteWithArea;
            if (area != null)
            {
                return area.Area;
            }
            var route2 = route as Route;
            if ((route2 != null) && (route2.DataTokens != null))
            {
                return (route2.DataTokens["area"] as string);
            }
            return null;
        }

        protected virtual ViewEngineResult FindThemeableView(ControllerContext controllerContext, string viewName, string masterName, bool useCache, bool mobile)
        {
            string[] strArray;
            string[] strArray2;
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException("View name cannot be null or empty.", "viewName");
            }
            var theme = GetCurrentTheme(mobile);
            string requiredString = controllerContext.RouteData.GetRequiredString("controller");
            string str2 = this.GetPath(controllerContext, this.ViewLocationFormats, this.AreaViewLocationFormats, "ViewLocationFormats", viewName, requiredString, theme, "View", useCache, mobile, out strArray);
            string str3 = this.GetPath(controllerContext, this.MasterLocationFormats, this.AreaMasterLocationFormats, "MasterLocationFormats", masterName, requiredString, theme, "Master", useCache, mobile, out strArray2);
            if (!string.IsNullOrEmpty(str2) && (!string.IsNullOrEmpty(str3) || string.IsNullOrEmpty(masterName)))
            {
                return new ViewEngineResult(this.CreateView(controllerContext, str2, str3), this);
            }
            if (strArray2 == null)
            {
                strArray2 = new string[0];
            }
            return new ViewEngineResult(strArray.Union<string>(strArray2));

        }

        protected virtual ViewEngineResult FindThemeablePartialView(ControllerContext controllerContext, string partialViewName, bool useCache, bool mobile)
        {
            string[] strArray;
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (string.IsNullOrEmpty(partialViewName))
            {
                throw new ArgumentException("Partial view name cannot be null or empty.", "partialViewName");
            }
            var theme = GetCurrentTheme(mobile);
            string requiredString = controllerContext.RouteData.GetRequiredString("controller");
            string str2 = this.GetPath(controllerContext, this.PartialViewLocationFormats, this.AreaPartialViewLocationFormats, "PartialViewLocationFormats", partialViewName, requiredString, theme, "Partial", useCache, mobile, out strArray);
            if (string.IsNullOrEmpty(str2))
            {
                return new ViewEngineResult(strArray);
            }
            return new ViewEngineResult(this.CreatePartialView(controllerContext, str2), this);

        }
    
        #endregion

        #region Methods

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            var mobileDeviceHelper = EngineContext.Current.Resolve<IMobileDeviceHelper>();
            bool useMobileDevice = mobileDeviceHelper.IsMobileDevice(controllerContext.HttpContext)
                && mobileDeviceHelper.MobileDevicesSupported()
                && !mobileDeviceHelper.CustomerDontUseMobileVersion();

            string overrideViewName = useMobileDevice ?
                string.Format("{0}.{1}", viewName, _mobileViewModifier)
                : viewName;

            ViewEngineResult result = FindThemeableView(controllerContext, overrideViewName, masterName, useCache, useMobileDevice);
            // If we're looking for a Mobile view and couldn't find it try again without modifying the viewname
            if (useMobileDevice && (result == null || result.View == null))
                result = FindThemeableView(controllerContext, viewName, masterName, useCache, false);
            return result;

        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            var mobileDeviceHelper = EngineContext.Current.Resolve<IMobileDeviceHelper>();
            bool useMobileDevice = mobileDeviceHelper.IsMobileDevice(controllerContext.HttpContext)
                && mobileDeviceHelper.MobileDevicesSupported()
                && !mobileDeviceHelper.CustomerDontUseMobileVersion();

            string overrideViewName = useMobileDevice ?
                string.Format("{0}.{1}", partialViewName, _mobileViewModifier)
                : partialViewName;

            ViewEngineResult result = FindThemeablePartialView(controllerContext, overrideViewName, useCache, useMobileDevice);
            // If we're looking for a Mobile view and couldn't find it try again without modifying the viewname
            if (useMobileDevice && (result == null || result.View == null))
                result = FindThemeablePartialView(controllerContext, partialViewName, useCache, false);
            return result;
        }
    
        #endregion
}

    public class AreaAwareViewLocation : ViewLocation
    {
        public AreaAwareViewLocation(string virtualPathFormatString)
            : base(virtualPathFormatString)
        {
        }

        public override string Format(string viewName, string controllerName, string areaName, string theme)
        {
            return string.Format(CultureInfo.InvariantCulture, _virtualPathFormatString, viewName, controllerName, areaName, theme);
        }
    }

    public class ViewLocation
    {
        protected readonly string _virtualPathFormatString;

        public ViewLocation(string virtualPathFormatString)
        {
            _virtualPathFormatString = virtualPathFormatString;
        }

        public virtual string Format(string viewName, string controllerName, string areaName, string theme)
        {
            return string.Format(CultureInfo.InvariantCulture, _virtualPathFormatString, viewName, controllerName, theme);
        }
    }
}
