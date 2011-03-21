using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Nop.Web.Framework.Themes
{
    public class ThemeConfiguration
    {
		#region Constructors (1) 

        public ThemeConfiguration(string themeName, string path, XmlDocument doc)
        {
            ThemeName = themeName;
            Path = path;
            var node = doc.SelectSingleNode("Theme");
            if (node != null)
            {
                var attribute = node.Attributes["title"];
                ThemeTitle = attribute == null ? string.Empty : attribute.Value;
                attribute = node.Attributes["previewImageUrl"];
                PreviewImageUrl = attribute == null ? string.Empty : attribute.Value;
                attribute = node.Attributes["previewText"];
                PreviewText = attribute == null ? string.Empty : attribute.Value;
                attribute = node.Attributes["isDefault"];
                IsDefault = attribute == null ? false : bool.Parse(attribute.Value);
            }
        }

		#endregion Constructors 

		#region Properties (5) 

        public string Path { get; protected set; }

        public string PreviewImageUrl { get; protected set; }

        public string PreviewText { get; protected set; }

        public string ThemeName { get; protected set; }

        public string ThemeTitle { get; protected set; }

        public bool IsDefault { get; protected set; }

		#endregion Properties 
    }
}
