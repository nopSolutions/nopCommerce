using Nop.Plugin.Misc.AbcCore.Domain;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.AbcPickupInStore.Models;

namespace Nop.Plugin.Widgets.AbcPickupInStore.Factories
{
    public interface IPickStoreModelFactory
    {
        Task<PickStoreModel> InitializePickStoreModelAsync();
    }
}