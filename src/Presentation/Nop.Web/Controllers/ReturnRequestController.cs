using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Http;
using Nop.Core.Http.Extensions;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Order;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class ReturnRequestController : BasePublicController
{
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    protected readonly IAddressService _addressService;
    protected readonly ICustomerService _customerService;
    protected readonly ICustomNumberFormatter _customNumberFormatter;
    protected readonly IDownloadService _downloadService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderService _orderService;
    protected readonly IReturnRequestModelFactory _returnRequestModelFactory;
    protected readonly IReturnRequestService _returnRequestService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly ReturnRequestSettings _returnRequestSettings;

    #endregion

    #region Ctor

    public ReturnRequestController(CaptchaSettings captchaSettings,
        IAddressService addressService,
        ICustomerService customerService,
        ICustomNumberFormatter customNumberFormatter,
        IDownloadService downloadService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        INopFileProvider fileProvider,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        IReturnRequestModelFactory returnRequestModelFactory,
        IReturnRequestService returnRequestService,
        IStoreContext storeContext,
        IWorkContext workContext,
        IWorkflowMessageService workflowMessageService,
        LocalizationSettings localizationSettings,
        ReturnRequestSettings returnRequestSettings)
    {
        _captchaSettings = captchaSettings;
        _addressService = addressService;
        _customerService = customerService;
        _customNumberFormatter = customNumberFormatter;
        _downloadService = downloadService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _fileProvider = fileProvider;
        _orderProcessingService = orderProcessingService;
        _orderService = orderService;
        _returnRequestModelFactory = returnRequestModelFactory;
        _returnRequestService = returnRequestService;
        _storeContext = storeContext;
        _workContext = workContext;
        _workflowMessageService = workflowMessageService;
        _localizationSettings = localizationSettings;
        _returnRequestSettings = returnRequestSettings;
    }

    #endregion

    #region Utilities

    protected virtual async Task<bool> ValidateWithdrawalTokenAsync(Order order, string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        var orderCustomer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

        //order has been created by registered customer
        if (orderCustomer != null && !await _customerService.IsGuestAsync(orderCustomer))
            return false;

        var existedToken = await _genericAttributeService.GetAttributeAsync<string>(order, NopOrderDefaults.WithdrawalTokenAttribute);
        var existedTokenGeneratedDate = await _genericAttributeService.GetAttributeAsync<DateTime>(order, NopOrderDefaults.WithdrawalTokenDateGeneratedAttribute);

        return string.Equals(token, existedToken, StringComparison.OrdinalIgnoreCase) && existedTokenGeneratedDate > DateTime.UtcNow.AddDays(-_returnRequestSettings.WithdrawalLinkDaysValid);
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> CustomerReturnRequests()
    {
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        var model = await _returnRequestModelFactory.PrepareCustomerReturnRequestsModelAsync();
        return View(model);
    }

    public virtual async Task<IActionResult> Find()
    {
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();

        if (await _customerService.IsRegisteredAsync(currentCustomer))
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_ORDERS);

        if (!_returnRequestSettings.GuestReturnRequestsAllowed)
            return Challenge();

        return View(await _returnRequestModelFactory.PrepareWithdrawalFormModelAsync(null));
    }

    [HttpPost]
    [ValidateCaptcha]
    public virtual async Task<IActionResult> Find(WithdrawalFormModel model, bool captchaValid)
    {
        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnWithdrawalForm && !captchaValid)
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();

        if (await _customerService.IsRegisteredAsync(currentCustomer))
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_ORDERS);

        if (!ModelState.IsValid)
            return View(await _returnRequestModelFactory.PrepareWithdrawalFormModelAsync(model));

        var order = int.TryParse(model.OrderNumber, out var orderNumber)
            ? await _orderService.GetOrderByIdAsync(orderNumber) :
            await _orderService.GetOrderByCustomOrderNumberAsync(model.OrderNumber);

        var resultText = await _localizationService.GetResourceAsync("ReturnRequests.WithdrawalForm.ConfirnationText");
        var store = await _storeContext.GetCurrentStoreAsync();

        model = await _returnRequestModelFactory.PrepareWithdrawalFormModelAsync(model);

        if (order is null || order.StoreId != store.Id)
            return View(model with { Result = resultText });

        if (!await _orderProcessingService.IsReturnRequestAllowedAsync(order))
            return View(model with { Result = resultText });

        var address = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        if (!string.Equals(address?.Email, model.EmailAddress, StringComparison.OrdinalIgnoreCase))
            return View(model with { Result = resultText });

        //save token and current date
        await _genericAttributeService.SaveAttributeAsync(order, NopOrderDefaults.WithdrawalTokenAttribute, Guid.NewGuid().ToString());
        await _genericAttributeService.SaveAttributeAsync(order, NopOrderDefaults.WithdrawalTokenDateGeneratedAttribute, DateTime.UtcNow);

        //send email
        await _workflowMessageService.SendWithdrawalRequestConfirmationNotificationAsync(order);

        return View(model with { Result = resultText });
    }

    public virtual async Task<IActionResult> ReturnRequest(int orderId, string token)
    {

        var order = await _orderService.GetOrderByIdAsync(orderId);

        if (order == null || order.Deleted)
            return Challenge();

        if (!string.IsNullOrEmpty(token))
        {
            if (!await ValidateWithdrawalTokenAsync(order, token))
                return RedirectToRoute(NopRouteNames.Standard.RETURN_REQUEST);
        }
        else
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer.Id != order.CustomerId)
                return Challenge();
        }

        if (!await _orderProcessingService.IsReturnRequestAllowedAsync(order))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = new SubmitReturnRequestModel() { WithdrawalToken = token };
        model = await _returnRequestModelFactory.PrepareSubmitReturnRequestModelAsync(model, order);

        return View(model);
    }

    [HttpPost, ActionName("ReturnRequest")]
    public virtual async Task<IActionResult> ReturnRequestSubmit(int orderId, SubmitReturnRequestModel model, IFormCollection form)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null || order.Deleted)
            return Challenge();

        var customer = await _workContext.GetCurrentCustomerAsync();

        if (!string.IsNullOrEmpty(model.WithdrawalToken))
        {
            if (!await ValidateWithdrawalTokenAsync(order, model.WithdrawalToken))
                return Challenge();
        }
        else
        {
            if (customer.Id != order.CustomerId)
                return Challenge();
        }

        if (!await _orderProcessingService.IsReturnRequestAllowedAsync(order))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var count = 0;

        var downloadId = 0;
        if (_returnRequestSettings.ReturnRequestsAllowFiles)
        {
            var download = await _downloadService.GetDownloadByGuidAsync(model.UploadedFileGuid);
            if (download != null)
                downloadId = download.Id;
        }

        //returnable products
        var orderItems = await _orderService.GetOrderItemsAsync(order.Id, isNotReturnable: false);
        foreach (var orderItem in orderItems)
        {
            var quantity = 0; //parse quantity
            foreach (var formKey in form.Keys)
            {
                if (formKey.Equals($"quantity{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
                {
                    _ = int.TryParse(form[formKey], out quantity);
                    break;
                }
            }

            if (quantity > 0)
            {
                var store = await _storeContext.GetCurrentStoreAsync();

                var rrr = await _returnRequestService.GetReturnRequestReasonByIdAsync(model.ReturnRequestReasonId);
                var rra = await _returnRequestService.GetReturnRequestActionByIdAsync(model.ReturnRequestActionId);

                var rr = new ReturnRequest
                {
                    CustomNumber = "",
                    StoreId = store.Id,
                    OrderItemId = orderItem.Id,
                    Quantity = quantity,
                    CustomerId = customer.Id,
                    CustomerComments = model.Comments,
                    UploadedFileId = downloadId,
                    StaffNotes = string.Empty,
                    ReasonForReturn = _returnRequestSettings.ReturnReasonsEnabled && rrr != null ? await _localizationService.GetLocalizedAsync(rrr, x => x.Name) : "not available",
                    RequestedAction = _returnRequestSettings.ReturnActionsEnabled && rra != null ? await _localizationService.GetLocalizedAsync(rra, x => x.Name) : "not available",
                    ReturnRequestStatus = ReturnRequestStatus.Pending,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow
                };

                await _returnRequestService.InsertReturnRequestAsync(rr);

                //set return request custom number
                rr.CustomNumber = _customNumberFormatter.GenerateReturnRequestCustomNumber(rr);
                await _customerService.UpdateCustomerAsync(customer);
                await _returnRequestService.UpdateReturnRequestAsync(rr);

                //notify store owner
                await _workflowMessageService.SendNewReturnRequestStoreOwnerNotificationAsync(rr, orderItem, order, _localizationSettings.DefaultAdminLanguageId);
                //notify customer
                await _workflowMessageService.SendNewReturnRequestCustomerNotificationAsync(rr, orderItem, order);

                count++;
            }
        }

        model = await _returnRequestModelFactory.PrepareSubmitReturnRequestModelAsync(model, order);
        if (count > 0)
            model.Result = _returnRequestSettings.UseEuWithdrawalLocales ? await _localizationService.GetResourceAsync("ReturnRequests.Withdrawal.Submitted") :
                await _localizationService.GetResourceAsync("ReturnRequests.Submitted");
        else
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("ReturnRequests.NoItemsSubmitted"));

        return View(model);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public virtual async Task<IActionResult> UploadFileReturnRequest()
    {
        if (!_returnRequestSettings.ReturnRequestsEnabled || !_returnRequestSettings.ReturnRequestsAllowFiles)
        {
            return Json(new
            {
                success = false,
                downloadGuid = Guid.Empty,
            });
        }

        var httpPostedFile = await Request.GetFirstOrDefaultFileAsync();
        if (httpPostedFile == null)
        {
            return Json(new
            {
                success = false,
                message = "No file uploaded",
                downloadGuid = Guid.Empty,
            });
        }

        var fileBinary = await _downloadService.GetDownloadBitsAsync(httpPostedFile);

        var fileName = httpPostedFile.FileName;

        //remove path (passed in IE)
        fileName = _fileProvider.GetFileName(fileName);

        var contentType = httpPostedFile.ContentType;

        var fileExtension = _fileProvider.GetFileExtension(fileName);
        if (!string.IsNullOrEmpty(fileExtension))
            fileExtension = fileExtension.ToLowerInvariant();

        var validationFileMaximumSize = _returnRequestSettings.ReturnRequestsFileMaximumSize;
        if (validationFileMaximumSize > 0)
        {
            //compare in bytes
            var maxFileSizeBytes = validationFileMaximumSize * 1024;
            if (fileBinary.Length > maxFileSizeBytes)
            {
                return Json(new
                {
                    success = false,
                    message = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumUploadedFileSize"), validationFileMaximumSize),
                    downloadGuid = Guid.Empty,
                });
            }
        }

        var download = new Download
        {
            DownloadGuid = Guid.NewGuid(),
            UseDownloadUrl = false,
            DownloadUrl = "",
            DownloadBinary = fileBinary,
            ContentType = contentType,
            //we store filename without extension for downloads
            Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
            Extension = fileExtension,
            IsNew = true
        };
        await _downloadService.InsertDownloadAsync(download);

        //when returning JSON the mime-type must be set to text/plain
        //otherwise some browsers will pop-up a "Save As" dialog.
        return Json(new
        {
            success = true,
            message = await _localizationService.GetResourceAsync("ShoppingCart.FileUploaded"),
            downloadUrl = Url.RouteUrl(NopRouteNames.Standard.DOWNLOAD_GET_FILE_UPLOAD, new { downloadId = download.DownloadGuid }),
            downloadGuid = download.DownloadGuid,
        });
    }

    #endregion
}