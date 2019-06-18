using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FuraFila.Payments.MercadoPago.Services;
using Nop.Core;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.MercadoPago;
using Nop.Plugin.Payments.MercadoPago.Exceptions;
using Nop.Plugin.Payments.MercadoPago.FuraFila;
using Nop.Plugin.Payments.MercadoPago.FuraFila.Models;
using Nop.Plugin.Payments.MercadoPago.FuraFila.Payments;
using Nop.Plugin.Payments.MercadoPago.FuraFila.Preferences;
using Nop.Services.Configuration;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;

namespace Nop.Plugin.Payments.MercadoPago.FuraFila
{
    public class MPPaymentService : IMPPaymentService
    {
        private const string CURRENCY_CODE = "BRL";

        private readonly MPHttpService _service;

        public MPPaymentService(MPHttpService service)
        {
            _service = service;
        }

        private string GetExternalReferenceFromOrder(Core.Domain.Orders.Order order)
        {
            return $"NP-{order.StoreId}-{order.OrderGuid}";
        }

        public async Task<Uri> CreatePaymentRequest(PostProcessPaymentRequest postProcessPaymentRequest, MercadoPagoPaymentSettings settings, CancellationToken cancellationToken = default)
        {
            if (postProcessPaymentRequest == null)
                throw new ArgumentNullException(nameof(postProcessPaymentRequest));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!settings.IsSetup)
                throw new SettingNotConfiguredException();

            var payment = new PreferenceRequest();

            LoadingItems(postProcessPaymentRequest, payment);
            LoadingShipping(postProcessPaymentRequest, payment);
            LoadingSender(postProcessPaymentRequest, payment);

            payment.AutoReturn = AutoReturn.ALL;

            payment.ExternalReference = GetExternalReferenceFromOrder(postProcessPaymentRequest.Order);
            payment.AdditionalInfo = $"NOP-STORE-{postProcessPaymentRequest.Order.StoreId}-ORDER{postProcessPaymentRequest.Order.Id}";


            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            string storeLocation = webHelper.GetStoreLocation();

            payment.BackUrls = new BackUrls();
            payment.BackUrls.Success = payment.BackUrls.Pending = payment.BackUrls.Failure = $"{storeLocation}/checkout/completed/{postProcessPaymentRequest.Order.Id}";

            string accessToken = settings.GetAccessTokenEnvironment();
            var preferenceResponse = await _service.CreatePaymentPreference(payment, accessToken, cancellationToken);

            return GetRedirectUri(preferenceResponse.InitPoint, preferenceResponse.SandboxInitPoint, settings.UseSandbox);
        }

        private Uri GetRedirectUri(string initPoint, string initPointSandbox, bool isSandbox)
        {
            return isSandbox ? new Uri(initPointSandbox) : new Uri(initPoint);
        }

        private void LoadingSender(PostProcessPaymentRequest postProcessPaymentRequest, PreferenceRequest payment)
        {
            payment.Payer = new Payer();
            payment.Payer.Email = postProcessPaymentRequest.Order.Customer.Email;

            payment.Payer.Name = postProcessPaymentRequest.Order.BillingAddress.FirstName;
            payment.Payer.Surname = postProcessPaymentRequest.Order.BillingAddress.LastName;

            payment.Payer.Email = postProcessPaymentRequest.Order.Customer.Email;

            payment.Payer.Address = new Address();
            payment.Payer.Address.ZipCode = postProcessPaymentRequest.Order.BillingAddress.ZipPostalCode;
        }

        private void LoadingShipping(PostProcessPaymentRequest postProcessPaymentRequest, PreferenceRequest payment)
        {
            payment.Shipments = new Shipment
            {
                Mode = ShipmentModes.NOT_SPECIFIED
            };

            var adress = new Address();
            if (postProcessPaymentRequest.Order.ShippingAddress != null)
            {
                adress.ZipCode = postProcessPaymentRequest.Order.ShippingAddress.ZipPostalCode;
                adress.StreetName = postProcessPaymentRequest.Order.ShippingAddress.Address1;
            }
            payment.Shipments.Cost = Math.Round(postProcessPaymentRequest.Order.OrderShippingInclTax, 2);
        }

        private void LoadingItems(PostProcessPaymentRequest postProcessPaymentRequest, PreferenceRequest payment)
        {
            payment.Items = new List<Item>();
            foreach (var product in postProcessPaymentRequest.Order.OrderItems)
            {
                var item = new Item();

                item.Id = product.Id.ToString();

                item.Title = product.Product.Name;
                item.Description = product.Product.ShortDescription;


                item.UnitPrice = Math.Round(product.UnitPriceInclTax, 2);
                item.Quantity = product.Quantity;
                item.CurrencyId = CURRENCY_CODE;

                payment.Items.Add(item);
            }
        }

        private IEnumerable<Core.Domain.Orders.Order> GetPendingOrders(IOrderService orderService, Store store, IOrderProcessingService orderProcessingService)
        {
            return orderService.SearchOrders(storeId: store.Id,
                paymentMethodSystemName: MercadoPagoPaymentProcessor.PAYMENT_METHOD_NAME,
                psIds: new List<int>() { 10 })
                    .Where(o => orderProcessingService.CanMarkOrderAsPaid(o)
                );
        }

        private async Task<Payment> GetTransaction(MercadoPagoPaymentSettings settings, string referenceCode, CancellationToken cancellationToken = default)
        {
            var svc = EngineContext.Current.Resolve<MPHttpService>();

            var rq = new SearchPaymentRequest { ExternalReference = referenceCode };

            string accessToken = settings.GetAccessTokenEnvironment();
            var result = await svc.SearchByReference(rq, accessToken, cancellationToken);

            if (result != null)
                return result.Results.FirstOrDefault(x => string.Compare(referenceCode, x.ExternalReference, true) == 0);
            return null;
        }

        private bool TransactionIsPaid(Payment transaction) => (string.Compare(PaymentStatuses.APPROVED, transaction?.Status, true) == 0);

        public async Task CheckPayments(CancellationToken cancellationToken = default)
        {
            var storeService = EngineContext.Current.Resolve<IStoreService>();
            var storesToSyncColl = storeService.GetAllStores();

            var settingService = EngineContext.Current.Resolve<ISettingService>();
            var orderService = EngineContext.Current.Resolve<IOrderService>();
            var orderProcessingSvc = EngineContext.Current.Resolve<IOrderProcessingService>();

            foreach (var store in storesToSyncColl)
            {
                var storeSetting = settingService.LoadSetting<MercadoPagoPaymentSettings>(store.Id);
                await SyncStore(store, storeSetting, orderService, orderProcessingSvc, cancellationToken);
            }
        }

        private async Task SyncStore(Store store, MercadoPagoPaymentSettings settings, IOrderService orderService, IOrderProcessingService orderProcessingSvc, CancellationToken cancellationToken = default)
        {
            if (settings.IsSetup)
            {
                foreach (var order in GetPendingOrders(orderService, store, orderProcessingSvc))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string referenceId = GetExternalReferenceFromOrder(order);
                    var transaction = await GetTransaction(settings, referenceId, cancellationToken);

                    if (TransactionIsPaid(transaction))
                    {
                        orderProcessingSvc.MarkOrderAsPaid(order);
                    }
                }
            }
        }
    }
}
