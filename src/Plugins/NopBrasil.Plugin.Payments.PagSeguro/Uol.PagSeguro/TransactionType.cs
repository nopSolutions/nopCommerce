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
    /// Defines a list of known transaction types.
    /// </summary>
    /// <remarks>
    /// This class is not an enum to enable the introduction of new shipping types
    /// without breaking this version of the library.
    /// </remarks>
    public static class TransactionType
    {
        /// <summary>
        /// Payment
        /// </summary>
        public const int Payment = 1;

        /// <summary>
        /// Transfer
        /// </summary>
        public const int Transfer = 2;

        /// <summary>
        /// Fund addition
        /// </summary>
        public const int FundAddition = 3;

        /// <summary>
        /// Withdraw
        /// </summary>
        public const int Withdraw = 4;

        /// <summary>
        /// Charge
        /// </summary>
        public const int Charge = 5;

        /// <summary>
        /// Donation
        /// </summary>
        public const int Donation = 6;

        /// <summary>
        /// Bonus
        /// </summary>
        public const int Bonus = 7;

        /// <summary>
        /// Bonus repass
        /// </summary>
        public const int BonusRepass = 8;

        /// <summary>
        /// Operational
        /// </summary>
        public const int Operational = 9;

        /// <summary>
        /// Political donation
        /// </summary>
        public const int PoliticalDonation = 10;
    }
}
