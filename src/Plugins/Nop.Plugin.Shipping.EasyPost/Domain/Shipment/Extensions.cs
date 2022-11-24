using System.Collections.Generic;
using System.Linq;
using EasyPost.Models.API;

namespace Nop.Plugin.Shipping.EasyPost.Domain.Shipment
{
    /// <summary>
    /// Represents domain extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Check whether two object instances are matches
        /// </summary>
        /// <param name="parameters">Parcel parameters</param>
        /// <param name="parcel">Parcel</param>
        /// <returns>Result</returns>
        public static bool Matches(this Dictionary<string, object> parameters, Parcel parcel)
        {
            if (parcel is null)
                return false;

            var parameters1 = parcel.ToDictionary();
            return parameters.Keys.All(key => parameters1.TryGetValue(key, out var value) &&
                string.Equals(parameters[key]?.ToString(), value?.ToString(), System.StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Check whether two object instances are matches
        /// </summary>
        /// <param name="parameters">Address parameters</param>
        /// <param name="address">Address</param>
        /// <returns>Result</returns>
        public static bool Matches(this Dictionary<string, object> parameters, Address address)
        {
            if (address is null)
                return false;

            var parameters1 = address.ToDictionary();
            return parameters.Keys.All(key => parameters1.TryGetValue(key, out var value) &&
                string.Equals(parameters[key]?.ToString(), value?.ToString(), System.StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Check whether two object instances are matches
        /// </summary>
        /// <param name="parameters">Options parameters</param>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static bool Matches(this Dictionary<string, object> parameters, Options options)
        {
            if (options is null)
                return false;

            var parameters1 = options.ToDictionary();
            return parameters.Keys.All(key => parameters1.TryGetValue(key, out var value) &&
                string.Equals(parameters[key]?.ToString(), value?.ToString(), System.StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Check whether two object instances are matches
        /// </summary>
        /// <param name="parameters">Customs info parameters</param>
        /// <param name="customsInfo">Customs info</param>
        /// <returns>Result</returns>
        public static bool Matches(this Dictionary<string, object> parameters, CustomsInfo customsInfo)
        {
            if (customsInfo is null)
                return false;

            var parameters1 = customsInfo.ToDictionary();
            return parameters.Keys.Where(key => key != "customs_items").All(key => parameters1.TryGetValue(key, out var value) &&
                string.Equals(parameters[key]?.ToString(), value?.ToString(), System.StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Convert object to dictionary
        /// </summary>
        /// <param name="parcel">Parcel</param>
        /// <returns>Result</returns>
        public static Dictionary<string, object> ToDictionary(this Parcel parcel)
        {
            return new()
            {
                ["weight"] = parcel.Weight,
                ["width"] = parcel.Width,
                ["length"] = parcel.Length,
                ["height"] = parcel.Height,
                ["predefined_package"] = parcel.PredefinedPackage
            };
        }

        /// <summary>
        /// Convert object to dictionary
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Result</returns>
        public static Dictionary<string, object> ToDictionary(this Address address)
        {
            return new()
            {
                ["name"] = address.Name,
                ["email"] = address.Email,
                ["phone"] = address.Phone,
                ["company"] = address.Company,
                ["street1"] = address.Street1,
                ["street2"] = address.Street2,
                ["city"] = address.City,
                ["state"] = address.State,
                ["country"] = address.Country,
                ["zip"] = address.Zip
            };
        }

        /// <summary>
        /// Convert object to dictionary
        /// </summary>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static Dictionary<string, object> ToDictionary(this Options options)
        {
            return new()
            {
                ["additional_handling"] = options.AdditionalHandling,
                ["alcohol"] = options.Alcohol,
                ["by_drone"] = options.ByDrone,
                ["carbon_neutral"] = options.CarbonNeutral,
                ["delivery_confirmation"] = options.DeliveryConfirmation,
                ["endorsement"] = options.Endorsement,
                ["handling_instructions"] = options.HandlingInstructions,
                ["hazmat"] = options.Hazmat,
                ["invoice_number"] = options.InvoiceNumber,
                ["machinable"] = options.Machinable,
                ["print_custom_1"] = options.PrintCustom1,
                ["print_custom_1_code"] = options.PrintCustom1Code,
                ["print_custom_2"] = options.PrintCustom2,
                ["print_custom_2_code"] = options.PrintCustom2Code,
                ["print_custom_3"] = options.PrintCustom3,
                ["print_custom_3_code"] = options.PrintCustom3Code,
                ["special_rates_eligibility"] = options.SpecialRatesEligibility,
                ["certified_mail"] = options.CertifiedMail,
                ["registered_mail"] = options.RegisteredMail,
                ["registered_mail_amount"] = options.RegisteredMailAmount,
                ["return_receipt"] = options.ReturnReceipt
            };
        }

        /// <summary>
        /// Convert object to dictionary
        /// </summary>
        /// <param name="customsInfo">Customs info</param>
        /// <returns>Result</returns>
        public static Dictionary<string, object> ToDictionary(this CustomsInfo customsInfo)
        {
            return new()
            {
                ["contents_type"] = customsInfo.ContentsType,
                ["restriction_type"] = customsInfo.RestrictionType,
                ["non_delivery_option"] = customsInfo.NonDeliveryOption,
                ["contents_explanation"] = customsInfo.ContentsExplanation,
                ["restriction_comments"] = customsInfo.RestrictionComments,
                ["customs_certify"] = customsInfo.CustomsCertify,
                ["customs_signer"] = customsInfo.CustomsSigner,
                ["eel_pfc"] = customsInfo.EelPfc
            };
        }
    }
}