using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public class SearchBoxModel : BaseNopModel
    {
        public bool AutoCompleteEnabled { get; set; }
        public bool ShowProductImagesInSearchAutoComplete { get; set; }
        public int SearchTermMinimumLength { get; set; }
    }
}