using LinqToDB.Mapping;
using Nop.Data;
using Nop.Plugin.Tax.Avalara.Domain;

namespace Nop.Plugin.Tax.Avalara.Data
{
    /// <summary>
    /// Represents the tax transaction log mapping class
    /// </summary>
    public class TaxTransactionLogMap : NopEntityTypeConfiguration<TaxTransactionLog>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<TaxTransactionLog> builder)
        {
            builder.HasTableName(nameof(TaxTransactionLog));
            builder.Property(taxTransactionLog => taxTransactionLog.StatusCode);
            builder.Property(taxTransactionLog => taxTransactionLog.Url);
            builder.Property(taxTransactionLog => taxTransactionLog.RequestMessage);
            builder.Property(taxTransactionLog => taxTransactionLog.ResponseMessage);
            builder.Property(taxTransactionLog => taxTransactionLog.CustomerId);
            builder.Property(taxTransactionLog => taxTransactionLog.CreatedDateUtc);
        }

        #endregion
    }
}