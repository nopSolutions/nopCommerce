using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Http.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Payments;
using NUnit.Framework;


namespace Nop.Tests;
internal class PaymentServiceTest
{
    [TestFixture]
    public class PaymentServiceTests
    {
        private PaymentService _paymentService;
        private Mock<ICustomerService> _customerServiceMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IPaymentPluginManager> _paymentPluginManagerMock;
        private Mock<IPriceCalculationService> _priceCalculationServiceMock;
        private PaymentSettings _paymentSettings;
        private ShoppingCartSettings _shoppingCartSettings;

        [SetUp]
        public void SetUp()
        {
            _customerServiceMock = new Mock<ICustomerService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _paymentPluginManagerMock = new Mock<IPaymentPluginManager>();
            _priceCalculationServiceMock = new Mock<IPriceCalculationService>();
            _paymentSettings = new PaymentSettings();
            _shoppingCartSettings = new ShoppingCartSettings();

            _paymentService = new PaymentService(
                _customerServiceMock.Object,
                _httpContextAccessorMock.Object,
                _paymentPluginManagerMock.Object,
                _priceCalculationServiceMock.Object,
                _paymentSettings,
                _shoppingCartSettings
            );
        }

        [Test]
        public async Task ProcessPaymentAsync_OrderTotalIsZero_ShouldReturnPaidStatus()
        {
            // Arrange
            var processPaymentRequest = new ProcessPaymentRequest
            {
                OrderTotal = decimal.Zero
            };

            // Act
            var result = await _paymentService.ProcessPaymentAsync(processPaymentRequest);

            // Assert
            Assert.AreEqual(PaymentStatus.Paid, result.NewPaymentStatus);
        }

        [Test]
        public async Task ProcessPaymentAsync_ValidRequest_ShouldCallPaymentMethodProcessPaymentAsync()
        {
            // Arrange
            var processPaymentRequest = new ProcessPaymentRequest
            {
                OrderTotal = 100.00m,
                CustomerId = 1,
                PaymentMethodSystemName = "TestPaymentMethod",
                StoreId = 1
            };

            var customer = new Customer { Id = 1 };
            _customerServiceMock.Setup(x => x.GetCustomerByIdAsync(processPaymentRequest.CustomerId)).ReturnsAsync(customer);

            var paymentMethodMock = new Mock<IPaymentMethod>();
            _paymentPluginManagerMock.Setup(x => x.LoadPluginBySystemNameAsync(processPaymentRequest.PaymentMethodSystemName, customer, processPaymentRequest.StoreId))
                                     .ReturnsAsync(paymentMethodMock.Object);

            // Act
            await _paymentService.ProcessPaymentAsync(processPaymentRequest);

            // Assert
            paymentMethodMock.Verify(x => x.ProcessPaymentAsync(processPaymentRequest), Times.Once);
        }

        // Add more test cases for other methods in the PaymentService class as needed

        [Test]
        public async Task PostProcessPaymentAsync_OrderAlreadyPaid_ShouldNotCallPaymentMethodPostProcessPaymentAsync()
        {
            // Arrange
            var postProcessPaymentRequest = new PostProcessPaymentRequest
            {
                Order = new Order { PaymentStatus = PaymentStatus.Paid }
            };

            // Act
            await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

            // Assert
            _paymentPluginManagerMock.Verify(x => x.LoadPluginBySystemNameAsync(It.IsAny<string>(), It.IsAny<Customer>(), It.IsAny<int>()), Times.Never);
        }
        [Test]
        public async Task PostProcessPaymentAsync_ValidRequest_ShouldCallPaymentMethodPostProcessPaymentAsync()
        {
            // Arrange
            var postProcessPaymentRequest = new PostProcessPaymentRequest
            {
                Order = new Order { PaymentStatus = PaymentStatus.Pending, CustomerId = 1, PaymentMethodSystemName = "TestPaymentMethod", StoreId = 1 }
            };

            var customer = new Customer { Id = 1 };
            _customerServiceMock.Setup(x => x.GetCustomerByIdAsync(postProcessPaymentRequest.Order.CustomerId)).ReturnsAsync(customer);

            var paymentMethodMock = new Mock<IPaymentMethod>();
            _paymentPluginManagerMock.Setup(x => x.LoadPluginBySystemNameAsync(postProcessPaymentRequest.Order.PaymentMethodSystemName, customer, postProcessPaymentRequest.Order.StoreId))
                                     .ReturnsAsync(paymentMethodMock.Object);

            // Act
            await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

            // Assert
            paymentMethodMock.Verify(x => x.PostProcessPaymentAsync(postProcessPaymentRequest), Times.Once);
        }
     

