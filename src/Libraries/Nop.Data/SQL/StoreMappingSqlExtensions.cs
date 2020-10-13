using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToDB;
using Nop.Core;
using Nop.Core.Domain.Stores;

namespace Nop.Data.DataProviders.SQL
{
    public static class MultiStoreSqlExtensions
    {
        /// <summary>
        /// Builds store mapping predicate
        /// </summary>
        /// <param name="subjectEntity">Mapped entity</param>
        /// <param name="storeMapping">Source with store mapping records</param>
        /// <param name="storeId">Store identifier</param>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns></returns>
        [ExpressionMethod(nameof(LimitedToStoresImpl))]
		public static bool LimitedToStores<T>(this T subjectEntity, IQueryable<StoreMapping> storeMapping, int storeId) where T : BaseEntity, IStoreMappingSupported
		{
            return !subjectEntity.LimitedToStores || storeMapping.Any(sm =>
                        sm.EntityName == typeof(T).Name &&
                        sm.EntityId == subjectEntity.Id &&
                        sm.StoreId == storeId);
		}

		static Expression<Func<T, IQueryable<StoreMapping>, int, bool>> LimitedToStoresImpl<T>() where T : BaseEntity, IStoreMappingSupported
		{
			return (subjectEntity, storeMapping, storeId) => !subjectEntity.LimitedToStores || 
                storeMapping.Any(sm =>
                        sm.EntityName == typeof(T).Name &&
                        sm.EntityId == subjectEntity.Id &&
                        sm.StoreId == storeId);
		}
    }
}