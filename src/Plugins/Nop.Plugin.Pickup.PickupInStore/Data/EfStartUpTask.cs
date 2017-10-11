using Microsoft.EntityFrameworkCore.Storage;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Pickup.PickupInStore.Data
{
    public class EfStartUpTask : IStartupTask
    {
        public void Execute()
        {
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
