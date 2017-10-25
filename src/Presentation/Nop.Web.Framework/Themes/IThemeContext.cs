namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// Work context
    /// 主题上下文接口，它是主题的入口，也是中心。
    /// </summary>
    public interface IThemeContext
    {
        /// <summary>
        /// Get or set current theme system name
        /// 获取系统当前主题的名称
        /// </summary>
        string WorkingThemeName { get; set; }
    }
}
