using System.Collections.Generic;

namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// 主题全部集合：项目的路径~/Themes/，一个主题一个目录，
    /// 遍历目录并加载每一个主题目录下面的配置信息theme.config
    /// </summary>
    public partial interface IThemeProvider
    {
        /// <summary>
        /// 获取指定主题配置信息
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns></returns>
        ThemeConfiguration GetThemeConfiguration(string themeName);
        /// <summary>
        /// 获取全部主题的配置信息集合
        /// </summary>
        /// <returns></returns>

        IList<ThemeConfiguration> GetThemeConfigurations();

        /// <summary>
        /// 判断主题是否存在
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns></returns>
        bool ThemeConfigurationExists(string themeName);
    }
}
