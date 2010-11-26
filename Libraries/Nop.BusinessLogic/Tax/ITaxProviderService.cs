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

using System.Collections.Generic;

namespace NopSolutions.NopCommerce.BusinessLogic.Tax
{
    /// <summary>
    /// Tax provider service interface
    /// </summary>
    public partial interface ITaxProviderService
    {
        /// <summary>
        /// Deletes a tax provider
        /// </summary>
        /// <param name="taxProviderId">Tax provider identifier</param>
        void DeleteTaxProvider(int taxProviderId);

        /// <summary>
        /// Gets a tax provider
        /// </summary>
        /// <param name="taxProviderId">Tax provider identifier</param>
        /// <returns>Tax provider</returns>
        TaxProvider GetTaxProviderById(int taxProviderId);

        /// <summary>
        /// Gets all tax providers
        /// </summary>
        /// <returns>Shipping rate computation method collection</returns>
        List<TaxProvider> GetAllTaxProviders();

        /// <summary>
        /// Inserts a tax provider
        /// </summary>
        /// <param name="taxProvider">Tax provider</param>
        void InsertTaxProvider(TaxProvider taxProvider);

        /// <summary>
        /// Updates the tax provider
        /// </summary>
        /// <param name="taxProvider">Tax provider</param>
        void UpdateTaxProvider(TaxProvider taxProvider);
    }
}
