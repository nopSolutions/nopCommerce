using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Services.Topics;

namespace Nop.Web.Controllers
{
    public partial class BackwardCompatibility2XController : BaseNopController
    {
		#region Fields

        private readonly IProductTagService _productTagService;
        #endregion

		#region Constructors

        public BackwardCompatibility2XController(IProductTagService productTagService)
        {
            this._productTagService = productTagService;
        }

		#endregion
        
        #region Methods

        //in versions 2.00-2.65 we had typo in producttag URLs ("productag" instead of "producttag")
        public ActionResult RedirectProductsByTag(int productTagId, string seName)
        {
            return RedirectToRoutePermanent("ProductsByTag", new { productTagId = productTagId, SeName = seName });
        }
        public ActionResult RedirectProductTagsAll()
        {
            return RedirectToRoutePermanent("ProductTagsAll");
        }
        
        #endregion
    }
}
