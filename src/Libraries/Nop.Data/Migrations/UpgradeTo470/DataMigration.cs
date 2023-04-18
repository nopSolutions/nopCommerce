using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;

namespace Nop.Data.Migrations.UpgradeTo470
{
    [NopUpdateMigration("2023-01-01 00:00:00", "4.70.0", UpdateMigrationType.Data)]
    public class DataMigration : Migration
    {
        private readonly INopDataProvider _dataProvider;

        public DataMigration(INopDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            //#5312 new activity log type
            var activityLogTypeTable = _dataProvider.GetTable<ActivityLogType>();

            if (!activityLogTypeTable.Any(alt =>
                    string.Compare(alt.SystemKeyword, "ImportCustomers", StringComparison.InvariantCultureIgnoreCase) ==
                    0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ImportCustomers", Enabled = true, Name = "Customers were imported"
                    }
                );

            //6660 new activity log type for update plugin
            if (!activityLogTypeTable.Any(alt =>
                    string.Compare(alt.SystemKeyword, "UpdatePlugin", StringComparison.InvariantCultureIgnoreCase) ==
                    0))
                _dataProvider.InsertEntity(
                    new ActivityLogType { SystemKeyword = "UpdatePlugin", Enabled = true, Name = "Update a plugin" }
                );

            //1934
            int pageIndex;
            var pageSize = 500;
            var productAttributeCombinationTableName = nameof(ProductAttributeCombination);

            var pac = Schema.Table(productAttributeCombinationTableName);
            var columnName = "PictureId";

            if (pac.Column(columnName).Exists())
            {
                var combinationQuery =
                    from c in _dataProvider.GetTable<ProductAttributeCombination>()
                    join p in _dataProvider.GetTable<Picture>() on c.PictureId equals p.Id
                    select c;

                pageIndex = 0;

                while (true)
                {
                    var combinations = combinationQuery.ToPagedListAsync(pageIndex, pageSize).Result;

                    if (!combinations.Any())
                        break;

                    foreach (var combination in combinations)
                        _dataProvider.InsertEntity(new ProductAttributeCombinationPicture
                        {
                            PictureId = combination.PictureId, ProductAttributeCombinationId = combination.Id
                        });

                    pageIndex++;
                }

                // delete picture column after moving data to combination piture table
                Delete.Column(columnName).FromTable(productAttributeCombinationTableName);
            }

            var productAttributeValueTableName = nameof(ProductAttributeValue);
            var pav = Schema.Table(productAttributeValueTableName);

            if (pav.Column(columnName).Exists())
            {
                var valueQuery =
                    from c in _dataProvider.GetTable<ProductAttributeValue>()
                    join p in _dataProvider.GetTable<Picture>() on c.PictureId equals p.Id
                    select c;

                pageIndex = 0;

                while (true)
                {
                    var values = valueQuery.ToPagedListAsync(pageIndex, pageSize).Result;

                    if (!values.Any())
                        break;

                    foreach (var value in values)
                        _dataProvider.InsertEntity(new ProductAttributeValuePicture
                        {
                            PictureId = value.PictureId, ProductAttributeValueId = value.Id
                        });

                    pageIndex++;
                }

                // delete picture column after moving data to value piture table
                Delete.Column(columnName).FromTable(productAttributeValueTableName);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
