using LinqToDB;
using LinqToDB.Mapping;
using Nop.Core;

namespace Nop.Data.Extensions
{
    public static partial class DataMappingExtentions
    {
        public static PropertyMappingBuilder<TEntity> HasDecimal<TEntity>(this PropertyMappingBuilder<TEntity> builder,
            int precision = 18,
            int scale = 4) where TEntity : BaseEntity
        {
            return builder
                .HasDataType(DataType.Decimal)
                .HasPrecision(precision)
                .HasScale(scale);
        }
    }
}
