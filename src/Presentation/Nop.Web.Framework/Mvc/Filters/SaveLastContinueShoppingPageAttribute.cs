using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Helpers;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute that allow to store current URL in the generic attribute for "Continue Shopping" button
/// </summary>
public sealed class SaveLastContinueShoppingPageAttribute : TypeFilterAttribute
{
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    public SaveLastContinueShoppingPageAttribute() : base(typeof(SaveLastContinueShoppingPageFilter))
    {
    }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter confirming that user with "Vendor" customer role has appropriate vendor account associated (and active)
    /// </summary>
    private class SaveLastContinueShoppingPageFilter : IAsyncActionFilter
    {
        #region Fields

        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly IStoreContext _storeContext;
        protected readonly IWebHelper _webHelper;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SaveLastContinueShoppingPageFilter(IGenericAttributeService genericAttributeService, IStoreContext storeContext, IWebHelper webHelper, IWorkContext workContext)
        {
            _genericAttributeService = genericAttributeService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion
        
        #region Methods

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                store.Id);
        }

        #endregion
    }

    #endregion
}