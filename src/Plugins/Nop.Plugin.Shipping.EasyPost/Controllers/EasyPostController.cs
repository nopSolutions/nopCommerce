using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
using Nop.Plugin.Shipping.EasyPost.Domain.Batch;
using Nop.Plugin.Shipping.EasyPost.Domain.Shipment;
using Nop.Plugin.Shipping.EasyPost.Factories;
using Nop.Plugin.Shipping.EasyPost.Models.Batch;
using Nop.Plugin.Shipping.EasyPost.Models.Configuration;
using Nop.Plugin.Shipping.EasyPost.Models.Pickup;
using Nop.Plugin.Shipping.EasyPost.Models.Shipment;
using Nop.Plugin.Shipping.EasyPost.Services;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Shipping.EasyPost.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class EasyPostController : BasePluginController
    {
        #region Fields

        private readonly EasyPostModelFactory _easyPostModelFactory;
        private readonly EasyPostService _easyPostService;
        private readonly EasyPostSettings _easyPostSettings;
        private readonly ICurrencyService _currencyService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly INopFileProvider _nopFileProvider;
        private readonly INotificationService _notificationService;
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IShipmentService _shipmentService;
        private readonly ShippingSettings _shippingSettings;

        #endregion

        #region Ctor

        public EasyPostController(EasyPostModelFactory easyPostModelFactory,
            EasyPostService easyPostService,
            EasyPostSettings easyPostSettings,
            ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            IHttpClientFactory httpClientFactory,
            ILocalizationService localizationService,
            IMeasureService measureService,
            INopFileProvider nopFileProvider,
            INotificationService notificationService,
            IOrderModelFactory orderModelFactory,
            IPermissionService permissionService,
            ISettingService settingService,
            IShipmentService shipmentService,
            ShippingSettings shippingSettings)
        {
            _easyPostModelFactory = easyPostModelFactory;
            _easyPostService = easyPostService;
            _easyPostSettings = easyPostSettings;
            _currencyService = currencyService;
            _dateTimeHelper = dateTimeHelper;
            _httpClientFactory = httpClientFactory;
            _localizationService = localizationService;
            _measureService = measureService;
            _nopFileProvider = nopFileProvider;
            _notificationService = notificationService;
            _orderModelFactory = orderModelFactory;
            _permissionService = permissionService;
            _settingService = settingService;
            _shipmentService = shipmentService;
            _shippingSettings = shippingSettings;
        }

        #endregion

        #region Methods

        #region Configuration

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                ApiKey = _easyPostSettings.ApiKey,
                TestApiKey = _easyPostSettings.TestApiKey,
                UseSandbox = _easyPostSettings.UseSandbox,
                UseAllAvailableCarriers = _easyPostSettings.UseAllAvailableCarriers,
                CarrierAccounts = _easyPostSettings.CarrierAccounts ?? new(),
                AddressVerification = _easyPostSettings.AddressVerification,
                StrictAddressVerification = _easyPostSettings.StrictAddressVerification
            };

            //get configured carrier accounts
            if (!_easyPostSettings.UseSandbox)
            {
                var (accounts, accountsError) = await _easyPostService.GetCarrierAccountsAsync();
                if (!string.IsNullOrEmpty(accountsError))
                {
                    //accounts are available only in production, so don't display errors in sandbox mode
                    var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Error");
                    var errorMessage = string.Format(locale, accountsError, Url.Action("List", "Log"));
                    _notificationService.ErrorNotification(errorMessage, false);
                }

                model.AvailableCarrierAccounts = accounts?.Select(account => new SelectListItem
                {
                    Value = account.id,
                    Text = account.readable ?? account.type,
                    Selected = _easyPostSettings.CarrierAccounts?.Contains(account.id) ?? false
                }).ToList() ?? new();
            }

            //check default currency availability
            var currency = await _currencyService.GetCurrencyByCodeAsync(EasyPostDefaults.CurrencyCode);
            if (currency is null)
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Configuration.Currency.Warning");
                var warningMessage = string.Format(locale, EasyPostDefaults.CurrencyCode, Url.Action("List", "Currency"));
                _notificationService.WarningNotification(warningMessage, false);
            }

            //check default dimension and weight measures availability
            var weight = await _measureService.GetMeasureWeightBySystemKeywordAsync(EasyPostDefaults.MeasureWeightSystemName);
            var dimension = await _measureService.GetMeasureDimensionBySystemKeywordAsync(EasyPostDefaults.MeasureDimensionSystemName);
            if (weight is null || dimension is null)
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Configuration.Measures.Warning");
                var warningMessage = string.Format(locale,
                    EasyPostDefaults.MeasureDimensionSystemName, EasyPostDefaults.MeasureWeightSystemName, Url.Action("List", "Measure"));
                _notificationService.WarningNotification(warningMessage, false);
            }

            return View("~/Plugins/Shipping.EasyPost/Views/Configuration/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            _easyPostSettings.ApiKey = model.ApiKey;
            _easyPostSettings.TestApiKey = model.TestApiKey;
            _easyPostSettings.UseSandbox = model.UseSandbox;
            _easyPostSettings.UseAllAvailableCarriers = model.UseAllAvailableCarriers;
            _easyPostSettings.CarrierAccounts = model.CarrierAccounts?.ToList();
            _easyPostSettings.AddressVerification = model.AddressVerification;
            _easyPostSettings.StrictAddressVerification = model.StrictAddressVerification;
            await _settingService.SaveSettingAsync(_easyPostSettings);

            //reset client configuration
            EasyPostService.ResetClientConfiguration();

            //create webhook
            var (webhook, webhookError) = await _easyPostService.CreateWebhookAsync();
            if (!string.IsNullOrEmpty(webhookError))
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Error");
                var errorMessage = string.Format(locale, webhookError, Url.Action("List", "Log"));
                _notificationService.ErrorNotification(errorMessage, false);
            }
            await _settingService.SetSettingAsync($"{nameof(EasyPostSettings)}.{nameof(EasyPostSettings.WebhookUrl)}", webhook?.url);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion

        #region Shipment

        [HttpPost]
        public async Task<IActionResult> UpdateShipment(ShipmentDetailsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
            if (shipment is not null)
            {
                string getAttributeValue(Enum enumValue) => enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault()
                    ?.GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault()
                    ?.Value is string value && value != "---" ? value : null;

                //prepare the details to update the shipment
                var request = new UpdateShipmentRequest
                {
                    OptionsDetails = new()
                    {
                        AdditionalHandling = model.AdditionalHandling,
                        Alcohol = model.Alcohol,
                        ByDrone = model.ByDrone,
                        CarbonNeutral = model.CarbonNeutral,
                        DeliveryConfirmation = getAttributeValue((DeliveryConfirmation)model.DeliveryConfirmation),
                        Endorsement = getAttributeValue((Endorsement)model.Endorsement),
                        HandlingInstructions = model.HandlingInstructions,
                        Hazmat = getAttributeValue((HazmatType)model.Hazmat),
                        InvoiceNumber = model.InvoiceNumber,
                        Machinable = model.Machinable,
                        PrintCustom1 = model.PrintCustom1,
                        PrintCustomCode1 = getAttributeValue((CustomCode)model.PrintCustomCode1),
                        PrintCustom2 = model.PrintCustom2,
                        PrintCustomCode2 = getAttributeValue((CustomCode)model.PrintCustomCode2),
                        PrintCustom3 = model.PrintCustom3,
                        PrintCustomCode3 = getAttributeValue((CustomCode)model.PrintCustomCode3),
                        SpecialRatesEligibility = getAttributeValue((SpecialRate)model.SpecialRatesEligibility),
                        CertifiedMail = model.CertifiedMail,
                        RegisteredMail = model.RegisteredMail,
                        RegisteredMailAmount = model.RegisteredMailAmount,
                        ReturnReceipt = model.ReturnReceipt
                    },
                    CustomsInfoDetails = !model.UseCustomsInfo ? null : new()
                    {
                        ContentsType = getAttributeValue((ContentsType)model.ContentsType),
                        RestrictionType = getAttributeValue((RestrictionType)model.RestrictionType),
                        NonDeliveryOption = getAttributeValue((NonDeliveryOption)model.NonDeliveryOption),
                        ContentsExplanation = model.ContentsExplanation,
                        RestrictionComments = model.RestrictionComments,
                        CustomsCertify = model.CustomsCertify,
                        CustomsSigner = model.CustomsSigner,
                        EelPfc = model.EelPfc
                    }
                };
                var (_, error) = await _easyPostService.UpdateShipmentAsync(shipment, request);
                if (!string.IsNullOrEmpty(error))
                {
                    var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Error");
                    _notificationService.ErrorNotification(string.Format(locale, error, Url.Action("List", "Log")), false);
                }
                else
                {
                    var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Success");
                    _notificationService.SuccessNotification(locale);
                }
            }

            return RedirectToAction("ShipmentDetails", "Order", new { id = shipment?.Id ?? 0 });
        }

        [HttpPost]
        public async Task<IActionResult> BuyLabel(ShipmentDetailsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
            if (shipment is not null)
            {
                var (_, error) = await _easyPostService.BuyLabelAsync(shipment, model.RateId, model.Insurance);
                if (!string.IsNullOrEmpty(error))
                {
                    var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Error");
                    _notificationService.ErrorNotification(string.Format(locale, error, Url.Action("List", "Log")), false);
                }
                else
                {
                    var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Success");
                    _notificationService.SuccessNotification(locale);
                }
            }

            return RedirectToAction("ShipmentDetails", "Order", new { id = shipment?.Id ?? 0 });
        }

        [HttpPost]
        public async Task<IActionResult> DownloadLabel(ShipmentDetailsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
            if (shipment is not null)
            {
                var ((downloadUrl, contentType), error) = await _easyPostService.DownloadLabelAsync(shipment, model.LabelFormat);
                if (!string.IsNullOrEmpty(error))
                {
                    var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Error");
                    _notificationService.ErrorNotification(string.Format(locale, error, Url.Action("List", "Log")), false);
                }
                else
                {
                    //return file
                    var filename = _nopFileProvider.GetFileName(downloadUrl);
                    var client = _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
                    var stream = await client.GetStreamAsync(downloadUrl);
                    return File(stream, contentType, filename, true);
                }
            }

            return RedirectToAction("ShipmentDetails", "Order", new { id = shipment?.Id ?? 0 });
        }

        [HttpPost]
        public async Task<IActionResult> DownloadInvoice(ShipmentDetailsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = await _shipmentService.GetShipmentByIdAsync(model.Id);
            if (shipment is not null)
            {
                var ((downloadUrl, contentType), error) = await _easyPostService.DownloadInvoiceAsync(shipment);
                if (!string.IsNullOrEmpty(error))
                {
                    var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Error");
                    _notificationService.ErrorNotification(string.Format(locale, error, Url.Action("List", "Log")), false);
                }
                else
                {
                    //return file
                    var filename = _nopFileProvider.GetFileName(downloadUrl);
                    var client = _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
                    var stream = await client.GetStreamAsync(downloadUrl);
                    return File(stream, contentType, filename, true);
                }
            }

            return RedirectToAction("ShipmentDetails", "Order", new { id = shipment?.Id ?? 0 });
        }

        #endregion

        #region Pickup

        [HttpPost]
        public async Task<IActionResult> CreatePickup(PickupModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = model.ShipmentId.HasValue ? await _shipmentService.GetShipmentByIdAsync(model.ShipmentId.Value) : null;
            var batch = model.BatchId.HasValue ? await _easyPostService.GetBatchByIdAsync(model.BatchId.Value) : null;

            //prepare the details to create a pickup
            var request = new CreatePickupRequest
            {
                Address = model.PickupAddress?.ToEntity<Address>() ?? new(),
                Instructions = model.Instructions,
                MaxDate = model.MaxDate,
                MinDate = model.MinDate
            };
            var (_, error) = await _easyPostService.CreatePickupAsync(shipment, batch, request);
            if (!string.IsNullOrEmpty(error))
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Error");
                _notificationService.ErrorNotification(string.Format(locale, error, Url.Action("List", "Log")), false);
            }
            else
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Success");
                _notificationService.SuccessNotification(locale);
            }

            if (batch is not null)
                return RedirectToAction("BatchEdit", "EasyPost", new { id = batch.Id });

            return RedirectToAction("ShipmentDetails", "Order", new { id = shipment?.Id ?? 0 });
        }

        [HttpPost]
        public async Task<IActionResult> BuyPickup(PickupModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = model.ShipmentId.HasValue ? await _shipmentService.GetShipmentByIdAsync(model.ShipmentId.Value) : null;
            var batch = model.BatchId.HasValue ? await _easyPostService.GetBatchByIdAsync(model.BatchId.Value) : null;

            var (_, error) = await _easyPostService.BuyPickupAsync(shipment, batch, model.PickupRateId);
            if (!string.IsNullOrEmpty(error))
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Error");
                _notificationService.ErrorNotification(string.Format(locale, error, Url.Action("List", "Log")), false);
            }
            else
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Success");
                _notificationService.SuccessNotification(locale);
            }

            if (batch is not null)
                return RedirectToAction("BatchEdit", "EasyPost", new { id = batch.Id });

            return RedirectToAction("ShipmentDetails", "Order", new { id = shipment?.Id ?? 0 });
        }

        [HttpPost]
        public async Task<IActionResult> CancelPickup(PickupModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = model.ShipmentId.HasValue ? await _shipmentService.GetShipmentByIdAsync(model.ShipmentId.Value) : null;
            var batch = model.BatchId.HasValue ? await _easyPostService.GetBatchByIdAsync(model.BatchId.Value) : null;

            var (_, error) = await _easyPostService.CancelPickupAsync(shipment, batch);
            if (!string.IsNullOrEmpty(error))
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Error");
                _notificationService.ErrorNotification(string.Format(locale, error, Url.Action("List", "Log")), false);
            }
            else
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Success");
                _notificationService.SuccessNotification(locale);
            }

            if (batch is not null)
                return RedirectToAction("BatchEdit", "EasyPost", new { id = batch.Id });

            return RedirectToAction("ShipmentDetails", "Order", new { id = shipment?.Id ?? 0 });
        }

        #endregion

        #region Batch

        public async Task<IActionResult> BatchList()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var model = new BatchSearchModel();
            model.SetGridPageSize();
            model.AvailableStatuses = (await BatchStatus.Unknown.ToSelectListAsync(false, new[] { (int)BatchStatus.Unknown }, false))
                .Select(item => new SelectListItem(item.Text, item.Value))
                .ToList();
            var locale = await _localizationService.GetResourceAsync("Admin.Common.All");
            model.AvailableStatuses.Insert(0, new SelectListItem(locale, 0.ToString()));

            return View("~/Plugins/Shipping.EasyPost/Views/Batch/List.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> BatchList(BatchSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            var status = searchModel.StatusId > 0 ? (BatchStatus?)searchModel.StatusId : null;
            var batches = (await _easyPostService.GetAllBatchesAsync(status: status)).ToPagedList(searchModel);
            var model = await new BatchListModel().PrepareToGridAsync(searchModel, batches, () => batches.SelectAwait(async batch =>
            {
                return new BatchModel
                {
                    Id = batch.Id,
                    Status = CommonHelper.ConvertEnum(((BatchStatus)batch.StatusId).ToString()),
                    UpdatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(batch.UpdatedOnUtc, DateTimeKind.Utc),
                    CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(batch.CreatedOnUtc, DateTimeKind.Utc)
                };
            }));

            return Json(model);
        }

        public async Task<IActionResult> BatchCreate()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var batch = new EasyPostBatch
            {
                StatusId = (int)BatchStatus.Unknown,
                ShipmentIds = string.Empty,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            await _easyPostService.InsertBatchAsync(batch);

            return RedirectToAction("BatchEdit", new { id = batch.Id });
        }

        public async Task<IActionResult> BatchEdit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var batch = await _easyPostService.GetBatchByIdAsync(id);
            if (batch is null)
                return RedirectToAction("BatchList");

            var model = new BatchModel
            {
                Id = batch.Id,
                BatchId = batch.BatchId,
                BatchStatus = (BatchStatus)batch.StatusId,
                UpdatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(batch.UpdatedOnUtc, DateTimeKind.Utc),
                CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(batch.CreatedOnUtc, DateTimeKind.Utc),
                ManifestGenerated = !string.IsNullOrEmpty(batch.ManifestUrl)
            };
            model.Status = CommonHelper.ConvertEnum(model.BatchStatus.ToString());
            model.Purchased = model.BatchStatus == BatchStatus.Purchased ||
                model.BatchStatus == BatchStatus.LabelGenerating ||
                model.BatchStatus == BatchStatus.LabelGenerated;

            if (model.Purchased)
            {
                model.PickupModel = await _easyPostModelFactory
                    .PreparePickupModelAsync(model.PickupModel, null, batch, _shippingSettings.ShippingOriginAddressId);
                model.PickupStatus = model.PickupModel.Status;
            }

            model.BatchShipmentSearchModel.BatchId = batch.Id;
            model.BatchShipmentSearchModel.SetGridPageSize();

            return View("~/Plugins/Shipping.EasyPost/Views/Batch/Edit.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> BatchEdit(BatchModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var batch = await _easyPostService.GetBatchByIdAsync(model.Id);
            if (batch is null)
                return RedirectToAction("BatchList");

            var (_, error) = await _easyPostService.CreateOrUpdateBatchAsync(batch);
            if (!string.IsNullOrEmpty(error))
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Error");
                _notificationService.ErrorNotification(string.Format(locale, error, Url.Action("List", "Log")), false);
                return RedirectToAction("BatchList");
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Success"));

            return RedirectToAction("BatchEdit", new { id = batch.Id });

        }

        [HttpPost]
        public async Task<IActionResult> BatchDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var batch = await _easyPostService.GetBatchByIdAsync(id);
            if (batch is not null)
            {
                await _easyPostService.DeleteBatchAsync(batch);
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Success"));
            }

            return RedirectToAction("BatchList");
        }

        [HttpPost]
        public async Task<IActionResult> BatchShipmentList(BatchShipmentSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            var batch = await _easyPostService.GetBatchByIdAsync(searchModel.BatchId)
                ?? throw new ArgumentException("No batch found with the specified id");

            var (batchShipments, error) = await _easyPostService.GetBatchShipmentsAsync(batch);
            if (!string.IsNullOrEmpty(error))
                throw new NopException(error);

            var shipments = batchShipments.ToPagedList(searchModel);
            var model = new BatchShipmentListModel().PrepareToGrid(searchModel, shipments, () => shipments.Select(shipment =>
            {
                return new BatchShipmentModel
                {
                    Id = shipment.Id,
                    BatchId = shipment.BatchId,
                    ShipmentId = shipment.ShipmentId,
                    CustomOrderNumber = shipment.CustomOrderNumber,
                    TotalWeight = shipment.TotalWeight
                };
            }));

            return Json(model);
        }

        public async Task<IActionResult> BatchShipmentAdd(int batchId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var batch = await _easyPostService.GetBatchByIdAsync(batchId);
            if (batch is null)
                return RedirectToAction("BatchList");

            var model = await _orderModelFactory.PrepareShipmentSearchModelAsync(new ShipmentSearchModel());

            return View("~/Plugins/Shipping.EasyPost/Views/Batch/Shipments.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SelectShipments(int batchId, ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var batch = await _easyPostService.GetBatchByIdAsync(batchId)
                ?? throw new ArgumentException("No batch found with the specified id", nameof(batchId));

            var ids = batch.ShipmentIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(idValue => int.TryParse(idValue, out var id) ? id : 0)
                .Distinct()
                .ToList();
            ids.AddRange(selectedIds);
            batch.ShipmentIds = string.Join(',', ids.Distinct().OrderBy(value => value));
            await _easyPostService.UpdateBatchAsync(batch);

            return Json(new { Result = true });
        }

        public async Task<IActionResult> BatchShipmentDelete(int batchId, int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var batch = await _easyPostService.GetBatchByIdAsync(batchId)
                ?? throw new ArgumentException("No batch found with the specified id", nameof(batchId));

            var ids = batch.ShipmentIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(idValue => int.TryParse(idValue, out var id) ? id : 0)
                .Distinct()
                .ToList();
            ids.Remove(id);
            batch.ShipmentIds = string.Join(',', ids.Distinct().OrderBy(value => value));
            await _easyPostService.UpdateBatchAsync(batch);

            return RedirectToAction("BatchEdit", new { id = batch.Id });
        }

        [HttpPost]
        public async Task<IActionResult> BatchGenerateLabel(BatchModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var batch = await _easyPostService.GetBatchByIdAsync(model.Id);
            if (batch is null)
                return RedirectToAction("BatchList");

            var (_, error) = await _easyPostService.GenerateBatchLabelAsync(batch, model.LabelFormat);
            if (!string.IsNullOrEmpty(error))
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Error");
                _notificationService.ErrorNotification(string.Format(locale, error, Url.Action("List", "Log")), false);
                return RedirectToAction("BatchList");
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Success"));

            return RedirectToAction("BatchEdit", new { id = batch.Id });
        }

        [HttpPost]
        public async Task<IActionResult> BatchDownloadLabel(BatchModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var batch = await _easyPostService.GetBatchByIdAsync(model.Id);
            if (batch is not null)
            {
                var ((downloadUrl, contentType), error) = await _easyPostService.DownloadBatchLabelAsync(batch);
                if (!string.IsNullOrEmpty(error))
                {
                    var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Error");
                    _notificationService.ErrorNotification(string.Format(locale, error, Url.Action("List", "Log")), false);
                }
                else
                {
                    //return file
                    var filename = _nopFileProvider.GetFileName(downloadUrl);
                    var client = _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
                    var stream = await client.GetStreamAsync(downloadUrl);
                    return File(stream, contentType, filename, true);
                }
            }

            return RedirectToAction("BatchEdit", new { id = batch?.Id ?? 0 });
        }

        [HttpPost]
        public async Task<IActionResult> BatchGenerateManifest(BatchModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var batch = await _easyPostService.GetBatchByIdAsync(model.Id);
            if (batch is null)
                return RedirectToAction("BatchList");

            var (_, error) = await _easyPostService.GenerateBatchManifestAsync(batch);
            if (!string.IsNullOrEmpty(error))
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Error");
                _notificationService.ErrorNotification(string.Format(locale, error, Url.Action("List", "Log")), false);
                return RedirectToAction("BatchList");
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Success"));

            return RedirectToAction("BatchEdit", new { id = batch.Id });
        }

        [HttpPost]
        public async Task<IActionResult> BatchDownloadManifest(BatchModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var batch = await _easyPostService.GetBatchByIdAsync(model.Id);
            if (!string.IsNullOrEmpty(batch?.ManifestUrl))
            {
                //return file
                var filename = _nopFileProvider.GetFileName(batch.ManifestUrl);
                var client = _httpClientFactory.CreateClient(NopHttpDefaults.DefaultHttpClient);
                var stream = await client.GetStreamAsync(batch.ManifestUrl);
                return File(stream, MimeTypes.ApplicationPdf, filename, true);
            }

            return RedirectToAction("BatchEdit", new { id = batch?.Id ?? 0 });
        }

        #endregion

        #endregion
    }
}