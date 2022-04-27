using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Vendors;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Messages
{
    [TestFixture]
    public class WorkflowMessageServiceTests : ServiceTest
    {
        private readonly IWorkflowMessageService _workflowMessageService;

        private readonly List<int> _notActiveTempletes = new();
        private readonly IMessageTemplateService _messageTemplateService;
        private Customer _customer;
        private readonly IRepository<QueuedEmail> _queuedEmailRepository;
        private Order _order;
        private Vendor _vendor;
        private Shipment _shipment;
        private IList<MessageTemplate> _allMessageTemplates;
        private OrderNote _orderNote;
        private RecurringPayment _recurringPayment;
        private NewsLetterSubscription _subscription;
        private Product _product;
        private OrderItem _orderItem;
        private ReturnRequest _returnRequest;
        private Forum _forum;
        private ForumTopic _forumTopic;
        private ForumPost _forumPost;
        private PrivateMessage _privateMessage;
        private ProductReview _productReview;
        private GiftCard _giftCard;
        private BlogComment _blogComment;
        private NewsComment _newsComment;
        private BackInStockSubscription _backInStockSubscription;
        private readonly IForumService _forumService;

        public WorkflowMessageServiceTests()
        {
            _workflowMessageService = GetService<IWorkflowMessageService>();
            _messageTemplateService = GetService<IMessageTemplateService>();
            _queuedEmailRepository = GetService<IRepository<QueuedEmail>>();
            _forumService = GetService<IForumService>();
        }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var customerService = GetService<ICustomerService>();
            var orderService = GetService<IOrderService>();
            var vendorService = GetService<IVendorService>();
            var shipmentService = GetService<IShipmentService>();
            var productService = GetService<IProductService>();
            var giftCardService = GetService<IGiftCardService>();
            var blogService = GetService<IBlogService>();
            var newsService = GetService<INewsService>();

            _order = await orderService.GetOrderByIdAsync(1);
            _orderItem = (await orderService.GetOrderItemsAsync(1)).First();
            _customer = await customerService.GetCustomerByEmailAsync(NopTestsDefaults.AdminEmail);
            _vendor = await vendorService.GetVendorByIdAsync(1);
            _shipment = await shipmentService.GetShipmentByIdAsync(1);
            _orderNote = await orderService.GetOrderNoteByIdAsync(1);
            _recurringPayment = new RecurringPayment {InitialOrderId = _order.Id, IsActive = true};
            _subscription = new NewsLetterSubscription {Active = true, Email = NopTestsDefaults.AdminEmail};
            _product = await productService.GetProductByIdAsync(1);
            _returnRequest = new ReturnRequest {CustomerId = _customer.Id, OrderItemId = _orderItem.Id};
            _forum = await _forumService.GetForumByIdAsync(1);
            _forumTopic = new ForumTopic {CustomerId = _customer.Id, ForumId = _forum.Id, Subject = "Subject"};
            await _forumService.InsertTopicAsync(_forumTopic, false);
            _forumPost = new ForumPost { CustomerId = _customer.Id, TopicId = _forumTopic.Id, Text = "Text"};
            await _forumService.InsertPostAsync(_forumPost, false);

            _privateMessage = new PrivateMessage
            {
                FromCustomerId = 1, ToCustomerId = 2, Subject = string.Empty, Text = string.Empty
            };
            _productReview = (await productService.GetAllProductReviewsAsync()).FirstOrDefault();
            _giftCard = await giftCardService.GetGiftCardByIdAsync(1);
            _blogComment = await blogService.GetBlogCommentByIdAsync(1);
            _newsComment = await newsService.GetNewsCommentByIdAsync(1);
            _backInStockSubscription = new BackInStockSubscription {ProductId = _product.Id, CustomerId = _customer.Id};

            _allMessageTemplates = await _messageTemplateService.GetAllMessageTemplatesAsync(0);

            foreach (var template in _allMessageTemplates.Where(t=>!t.IsActive))
            {
                template.IsActive = true;
                _notActiveTempletes.Add(template.Id);
                await _messageTemplateService.UpdateMessageTemplateAsync(template);
            }
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            foreach (var template in _allMessageTemplates.Where(t => _notActiveTempletes.Contains(t.Id)))
            {
                template.IsActive = false;
                await _messageTemplateService.UpdateMessageTemplateAsync(template);
            }

            await _forumService.DeletePostAsync(_forumPost);
            await _forumService.DeleteTopicAsync(_forumTopic);
        }

        [SetUp]
        public async Task SetUp()
        {
            await _queuedEmailRepository.TruncateAsync();
        }

        protected async Task CheckData(Func<Task<IList<int>>> func)
        {
            var queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
            queuedEmails.Count.Should().Be(0);

            var emailIds = await func();

            emailIds.Count.Should().BeGreaterThan(0);

            queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
            queuedEmails.Count.Should().Be(emailIds.Count);
        }

        #region Customer workflow

        [Test]
        public async Task CanSendCustomerRegisteredNotificationMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendCustomerRegisteredNotificationMessageAsync(_customer, 1));
        }

        [Test]
        public async Task CanSendCustomerWelcomeMessage()
        {
            await CheckData(async () => 
                await _workflowMessageService.SendCustomerWelcomeMessageAsync(_customer, 1));
        }

        [Test]
        public async Task CanSendCustomerEmailValidationMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendCustomerEmailValidationMessageAsync(_customer, 1));
        }

        [Test]
        public async Task CanSendCustomerEmailRevalidationMessage()
        {
            _customer.EmailToRevalidate = NopTestsDefaults.AdminEmail;
            await CheckData(async () =>
                await _workflowMessageService.SendCustomerEmailRevalidationMessageAsync(_customer, 1));
        }

        [Test]
        public async Task CanSendCustomerPasswordRecoveryMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendCustomerPasswordRecoveryMessageAsync(_customer, 1));
        }

        #endregion

        #region Order workflow

        [Test]
        public async Task CanSendOrderPlacedVendorNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderPlacedVendorNotificationAsync(_order, _vendor, 1));
        }

        [Test]
        public async Task CanSendOrderPlacedStoreOwnerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderPlacedStoreOwnerNotificationAsync(_order, 1));
        }

        [Test]
        public async Task CanSendOrderPlacedAffiliateNotification()
        {
            _order.AffiliateId = 1;
            await CheckData(async () =>
                await _workflowMessageService.SendOrderPlacedAffiliateNotificationAsync(_order, 1));
        }

        [Test]
        public async Task CanSendOrderPaidStoreOwnerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderPaidStoreOwnerNotificationAsync(_order, 1));
        }

        [Test]
        public async Task CanSendOrderPaidCustomerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderPaidCustomerNotificationAsync(_order, 1));
        }

        [Test]
        public async Task CanSendOrderPaidVendorNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderPaidVendorNotificationAsync(_order, _vendor, 1));
        }

        [Test]
        public async Task CanSendOrderPaidAffiliateNotification()
        {
            _order.AffiliateId = 1;
            await CheckData(async () =>
                await _workflowMessageService.SendOrderPaidAffiliateNotificationAsync(_order, 1));
        }

        [Test]
        public async Task CanSendOrderPlacedCustomerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderPlacedCustomerNotificationAsync(_order, 1));
        }

        [Test]
        public async Task CanSendShipmentSentCustomerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendShipmentSentCustomerNotificationAsync(_shipment, 1));
        }
        [Test]
        public async Task CanSendShipmentDeliveredCustomerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendShipmentDeliveredCustomerNotificationAsync(_shipment, 1));
        }

        [Test]
        public async Task CanSendOrderCompletedCustomerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderCompletedCustomerNotificationAsync(_order, 1));
        }

        [Test]
        public async Task CanSendOrderCancelledCustomerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderCancelledCustomerNotificationAsync(_order, 1));
        }

        [Test]
        public async Task CanSendOrderRefundedStoreOwnerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderRefundedStoreOwnerNotificationAsync(_order, 1M, 1));
        }

        [Test]
        public async Task CanSendOrderRefundedCustomerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderRefundedCustomerNotificationAsync(_order, 1M, 1));
        }

        [Test]
        public async Task CanSendNewOrderNoteAddedCustomerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewOrderNoteAddedCustomerNotificationAsync(_orderNote, 1));
        }

        [Test]
        public async Task CanSendRecurringPaymentCancelledStoreOwnerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendRecurringPaymentCancelledStoreOwnerNotificationAsync(_recurringPayment, 1));
        }

        [Test]
        public async Task CanSendRecurringPaymentCancelledCustomerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendRecurringPaymentCancelledCustomerNotificationAsync(_recurringPayment, 1));
        }

        [Test]
        public async Task CanSendRecurringPaymentFailedCustomerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendRecurringPaymentFailedCustomerNotificationAsync(_recurringPayment, 1));
        }

        #endregion

        #region Newsletter workflow

        [Test]
        public async Task CanSendNewsLetterSubscriptionActivationMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(_subscription, 1));
        }

        [Test]
        public async Task CanSendNewsLetterSubscriptionDeactivationMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessageAsync(_subscription, 1));
        }

        #endregion

        #region Send a message to a friend

        [Test]
        public async Task CanSendProductEmailAFriendMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendProductEmailAFriendMessageAsync(_customer, 1, _product, NopTestsDefaults.AdminEmail, NopTestsDefaults.AdminEmail, string.Empty));
        }

        [Test]
        public async Task CanSendWishlistEmailAFriendMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendWishlistEmailAFriendMessageAsync(_customer, 1, NopTestsDefaults.AdminEmail, NopTestsDefaults.AdminEmail, string.Empty));
        }

        #endregion

        #region Return requests

        [Test]
        public async Task CanSendNewReturnRequestStoreOwnerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewReturnRequestStoreOwnerNotificationAsync(_returnRequest, _orderItem, _order, 1));
        }

        [Test]
        public async Task CanSendNewReturnRequestCustomerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewReturnRequestCustomerNotificationAsync(_returnRequest, _orderItem, _order));
        }

        [Test]
        public async Task CanSendReturnRequestStatusChangedCustomerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendReturnRequestStatusChangedCustomerNotificationAsync(_returnRequest, _orderItem, _order));
        }

        #endregion

        #region Forum Notifications

        [Test]
        public async Task CanSendNewForumTopicMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewForumTopicMessageAsync(_customer, _forumTopic, _forum, 1));
        }

        [Test]
        public async Task CanSendNewForumPostMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewForumPostMessageAsync(_customer, _forumPost, _forumTopic, _forum, 1, 1));
        }

        [Test]
        public async Task CanSendPrivateMessageNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendPrivateMessageNotificationAsync(_privateMessage, 1));
        }

        #endregion

        #region Misc

        [Test]
        public async Task CanSendNewVendorAccountApplyStoreOwnerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewVendorAccountApplyStoreOwnerNotificationAsync(_customer, _vendor, 1));
        }

        [Test]
        public async Task CanSendVendorInformationChangeNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendVendorInformationChangeNotificationAsync(_vendor, 1));
        }

        [Test]
        public async Task CanSendProductReviewNotificationMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendProductReviewNotificationMessageAsync(_productReview, 1));
        }

        [Test]
        public async Task CanSendProductReviewReplyCustomerNotificationMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendProductReviewReplyCustomerNotificationMessageAsync(_productReview, 1));
        }

        [Test]
        public async Task CanSendGiftCardNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendGiftCardNotificationAsync(_giftCard, 1));
        }

        [Test]
        public async Task CanSendQuantityBelowStoreOwnerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendQuantityBelowStoreOwnerNotificationAsync(_product, 1));
        }
        
        [Test]
        public async Task CanSendNewVatSubmittedStoreOwnerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewVatSubmittedStoreOwnerNotificationAsync(_customer, "vat name", "vat address", 1));
        }

        [Test]
        public async Task CanSendBlogCommentNotificationMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendBlogCommentNotificationMessageAsync(_blogComment, 1));
        }

        [Test]
        public async Task CanSendNewsCommentNotificationMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewsCommentNotificationMessageAsync(_newsComment, 1));
        }

        [Test]
        public async Task CanSendBackInStockNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendBackInStockNotificationAsync(_backInStockSubscription, 1));
        }

        [Test]
        public async Task CanSendContactUsMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendContactUsMessageAsync(1, NopTestsDefaults.AdminEmail, "sender name", "subject", "body"));
        }

        [Test]
        public async Task CanSendContactVendorMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendContactVendorMessageAsync(_vendor, 1, NopTestsDefaults.AdminEmail, "sender name", "subject", "body"));
        }

        #endregion
    }
}
