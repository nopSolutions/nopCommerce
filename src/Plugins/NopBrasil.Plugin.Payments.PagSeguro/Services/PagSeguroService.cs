using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uol.PagSeguro;

namespace NopBrasil.Plugin.Payments.PagSeguro.Services
{
    public class PagSeguroService : IPagSeguroService
    {
        //todo: colocar a moeda utilizada como configuração
        private const string CURRENCY_CODE = "BRL";

        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly PagSeguroPaymentSetting _pagSeguroPaymentSetting;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IStoreContext _storeContext;

        public PagSeguroService(ICurrencyService currencyService,
            CurrencySettings currencySettings,
            PagSeguroPaymentSetting pagSeguroPaymentSetting,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            IStoreContext storeContext)
        {
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
            _currencySettings = currencySettings ?? throw new ArgumentNullException(nameof(currencySettings));
            _pagSeguroPaymentSetting = pagSeguroPaymentSetting ?? throw new ArgumentNullException(nameof(pagSeguroPaymentSetting));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _orderProcessingService = orderProcessingService ?? throw new ArgumentNullException(nameof(orderProcessingService));
            _storeContext = storeContext ?? throw new ArgumentNullException(nameof(storeContext));
        }

        [Obsolete("Use async version instead")]
        public Uri CreatePayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            // Seta as credenciais
            var credentials = new AccountCredentials(_pagSeguroPaymentSetting.PagSeguroEmail,
                _pagSeguroPaymentSetting.PagSeguroToken,
                _pagSeguroPaymentSetting.IsSandbox);

            var payment = new PaymentRequest
            {
                Currency = CURRENCY_CODE,
                Reference = postProcessPaymentRequest.Order.Id.ToString()
            };

            LoadingItems(postProcessPaymentRequest, payment);
            LoadingShipping(postProcessPaymentRequest, payment);
            LoadingSender(postProcessPaymentRequest, payment);

            return Uol.PagSeguro.PaymentService.Register(credentials, payment);
        }

        public Task<Uri> CreatePaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest, CancellationToken cancellationToken = default)
        {
            // Seta as credenciais
            var credentials = new AccountCredentials(_pagSeguroPaymentSetting.PagSeguroEmail,
                _pagSeguroPaymentSetting.PagSeguroToken,
                _pagSeguroPaymentSetting.IsSandbox);

            var payment = new PaymentRequest
            {
                Currency = CURRENCY_CODE,
                Reference = postProcessPaymentRequest.Order.Id.ToString()
            };

            LoadingItems(postProcessPaymentRequest, payment);
            LoadingShipping(postProcessPaymentRequest, payment);
            LoadingSender(postProcessPaymentRequest, payment);

            return Uol.PagSeguro.PaymentService.RegisterAsync(credentials, payment);
        }

        private void LoadingSender(PostProcessPaymentRequest postProcessPaymentRequest, PaymentRequest payment)
        {
            payment.Sender = new Sender();
            payment.Sender.Email = postProcessPaymentRequest.Order.Customer.Email;
            payment.Sender.Name = $"{postProcessPaymentRequest.Order.BillingAddress.FirstName} {postProcessPaymentRequest.Order.BillingAddress.LastName}";
        }

        private decimal GetConvertedRate(decimal rate)
        {
            var usedCurrency = _currencyService.GetCurrencyByCode(CURRENCY_CODE);
            if (usedCurrency == null)
                throw new NopException($"PagSeguro payment service. Could not load \"{CURRENCY_CODE}\" currency");

            if (usedCurrency.CurrencyCode == _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode)
                return rate;
            else
                return _currencyService.ConvertFromPrimaryStoreCurrency(rate, usedCurrency);
        }

        private void LoadingShipping(PostProcessPaymentRequest postProcessPaymentRequest, PaymentRequest payment)
        {
            payment.Shipping = new Shipping
            {
                ShippingType = ShippingType.NotSpecified
            };
            var adress = new Address
            {
                Complement = string.Empty,
                District = string.Empty,
                Number = string.Empty
            };
            if (postProcessPaymentRequest.Order.ShippingAddress != null)
            {
                adress.City = postProcessPaymentRequest.Order.ShippingAddress.City;
                adress.Country = postProcessPaymentRequest.Order.ShippingAddress.Country.Name;
                adress.PostalCode = postProcessPaymentRequest.Order.ShippingAddress.ZipPostalCode;
                adress.State = postProcessPaymentRequest.Order.ShippingAddress.StateProvince.Name;
                adress.Street = postProcessPaymentRequest.Order.ShippingAddress.Address1;
            }
            payment.Shipping.Cost = Math.Round(GetConvertedRate(postProcessPaymentRequest.Order.OrderShippingInclTax), 2);
        }

        private void LoadingItems(PostProcessPaymentRequest postProcessPaymentRequest, PaymentRequest payment)
        {
            foreach (var product in postProcessPaymentRequest.Order.OrderItems)
            {
                var item = new Item();
                item.Amount = Math.Round(GetConvertedRate(product.UnitPriceInclTax), 2);
                item.Description = product.Product.Name;
                item.Id = product.Id.ToString();
                item.Quantity = product.Quantity;
                if (product.ItemWeight.HasValue)
                    item.Weight = Convert.ToInt64(product.ItemWeight);
                payment.Items.Add(item);
            }
        }

        private IEnumerable<Order> GetPendingOrders() => _orderService.SearchOrders(_storeContext.CurrentStore.Id, paymentMethodSystemName: "Payments.PagSeguro", psIds: new List<int>() { 10 }).Where(o => _orderProcessingService.CanMarkOrderAsPaid(o));

        private TransactionSummary GetTransaction(AccountCredentials credentials, string referenceCode) => TransactionSearchService.SearchByReference(credentials, referenceCode)?.Items?.FirstOrDefault();

        private bool TransactionIsPaid(TransactionSummary transaction) => (transaction?.TransactionStatus == TransactionStatus.Paid || transaction?.TransactionStatus == TransactionStatus.Available);

        public void CheckPayments()
        {
            var credentials = new AccountCredentials(_pagSeguroPaymentSetting.PagSeguroEmail, _pagSeguroPaymentSetting.PagSeguroToken, _pagSeguroPaymentSetting.IsSandbox);
            foreach (var order in GetPendingOrders())
                if (TransactionIsPaid(GetTransaction(credentials, order.Id.ToString())))
                    _orderProcessingService.MarkOrderAsPaid(order);
        }
    }
}