using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.Square.Domain
{
    /// <summary>
    /// Represents access token grant type enumeration
    /// </summary>
    public enum GrantType
    {
        /// <summary>
        /// Obtain a new OAuth access token
        /// </summary>
        [EnumMember(Value = "authorization_code")]
        New,

        /// <summary>
        /// Refresh expired OAuth access token
        /// </summary>
        [EnumMember(Value = "refresh_token")]
        Refresh,

        /// <summary>
        /// Migrate from using legacy OAuth access token
        /// </summary>
        [EnumMember(Value = "migration_token")]
        Migration
    }
}