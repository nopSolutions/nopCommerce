using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public interface IBaseService
    {
        void Delete(string table_name, string where_clause, IList<OdbcParameter> where_params, bool batch = false);
        void ExecuteBatch();
        DataSet Get(string table_name, IList<string> columns, string where_clause, IList<OdbcParameter> where_params);
        void Insert(string table_name, IList<string> column_names, IList<OdbcParameter> parameters, bool batch = false);
        void Update(string table_name, IList<string> column_names, IList<OdbcParameter> parameters, string where_clause, IList<OdbcParameter> where_params, bool batch = false);
    }
}