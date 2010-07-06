//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates
{
    /// <summary>
    /// Affiliate manager
    /// </summary>
    public partial class AffiliateManager
    {
        #region Methods
        /// <summary>
        /// Gets an affiliate by affiliate identifier
        /// </summary>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <returns>Affiliate</returns>
        public static Affiliate GetAffiliateById(int affiliateId)
        {
            if (affiliateId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from a in context.Affiliates
                        where a.AffiliateId == affiliateId
                        select a;
            var affiliate = query.SingleOrDefault();

            return affiliate;
        }

        /// <summary>
        /// Marks affiliate as deleted 
        /// </summary>
        /// <param name="affiliateId">Affiliate identifier</param>
        public static void MarkAffiliateAsDeleted(int affiliateId)
        {
            var affiliate = GetAffiliateById(affiliateId);
            if (affiliate != null)
            {
                affiliate = UpdateAffiliate(affiliate.AffiliateId, affiliate.FirstName, affiliate.LastName, affiliate.MiddleName, affiliate.PhoneNumber,
                      affiliate.Email, affiliate.FaxNumber, affiliate.Company, affiliate.Address1, affiliate.Address2, affiliate.City,
                      affiliate.StateProvince, affiliate.ZipPostalCode, affiliate.CountryId, true, affiliate.Active);
            }
        }

        /// <summary>
        /// Gets all affiliates
        /// </summary>
        /// <returns>Affiliate collection</returns>
        public static List<Affiliate> GetAllAffiliates()
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from a in context.Affiliates
                        orderby a.LastName
                        where !a.Deleted
                        select a;
            var affiliates = query.ToList();

            return affiliates;
        }

        /// <summary>
        /// Inserts an affiliate
        /// </summary>
        /// <param name="firstName">The first name</param>
        /// <param name="lastName">The last name</param>
        /// <param name="middleName">The middle name</param>
        /// <param name="phoneNumber">The phone number</param>
        /// <param name="email">The email</param>
        /// <param name="faxNumber">The fax number</param>
        /// <param name="company">The company</param>
        /// <param name="address1">The address 1</param>
        /// <param name="address2">The address 2</param>
        /// <param name="city">The city</param>
        /// <param name="stateProvince">The state/province</param>
        /// <param name="zipPostalCode">The zip/postal code</param>
        /// <param name="countryId">The country identifier</param>
        /// <param name="deleted">A value indicating whether the entity has been deleted</param>
        /// <param name="active">A value indicating whether the entity is active</param>
        /// <returns>An affiliate</returns>
        public static Affiliate InsertAffiliate(string firstName,
            string lastName, string middleName, string phoneNumber,
            string email, string faxNumber, string company, string address1,
            string address2, string city, string stateProvince, string zipPostalCode,
            int countryId, bool deleted, bool active)
        {
            firstName = CommonHelper.EnsureMaximumLength(firstName, 100);
            lastName = CommonHelper.EnsureMaximumLength(lastName, 100);
            middleName = CommonHelper.EnsureMaximumLength(middleName, 100);
            phoneNumber = CommonHelper.EnsureMaximumLength(phoneNumber, 50);
            email = CommonHelper.EnsureMaximumLength(email, 255);
            faxNumber = CommonHelper.EnsureMaximumLength(faxNumber, 50);
            company = CommonHelper.EnsureMaximumLength(company, 100);
            address1 = CommonHelper.EnsureMaximumLength(address1, 100);
            address2 = CommonHelper.EnsureMaximumLength(address2, 100);
            city = CommonHelper.EnsureMaximumLength(city, 100);
            stateProvince = CommonHelper.EnsureMaximumLength(stateProvince, 100);
            zipPostalCode = CommonHelper.EnsureMaximumLength(zipPostalCode, 10);

            var context = ObjectContextHelper.CurrentObjectContext;

            var affiliate = context.Affiliates.CreateObject();
            affiliate.FirstName = firstName;
            affiliate.LastName = lastName;
            affiliate.MiddleName = middleName;
            affiliate.PhoneNumber = phoneNumber;
            affiliate.Email = email;
            affiliate.FaxNumber = faxNumber;
            affiliate.Company = company;
            affiliate.Address1 = address1;
            affiliate.Address2 = address2;
            affiliate.City = city;
            affiliate.StateProvince = stateProvince;
            affiliate.ZipPostalCode = zipPostalCode;
            affiliate.CountryId = countryId;
            affiliate.Deleted = deleted;
            affiliate.Active = active;

            context.Affiliates.AddObject(affiliate);
            context.SaveChanges();
            return affiliate;
        }

        /// <summary>
        /// Updates the affiliate
        /// </summary>
        /// <param name="affiliateId">The affiliate identifier</param>
        /// <param name="firstName">The first name</param>
        /// <param name="lastName">The last name</param>
        /// <param name="middleName">The middle name</param>
        /// <param name="phoneNumber">The phone number</param>
        /// <param name="email">The email</param>
        /// <param name="faxNumber">The fax number</param>
        /// <param name="company">The company</param>
        /// <param name="address1">The address 1</param>
        /// <param name="address2">The address 2</param>
        /// <param name="city">The city</param>
        /// <param name="stateProvince">The state/province</param>
        /// <param name="zipPostalCode">The zip/postal code</param>
        /// <param name="countryId">The country identifier</param>
        /// <param name="deleted">A value indicating whether the entity has been deleted</param>
        /// <param name="active">A value indicating whether the entity is active</param>
        /// <returns>An affiliate</returns>
        public static Affiliate UpdateAffiliate(int affiliateId, string firstName,
            string lastName, string middleName, string phoneNumber,
            string email, string faxNumber, string company, string address1,
            string address2, string city, string stateProvince, string zipPostalCode,
            int countryId, bool deleted, bool active)
        {
            firstName = CommonHelper.EnsureMaximumLength(firstName, 100);
            lastName = CommonHelper.EnsureMaximumLength(lastName, 100);
            middleName = CommonHelper.EnsureMaximumLength(middleName, 100);
            phoneNumber = CommonHelper.EnsureMaximumLength(phoneNumber, 50);
            email = CommonHelper.EnsureMaximumLength(email, 255);
            faxNumber = CommonHelper.EnsureMaximumLength(faxNumber, 50);
            company = CommonHelper.EnsureMaximumLength(company, 100);
            address1 = CommonHelper.EnsureMaximumLength(address1, 100);
            address2 = CommonHelper.EnsureMaximumLength(address2, 100);
            city = CommonHelper.EnsureMaximumLength(city, 100);
            stateProvince = CommonHelper.EnsureMaximumLength(stateProvince, 100);
            zipPostalCode = CommonHelper.EnsureMaximumLength(zipPostalCode, 10);

            var affiliate = GetAffiliateById(affiliateId);
            if (affiliate == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(affiliate))
                context.Affiliates.Attach(affiliate);

            affiliate.FirstName = firstName;
            affiliate.LastName = lastName;
            affiliate.MiddleName = middleName;
            affiliate.PhoneNumber = phoneNumber;
            affiliate.Email = email;
            affiliate.FaxNumber = faxNumber;
            affiliate.Company = company;
            affiliate.Address1 = address1;
            affiliate.Address2 = address2;
            affiliate.City = city;
            affiliate.StateProvince = stateProvince;
            affiliate.ZipPostalCode = zipPostalCode;
            affiliate.CountryId = countryId;
            affiliate.Deleted = deleted;
            affiliate.Active = active;
            context.SaveChanges();
            return affiliate;
        }
        #endregion
    }
}
