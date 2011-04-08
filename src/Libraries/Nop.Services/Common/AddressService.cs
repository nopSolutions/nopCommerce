

using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Data;

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
                throw new ArgumentNullException("address");

            //ensure that an associated order will not be deleted (an order with BillingAddress set to this one)
            //TODO uncomment code below
            //if (address.Id > 0)
            //{
            //    var query = from o in _orderRepository.Table
            //                where (o.BillingAddress.Id == address.Id)
            //                || (o.ShippingAddress != null && o.ShippingAddress.Id == address.Id)
            //                select o;
            //    var order = query.FirstOrDefault();
            //    if (order != null)
            //        throw new NopException(string.Format("Deleting address is not allowed because it's associated with order #{0}", order.Id));
            //}

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
            
            //TODO:Make sure other entities are doing this
            address.CreatedOnUtc = DateTime.UtcNow;

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