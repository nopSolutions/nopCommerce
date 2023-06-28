using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Messages;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo470
{
    [NopSchemaMigration("2023-03-07 00:00:01", "SchemaMigration for 4.70.0")]
    public class SchemaMigration : ForwardOnlyMigration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            //#6167
            var messageTemplateTableName = nameof(MessageTemplate);
            var allowDirectReplyColumnName = nameof(MessageTemplate.AllowDirectReply);

            //add column
            if (!Schema.Table(messageTemplateTableName).Column(allowDirectReplyColumnName).Exists())
                Alter.Table(messageTemplateTableName)
                    .AddColumn(allowDirectReplyColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);

            //1934
            if (!Schema.Table(nameof(ProductAttributeCombinationPicture)).Exists()) 
                Create.TableFor<ProductAttributeCombinationPicture>();

            if (!Schema.Table(nameof(ProductAttributeValuePicture)).Exists()) 
                Create.TableFor<ProductAttributeValuePicture>();

            var productTableName = nameof(Product);
            var displayAttributeCombinationImagesOnlyColumnName = nameof(Product.DisplayAttributeCombinationImagesOnly);

            if (!Schema.Table(productTableName).Column(displayAttributeCombinationImagesOnlyColumnName).Exists())
                Alter.Table(productTableName)
                    .AddColumn(displayAttributeCombinationImagesOnlyColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
        }
    }
}
