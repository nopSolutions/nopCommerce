using System;
using System.Web.Mvc;
using Nop.Core;
using Nop.Services.Common;

namespace Nop.Admin.Controllers
{
    public partial class PreferencesController : BaseAdminController
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public PreferencesController(IGenericAttributeService genericAttributeService,
            IWorkContext workContext)
        {
            this._genericAttributeService = genericAttributeService;
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        [HttpPost]
        public virtual ActionResult SavePreference(string name, bool value)
        {
            //permission validation is not required here
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, name, value);

            return Json(new
            {
                Result = true
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}