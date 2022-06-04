using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;

namespace Nop.Services.Messages
{
    public partial interface IWorkflowMessageService
    {
        Task<IList<int>> SendCustomerAvilableNotificationToOtherCustomersAsync(Product product, Customer customer, int languageId);
    }

    public partial class WorkflowMessageService
    {
        public virtual async Task<IList<int>> SendCustomerAvilableNotificationToOtherCustomersAsync(Product product, Customer customer, int languageId)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var store = await _storeContext.GetCurrentStoreAsync();
            languageId = await EnsureLanguageIsActiveAsync(languageId, store.Id);

            var messageTemplates = await GetActiveMessageTemplatesAsync(MessageTemplateSystemNames.CustomerAvailableNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            var commonTokens = new List<Token>();
            await _messageTokenProvider.AddCustomProductTokensAsync(commonTokens, product, languageId);

            return await messageTemplates.SelectAwait(async messageTemplate =>
            {
                //email account
                var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                await _messageTokenProvider.AddStoreTokensAsync(tokens, store, emailAccount);

                //event notification
                await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

                var toEmail = customer.Email;
                var toName = await _customerService.GetCustomerFullNameAsync(customer);

                return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToListAsync();
        }
    }
}