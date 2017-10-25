using System.Web.Mvc;

namespace Nop.Web.Framework.UI
{
    /// <summary>
    /// Page head builder
    /// 页面头部生成器接口
    /// </summary>
    public partial interface IPageHeadBuilder
    {
        /// <summary>
        /// 添加标题到现有标题前（标题列表最终输出时会反转）
        /// </summary>
        /// <param name="part"></param>
        void AddTitleParts(string part);
        /// <summary>
        /// 添加标题到现有标题后（标题列表最终输出时会反转）
        /// </summary>
        /// <param name="part"></param>
        void AppendTitleParts(string part);
        /// <summary>
        /// 生成标题
        /// </summary>
        /// <param name="addDefaultTitle">是否附加默认标题</param>
        /// <returns></returns>
        string GenerateTitle(bool addDefaultTitle);

        void AddMetaDescriptionParts(string part);
        /// <summary>
        /// 附加页面描述片断
        /// </summary>
        /// <param name="part"></param>
        void AppendMetaDescriptionParts(string part);
        string GenerateMetaDescription();

        void AddMetaKeywordParts(string part);
        /// <summary>
        /// 附加页面关键词
        /// </summary>
        /// <param name="part"></param>
        void AppendMetaKeywordParts(string part);

        /// <summary>
        /// 生成页面关键词
        /// </summary>
        /// <returns></returns>
        string GenerateMetaKeywords();

        void AddScriptParts(ResourceLocation location, string part, bool excludeFromBundle, bool isAync);
        void AppendScriptParts(ResourceLocation location, string part, bool excludeFromBundle, bool isAsync);
        string GenerateScripts(UrlHelper urlHelper, ResourceLocation location, bool? bundleFiles = null);

        void AddCssFileParts(ResourceLocation location, string part, bool excludeFromBundle = false);
        void AppendCssFileParts(ResourceLocation location, string part, bool excludeFromBundle = false);
        string GenerateCssFiles(UrlHelper urlHelper, ResourceLocation location, bool? bundleFiles = null);
        
        void AddCanonicalUrlParts(string part);
        void AppendCanonicalUrlParts(string part);
        string GenerateCanonicalUrls();

        void AddHeadCustomParts(string part);
        void AppendHeadCustomParts(string part);
        /// <summary>
        /// 生成头部自定内容
        /// </summary>
        /// <returns></returns>
        string GenerateHeadCustom();
        
        void AddPageCssClassParts(string part);
        void AppendPageCssClassParts(string part);
        string GeneratePageCssClasses();

        /// <summary>
        /// Specify "edit page" URL
        /// </summary>
        /// <param name="url">URL</param>
        void AddEditPageUrl(string url);
        /// <summary>
        /// Get "edit page" URL
        /// </summary>
        /// <returns>URL</returns>
        string GetEditPageUrl();

        /// <summary>
        /// Specify system name of admin menu item that should be selected (expanded)
        /// </summary>
        /// <param name="systemName">System name</param>
        void SetActiveMenuItemSystemName(string systemName);
        /// <summary>
        /// Get system name of admin menu item that should be selected (expanded)
        /// </summary>
        /// <returns>System name</returns>
        string GetActiveMenuItemSystemName();
    }
}
