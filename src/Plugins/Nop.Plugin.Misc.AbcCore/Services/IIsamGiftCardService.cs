using Nop.Core.Domain.Orders;
using System;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public interface IIsamGiftCardService
    {
        GiftCardInfo GetGiftCardInfo(string code);
        void UpdateGiftCardAmt(GiftCard giftCard, decimal amtUsed);
    }
}