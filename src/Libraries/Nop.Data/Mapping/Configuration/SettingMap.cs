using LinqToDB.Mapping;
using Nop.Core.Domain.Configuration;

namespace Nop.Data.Mapping.Configuration
{
    /// <summary>
    /// Represents a setting mapping configuration
    /// </summary>
    public partial class SettingMap : NopEntityTypeConfiguration<Setting>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Setting> builder)
        {
            builder.HasTableName(nameof(Setting));

            builder.Property(setting => setting.Name).HasLength(200);
            builder.HasColumn(setting => setting.Name).IsColumnRequired();
            builder.HasColumn(setting => setting.Value).IsColumnRequired();
            builder.Property(setting => setting.StoreId);
        }

        #endregion
    }
}