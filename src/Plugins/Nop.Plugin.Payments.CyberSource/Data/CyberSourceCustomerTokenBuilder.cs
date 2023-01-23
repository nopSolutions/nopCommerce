using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Payments.CyberSource.Domain;

namespace Nop.Plugin.Payments.CyberSource.Data
{
    /// <summary>
    /// Represents a customer token entity builder
    /// </summary>
    public class CyberSourceCustomerTokenBuilder : NopEntityBuilder<CyberSourceCustomerToken>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CyberSourceCustomerToken.CustomerId)).AsInt32().NotNullable().ForeignKey<Customer>()
                .WithColumn(nameof(CyberSourceCustomerToken.SubscriptionId)).AsString(100).NotNullable()
                .WithColumn(nameof(CyberSourceCustomerToken.LastFourDigitOfCard)).AsString(4).NotNullable()
                .WithColumn(nameof(CyberSourceCustomerToken.FirstSixDigitOfCard)).AsString(6).Nullable()
                .WithColumn(nameof(CyberSourceCustomerToken.CardExpirationYear)).AsString(4).NotNullable()
                .WithColumn(nameof(CyberSourceCustomerToken.CardExpirationMonth)).AsString(2).NotNullable()
                .WithColumn(nameof(CyberSourceCustomerToken.ThreeDigitCardType)).AsString(3).Nullable()
                .WithColumn(nameof(CyberSourceCustomerToken.InstrumentIdentifier)).AsString(100).Nullable()
                .WithColumn(nameof(CyberSourceCustomerToken.InstrumentIdentifierStatus)).AsString(100).Nullable()
                .WithColumn(nameof(CyberSourceCustomerToken.CyberSourceCustomerId)).AsString(100).Nullable()
                .WithColumn(nameof(CyberSourceCustomerToken.TransactionId)).AsString(100).Nullable();
        }

        #endregion
    }
}