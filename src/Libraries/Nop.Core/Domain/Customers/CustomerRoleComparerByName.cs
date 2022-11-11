using System.Collections.Generic;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Custom comparer for the CustomerRole class by Name and SystemName
    /// </summary>
    public partial class CustomerRoleComparerByName : IEqualityComparer<CustomerRole>
    {
        /// <summary>
        /// Customer roles are equal if their names and system names are equal.
        /// </summary>
        public bool Equals(CustomerRole x, CustomerRole y)
        {
            //Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y))
                return true;

            //Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            //Check whether the customer role properties are equal.
            return x.Name == y.Name && x.SystemName == y.SystemName;
        }

        public int GetHashCode(CustomerRole customerRole)
        {
            //Check whether the object is null
            if (ReferenceEquals(customerRole, null))
                return 0;

            //Get hash code for the Name field if it is not null.
            var hashCustomerRoleName = customerRole.Name == null ? 0 : customerRole.Name.GetHashCode();

            //Get hash code for the SystemName field.
            var hashCustomerRoleSystemName = customerRole.SystemName == null ? 0 : customerRole.SystemName.GetHashCode();

            //Calculate the hash code for the CustomerRole.
            return hashCustomerRoleName ^ hashCustomerRoleSystemName;
        }
    }
}
