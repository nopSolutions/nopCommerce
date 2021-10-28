using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Common;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class PreferencesController : BaseAdminController
    {
        #region Fields

        protected IGenericAttributeService GenericAttributeService { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public PreferencesController(IGenericAttributeService genericAttributeService,
            IWorkContext workContext)
        {
            GenericAttributeService = genericAttributeService;
            WorkContext = workContext;
        }

        #endregion

        #region Methods

        [HttpPost]
        public virtual async Task<IActionResult> SavePreference(string name, bool value)
        {
            //permission validation is not required here
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            await GenericAttributeService.SaveAttributeAsync(await WorkContext.GetCurrentCustomerAsync(), name, value);

            return Json(new
            {
                Result = true
            });
        }

        #endregion
    }
}