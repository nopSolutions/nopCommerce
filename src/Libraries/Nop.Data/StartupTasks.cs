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

        public int Order
        {
            //ensure that this task is run first 
            get { return -1000; }
        }
    }
}
