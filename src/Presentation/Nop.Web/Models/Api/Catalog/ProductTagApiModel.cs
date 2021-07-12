using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Api.Catalog
{
    public partial record ProductTagApiModel : BaseNopEntityModel
    {
        public string Name { get; set; }
    }
}