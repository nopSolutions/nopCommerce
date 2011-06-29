
using System.Runtime.Serialization;

namespace Nop.Plugin.Shipping.CanadaPost.Domain
{
    /// <summary>
    /// Contain the Box detail selected by the response
    /// </summary>
    [DataContract]
    public class BoxDetail
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>The weight.</value>
        [DataMember]
        public double Weight { get; set; }

        /// <summary>
        /// Gets or sets the exprediter weight.
        /// </summary>
        /// <value>The exprediter weight.</value>
        [DataMember]
        public double ExpediterWeight { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        [DataMember]
        public double Length { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        [DataMember]
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        [DataMember]
        public double Height { get; set; }
        
        /// <summary>
        /// Gets or sets the Quantity.
        /// </summary>
        /// <value>The quantity.</value>
        [DataMember]
        public int Quantity { get; set; }
        #endregion
    }
}
