// Copyright [2011] [PagSeguro Internet Ltda.]
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Uol.PagSeguro
{
    /// <summary>
    /// Shipping information
    /// </summary>
    public class Shipping
    {
        /// <summary>
        /// Initializes a new instance of the Shipping class
        /// </summary>
        public Shipping()
        {
        }

        /// <summary>
        /// Shipping address
        /// </summary>
        public Address Address
        {
            get;
            set;
        }

        /// <summary>
        /// Shipping type. See the ShippingType helper class for a list of known shipping
        /// types.
        /// </summary>
        public int? ShippingType
        {
            get;
            set;
        }

        /// <summary>
        /// Total shipping cost. This is a read-only property and it is calculated by PagSeguro 
        /// based on the shipping information provided with the payment request.
        /// </summary>
        public decimal? Cost
        {
            get;
            internal set;
        }
    }
}
