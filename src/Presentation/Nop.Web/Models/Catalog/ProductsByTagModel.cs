<<<<<<< HEAD
﻿using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial record ProductsByTagModel : BaseNopEntityModel
    {
        public ProductsByTagModel()
        {
            CatalogProductsModel = new CatalogProductsModel();
        }

        public string TagName { get; set; }
        public string TagSeName { get; set; }

        public CatalogProductsModel CatalogProductsModel { get; set; }
    }
=======
﻿using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial record ProductsByTagModel : BaseNopEntityModel
    {
        public ProductsByTagModel()
        {
            CatalogProductsModel = new CatalogProductsModel();
        }

        public string TagName { get; set; }
        public string TagSeName { get; set; }

        public CatalogProductsModel CatalogProductsModel { get; set; }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}