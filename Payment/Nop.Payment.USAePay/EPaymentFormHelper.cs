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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.Common.Utils;
using System.Security.Cryptography;
using System.Collections.Specialized;
using NopSolutions.NopCommerce.Payment.Methods.USAePay.services;

namespace NopSolutions.NopCommerce.Payment.Methods.USAePay
{
    public class EPaymentFormHelper
    {
        #region Methods
        public static string CalcRequestSign(NameValueCollection reqParams)
        {
            string seed = CommonHelper.GenerateRandomDigitCode(5);
            string hash = CalcMd5Hash(String.Format("{0}:{1}:{2}:{3}:{4}", reqParams["UMcommand"], EPaymentFormSettings.PIN, reqParams["UMamount"], reqParams["UMinvoice"], seed));
            return String.Format("m/{0}/{1}/y", seed, hash);
        }

        public static bool ValidateResponseSign(NameValueCollection rspParams)
        {
            string rspHash = rspParams["UMresponseHash"];
            if(String.IsNullOrEmpty(rspHash))
            {
                return false;
            }
            string[] tmp = rspHash.Split(new char[] { '/' });
            if(tmp.Length != 3)
            {
                return false;
            }
            string seed = tmp[1];
            string hash = tmp[2];
            if(tmp[0].Equals("s"))
            {
                return hash.Equals(CalcSHA1Hash(String.Format("{0}:{1}:{2}:{3}", EPaymentFormSettings.PIN, rspParams["UMresult"], rspParams["UMrefNum"], seed)));
            }
            else if(tmp[0].Equals("m"))
            {
                return hash.Equals(CalcMd5Hash(String.Format("{0}:{1}:{2}:{3}", EPaymentFormSettings.PIN, rspParams["UMresult"], rspParams["UMrefNum"], seed)));
            }
            else
            {
                return false;
            }
        }

        internal static ueSecurityToken ServiceSecurityToken
        {
            get
            {
                ueSecurityToken token = new ueSecurityToken();
                token.SourceKey = EPaymentFormSettings.SourceKey;
                token.ClientIP = "127.0.0.1";
                token.PinHash = new ueHash();
                token.PinHash.Type = "md5";  // Type of encryption 
                token.PinHash.Seed = Guid.NewGuid().ToString();  // unique encryption seed
                token.PinHash.HashValue = CalcMd5Hash(String.Format("{0}{1}{2}", token.SourceKey, token.PinHash.Seed, EPaymentFormSettings.PIN)); // generate hash
                return token;
            }
        }
        #endregion

        #region Utilities
        private static string CalcMd5Hash(string s)
        {
            using(MD5 cs = MD5.Create())
            {
                StringBuilder sb = new StringBuilder();
                byte[] hash;

                hash = cs.ComputeHash(Encoding.UTF8.GetBytes(s));
                sb.Length = 0;
                foreach(byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        private static string CalcSHA1Hash(string s)
        {
            using(SHA1 cs = SHA1.Create())
            {
                StringBuilder sb = new StringBuilder();
                byte[] hash;

                hash = cs.ComputeHash(Encoding.UTF8.GetBytes(s));
                sb.Length = 0;
                foreach(byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
        #endregion
    }
}
