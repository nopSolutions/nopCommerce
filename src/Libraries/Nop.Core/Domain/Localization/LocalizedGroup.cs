namespace Nop.Core.Domain.Localization
{
    public class LocalizedGroup : BaseEntity
    {
        /// <summary>
        /// Gets or sets the locale key group
        /// </summary>
        public string LocaleKeyGroup { get; set; }
    }

    public class LocalizedLocalGroup : BaseEntity
    {
        /// <summary>
        /// Gets or sets the locale key
        /// </summary>
        public string LocaleKey { get; set; }


        /// <summary>
        /// Gets or sets the locale key group id
        /// </summary>

        public int LocaleKeyGroupId { get; set; }
    }
}
