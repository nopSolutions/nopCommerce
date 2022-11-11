using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Directory
{
    public partial record StateProvinceModel : BaseNopModel
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}