namespace Nop.Web.Framework.UI
{
    public interface IPageTitleBuilder
    {
        void AddTitleParts(params string[] parts);
        void AppendTitleParts(params string[] parts);
        string GenerateTitle();

        void AddMetaDescriptionParts(params string[] parts);
        void AppendMetaDescriptionParts(params string[] parts);
        string GenerateMetaDescription();

        void AddMetaKeywordParts(params string[] parts);
        void AppendMetaKeywordParts(params string[] parts);
        string GenerateMetaKeywords();

        void AddScriptParts(params string[] parts);
        void AppendScriptParts(params string[] parts);
        string GenerateScripts();

        void AddCssFileParts(params string[] parts);
        void AppendCssFileParts(params string[] parts);
        string GenerateCssFiles();


        void AddCanonicalUrlParts(params string[] parts);
        void AppendCanonicalUrlParts(params string[] parts);
        string GenerateCanonicalUrls();
    }
}
