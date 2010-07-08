//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Blog;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.Content.NewsManagement;
using NopSolutions.NopCommerce.BusinessLogic.Content.Polls;
using NopSolutions.NopCommerce.BusinessLogic.Content.Topics;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Campaigns;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.BusinessLogic.Warehouses;
using NopSolutions.NopCommerce.BusinessLogic.QuickBooks;

namespace NopSolutions.NopCommerce.BusinessLogic.Data
{
    /// <summary>
    /// Represents a nopCommerce object context
    /// </summary>
    public partial class NopObjectContext : ObjectContext
    {
        #region Fields

        private readonly Dictionary<Type, object> _entitySets;

        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the NopObjectContext class
        /// </summary>
        public NopObjectContext()
            : this("name=NopEntities")
        {

        }

        /// <summary>
        /// Creates a new instance of the NopObjectContext class
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        public NopObjectContext(string connectionString)
            : base(connectionString, "NopEntities")
        {
            _entitySets = new Dictionary<Type, object>();
            this.ContextOptions.LazyLoadingEnabled = true;
        }
        #endregion

        #region Object sets

        public ObjectSet<T> EntitySet<T>()
            where T : BaseEntity
        {
            var t = typeof(T);
            object match;

            if (!_entitySets.TryGetValue(t, out match))
            {
                match = CreateObjectSet<T>();
                _entitySets.Add(t, match);
            }

            return (ObjectSet<T>)match;
        }

        public ObjectSet<ACL> ACL
        {
            get
            {
                if ((_acl == null))
                {
                    _acl = CreateObjectSet<ACL>();
                }
                return _acl;
            }
        }
        private ObjectSet<ACL> _acl;

        public ObjectSet<ActivityLog> ActivityLog
        {
            get
            {
                if ((_activityLog == null))
                {
                    _activityLog = CreateObjectSet<ActivityLog>();
                }
                return _activityLog;
            }
        }
        private ObjectSet<ActivityLog> _activityLog;

        public ObjectSet<ActivityLogType> ActivityLogTypes
        {
            get
            {
                if ((_activityLogTypes == null))
                {
                    _activityLogTypes = CreateObjectSet<ActivityLogType>();
                }
                return _activityLogTypes;
            }
        }
        private ObjectSet<ActivityLogType> _activityLogTypes;

        public ObjectSet<Address> Addresses
        {
            get
            {
                if ((_addresses == null))
                {
                    _addresses = CreateObjectSet<Address>();
                }
                return _addresses;
            }
        }
        private ObjectSet<Address> _addresses;

        public ObjectSet<Affiliate> Affiliates
        {
            get
            {
                if ((_affiliates == null))
                {
                    _affiliates = CreateObjectSet<Affiliate>();
                }
                return _affiliates;
            }
        }
        private ObjectSet<Affiliate> _affiliates;

        public ObjectSet<BannedIpAddress> BannedIpAddresses
        {
            get
            {
                if ((_bannedIpAddresses == null))
                {
                    _bannedIpAddresses = CreateObjectSet<BannedIpAddress>();
                }
                return _bannedIpAddresses;
            }
        }
        private ObjectSet<BannedIpAddress> _bannedIpAddresses;

        public ObjectSet<BannedIpNetwork> BannedIpNetworks
        {
            get
            {
                if ((_bannedIpNetworks == null))
                {
                    _bannedIpNetworks = CreateObjectSet<BannedIpNetwork>();
                }
                return _bannedIpNetworks;
            }
        }
        private ObjectSet<BannedIpNetwork> _bannedIpNetworks;

        public ObjectSet<BlogPost> BlogPosts
        {
            get
            {
                if ((_blogPosts == null))
                {
                    _blogPosts = CreateObjectSet<BlogPost>();
                }
                return _blogPosts;
            }
        }
        private ObjectSet<BlogPost> _blogPosts;

        public ObjectSet<BlogComment> BlogComments
        {
            get
            {
                if ((_blogComments == null))
                {
                    _blogComments = CreateObjectSet<BlogComment>();
                }
                return _blogComments;
            }
        }
        private ObjectSet<BlogComment> _blogComments;

        public ObjectSet<Campaign> Campaigns
        {
            get
            {
                if ((_campaigns == null))
                {
                    _campaigns = CreateObjectSet<Campaign>();
                }
                return _campaigns;
            }
        }
        private ObjectSet<Campaign> _campaigns;

        public ObjectSet<CategoryLocalized> CategoryLocalized
        {
            get
            {
                if ((_categoryLocalized == null))
                {
                    _categoryLocalized = CreateObjectSet<CategoryLocalized>();
                }
                return _categoryLocalized;
            }
        }
        private ObjectSet<CategoryLocalized> _categoryLocalized;

        public ObjectSet<Category> Categories
        {
            get
            {
                if ((_categories == null))
                {
                    _categories = CreateObjectSet<Category>();
                }
                return _categories;
            }
        }
        private ObjectSet<Category> _categories;
        
        public ObjectSet<CategoryTemplate> CategoryTemplates
        {
            get
            {
                if ((_categoryTemplates == null))
                {
                    _categoryTemplates = CreateObjectSet<CategoryTemplate>();
                }
                return _categoryTemplates;
            }
        }
        private ObjectSet<CategoryTemplate> _categoryTemplates;

        public ObjectSet<CheckoutAttributeLocalized> CheckoutAttributeLocalized
        {
            get
            {
                if ((_checkoutAttributeLocalized == null))
                {
                    _checkoutAttributeLocalized = CreateObjectSet<CheckoutAttributeLocalized>();
                }
                return _checkoutAttributeLocalized;
            }
        }
        private ObjectSet<CheckoutAttributeLocalized> _checkoutAttributeLocalized;

        public ObjectSet<CheckoutAttribute> CheckoutAttributes
        {
            get
            {
                if ((_checkoutAttributes == null))
                {
                    _checkoutAttributes = CreateObjectSet<CheckoutAttribute>();
                }
                return _checkoutAttributes;
            }
        }
        private ObjectSet<CheckoutAttribute> _checkoutAttributes;

        public ObjectSet<CheckoutAttributeValueLocalized> CheckoutAttributeValueLocalized
        {
            get
            {
                if ((_checkoutAttributeValueLocalized == null))
                {
                    _checkoutAttributeValueLocalized = CreateObjectSet<CheckoutAttributeValueLocalized>();
                }
                return _checkoutAttributeValueLocalized;
            }
        }
        private ObjectSet<CheckoutAttributeValueLocalized> _checkoutAttributeValueLocalized;

        public ObjectSet<CheckoutAttributeValue> CheckoutAttributeValues
        {
            get
            {
                if ((_checkoutAttributeValues == null))
                {
                    _checkoutAttributeValues = CreateObjectSet<CheckoutAttributeValue>();
                }
                return _checkoutAttributeValues;
            }
        }
        private ObjectSet<CheckoutAttributeValue> _checkoutAttributeValues;

        public ObjectSet<Country> Countries
        {
            get
            {
                if ((_countries == null))
                {
                    _countries = CreateObjectSet<Country>();
                }
                return _countries;
            }
        }
        private ObjectSet<Country> _countries;

        public ObjectSet<CreditCardType> CreditCardTypes
        {
            get
            {
                if ((_creditCardTypes == null))
                {
                    _creditCardTypes = CreateObjectSet<CreditCardType>();
                }
                return _creditCardTypes;
            }
        }
        private ObjectSet<CreditCardType> _creditCardTypes;

        public ObjectSet<Currency> Currencies
        {
            get
            {
                if ((_currencies == null))
                {
                    _currencies = CreateObjectSet<Currency>();
                }
                return _currencies;
            }
        }
        private ObjectSet<Currency> _currencies;
        
        public ObjectSet<Customer> Customers
        {
            get
            {
                if ((_customers == null))
                {
                    _customers = CreateObjectSet<Customer>();
                }
                return _customers;
            }
        }
        private ObjectSet<Customer> _customers;

        public ObjectSet<CustomerAction> CustomerActions
        {
            get
            {
                if ((_customerActions == null))
                {
                    _customerActions = CreateObjectSet<CustomerAction>();
                }
                return _customerActions;
            }
        }
        private ObjectSet<CustomerAction> _customerActions;

        public ObjectSet<CustomerAttribute> CustomerAttributes
        {
            get
            {
                if ((_customerAttributes == null))
                {
                    _customerAttributes = CreateObjectSet<CustomerAttribute>();
                }
                return _customerAttributes;
            }
        }
        private ObjectSet<CustomerAttribute> _customerAttributes;

        public ObjectSet<CustomerRole> CustomerRoles
        {
            get
            {
                if ((_customerRoles == null))
                {
                    _customerRoles = CreateObjectSet<CustomerRole>();
                }
                return _customerRoles;
            }
        }
        private ObjectSet<CustomerRole> _customerRoles;

        public ObjectSet<CustomerRoleProductPrice> CustomerRoleProductPrices
        {
            get
            {
                if ((_customerRoleProductPrices == null))
                {
                    _customerRoleProductPrices = CreateObjectSet<CustomerRoleProductPrice>();
                }
                return _customerRoleProductPrices;
            }
        }
        private ObjectSet<CustomerRoleProductPrice> _customerRoleProductPrices;

        public ObjectSet<CustomerSession> CustomerSessions
        {
            get
            {
                if ((_customerSessions == null))
                {
                    _customerSessions = CreateObjectSet<CustomerSession>();
                }
                return _customerSessions;
            }
        }
        private ObjectSet<CustomerSession> _customerSessions;

        public ObjectSet<DiscountLimitation> DiscountLimitations
        {
            get
            {
                if ((_discountLimitations == null))
                {
                    _discountLimitations = CreateObjectSet<DiscountLimitation>();
                }
                return _discountLimitations;
            }
        }
        private ObjectSet<DiscountLimitation> _discountLimitations;

