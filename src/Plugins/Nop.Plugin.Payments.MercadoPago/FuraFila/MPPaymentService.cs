using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FuraFila.Payments.MercadoPago.Services;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.MercadoPago;
using Nop.Plugin.Payments.MercadoPago.FuraFila;
using Nop.Plugin.Payments.MercadoPago.FuraFila.Models;
using Nop.Plugin.Payments.MercadoPago.FuraFila.Payments;
using Nop.Plugin.Payments.MercadoPago.FuraFila.Preferences;
using Nop.Services.Orders;
using Nop.Services.Payments;

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

        public async Task<Uri> CreatePaymentRequest(PostProcessPaymentRequest postProcessPaymentRequest, MercadoPagoPaymentSettings settings, CancellationToken cancellationToken = default)
        {
            var payment = new PreferenceRequest();

            LoadingItems(postProcessPaymentRequest, payment);
            LoadingShipping(postProcessPaymentRequest, payment);
            LoadingSender(postProcessPaymentRequest, payment);

            payment.AutoReturn = AutoReturn.ALL;

            payment.ExternalReference = $"NP-{postProcessPaymentRequest.Order.StoreId}-{postProcessPaymentRequest.Order.Id}";
            payment.AdditionalInfo = $"NOP-STORE-{postProcessPaymentRequest.Order.StoreId}-ORDER{postProcessPaymentRequest.Order.Id}";


            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            string storeLocation = webHelper.GetStoreLocation();

            payment.BackUrls = new BackUrls();
            payment.BackUrls.Success = $"{storeLocation}/checkout/completed/{postProcessPaymentRequest.Order.Id}";
            payment.BackUrls.Pending = $"{storeLocation}/checkout/completed/{postProcessPaymentRequest.Order.Id}";
            payment.BackUrls.Failure = $"{storeLocation}/checkout/completed/{postProcessPaymentRequest.Order.Id}";

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


        private IEnumerable<Core.Domain.Orders.Order> GetPendingOrders(IOrderService orderService, IStoreContext storeContext, IOrderProcessingService orderProcessingService)
        {
            return orderService.SearchOrders(storeId: storeContext.CurrentStore.Id,
                paymentMethodSystemName: "Payments.MercadoPago",
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
            var settings = EngineContext.Current.Resolve<MercadoPagoPaymentSettings>();
            var store = EngineContext.Current.Resolve<IStoreContext>();
            var orderService = EngineContext.Current.Resolve<IOrderService>();
            var orderPorcessingSvc = EngineContext.Current.Resolve<IOrderProcessingService>();

            foreach (var order in GetPendingOrders(orderService, store, orderPorcessingSvc))
            {
                if (TransactionIsPaid(await GetTransaction(settings, order.Id.ToString())))
                    orderPorcessingSvc.MarkOrderAsPaid(order);
            }
        }
    }
}
