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
    /// Defines a list of known payment method types.
    /// </summary>
    /// <remarks>
    /// This class is not an enum to enable the introduction of new shipping types
    /// without breaking this version of the library.
    /// </remarks>
    public static class PaymentMethodType
    {
        /// <summary>
        /// Credit card
        /// </summary>
        public const int CreditCard = 1;

        /// <summary>
        /// Boleto is a form of invoicing in Brazil
        /// </summary>
        public const int Boleto = 2;

        /// <summary>
        /// Online transfer
        /// </summary>
        public const int OnlineTransfer = 3;

        /// <summary>
        /// PagSeguro account balance
        /// </summary>
        public const int Balance = 4;

        /// <summary>
        /// OiPaggo
        /// </summary>
        public const int OiPaggo = 5;
    }
}