        public ObjectSet<DiscountRequirement> DiscountRequirements
        {
            get
            {
                if ((_discountRequirements == null))
                {
                    _discountRequirements = CreateObjectSet<DiscountRequirement>();
                }
                return _discountRequirements;
            }
        }
        private ObjectSet<DiscountRequirement> _discountRequirements;

        public ObjectSet<Discount> Discounts
        {
            get
            {
                if ((_discounts == null))
                {
                    _discounts = CreateObjectSet<Discount>();
                }
                return _discounts;
            }
        }
        private ObjectSet<Discount> _discounts;

        public ObjectSet<DiscountType> DiscountTypes
        {
            get
            {
                if ((_discountTypes == null))
                {
                    _discountTypes = CreateObjectSet<DiscountType>();
                }
                return _discountTypes;
            }
        }
        private ObjectSet<DiscountType> _discountTypes;

        public ObjectSet<DiscountUsageHistory> DiscountUsageHistory
        {
            get
            {
                if ((_discountUsageHistory == null))
                {
                    _discountUsageHistory = CreateObjectSet<DiscountUsageHistory>();
                }
                return _discountUsageHistory;
            }
        }
        private ObjectSet<DiscountUsageHistory> _discountUsageHistory;

        public ObjectSet<Download> Downloads
        {
            get
            {
                if ((_downloads == null))
                {
                    _downloads = CreateObjectSet<Download>();
                }
                return _downloads;
            }
        }
        private ObjectSet<Download> _downloads;

        public ObjectSet<Forum> Forums
        {
            get
            {
                if ((_forums == null))
                {
                    _forums = CreateObjectSet<Forum>();
                }
                return _forums;
            }
        }
        private ObjectSet<Forum> _forums;

        public ObjectSet<ForumGroup> ForumGroups
        {
            get
            {
                if ((_forumGroups == null))
                {
                    _forumGroups = CreateObjectSet<ForumGroup>();
                }
                return _forumGroups;
            }
        }
        private ObjectSet<ForumGroup> _forumGroups;

        public ObjectSet<ForumPost> ForumPosts
        {
            get
            {
                if ((_forumPosts == null))
                {
                    _forumPosts = CreateObjectSet<ForumPost>();
                }
                return _forumPosts;
            }
        }
        private ObjectSet<ForumPost> _forumPosts;

        public ObjectSet<ForumSubscription> ForumSubscriptions
        {
            get
            {
                if ((_forumSubscriptions == null))
                {
                    _forumSubscriptions = CreateObjectSet<ForumSubscription>();
                }
                return _forumSubscriptions;
            }
        }
        private ObjectSet<ForumSubscription> _forumSubscriptions;

        public ObjectSet<ForumTopic> ForumTopics
        {
            get
            {
                if ((_forumTopics == null))
                {
                    _forumTopics = CreateObjectSet<ForumTopic>();
                }
                return _forumTopics;
            }
        }
        private ObjectSet<ForumTopic> _forumTopics;

        public ObjectSet<GiftCard> GiftCards
        {
            get
            {
                if ((_giftCards == null))
                {
                    _giftCards = CreateObjectSet<GiftCard>();
                }
                return _giftCards;
            }
        }
        private ObjectSet<GiftCard> _giftCards;

        public ObjectSet<GiftCardUsageHistory> GiftCardUsageHistory
        {
            get
            {
                if ((_giftCardUsageHistory == null))
                {
                    _giftCardUsageHistory = CreateObjectSet<GiftCardUsageHistory>();
                }
                return _giftCardUsageHistory;
            }
        }
        private ObjectSet<GiftCardUsageHistory> _giftCardUsageHistory;

        public ObjectSet<Language> Languages
        {
            get
            {
                if ((_languages == null))
                {
                    _languages = CreateObjectSet<Language>();
                }
                return _languages;
            }
        }
        private ObjectSet<Language> _languages;

        public ObjectSet<LocaleStringResource> LocaleStringResources
        {
            get
            {
                if ((_localeStringResources == null))
                {
                    _localeStringResources = CreateObjectSet<LocaleStringResource>();
                }
                return _localeStringResources;
            }
        }
        private ObjectSet<LocaleStringResource> _localeStringResources;

        public ObjectSet<LocalizedMessageTemplate> LocalizedMessageTemplates
        {
            get
            {
                if ((_localizedMessageTemplates == null))
                {
                    _localizedMessageTemplates = CreateObjectSet<LocalizedMessageTemplate>();
                }
                return _localizedMessageTemplates;
            }
        }
        private ObjectSet<LocalizedMessageTemplate> _localizedMessageTemplates;

        public ObjectSet<LocalizedTopic> LocalizedTopics
        {
            get
            {
                if ((_localizedTopics == null))
                {
                    _localizedTopics = CreateObjectSet<LocalizedTopic>();
                }
                return _localizedTopics;
            }
        }
        private ObjectSet<LocalizedTopic> _localizedTopics;

        public ObjectSet<Log> Log
        {
            get
            {
                if ((_log == null))
                {
                    _log = CreateObjectSet<Log>();
                }
                return _log;
            }
        }
        private ObjectSet<Log> _log;

        public ObjectSet<LogType> LogTypes
        {
            get
            {
                if ((_logType == null))
                {
                    _logType = CreateObjectSet<LogType>();
                }
                return _logType;
            }
        }
        private ObjectSet<LogType> _logType;

        public ObjectSet<LowStockActivity> LowStockActivities
        {
            get
            {
                if ((_lowStockActivities == null))
                {
                    _lowStockActivities = CreateObjectSet<LowStockActivity>();
                }
                return _lowStockActivities;
            }
        }
        private ObjectSet<LowStockActivity> _lowStockActivities;

        public ObjectSet<ManufacturerLocalized> ManufacturerLocalized
        {
            get
            {
                if ((_manufacturerLocalized == null))
                {
                    _manufacturerLocalized = CreateObjectSet<ManufacturerLocalized>();
                }
                return _manufacturerLocalized;
            }
        }
        private ObjectSet<ManufacturerLocalized> _manufacturerLocalized;

        public ObjectSet<Manufacturer> Manufacturers
        {
            get
            {
                if ((_manufacturers == null))
                {
                    _manufacturers = CreateObjectSet<Manufacturer>();
                }
                return _manufacturers;
            }
        }
        private ObjectSet<Manufacturer> _manufacturers;

        public ObjectSet<ManufacturerTemplate> ManufacturerTemplates
        {
            get
            {
                if ((_manufacturerTemplates == null))
                {
                    _manufacturerTemplates = CreateObjectSet<ManufacturerTemplate>();
                }
                return _manufacturerTemplates;
            }
        }
        private ObjectSet<ManufacturerTemplate> _manufacturerTemplates;

        public ObjectSet<MeasureDimension> MeasureDimensions
        {
            get
            {
                if ((_measureDimensions == null))
                {
                    _measureDimensions = CreateObjectSet<MeasureDimension>();
                }
                return _measureDimensions;
            }
        }
        private ObjectSet<MeasureDimension> _measureDimensions;

        public ObjectSet<MeasureWeight> MeasureWeights
        {
            get
            {
                if ((_measureWeights == null))
                {
                    _measureWeights = CreateObjectSet<MeasureWeight>();
                }
                return _measureWeights;
            }
        }
        private ObjectSet<MeasureWeight> _measureWeights;

        public ObjectSet<MessageTemplate> MessageTemplates
        {
            get
            {
                if ((_messageTemplates == null))
                {
                    _messageTemplates = CreateObjectSet<MessageTemplate>();
                }
                return _messageTemplates;
            }
        }
        private ObjectSet<MessageTemplate> _messageTemplates;

        public ObjectSet<News> News
        {
            get
            {
                if ((_news == null))
                {
                    _news = CreateObjectSet<News>();
                }
                return _news;
            }
        }
        private ObjectSet<News> _news;

        public ObjectSet<NewsComment> NewsComments
        {
            get
            {
                if ((_newsComments == null))
                {
                    _newsComments = CreateObjectSet<NewsComment>();
                }
                return _newsComments;
            }
        }
        private ObjectSet<NewsComment> _newsComments;

        public ObjectSet<NewsLetterSubscription> NewsLetterSubscriptions
        {
            get
            {
                if ((_newsLetterSubscriptions == null))
                {
                    _newsLetterSubscriptions = CreateObjectSet<NewsLetterSubscription>();
                }
                return _newsLetterSubscriptions;
            }
        }
        private ObjectSet<NewsLetterSubscription> _newsLetterSubscriptions;

        public ObjectSet<OrderNote> OrderNotes
        {
            get
            {
                if ((_ordernotes == null))
                {
                    _ordernotes = CreateObjectSet<OrderNote>();
                }
                return _ordernotes;
            }
        }
        private ObjectSet<OrderNote> _ordernotes;

        public ObjectSet<OrderProductVariant> OrderProductVariants
        {
            get
            {
                if ((_orderProductVariants == null))
                {
                    _orderProductVariants = CreateObjectSet<OrderProductVariant>();
                }
                return _orderProductVariants;
            }
        }
        private ObjectSet<OrderProductVariant> _orderProductVariants;

        public ObjectSet<Order> Orders
        {
            get
            {
                if ((_orders == null))
                {
                    _orders = CreateObjectSet<Order>();
                }
                return _orders;
            }
        }
        private ObjectSet<Order> _orders;

        public ObjectSet<OrderStatus> OrderStatuses
        {
            get
            {
                if ((_orderStatuses == null))
                {
                    _orderStatuses = CreateObjectSet<OrderStatus>();
                }
                return _orderStatuses;
            }
        }
        private ObjectSet<OrderStatus> _orderStatuses;

