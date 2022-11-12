<<<<<<< HEAD
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
=======
=======
<<<<<<< HEAD
>>>>>>> 974287325803649b246516d81982b95e372d09b9
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
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
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
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
}