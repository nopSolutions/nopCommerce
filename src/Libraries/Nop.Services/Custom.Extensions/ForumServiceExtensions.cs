using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Forums
{
    public partial interface IForumService
    {
        //Task<string> GetCustomerAttributeAsync(Customer customer, string attribute);
    }

    public partial class ForumService
    {

        public virtual async Task<int> GetPrivateMessageParentIdAsync(int parentMessageId)
        {
            var pm = await _forumPrivateMessageRepository.GetByIdAsync(parentMessageId);
            return 1;
        }

    }
}
