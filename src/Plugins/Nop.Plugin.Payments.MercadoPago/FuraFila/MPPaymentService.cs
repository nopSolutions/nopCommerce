using System;
using System.Threading;
using System.Threading.Tasks;
using FuraFila.Payments.MercadoPago.Services;
using Nop.Plugin.Payments.MercadoPago;
using Nop.Plugin.Payments.MercadoPago.FuraFila;
using Nop.Plugin.Payments.MercadoPago.FuraFila.Models;
using Nop.Plugin.Payments.MercadoPago.FuraFila.Preferences;
using Nop.Services.Payments;

namespace FuraFila.Payments.MercadoPago
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

            string accessToken = settings.UseSandbox ? settings.AccessTokenSandbox : settings.AccessToken;
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
    }
}
