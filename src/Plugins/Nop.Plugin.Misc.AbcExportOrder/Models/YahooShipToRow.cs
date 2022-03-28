using System.Collections.Generic;

namespace Nop.Plugin.Misc.AbcExportOrder.Models
{
    public class YahooShipToRow
    {
        public string Id { get; protected set; }
        public string FullName { get; protected set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public string Address1 { get; protected set; }
        public string Address2 { get; protected set; }
        public string City { get; protected set; }
        public string State { get; protected set; }
        public string Zip { get; protected set; }
        public string Country { get; protected set; }
        public string Phone { get; protected set; }
        public string Email { get; protected set; }

        public YahooShipToRow(
            string prefix,
            int orderId
        )
        {
            Id = $"{prefix}{orderId}+p";
        }

        // converts to string values
        public List<string> ToStringValues()
        {
            return new List<string>()
            {
                Id,
                FullName,
                FirstName,
                LastName,
                Address1,
                Address2,
                City,
                State,
                Zip,
                Country,
                Phone,
                Email,
                "", // shipping (unused)
                "" // comments (unused)
            };
        }
    }
}