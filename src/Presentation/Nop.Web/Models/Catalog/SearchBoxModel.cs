using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial record SearchBoxModel : BaseNopModel
    {
        public bool AutoCompleteEnabled { get; set; }
        public bool ShowProductImagesInSearchAutoComplete { get; set; }
        public int SearchTermMinimumLength { get; set; }
        public bool ShowSearchBox { get; set; }
    }
}