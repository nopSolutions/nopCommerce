using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;

namespace Nop.Data.Migrations.Builders
{
    /// <summary>
    /// Represents a generic attribute mapping configuration
    /// </summary>
    public partial class GenericAttributeBuilder : BaseEntityBuilder<GenericAttribute>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(GenericAttribute.KeyGroup)).AsString(400).NotNullable()
                .WithColumn(nameof(GenericAttribute.Key)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(GenericAttribute.Value)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}