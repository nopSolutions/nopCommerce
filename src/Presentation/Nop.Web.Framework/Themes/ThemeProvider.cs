using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Nop.Core;

namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// 主题全部集合：项目的路径~/Themes/，一个主题一个目录，
    /// 遍历目录并加载每一个主题目录下面的配置信息theme.config
    /// </summary>
    public partial class ThemeProvider : IThemeProvider
    {
        #region Fields

        private readonly IList<ThemeConfiguration> _themeConfigurations = new List<ThemeConfiguration>();
        private readonly string _basePath = string.Empty;

        #endregion

        #region Constructors

        public ThemeProvider()
        {
            _basePath = CommonHelper.MapPath("~/Themes/");
            LoadConfigurations();
        }

        #endregion

        #region IThemeProvider

        public ThemeConfiguration GetThemeConfiguration(string themeName)
        {
            return _themeConfigurations
                .SingleOrDefault(x => x.ThemeName.Equals(themeName, StringComparison.InvariantCultureIgnoreCase));
        }

        public IList<ThemeConfiguration> GetThemeConfigurations()
        {
            return _themeConfigurations;
        }

        public bool ThemeConfigurationExists(string themeName)
        {
            return GetThemeConfigurations().Any(configuration => configuration.ThemeName.Equals(themeName, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion

        #region Utility

        /// <summary>
        /// 从主题所在根目录开始遍历，并加载这些主题
        /// </summary>
        private void LoadConfigurations()
        {
            //TODO:Use IFileStorage?
            foreach (string themeName in Directory.GetDirectories(_basePath))
            {
                var configuration = CreateThemeConfiguration(themeName);
                if (configuration != null)
                {
                    _themeConfigurations.Add(configuration);
                }
            }
        }

        /// <summary>
        /// 读取主题配置文件里面配置信息并封装成一个对象ThemeConfiguration
        /// </summary>
        /// <param name="themePath"></param>
        /// <returns></returns>
        private ThemeConfiguration CreateThemeConfiguration(string themePath)
        {
            var themeDirectory = new DirectoryInfo(themePath);
            var themeConfigFile = new FileInfo(Path.Combine(themeDirectory.FullName, "theme.config"));

            if (themeConfigFile.Exists)
            {
                var doc = new XmlDocument();
                doc.Load(themeConfigFile.FullName);
                return new ThemeConfiguration(themeDirectory.Name, themeDirectory.FullName, doc);
            }

            return null;
        }

        #endregion
    }
}
