using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Localization;

namespace Nop.Data.Mapping.Builders.Localization
{
    public partial class LocalizedPropertyLocalGroupsBuilder : NopEntityBuilder<LocalizedLocalGroup>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(LocalizedLocalGroup.LocaleKey)).AsString(size: 256)
                .WithColumn(nameof(LocalizedLocalGroup.LocaleKeyGroupId)).AsInt32();
        }

        #endregion
    }
}
