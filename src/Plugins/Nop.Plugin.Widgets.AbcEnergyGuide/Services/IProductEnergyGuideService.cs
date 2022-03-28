using Nop.Plugin.Misc.AbcCore.Domain;

namespace Nop.Plugin.Widgets.AbcEnergyGuide.Services
{
    public interface IProductEnergyGuideService
    {
        ProductEnergyGuide GetEnergyGuideByProductId(int productId);
    }
}