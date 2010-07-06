//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): 
//------------------------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.Text;

namespace NopSolutions.NopCommerce.Payment.Methods.Dibs
{
    /// <summary>
    /// FlexWin payment method utilities
    /// </summary>
    public class FlexWinHelper
    {
        /// <summary>
        /// Calculates MD5 hash
        /// </summary>
        /// <param name="merachantID">Merchant ID</param>
        /// <param name="orderID">Order ID</param>
        /// <param name="currency">Currency number</param>
        /// <param name="amount">Amount</param>
        /// <returns>MD5 hash string</returns>
        public static string CalcMD5Key(int merachantID, int orderID, int currency, int amount)
        {
            using(MD5 cs = MD5.Create())
            {
                StringBuilder sb = new StringBuilder();
                byte[] hash;

                hash = cs.ComputeHash(Encoding.ASCII.GetBytes(FlexWinSettings.MD5Key1 + String.Format("merchant={0}&orderid={1}&currency={2}&amount={3}", merachantID, orderID, currency, amount)));
                foreach(byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                hash = cs.ComputeHash(Encoding.ASCII.GetBytes(FlexWinSettings.MD5Key2 + sb.ToString()));
                sb.Length = 0;
                foreach(byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Calculates auth key
        /// </summary>
        /// <param name="transact">Transact number</param>
        /// <param name="amount">Amount</param>
        /// <param name="currency">Currency number</param>
        /// <returns>Auth key</returns>
        public static string CalcAuthKey(int transact, int amount, int currency)
        {
            using(MD5 cs = MD5.Create())
            {
                StringBuilder sb = new StringBuilder();
                byte[] hash;

                hash = cs.ComputeHash(Encoding.ASCII.GetBytes(FlexWinSettings.MD5Key1 + String.Format("transact={0}&amount={1}&currency={2}", transact, amount, currency)));
                foreach(byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                hash = cs.ComputeHash(Encoding.ASCII.GetBytes(FlexWinSettings.MD5Key2 + sb.ToString()));
                sb.Length = 0;
                foreach(byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
