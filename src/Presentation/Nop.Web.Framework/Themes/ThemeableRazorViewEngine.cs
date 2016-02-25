using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using Nop.Core.Plugins;

namespace Nop.Web.Framework.Themes
{
    public class ThemeableRazorViewEngine : ThemeableVirtualPathProviderViewEngine
    {
        private const string InstalledPluginsFilePath = "~/App_Data/InstalledPlugins.txt";
        private const string PluginReplacementString = "%PluginSystemName%";
        private const string PluginsLocationFormat = "~/Plugins/" + PluginReplacementString + "/Views/{1}/{0}.cshtml";

        public ThemeableRazorViewEngine()
        {
            var installedPluginSystemNames = PluginFileParser.ParseInstalledPluginsFile(HostingEnvironment.MapPath(InstalledPluginsFilePath));

            var customViewLocationFormats = new List<string>()
                                                {
                                                    //themes
                                                    "~/Themes/{2}/Views/{1}/{0}.cshtml",
                                                    "~/Themes/{2}/Views/Shared/{0}.cshtml",

                                                    //default
                                                    "~/Views/{1}/{0}.cshtml",
                                                    "~/Views/Shared/{0}.cshtml",

                                                    //Admin
                                                    "~/Administration/Views/{1}/{0}.cshtml",
                                                    "~/Administration/Views/Shared/{0}.cshtml"
                                                };

            customViewLocationFormats.AddRange(installedPluginSystemNames.Select(systemName => PluginsLocationFormat.Replace(PluginReplacementString, systemName)));

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
            
            ViewLocationFormats = customViewLocationFormats.ToArray();

            MasterLocationFormats = new[]
                                        {
                                            //themes
                                            "~/Themes/{2}/Views/{1}/{0}.cshtml", 
                                            "~/Themes/{2}/Views/Shared/{0}.cshtml", 

                                            //default
                                            "~/Views/{1}/{0}.cshtml", 
                                            "~/Views/Shared/{0}.cshtml"
                                        };

            PartialViewLocationFormats = customViewLocationFormats.ToArray();

            FileExtensions = new[] { "cshtml" };
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            IEnumerable<string> fileExtensions = base.FileExtensions;

            return new RazorView(controllerContext, partialPath, null, false, fileExtensions);
            //return new RazorView(controllerContext, partialPath, layoutPath, runViewStartPages, fileExtensions, base.ViewPageActivator);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, viewPath, masterPath, true, fileExtensions);
        }
    }
}
