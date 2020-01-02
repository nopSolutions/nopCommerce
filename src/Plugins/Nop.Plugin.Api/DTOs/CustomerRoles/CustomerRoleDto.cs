using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;

namespace Nop.Plugin.Api.DTOs.CustomerRoles
{
    [JsonObject(Title = "customer_role")]
    public class CustomerRoleDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the customer role name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer role is marked as free shiping
        /// </summary>
        [JsonProperty("free_shipping")]
        public bool? FreeShipping { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer role is marked as tax exempt
        /// </summary>
        [JsonProperty("tax_exempt")]

        public bool? TaxExempt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer role is active
        /// </summary>
        [JsonProperty("active")]
        public bool? Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer role is system
        /// </summary>
        [JsonProperty("is_system_role")]
        public bool? IsSystemRole { get; set; }

        /// <summary>
        /// Gets or sets the customer role system name
        /// </summary>
        [JsonProperty("system_name")]
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customers must change passwords after a specified time
        /// </summary>
        [JsonProperty("enable_password_lifetime")]
        public bool? EnablePasswordLifetime { get; set; }

        /// <summary>
        /// Gets or sets a product identifier that is required by this customer role. 
        /// A customer is added to this customer role once a specified product is purchased.
        /// </summary>
        [JsonProperty("purchased_with_product_id")]
        public int? PurchasedWithProductId { get; set; }
    }
}
