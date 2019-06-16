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
    /// Defines a list of known transaction statuses.
    /// </summary>
    /// <remarks>
    /// This class is not an enum to enable the introduction of new shipping types
    /// without breaking this version of the library.
    /// </remarks>
    public static class TransactionStatus
    {
        /// <summary>
        /// Initiated
        /// </summary>
        public const int Initiated = 0;

        /// <summary>
        /// Waiting payment
        /// </summary>
        public const int WaitingPayment = 1;

        /// <summary>
        /// In analysis
        /// </summary>
        public const int InAnalysis = 2;

        /// <summary>
        /// Paid
        /// </summary>
        public const int Paid = 3;

        /// <summary>
        /// Available
        /// </summary>
        public const int Available = 4;

        /// <summary>
        /// In dispute
        /// </summary>
        public const int InDispute = 5;

        /// <summary>
        /// Refunded
        /// </summary>
        public const int Refunded = 6;

        /// <summary>
        /// Cancelled
        /// </summary>
        public const int Cancelled = 7;
    }
}
