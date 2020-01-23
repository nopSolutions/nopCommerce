using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Builders
{
    public partial class ManufacturerTemplateBuilder : BaseEntityBuilder<ManufacturerTemplate>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table) 
        {
            table
                .WithColumn(nameof(ManufacturerTemplate.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(ManufacturerTemplate.ViewPath)).AsString(400).NotNullable();
        }
        
        #endregion
    }
}