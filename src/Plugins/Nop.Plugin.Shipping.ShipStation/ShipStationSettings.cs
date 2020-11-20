using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.ShipStation
{
    public class ShipStationSettings : ISettings
    {
        /// <summary>
        /// API key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// API secret
        /// </summary>
        public string ApiSecret { get; set; }

        /// <summary>
        /// Set to true if need pass dimensions to the ShipStation server
        /// </summary>
        public bool PassDimensions { get; set; }

        /// <summary>
        /// Packing type
        /// </summary>
        public PackingType PackingType { get; set; }
        
        /// <summary>
        /// Package volume
        /// </summary>
        public int PackingPackageVolume { get; set; }

        /// <summary>
        /// ShipStation user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// ShipStation password
        /// </summary>
        public string Password { get; set; }
    }
}