        [Test]
        public async Task GetAdditionalHandlingFeeAsync_ValidRequest_ShouldCallPaymentMethodGetAdditionalHandlingFeeAsync()
        {
            // Arrange
            var cart = new List<ShoppingCartItem> { new ShoppingCartItem { CustomerId = 1 } };
            var paymentMethodMock = new Mock<IPaymentMethod>();
            _paymentPluginManagerMock.Setup(x => x.LoadPluginBySystemNameAsync(It.IsAny<string>(), It.IsAny<Customer>(), It.IsAny<int>()))
                                     .ReturnsAsync(paymentMethodMock.Object);

            // Act
            await _paymentService.GetAdditionalHandlingFeeAsync(cart, "TestPaymentMethod");

            // Assert
            paymentMethodMock.Verify(x => x.GetAdditionalHandlingFeeAsync(cart), Times.Once);
        }

        [Test]
        public async Task GetAdditionalHandlingFeeAsync_NullPaymentMethodSystemName_ShouldReturnZero()
        {
            // Arrange
            var cart = new List<ShoppingCartItem> { new ShoppingCartItem { CustomerId = 1 } };

            // Act
            var result = await _paymentService.GetAdditionalHandlingFeeAsync(cart, null);

            // Assert
            Assert.AreEqual(decimal.Zero, result);
        }
       

        [Test]
        public async Task ProcessRecurringPaymentAsync_OrderTotalIsZero_ShouldReturnPaidStatus()
        {
            // Arrange
            var processPaymentRequest = new ProcessPaymentRequest
            {
                OrderTotal = decimal.Zero
            };

            // Act
            var result = await _paymentService.ProcessRecurringPaymentAsync(processPaymentRequest);

            // Assert
            Assert.AreEqual(PaymentStatus.Paid, result.NewPaymentStatus);
        }


        [Test]
        public void GetMaskedCreditCardNumber_NullOrEmptyCreditCardNumber_ShouldReturnEmptyString()
        {
            // Arrange
            string creditCardNumber = null;

            // Act
            var result = _paymentService.GetMaskedCreditCardNumber(creditCardNumber);

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void GetMaskedCreditCardNumber_CreditCardNumberLessThanOrEqualToFourDigits_ShouldReturnOriginalNumber()
        {
            // Arrange
            string creditCardNumber = "1234";

            // Act
            var result = _paymentService.GetMaskedCreditCardNumber(creditCardNumber);

            // Assert
            Assert.AreEqual(creditCardNumber, result);
        }

        [Test]
        public void GetMaskedCreditCardNumber_ValidCreditCardNumber_ShouldReturnMaskedNumber()
        {
            // Arrange
            string creditCardNumber = "1234567890123456";

            // Act
            var result = _paymentService.GetMaskedCreditCardNumber(creditCardNumber);

            // Assert
            Assert.AreEqual("************3456", result);
        }


        [Test]
        public async Task SerializeCustomValues_EmptyCustomValues_ShouldReturnNull()
        {
            // Arrange
            var processPaymentRequest = new ProcessPaymentRequest();

            // Act
            var result = _paymentService.SerializeCustomValues(processPaymentRequest);

            // Assert
            Assert.IsNull(result);
        }

        

        
        [Test]
        public async Task GenerateOrderGuidAsync_ValidProcessPaymentRequest_ShouldSetOrderGuid()
        {
            // Arrange
            var processPaymentRequest = new ProcessPaymentRequest();

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var httpContextMock = new Mock<HttpContext>();
            var sessionMock = new Mock<ISession>();

            httpContextMock.Setup(x => x.Session).Returns(sessionMock.Object);
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

            // Other mock setups...

            var paymentService = new PaymentService(
                _customerServiceMock.Object,
                httpContextAccessorMock.Object,
                _paymentPluginManagerMock.Object,
                _priceCalculationServiceMock.Object,
                _paymentSettings,
                _shoppingCartSettings
            );

            // Act
            await paymentService.GenerateOrderGuidAsync(processPaymentRequest);

            // Assert
            Assert.AreNotEqual(Guid.Empty, processPaymentRequest.OrderGuid);
            Assert.IsNotNull(processPaymentRequest.OrderGuidGeneratedOnUtc);
        }



      
      

        [Test]
        public void DeserializeCustomValues_EmptyCustomValuesXml_ShouldReturnEmptyDictionary()
        {
            // Arrange
            var order = new Order { CustomValuesXml = string.Empty };

            // Act
            var result = _paymentService.DeserializeCustomValues(order);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }



    }


}
