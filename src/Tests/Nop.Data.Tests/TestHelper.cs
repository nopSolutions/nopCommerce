using System;
using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tasks;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Tests
{
    public static class TestHelper
    {
        private static readonly Guid _customerGuid = Guid.NewGuid();
        private static readonly Guid _orderGuid = Guid.NewGuid();
        private static readonly Guid _downloadGuid = Guid.NewGuid();
        private static readonly Guid _newsLetterSubscriptionGuid = Guid.NewGuid();

        #region Affiliates

        public static Affiliate GetTestAffiliate(this PersistenceTest test)
        {
            return new Affiliate
            {
                Deleted = true,
                Active = true,
                Address = test.GetTestAddress(),
                AdminComment = "AdminComment 1",
                FriendlyUrlName = "FriendlyUrlName 1"
            };
        }

        #endregion

        #region Blogs

        public static BlogComment GetTestBlogComment(this PersistenceTest test)
        {
            return new BlogComment
            {
                IsApproved = true,
                CreatedOnUtc = new DateTime(2010, 01, 03),
                Customer = test.GetTestCustomer(),
                Store = test.GetTestStore()
            };
        }

        public static BlogPost GetTestBlogPost(this PersistenceTest test)
        {
            return new BlogPost
            {
                Title = "Title 1",
                Body = "Body 1",
                BodyOverview = "BodyOverview 1",
                AllowComments = true,
                Tags = "Tags 1",
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                CreatedOnUtc = new DateTime(2010, 01, 03),
                MetaTitle = "MetaTitle 1",
                MetaDescription = "MetaDescription 1",
                MetaKeywords = "MetaKeywords 1",
                LimitedToStores = true,
                Language = test.GetTestLanguage()
            };
        }

        #endregion

        #region Catalog

        public static BackInStockSubscription GetTestBackInStockSubscription(this PersistenceTest test)
        {
            return new BackInStockSubscription
            {
                Product = test.GetTestProduct(),
                Customer = test.GetTestCustomer(),
                CreatedOnUtc = new DateTime(2010, 01, 02)
            };
        }

        public static Category GetTestCategory(this PersistenceTest test)
        {
            return new Category
            {
                Name = "Books",
                Description = "Description 1",
                CategoryTemplateId = 1,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                ParentCategoryId = 2,
                PictureId = 3,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                PriceRanges = "1-3;",
                ShowOnHomePage = false,
                IncludeInTopMenu = true,
                Published = true,
                SubjectToAcl = true,
                LimitedToStores = true,
                Deleted = false,
                DisplayOrder = 5,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
        }

        public static CategoryTemplate GetTestCategoryTemplate(this PersistenceTest test)
        {
            return new CategoryTemplate
            {
                Name = "Name 1",
                ViewPath = "ViewPath 1",
                DisplayOrder = 1,
            };
        }

        public static Manufacturer GetTestManufacturer(this PersistenceTest test)
        {
            return new Manufacturer
            {
                Name = "Name",
                Description = "Description 1",
                ManufacturerTemplateId = 1,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                PictureId = 3,
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12",
                PriceRanges = "1-3;",
                Published = true,
                SubjectToAcl = true,
                LimitedToStores = true,
                Deleted = false,
                DisplayOrder = 5,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
        }

        public static ManufacturerTemplate GetTestManufacturerTemplate(this PersistenceTest test)
        {
            return new ManufacturerTemplate
            {
                Name = "Name 1",
                ViewPath = "ViewPath 1",
                DisplayOrder = 1,
            };
        }

        public static PredefinedProductAttributeValue GetTestPredefinedProductAttributeValue(this PersistenceTest test)
        {
            return new PredefinedProductAttributeValue
            {
                Name = "Name 1",
                PriceAdjustment = 1.1M,
                PriceAdjustmentUsePercentage = true,
                WeightAdjustment = 2.1M,
                Cost = 3.1M,
                IsPreSelected = true,
                DisplayOrder = 3,
                ProductAttribute = test.GetTestProductAttribute()
            };
        }

        public static Product GetTestProduct(this PersistenceTest test)
        {
            return new Product
            {
                ProductType = ProductType.GroupedProduct,
                ParentGroupedProductId = 2,
                VisibleIndividually = true,
                Name = "Name 1",
                ShortDescription = "ShortDescription 1",
                FullDescription = "FullDescription 1",
                AdminComment = "AdminComment 1",
                VendorId = 1,
                ProductTemplateId = 2,
                ShowOnHomePage = false,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                AllowCustomerReviews = true,
                ApprovedRatingSum = 2,
                NotApprovedRatingSum = 3,
                ApprovedTotalReviews = 4,
                NotApprovedTotalReviews = 5,
                SubjectToAcl = true,
                LimitedToStores = true,
                Sku = "SKU 1",
                ManufacturerPartNumber = "manufacturerPartNumber",
                Gtin = "GTIN 1",
                IsGiftCard = true,
                GiftCardTypeId = 1,
                OverriddenGiftCardAmount = 1,
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
                IsRental = true,
                RentalPriceLength = 9,
                RentalPricePeriodId = 10,
                IsShipEnabled = true,
                IsFreeShipping = true,
                ShipSeparately = true,
                AdditionalShippingCharge = 10.1M,
                DeliveryDateId = 5,
                IsTaxExempt = true,
                TaxCategoryId = 11,
                IsTelecommunicationsOrBroadcastingOrElectronicServices = true,
                ManageInventoryMethodId = 12,
                ProductAvailabilityRangeId = 1,
                UseMultipleWarehouses = true,
                WarehouseId = 6,
                StockQuantity = 13,
                DisplayStockAvailability = true,
                DisplayStockQuantity = true,
                MinStockQuantity = 14,
                LowStockActivityId = 15,
                NotifyAdminForQuantityBelow = 16,
                BackorderModeId = 17,
                AllowBackInStockSubscriptions = true,
                OrderMinimumQuantity = 18,
                OrderMaximumQuantity = 19,
                AllowedQuantities = "1, 5,6,10",
                AllowAddingOnlyExistingAttributeCombinations = true,
                NotReturnable = true,
                DisableBuyButton = true,
                DisableWishlistButton = true,
                AvailableForPreOrder = true,
                PreOrderAvailabilityStartDateTimeUtc = new DateTime(2010, 01, 01),
                CallForPrice = true,
                Price = 21.1M,
                OldPrice = 22.1M,
                ProductCost = 23.1M,
                CustomerEntersPrice = true,
                MinimumCustomerEnteredPrice = 24.1M,
                MaximumCustomerEnteredPrice = 25.1M,
                BasepriceEnabled = true,
                BasepriceAmount = 33.1M,
                BasepriceUnitId = 4,
                BasepriceBaseAmount = 34.1M,
                BasepriceBaseUnitId = 5,
                MarkAsNew = true,
                MarkAsNewStartDateTimeUtc = new DateTime(2010, 01, 07),
                MarkAsNewEndDateTimeUtc = new DateTime(2010, 01, 08),
                HasTierPrices = true,
                HasDiscountsApplied = true,
                Weight = 26.1M,
                Length = 27.1M,
                Width = 28.1M,
                Height = 29.1M,
                AvailableStartDateTimeUtc = new DateTime(2010, 01, 01),
                AvailableEndDateTimeUtc = new DateTime(2010, 01, 02),
                RequireOtherProducts = true,
                RequiredProductIds = "1,2,3",
                AutomaticallyAddRequiredProducts = true,
                DisplayOrder = 30,
                Published = true,
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
        }
        
        public static ProductAttribute GetTestProductAttribute(this PersistenceTest test)
        {
            return new ProductAttribute
            {
                Name = "Name 1",
                Description = "Description 1",
            };
        }

        public static ProductAttributeCombination GetTestProductAttributeCombination(this PersistenceTest test)
        {
            return new ProductAttributeCombination
            {
                AttributesXml = "Some XML",
                StockQuantity = 2,
                AllowOutOfStockOrders = true,
                Sku = "Sku1",
                ManufacturerPartNumber = "ManufacturerPartNumber1",
                Gtin = "Gtin1",
                OverriddenPrice = 0.01M,
                NotifyAdminForQuantityBelow = 3,
                PictureId = 1,
                Product = test.GetTestProduct()
            };
        }

        public static ProductAttributeMapping GetTestProductAttributeMapping(this PersistenceTest test)
        {
            return new ProductAttributeMapping
            {
                TextPrompt = "TextPrompt 1",
                IsRequired = true,
                AttributeControlType = AttributeControlType.DropdownList,
                DisplayOrder = 1,
                ValidationMinLength = 2,
                ValidationMaxLength = 3,
                ValidationFileAllowedExtensions = "ValidationFileAllowedExtensions 1",
                ValidationFileMaximumSize = 4,
                DefaultValue = "DefaultValue 1",
                ConditionAttributeXml = "ConditionAttributeXml 1",
                Product = test.GetTestProduct(),
                ProductAttribute = test.GetTestProductAttribute()
            };
        }

        public static ProductAttributeValue GetTestProductAttributeValue(this PersistenceTest test)
        {
            return new ProductAttributeValue
            {
                AttributeValueType = AttributeValueType.AssociatedToProduct,
                AssociatedProductId = 10,
                Name = "Name 1",
                ColorSquaresRgb = "12FF33",
                ImageSquaresPictureId = 1,
                PriceAdjustment = 1.1M,
                PriceAdjustmentUsePercentage = true,
                WeightAdjustment = 2.1M,
                Cost = 3.1M,
                Quantity = 2,
                IsPreSelected = true,
                DisplayOrder = 3,
                ProductAttributeMapping = test.GetTestProductAttributeMapping()
            };
        }

        public static ProductReview GetTestProductReview(this PersistenceTest test)
        {
            var testProduct = test.GetTestProduct();
            var testStore = test.GetTestStore();
            var testCustomer = test.GetTestCustomer();

            return new ProductReview
            {
                Product = testProduct,
                ProductId = testProduct.Id,
                Store = testStore,
                StoreId = testStore.Id,
                Customer = testCustomer,
                CustomerId = testCustomer.Id,
                ReviewText = "TestText",
                Title = "TestTitle",
                IsApproved = true,
                Rating = 5,
                HelpfulNoTotal = 0,
                HelpfulYesTotal = 1,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                CustomerNotifiedOfReply = true,
                ReplyText = "Test"
            };
        }

        public static ReviewType GetTestReviewType(this PersistenceTest test)
        {
            return new ReviewType
            {
                Name = "TestName",
                Description = "TestDescription",
                DisplayOrder = 1,
                VisibleToAllCustomers = true
            };
        }

        public static ProductCategory GetTestProductCategory(this PersistenceTest test)
        {
            return new ProductCategory
            {
                IsFeaturedProduct = true,
                DisplayOrder = 1
            };
        }
        
        public static ProductManufacturer GetTestProductManufacturer(this PersistenceTest test)
        {
            return new ProductManufacturer
            {
                IsFeaturedProduct = true,
                DisplayOrder = 1
            };
        }

        public static ProductPicture GetTestProductPicture(this PersistenceTest test)
        {
            return new ProductPicture
            {
                DisplayOrder = 1,
                Picture = test.GetTestPicture()
            };
        }

        public static ProductSpecificationAttribute GetTestProductSpecificationAttribute(this PersistenceTest test)
        {
            var specificationAttributeOption = test.GetTestSpecificationAttributeOption();
            specificationAttributeOption.SpecificationAttribute = test.GetTestSpecificationAttribute();

            return new ProductSpecificationAttribute
            {
                AttributeType = SpecificationAttributeType.Hyperlink,
                AllowFiltering = true,
                ShowOnProductPage = true,
                DisplayOrder = 1,
                Product = test.GetTestProduct(),
                SpecificationAttributeOption = specificationAttributeOption
            };
        }

        public static ProductTag GetTestProductTag(this PersistenceTest test)
        {
            return new ProductTag
            {
                Name = "Tag name 1",
            };
        }

        public static ProductTemplate GetTestProductTemplate(this PersistenceTest test)
        {
            return new ProductTemplate
            {
                Name = "Name 1",
                ViewPath = "ViewPath 1",
                DisplayOrder = 1,
            };
        }

        public static ProductWarehouseInventory GetTestProductWarehouseInventory(this PersistenceTest test)
        {
            return new ProductWarehouseInventory
            {
                Warehouse = test.GetTestWarehouse(),
                StockQuantity = 2
            };
        }
        
        public static SpecificationAttribute GetTestSpecificationAttribute(this PersistenceTest test)
        {
            return new SpecificationAttribute
            {
                Name = "SpecificationAttribute name 1",
                DisplayOrder = 2
            };
        }

        public static SpecificationAttributeOption GetTestSpecificationAttributeOption(this PersistenceTest test)
        {
            return new SpecificationAttributeOption
            {
                Name = "SpecificationAttributeOption name 1",
                DisplayOrder = 1
            };
        }
        
        public static TierPrice GetTestTierPrice(this PersistenceTest test)
        {
            return new TierPrice
            {
                StoreId = 7,
                Quantity = 1,
                Price = 2.1M,
                StartDateTimeUtc = new DateTime(2010, 01, 03)
            };
        }
        
        #endregion

        #region Common

        public static Address GetTestAddress(this PersistenceTest test)
        {
            return new Address
            {
                FirstName = "FirstName 1",
                LastName = "LastName 1",
                Email = "Email 1",
                Company = "Company 1",
                City = "City 1",
                County = "County 1",
                Address1 = "Address1a",
                Address2 = "Address1a",
                ZipPostalCode = "ZipPostalCode 1",
                PhoneNumber = "PhoneNumber 1",
                FaxNumber = "FaxNumber 1",
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        public static AddressAttribute GetTestAddressAttribute(this PersistenceTest test)
        {
            return new AddressAttribute
            {
                Name = "Name 1",
                IsRequired = true,
                AttributeControlType = AttributeControlType.Datepicker,
                DisplayOrder = 2
            };
        }

        public static AddressAttributeValue GetTestAddressAttributeValue(this PersistenceTest test)
        {
            return new AddressAttributeValue
            {
                Name = "Name 2",
                IsPreSelected = true,
                DisplayOrder = 1,
            };
        }
       
        public static GenericAttribute GetTestGenericAttribute(this PersistenceTest test)
        {
            return new GenericAttribute
            {
                EntityId = 1,
                KeyGroup = "KeyGroup 1",
                Key = "Key 1",
                Value = "Value 1",
                StoreId = 2,
            };
        }

        public static SearchTerm GetTestSearchTerm(this PersistenceTest test)
        {
            return new SearchTerm
            {
                Keyword = "Keyword 1",
                StoreId = 1,
                Count = 2,
            };
        }

        #endregion

        #region Configuration

        public static Setting GetTestSetting(this PersistenceTest test)
        {
            return new Setting
            {
                Name = "Setting1",
                Value = "Value1",
                StoreId = 1,
            };
        }

        #endregion

        #region Customers

        public static Customer GetTestCustomer(this PersistenceTest test)
        {
            return new Customer
            {
                Username = "a@b.com",
                Email = "a@b.com",
                CustomerGuid = _customerGuid,
                AdminComment = "some comment here",
                IsTaxExempt = true,
                AffiliateId = 1,
                VendorId = 2,
                HasShoppingCartItems = true,
                RequireReLogin = true,
                Active = true,
                Deleted = false,
                IsSystemAccount = true,
                SystemName = "SystemName 1",
                LastIpAddress = "192.168.1.1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastLoginDateUtc = new DateTime(2010, 01, 02),
                LastActivityDateUtc = new DateTime(2010, 01, 03)
            };
        }

        public static CustomerAttribute GetTestCustomerAttribute(this PersistenceTest test)
        {
            return new CustomerAttribute
            {
                Name = "Name 1",
                IsRequired = true,
                AttributeControlType = AttributeControlType.Datepicker,
                DisplayOrder = 2
            };
        }

        public static CustomerAttributeValue GetTestCustomerAttributeValue(this PersistenceTest test)
        {
            return new CustomerAttributeValue
            {
                Name = "Name 2",
                IsPreSelected = true,
                DisplayOrder = 1,
            };
        }

        public static CustomerPassword GetTestCustomerPassword(this PersistenceTest test)
        {
            return new CustomerPassword
            {
                Password = "password",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        public static CustomerRole GetTestCustomerRole(this PersistenceTest test)
        {
            return new CustomerRole
            {
                Name = "Administrators",
                FreeShipping = true,
                TaxExempt = true,
                Active = true,
                IsSystemRole = true,
                SystemName = "Administrators",
                OverrideTaxDisplayType = true,
                DefaultTaxDisplayTypeId = 2,
                EnablePasswordLifetime = true
            };
        }

        public static ExternalAuthenticationRecord GetTestExternalAuthenticationRecord(this PersistenceTest test)
        {
            return new ExternalAuthenticationRecord
            {
                Email = "Email 1",
                ExternalIdentifier = "ExternalIdentifier 1",
                ExternalDisplayIdentifier = "ExternalDisplayIdentifier 1",
                OAuthToken = "OAuthToken 1",
                OAuthAccessToken = "OAuthAccessToken 1",
                ProviderSystemName = "ProviderSystemName 1"
            };
        }
        
        public static RewardPointsHistory GetTestRewardPointsHistory(this PersistenceTest test)
        {
            return new RewardPointsHistory
            {
                Customer = test.GetTestCustomer(),
                StoreId = 1,
                Points = 2,
                Message = "Points for registration",
                PointsBalance = 3,
                UsedAmount = 3.1M,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UsedWithOrder = test.GetTestOrder()
            };
        }

        #endregion

        #region Directory

        public static Country GetTestCountry(this PersistenceTest test)
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
                DisplayOrder = 1,
                LimitedToStores = true
            };
        }

        public static Currency GetTestCurrency(this PersistenceTest test)
        {
            return new Currency
            {
                Name = "US Dollar",
                CurrencyCode = "USD",
                Rate = 1.1M,
                DisplayLocale = "en-US",
                CustomFormatting = "CustomFormatting 1",
                LimitedToStores = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
        }
        
        public static MeasureDimension GetTestMeasureDimension(this PersistenceTest test)
        {
            return new MeasureDimension
            {
                Name = "inch(es)",
                SystemKeyword = "inches",
                Ratio = 1.12345678M,
                DisplayOrder = 2,
            };
        }

        public static MeasureWeight GetTestMeasureWeight(this PersistenceTest test)
        {
            return new MeasureWeight
            {
                Name = "ounce(s)",
                SystemKeyword = "ounce",
                Ratio = 1.12345678M,
                DisplayOrder = 2,
            };
        }

        public static StateProvince GetTestStateProvince(this PersistenceTest test)
        {
            return new StateProvince
            {
                Name = "California",
                Abbreviation = "CA",
                DisplayOrder = 1
            };
        }

        #endregion

        #region Discounts

        public static Discount GetTestDiscount(this PersistenceTest test)
        {
            return new Discount
            {
                DiscountType = DiscountType.AssignedToCategories,
                Name = "Discount 1",
                UsePercentage = true,
                DiscountPercentage = 1.1M,
                DiscountAmount = 2.1M,
                MaximumDiscountAmount = 3.1M,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                RequiresCouponCode = true,
                CouponCode = "SecretCode",
                IsCumulative = true,
                DiscountLimitation = DiscountLimitationType.Unlimited,
                LimitationTimes = 3,
                MaximumDiscountedQuantity = 4,
                AppliedToSubCategories = true
            };
        }

        public static DiscountRequirement GetTestDiscountRequirement(this PersistenceTest test)
        {
            return new DiscountRequirement
            {
                DiscountRequirementRuleSystemName = "BillingCountryIs"
            };
        }

        public static DiscountUsageHistory GetTestDiscountUsageHistory(this PersistenceTest test)
        {
            var order = test.GetTestOrder();
            order.Customer = test.GetTestCustomer();
            return new DiscountUsageHistory
            {
                Discount = test.GetTestDiscount(),
                Order = order,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        #endregion

        #region Forums

        public static Forum GetTestForum(this PersistenceTest test)
        {
            return new Forum
            {
                Name = "Forum 1",
                Description = "Forum 1 Description",
                DisplayOrder = 10,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
                NumPosts = 25,
                NumTopics = 15,
            };
        }

        public static ForumGroup GetTestForumGroup(this PersistenceTest test)
        {
            return new ForumGroup
            {
                Name = "Forum Group 1",
                DisplayOrder = 1,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
        }

        public static ForumPost GetTestForumPost(this PersistenceTest test)
        {
            return new ForumPost
            {
                Text = "Forum Post 1 Text",
                IPAddress = "127.0.0.1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02)
            };
        }

        public static ForumSubscription GetTestForumSubscription(this PersistenceTest test)
        {
            return new ForumSubscription
            {
                CreatedOnUtc = new DateTime(2010, 01, 01),
                SubscriptionGuid = new Guid("11111111-2222-3333-4444-555555555555")
            };
        }

        public static ForumTopic GetTestForumTopic(this PersistenceTest test)
        {
            return new ForumTopic
            {
                Subject = "Forum Topic 1",
                TopicTypeId = (int)ForumTopicType.Sticky,
                Views = 123,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
                NumPosts = 100
            };
        }
        
        public static PrivateMessage GetTestPrivateMessage(this PersistenceTest test)
        {
            return new PrivateMessage
            {
                Subject = "Private Message 1 Subject",
                Text = "Private Message 1 Text",
                IsDeletedByAuthor = false,
                IsDeletedByRecipient = false,
                IsRead = false,
                CreatedOnUtc = new DateTime(2010, 01, 01),
            };
        }

        #endregion

        #region Localization

        public static Language GetTestLanguage(this PersistenceTest test)
        {
            return new Language
            {
                Name = "English",
                LanguageCulture = "en-Us",
                UniqueSeoCode = "en",
                FlagImageFileName = "us.png",
                Rtl = true,
                DefaultCurrencyId = 1,
                Published = true,
                LimitedToStores = true,
                DisplayOrder = 1
            };
        }

        public static LocaleStringResource GetTestLocaleStringResource(this PersistenceTest test)
        {
            return new LocaleStringResource
            {
                ResourceName = "ResourceName1",
                ResourceValue = "ResourceValue2"
            };
        }

        public static LocalizedProperty GetTestLocalizedProperty(this PersistenceTest test)
        {
            return new LocalizedProperty
            {
                EntityId = 1,
                LocaleKeyGroup = "LocaleKeyGroup 1",
                LocaleKey = "LocaleKey 1",
                LocaleValue = "LocaleValue 1"
            };
        }

        #endregion

        #region Logging

        public static ActivityLogType GetTestActivityLogType(this PersistenceTest test)
        {
            return new ActivityLogType
            {
                SystemKeyword = "SystemKeyword 1",
                Name = "Name 1",
                Enabled = true,
            };
        }

        public static Log GetTestLog(this PersistenceTest test)
        {
            return new Log
            {
                LogLevel = LogLevel.Error,
                ShortMessage = "ShortMessage1",
                FullMessage = "FullMessage1",
                IpAddress = "127.0.0.1",
                PageUrl = "http://www.someUrl1.com",
                ReferrerUrl = "http://www.someUrl2.com",
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        #endregion

        #region Media

        public static Download GetTestDownload(this PersistenceTest test)
        {
            return new Download
            {
                DownloadGuid = _downloadGuid,
                UseDownloadUrl = true,
                DownloadUrl = "http://www.someUrl.com/file.zip",
                DownloadBinary = new byte[] { 1, 2, 3 },
                ContentType = MimeTypes.ApplicationXZipCo,
                Filename = "file",
                Extension = ".zip",
                IsNew = true
            };
        }

        public static Picture GetTestPicture(this PersistenceTest test)
        {
            return new Picture
            {
                PictureBinary = new byte[] { 1, 2, 3 },
                MimeType = MimeTypes.ImagePJpeg,
                SeoFilename = "seo filename 1",
                AltAttribute = "AltAttribute 1",
                TitleAttribute = "TitleAttribute 1",
                IsNew = true
            };
        }
        
        #endregion

        #region Messages

        public static Campaign GetTestCampaign(this PersistenceTest test)
        {
            return new Campaign
            {
                Name = "Name 1",
                Subject = "Subject 1",
                Body = "Body 1",
                CreatedOnUtc = new DateTime(2010, 01, 02),
                DontSendBeforeDateUtc = new DateTime(2016, 2, 23),
                CustomerRoleId = 1,
                StoreId = 1
            };
        }

        public static EmailAccount GetTestEmailAccount(this PersistenceTest test)
        {
            return new EmailAccount
            {
                Email = "admin@yourstore.com",
                DisplayName = "Administrator",
                Host = "127.0.0.1",
                Port = 125,
                Username = "John",
                Password = "111",
                EnableSsl = true,
                UseDefaultCredentials = true
            };
        }

        public static MessageTemplate GetTestMessageTemplate(this PersistenceTest test)
        {
            return new MessageTemplate
            {
                Name = "Template1",
                BccEmailAddresses = "BCC",
                Subject = "Subj",
                Body = "Some text",
                IsActive = true,
                AttachedDownloadId = 3,
                EmailAccountId = 1,
                LimitedToStores = true,
                DelayBeforeSend = 2,
                DelayPeriodId = 0
            };
        }

        public static NewsLetterSubscription GetTestNewsLetterSubscription(this PersistenceTest test)
        {
            return new NewsLetterSubscription
            {
                Email = "me@yourstore.com",
                NewsLetterSubscriptionGuid = _newsLetterSubscriptionGuid,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                StoreId = 1,
                Active = true
            };
        }

        public static QueuedEmail GetTestQueuedEmail(this PersistenceTest test)
        {
            return new QueuedEmail
            {
                PriorityId = 5,
                From = "From",
                FromName = "FromName",
                To = "To",
                ToName = "ToName",
                ReplyTo = "ReplyTo",
                ReplyToName = "ReplyToName",
                CC = "CC",
                Bcc = "BCC",
                Subject = "Subject",
                Body = "Body",
                AttachmentFilePath = "some file path",
                AttachmentFileName = "some file name",
                AttachedDownloadId = 3,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                SentTries = 5,
                SentOnUtc = new DateTime(2010, 02, 02),
                DontSendBeforeDateUtc = new DateTime(2016, 2, 23)
            };
        }

        #endregion

        #region News

        public static NewsComment GetTestNewsComment(this PersistenceTest test)
        {
            return new NewsComment
            {
                CommentText = "Comment text 1",
                IsApproved = false,
                CreatedOnUtc = new DateTime(2010, 01, 03),
                Customer = test.GetTestCustomer(),
                Store = test.GetTestStore()
            };
        }

        public static NewsItem GetTestNewsItem(this PersistenceTest test)
        {
            return new NewsItem
            {
                Title = "Title 1",
                Short = "Short 1",
                Full = "Full 1",
                Published = true,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                AllowComments = true,
                LimitedToStores = true,
                CreatedOnUtc = new DateTime(2010, 01, 03),
                MetaTitle = "MetaTitle 1",
                MetaDescription = "MetaDescription 1",
                MetaKeywords = "MetaKeywords 1"
            };
        }

        #endregion

        #region Orders

        public static CheckoutAttribute GetTestCheckoutAttribute(this PersistenceTest test)
        {
            return new CheckoutAttribute
            {
                Name = "Name 1",
                TextPrompt = "TextPrompt 1",
                IsRequired = true,
                ShippableProductRequired = true,
                IsTaxExempt = true,
                TaxCategoryId = 1,
                AttributeControlType = AttributeControlType.Datepicker,
                DisplayOrder = 2,
                LimitedToStores = true,
                ValidationMinLength = 3,
                ValidationMaxLength = 4,
                ValidationFileAllowedExtensions = "ValidationFileAllowedExtensions 1",
                ValidationFileMaximumSize = 5,
                DefaultValue = "DefaultValue 1",
                ConditionAttributeXml = "ConditionAttributeXml 1"
            };
        }

        public static CheckoutAttributeValue GetTestCheckoutAttributeValue(this PersistenceTest test)
        {
            return new CheckoutAttributeValue
            {
                Name = "Name 2",
                PriceAdjustment = 1,
                WeightAdjustment = 2,
                IsPreSelected = true,
                DisplayOrder = 3,
            };
        }

        public static GiftCard GetTestGiftCard(this PersistenceTest test)
        {
            return new GiftCard
            {
                GiftCardType = GiftCardType.Physical,
                Amount = 1.1M,
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

        public static GiftCardUsageHistory GetTestGiftCardUsageHistory(this PersistenceTest test)
        {
            return new GiftCardUsageHistory
            {
                UsedValue = 1.1M,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                GiftCard = test.GetTestGiftCard()
            };
        }

        public static Order GetTestOrder(this PersistenceTest test)
        {
            return new Order
            {
                OrderGuid = _orderGuid,
                StoreId = 1,
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
                RefundedAmount = 13.1M,
                RewardPointsHistoryEntryId = 1,
                CheckoutAttributeDescription = "CheckoutAttributeDescription1",
                CheckoutAttributesXml = "CheckoutAttributesXml1",
                CustomerLanguageId = 14,
                CustomerIp = "CustomerIp1",
                AllowStoringCreditCardNumber = true,
                CardType = "Visa",
                CardName = "John Smith",
                CardNumber = "4111111111111111",
                MaskedCreditCardNumber = "************1111",
                CardCvv2 = "123",
                CardExpirationMonth = "12",
                CardExpirationYear = "2010",
                AuthorizationTransactionId = "AuthorizationTransactionId1",
                AuthorizationTransactionCode = "AuthorizationTransactionCode1",
                AuthorizationTransactionResult = "AuthorizationTransactionResult1",
                CaptureTransactionId = "CaptureTransactionId1",
                CaptureTransactionResult = "CaptureTransactionResult1",
                SubscriptionTransactionId = "SubscriptionTransactionId1",
                PaidDateUtc = new DateTime(2010, 01, 01),
                ShippingAddress = null,
                ShippingMethod = "ShippingMethod1",
                ShippingRateComputationMethodSystemName = "ShippingRateComputationMethodSystemName1",
                PickUpInStore = true,
                CustomValuesXml = "CustomValuesXml1",
                Deleted = false,
                CreatedOnUtc = new DateTime(2010, 01, 04),
                CustomOrderNumber = "1",
                BillingAddress = test.GetTestAddress()
            };
        }

        public static OrderItem GetTestOrderItem(this PersistenceTest test)
        {
            return new OrderItem
            {
                Product = test.GetTestProduct(),
                Quantity = 1,
                UnitPriceInclTax = 1.1M,
                UnitPriceExclTax = 2.1M,
                PriceInclTax = 3.1M,
                PriceExclTax = 4.1M,
                DiscountAmountInclTax = 5.1M,
                DiscountAmountExclTax = 6.1M,
                OriginalProductCost = 7.1M,
                AttributeDescription = "AttributeDescription1",
                AttributesXml = "AttributesXml1",
                DownloadCount = 7,
                IsDownloadActivated = true,
                LicenseDownloadId = 8,
                ItemWeight = 9.87M,
                RentalStartDateUtc = new DateTime(2010, 01, 01),
                RentalEndDateUtc = new DateTime(2010, 01, 02)
            };
        }

        public static OrderNote GetTestOrderNote(this PersistenceTest test)
        {
            return new OrderNote
            {
                Note = "Note1",
                DownloadId = 1,
                DisplayToCustomer = true,
                CreatedOnUtc = new DateTime(2010, 01, 01)
            };
        }

        public static RecurringPayment GetTestRecurringPayment(this PersistenceTest test)
        {
            return new RecurringPayment
            {
                CycleLength = 1,
                CyclePeriod = RecurringProductCyclePeriod.Days,
                TotalCycles = 3,
                StartDateUtc = new DateTime(2010, 01, 01),
                IsActive = true,
                Deleted = true,
                CreatedOnUtc = new DateTime(2010, 01, 02),
                InitialOrder = test.GetTestOrder()
            };
        }

        public static RecurringPaymentHistory GetTestRecurringPaymentHistory(this PersistenceTest test)
        {
            return new RecurringPaymentHistory
            {
                CreatedOnUtc = new DateTime(2010, 01, 03)
            };
        }

        public static ReturnRequest GetTestReturnRequest(this PersistenceTest test)
        {
            return new ReturnRequest
            {
                CustomNumber = "CustomNumber 1",
                StoreId = 1,
                Customer = test.GetTestCustomer(),
                Quantity = 2,
                ReasonForReturn = "Wrong product",
                RequestedAction = "Refund",
                CustomerComments = "Some comment",
                StaffNotes = "Some notes",
                ReturnRequestStatus = ReturnRequestStatus.ItemsRefunded,
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02),
            };
        }

        public static ReturnRequestAction GetTestReturnRequestAction(this PersistenceTest test)
        {
            return new ReturnRequestAction
            {
                Name = "Name 1",
                DisplayOrder = 1
            };
        }

        public static ReturnRequestReason GetTestReturnRequestReason(this PersistenceTest test)
        {
            return new ReturnRequestReason
            {
                Name = "Name 1",
                DisplayOrder = 1
            };
        }

        public static ShoppingCartItem GetTestShoppingCartItem(this PersistenceTest test)
        {
            return new ShoppingCartItem
            {
                StoreId = 1,
                ShoppingCartType = ShoppingCartType.ShoppingCart,
                AttributesXml = "AttributesXml 1",
                CustomerEnteredPrice = 1.1M,
                Quantity = 2,
                RentalStartDateUtc = new DateTime(2010, 01, 03),
                RentalEndDateUtc = new DateTime(2010, 01, 04),
                CreatedOnUtc = new DateTime(2010, 01, 01),
                UpdatedOnUtc = new DateTime(2010, 01, 02)
            };
        }
        
        #endregion

        #region Polls

        public static Poll GetTestPoll(this PersistenceTest test)
        {
            return new Poll
            {
                Name = "Name 1",
                SystemKeyword = "SystemKeyword 1",
                Published = true,
                ShowOnHomePage = true,
                DisplayOrder = 1,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02)
            };
        }

        public static PollAnswer GetTestPollAnswer(this PersistenceTest test)
        {
            return new PollAnswer
            {
                Name = "Answer 1",
                NumberOfVotes = 1,
                DisplayOrder = 2
            };
        }

        public static PollVotingRecord GetTestPollVotingRecord(this PersistenceTest test)
        {
            return new PollVotingRecord
            {
                Customer = test.GetTestCustomer(),
                CreatedOnUtc = DateTime.UtcNow
            };
        }

        #endregion

        #region Security

        public static AclRecord GetTestAclRecord(this PersistenceTest test)
        {
            return new AclRecord
            {
                EntityId = 1,
                EntityName = "EntityName 1"
            };
        }

        public static PermissionRecord GetTestPermissionRecord(this PersistenceTest test)
        {
            return new PermissionRecord
            {
                Name = "Name 1",
                SystemName = "SystemName 2",
                Category = "Category 4",
            };
        }
        
        #endregion

        #region Seo

        public static UrlRecord UrlRecord(this PersistenceTest test)
        {
            return new UrlRecord
            {
                EntityId = 1,
                EntityName = "EntityName 1",
                Slug = "Slug 1",
                LanguageId = 2
            };
        }

        #endregion

        #region Shipping

        public static DeliveryDate GetTestDeliveryDate(this PersistenceTest test)
        {
            return new DeliveryDate
            {
                Name = "Name 1",
                DisplayOrder = 1
            };
        }

        public static ProductAvailabilityRange GetTestProductAvailabilityRange(this PersistenceTest test)
        {
            return new ProductAvailabilityRange
            {
                Name = "product availability range",
                DisplayOrder = 1
            };
        }

        public static Shipment GetTestShipment(this PersistenceTest test)
        {
            return new Shipment
            {
                Order = test.GetTestOrder(),
                TrackingNumber = "TrackingNumber 1",
                ShippedDateUtc = new DateTime(2010, 01, 01),
                DeliveryDateUtc = new DateTime(2010, 01, 02),
                CreatedOnUtc = new DateTime(2010, 01, 03)
            };
        }

        public static ShipmentItem GetTestShipmentItem(this PersistenceTest test)
        {
            return new ShipmentItem
            {
                OrderItemId = 2,
                Quantity = 3,
                WarehouseId = 4
            };
        }

        public static ShippingMethod GetTestShippingMethod(this PersistenceTest test)
        {
            return new ShippingMethod
            {
                Name = "By train",
            };
        }
        
        public static Warehouse GetTestWarehouse(this PersistenceTest test)
        {
            return new Warehouse
            {
                Name = "Name 2",
                AddressId = 1,
            };
        }

        #endregion

        #region Stores

        public static Store GetTestStore(this PersistenceTest test)
        {
            return new Store
            {
                Name = "Computer store",
                Url = "http://www.yourStore.com",
                Hosts = "yourStore.com,www.yourStore.com",
                DefaultLanguageId = 1,
                DisplayOrder = 2,
                CompanyName = "company name",
                CompanyAddress = "some address",
                CompanyPhoneNumber = "123456789",
                CompanyVat = "some vat",
            };
        }

        public static StoreMapping GetTestStoreMapping(this PersistenceTest test)
        {
            return new StoreMapping
            {
                EntityId = 1,
                EntityName = "EntityName 1"
            };
        }

        #endregion

        #region Tasks

        public static ScheduleTask GetTestScheduleTask(this PersistenceTest test)
        {
            return new ScheduleTask
            {
                Name = "Task 1",
                Seconds = 1,
                Type = "some type 1",
                Enabled = true,
                StopOnError = true,
                LastStartUtc = new DateTime(2010, 01, 01),
                LastEndUtc = new DateTime(2010, 01, 02),
                LastSuccessUtc = new DateTime(2010, 01, 03)
            };
        }

        #endregion

        #region Tax

        public static TaxCategory GetTestTaxCategory(this PersistenceTest test)
        {
            return new TaxCategory
            {
                Name = "Books",
                DisplayOrder = 1
            };
        }

        #endregion

        #region Topics

        public static Topic GetTestTopic(this PersistenceTest test)
        {
            return new Topic
            {
                SystemName = "SystemName 1",
                IncludeInSitemap = true,
                IncludeInTopMenu = true,
                IncludeInFooterColumn1 = true,
                IncludeInFooterColumn2 = true,
                IncludeInFooterColumn3 = true,
                DisplayOrder = 1,
                AccessibleWhenStoreClosed = true,
                IsPasswordProtected = true,
                Password = "password",
                Title = "Title 1",
                Body = "Body 1",
                Published = true,
                TopicTemplateId = 1,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                SubjectToAcl = true,
                LimitedToStores = true
            };
        }

        public static TopicTemplate GetTestTopicTemplate(this PersistenceTest test)
        {
            return new TopicTemplate
            {
                Name = "Name 1",
                ViewPath = "ViewPath 1",
                DisplayOrder = 1,
            };
        }

        #endregion
        
        #region Vendors

        public static Vendor GetTestVendor(this PersistenceTest test)
        {
            return new Vendor
            {
                Name = "Name 1",
                Email = "Email 1",
                Description = "Description 1",
                AdminComment = "AdminComment 1",
                PictureId = 1,
                Active = true,
                Deleted = true,
                DisplayOrder = 2,
                MetaKeywords = "Meta keywords",
                MetaDescription = "Meta description",
                MetaTitle = "Meta title",
                PageSize = 4,
                AllowCustomersToSelectPageSize = true,
                PageSizeOptions = "4, 2, 8, 12"
            };
        }

        public static VendorNote GetTestVendorNote(this PersistenceTest test)
        {
            return new VendorNote
            {
                Note = "Note1",
                CreatedOnUtc = new DateTime(2010, 01, 01),
            };
        }

        #endregion
    }
}
