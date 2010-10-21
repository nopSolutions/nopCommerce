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
using NopSolutions.NopCommerce.BusinessLogic.Messages.SMS;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Products.Specs;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Campaigns;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.QuickBooks;
using NopSolutions.NopCommerce.BusinessLogic.Security;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.Templates;
using NopSolutions.NopCommerce.BusinessLogic.Warehouses;

namespace NopSolutions.NopCommerce.BusinessLogic.Data
{
    /// <summary>
    /// Represents a nopCommerce object context
    /// </summary>
    public partial class NopObjectContext : ObjectContext
    {
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
            DateTime? dateFrom, DateTime? dateTo, int pageSize, int pageIndex, out int totalRecords)
        {
            totalRecords = 0;

            ObjectParameter languageIdParameter = new ObjectParameter("LanguageID", languageId);

            ObjectParameter dateFromParameter;
            if (dateFrom.HasValue)
            {
                dateFromParameter = new ObjectParameter("DateFrom", dateFrom);
            }
            else
            {
                dateFromParameter = new ObjectParameter("DateFrom", typeof(DateTime));
            }
            ObjectParameter dateToParameter;
            if (dateTo.HasValue)
            {
                dateToParameter = new ObjectParameter("DateTo", dateTo);
            }
            else
            {
                dateToParameter = new ObjectParameter("DateTo", typeof(DateTime));
            }

            ObjectParameter pageSizeParameter = new ObjectParameter("PageSize", pageSize);
            ObjectParameter pageIndexParameter = new ObjectParameter("PageIndex", pageIndex);
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));

            var result = base.ExecuteFunction<BlogPost>("Sp_BlogPostLoadAll", 
                languageIdParameter, dateFromParameter, dateToParameter,
                pageSizeParameter, pageIndexParameter, totalRecordsParameter).ToList();
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
            bool dontLoadGuestCustomers, int dateOfBirthMonth, int dateOfBirthDay,
            int pageSize, int pageIndex, out int totalRecords)
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
            ObjectParameter dateOfBirthMonthParameter = new ObjectParameter("DateOfBirthMonth", dateOfBirthMonth);
            ObjectParameter dateOfBirthDayParameter = new ObjectParameter("DateOfBirthDay", dateOfBirthDay);
            ObjectParameter pageSizeParameter = new ObjectParameter("PageSize", pageSize);
            ObjectParameter pageIndexParameter = new ObjectParameter("PageIndex", pageIndex);
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));

            var result = base.ExecuteFunction<Customer>("Sp_CustomerLoadAll", startTimeParameter,
                endTimeParameter, emailParameter, usernameParameter, dontLoadGuestCustomersParameter, 
                dateOfBirthMonthParameter, dateOfBirthDayParameter,
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

        public List<CustomerSession> Sp_CustomerSessionLoadNonEmpty()
        {
            return base.ExecuteFunction<CustomerSession>("Sp_CustomerSessionLoadNonEmpty").ToList();
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
                isDeletedByAuthorParameter = new ObjectParameter("isDeletedByAuthor", typeof(bool));
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
            int userId, string keywords, int searchType,
            DateTime? limitDate, int pageSize,
            int pageIndex, out int totalRecords)
        {
            totalRecords = 0;

            ObjectParameter forumIdParameter = new ObjectParameter("forumId", forumId);
            ObjectParameter userIdParameter = new ObjectParameter("userId", userId);
            ObjectParameter keywordsParameter = new ObjectParameter("keywords", keywords);
            ObjectParameter searchTypeParameter = new ObjectParameter("searchType", searchType);
            ObjectParameter limitDateParameter;
            if (limitDate.HasValue)
            {
                limitDateParameter = new ObjectParameter("LimitDate", limitDate);
            }
            else
            {
                limitDateParameter = new ObjectParameter("LimitDate", typeof(DateTime));
            }
            ObjectParameter pageSizeParameter = new ObjectParameter("PageSize", pageSize);
            ObjectParameter pageIndexParameter = new ObjectParameter("PageIndex", pageIndex);
            ObjectParameter totalRecordsParameter = new ObjectParameter("TotalRecords", typeof(int));

            var result = base.ExecuteFunction<ForumTopic>("Sp_Forums_TopicLoadAll",
                forumIdParameter, userIdParameter, keywordsParameter,
                searchTypeParameter, limitDateParameter, 
                pageSizeParameter, pageIndexParameter, totalRecordsParameter).ToList();
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
            int relatedToProductId, string keywords, bool searchDescriptions,
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
            ObjectParameter relatedToProductIdParameter = new ObjectParameter("RelatedToProductID", relatedToProductId);
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
                relatedToProductIdParameter, keywordsParameter, searchDescriptionsParameter, showHiddenParameter,
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
