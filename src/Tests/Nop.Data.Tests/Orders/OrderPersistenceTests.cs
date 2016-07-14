using System;
using System.Linq;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Orders
{
    [TestFixture]
    public class OrderPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_order()
        {
            var order = new Order
            {
                OrderGuid = Guid.NewGuid(),
                StoreId = 1,
                Customer = GetTestCustomer(),
                OrderStatus = OrderStatus.Complete,
                ShippingStatus = ShippingStatus.Shipped,
                PaymentStatus = PaymentStatus.Paid,
                PaymentMethodSystemName = "PaymentMethodSystemName1",
                CustomerCurrencyCode = "RUR",
                CurrencyRate = 1.1M,
                CustomerTaxDisplayType = TaxDisplayType.ExcludingTax,
                VatNumber = "123456789",
                OrderSubtotalInclTax = 2.1M,
                OrderSubtotalExclTax = 3.1M,
                OrderSubTotalDiscountInclTax = 4.1M,
                OrderSubTotalDiscountExclTax = 5.1M,
                OrderShippingInclTax = 6.1M,
                OrderShippingExclTax = 7.1M,
                PaymentMethodAdditionalFeeInclTax = 8.1M,
                PaymentMethodAdditionalFeeExclTax = 9.1M,
                TaxRates = "1,3,5,7",
                OrderTax = 10.1M,
                OrderDiscount = 11.1M,
                OrderTotal = 12.1M,
                RefundedAmount  = 13.1M,
                RewardPointsWereAdded = true,
                CheckoutAttributeDescription = "CheckoutAttributeDescription1",
                CheckoutAttributesXml = "CheckoutAttributesXml1",
                CustomerLanguageId = 14,
                CustomerIp="CustomerIp1",
                AllowStoringCreditCardNumber= true,
                CardType= "Visa",
                CardName = "John Smith",
                CardNumber = "4111111111111111",
                MaskedCreditCardNumber= "************1111",
                CardCvv2= "123",
                CardExpirationMonth= "12",
                CardExpirationYear = "2010",
                AuthorizationTransactionId = "AuthorizationTransactionId1",
                AuthorizationTransactionCode="AuthorizationTransactionCode1",
                AuthorizationTransactionResult="AuthorizationTransactionResult1",
                CaptureTransactionId= "CaptureTransactionId1",
                CaptureTransactionResult = "CaptureTransactionResult1",
                SubscriptionTransactionId = "SubscriptionTransactionId1",
                PaidDateUtc= new DateTime(2010, 01, 01),
                BillingAddress = GetTestBillingAddress(),
                ShippingAddress = null,
                PickupAddress = GetTestPickupAddress(),
                ShippingMethod = "ShippingMethod1",
                ShippingRateComputationMethodSystemName = "ShippingRateComputationMethodSystemName1",
                PickUpInStore = true,
                CustomValuesXml = "CustomValuesXml1",
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 04)
            };

            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();
            fromDb.StoreId.ShouldEqual(1);
            fromDb.Customer.ShouldNotBeNull();
            fromDb.OrderStatus.ShouldEqual(OrderStatus.Complete);
            fromDb.ShippingStatus.ShouldEqual(ShippingStatus.Shipped);
            fromDb.PaymentStatus.ShouldEqual(PaymentStatus.Paid);
            fromDb.PaymentMethodSystemName.ShouldEqual("PaymentMethodSystemName1");
            fromDb.CustomerCurrencyCode.ShouldEqual("RUR");
            fromDb.CurrencyRate.ShouldEqual(1.1M);
            fromDb.CustomerTaxDisplayType.ShouldEqual(TaxDisplayType.ExcludingTax);
            fromDb.VatNumber.ShouldEqual("123456789");
            fromDb.OrderSubtotalInclTax.ShouldEqual(2.1M);
            fromDb.OrderSubtotalExclTax.ShouldEqual(3.1M);
            fromDb.OrderSubTotalDiscountInclTax.ShouldEqual(4.1M);
            fromDb.OrderSubTotalDiscountExclTax.ShouldEqual(5.1M);
            fromDb.OrderShippingInclTax.ShouldEqual(6.1M);
            fromDb.OrderShippingExclTax.ShouldEqual(7.1M);
            fromDb.PaymentMethodAdditionalFeeInclTax.ShouldEqual(8.1M);
            fromDb.PaymentMethodAdditionalFeeExclTax.ShouldEqual(9.1M);
            fromDb.TaxRates.ShouldEqual("1,3,5,7");
            fromDb.OrderTax.ShouldEqual(10.1M);
            fromDb.OrderDiscount.ShouldEqual(11.1M);
            fromDb.OrderTotal.ShouldEqual(12.1M);
            fromDb.RefundedAmount.ShouldEqual(13.1M);
            fromDb.RewardPointsWereAdded.ShouldEqual(true);
            fromDb.CheckoutAttributeDescription.ShouldEqual("CheckoutAttributeDescription1");
            fromDb.CheckoutAttributesXml.ShouldEqual("CheckoutAttributesXml1");
            fromDb.CustomerLanguageId.ShouldEqual(14);
            fromDb.CustomerIp.ShouldEqual("CustomerIp1");
            fromDb.AllowStoringCreditCardNumber.ShouldEqual(true);
            fromDb.CardType.ShouldEqual("Visa");
            fromDb.CardName.ShouldEqual("John Smith");
            fromDb.CardNumber.ShouldEqual("4111111111111111");
            fromDb.MaskedCreditCardNumber.ShouldEqual("************1111");
            fromDb.CardCvv2.ShouldEqual("123");
            fromDb.CardExpirationMonth.ShouldEqual("12");
            fromDb.CardExpirationYear.ShouldEqual("2010");
            fromDb.AuthorizationTransactionId.ShouldEqual("AuthorizationTransactionId1");
            fromDb.AuthorizationTransactionCode.ShouldEqual("AuthorizationTransactionCode1");
            fromDb.AuthorizationTransactionResult.ShouldEqual("AuthorizationTransactionResult1");
            fromDb.CaptureTransactionId.ShouldEqual("CaptureTransactionId1");
            fromDb.CaptureTransactionResult.ShouldEqual("CaptureTransactionResult1");
            fromDb.SubscriptionTransactionId.ShouldEqual("SubscriptionTransactionId1");
            fromDb.PaidDateUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.BillingAddress.ShouldNotBeNull();
            fromDb.BillingAddress.FirstName.ShouldEqual("FirstName 1");
            fromDb.ShippingAddress.ShouldBeNull();
            fromDb.PickupAddress.ShouldNotBeNull();
            fromDb.PickupAddress.LastName.ShouldEqual("LastName 3");
            fromDb.ShippingMethod.ShouldEqual("ShippingMethod1");
            fromDb.ShippingRateComputationMethodSystemName.ShouldEqual("ShippingRateComputationMethodSystemName1");
            fromDb.PickUpInStore.ShouldEqual(true);
            fromDb.CustomValuesXml.ShouldEqual("CustomValuesXml1");
            fromDb.Deleted.ShouldEqual(false);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 04));
        }

        [Test]
        public void Can_save_and_load_order_with_shipping_address()
        {
            var order = new Order
            {
                OrderGuid = Guid.NewGuid(),
                Customer = GetTestCustomer(),
                BillingAddress = GetTestBillingAddress(),
                ShippingAddress = GetTestShippingAddress(),
                CreatedOnUtc = new DateTime(2010, 01, 04)
            };

            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();
            fromDb.ShippingAddress.ShouldNotBeNull();
            fromDb.ShippingAddress.FirstName.ShouldEqual("FirstName 2");
        }

        [Test]
        public void Can_save_and_load_order_with_usedRewardPoints()
        {
            var order = new Order
            {
                OrderGuid = Guid.NewGuid(),
                Customer = GetTestCustomer(),
                RedeemedRewardPointsEntry = new RewardPointsHistory
                {
                    Customer = GetTestCustomer(),
                    StoreId = 1,
                    Points = -1,
                    Message = "Used with order",
                    PointsBalance = 2,
                    UsedAmount = 3,
                    CreatedOnUtc = new DateTime(2010, 01, 01)
                },
                BillingAddress = GetTestBillingAddress(),
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };

            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();
            fromDb.Deleted.ShouldEqual(false);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));

            fromDb.RedeemedRewardPointsEntry.ShouldNotBeNull();
            fromDb.RedeemedRewardPointsEntry.Points.ShouldEqual(-1);
        }

        [Test]
        public void Can_save_and_load_order_with_discountUsageHistory()
        {
            var testCustomer = GetTestCustomer();
            var order = new Order
            {
                OrderGuid = Guid.NewGuid(),
                Customer = testCustomer,
                BillingAddress = GetTestBillingAddress(),
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
            order.DiscountUsageHistory.Add
                (
                    new DiscountUsageHistory
                    {
                        Discount = GetTestDiscount(),
                        CreatedOnUtc = new DateTime(2010, 01, 01)
                    }
                );
            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();

            fromDb.DiscountUsageHistory.ShouldNotBeNull();
            fromDb.DiscountUsageHistory.ShouldNotBeNull();
            fromDb.DiscountUsageHistory.Count.ShouldEqual(1);
            fromDb.DiscountUsageHistory.First().CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }

        [Test]
        public void Can_save_and_load_order_with_giftCardUsageHistory()
        {
            var testCustomer = GetTestCustomer();
            var order = new Order
            {
                OrderGuid = Guid.NewGuid(),
                Customer = testCustomer,
                BillingAddress = GetTestBillingAddress(),
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
            order.GiftCardUsageHistory.Add
                (
                    new GiftCardUsageHistory
                    {
                        UsedValue = 1.1M,
                        CreatedOnUtc = new DateTime(2010, 01, 01),
                        GiftCard = GetTestGiftCard()
                    }
                );
            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();

            fromDb.GiftCardUsageHistory.ShouldNotBeNull();
            fromDb.GiftCardUsageHistory.Count.ShouldEqual(1);
            fromDb.GiftCardUsageHistory.First().CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }

        [Test]
        public void Can_save_and_load_order_with_orderNotes()
        {
            var order = new Order
            {
                OrderGuid = Guid.NewGuid(),
                Customer = GetTestCustomer(),
                BillingAddress = GetTestBillingAddress(),
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
            order.OrderNotes.Add
                (
                    new OrderNote
                    {
                        Note = "Note 1",
                        CreatedOnUtc = new DateTime(2010, 01, 01),
                    }
                );
            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();

            fromDb.OrderNotes.ShouldNotBeNull();
            fromDb.OrderNotes.Count.ShouldEqual(1);
            fromDb.OrderNotes.First().Note.ShouldEqual("Note 1");
        }

        [Test]
        public void Can_save_and_load_order_with_orderItems()
        {
            var order = new Order
            {
                OrderGuid = Guid.NewGuid(),
                Customer = GetTestCustomer(),
                BillingAddress = GetTestBillingAddress(),
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
            order.OrderItems.Add
                (
                    new OrderItem
                    {
                        Product = GetTestProduct(),
                        Quantity = 1
                    }
                );
            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();

            fromDb.OrderItems.ShouldNotBeNull();
            fromDb.OrderItems.Count.ShouldEqual(1);
            fromDb.OrderItems.First().Quantity.ShouldEqual(1);
        }
        
        [Test]
        public void Can_save_and_load_order_with_shipments()
        {
            var order = new Order
            {
                OrderGuid = Guid.NewGuid(),
                Customer = GetTestCustomer(),
                BillingAddress = GetTestBillingAddress(),
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
            order.Shipments.Add
                (
                    new Shipment
                    {
                        TrackingNumber = "TrackingNumber 1",
                        ShippedDateUtc = new DateTime(2010, 01, 01),
                        DeliveryDateUtc = new DateTime(2010, 01, 02),
                        CreatedOnUtc = new DateTime(2010, 01, 03),
                    }
                );
            var fromDb = SaveAndLoadEntity(order);
            fromDb.ShouldNotBeNull();

            fromDb.Shipments.ShouldNotBeNull();
            fromDb.Shipments.Count.ShouldEqual(1);
            fromDb.Shipments.First().TrackingNumber.ShouldEqual("TrackingNumber 1");
        }

        protected Product GetTestProduct()
        {
            return new Product
            {
                Name = "Product name 1",
                CreatedOnUtc = new DateTime(2010, 01, 03),
                UpdatedOnUtc = new DateTime(2010, 01, 04),
            };
        }
        
        protected Affiliate GetTestAffiliate()
        {
            return new Affiliate
            {
                Deleted = true,
                Active = true,
                Address = new Address
                {
                    FirstName = "FirstName 1",
                    LastName = "LastName 1",
                    Email = "Email 1",
                    Company = "Company 1",
                    City = "City 1",
                    Address1 = "Address1a",
                    Address2 = "Address1a",
                    ZipPostalCode = "ZipPostalCode 1",
                    PhoneNumber = "PhoneNumber 1",
                    FaxNumber = "FaxNumber 1",
                    CreatedOnUtc = new DateTime(2010, 01, 01),
                    Country = new Country
                    {
                        Name = "United States",
                        TwoLetterIsoCode = "US",
                        ThreeLetterIsoCode = "USA",
                    }
                },
            };
        }

        protected Address GetTestBillingAddress()
        {
            return new Address
            {
                FirstName = "FirstName 1",
                LastName = "LastName 1",
                Email = "Email 1",
                Company = "Company 1",
                City = "City 1",
                Address1 = "Address1a",
                Address2 = "Address1a",
                ZipPostalCode = "ZipPostalCode 1",
                PhoneNumber = "PhoneNumber 1",
                FaxNumber = "FaxNumber 1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                Country = GetTestCountry()
            };
        }

        protected Address GetTestShippingAddress()
        {
            return new Address
            {
                FirstName = "FirstName 2",
                LastName = "LastName 2",
                Email = "Email 2",
                Company = "Company 2",
                City = "City 2",
                Address1 = "Address2a",
                Address2 = "Address2b",
                ZipPostalCode = "ZipPostalCode 2",
                PhoneNumber = "PhoneNumber 2",
                FaxNumber = "FaxNumber 2",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                Country = GetTestCountry()
            };
        }

        protected Address GetTestPickupAddress()
        {
            return new Address
            {
                FirstName = "FirstName 3",
                LastName = "LastName 3",
                Email = "Email 3",
                Company = "Company 3",
                City = "City 3",
                Address1 = "Address3a",
                Address2 = "Address3b",
                ZipPostalCode = "ZipPostalCode 3",
                PhoneNumber = "PhoneNumber 3",
                FaxNumber = "FaxNumber 3",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                Country = GetTestCountry()
            };
        }

        protected Country GetTestCountry()
        {
            return new Country
            {
                Name = "United States",
                AllowsBilling = true,
                AllowsShipping = true,
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 1,
                SubjectToVat = true,
                Published = true,
                DisplayOrder = 1
            };
        }

        protected GiftCard GetTestGiftCard()
        {
            return new GiftCard
            {
                Amount = 1,
                IsGiftCardActivated = true,
                GiftCardCouponCode = "Secret",
                RecipientName = "RecipientName 1",
                RecipientEmail = "a@b.c",
                SenderName = "SenderName 1",
                SenderEmail = "d@e.f",
                Message = "Message 1",
                IsRecipientNotified = true,
                CreatedOnUtc = new DateTime(2010, 01, 01),
            };
        }

        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "some comment here",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
        }

        protected Discount GetTestDiscount()
        {
            return new Discount
            {
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToCategories,
                UsePercentage = true,
                DiscountPercentage = 1,
                DiscountAmount = 2,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                RequiresCouponCode = true,
                CouponCode = "SecretCode",
                DiscountLimitation = DiscountLimitationType.Unlimited,
                LimitationTimes = 3,
            };
        }
    }
}
