namespace Nop.Web.Areas.Admin.Helpers;

public partial interface ISummernoteHelper
{
    Task<string> GetRichEditorLanguageAsync();
}
