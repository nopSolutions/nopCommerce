using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class StoreMappingBuilder : BaseEntityBuilder<StoreMapping>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(StoreMapping.EntityName)).AsString(400).NotNullable()
                .WithColumn(nameof(StoreMapping.StoreId))
                    .AsInt32()
                    .ForeignKey<Store>();
        }

        #endregion
    }
}