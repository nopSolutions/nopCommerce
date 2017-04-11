#if NET451
namespace Nop.Web.Framework.Localization
{
    public delegate LocalizedString Localizer(string text, params object[] args);
}
#endif