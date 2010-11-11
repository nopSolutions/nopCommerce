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

using System.Globalization;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Shipping.Methods.AustraliaPost
{
    public class AustraliaPostSettings
    {
        #region Properties
        public static string GatewayUrl
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.AustraliaPost.GatewayUrl", "http://drc.edeliver.com.au/ratecalc.asp");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.AustraliaPost.GatewayUrl", value);
            }
        }

        public static string ShippedFromZipPostalCode
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.AustraliaPost.DefaultShippedFromZipPostalCode");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.AustraliaPost.DefaultShippedFromZipPostalCode", value);
            }
        }

        public static decimal AdditionalHandlingCharge
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("ShippingRateComputationMethod.AustraliaPost.AdditionalHandlingCharge");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("ShippingRateComputationMethod.AustraliaPost.AdditionalHandlingCharge", value.ToString(new CultureInfo("en-US")));
            }
        }

        public static MeasureWeight MeasureWeight
        {
            get
            {
                return IoC.Resolve<IMeasureService>().GetMeasureWeightBySystemKeyword("grams");
            }
        }

        public static MeasureDimension MeasureDimension
        {
            get
            {
                return IoC.Resolve<IMeasureService>().GetMeasureDimensionBySystemKeyword("millimetres");
            }
        }
        #endregion
    }
}
