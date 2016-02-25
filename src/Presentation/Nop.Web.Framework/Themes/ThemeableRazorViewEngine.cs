using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using Nop.Web.Framework.Controllers;

namespace Nop.Web.Framework.Themes
{
    public class ThemeableRazorViewEngine : ThemeableVirtualPathProviderViewEngine
    {
        private const string PluginReplacementString = "{DynamicPluginPath}";

        public ThemeableRazorViewEngine()
        {                                             
            AreaViewLocationFormats = new[]
                                          {
                                              //themes
                                              "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.cshtml",
                                              "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.cshtml",
                                              
                                              //default
                                              "~/Areas/{2}/Views/{1}/{0}.cshtml",
                                              "~/Areas/{2}/Views/Shared/{0}.cshtml",
                                          };

            AreaMasterLocationFormats = new[]
                                            {
                                                //themes
                                                "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.cshtml",
                                                "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.cshtml",


                                                //default
                                                "~/Areas/{2}/Views/{1}/{0}.cshtml",
                                                "~/Areas/{2}/Views/Shared/{0}.cshtml",
                                            };

            AreaPartialViewLocationFormats = new[]
                                                 {
                                                     //themes
                                                    "~/Areas/{2}/Themes/{3}/Views/{1}/{0}.cshtml",
                                                    "~/Areas/{2}/Themes/{3}/Views/Shared/{0}.cshtml",
                                                    
                                                    //default
                                                    "~/Areas/{2}/Views/{1}/{0}.cshtml",
                                                    "~/Areas/{2}/Views/Shared/{0}.cshtml"
                                                 };
            
            ViewLocationFormats = new[]{
                                                    //themes
                                                    "~/Themes/{2}/Views/{1}/{0}.cshtml",
                                                    "~/Themes/{2}/Views/Shared/{0}.cshtml",

                                                    //default
                                                    "~/Views/{1}/{0}.cshtml",
                                                    "~/Views/Shared/{0}.cshtml",

                                                    //Admin
                                                    "~/Administration/Views/{1}/{0}.cshtml",
                                                    "~/Administration/Views/Shared/{0}.cshtml",

                                                    //dynamic plugins
                                                    PluginReplacementString + "/{1}/{0}.cshtml"
                                                };

            MasterLocationFormats = new[]
                                        {
                                            //themes
                                            "~/Themes/{2}/Views/{1}/{0}.cshtml", 
                                            "~/Themes/{2}/Views/Shared/{0}.cshtml", 

                                            //default
                                            "~/Views/{1}/{0}.cshtml", 
                                            "~/Views/Shared/{0}.cshtml"
                                        };

            PartialViewLocationFormats = new []{
                                                    //themes
                                                    "~/Themes/{2}/Views/{1}/{0}.cshtml",
                                                    "~/Themes/{2}/Views/Shared/{0}.cshtml",

                                                    //default
                                                    "~/Views/{1}/{0}.cshtml",
                                                    "~/Views/Shared/{0}.cshtml",

                                                    //Admin
                                                    "~/Administration/Views/{1}/{0}.cshtml",
                                                    "~/Administration/Views/Shared/{0}.cshtml",

                                                    //dynamic plugins
                                                    PluginReplacementString + "/{1}/{0}.cshtml"
                                                };

            FileExtensions = new[] { "cshtml" };
        }
        private string GetPluginLocation(ControllerContext controllerContext, string path)
        {
            var controllerType = controllerContext.Controller.GetType();
            var pluginLocation = controllerType.GetCustomAttribute<ThemeablePluginViewLocationAttribute>(true);
            if (pluginLocation != null)
                return path.Replace(PluginReplacementString, pluginLocation.BaseViewLocation);

            // return default location base on controller namespace
            return path.Replace(PluginReplacementString, "~/" + controllerType.Namespace);
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, GetPluginLocation(controllerContext, partialPath), null, false, fileExtensions);
            //return new RazorView(controllerContext, partialPath, layoutPath, runViewStartPages, fileExtensions, base.ViewPageActivator);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, GetPluginLocation(controllerContext, viewPath), GetPluginLocation(controllerContext, masterPath), true, fileExtensions);
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            return base.FileExists(controllerContext, GetPluginLocation(controllerContext, virtualPath));
        }
    }
}
