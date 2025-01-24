using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Installation.JsonModels;

internal class CustomerJsonModel
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public string FaxNumber { get; set; } = string.Empty;

    public string Company { get; set; }

    public string Address1 { get; set; }

    public string Address2 { get; set; }

    public string City { get; set; }

    public string StateProvince { get; set; }

    public string CountryThreeLetterIsoCode { get; set; }

    public string ZipPostalCode { get; set; }

    public bool Active { get; set; } = true;

    public List<string> CustomerRoleSystemNames { get; set; } = new();

    public string Password { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public PasswordFormat PasswordFormat { get; set; } = PasswordFormat.Clear;

    public string PasswordSalt { get; set; } = string.Empty;
}
