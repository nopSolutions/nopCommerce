using Nop.Core.Domain.Common;

namespace Nop.Plugin.Misc.AbcExportOrder.Models
{
    public class YahooShipToRowShipping : YahooShipToRow
    {
        public YahooShipToRowShipping(
            string prefix,
            int orderId,
            Address address,
            string stateAbbreviation,
            string country
        ) : base(prefix, orderId)
        {
            Id = $"{prefix}{orderId}+s";
            FullName = $"{address.FirstName} {address.LastName}";
            FirstName = address.FirstName;
            LastName = address.LastName;
            Address1 = address.Address1;
            Address2 = address.Address2;
            City = address.City;
            State = stateAbbreviation;
            Zip = address.ZipPostalCode;
            Country = country;
            Phone = address.PhoneNumber;
            Email = address.Email;
        }
    }
}
