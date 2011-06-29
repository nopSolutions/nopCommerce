
using System;
using System.Runtime.Serialization;

namespace Nop.Plugin.Shipping.CanadaPost.Domain
{
    /// <summary>
    /// Information on a possible delivery rate
    /// </summary>
    [DataContract]
    public class DeliveryRate
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>The amount.</value>
        [DataMember]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the shipping date.
        /// </summary>
        /// <value>The shipping date.</value>
        [DataMember]
        public DateTime ShippingDate { get; set; }

        /// <summary>
        /// Gets or sets the delivery date.
        /// </summary>
        /// <value>The delivery date.</value>
        [DataMember]
        public string DeliveryDate { get; set; }

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        /// <value>The sequence.</value>
        [DataMember]
        public int Sequence { get; set; }

        #endregion
    }
}
