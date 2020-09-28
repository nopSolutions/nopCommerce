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