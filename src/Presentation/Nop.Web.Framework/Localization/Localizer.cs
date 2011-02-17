
using System.Linq;
using Nop.Web.Framework.Localization;

namespace Nop.Web.Framework.Localization
{
    public delegate LocalizedString Localizer(string text, params object[] args);
}