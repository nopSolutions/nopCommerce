using LinqToDB.Data;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Data
{
    public partial interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<IList<ProductCustom>> EntityFromSqlCustom(string storeProcedureName, params DataParameter[] dataParameters);
    }

    public partial class EntityRepository<TEntity>
    {

        #region Fields

        #endregion

        #region Ctor

        #endregion

        #region Methods

        public virtual Task<IList<ProductCustom>> EntityFromSqlCustom(string storeProcedureName, params DataParameter[] dataParameters)
        {
            return _dataProvider.QueryProcAsync<ProductCustom>(storeProcedureName, dataParameters?.ToArray());
        }

        #endregion
    }
}
