using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Models.Order;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class ReturnRequestController : BasePublicController
    {
        #region Fields

        protected ICustomerService CustomerService { get; }
        protected ICustomNumberFormatter CustomNumberFormatter { get; }
        protected IDownloadService DownloadService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INopFileProvider FileProvider { get; }
        protected IOrderProcessingService OrderProcessingService { get; }
        protected IOrderService OrderService { get; }
        protected IReturnRequestModelFactory ReturnRequestModelFactory { get; }
        protected IReturnRequestService ReturnRequestService { get; }
        protected IStoreContext StoreContext { get; }
        protected IWorkContext WorkContext { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }
        protected LocalizationSettings LocalizationSettings { get; }
        protected OrderSettings OrderSettings { get; }

        #endregion

        #region Ctor

        public ReturnRequestController(ICustomerService customerService,
            ICustomNumberFormatter customNumberFormatter,
            IDownloadService downloadService,
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
            OrderSettings orderSettings)
        {
            CustomerService = customerService;
            CustomNumberFormatter = customNumberFormatter;
            DownloadService = downloadService;
            LocalizationService = localizationService;
            FileProvider = fileProvider;
            OrderProcessingService = orderProcessingService;
            OrderService = orderService;
            ReturnRequestModelFactory = returnRequestModelFactory;
            ReturnRequestService = returnRequestService;
            StoreContext = storeContext;
            WorkContext = workContext;
            WorkflowMessageService = workflowMessageService;
            LocalizationSettings = localizationSettings;
            OrderSettings = orderSettings;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> CustomerReturnRequests()
        {
            if (!await CustomerService.IsRegisteredAsync(await WorkContext.GetCurrentCustomerAsync()))
                return Challenge();

            var model = await ReturnRequestModelFactory.PrepareCustomerReturnRequestsModelAsync();
            return View(model);
        }

        public virtual async Task<IActionResult> ReturnRequest(int orderId)
        {
            var order = await OrderService.GetOrderByIdAsync(orderId);
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (order == null || order.Deleted || customer.Id != order.CustomerId)
                return Challenge();

            if (!await OrderProcessingService.IsReturnRequestAllowedAsync(order))
                return RedirectToRoute("Homepage");

            var model = new SubmitReturnRequestModel();
            model = await ReturnRequestModelFactory.PrepareSubmitReturnRequestModelAsync(model, order);
            return View(model);
        }

        [HttpPost, ActionName("ReturnRequest")]
        public virtual async Task<IActionResult> ReturnRequestSubmit(int orderId, SubmitReturnRequestModel model)
        {
            var order = await OrderService.GetOrderByIdAsync(orderId);
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (order == null || order.Deleted || customer.Id != order.CustomerId)
                return Challenge();

            if (!await OrderProcessingService.IsReturnRequestAllowedAsync(order))
                return RedirectToRoute("Homepage");

            var count = 0;

            var downloadId = 0;
            if (OrderSettings.ReturnRequestsAllowFiles)
            {
                var download = await DownloadService.GetDownloadByGuidAsync(model.UploadedFileGuid);
                if (download != null)
                    downloadId = download.Id;
            }

            var form = model.Form;

            //returnable products
            var orderItems = await OrderService.GetOrderItemsAsync(order.Id, isNotReturnable: false);
            foreach (var orderItem in orderItems)
            {
                var quantity = 0; //parse quantity
                foreach (var formKey in form.Keys)
                    if (formKey.Equals($"quantity{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _ = int.TryParse(form[formKey], out quantity);
                        break;
                    }
                if (quantity > 0)
                {
                    var rrr = await ReturnRequestService.GetReturnRequestReasonByIdAsync(model.ReturnRequestReasonId);
                    var rra = await ReturnRequestService.GetReturnRequestActionByIdAsync(model.ReturnRequestActionId);
                    var store = await StoreContext.GetCurrentStoreAsync();

                    var rr = new ReturnRequest
                    {
                        CustomNumber = "",
                        StoreId = store.Id,
                        OrderItemId = orderItem.Id,
                        Quantity = quantity,
                        CustomerId = customer.Id,
                        ReasonForReturn = rrr != null ? await LocalizationService.GetLocalizedAsync(rrr, x => x.Name) : "not available",
                        RequestedAction = rra != null ? await LocalizationService.GetLocalizedAsync(rra, x => x.Name) : "not available",
                        CustomerComments = model.Comments,
                        UploadedFileId = downloadId,
                        StaffNotes = string.Empty,
                        ReturnRequestStatus = ReturnRequestStatus.Pending,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow
                    };

                    await ReturnRequestService.InsertReturnRequestAsync(rr);

                    //set return request custom number
                    rr.CustomNumber = CustomNumberFormatter.GenerateReturnRequestCustomNumber(rr);
                    await CustomerService.UpdateCustomerAsync(customer);
                    await ReturnRequestService.UpdateReturnRequestAsync(rr);

                    //notify store owner
                    await WorkflowMessageService.SendNewReturnRequestStoreOwnerNotificationAsync(rr, orderItem, order, LocalizationSettings.DefaultAdminLanguageId);
                    //notify customer
                    await WorkflowMessageService.SendNewReturnRequestCustomerNotificationAsync(rr, orderItem, order);

                    count++;
                }
            }

            model = await ReturnRequestModelFactory.PrepareSubmitReturnRequestModelAsync(model, order);
            if (count > 0)
                model.Result = await LocalizationService.GetResourceAsync("ReturnRequests.Submitted");
            else
                model.Result = await LocalizationService.GetResourceAsync("ReturnRequests.NoItemsSubmitted");

            return View(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> UploadFileReturnRequest()
        {
            if (!OrderSettings.ReturnRequestsEnabled || !OrderSettings.ReturnRequestsAllowFiles)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty,
                });
            }

            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty,
                });
            }

            var fileBinary = await DownloadService.GetDownloadBitsAsync(httpPostedFile);

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = FileProvider.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = FileProvider.GetFileExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            var validationFileMaximumSize = OrderSettings.ReturnRequestsFileMaximumSize;
            if (validationFileMaximumSize > 0)
            {
                //compare in bytes
                var maxFileSizeBytes = validationFileMaximumSize * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    return Json(new
                    {
                        success = false,
                        message = string.Format(await LocalizationService.GetResourceAsync("ShoppingCart.MaximumUploadedFileSize"), validationFileMaximumSize),
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
                Filename = FileProvider.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            await DownloadService.InsertDownloadAsync(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = await LocalizationService.GetResourceAsync("ShoppingCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid,
            });
        }

        #endregion
    }
}