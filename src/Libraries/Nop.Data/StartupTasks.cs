using System.Data.Entity.Database;
using Nop.Core.Tasks;

namespace Nop.Data
{
    public class EFStartUpTask : IStartupTask
    {
        public void Execute()
        {
            DbDatabase.SetInitializer<NopObjectContext>(new DatabaseInitializer());
        }
    }
}
