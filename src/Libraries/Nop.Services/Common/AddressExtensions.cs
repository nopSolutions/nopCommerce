using System.Collections.Generic;
using Nop.Core.Domain.Common;

namespace Nop.Services.Common
{
    /// <summary>
    /// Address extensions
    /// </summary>
    public static class AddressExtensions
    {
        /// <summary>
        /// Find an address
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="email">Email</param>
        /// <param name="faxNumber">Fax number</param>
        /// <param name="company">Company</param>
        /// <param name="address1">Address 1</param>
        /// <param name="address2">Address 2</param>
        /// <param name="city">City</param>
        /// <param name="county">County</param>
        /// <param name="stateProvinceId">State/province identifier</param>
        /// <param name="zipPostalCode">Zip postal code</param>
        /// <param name="countryId">Country identifier</param>
        /// <param name="customAttributes">Custom address attributes (XML format)</param>
        /// <returns>Address</returns>
        public static Address FindAddress(this List<Address> source,
            string firstName, string lastName, string phoneNumber,
            string email, string faxNumber, string company, string address1,
            string address2, string city, string county, int? stateProvinceId,
            string zipPostalCode, int? countryId, string customAttributes)
        {
            return source.Find(a => ((string.IsNullOrEmpty(a.FirstName) && string.IsNullOrEmpty(firstName)) || a.FirstName == firstName) &&
                ((string.IsNullOrEmpty(a.LastName) && string.IsNullOrEmpty(lastName)) || a.LastName == lastName) &&
                ((string.IsNullOrEmpty(a.PhoneNumber) && string.IsNullOrEmpty(phoneNumber)) || a.PhoneNumber == phoneNumber) &&
                ((string.IsNullOrEmpty(a.Email) && string.IsNullOrEmpty(email)) || a.Email == email) &&
                ((string.IsNullOrEmpty(a.FaxNumber) && string.IsNullOrEmpty(faxNumber)) || a.FaxNumber == faxNumber) &&
                ((string.IsNullOrEmpty(a.Company) && string.IsNullOrEmpty(company)) || a.Company == company) &&
                ((string.IsNullOrEmpty(a.Address1) && string.IsNullOrEmpty(address1)) || a.Address1 == address1) &&
                ((string.IsNullOrEmpty(a.Address2) && string.IsNullOrEmpty(address2)) || a.Address2 == address2) &&
                ((string.IsNullOrEmpty(a.City) && string.IsNullOrEmpty(city)) || a.City == city) &&
                ((string.IsNullOrEmpty(a.County) && string.IsNullOrEmpty(county)) || a.County == county) &&
                ((a.StateProvinceId == null && stateProvinceId == null) || a.StateProvinceId.Value == stateProvinceId.Value) &&
                ((string.IsNullOrEmpty(a.ZipPostalCode) && string.IsNullOrEmpty(zipPostalCode)) || a.ZipPostalCode == zipPostalCode) &&
                ((a.CountryId == null && countryId == null) || a.CountryId.Value == countryId.Value) &&
                //actually we should parse custom address attribute (in case if "Display order" is changed) and then compare
                //bu we simplify this process and simply compare their values in XML
                ((string.IsNullOrEmpty(a.CustomAttributes) && string.IsNullOrEmpty(customAttributes)) || a.CustomAttributes == customAttributes));
        }
    }
}
