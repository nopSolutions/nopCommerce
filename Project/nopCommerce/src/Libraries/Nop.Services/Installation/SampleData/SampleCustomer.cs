using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Installation.SampleData;

/// <summary>
/// Represents a sample customer
/// </summary>
public partial class SampleCustomer : SampleAddress
{
    /// <summary>
    /// Gets or sets a value indicating whether the customer is active
    /// </summary>
    public bool Active { get; set; } = true;

    /// <summary>
    /// Gets or sets customers role system names
    /// </summary>
    public List<string> CustomerRoleSystemNames { get; set; } = new();

    /// <summary>
    /// Gets or sets the password
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the password format
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public PasswordFormat PasswordFormat { get; set; }

    /// <summary>
    /// Gets or sets the password salt
    /// </summary>
    public string PasswordSalt { get; set; } = string.Empty;
}
