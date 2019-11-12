using LinqToDB.Mapping;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Messages
{
    /// <summary>
    /// Represents a campaign mapping configuration
    /// </summary>
    public partial class CampaignMap : NopEntityTypeConfiguration<Campaign>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Campaign> builder)
        {
            builder.HasTableName(nameof(Campaign));

            builder.HasColumn(campaign => campaign.Name).IsColumnRequired();
            builder.HasColumn(campaign => campaign.Subject).IsColumnRequired();
            builder.HasColumn(campaign => campaign.Body).IsColumnRequired();

            builder.Property(campaign => campaign.StoreId);
            builder.Property(campaign => campaign.CustomerRoleId);
            builder.Property(campaign => campaign.CreatedOnUtc);
            builder.Property(campaign => campaign.DontSendBeforeDateUtc);
        }

        #endregion
    }
}