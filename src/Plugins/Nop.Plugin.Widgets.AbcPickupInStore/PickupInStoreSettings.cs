using System;
using Nop.Core.Configuration;
using Nop.Plugin.Widgets.AbcPickupInStore.Models;

namespace Nop.Plugin.Widgets.AbcPickupInStore
{
    public class PickupInStoreSettings : ISettings
    {
        public string PickupInStoreText { get; private set; }

        internal PickupInStoreModel ToModel()
        {
            return new PickupInStoreModel()
            {
                PickupInStoreText = PickupInStoreText
            };
        }

        internal static PickupInStoreSettings FromModel(PickupInStoreModel model)
        {
            return new PickupInStoreSettings()
            {
                PickupInStoreText = model.PickupInStoreText
            };
        }

        internal static PickupInStoreSettings Default()
        {
            return new PickupInStoreSettings()
            {
                PickupInStoreText = "Pickup In-Store or Curbside"
            };
        }
    }
}
