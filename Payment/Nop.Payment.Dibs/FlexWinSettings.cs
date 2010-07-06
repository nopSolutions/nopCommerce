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

using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;

namespace NopSolutions.NopCommerce.Payment.Methods.Dibs
{
    /// <summary>
    /// FlexWin payment method settings
    /// </summary>
    public class FlexWinSettings : DibsSettings
    {
        #region Properties
        /// <summary>
        /// Gets or sets the color theme
        /// </summary>
        public static string ColorTheme
        {
            get
            {
                return SettingManager.GetSettingValue("PaymentMethod.Dibs.FlexWin.ColorTheme", "blue");
            }
            set
            {
                SettingManager.SetParam("PaymentMethod.Dibs.FlexWin.ColorTheme", value);
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the test mode is enabled
        /// </summary>
        public static bool UseSandbox
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("PaymentMethod.Dibs.UseSandbox", true);
            }
            set
            {
                SettingManager.SetParam("PaymentMethod.Dibs.UseSandbox", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the decorator
        /// </summary>
        public static string Decorator
        {
            get
            {
                return SettingManager.GetSettingValue("PaymentMethod.Dibs.FlexWin.Decorator", "default");
            }
            set
            {
                SettingManager.SetParam("PaymentMethod.Dibs.FlexWin.Decorator", value);
            }
        }

        /// <summary>
        /// Gets or sets the payment gateway URL
        /// </summary>
        public static string GatewayUrl
        {
            get
            {
                return SettingManager.GetSettingValue("PaymentMethod.Dibs.FlexWin.GatewayUrl", "https://payment.architrade.com/paymentweb/start.action");
            }
            set
            {
                SettingManager.SetParam("PaymentMethod.Dibs.FlexWin.GatewayUrl", value);
            }
        }

        /// <summary>
        /// Gets or sets a first MD5 key
        /// </summary>
        public static string MD5Key1
        {
            get
            {
                return SettingManager.GetSettingValue("PaymentMethod.Dibs.MD5Key1");
            }
            set
            {
                SettingManager.SetParam("PaymentMethod.Dibs.MD5Key1", value);
            }
        }

        /// <summary>
        /// Gets or sets a second MD5 key
        /// </summary>
        public static string MD5Key2
        {
            get
            {
                return SettingManager.GetSettingValue("PaymentMethod.Dibs.MD5Key2");
            }
            set
            {
                SettingManager.SetParam("PaymentMethod.Dibs.MD5Key2", value);
            }
        }

        public static decimal AdditionalFee
        {
            get
            {
                return SettingManager.GetSettingValueDecimalNative("PaymentMethod.Dibs.AdditionalFee");
            }
            set
            {
                SettingManager.SetParamNative("PaymentMethod.Dibs.AdditionalFee", value);
            }
        }
        #endregion
    }
}
