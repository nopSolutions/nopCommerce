using FluentMigrator;
using Newtonsoft.Json;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Data.Migrations;

namespace Nop.Plugin.Payments.Manual.Migrations;

[NopMigration("2026-06-30 00:00:00", "Payments.Manual. Move \"credit card info\" fields into generic attributes")]
public class OrderMigration : MigrationBase
{
    #region Fields

    private readonly INopDataProvider _dataProvider;
    private readonly IRepository<Order> _orderRepository;

    #endregion

    #region Ctor

    public OrderMigration(INopDataProvider dataProvider, IRepository<Order> orderRepository)
    {
        _dataProvider = dataProvider;
        _orderRepository = orderRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#8169
        if (!Schema.Table(nameof(Order)).Column("AllowStoringCreditCardNumber").Exists())
            return;

        var page = 0;
        const int pageSize = 500;

#pragma warning disable CS0618 // Type or member is obsolete
        while (true)
        {
            var orders = _orderRepository.Table.Where(o =>
                    o.CardType != string.Empty || o.CardName != string.Empty || o.CardNumber != string.Empty ||
                    o.MaskedCreditCardNumber != string.Empty || o.CardCvv2 != string.Empty ||
                    o.CardExpirationMonth != string.Empty || o.CardExpirationYear != string.Empty)
                .Skip(pageSize * page++)
                .Take(pageSize);

            if (!orders.Any())
                break;

            var attributes = orders.Select(order => new GenericAttribute
            {
                EntityId = order.Id,
                KeyGroup = nameof(Order),
                Key = "CreditCardInfo",
                Value = JsonConvert.SerializeObject(new CreditCardInfo
                {
                    CardType = order.CardType,
                    CardName = order.CardName,
                    CardNumber = order.CardNumber,
                    MaskedCreditCardNumber = order.MaskedCreditCardNumber,
                    CardCvv2 = order.CardCvv2,
                    CardExpirationMonth = order.CardExpirationMonth,
                    CardExpirationYear = order.CardExpirationYear
                }),
                StoreId = order.StoreId,
                CreatedOrUpdatedDateUTC = DateTime.UtcNow
            });

            _dataProvider.BulkInsertEntities(attributes);
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        //nothing
    }

    #endregion
}