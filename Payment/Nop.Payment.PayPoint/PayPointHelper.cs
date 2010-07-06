using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.Common.Utils;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace NopSolutions.NopCommerce.Payment.Methods.PayPoint
{
    public class PayPointHelper
    {
        #region Methods
        public static string CalcRequestSign(RemotePost post)
        {
            return CalcMD5Hash(String.Format("{0}{1}{2}", post.Params["trans_id"], post.Params["amount"], HostedPaymentSettings.RemotePassword));
        }

        public static bool ValidateResponseSign(Uri requestUrl)
        {
            string paq = requestUrl.PathAndQuery;
            Match m = Regex.Match(requestUrl.PathAndQuery, @"^.*\&hash=(?<hash>.*)$");
            if(!m.Success)
            {
                return false;
            }
            string hash = m.Groups["hash"].Value;
            paq = Regex.Replace(paq, @"hash=.*$", String.Empty);
            string hash2 = CalcMD5Hash(paq + HostedPaymentSettings.DigestKey);

            return hash.Equals(hash2);
        }
        #endregion

        #region Utilities
        private static string CalcMD5Hash(string s)
        {
            using(MD5 cs = MD5.Create())
            {
                StringBuilder sb = new StringBuilder();
                byte[] hash;

                hash = cs.ComputeHash(Encoding.UTF8.GetBytes(s));
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
