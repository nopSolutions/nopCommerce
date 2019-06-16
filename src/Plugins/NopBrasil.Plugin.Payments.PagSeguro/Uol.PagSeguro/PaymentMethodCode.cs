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
    /// Defines a list of known payment method codes.
    /// </summary>
    /// <remarks>
    /// This class is not an enum to enable the introduction of new shipping types
    /// without breaking this version of the library.
    /// </remarks>
    public static class PaymentMethodCode
    {
        /// <summary>
        /// VISA
        /// </summary>
        public const int Visa = 101;

        /// <summary>
        /// Mastercard
        /// </summary>
        public const int Mastercard = 102;

        /// <summary>
        /// American Express
        /// </summary>
        public const int Amex = 103;

        /// <summary>
        /// Diners
        /// </summary>
        public const int Diners = 104;

        /// <summary>
        /// Hipercard
        /// </summary>
        public const int Hipercard = 105;

        /// <summary>
        /// Aura
        /// </summary>
        public const int Aura = 106;

        /// <summary>
        /// Elo
        /// </summary>
        public const int Elo = 107;

        /// <summary>
        /// Bradesco boleto
        /// </summary>
        public const int BradescoBoleto = 201;

        /// <summary>
        /// Santander boleto
        /// </summary>
        public const int SantanderBoleto = 202;

        /// <summary>
        /// Bradesco online transfer
        /// </summary>
        public const int BradescoOnlineTransfer = 301;

        /// <summary>
        /// Itau online transfer
        /// </summary>
        public const int ItauOnlineTransfer = 302;

        /// <summary>
        /// Unibanco online transfer
        /// </summary>
        public const int UnibancoOnlineTransfer = 303;

        /// <summary>
        /// Banco do Brasil online transfer
        /// </summary>
        public const int BancoBrasilOnlineTransfer = 304;

        /// <summary>
        /// Banco Real online transfer
        /// </summary>
        public const int RealOnlineTransfer = 305;

        /// <summary>
        /// Banrisul online transfer
        /// </summary>
        public const int BanrisulOnlineTransfer = 306;

        /// <summary>
        /// PagSeguro account balance
        /// </summary>
        public const int PagSeguroBalance = 401;

        /// <summary>
        /// OiPaggo
        /// </summary>
        public const int OiPaggo = 501;
    }
}
