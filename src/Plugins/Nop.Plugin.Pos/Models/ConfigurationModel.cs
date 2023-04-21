using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Pos.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}
