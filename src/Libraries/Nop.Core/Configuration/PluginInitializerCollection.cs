using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;

namespace Nop.Core.Configuration
{
    /// <summary>
    /// A configurable collection of plugin initializers.
    /// </summary>
    [ConfigurationCollection(typeof(PluginInitializerElement))]
    public class PluginInitializerCollection : LazyRemovableCollection<PluginInitializerElement>
    {
        protected override void OnDeserializeRemoveElement(PluginInitializerElement element, XmlReader reader)
        {
            element.Type = reader.GetAttribute("type");
        }
    }
}
