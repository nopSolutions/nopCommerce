using System.Collections.Generic;

namespace Nop.Plugin.Shipping.EasyPost.Domain.Shipment
{
    /// <summary>
    /// Represents predefined package details
    /// </summary>
    public static class PredefinedPackage
    {
        /// <summary>
        /// Gets all available predefined packages
        /// </summary>
        public static Dictionary<string, List<string>> PredefinedPackages => new()
        {
            ["DHL Express"] = new()
            {
                "JumboDocument",
                "JumboParcel",
                "Document",
                "DHLFlyer",
                "Domestic",
                "ExpressDocument",
                "DHLExpressEnvelope",
                "JumboBox",
                "JumboJuniorDocument",
                "JuniorJumboBox",
                "JumboJuniorParcel",
                "OtherDHLPackaging",
                "Parcel"
            },

            ["DPD UK"] = new()
            {
                "Parcel",
                "Pallet",
                "ExpressPak",
                "FreightParcel",
                "Freight"
            },

            ["Estafeta"] = new()
            {
                "ENVELOPE",
                "PARCEL"
            },

            ["Fastway"] = new()
            {
                "Parcel",
                "A2",
                "A3",
                "A4",
                "A5",
                "BOXSML",
                "BOXMED",
                "BOXLRG"
            },

            ["FedEx"] = new()
            {
                "FedExEnvelope",
                "FedExBox",
                "FedExPak",
                "FedExTube",
                "FedEx10kgBox",
                "FedEx25kgBox",
                "FedExSmallBox",
                "FedExMediumBox",
                "FedExLargeBox",
                "FedExExtraLargeBox",
            },

            ["Interlink Express"] = new()
            {
                "Parcel",
                "Pallet",
                "ExpressPak",
                "FreightParcel",
                "Freight"
            },

            ["LaserShip"] = new()
            {
                "Envelope"
            },

            ["OnTrac"] = new()
            {
                "Letter"
            },

            ["Purolator"] = new()
            {
                "CustomerPackaging",
                "ExpressPack",
                "ExpressBox",
                "ExpressEnvelope"
            },

            ["Royal Mail"] = new()
            {
                "Letter",
                "LargeLetter",
                "SmallParcel",
                "MediumParcel",
                "Parcel"
            },

            ["SEKO OmniParcel"] = new()
            {
                "Bag",
                "Box",
                "Carton",
                "Container",
                "Crate",
                "Envelope",
                "Pail",
                "Pallet",
                "Satchel",
                "Tub"
            },

            ["StarTrack"] = new()
            {
                "Carton",
                "Pallet",
                "Satchel",
                "Bag",
                "Envelope",
                "Item",
                "Jiffybag",
                "Skid"
            },

            ["TForce"] = new()
            {
                "Parcel",
                "Letter"
            },

            ["UPS"] = new()
            {
                "UPSLetter",
                "UPSExpressBox",
                "UPS25kgBox",
                "UPS10kgBox",
                "Tube",
                "Pak",
                "SmallExpressBox",
                "MediumExpressBox",
                "LargeExpressBox"
            },

            ["USPS"] = new()
            {
                "Card",
                "Letter",
                "Flat",
                "FlatRateEnvelope",
                "FlatRateLegalEnvelope",
                "FlatRatePaddedEnvelope",
                "FlatRateGiftCardEnvelope",
                "FlatRateWindowEnvelope",
                "FlatRateCardboardEnvelope",
                "SmallFlatRateEnvelope",
                "Parcel",
                "LargeParcel",
                "IrregularParcel",
                "SoftPack",
                "SmallFlatRateBox",
                "MediumFlatRateBox",
                "LargeFlatRateBox",
                "LargeFlatRateBoxAPOFPO",
                "LargeFlatRateBoardGameBox",
                "RegionalRateBoxA",
                "RegionalRateBoxB",
                "FlatTubTrayBox",
                "EMMTrayBox",
                "FullTrayBox",
                "HalfTrayBox",
                "PMODSack"
            }
        };
    }
}