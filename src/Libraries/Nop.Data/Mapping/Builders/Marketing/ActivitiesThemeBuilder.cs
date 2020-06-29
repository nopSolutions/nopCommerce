using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using System.Data;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class ActivitiesThemeBuilder : NopEntityBuilder<ActivitiesTheme>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ActivitiesTheme.Title)).AsString(64).NotNullable()
                .WithColumn(nameof(ActivitiesTheme.CustomerRoleId)).AsInt32().Nullable().ForeignKey<CustomerRole>().OnDelete(Rule.SetNull)
                .WithColumn(nameof(ActivitiesTheme.ImageUrl)).AsString(1024).Nullable()
                .WithColumn(nameof(ActivitiesTheme.BannerImageUrl)).AsString(1024).Nullable()
                .WithColumn(nameof(ActivitiesTheme.ShareImageUrl)).AsString(1024).Nullable()
                .WithColumn(nameof(ActivitiesTheme.Url)).AsString(1024).Nullable()
                .WithColumn(nameof(ActivitiesTheme.QrcodeUrl)).AsString(1024).Nullable()
                .WithColumn(nameof(ActivitiesTheme.StartDateTimeUtc)).AsDateTime2().Nullable()
                .WithColumn(nameof(ActivitiesTheme.EndDateTimeUtc)).AsDateTime2().Nullable()
                ;
        }

        #endregion
    }
}
