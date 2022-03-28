using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using Nop.Core;
using Nop.Data;

namespace Nop.Plugin.Misc.AbcCore.Data
{
    /// <summary>
    /// Represents a data provider
    /// </summary>
    public partial interface ICustomNopDataProvider : INopDataProvider
    {
        Task<int> ExecuteNonQueryAsync(string sql, int timeout, params DataParameter[] dataParameters);
    }
}