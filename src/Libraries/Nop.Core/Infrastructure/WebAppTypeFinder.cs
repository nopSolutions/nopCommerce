using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
        private IWebHelper _webHelper;

        public WebAppTypeFinder(IWebHelper webHelper)
        {
            this._webHelper = webHelper;
        }

        public WebAppTypeFinder(IWebHelper webHelper, EngineSection engineConfiguration)
        {
            this._webHelper = webHelper;
            this.ensureBinFolderAssembliesLoaded = engineConfiguration.DynamicDiscovery;
            foreach (var assembly in engineConfiguration.Assemblies.AllElements)
                AssemblyNames.Add(assembly.Assembly);
        }

        #region Properties
        /// <summary>Gets or sets wether assemblies in the bin folder of the web application should be specificly checked for beeing loaded on application load. This is need in situations where plugins need to be loaded in the AppDomain after the application been reloaded.</summary>
        public bool EnsureBinFolderAssembliesLoaded
        {
            get { return ensureBinFolderAssembliesLoaded; }
            set { ensureBinFolderAssembliesLoaded = value; }
        }

        #endregion

        #region Methods
        public override IList<Assembly> GetAssemblies()
        {
            if (EnsureBinFolderAssembliesLoaded && !binFolderAssembliesLoaded)
            {
                binFolderAssembliesLoaded = true;
                LoadMatchingAssemblies(_webHelper.MapPath("~/bin"));
            }

            return base.GetAssemblies();
        }
        #endregion
    }
}
