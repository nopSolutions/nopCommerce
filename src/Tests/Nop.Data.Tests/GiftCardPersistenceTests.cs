using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests
{
    [TestFixture]
    public class GiftCardPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_giftCard()
        {
            var giftCard = new GiftCard()
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

            var fromDb = SaveAndLoadEntity(giftCard);
            fromDb.ShouldNotBeNull();
            fromDb.GiftCardType.ShouldEqual(GiftCardType.Physical);
            fromDb.Amount.ShouldEqual(1.1M);
            fromDb.IsGiftCardActivated.ShouldEqual(true);
            fromDb.GiftCardCouponCode.ShouldEqual("Secret");
            fromDb.RecipientName.ShouldEqual("RecipientName 1");
            fromDb.RecipientEmail.ShouldEqual("a@b.c");
            fromDb.SenderName.ShouldEqual("SenderName 1");
            fromDb.SenderEmail.ShouldEqual("d@e.f");
            fromDb.Message.ShouldEqual("Message 1");
            fromDb.IsRecipientNotified.ShouldEqual(true);
            fromDb.CreatedOnUtc.ShouldEqual(new DateTime(2010, 01, 01));
        }
    }
}