using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common;

public partial record LanguageModel : BaseNopEntityModel
{
    public string Name { get; set; }

    public string FlagImageFileName { get; set; }
}