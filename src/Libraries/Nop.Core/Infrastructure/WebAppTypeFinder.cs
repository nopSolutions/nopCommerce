using System.Collections.Generic;
using System.Reflection;
using Nop.Core.Configuration;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Provides information about types in the current web application. 
    /// Optionally this class can look at all assemblies in the bin folder.
    /// </summary>
    public class WebAppTypeFinder : AppDomainTypeFinder
    {
        private bool ensureBinFolderAssembliesLoaded = true;
        private bool binFolderAssembliesLoaded = false;

        //private bool ensurePluginFolderAssembliesLoaded = true;
        //private bool pluginFolderAssembliesLoaded = false;

        private IWebHelper _webHelper;

        public WebAppTypeFinder(IWebHelper webHelper)
        {
            this._webHelper = webHelper;
        }

        public WebAppTypeFinder(IWebHelper webHelper, NopConfig config)
        {
            this._webHelper = webHelper;
            this.ensureBinFolderAssembliesLoaded = config.DynamicDiscovery;
            //this.ensurePluginFolderAssembliesLoaded = config.DynamicDiscovery;
        }

        #region Properties

        /// <summary>
        /// Gets or sets wether assemblies in the bin folder of the web application should be specificly checked for beeing loaded on application load. This is need in situations where plugins need to be loaded in the AppDomain after the application been reloaded.
        /// </summary>
        public bool EnsureBinFolderAssembliesLoaded
        {
            get { return ensureBinFolderAssembliesLoaded; }
            set { ensureBinFolderAssembliesLoaded = value; }
        }

        ///// <summary>
        ///// Gets or sets wether assemblies in the bin folder of the web application should be specificly checked for beeing loaded on application load. This is need in situations where plugins need to be loaded in the AppDomain after the application been reloaded.
        ///// </summary>
        //public bool EnsurePluginFolderAssembliesLoaded
        //{
        //    get { return ensurePluginFolderAssembliesLoaded; }
        //    set { ensurePluginFolderAssembliesLoaded = value; }
        //}


        #endregion

        #region Methods
        public override IList<Assembly> GetAssemblies()
        {
            //if (this.EnsurePluginFolderAssembliesLoaded && !pluginFolderAssembliesLoaded)
            //{
            //    pluginFolderAssembliesLoaded = true;
            //    LoadMatchingAssemblies(_webHelper.MapPath("~/Plugins"));
            //}

            if (this.EnsureBinFolderAssembliesLoaded && !binFolderAssembliesLoaded)
            {
                binFolderAssembliesLoaded = true;
                LoadMatchingAssemblies(_webHelper.MapPath("~/bin"));
            }

            return base.GetAssemblies();
        }
        #endregion
    }
}
