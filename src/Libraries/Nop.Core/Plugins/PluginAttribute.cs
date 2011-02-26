using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Use this attribute to reference a plugin initializer 
    /// that is invoked when the factory is started.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class PluginInitializerAttribute : InitializerCreatingAttribute
    {
        /// <summary>Creates a new instance of the PlugInAttribute class.</summary>
        public PluginInitializerAttribute()
        {
        }

        /// <summary>Creates a new instance of the PlugInAttribute class.</summary>
        /// <param name="title">The title of the plugin.</param>
        /// <param name="name">The name of the plugin.</param>
        /// <param name="initializerType">The name of the type responsible for initializing this plugin.</param>
        public PluginInitializerAttribute(string title, string name, Type initializerType)
        {
            Title = title;
            Name = name;
            InitializerType = initializerType;
        }



        /// <summary>Gets or sets the title of the plugin marked by this attribute.</summary>
        public string Title { get; set; }

        /// <summary>Gets or sets the name of the plugin marked by this attribute.</summary>
        public string Name { get; set; }
    }
}