        public ObjectSet<PaymentMethod> PaymentMethods
        {
            get
            {
                if ((_paymentMethods == null))
                {
                    _paymentMethods = CreateObjectSet<PaymentMethod>();
                }
                return _paymentMethods;
            }
        }
        private ObjectSet<PaymentMethod> _paymentMethods;

        public ObjectSet<PaymentStatus> PaymentStatuses
        {
            get
            {
                if ((_paymentStatuses == null))
                {
                    _paymentStatuses = CreateObjectSet<PaymentStatus>();
                }
                return _paymentStatuses;
            }
        }
        private ObjectSet<PaymentStatus> _paymentStatuses;

        public ObjectSet<Picture> Pictures
        {
            get
            {
                if ((_pictures== null))
                {
                    _pictures = CreateObjectSet<Picture>();
                }
                return _pictures;
            }
        }
        private ObjectSet<Picture> _pictures;

        public ObjectSet<Poll> Polls
        {
            get
            {
                if ((_polls == null))
                {
                    _polls = CreateObjectSet<Poll>();
                }
                return _polls;
            }
        }
        private ObjectSet<Poll> _polls;

        public ObjectSet<PollAnswer> PollAnswers
        {
            get
            {
                if ((_pollAnswers == null))
                {
                    _pollAnswers = CreateObjectSet<PollAnswer>();
                }
                return _pollAnswers;
            }
        }
        private ObjectSet<PollAnswer> _pollAnswers;

        public ObjectSet<PollVotingRecord> PollVotingRecords
        {
            get
            {
                if ((_pollVotingRecords == null))
                {
                    _pollVotingRecords = CreateObjectSet<PollVotingRecord>();
                }
                return _pollVotingRecords;
            }
        }
        private ObjectSet<PollVotingRecord> _pollVotingRecords;

        public ObjectSet<Pricelist> Pricelists
        {
            get
            {
                if ((_pricelists == null))
                {
                    _pricelists = CreateObjectSet<Pricelist>();
                }
                return _pricelists;
            }
        }
        private ObjectSet<Pricelist> _pricelists;

        public ObjectSet<PrivateMessage> PrivateMessages
        {
            get
            {
                if ((_privateMessagess == null))
                {
                    _privateMessagess = CreateObjectSet<PrivateMessage>();
                }
                return _privateMessagess;
            }
        }
        private ObjectSet<PrivateMessage> _privateMessagess;

        public ObjectSet<ProductAttributeLocalized> ProductAttributeLocalized
        {
            get
            {
                if ((_productAttributeLocalized == null))
                {
                    _productAttributeLocalized = CreateObjectSet<ProductAttributeLocalized>();
                }
                return _productAttributeLocalized;
            }
        }
        private ObjectSet<ProductAttributeLocalized> _productAttributeLocalized;

        public ObjectSet<ProductAttribute> ProductAttributes
        {
            get
            {
                if ((_productAttributes == null))
                {
                    _productAttributes = CreateObjectSet<ProductAttribute>();
                }
                return _productAttributes;
            }
        }
        private ObjectSet<ProductAttribute> _productAttributes;

        public ObjectSet<ProductCategory> ProductCategories
        {
            get
            {
                if ((_productCategories == null))
                {
                    _productCategories = CreateObjectSet<ProductCategory>();
                }
                return _productCategories;
            }
        }
        private ObjectSet<ProductCategory> _productCategories;

        public ObjectSet<ProductLocalized> ProductLocalized
        {
            get
            {
                if ((_productLocalized == null))
                {
                    _productLocalized = CreateObjectSet<ProductLocalized>();
                }
                return _productLocalized;
            }
        }
        private ObjectSet<ProductLocalized> _productLocalized;

        public ObjectSet<ProductManufacturer> ProductManufacturers
        {
            get
            {
                if ((_productManufacturers == null))
                {
                    _productManufacturers = CreateObjectSet<ProductManufacturer>();
                }
                return _productManufacturers;
            }
        }
        private ObjectSet<ProductManufacturer> _productManufacturers;

        public ObjectSet<ProductPicture> ProductPictures
        {
            get
            {
                if ((_productPictures == null))
                {
                    _productPictures = CreateObjectSet<ProductPicture>();
                }
                return _productPictures;
            }
        }
        private ObjectSet<ProductPicture> _productPictures;

        public ObjectSet<ProductRating> ProductRatings
        {
            get
            {
                if ((_productRatings == null))
                {
                    _productRatings = CreateObjectSet<ProductRating>();
                }
                return _productRatings;
            }
        }
        private ObjectSet<ProductRating> _productRatings;

        public ObjectSet<ProductReviewHelpfulness> ProductReviewHelpfulness
        {
            get
            {
                if ((_productReviewHelpfulness == null))
                {
                    _productReviewHelpfulness = CreateObjectSet<ProductReviewHelpfulness>();
                }
                return _productReviewHelpfulness;
            }
        }
        private ObjectSet<ProductReviewHelpfulness> _productReviewHelpfulness;

        public ObjectSet<ProductReview> ProductReviews
        {
            get
            {
                if ((_productReviews == null))
                {
                    _productReviews = CreateObjectSet<ProductReview>();
                }
                return _productReviews;
            }
        }
        private ObjectSet<ProductReview> _productReviews;

        public ObjectSet<Product> Products
        {
            get
            {
                if ((_products == null))
                {
                    _products = CreateObjectSet<Product>();
                }
                return _products;
            }
        }
        private ObjectSet<Product> _products;

        public ObjectSet<ProductSpecificationAttribute> ProductSpecificationAttributes
        {
            get
            {
                if ((_productSpecificationAttributes == null))
                {
                    _productSpecificationAttributes = CreateObjectSet<ProductSpecificationAttribute>();
                }
                return _productSpecificationAttributes;
            }
        }
        private ObjectSet<ProductSpecificationAttribute> _productSpecificationAttributes;

        public ObjectSet<ProductTag> ProductTags
        {
            get
            {
                if ((productTags == null))
                {
                    productTags = CreateObjectSet<ProductTag>();
                }
                return productTags;
            }
        }
        private ObjectSet<ProductTag> productTags;

        public ObjectSet<ProductTemplate> ProductTemplates
        {
            get
            {
                if ((_productTemplates == null))
                {
                    _productTemplates = CreateObjectSet<ProductTemplate>();
                }
                return _productTemplates;
            }
        }
        private ObjectSet<ProductTemplate> _productTemplates;

        public ObjectSet<ProductType> ProductTypes
        {
            get
            {
                if ((_productTypes == null))
                {
                    _productTypes = CreateObjectSet<ProductType>();
                }
                return _productTypes;
            }
        }
        private ObjectSet<ProductType> _productTypes;

        public ObjectSet<ProductVariantAttributeCombination> ProductVariantAttributeCombinations
        {
            get
            {
                if ((_productVariantAttributeCombinations == null))
                {
                    _productVariantAttributeCombinations = CreateObjectSet<ProductVariantAttributeCombination>();
                }
                return _productVariantAttributeCombinations;
            }
        }
        private ObjectSet<ProductVariantAttributeCombination> _productVariantAttributeCombinations;

        public ObjectSet<ProductVariantAttribute> ProductVariantAttributes
        {
            get
            {
                if ((_productVariantAttributes == null))
                {
                    _productVariantAttributes = CreateObjectSet<ProductVariantAttribute>();
                }
                return _productVariantAttributes;
            }
        }
        private ObjectSet<ProductVariantAttribute> _productVariantAttributes;

        public ObjectSet<ProductVariantAttributeValueLocalized> ProductVariantAttributeValueLocalized
        {
            get
            {
                if ((_productVariantAttributeValueLocalized == null))
                {
                    _productVariantAttributeValueLocalized = CreateObjectSet<ProductVariantAttributeValueLocalized>();
                }
                return _productVariantAttributeValueLocalized;
            }
        }
        private ObjectSet<ProductVariantAttributeValueLocalized> _productVariantAttributeValueLocalized;

        public ObjectSet<ProductVariantAttributeValue> ProductVariantAttributeValues
        {
            get
            {
                if ((_productVariantAttributeValues == null))
                {
                    _productVariantAttributeValues = CreateObjectSet<ProductVariantAttributeValue>();
                }
                return _productVariantAttributeValues;
            }
        }
        private ObjectSet<ProductVariantAttributeValue> _productVariantAttributeValues;

        public ObjectSet<ProductVariantLocalized> ProductVariantLocalized
        {
            get
            {
                if ((_productVariantLocalized == null))
                {
                    _productVariantLocalized = CreateObjectSet<ProductVariantLocalized>();
                }
                return _productVariantLocalized;
            }
        }
        private ObjectSet<ProductVariantLocalized> _productVariantLocalized;

        public ObjectSet<ProductVariantPricelist> ProductVariantPricelists
        {
            get
            {
                if ((_productVariantPricelists == null))
                {
                    _productVariantPricelists = CreateObjectSet<ProductVariantPricelist>();
                }
                return _productVariantPricelists;
            }
        }
        private ObjectSet<ProductVariantPricelist> _productVariantPricelists;

        public ObjectSet<ProductVariant> ProductVariants
        {
            get
            {
                if ((_productVariants == null))
                {
                    _productVariants = CreateObjectSet<ProductVariant>();
                }
                return _productVariants;
            }
        }
        private ObjectSet<ProductVariant> _productVariants;

        public ObjectSet<QueuedEmail> QueuedEmails
        {
            get
            {
                if ((_queuedEmails == null))
                {
                    _queuedEmails = CreateObjectSet<QueuedEmail>();
                }
                return _queuedEmails;
            }
        }
        private ObjectSet<QueuedEmail> _queuedEmails;

