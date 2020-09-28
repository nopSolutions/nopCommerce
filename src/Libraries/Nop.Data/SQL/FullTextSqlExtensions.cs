using System;
using LinqToDB;
using LinqToDB.Expressions;
using LinqToDB.Linq;
using Nop.Core.Domain.Common;
using static LinqToDB.Sql;

namespace Nop.Data.DataProviders.SQL
{
    public static class FullTextSqlExtensions
    {
        [Extension(ProviderName.PostgreSQL, "to_tsvector(concat({columns, ', '}, ' ')) @@ {queryFnName}('{keywords}')", IsPredicate = true, ServerSideOnly = true, BuilderType = typeof(PostgreSQLFullTextArgumentsBuilder))]
        public static bool FullTextSearch(this ISqlExtension ext, [SqlQueryDependent] FulltextSearchMode mode, [SqlQueryDependent] string keywords, [ExprParameter] params object[] columns)
        {
            throw new LinqException($"'{nameof(FullTextSearch)}' is server-side method.");
        }

        class PostgreSQLFullTextArgumentsBuilder : IExtensionCallBuilder
        {
            public void Build(ISqExtensionBuilder builder)
            {
                var mode = builder.GetValue<FulltextSearchMode>("mode");
                var keywords = builder.GetValue<string>("keywords");

                var queryFnName = "to_tsquery";

                if (keywords is null)
                    throw new ArgumentNullException(nameof(keywords));

                var searchKeywords = keywords.Trim().Replace("''", "'").Replace("\"", string.Empty);

                switch (mode)
                {
                    case FulltextSearchMode.Or:
                        keywords = keywords.Replace(" ", " | ");
                        queryFnName = "to_tsquery";
                        break;
                    case FulltextSearchMode.And:
                        queryFnName = "plainto_tsquery";
                        break;
                    case FulltextSearchMode.ExactMatch:
                        queryFnName = "phraseto_tsquery";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode));
                }

                builder.AddExpression("queryFnName", queryFnName);
                builder.AddExpression("keywords", keywords);
            }
        }
    }
}