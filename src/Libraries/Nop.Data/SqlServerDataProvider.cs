using System.Data.Entity.Infrastructure;

namespace Nop.Data
{
    public class SqlServerDataProvider : BaseEfDataProvider
    {
        public override IDbConnectionFactory GetConnectionFactory()
        {
            return new SqlConnectionFactory();
        }
    }
}