        public ObjectSet<QBEntity> QBEntities
        {
            get
            {
                if ((_qbEntities == null))
                {
                    _qbEntities = CreateObjectSet<QBEntity>();
                }
                return _qbEntities;
            }
        }
        private ObjectSet<QBEntity> _qbEntities;

        public ObjectSet<RecurringPaymentHistory> RecurringPaymentHistory
        {
            get
            {
                if ((_recurringPaymentHistory == null))
                {
                    _recurringPaymentHistory = CreateObjectSet<RecurringPaymentHistory>();
                }
                return _recurringPaymentHistory;
            }
        }
        private ObjectSet<RecurringPaymentHistory> _recurringPaymentHistory;

        public ObjectSet<RecurringPayment> RecurringPayments
        {
            get
            {
                if ((_recurringPayments == null))
                {
                    _recurringPayments = CreateObjectSet<RecurringPayment>();
                }
                return _recurringPayments;
            }
        }
        private ObjectSet<RecurringPayment> _recurringPayments;

        public ObjectSet<RelatedProduct> RelatedProducts
        {
            get
            {
                if ((_relatedProducts == null))
                {
                    _relatedProducts = CreateObjectSet<RelatedProduct>();
                }
                return _relatedProducts;
            }
        }
        private ObjectSet<RelatedProduct> _relatedProducts;

        public ObjectSet<RewardPointsHistory> RewardPointsHistory
        {
            get
            {
                if ((_rewardPointsHistory == null))
                {
                    _rewardPointsHistory = CreateObjectSet<RewardPointsHistory>();
                }
                return _rewardPointsHistory;
            }
        }
        private ObjectSet<RewardPointsHistory> _rewardPointsHistory;

        public ObjectSet<SearchLog> SearchLog
        {
            get
            {
                if ((_searchLog == null))
                {
                    _searchLog = CreateObjectSet<SearchLog>();
                }
                return _searchLog;
            }
        }
        private ObjectSet<SearchLog> _searchLog;

        public ObjectSet<Setting> Settings
        {
            get
            {
                if ((_settings == null))
                {
                    _settings = CreateObjectSet<Setting>();
                }
                return _settings;
            }
        }
        private ObjectSet<Setting> _settings;

        public ObjectSet<ShippingByTotal> ShippingByTotal
        {
            get
            {
                if ((_shippingByTotal == null))
                {
                    _shippingByTotal = CreateObjectSet<ShippingByTotal>();
                }
                return _shippingByTotal;
            }
        }
        private ObjectSet<ShippingByTotal> _shippingByTotal;

        public ObjectSet<ShippingByWeight> ShippingByWeight
        {
            get
            {
                if ((_shippingByWeight == null))
                {
                    _shippingByWeight = CreateObjectSet<ShippingByWeight>();
                }
                return _shippingByWeight;
            }
        }
        private ObjectSet<ShippingByWeight> _shippingByWeight;

        public ObjectSet<ShippingByWeightAndCountry> ShippingByWeightAndCountry
        {
            get
            {
                if ((_shippingByWeightAndCountry == null))
                {
                    _shippingByWeightAndCountry = CreateObjectSet<ShippingByWeightAndCountry>();
                }
                return _shippingByWeightAndCountry;
            }
        }
        private ObjectSet<ShippingByWeightAndCountry> _shippingByWeightAndCountry;

        public ObjectSet<ShippingMethod> ShippingMethods
        {
            get
            {
                if ((_shippingMethods == null))
                {
                    _shippingMethods = CreateObjectSet<ShippingMethod>();
                }
                return _shippingMethods;
            }
        }
        private ObjectSet<ShippingMethod> _shippingMethods;

        public ObjectSet<ShippingRateComputationMethod> ShippingRateComputationMethods
        {
            get
            {
                if ((_shippingRateComputationMethods == null))
                {
                    _shippingRateComputationMethods = CreateObjectSet<ShippingRateComputationMethod>();
                }
                return _shippingRateComputationMethods;
            }
        }
        private ObjectSet<ShippingRateComputationMethod> _shippingRateComputationMethods;

        public ObjectSet<ShippingStatus> ShippingStatuses
        {
            get
            {
                if ((_shippingStatuses == null))
                {
                    _shippingStatuses = CreateObjectSet<ShippingStatus>();
                }
                return _shippingStatuses;
            }
        }
        private ObjectSet<ShippingStatus> _shippingStatuses;

        public ObjectSet<ShoppingCartItem> ShoppingCartItems
        {
            get
            {
                if ((_shoppingCartItems == null))
                {
                    _shoppingCartItems = CreateObjectSet<ShoppingCartItem>();
                }
                return _shoppingCartItems;
            }
        }
        private ObjectSet<ShoppingCartItem> _shoppingCartItems;

        public ObjectSet<ShoppingCartType> ShoppingCartTypes
        {
            get
            {
                if ((_shoppingCartTypes == null))
                {
                    _shoppingCartTypes = CreateObjectSet<ShoppingCartType>();
                }
                return _shoppingCartTypes;
            }
        }
        private ObjectSet<ShoppingCartType> _shoppingCartTypes;

        public ObjectSet<SpecificationAttributeLocalized> SpecificationAttributeLocalized
        {
            get
            {
                if ((_specificationAttributeLocalized == null))
                {
                    _specificationAttributeLocalized = CreateObjectSet<SpecificationAttributeLocalized>();
                }
                return _specificationAttributeLocalized;
            }
        }
        private ObjectSet<SpecificationAttributeLocalized> _specificationAttributeLocalized;

        public ObjectSet<SpecificationAttributeOptionLocalized> SpecificationAttributeOptionLocalized
        {
            get
            {
                if ((_specificationAttributeOptionLocalized == null))
                {
                    _specificationAttributeOptionLocalized = CreateObjectSet<SpecificationAttributeOptionLocalized>();
                }
                return _specificationAttributeOptionLocalized;
            }
        }
        private ObjectSet<SpecificationAttributeOptionLocalized> _specificationAttributeOptionLocalized;

        public ObjectSet<SpecificationAttributeOption> SpecificationAttributeOptions
        {
            get
            {
                if ((_specificationAttributeOptions == null))
                {
                    _specificationAttributeOptions = CreateObjectSet<SpecificationAttributeOption>();
                }
                return _specificationAttributeOptions;
            }
        }
        private ObjectSet<SpecificationAttributeOption> _specificationAttributeOptions;

        public ObjectSet<SpecificationAttribute> SpecificationAttributes
        {
            get
            {
                if ((_specificationAttributes == null))
                {
                    _specificationAttributes = CreateObjectSet<SpecificationAttribute>();
                }
                return _specificationAttributes;
            }
        }
        private ObjectSet<SpecificationAttribute> _specificationAttributes;

        public ObjectSet<StateProvince> StateProvinces
        {
            get
            {
                if ((_stateProvinces == null))
                {
                    _stateProvinces = CreateObjectSet<StateProvince>();
                }
                return _stateProvinces;
            }
        }
        private ObjectSet<StateProvince> _stateProvinces;

        public ObjectSet<TaxCategory> TaxCategories
        {
            get
            {
                if ((_taxCategories == null))
                {
                    _taxCategories = CreateObjectSet<TaxCategory>();
                }
                return _taxCategories;
            }
        }
        private ObjectSet<TaxCategory> _taxCategories;

        public ObjectSet<TaxProvider> TaxProviders
        {
            get
            {
                if ((_taxProviders == null))
                {
                    _taxProviders = CreateObjectSet<TaxProvider>();
                }
                return _taxProviders;
            }
        }
        private ObjectSet<TaxProvider> _taxProviders;

        public ObjectSet<TaxRate> TaxRates
        {
            get
            {
                if ((_taxRates == null))
                {
                    _taxRates = CreateObjectSet<TaxRate>();
                }
                return _taxRates;
            }
        }
        private ObjectSet<TaxRate> _taxRates;

        public ObjectSet<TierPrice> TierPrices
        {
            get
            {
                if ((_tierPrices == null))
                {
                    _tierPrices = CreateObjectSet<TierPrice>();
                }
                return _tierPrices;
            }
        }
        private ObjectSet<TierPrice> _tierPrices;

        public ObjectSet<Topic> Topics
        {
            get
            {
                if ((_topics == null))
                {
                    _topics = CreateObjectSet<Topic>();
                }
                return _topics;
            }
        }
        private ObjectSet<Topic> _topics;

        public ObjectSet<Warehouse> Warehouses
        {
            get
            {
                if ((_warehouses == null))
                {
                    _warehouses = CreateObjectSet<Warehouse>();
                }
                return _warehouses;
            }
        }
        private ObjectSet<Warehouse> _warehouses;

        #endregion

        #region Function Imports (Stored Procedures)
        
        public void Sp_ActivityLogClearAll()
        {
            base.ExecuteFunction("Sp_ActivityLogClearAll");
        }

        public List<ActivityLog> Sp_ActivityLogLoadAll(DateTime? createdOnFrom,
            DateTime? createdOnTo, string email, string username, int activityLogTypeID,
            int pageSize, int pageIndex, out int totalRecords)
        {
            totalRecords = 0;

            ObjectParameter createdOnFromParameter;
            if (createdOnFrom.HasValue)
            {
                createdOnFromParameter = new ObjectParameter("CreatedOnFrom", createdOnFrom);
            }
            else
            {
                createdOnFromParameter = new ObjectParameter("CreatedOnFrom", typeof(DateTime));
            }

            ObjectParameter createdOnToParameter;
            if (createdOnTo.HasValue)
            {
                createdOnToParameter = new ObjectParameter("CreatedOnTo", createdOnTo);
            }
            else
            {
                createdOnToParameter = new ObjectParameter("CreatedOnTo", typeof(DateTime));
            }

            ObjectParameter emailParameter = new ObjectParameter("Email", email);
            ObjectParameter usernameParameter = new ObjectParameter("Username", username);
            ObjectParameter activityLogTypeIDParameter = new ObjectParameter("ActivityLogTypeID", activityLogTypeID);
            ObjectParameter pageSizeParameter = new ObjectParameter("PageSize", pageSize);
            ObjectParameter pageIndexParameter = new ObjectParameter("PageIndex", pageIndex);
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));

