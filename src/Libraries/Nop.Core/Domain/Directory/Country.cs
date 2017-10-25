using System.Collections.Generic;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Directory
{
    /// <summary>
    /// 国家信息
    /// Represents a country
    /// </summary>
    public partial class Country : BaseEntity, ILocalizedEntity, IStoreMappingSupported
    {
        private ICollection<StateProvince> _stateProvinces;
        private ICollection<ShippingMethod> _restrictedShippingMethods;


        /// <summary>
        /// 名称
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否允许开发票
        /// Gets or sets a value indicating whether billing is allowed to this country
        /// </summary>
        public bool AllowsBilling { get; set; }

        /// <summary>
        /// 是否允许海运
        /// Gets or sets a value indicating whether shipping is allowed to this country
        /// </summary>
        public bool AllowsShipping { get; set; }

        /// <summary>
        /// 第二信件编号
        /// Gets or sets the two letter ISO code
        /// </summary>
        public string TwoLetterIsoCode { get; set; }

        /// <summary>
        /// 第三信件编号
        /// Gets or sets the three letter ISO code
        /// </summary>
        public string ThreeLetterIsoCode { get; set; }

        /// <summary>
        /// 数字编号
        /// Gets or sets the numeric ISO code
        /// </summary>
        public int NumericIsoCode { get; set; }

        /// <summary>
        /// 是否收取增值税
        /// Gets or sets a value indicating whether customers in this country must be charged EU VAT
        /// </summary>
        public bool SubjectToVat { get; set; }

        /// <summary>
        /// 是否已发布
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// 排序次序
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public bool LimitedToStores { get; set; }
       
        /// <summary>
        /// Gets or sets the state/provinces
        /// </summary>
        public virtual ICollection<StateProvince> StateProvinces
        {
            get { return _stateProvinces ?? (_stateProvinces = new List<StateProvince>()); }
            protected set { _stateProvinces = value; }
        }

        /// <summary>
        /// Gets or sets the restricted shipping methods
        /// </summary>
        public virtual ICollection<ShippingMethod> RestrictedShippingMethods
        {
            get { return _restrictedShippingMethods ?? (_restrictedShippingMethods = new List<ShippingMethod>()); }
            protected set { _restrictedShippingMethods = value; }
        }
    }

}
