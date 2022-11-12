<<<<<<< HEAD
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial record SearchBoxModel : BaseNopModel
    {
        public bool AutoCompleteEnabled { get; set; }
        public bool ShowProductImagesInSearchAutoComplete { get; set; }
        public int SearchTermMinimumLength { get; set; }
        public bool ShowSearchBox { get; set; }
    }
=======
﻿using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial record SearchBoxModel : BaseNopModel
    {
        public bool AutoCompleteEnabled { get; set; }
        public bool ShowProductImagesInSearchAutoComplete { get; set; }
        public int SearchTermMinimumLength { get; set; }
        public bool ShowSearchBox { get; set; }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}