            var result = base.ExecuteFunction<ActivityLog>("Sp_ActivityLogLoadAll", 
                createdOnFromParameter, createdOnToParameter, emailParameter, 
                usernameParameter, activityLogTypeIDParameter, pageSizeParameter, 
                pageIndexParameter, totalRecordsParameter).ToList();
            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);
            return result;
        }

        public List<BlogPost> Sp_BlogPostLoadAll(int languageId,
            int pageSize, int pageIndex, out int totalRecords)
        {
            totalRecords = 0;

            ObjectParameter languageIdParameter = new ObjectParameter("LanguageID", languageId);
            ObjectParameter pageSizeParameter = new ObjectParameter("PageSize", pageSize);
            ObjectParameter pageIndexParameter = new ObjectParameter("PageIndex", pageIndex);
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));

            var result = base.ExecuteFunction<BlogPost>("Sp_BlogPostLoadAll", 
                languageIdParameter, pageSizeParameter, 
                pageIndexParameter, totalRecordsParameter).ToList();
            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);
            return result;
        }

        public List<CustomerBestReportLine> Sp_CustomerBestReport(DateTime? startTime,
            DateTime? endTime, int? orderStatusId, int? paymentStatusId,
            int? shippingStatusId, int orderBy)
        {
            ObjectParameter startTimeParameter;
            if (startTime.HasValue)
            {
                startTimeParameter = new ObjectParameter("StartTime", startTime);
            }
            else
            {
                startTimeParameter = new ObjectParameter("StartTime", typeof(DateTime));
            }

            ObjectParameter endTimeParameter;
            if (endTime.HasValue)
            {
                endTimeParameter = new ObjectParameter("EndTime", endTime);
            }
            else
            {
                endTimeParameter = new ObjectParameter("EndTime", typeof(DateTime));
            }

            ObjectParameter orderStatusIDParameter;
            if (orderStatusId.HasValue)
            {
                orderStatusIDParameter = new ObjectParameter("OrderStatusID", orderStatusId);
            }
            else
            {
                orderStatusIDParameter = new ObjectParameter("OrderStatusID", typeof(int));
            }

            ObjectParameter paymentStatusIDParameter;
            if (paymentStatusId.HasValue)
            {
                paymentStatusIDParameter = new ObjectParameter("PaymentStatusID", paymentStatusId);
            }
            else
            {
                paymentStatusIDParameter = new ObjectParameter("PaymentStatusID", typeof(int));
            }

            ObjectParameter shippingStatusIDParameter;
            if (shippingStatusId.HasValue)
            {
                shippingStatusIDParameter = new ObjectParameter("ShippingStatusID", shippingStatusId);
            }
            else
            {
                shippingStatusIDParameter = new ObjectParameter("ShippingStatusID", typeof(int));
            }

            ObjectParameter orderByParameter = new ObjectParameter("OrderBy", orderBy);

            var result = base.ExecuteFunction<CustomerBestReportLine>("Sp_CustomerBestReport",
                startTimeParameter, endTimeParameter, orderStatusIDParameter,
                paymentStatusIDParameter, shippingStatusIDParameter, orderByParameter).ToList();
            return result;
        }
        
        public List<Customer> Sp_CustomerLoadAll(DateTime? registrationFrom,
            DateTime? registrationTo, string email, string username,
            bool dontLoadGuestCustomers, int pageSize, int pageIndex, out int totalRecords)
        {
            totalRecords = 0;

            ObjectParameter startTimeParameter;
            if (registrationFrom.HasValue)
            {
                startTimeParameter = new ObjectParameter("StartTime", registrationFrom);
            }
            else
            {
                startTimeParameter = new ObjectParameter("StartTime", typeof(DateTime));
            }

            ObjectParameter endTimeParameter;
            if (registrationTo.HasValue)
            {
                endTimeParameter = new ObjectParameter("EndTime", registrationTo);
            }
            else
            {
                endTimeParameter = new ObjectParameter("EndTime", typeof(DateTime));
            }

            ObjectParameter emailParameter = new ObjectParameter("Email", email);
            ObjectParameter usernameParameter = new ObjectParameter("Username", username);
            ObjectParameter dontLoadGuestCustomersParameter = new ObjectParameter("DontLoadGuestCustomers", dontLoadGuestCustomers);
            ObjectParameter pageSizeParameter = new ObjectParameter("PageSize", pageSize);
            ObjectParameter pageIndexParameter = new ObjectParameter("PageIndex", pageIndex);
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));

            var result = base.ExecuteFunction<Customer>("Sp_CustomerLoadAll", startTimeParameter,
                endTimeParameter, emailParameter, usernameParameter, dontLoadGuestCustomersParameter, 
                pageSizeParameter, pageIndexParameter, totalRecordsParameter).ToList();
            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);
            return result;
        }

        public List<CustomerReportByAttributeKeyLine> Sp_CustomerReportByAttributeKey(string customerAttributeKey)
        {
            ObjectParameter customerAttributeKeyParameter = new ObjectParameter("CustomerAttributeKey", customerAttributeKey);

            var result = base.ExecuteFunction<CustomerReportByAttributeKeyLine>("Sp_CustomerReportByAttributeKey",
                customerAttributeKeyParameter).ToList();
            return result;
        }

        public List<CustomerReportByLanguageLine> Sp_CustomerReportByLanguage()
        {
            var result = base.ExecuteFunction<CustomerReportByLanguageLine>("Sp_CustomerReportByLanguage").ToList();
            return result;
        }

        public void Sp_CustomerSessionDeleteExpired(DateTime olderThen)
        {
            ObjectParameter olderThanParameter = new ObjectParameter("OlderThan", olderThen);

            base.ExecuteFunction("Sp_CustomerSessionDeleteExpired", olderThanParameter);
        }

        public List<DiscountUsageHistory> Sp_DiscountUsageHistoryLoadAll(int? discountId,
            int? customerId, int? orderId)
        {
            ObjectParameter discountIdParameter;
            if (discountId.HasValue)
            {
                discountIdParameter = new ObjectParameter("DiscountID", discountId);
            }
            else
            {
                discountIdParameter = new ObjectParameter("DiscountID", typeof(int));
            }

            ObjectParameter orderIdParameter;
            if (orderId.HasValue)
            {
                orderIdParameter = new ObjectParameter("OrderID", orderId);
            }
            else
            {
                orderIdParameter = new ObjectParameter("OrderID", typeof(int));
            }

            ObjectParameter customerIdParameter;
            if (customerId.HasValue)
            {
                customerIdParameter = new ObjectParameter("CustomerID", customerId);
            }
            else
            {
                customerIdParameter = new ObjectParameter("CustomerID", typeof(int));
            }

            var result = base.ExecuteFunction<DiscountUsageHistory>("Sp_DiscountUsageHistoryLoadAll",
                discountIdParameter, orderIdParameter, customerIdParameter).ToList();
            return result;
        }
        
        public void Sp_Forums_ForumDelete(int forumId)
        {
            ObjectParameter forumIdParameter = new ObjectParameter("forumId", forumId);

            base.ExecuteFunction("Sp_Forums_ForumDelete", forumIdParameter);
        }

        public void Sp_Forums_ForumUpdateCounts(int forumId)
        {
            ObjectParameter forumIdParameter = new ObjectParameter("forumId", forumId);

            base.ExecuteFunction("Sp_Forums_ForumUpdateCounts", forumIdParameter);
        }
       
        public List<ForumPost> Sp_Forums_PostLoadAll(int forumTopicId, int userId,
            string keywords, bool ascSort, int pageSize, int pageIndex, out int totalRecords)
        {
            totalRecords = 0;

            ObjectParameter forumTopicIdParameter = new ObjectParameter("TopicID", forumTopicId);
            ObjectParameter userIdParameter = new ObjectParameter("userId", userId);
            ObjectParameter keywordsParameter = new ObjectParameter("keywords", keywords);
            ObjectParameter ascSortParameter = new ObjectParameter("ascSort", ascSort);
            ObjectParameter pageSizeParameter = new ObjectParameter("PageSize", pageSize);
            ObjectParameter pageIndexParameter = new ObjectParameter("PageIndex", pageIndex);
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));

            var result = base.ExecuteFunction<ForumPost>("Sp_Forums_PostLoadAll",
                forumTopicIdParameter, userIdParameter, keywordsParameter,
                ascSortParameter, pageSizeParameter, 
                pageIndexParameter, totalRecordsParameter).ToList();
            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);
            return result;
        }     
        
        public List<PrivateMessage> Sp_Forums_PrivateMessageLoadAll(int fromUserId,
            int toUserId, bool? isRead, bool? isDeletedByAuthor, bool? isDeletedByRecipient,
            string keywords, int pageSize, int pageIndex, out int totalRecords)
        {
            totalRecords = 0;

            ObjectParameter fromUserIdParameter = new ObjectParameter("fromUserId", fromUserId);
            ObjectParameter toUserIdParameter = new ObjectParameter("toUserId", toUserId);

            ObjectParameter isReadParameter;
            if (isRead.HasValue)
            {
                isReadParameter = new ObjectParameter("isRead", isRead);
            }
            else
            {
                isReadParameter = new ObjectParameter("isRead", typeof(bool));
            }

            ObjectParameter isDeletedByAuthorParameter;
            if (isDeletedByAuthor.HasValue)
            {
                isDeletedByAuthorParameter = new ObjectParameter("isDeletedByAuthor", isDeletedByAuthor);
            }
            else
            {
                isDeletedByAuthorParameter = new ObjectParameter("isDeletedByAuthor", typeof(decimal));
            }

            ObjectParameter isDeletedByRecipientParameter;
            if (isDeletedByRecipient.HasValue)
            {
                isDeletedByRecipientParameter = new ObjectParameter("IsDeletedByRecipient", isDeletedByRecipient);
            }
            else
            {
                isDeletedByRecipientParameter = new ObjectParameter("IsDeletedByRecipient", typeof(bool));
            }

            ObjectParameter keywordsParameter = new ObjectParameter("Keywords", keywords);
            ObjectParameter pageSizeParameter = new ObjectParameter("PageSize", pageSize);
            ObjectParameter pageIndexParameter = new ObjectParameter("PageIndex", pageIndex);
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));

            var result = base.ExecuteFunction<PrivateMessage>("Sp_Forums_PrivateMessageLoadAll",
                fromUserIdParameter, toUserIdParameter, isReadParameter,
                isDeletedByAuthorParameter, isDeletedByRecipientParameter,
                keywordsParameter, pageSizeParameter, pageIndexParameter,
                totalRecordsParameter).ToList();
            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);
            return result;
        }

        public List<ForumSubscription> Sp_Forums_SubscriptionLoadAll(int userId, int forumId,
            int topicId, int pageSize, int pageIndex, out int totalRecords)
        {
            totalRecords = 0;

            ObjectParameter userIdParameter = new ObjectParameter("userId", userId);
            ObjectParameter forumIdParameter = new ObjectParameter("forumId", forumId);
            ObjectParameter topicIdParameter = new ObjectParameter("topicId", topicId);
            ObjectParameter pageSizeParameter = new ObjectParameter("PageSize", pageSize);
            ObjectParameter pageIndexParameter = new ObjectParameter("PageIndex", pageIndex);
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));

            var result = base.ExecuteFunction<ForumSubscription>("Sp_Forums_SubscriptionLoadAll",
                userIdParameter, forumIdParameter, topicIdParameter,
                pageSizeParameter, pageIndexParameter, totalRecordsParameter).ToList();
            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);
            return result;
        }

        public List<ForumTopic> Sp_Forums_TopicLoadActive(int forumId, int topicCount)
        {
            ObjectParameter forumIdParameter = new ObjectParameter("forumId", forumId);
            ObjectParameter topicCountParameter = new ObjectParameter("topicCount", topicCount);

            var result = base.ExecuteFunction<ForumTopic>("Sp_Forums_TopicLoadActive",
                forumIdParameter, topicCountParameter).ToList();
            return result;
        }     

        public List<ForumTopic> Sp_Forums_TopicLoadAll(int forumId,
            int userId, string keywords, bool searchPosts, int pageSize,
            int pageIndex, out int totalRecords)
        {
            totalRecords = 0;

            ObjectParameter forumIdParameter = new ObjectParameter("forumId", forumId);
            ObjectParameter userIdParameter = new ObjectParameter("userId", userId);
            ObjectParameter keywordsParameter = new ObjectParameter("keywords", keywords);
            ObjectParameter searchPostsParameter = new ObjectParameter("searchPosts", searchPosts);
            ObjectParameter pageSizeParameter = new ObjectParameter("PageSize", pageSize);
            ObjectParameter pageIndexParameter = new ObjectParameter("PageIndex", pageIndex);
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));

            var result = base.ExecuteFunction<ForumTopic>("Sp_Forums_TopicLoadAll",
                forumIdParameter, userIdParameter, keywordsParameter,
                searchPostsParameter, pageSizeParameter, 
                pageIndexParameter, totalRecordsParameter).ToList();
            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);
            return result;
        }     
                
        public List<GiftCard> Sp_GiftCardLoadAll(int? orderId,
            int? customerId, DateTime? startTime, DateTime? endTime,
            int? orderStatusId, int? paymentStatusId, int? shippingStatusId,
            bool? isGiftCardActivated, string giftCardCouponCode)
        {
            ObjectParameter orderIdParameter;
            if (orderId.HasValue)
            {
                orderIdParameter = new ObjectParameter("OrderID", orderId);
            }
            else
            {
                orderIdParameter = new ObjectParameter("OrderID", typeof(int));
            }

            ObjectParameter customerIdParameter;
            if (customerId.HasValue)
            {
                customerIdParameter = new ObjectParameter("CustomerID", customerId);
            }
            else
            {
                customerIdParameter = new ObjectParameter("CustomerID", typeof(int));
            }

            ObjectParameter startTimeParameter;
            if (startTime.HasValue)
            {
                startTimeParameter = new ObjectParameter("StartTime", startTime);
            }
            else
            {
                startTimeParameter = new ObjectParameter("StartTime", typeof(DateTime));
            }

            ObjectParameter endTimeParameter;
            if (endTime.HasValue)
            {
                endTimeParameter = new ObjectParameter("EndTime", endTime);
            }
            else
            {
                endTimeParameter = new ObjectParameter("EndTime", typeof(DateTime));
            }

            ObjectParameter orderStatusIDParameter;
            if (orderStatusId.HasValue)
            {
                orderStatusIDParameter = new ObjectParameter("OrderStatusID", orderStatusId);
            }
            else
            {
                orderStatusIDParameter = new ObjectParameter("OrderStatusID", typeof(int));
            }

            ObjectParameter paymentStatusIDParameter;
            if (paymentStatusId.HasValue)
            {
                paymentStatusIDParameter = new ObjectParameter("PaymentStatusID", paymentStatusId);
            }
            else
            {
                paymentStatusIDParameter = new ObjectParameter("PaymentStatusID", typeof(int));
            }

            ObjectParameter shippingStatusIDParameter;
            if (shippingStatusId.HasValue)
            {
                shippingStatusIDParameter = new ObjectParameter("ShippingStatusID", shippingStatusId);
            }
            else
            {
                shippingStatusIDParameter = new ObjectParameter("ShippingStatusID", typeof(int));
            }

            ObjectParameter isGiftCardActivatedParameter;
            if (isGiftCardActivated.HasValue)
            {
                isGiftCardActivatedParameter = new ObjectParameter("IsGiftCardActivated", isGiftCardActivated);
            }
            else
            {
                isGiftCardActivatedParameter = new ObjectParameter("IsGiftCardActivated", typeof(int));
            }

            ObjectParameter giftCardCouponCodeParameter = new ObjectParameter("GiftCardCouponCode", giftCardCouponCode);

            var result = base.ExecuteFunction<GiftCard>("Sp_GiftCardLoadAll",
                orderIdParameter, customerIdParameter,
                startTimeParameter, endTimeParameter, orderStatusIDParameter,
                paymentStatusIDParameter, shippingStatusIDParameter,
                isGiftCardActivatedParameter, giftCardCouponCodeParameter).ToList();
            return result;
        }

        public List<GiftCardUsageHistory> Sp_GiftCardUsageHistoryLoadAll(int? giftCardId,
            int? customerId, int? orderId)
        {
            ObjectParameter giftCardIdParameter;
            if (giftCardId.HasValue)
            {
                giftCardIdParameter = new ObjectParameter("GiftCardID", giftCardId);
            }
            else
            {
                giftCardIdParameter = new ObjectParameter("GiftCardID", typeof(int));
            }

            ObjectParameter orderIdParameter;
            if (orderId.HasValue)
            {
                orderIdParameter = new ObjectParameter("OrderID", orderId);
            }
            else
            {
                orderIdParameter = new ObjectParameter("OrderID", typeof(int));
            }

            ObjectParameter customerIdParameter;
            if (customerId.HasValue)
            {
                customerIdParameter = new ObjectParameter("CustomerID", customerId);
            }
            else
            {
                customerIdParameter = new ObjectParameter("CustomerID", typeof(int));
            }

            var result = base.ExecuteFunction<GiftCardUsageHistory>("Sp_GiftCardUsageHistoryLoadAll",
                giftCardIdParameter, orderIdParameter, customerIdParameter).ToList();
            return result;
        }

        public void Sp_LanguagePackExport(int languageID, out string xmlPackage)
        {
            ObjectParameter languageIDParameter = new ObjectParameter("LanguageID", languageID);
            ObjectParameter xmlPackageParameter = new ObjectParameter("XmlPackage", typeof(string));
            base.ExecuteFunction("Sp_LanguagePackExport", languageIDParameter, xmlPackageParameter);
            xmlPackage = Convert.ToString(xmlPackageParameter.Value);
        }

        public void Sp_LanguagePackImport(int languageID, string xmlPackage)
        {
            ObjectParameter languageIDParameter = new ObjectParameter("LanguageID", languageID);
            ObjectParameter xmlPackageParameter = new ObjectParameter("XmlPackage", xmlPackage);

            base.ExecuteFunction("Sp_LanguagePackImport", languageIDParameter, xmlPackageParameter);
        }

        public void Sp_LogClear()
        {
            base.ExecuteFunction("Sp_LogClear");
        }

        public void Sp_Maintenance_ReindexTables()
        {
            base.ExecuteFunction("Sp_Maintenance_ReindexTables");
        }

        public List<NewsLetterSubscription> Sp_NewsLetterSubscriptionLoadAll(string email, bool showHidden)
        {
            ObjectParameter emailParameter = new ObjectParameter("Email", email);
            ObjectParameter showHiddenParameter = new ObjectParameter("ShowHidden", showHidden);

            var result = base.ExecuteFunction<NewsLetterSubscription>("Sp_NewsLetterSubscriptionLoadAll",
                emailParameter, showHiddenParameter).ToList();
            return result;
        }

        public List<News> Sp_NewsLoadAll(int languageId, bool showHidden,
            int pageSize, int pageIndex, out int totalRecords)
        {
            totalRecords = 0;

            ObjectParameter languageIdParameter = new ObjectParameter("LanguageID", languageId);
            ObjectParameter showHiddenParameter = new ObjectParameter("ShowHidden", showHidden);
            ObjectParameter pageSizeParameter = new ObjectParameter("PageSize", pageSize);
            ObjectParameter pageIndexParameter = new ObjectParameter("PageIndex", pageIndex);
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));

            var result = base.ExecuteFunction<News>("Sp_NewsLoadAll",
                languageIdParameter, showHiddenParameter, pageSizeParameter, 
                pageIndexParameter, totalRecordsParameter).ToList();
            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);
            return result;
        }

        public OrderAverageReportLine Sp_OrderAverageReport(DateTime? startTime,
            DateTime? endTime, int orderStatusId)
        {
            ObjectParameter startTimeParameter;
            if (startTime.HasValue)
            {
                startTimeParameter = new ObjectParameter("StartTime", startTime);
            }
            else
            {
                startTimeParameter = new ObjectParameter("StartTime", typeof(DateTime));
            }

            ObjectParameter endTimeParameter;
            if (endTime.HasValue)
            {
                endTimeParameter = new ObjectParameter("EndTime", endTime);
            }
            else
            {
                endTimeParameter = new ObjectParameter("EndTime", typeof(DateTime));
            }

            ObjectParameter orderStatusIDParameter = new ObjectParameter("OrderStatusID", orderStatusId);

            var result = base.ExecuteFunction<OrderAverageReportLine>("Sp_OrderAverageReport",
                startTimeParameter, endTimeParameter, orderStatusIDParameter).FirstOrDefault();
            return result;
        }

        public OrderIncompleteReportLine Sp_OrderIncompleteReport(int? orderStatusId, 
            int? paymentStatusId, int? shippingStatusId)
        {
            ObjectParameter orderStatusIDParameter;
            if (orderStatusId.HasValue)
            {
                orderStatusIDParameter = new ObjectParameter("OrderStatusID", orderStatusId);
            }
            else
            {
                orderStatusIDParameter = new ObjectParameter("OrderStatusID", typeof(int));
            }

            ObjectParameter paymentStatusIDParameter;
            if (paymentStatusId.HasValue)
            {
                paymentStatusIDParameter = new ObjectParameter("PaymentStatusID", paymentStatusId);
            }
            else
            {
                paymentStatusIDParameter = new ObjectParameter("PaymentStatusID", typeof(int));
            }

            ObjectParameter shippingStatusIDParameter;
            if (shippingStatusId.HasValue)
            {
                shippingStatusIDParameter = new ObjectParameter("ShippingStatusID", shippingStatusId);
            }
            else
            {
                shippingStatusIDParameter = new ObjectParameter("ShippingStatusID", typeof(int));
            }

            var result = base.ExecuteFunction<OrderIncompleteReportLine>("Sp_OrderIncompleteReport",
                orderStatusIDParameter, paymentStatusIDParameter, 
                shippingStatusIDParameter).FirstOrDefault();
            return result;
        }

        public List<OrderProductVariantReportLine> Sp_OrderProductVariantReport(DateTime? startTime,
            DateTime? endTime, int? orderStatusId,
            int? paymentStatusId, int? billingCountryID)
        {
            ObjectParameter startTimeParameter;
            if (startTime.HasValue)
            {
                startTimeParameter = new ObjectParameter("StartTime", startTime);
            }
            else
            {
                startTimeParameter = new ObjectParameter("StartTime", typeof(DateTime));
            }

            ObjectParameter endTimeParameter;
            if (endTime.HasValue)
            {
                endTimeParameter = new ObjectParameter("EndTime", endTime);
            }
            else
            {
                endTimeParameter = new ObjectParameter("EndTime", typeof(DateTime));
            }

            ObjectParameter orderStatusIDParameter;
            if (orderStatusId.HasValue)
            {
                orderStatusIDParameter = new ObjectParameter("OrderStatusID", orderStatusId);
            }
            else
            {
                orderStatusIDParameter = new ObjectParameter("OrderStatusID", typeof(int));
            }

            ObjectParameter paymentStatusIDParameter;
            if (paymentStatusId.HasValue)
            {
                paymentStatusIDParameter = new ObjectParameter("PaymentStatusID", paymentStatusId);
            }
            else
            {
                paymentStatusIDParameter = new ObjectParameter("PaymentStatusID", typeof(int));
            }

            ObjectParameter billingCountryIDParameter;
            if (billingCountryID.HasValue)
            {
                billingCountryIDParameter = new ObjectParameter("BillingCountryID", billingCountryID);
            }
            else
            {
                billingCountryIDParameter = new ObjectParameter("BillingCountryID", typeof(int));
            }

            var result = base.ExecuteFunction<OrderProductVariantReportLine>("Sp_OrderProductVariantReport",
                startTimeParameter, endTimeParameter, orderStatusIDParameter, paymentStatusIDParameter,
                billingCountryIDParameter).ToList();
            return result;
        }
                
        public List<PaymentMethod> Sp_PaymentMethodLoadAll(bool showHidden, int? filterByCountryId)
        {
            ObjectParameter showHiddenParameter = new ObjectParameter("ShowHidden", showHidden);
            
            ObjectParameter filterByCountryIdParameter;
            if (filterByCountryId.HasValue)
            {
                filterByCountryIdParameter = new ObjectParameter("FilterByCountryID", filterByCountryId);
            }
            else
            {
                filterByCountryIdParameter = new ObjectParameter("FilterByCountryID", typeof(int));
            }

            var result = base.ExecuteFunction<PaymentMethod>("Sp_PaymentMethodLoadAll",
                showHiddenParameter, filterByCountryIdParameter).ToList();
            return result;
        }

        public List<Picture> Sp_PictureLoadAllPaged(int pageSize, 
            int pageIndex, out int totalRecords)
        {
            totalRecords = 0;

            ObjectParameter pageSizeParameter = new ObjectParameter("PageSize", pageSize);
            ObjectParameter pageIndexParameter = new ObjectParameter("PageIndex", pageIndex);
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));

            var result = base.ExecuteFunction<Picture>("Sp_PictureLoadAllPaged", 
                pageSizeParameter, pageIndexParameter, totalRecordsParameter).ToList();
            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);
            return result;
        }
        
        public List<Product> Sp_ProductAlsoPurchasedLoadByProductID(int productId,
            bool showHidden, int pageSize, int pageIndex, out int totalRecords)
        {
            totalRecords = 0;

            ObjectParameter productIdParameter = new ObjectParameter("ProductID", productId);
            ObjectParameter showHiddenParameter = new ObjectParameter("ShowHidden", showHidden);
            ObjectParameter pageSizeParameter = new ObjectParameter("PageSize", pageSize);
            ObjectParameter pageIndexParameter = new ObjectParameter("PageIndex", pageIndex);
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));

            var result = base.ExecuteFunction<Product>("Sp_ProductAlsoPurchasedLoadByProductID", 
                productIdParameter, showHiddenParameter, pageSizeParameter, 
                pageIndexParameter, totalRecordsParameter).ToList();
            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);
            return result;
        }

        public List<Product> Sp_ProductLoadAllPaged(int categoryId,
            int manufacturerId, int productTagId,
            bool? featuredProducts, decimal? priceMin, decimal? priceMax,
            string keywords, bool searchDescriptions,
            int pageSize, int pageIndex, List<int> filteredSpecs,
            int languageId, int orderBy, bool showHidden, out int totalRecords)
        {
            totalRecords = 0;

            ObjectParameter categoryIdParameter = new ObjectParameter("CategoryID", categoryId);
            ObjectParameter manufacturerIdParameter = new ObjectParameter("ManufacturerID", manufacturerId);
            ObjectParameter productTagIdParameter = new ObjectParameter("ProductTagID", productTagId);
            ObjectParameter featuredProductsParameter;
            if (featuredProducts.HasValue)
            {
                featuredProductsParameter = new ObjectParameter("FeaturedProducts", featuredProducts);
            }
            else
            {
                featuredProductsParameter = new ObjectParameter("FeaturedProducts", typeof(bool));
            }
            ObjectParameter priceMinParameter;
            if (priceMin.HasValue)
            {
                priceMinParameter = new ObjectParameter("PriceMin", priceMin);
            }
            else
            {
                priceMinParameter = new ObjectParameter("PriceMin", typeof(decimal));
            }
            ObjectParameter priceMaxParameter;
            if (priceMax.HasValue)
            {
                priceMaxParameter = new ObjectParameter("PriceMax", priceMax);
            }
            else
            {
                priceMaxParameter = new ObjectParameter("PriceMax", typeof(decimal));
            }
            ObjectParameter keywordsParameter = new ObjectParameter("Keywords", keywords);
            ObjectParameter searchDescriptionsParameter = new ObjectParameter("SearchDescriptions", searchDescriptions);
            ObjectParameter showHiddenParameter = new ObjectParameter("ShowHidden", showHidden);
            ObjectParameter pageSizeParameter = new ObjectParameter("PageSize", pageSize);
            ObjectParameter pageIndexParameter = new ObjectParameter("PageIndex", pageIndex);

            string commaSeparatedSpecIds = string.Empty;
            if (filteredSpecs != null)
            {
                filteredSpecs.Sort();
                for (int i = 0; i < filteredSpecs.Count; i++)
                {
                    commaSeparatedSpecIds += filteredSpecs[i].ToString();
                    if (i != filteredSpecs.Count - 1)
                    {
                        commaSeparatedSpecIds += ",";
                    }
                }
            }
            ObjectParameter filteredSpecsParameter = new ObjectParameter("FilteredSpecs", commaSeparatedSpecIds);
            ObjectParameter languageIdParameter = new ObjectParameter("LanguageID", languageId);
            ObjectParameter orderByParameter = new ObjectParameter("OrderBy", orderBy);
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));

            var result = base.ExecuteFunction<Product>("Sp_ProductLoadAllPaged",
                categoryIdParameter, manufacturerIdParameter, productTagIdParameter,
                featuredProductsParameter, priceMinParameter, priceMaxParameter,
                keywordsParameter, searchDescriptionsParameter, showHiddenParameter,
                pageSizeParameter, pageIndexParameter, filteredSpecsParameter,
                languageIdParameter, orderByParameter, totalRecordsParameter).ToList();
            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);
            return result;
        }

        public void Sp_ProductRatingCreate(int productId,
            int customerId, int rating, DateTime ratedOn)
        {
            ObjectParameter productIdParameter = new ObjectParameter("productId", productId);
            ObjectParameter customerIdParameter = new ObjectParameter("customerId", customerId);
            ObjectParameter ratingParameter = new ObjectParameter("rating", rating);
            ObjectParameter ratedOnParameter= new ObjectParameter("RatedOn", ratedOn);

            base.ExecuteFunction("Sp_ProductRatingCreate",
                productIdParameter, customerIdParameter, ratingParameter,
                ratedOnParameter);
        }

        public void Sp_ProductTag_Product_MappingDelete(int productTagId, int productId)
        {
            ObjectParameter productTagIdParameter = new ObjectParameter("productTagId", productTagId);
            ObjectParameter productIdParameter = new ObjectParameter("productId", productId);

            base.ExecuteFunction("Sp_ProductTag_Product_MappingDelete",
                productTagIdParameter, productIdParameter);
        }

        public void Sp_ProductTag_Product_MappingInsert(int productTagId, int productId)
        {
            ObjectParameter productTagIdParameter = new ObjectParameter("productTagId", productTagId);
            ObjectParameter productIdParameter = new ObjectParameter("productId", productId);

            base.ExecuteFunction("Sp_ProductTag_Product_MappingInsert",
                productTagIdParameter, productIdParameter);
        }

        public List<ProductTag> Sp_ProductTagLoadAll(int productId, string name)
        {
            ObjectParameter productIdParameter = new ObjectParameter("ProductID", productId);
            ObjectParameter nameParameter = new ObjectParameter("Name", name);

            var result = base.ExecuteFunction<ProductTag>("Sp_ProductTagLoadAll",
                productIdParameter, nameParameter).ToList();
            return result;
        }

        public List<ProductVariant> Sp_ProductVariantLoadAll(int categoryId,
            int manufacturerId, string keywords, bool showHidden,
            int pageSize, int pageIndex, out int totalRecords)
        {
            totalRecords = 0;

            ObjectParameter categoryIdParameter = new ObjectParameter("CategoryID", categoryId);
            ObjectParameter manufacturerIdParameter = new ObjectParameter("ManufacturerID", manufacturerId);
            ObjectParameter keywordsParameter = new ObjectParameter("Keywords", keywords);
            ObjectParameter showHiddenParameter = new ObjectParameter("ShowHidden", showHidden);
            ObjectParameter pageSizeParameter = new ObjectParameter("PageSize", pageSize);
            ObjectParameter pageIndexParameter = new ObjectParameter("PageIndex", pageIndex);
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));

            var result = base.ExecuteFunction<ProductVariant>("Sp_ProductVariantLoadAll",
                categoryIdParameter, manufacturerIdParameter,
                keywordsParameter, showHiddenParameter,
                pageSizeParameter, pageIndexParameter, totalRecordsParameter).ToList();
            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);
            return result;
        }
        
        public List<RecurringPaymentHistory> Sp_RecurringPaymentHistoryLoadAll(int recurringPaymentId, 
            int orderId)
        {
            ObjectParameter recurringPaymentIdParameter = new ObjectParameter("recurringPaymentId", recurringPaymentId);
            ObjectParameter orderIdParameter = new ObjectParameter("orderId", orderId);

           var result =  base.ExecuteFunction<RecurringPaymentHistory>("Sp_RecurringPaymentHistoryLoadAll",
                recurringPaymentIdParameter, orderIdParameter).ToList();
            return result;
        }
        
        public List<RecurringPayment> Sp_RecurringPaymentLoadAll(bool showHidden,
            int customerId, int initialOrderId, int? initialOrderStatusId)
        {
            ObjectParameter showHiddenParameter = new ObjectParameter("ShowHidden", showHidden);
            ObjectParameter customerIdParameter = new ObjectParameter("customerId", customerId);
            ObjectParameter initialOrderIdParameter = new ObjectParameter("initialOrderId", initialOrderId);

           ObjectParameter initialOrderStatusIdParameter;
            if (initialOrderStatusId.HasValue)
            {
                initialOrderStatusIdParameter = new ObjectParameter("initialOrderStatusId", initialOrderStatusId);
            }
            else
            {
                initialOrderStatusIdParameter = new ObjectParameter("initialOrderStatusId", typeof(int));
            }

            var result = base.ExecuteFunction<RecurringPayment>("Sp_RecurringPaymentLoadAll", 
                showHiddenParameter, customerIdParameter,
                initialOrderIdParameter, initialOrderStatusIdParameter).ToList();
            return result;
        }
                
        public List<RewardPointsHistory> Sp_RewardPointsHistoryLoadAll(int? customerId,
            int? orderId, int pageSize, int pageIndex, out int totalRecords)
        {
            totalRecords = 0;
            
            ObjectParameter customerIdParameter;
            if (customerId.HasValue)
            {
                customerIdParameter = new ObjectParameter("customerId", customerId);
            }
            else
            {
                customerIdParameter = new ObjectParameter("customerId", typeof(int));
            }

            ObjectParameter orderIdParameter;
            if (orderId.HasValue)
            {
                orderIdParameter = new ObjectParameter("orderId", orderId);
            }
            else
            {
               orderIdParameter = new ObjectParameter("orderId", typeof(int));
            }

            ObjectParameter pageSizeParameter = new ObjectParameter("PageSize", pageSize);
            ObjectParameter pageIndexParameter = new ObjectParameter("PageIndex", pageIndex);
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));


            var result = base.ExecuteFunction<RewardPointsHistory>("Sp_RewardPointsHistoryLoadAll", 
                customerIdParameter, orderIdParameter, pageSizeParameter, 
                pageIndexParameter, totalRecordsParameter).ToList();

            totalRecords = Convert.ToInt32(totalRecordsParameter.Value);
            return result;
        }

        public List<BestSellersReportLine> Sp_SalesBestSellersReport(int lastDays,
            int recordsToReturn, int orderBy)
        {
            ObjectParameter lastDaysParameter = new ObjectParameter("lastDays", lastDays);
            ObjectParameter recordsToReturnParameter = new ObjectParameter("recordsToReturn", recordsToReturn);
            ObjectParameter orderByParameter = new ObjectParameter("orderBy", orderBy);
            
            var result = base.ExecuteFunction<BestSellersReportLine>("Sp_SalesBestSellersReport",
                lastDaysParameter, recordsToReturnParameter, orderByParameter).ToList();
            return result;
        }          
        
        public void Sp_SearchLogClear()
        {
            base.ExecuteFunction("Sp_SearchLogClear");
        }
        
        public List<SearchTermReportLine> Sp_SearchTermReport(DateTime? startTime, 
            DateTime? endTime, int count)
        {
            ObjectParameter startTimeParameter;
            if (startTime.HasValue)
            {
                startTimeParameter = new ObjectParameter("StartTime", startTime);
            }
            else
            {
                startTimeParameter = new ObjectParameter("StartTime", typeof(DateTime));
            }

            ObjectParameter endTimeParameter;
            if (endTime.HasValue)
            {
                endTimeParameter = new ObjectParameter("EndTime", endTime);
            }
            else
            {
                endTimeParameter = new ObjectParameter("EndTime", typeof(DateTime));
            }

            ObjectParameter countParameter = new ObjectParameter("count", count);

            var result = base.ExecuteFunction<SearchTermReportLine>("Sp_SearchTermReport",
                startTimeParameter, endTimeParameter, countParameter).ToList();
            return result;
        }
            
        public List<ShippingMethod> Sp_ShippingMethodLoadAll( int? filterByCountryId)
        {
            ObjectParameter filterByCountryIdParameter;
            if (filterByCountryId.HasValue)
            {
                filterByCountryIdParameter = new ObjectParameter("FilterByCountryID", filterByCountryId);
            }
            else
            {
                filterByCountryIdParameter = new ObjectParameter("FilterByCountryID", typeof(int));
            }

            var result = base.ExecuteFunction<ShippingMethod>("Sp_ShippingMethodLoadAll",
                filterByCountryIdParameter).ToList();
            return result;
        }

        public void Sp_ShoppingCartItemDeleteExpired(DateTime olderThan)
        {
            ObjectParameter olderThanParameter = new ObjectParameter("OlderThan", olderThan);
            base.ExecuteFunction("Sp_ShoppingCartItemDeleteExpired", olderThanParameter);
        }
        public List<SpecificationAttributeOptionFilter> Sp_SpecificationAttributeOptionFilter_LoadByFilter(int categoryId, int languageId)
        {
            ObjectParameter categoryIdParameter = new ObjectParameter("categoryId", categoryId);
            ObjectParameter languageIdParameter = new ObjectParameter("languageId", languageId);

            var result = base.ExecuteFunction<SpecificationAttributeOptionFilter>("Sp_SpecificationAttributeOptionFilter_LoadByFilter",
                categoryIdParameter, languageIdParameter).ToList();
            return result;
        }

        public List<TaxRate> Sp_TaxRateLoadAll()
        {
            var result = base.ExecuteFunction<TaxRate>("Sp_TaxRateLoadAll").ToList();
            return result;
        }
                 
        #endregion
    }
}
