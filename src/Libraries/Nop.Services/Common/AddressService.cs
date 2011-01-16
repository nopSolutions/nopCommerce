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
using System.Linq;
using Nop.Data;
using Nop.Core.Domain.Common;

namespace Nop.Services.Common
{
    /// <summary>
    /// Customer service
    /// </summary>
    public partial class AddressService : IAddressService
    {
        #region Fields

        private readonly IRepository<Address> _addressRepository;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="addressRepository">Address repository</param>
        public AddressService(IRepository<Address> addressRepository)
        {
            this._addressRepository = addressRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an address
        /// </summary>
        /// <param name="address">Address</param>
        public void DeleteAddress(Address address)
        {
            if (address == null)
                return;

            //TODO ensure that customer will not be deleted
            //in case BillingAddress or ShippingAddress is set to this address

            _addressRepository.Delete(address);
        }

        /// <summary>
        /// Gets an address by address identifier
        /// </summary>
        /// <param name="addressId">Address identifier</param>
        /// <returns>Address</returns>
        public Address GetAddressById(int addressId)
        {
            if (addressId == 0)
                return null;

            var address = _addressRepository.GetById(addressId);
            return address;
        }

        /// <summary>
        /// Inserts an address
        /// </summary>
        /// <param name="address">Address</param>
        public void InsertAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            
            _addressRepository.Insert(address);
        }

        /// <summary>
        /// Updates the address
        /// </summary>
        /// <param name="address">Address</param>
        public void UpdateAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            _addressRepository.Update(address);
        }

        #endregion
    }
}