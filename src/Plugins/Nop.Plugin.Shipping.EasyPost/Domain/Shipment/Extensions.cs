using EasyPost;

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
        /// <param name="parcel">Parcel</param>
        /// <param name="parcel1">Parcel</param>
        /// <returns>Result</returns>
        public static bool Matches(this Parcel parcel, Parcel parcel1)
        {
            if (parcel1 is null)
                return false;

            return parcel.weight == parcel1.weight &&
                parcel.width == parcel1.width &&
                parcel.length == parcel1.length &&
                parcel.height == parcel1.height &&
                string.Equals(parcel.predefined_package, parcel1.predefined_package, System.StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Check whether two object instances are matches
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="address1">Address</param>
        /// <returns>Result</returns>
        public static bool Matches(this Address address, Address address1)
        {
            if (address1 is null)
                return false;

            return string.Equals(address.street1, address1.street1, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(address.city, address1.city, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(address.state, address1.state, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(address.country, address1.country, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(address.zip, address1.zip, System.StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Check whether two object instances are matches
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="options1">Options</param>
        /// <returns>Result</returns>
        public static bool Matches(this Options options, Options options1)
        {
            if (options1 is null)
                return false;

            return options.additional_handling == options1.additional_handling &&
                options.alcohol == options1.alcohol &&
                options.by_drone == options1.by_drone &&
                options.carbon_neutral == options1.carbon_neutral &&
                string.Equals(options.delivery_confirmation, options1.delivery_confirmation, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(options.endorsement, options1.endorsement, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(options.handling_instructions, options1.handling_instructions, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(options.hazmat, options1.hazmat, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(options.invoice_number, options1.invoice_number, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(options.machinable, options1.machinable, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(options.print_custom_1, options1.print_custom_1, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(options.print_custom_1_code, options1.print_custom_1_code, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(options.print_custom_2, options1.print_custom_2, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(options.print_custom_2_code, options1.print_custom_2_code, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(options.print_custom_3, options1.print_custom_3, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(options.print_custom_3_code, options1.print_custom_3_code, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(options.special_rates_eligibility, options1.special_rates_eligibility, System.StringComparison.InvariantCultureIgnoreCase) &&
                options.certified_mail == options1.certified_mail &&
                options.registered_mail == options1.registered_mail &&
                options.registered_mail_amount == options1.registered_mail_amount &&
                options.return_receipt == options1.return_receipt;
        }

        /// <summary>
        /// Check whether two object instances are matches
        /// </summary>
        /// <param name="customsInfo">Customs info</param>
        /// <param name="customsInfo1">Customs info</param>
        /// <returns>Result</returns>
        public static bool Matches(this CustomsInfo customsInfo, CustomsInfo customsInfo1)
        {
            if (customsInfo1 is null)
                return false;

            return string.Equals(customsInfo.contents_type, customsInfo1.contents_type, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(customsInfo.restriction_type, customsInfo1.restriction_type, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(customsInfo.non_delivery_option, customsInfo1.non_delivery_option, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(customsInfo.contents_explanation, customsInfo1.contents_explanation, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(customsInfo.restriction_comments, customsInfo1.restriction_comments, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(customsInfo.customs_certify, customsInfo1.customs_certify, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(customsInfo.customs_signer, customsInfo1.customs_signer, System.StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(customsInfo.eel_pfc, customsInfo1.eel_pfc, System.StringComparison.InvariantCultureIgnoreCase);
        }
    }
}