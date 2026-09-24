using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-08-12 00:00:00", "Remove obsolete ProductAttributeCombination.PictureId and ProductAttributeValue.PictureId properties")]
public class MultiplePicturesMigration : ForwardOnlyMigration
{
    private readonly INopDataProvider _dataProvider;

    public MultiplePicturesMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        int pageIndex;
        var pageSize = 500;

        var pac = Schema.Table(nameof(ProductAttributeCombination));
        var columnName = "PictureId";

        if (pac.Column(columnName).Exists())
        {
            var combinationQuery =
                from c in _dataProvider.GetTable<ProductAttributeCombinationWithPictureId>()
                join p in _dataProvider.GetTable<Picture>() on c.PictureId equals p.Id
                select c;

            pageIndex = 0;

            while (true)
            {
                var combinations = combinationQuery.Skip(pageIndex * pageSize).Take(pageSize).ToList();

                if (!combinations.Any())
                    break;

                foreach (var combination in combinations.Where(combination => combination.PictureId.HasValue))
                {
                    _dataProvider.InsertEntity(new ProductAttributeCombinationPicture
                    {
                        PictureId = combination.PictureId!.Value,
                        ProductAttributeCombinationId = combination.Id
                    });

                    combination.PictureId = null;
                }

                _dataProvider.UpdateEntitiesAsync(combinations);

                pageIndex++;
            }

            this.DeleteColumnsIfExists<ProductAttributeCombination>(["PictureId"]);
        }

        var pav = Schema.Table(nameof(ProductAttributeValue));

        if (pav.Column(columnName).Exists())
        {
            var valueQuery =
                from c in _dataProvider.GetTable<ProductAttributeValueWithPictureId>()
                join p in _dataProvider.GetTable<Picture>() on c.PictureId equals p.Id
                select c;

            pageIndex = 0;

            while (true)
            {
                var values = valueQuery.Skip(pageIndex * pageSize).Take(pageSize).ToList();

                if (!values.Any())
                    break;

                foreach (var value in values.Where(value => value.PictureId.HasValue))
                {
                    _dataProvider.InsertEntity(new ProductAttributeValuePicture
                    {
                        PictureId = value.PictureId!.Value,
                        ProductAttributeValueId = value.Id
                    });

                    value.PictureId = null;
                }

                _dataProvider.UpdateEntitiesAsync(values);

                pageIndex++;
            }

            this.DeleteColumnsIfExists<ProductAttributeValue>(["PictureId"]);
        }
    }

    #region Nested classes

    public class ProductAttributeCombinationWithPictureId : ProductAttributeCombination
    {
        public int? PictureId { get; set; }
    }

    public class ProductAttributeValueWithPictureId : ProductAttributeValue
    {
        public int? PictureId { get; set; }
    }

    public class PictureIdNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames =>
            new()
            {
                [typeof(ProductAttributeCombinationWithPictureId)] = nameof(ProductAttributeCombination),
                [typeof(ProductAttributeValueWithPictureId)] = nameof(ProductAttributeValue)
            };

        public Dictionary<(Type, string), string> ColumnName => [];
    }

    #endregion
}