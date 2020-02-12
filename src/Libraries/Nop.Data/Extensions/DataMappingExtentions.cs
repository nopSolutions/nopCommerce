using LinqToDB;
using LinqToDB.Mapping;
using Nop.Core;

namespace Nop.Data.Extensions
{
    /// <summary>
    /// PropertyMappingBuilder extensions
    /// </summary>
    public static partial class DataMappingExtensions
    {
        #region Methods

        /// <summary>
        /// Sets LINQ Decimal to DB type for current column
        /// </summary>
        /// <typeparam name="TEntity">Type of entity</typeparam>
        /// <param name="builder">Property mapping builder</param>
        /// <param name="precision">Precision</param>
        /// <param name="scale">Scale</param>
        /// <returns></returns>
        public static PropertyMappingBuilder<TEntity> HasDecimal<TEntity>(this PropertyMappingBuilder<TEntity> builder,
            int precision = 18,
            int scale = 4) where TEntity : BaseEntity
        {
            return builder
                .HasDataType(DataType.Decimal)
                .HasPrecision(precision)
                .HasScale(scale);
        }

        #endregion
    }
}
