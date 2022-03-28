using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;
using Nop.Plugin.Misc.AbcFrontend.Models;
using Nop.Services.Payments;

namespace Nop.Plugin.Misc.AbcFrontend.Services
{
    public interface IErpCheckCardService
    {
        ErpCheckCardResponse CheckCreditCard(ProcessPaymentRequest processPaymentRequest);
    }
}