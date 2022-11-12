<<<<<<< HEAD
﻿using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    /// <summary>
    /// Represents a product specification model
    /// </summary>
    public partial record ProductSpecificationModel : BaseNopModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the grouped specification attribute models
        /// </summary>
        public IList<ProductSpecificationAttributeGroupModel> Groups { get; set; }

        #endregion

        #region Ctor

        public ProductSpecificationModel()
        {
            Groups = new List<ProductSpecificationAttributeGroupModel>();
        }

        #endregion
    }
=======
﻿using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    /// <summary>
    /// Represents a product specification model
    /// </summary>
    public partial record ProductSpecificationModel : BaseNopModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the grouped specification attribute models
        /// </summary>
        public IList<ProductSpecificationAttributeGroupModel> Groups { get; set; }

        #endregion

        #region Ctor

        public ProductSpecificationModel()
        {
            Groups = new List<ProductSpecificationAttributeGroupModel>();
        }

        #endregion
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}