using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Affiliates;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class AffiliateController : BaseAdminController
{
    #region Fields

    protected readonly IAddressService _addressService;
    protected readonly IAffiliateModelFactory _affiliateModelFactory;
    protected readonly IAffiliateService _affiliateService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IOrderService _orderService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IWorkflowMessageService _workflowMessageService;

    #endregion

    #region Ctor

    public AffiliateController(IAddressService addressService,
        IAffiliateModelFactory affiliateModelFactory,
        IAffiliateService affiliateService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IOrderService orderService,
        IPermissionService permissionService,
        IPriceFormatter priceFormatter,
        IWorkflowMessageService workflowMessageService)
    {
        _addressService = addressService;
        _affiliateModelFactory = affiliateModelFactory;
        _affiliateService = affiliateService;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _orderService = orderService;
        _permissionService = permissionService;
        _priceFormatter = priceFormatter;
        _workflowMessageService = workflowMessageService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Sends a notification message to an affiliated customer
    /// </summary>
    /// <param name="affiliate">An affiliate</param>
    /// <returns>The task representing the asynchronous operation</returns>
    protected virtual async Task SendMessageForAffiliatedCustomerAsync(Affiliate affiliate)
    {
        if (affiliate.AssociatedCustomerId == null || affiliate.AssociatedCustomerId == 0 || !affiliate.Active)
            return;

        var customer = await _customerService.GetCustomerByIdAsync(affiliate.AssociatedCustomerId.Value);

        if (customer == null || customer.Deleted || !customer.Active)
            return;

        await _workflowMessageService.SendAffiliateAccountActiveCustomerNotificationAsync(customer, affiliate,
            customer.LanguageId ?? 0);
    }

    #endregion

    #region Methods

    public virtual IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [CheckPermission(StandardPermission.Promotions.AFFILIATES_VIEW)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliateSearchModelAsync(new AffiliateSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_VIEW)]
    public virtual async Task<IActionResult> List(AffiliateSearchModel searchModel)
    {
        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliateListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliateModelAsync(new AffiliateModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [FormValueRequired("save", "save-continue")]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(AffiliateModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var address = model.Address.ToEntity<Address>();

            address.CreatedOnUtc = DateTime.UtcNow;

            //some validation
            if (address.CountryId == 0)
                address.CountryId = null;
            if (address.StateProvinceId == 0)
                address.StateProvinceId = null;

            await _addressService.InsertAddressAsync(address);

            var affiliate = model.ToEntity<Affiliate>();

            //validate friendly URL name
            var friendlyUrlName = await _affiliateService.ValidateFriendlyUrlNameAsync(affiliate, model.FriendlyUrlName);
            affiliate.FriendlyUrlName = friendlyUrlName;
            affiliate.AddressId = address.Id;

            await _affiliateService.InsertAffiliateAsync(affiliate);

            if (affiliate.Active)
                await SendMessageForAffiliatedCustomerAsync(affiliate);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewAffiliate",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewAffiliate"), affiliate.Id), affiliate);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Affiliates.Added"));

            return continueEditing ? RedirectToAction("Edit", new { id = affiliate.Id }) : RedirectToAction("List");
        }

        //prepare model
        model = await _affiliateModelFactory.PrepareAffiliateModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Promotions.AFFILIATES_VIEW)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        //try to get an affiliate with the specified id
        var affiliate = await _affiliateService.GetAffiliateByIdAsync(id);
        if (affiliate == null || affiliate.Deleted)
            return RedirectToAction("List");

        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliateModelAsync(null, affiliate);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Edit(AffiliateModel model, bool continueEditing)
    {
        //try to get an affiliate with the specified id
        var affiliate = await _affiliateService.GetAffiliateByIdAsync(model.Id);
        if (affiliate == null || affiliate.Deleted)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            var address = await _addressService.GetAddressByIdAsync(affiliate.AddressId);
            address = model.Address.ToEntity(address);

            //some validation
            if (address.CountryId == 0)
                address.CountryId = null;
            if (address.StateProvinceId == 0)
                address.StateProvinceId = null;

            await _addressService.UpdateAddressAsync(address);
            var active = affiliate.Active;

            affiliate = model.ToEntity(affiliate);

            //validate friendly URL name
            var friendlyUrlName = await _affiliateService.ValidateFriendlyUrlNameAsync(affiliate, model.FriendlyUrlName);
            affiliate.FriendlyUrlName = friendlyUrlName;
            affiliate.AddressId = address.Id;

            await _affiliateService.UpdateAffiliateAsync(affiliate);

            if (!active && affiliate.Active)
                await SendMessageForAffiliatedCustomerAsync(affiliate);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditAffiliate",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditAffiliate"), affiliate.Id), affiliate);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Affiliates.Updated"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = affiliate.Id });
        }

        //prepare model
        model = await _affiliateModelFactory.PrepareAffiliateModelAsync(model, affiliate, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    //delete
    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get an affiliate with the specified id
        var affiliate = await _affiliateService.GetAffiliateByIdAsync(id);
        if (affiliate == null)
            return RedirectToAction("List");

        await _affiliateService.DeleteAffiliateAsync(affiliate);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteAffiliate",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteAffiliate"), affiliate.Id), affiliate);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Affiliates.Deleted"));

        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_VIEW)]
    public virtual async Task<IActionResult> AffiliatedOrderListGrid(AffiliatedOrderSearchModel searchModel)
    {
        //try to get an affiliate with the specified id
        var affiliate = await _affiliateService.GetAffiliateByIdAsync(searchModel.AffiliateId)
                        ?? throw new ArgumentException("No affiliate found with the specified id");

        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliatedOrderListModelAsync(searchModel, affiliate);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_VIEW)]
    public virtual async Task<IActionResult> AffiliateCommissionListGrid(AffiliateCommissionSearchModel searchModel)
    {
        //try to get an affiliate with the specified id
        var affiliate = await _affiliateService.GetAffiliateByIdAsync(searchModel.AffliateId)
                        ?? throw new ArgumentException("No affiliate found with the specified id");

        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliateCommissionListModelAsync(searchModel, affiliate);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_VIEW)]
    public virtual async Task<IActionResult> AffiliatedCustomerList(AffiliatedCustomerSearchModel searchModel)
    {
        //try to get an affiliate with the specified id
        var affiliate = await _affiliateService.GetAffiliateByIdAsync(searchModel.AffliateId)
                        ?? throw new ArgumentException("No affiliate found with the specified id");

        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliatedCustomerListModelAsync(searchModel, affiliate);

        return Json(model);
    }

    [CheckPermission([StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE, StandardPermission.Promotions.AFFILIATES_VIEW])]
    public virtual async Task<IActionResult> AddCustomerToAffiliatePopup()
    {
        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliateCustomerSearchModelAsync(new AffiliateCustomerSearchModel());

        return View(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission([StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE, StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE])]
    public virtual async Task<IActionResult> AddCustomerToAffiliatePopup([Bind(Prefix = nameof(AddCustomerToAffiliateModel))] AddCustomerToAffiliateModel model)
    {
        //try to get a customer with the specified id
        var customer = await _customerService.GetCustomerByIdAsync(model.CustomerId);
        if (customer == null)
            return Content("Cannot load a customer");

        ViewBag.RefreshPage = true;
        ViewBag.customerId = customer.Id;
        ViewBag.customerInfo = customer.Email;

        return View(new AffiliateCustomerSearchModel());
    }

    [HttpPost]
    [CheckPermission([StandardPermission.Customers.CUSTOMERS_CREATE_EDIT_DELETE, StandardPermission.Promotions.AFFILIATES_VIEW])]
    public virtual async Task<IActionResult> AddCustomerToAffiliatePopupList(AffiliateCustomerSearchModel searchModel)
    {
        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliateCustomerListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CreateCommission(int id)
    {
        //try to get an affiliate with the specified id
        var affiliate = await _affiliateService.GetAffiliateByIdAsync(id);
        if (affiliate == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliateCommissionEditModelAsync(new AffiliateCommissionEditModel
        {
            AffiliateId = affiliate.Id
        });

        return View(model);
    }


    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> CreateCommission(AffiliateCommissionEditModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var commission = new AffiliateCommission
            {
                AdminComment = model.AdminComment,
                CreateOn = DateTime.UtcNow,
                CommissionStatus = CommissionStatus.Pending,
                AffiliateId = model.AffiliateId
            };

            await _affiliateService.InsertAffiliateCommission(commission);
            
            return continueEditing ? RedirectToAction("EditCommission", new { id = commission.Id }) : RedirectToAction("Edit", new { id = model.AffiliateId });
        }

        //try to get an affiliate with the specified id
        var affiliate = await _affiliateService.GetAffiliateByIdAsync(model.AffiliateId);
        if (affiliate == null)
            return RedirectToAction("List");

        //prepare model
        model = await _affiliateModelFactory.PrepareAffiliateCommissionEditModelAsync(model);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditCommission(int id)
    {
        //try to get an affiliate with the specified id
        var commission = await _affiliateService.GetAffiliateCommissionByIdAsync(id);
        if (commission == null)
            return RedirectToAction("List");

        //prepare model
        var model = await _affiliateModelFactory.PrepareAffiliateCommissionEditModelAsync(new AffiliateCommissionEditModel
        {
            Id = id,
            AffiliateId = commission.AffiliateId
        });

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> EditCommission(AffiliateCommissionEditModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var commission = await _affiliateService.GetAffiliateCommissionByIdAsync(model.Id);
            commission.AdminComment = model.AdminComment;

            await _affiliateService.UpdateAffiliateCommissionAsync(commission);

            return continueEditing ? RedirectToAction("EditCommission", new { id = commission.Id }) : RedirectToAction("Edit", new { id = model.AffiliateId });
        }

        //try to get an affiliate with the specified id
        var affiliate = await _affiliateService.GetAffiliateByIdAsync(model.AffiliateId);
        if (affiliate == null)
            return RedirectToAction("List");

        //prepare model
        model = await _affiliateModelFactory.PrepareAffiliateCommissionEditModelAsync(model);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [ActionName("EditCommission")]
    [FormValueRequired("mark_as_paid")]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> MarkAsPaid(AffiliateCommissionEditModel model)
    {
        var commission = await _affiliateService.GetAffiliateCommissionByIdAsync(model.Id);

        if (commission == null)
            return RedirectToAction("List");

        commission.PaidOn = DateTime.UtcNow;
        commission.CommissionStatus = CommissionStatus.Paid;

        await _affiliateService.UpdateAffiliateCommissionAsync(commission);

        return RedirectToAction("EditCommission", new { id = commission.Id });
    }


    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> AddSelectedOrders(ICollection<int> selectedIds, int affiliateCommissionId)
    {
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var commission = await _affiliateService.GetAffiliateCommissionByIdAsync(affiliateCommissionId);

        if (commission == null)
            return Json(new { Result = false });

        var orders = await _orderService.GetOrdersByIdsAsync(selectedIds.ToArray());

        foreach (var order in orders)
        {
            order.AffiliateCommissionId = affiliateCommissionId;
            await _orderService.UpdateOrderAsync(order);
            commission.TotalCommissionAmount += order.AffiliateCommissionAmount ?? 0;
        }

        await _affiliateService.UpdateAffiliateCommissionAsync(commission);
        var totalCommissionAmount = await _priceFormatter.FormatPriceAsync(commission.TotalCommissionAmount, true, false);
        
        return Json(new { Result = true, totalCommissionAmount });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> RemoveOrderFromCommission(int orderId, int affiliateCommissionId)
    {
        var commission = await _affiliateService.GetAffiliateCommissionByIdAsync(affiliateCommissionId);

        if (commission == null)
            return Json(new { Result = false });

        var orders = await _orderService.GetOrderByIdAsync(orderId);

        if (orders != null)
        {
            orders.AffiliateCommissionId = null;
            await _orderService.UpdateOrderAsync(orders);
            commission.TotalCommissionAmount -= orders.AffiliateCommissionAmount ?? 0;
            await _affiliateService.UpdateAffiliateCommissionAsync(commission);
        }
        
        var totalCommissionAmount = await _priceFormatter.FormatPriceAsync(commission.TotalCommissionAmount, true, false);

        return Json(new { Result = true, totalCommissionAmount });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Promotions.AFFILIATES_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> DeleteCommission(int id)
    {
        var commission = await _affiliateService.GetAffiliateCommissionByIdAsync(id);
        await _affiliateService.DeleteAffiliateCommissionAsync(commission);

        return RedirectToAction("Edit", new { id = commission.AffiliateId });
    }

    #endregion
}