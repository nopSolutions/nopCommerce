using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Affiliates;
using NUnit.Framework;
using Nop.Tests;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class CustomerPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_customer()
        {
            var customer = GetTestCustomer();

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();
            fromDb.AssociatedUserId.ShouldEqual(4);
            fromDb.AdminComment.ShouldEqual("some comment here");
            fromDb.TaxDisplayType.ShouldEqual(TaxDisplayType.IncludingTax);
            fromDb.IsTaxExempt.ShouldEqual(true);
            fromDb.SelectedPaymentMethodSystemName.ShouldEqual("test1");
            fromDb.VatNumber.ShouldEqual("123456");
            fromDb.VatNumberStatus.ShouldEqual(VatNumberStatus.Valid);
            fromDb.CheckoutAttributes.ShouldEqual("CheckoutAttributes 1");
            fromDb.DiscountCouponCode.ShouldEqual("coupon1");
            fromDb.GiftCardCouponCodes.ShouldEqual("GiftCardCouponCodes 1");
            fromDb.Active.ShouldEqual(true);
            fromDb.Deleted.ShouldEqual(false);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.LastActivityDateUtc.ShouldEqual(new DateTime(2010, 01, 02));
        }

        [Test]
        public void Can_save_and_load_customer_with_customerRoles()
        {
            var customer = GetTestCustomer();
            customer.CustomerRoles = new List<CustomerRole>()
            {
                new CustomerRole()
                {
                    Name = "Administrators",
                    FreeShipping = true,
                    TaxExempt = true,
                    Active = true,
                    IsSystemRole = true,
                    SystemName = "Administrators"
                }
            };


            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();

            fromDb.CustomerRoles.ShouldNotBeNull();
            (fromDb.CustomerRoles.Count == 1).ShouldBeTrue();
            fromDb.CustomerRoles.First().Name.ShouldEqual("Administrators");
        }

        [Test]
        public void Can_save_and_load_customer_with_language()
        {
            var customer = GetTestCustomer();
            customer.Language = new Language()
            {
                Name = "English",
                LanguageCulture = "en-Us",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 1
            };

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();

            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.Name.ShouldEqual("English");
        }

        [Test]
        public void Can_save_and_load_customer_with_currency()
        {
            var customer = GetTestCustomer();
            customer.Currency = new Currency()
            {
                Name = "US Dollar",
                CurrencyCode = "USD",
                Rate = 1,
                DisplayLocale = "en-US",
                CustomFormatting = "CustomFormatting 1",
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();

            fromDb.Currency.ShouldNotBeNull();
            fromDb.Currency.Name.ShouldEqual("US Dollar");
        }

        [Test]
        public void Can_save_customer_with_rewardPointsHistoryEntry()
        {
            var customer = GetTestCustomer();
            
            customer.AddRewardPointsHistoryEntry(1, "Points for registration");

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();
            fromDb.RewardPointsHistory.Count.ShouldEqual(1);
            fromDb.RewardPointsHistory.First().Points.ShouldEqual(1);
        }

        [Test]
        public void Can_save_customer_with_address()
        {
            var customer = GetTestCustomer();

            var address = new Address 
            { 
                FirstName = "Test",
                Country = GetTestCountry(),
                CreatedOnUtc = new DateTime(2010, 01, 01),
            };

            customer.AddAddress(address);

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();
            fromDb.Addresses.Count.ShouldEqual(1);
            fromDb.Addresses.First().FirstName.ShouldEqual("Test");
        }

        [Test]
        public void Can_save_customer_with_affiliate()
        {
            var customer = GetTestCustomer();
            customer.Affiliate = GetTestAffiliate();

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();
            fromDb.Affiliate.ShouldNotBeNull();
            fromDb.Affiliate.Active.ShouldEqual(true);
        }

        [Test]
        public void Can_set_default_billing_and_shipping_address()
        {
            var customer = GetTestCustomer();

            var address = new Address { FirstName = "Billing", Country = GetTestCountry(), CreatedOnUtc = new DateTime(2010, 01, 01) };
            var address2 = new Address { FirstName = "Shipping", Country = GetTestCountry(), CreatedOnUtc = new DateTime(2010, 01, 01) };

            customer.AddAddress(address);
            customer.AddAddress(address2);

            customer.SetBillingAddress(address);
            customer.SetShippingAddress(address2);

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();
            fromDb.Addresses.Count.ShouldEqual(2);

            fromDb.BillingAddress.FirstName.ShouldEqual("Billing");
            fromDb.ShippingAddress.FirstName.ShouldEqual("Shipping");

            var addresses = fromDb.Addresses.ToList();

            fromDb.BillingAddress.ShouldBeTheSameAs(addresses[0]);
            fromDb.ShippingAddress.ShouldBeTheSameAs(addresses[1]);
        }

        [Test]
        public void Can_remove_a_customer_address()
        {
            var customer = GetTestCustomer();
            var address = new Address { FirstName = "Test", Country = GetTestCountry(), CreatedOnUtc = new DateTime(2010, 01, 01) };
            customer.AddAddress(address);
            customer.SetBillingAddress(address);

            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();
            fromDb.Addresses.Count.ShouldEqual(1);
            fromDb.BillingAddress.ShouldNotBeNull();

            fromDb.RemoveAddress(address);

            context.SaveChanges();

            fromDb.Addresses.Count.ShouldEqual(0);
            fromDb.BillingAddress.ShouldBeNull();
        }
        
        [Test]
        public void Can_save_customer_with_shopping_cart()
        {
            var customer = GetTestCustomer();
            var productVariant = GetTestProductVariant();

            customer.ShoppingCartItems = new List<ShoppingCartItem>()
            {
                new ShoppingCartItem()
                {
                    ShoppingCartType = ShoppingCartType.ShoppingCart,
                    AttributesXml = "AttributesXml 1",
                    CustomerEnteredPrice = 1,
                    Quantity = 2,
                    CreatedOnUtc = new DateTime(2010, 01, 01),
                    UpdatedOnUtc = new DateTime(2010, 01, 02),
                    ProductVariant = GetTestProductVariant()
                }
            };


            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();

            fromDb.ShoppingCartItems.ShouldNotBeNull();
            (fromDb.ShoppingCartItems.Count == 1).ShouldBeTrue();
            fromDb.ShoppingCartItems.First().AttributesXml.ShouldEqual("AttributesXml 1");
        }

        [Test]
        public void Can_save_and_load_customer_with_orders()
        {
            var customer = GetTestCustomer();
            customer.Orders = new List<Order>()
            {
                new Order()
                {
                    OrderGuid = Guid.NewGuid(),
                    BillingAddress = new Address()
                    {
                        Country = new Country()
                        {
                            Name = "United States",
                            TwoLetterIsoCode = "US",
                            ThreeLetterIsoCode = "USA",
                        },
                        CreatedOnUtc = new DateTime(2010, 01, 01),
                    },
                    Deleted = true,
                    CreatedOnUtc = new DateTime(2010, 01, 01)
                }
            };


            var fromDb = SaveAndLoadEntity(customer);
            fromDb.ShouldNotBeNull();

            fromDb.Orders.ShouldNotBeNull();
            (fromDb.Orders.Count == 1).ShouldBeTrue();
            fromDb.Orders.First().Deleted.ShouldEqual(true);
        }

        protected Affiliate GetTestAffiliate()
        {
            return new Affiliate
            {
                Deleted = true,
                Active = true,
                Address = GetTestAddress(),
            };
        }

        protected Address GetTestAddress()
        {
            return new Address()
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
            };
        }

        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                AssociatedUserId = 4,
                AdminComment = "some comment here",
                TaxDisplayType = TaxDisplayType.IncludingTax,
                IsTaxExempt = true,
                SelectedPaymentMethodSystemName = "test1",
                VatNumber = "123456",
                VatNumberStatus = VatNumberStatus.Valid,
                CheckoutAttributes = "CheckoutAttributes 1",
                DiscountCouponCode= "coupon1",
                GiftCardCouponCodes = "GiftCardCouponCodes 1",
                Active = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
        }

        protected ProductVariant GetTestProductVariant()
        {
            return new ProductVariant
            {
                Name = "Product variant name 1",
                Sku = "sku 1",
                Description = "description",
                AdminComment = "adminComment",
                ManufacturerPartNumber = "manufacturerPartNumber",
                IsGiftCard = true,
                GiftCardTypeId = 1,
                IsDownload = true,
                DownloadId = 2,
                UnlimitedDownloads = true,
                MaxNumberOfDownloads = 3,
                DownloadExpirationDays = 4,
                DownloadActivationTypeId = 5,
                HasSampleDownload = true,
                SampleDownloadId = 6,
                HasUserAgreement = true,
                UserAgreementText = "userAgreementText",
                IsRecurring = true,
                RecurringCycleLength = 7,
                RecurringCyclePeriodId = 8,
                RecurringTotalCycles = 9,
                IsShipEnabled = true,
                IsFreeShipping = true,
                AdditionalShippingCharge = 10,
                IsTaxExempt = true,
                TaxCategoryId = 11,
                ManageInventoryMethodId = 12,
                StockQuantity = 13,
                DisplayStockAvailability = true,
                DisplayStockQuantity = true,
                MinStockQuantity = 14,
                LowStockActivityId = 15,
                NotifyAdminForQuantityBelow = 16,
                BackorderModeId = 17,
                OrderMinimumQuantity = 18,
                OrderMaximumQuantity = 19,
                WarehouseId = 20,
                DisableBuyButton = true,
                CallForPrice = true,
                Price = 21,
                OldPrice = 22,
                ProductCost = 23,
                CustomerEntersPrice = true,
                MinimumCustomerEnteredPrice = 24,
                MaximumCustomerEnteredPrice = 25,
                Weight = 26,
                Length = 27,
                Width = 28,
                Height = 29,
                PictureId = 0,
                AvailableStartDateTimeUtc = new DateTime(2010, 01, 01),
                AvailableEndDateTimeUtc = new DateTime(2010, 01, 02),
                Published = true,
                Deleted = false,
                DisplayOrder = 31,
                CreatedOnUtc = new DateTime(2010, 01, 03),
                UpdatedOnUtc = new DateTime(2010, 01, 04),
                Product = new Product()
                {
                    Name = "Name 1",
                    Published = true,
                    Deleted = false,
                    CreatedOnUtc = new DateTime(2010, 01, 01),
                    UpdatedOnUtc = new DateTime(2010, 01, 02)
                }
            };
        }

        protected Country GetTestCountry()
        {
            return new Country
            {
                Name = "United States",
                AllowsRegistration = true,
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

    }
}
