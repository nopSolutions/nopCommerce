using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Pos.Models
{
    public record PosAddressModel : BaseNopModel
    {
        public Address posaddress { get; set; }
        public string Country { get; set; }

        public string state { get; set; }
    }
}
