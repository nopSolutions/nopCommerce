using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nop.Plugin.Shipping.CanadaPost.Domain
{
    /// <summary>
    /// Contain the result of a shipping request
    /// </summary>
    [DataContract]
    public class RequestResult
    {
        #region Fields
        private readonly List<DeliveryRate> _rates;
        private readonly List<BoxDetail> _boxes;
        #endregion

        #region Constructor

        public RequestResult()
        {
            this._rates = new List<DeliveryRate>();
            this._boxes = new List<BoxDetail>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>The status code.</value>
        [DataMember]
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        /// <value>The status message.</value>
        [DataMember]
        public string StatusMessage { get; set; }

        /// <summary>
        /// Gets or sets the available rates.
        /// </summary>
        /// <value>The available rates.</value>
        [DataMember]
        public List<DeliveryRate> AvailableRates
        {
            get
            {
                return _rates;
            }
        }

        /// <summary>
        /// Gets the boxes.
        /// </summary>
        /// <value>The boxes.</value>
        [DataMember]
        public List<BoxDetail> Boxes
        {
            get
            {
                return _boxes;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether it's error
        /// </summary>
        public bool IsError { get; set; }

        #endregion
    }
}
