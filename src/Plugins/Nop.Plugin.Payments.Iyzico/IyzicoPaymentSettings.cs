using Nop.Core.Configuration;
using System.Collections.Generic;

namespace Nop.Plugin.Payments.Iyzico
{
    /// <summary>
    /// Represents settings of iyzico payment plugin
    /// </summary>
    public class IyzicoPaymentSettings : ISettings
    {
        /// <summary>
        /// 
        /// </summary>
        public bool UseToPaymentPopup { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ApiSecret { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private string _ApiUrl;

        public string ApiUrl
        {
            get { return string.IsNullOrEmpty(_ApiUrl) ? "https://api.iyzipay.com" : _ApiUrl; }
            set { _ApiUrl = value; }
        }

        /// <summary>
        /// Maskelenmiþ kart bilgilerinin yönetici tarafýndaki sipariþ detayýnda görüntülenmesini saðlar
        /// </summary>
        public bool IsCardStorage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private string _PaymentSuccessUrl;

        public string PaymentSuccessUrl
        {
            get { return string.IsNullOrEmpty(_PaymentSuccessUrl) ? "/checkout/completed/" : _PaymentSuccessUrl; }
            set { _PaymentSuccessUrl = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _PaymentErrorUrl;

        public string PaymentErrorUrl
        {
            get { return string.IsNullOrEmpty(_PaymentErrorUrl) ? "/onepagecheckout/" : _PaymentErrorUrl; }
            set { _PaymentErrorUrl = value; }
        }

        public bool InstallmentNumber2 { get; set; }

        public bool InstallmentNumber3 { get; set; }

        public bool InstallmentNumber6 { get; set; }

        public bool InstallmentNumber9 { get; set; }

        public bool InstallmentNumber12 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<int> InstallmentNumbers
        {
            get
            {
                List<int> r = new();

                if (InstallmentNumber2)
                {
                    r.Add(2);
                }

                if (InstallmentNumber3)
                {
                    r.Add(3);
                }

                if (InstallmentNumber6)
                {
                    r.Add(6);
                }
                if (InstallmentNumber9)
                {
                    r.Add(9);
                }

                if (InstallmentNumber12)
                {
                    r.Add(12);
                }

                return r;
            }
            set { }
        }
    }
}
