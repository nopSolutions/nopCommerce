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
// Contributor(s): mb. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using Nop.Shipping.FedEx.RateServiceWebReference;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Shipping.Methods.FedEx
{
    /// <summary>
    /// FedEx computation method
    /// </summary>
    public class FedExComputationMethod : IShippingRateComputationMethod
    {
        #region Const

        private const int MAXPACKAGEWEIGHT = 150;
        private const string MEASUREWEIGHTSYSTEMKEYWORD = "lb";
        private const string MEASUREDIMENSIONSYSTEMKEYWORD = "inches";

        #endregion

        #region Utilities

        private RateRequest CreateRateRequest(ShipmentPackage ShipmentPackage)
        {
            // Build the RateRequest
            var request = new RateRequest();
            
            request.WebAuthenticationDetail = new WebAuthenticationDetail();
            request.WebAuthenticationDetail.UserCredential = new WebAuthenticationCredential();
            request.WebAuthenticationDetail.UserCredential.Key = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.Key"); // Replace "XXX" with the Key
            request.WebAuthenticationDetail.UserCredential.Password = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.Password"); // Replace "XXX" with the Password
            
            request.ClientDetail = new ClientDetail();
            request.ClientDetail.AccountNumber = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.AccountNumber"); // Replace "XXX" with client's account number
            request.ClientDetail.MeterNumber = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.MeterNumber"); // Replace "XXX" with client's meter number
            
            request.TransactionDetail = new TransactionDetail();
            request.TransactionDetail.CustomerTransactionId = "***Rate Available Services v7 Request - nopCommerce***"; // This is a reference field for the customer.  Any value can be used and will be provided in the response.

            request.Version = new VersionId(); // WSDL version information, value is automatically set from wsdl            

            request.ReturnTransitAndCommit = true;
            request.ReturnTransitAndCommitSpecified = true;
            request.CarrierCodes = new CarrierCodeType[2];
            // Insert the Carriers you would like to see the rates for
            request.CarrierCodes[0] = CarrierCodeType.FDXE;
            request.CarrierCodes[1] = CarrierCodeType.FDXG;

            decimal subtotalBase = decimal.Zero;
            decimal orderSubTotalDiscountAmount = decimal.Zero;
            Discount orderSubTotalAppliedDiscount = null;
            decimal subTotalWithoutDiscountBase = decimal.Zero;
            decimal subTotalWithDiscountBase = decimal.Zero;
            IoCFactory.Resolve<IShoppingCartManager>().GetShoppingCartSubTotal(ShipmentPackage.Items,
                ShipmentPackage.Customer, out orderSubTotalDiscountAmount, out orderSubTotalAppliedDiscount,
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
            subtotalBase = subTotalWithDiscountBase;
            SetShipmentDetails(request, ShipmentPackage, subtotalBase);
            SetOrigin(request);
            SetDestination(request, ShipmentPackage);
            SetPayment(request, ShipmentPackage);
            SetIndividualPackageLineItems(request, ShipmentPackage, subtotalBase);

            return request;
        }

        private void SetShipmentDetails(RateRequest request, ShipmentPackage ShipmentPackage, decimal orderSubTotal)
        {
            request.RequestedShipment = new RequestedShipment();
            request.RequestedShipment.DropoffType = DropoffType.REGULAR_PICKUP; //Drop off types are BUSINESS_SERVICE_CENTER, DROP_BOX, REGULAR_PICKUP, REQUEST_COURIER, STATION
            request.RequestedShipment.TotalInsuredValue = new Money();
            request.RequestedShipment.TotalInsuredValue.Amount = orderSubTotal;
            request.RequestedShipment.TotalInsuredValue.Currency = IoCFactory.Resolve<ICurrencyManager>().PrimaryStoreCurrency.CurrencyCode.ToString();
            request.RequestedShipment.ShipTimestamp = DateTime.Now; // Shipping date and time
            request.RequestedShipment.ShipTimestampSpecified = true;
            request.RequestedShipment.RateRequestTypes = new RateRequestType[2];
            request.RequestedShipment.RateRequestTypes[0] = RateRequestType.ACCOUNT;
            request.RequestedShipment.RateRequestTypes[1] = RateRequestType.LIST;
            request.RequestedShipment.PackageDetail = RequestedPackageDetailType.INDIVIDUAL_PACKAGES;
            request.RequestedShipment.PackageDetailSpecified = true;
        }

        private void SetPayment(RateRequest request, ShipmentPackage ShipmentPackage)
        {
            request.RequestedShipment.ShippingChargesPayment = new Payment(); // Payment Information
            request.RequestedShipment.ShippingChargesPayment.PaymentType = PaymentType.SENDER; // Payment options are RECIPIENT, SENDER, THIRD_PARTY
            request.RequestedShipment.ShippingChargesPayment.PaymentTypeSpecified = true;
            request.RequestedShipment.ShippingChargesPayment.Payor = new Payor();
            request.RequestedShipment.ShippingChargesPayment.Payor.AccountNumber = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.AccountNumber"); // Replace "XXX" with client's account number
        }

        private void SetDestination(RateRequest request, ShipmentPackage ShipmentPackage)
        {
            request.RequestedShipment.Recipient = new Party();
            request.RequestedShipment.Recipient.Address = new Address();
            if (IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("ShippingRateComputationMethod.FedEx.UseResidentialRates", false))
            {
                request.RequestedShipment.Recipient.Address.Residential = true;
                request.RequestedShipment.Recipient.Address.ResidentialSpecified = true;
            }
            request.RequestedShipment.Recipient.Address.StreetLines = new string[1] { "Recipient Address Line 1" };
            request.RequestedShipment.Recipient.Address.City = ShipmentPackage.ShippingAddress.City;
            if (ShipmentPackage.ShippingAddress.StateProvince != null)
            {
                request.RequestedShipment.Recipient.Address.StateOrProvinceCode = ShipmentPackage.ShippingAddress.StateProvince.Abbreviation;
            }
            else
            {
                request.RequestedShipment.Recipient.Address.StateOrProvinceCode = string.Empty;
            }
            request.RequestedShipment.Recipient.Address.PostalCode = ShipmentPackage.ShippingAddress.ZipPostalCode;
            request.RequestedShipment.Recipient.Address.CountryCode = ShipmentPackage.ShippingAddress.Country.TwoLetterIsoCode;
        }

        private void SetOrigin(RateRequest request)
        {
            request.RequestedShipment.Shipper = new Party();
            request.RequestedShipment.Shipper.Address = new Address();
            request.RequestedShipment.Shipper.Address.StreetLines = new string[1] { IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.ShippingOrigin.Street") };
            request.RequestedShipment.Shipper.Address.City = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.ShippingOrigin.City");
            request.RequestedShipment.Shipper.Address.StateOrProvinceCode = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.ShippingOrigin.StateOrProvinceCode");
            request.RequestedShipment.Shipper.Address.PostalCode = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.ShippingOrigin.PostalCode");
            request.RequestedShipment.Shipper.Address.CountryCode = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.ShippingOrigin.CountryCode");
        }

        private void SetIndividualPackageLineItems(RateRequest request, ShipmentPackage ShipmentPackage, decimal orderSubTotal)
        {
            // ------------------------------------------
            // Passing individual pieces rate request
            // ------------------------------------------

            var usedMeasureWeight = IoCFactory.Resolve<IMeasureManager>().GetMeasureWeightBySystemKeyword(MEASUREWEIGHTSYSTEMKEYWORD);
            if (usedMeasureWeight == null)
                throw new NopException(string.Format("FedEx shipping service. Could not load \"{0}\" measure weight", MEASUREWEIGHTSYSTEMKEYWORD));

            var usedMeasureDimension = IoCFactory.Resolve<IMeasureManager>().GetMeasureDimensionBySystemKeyword(MEASUREDIMENSIONSYSTEMKEYWORD);
            if (usedMeasureDimension == null)
                throw new NopException(string.Format("FedEx shipping service. Could not load \"{0}\" measure dimension", MEASUREDIMENSIONSYSTEMKEYWORD));

            int length = Convert.ToInt32(Math.Ceiling(IoCFactory.Resolve<IMeasureManager>().ConvertDimension(ShipmentPackage.GetTotalLength(), IoCFactory.Resolve<IMeasureManager>().BaseDimensionIn, usedMeasureDimension)));
            int height = Convert.ToInt32(Math.Ceiling(IoCFactory.Resolve<IMeasureManager>().ConvertDimension(ShipmentPackage.GetTotalHeight(), IoCFactory.Resolve<IMeasureManager>().BaseDimensionIn, usedMeasureDimension)));
            int width = Convert.ToInt32(Math.Ceiling(IoCFactory.Resolve<IMeasureManager>().ConvertDimension(ShipmentPackage.GetTotalWidth(), IoCFactory.Resolve<IMeasureManager>().BaseDimensionIn, usedMeasureDimension)));
            int weight = Convert.ToInt32(Math.Ceiling(IoCFactory.Resolve<IMeasureManager>().ConvertWeight(IoCFactory.Resolve<IShippingManager>().GetShoppingCartTotalWeight(ShipmentPackage.Items, ShipmentPackage.Customer), IoCFactory.Resolve<IMeasureManager>().BaseWeightIn, usedMeasureWeight)));
            if (length < 1)
                length = 1;
            if (height < 1)
                height = 1;
            if (width < 1)
                width = 1;
            if (weight < 1)
                weight = 1;

            if ((!IsPackageTooHeavy(weight)) && (!IsPackageTooLarge(length, height, width)))
            {
                request.RequestedShipment.PackageCount = "1";

                request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[1];
                request.RequestedShipment.RequestedPackageLineItems[0] = new RequestedPackageLineItem();
                request.RequestedShipment.RequestedPackageLineItems[0].SequenceNumber = "1"; // package sequence number            
                request.RequestedShipment.RequestedPackageLineItems[0].Weight = new Weight(); // package weight
                request.RequestedShipment.RequestedPackageLineItems[0].Weight.Units = WeightUnits.LB;
                request.RequestedShipment.RequestedPackageLineItems[0].Weight.Value = weight;
                request.RequestedShipment.RequestedPackageLineItems[0].Dimensions = new Dimensions(); // package dimensions

                //it's better to don't pass dims now
                request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Length = "0";
                request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Width = "0";
                request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Height = "0";
                request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Units = LinearUnits.IN;
                request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue = new Money(); // insured value
                request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue.Amount = orderSubTotal;
                request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue.Currency = IoCFactory.Resolve<ICurrencyManager>().PrimaryStoreCurrency.CurrencyCode.ToString();

            }
            else
            {
                int totalPackages = 1;
                int totalPackagesDims = 1;
                int totalPackagesWeights = 1;
                if (IsPackageTooHeavy(weight))
                {
                    totalPackagesWeights = Convert.ToInt32(Math.Ceiling((decimal)weight / (decimal)MAXPACKAGEWEIGHT));
                }
                if (IsPackageTooLarge(length, height, width))
                {
                    totalPackagesDims = Convert.ToInt32(Math.Ceiling((decimal)TotalPackageSize(length, height, width) / (decimal)108));
                }
                totalPackages = totalPackagesDims > totalPackagesWeights ? totalPackagesDims : totalPackagesWeights;
                if (totalPackages == 0)
                    totalPackages = 1;

                int weight2 = weight / totalPackages;
                int height2 = height / totalPackages;
                int width2 = width / totalPackages;
                int length2 = length / totalPackages;
                if (weight2 < 1)
                    weight2 = 1;
                if (height2 < 1)
                    height2 = 1;
                if (width2 < 1)
                    width2 = 1;
                if (length2 < 1)
                    length2 = 1;

                decimal orderSubTotal2 = orderSubTotal / totalPackages;

                request.RequestedShipment.PackageCount = totalPackages.ToString();
                request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[totalPackages];

                for (int i = 0; i < totalPackages; i++)
                {
                    request.RequestedShipment.RequestedPackageLineItems[i] = new RequestedPackageLineItem();
                    request.RequestedShipment.RequestedPackageLineItems[i].SequenceNumber = (i + 1).ToString(); // package sequence number            
                    request.RequestedShipment.RequestedPackageLineItems[i].Weight = new Weight(); // package weight
                    request.RequestedShipment.RequestedPackageLineItems[i].Weight.Units = WeightUnits.LB;
                    request.RequestedShipment.RequestedPackageLineItems[i].Weight.Value = (decimal)weight2;
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions = new Dimensions(); // package dimensions

                    //it's better to don't pass dims now
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Length = "0";
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Width = "0";
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Height = "0";
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Units = LinearUnits.IN;
                    request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue = new Money(); // insured value
                    request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue.Amount = orderSubTotal2;
                    request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue.Currency = IoCFactory.Resolve<ICurrencyManager>().PrimaryStoreCurrency.CurrencyCode.ToString();
                }
            }
        }

        private List<ShippingOption> ParseResponse(RateReply reply)
        {
            var additionalFee = IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("ShippingRateComputationMethod.FedEx.AdditionalFee", 0);
            string carrierServicesOffered = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.CarrierServicesOffered");
            bool applyDiscount = IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("ShippingRateComputationMethod.FedEx.ApplyDiscounts", false);

            var result = new List<ShippingOption>();

            Debug.WriteLine("RateReply details:");
            Debug.WriteLine("**********************************************************");
            foreach (var rateDetail in reply.RateReplyDetails)
            {
                var shippingOption = new ShippingOption();
                string serviceName = FedExServices.GetServiceName(rateDetail.ServiceType.ToString());

                // Skip the current service if services are selected and this service hasn't been selected
                if (!String.IsNullOrEmpty(carrierServicesOffered) && !carrierServicesOffered.Contains(rateDetail.ServiceType.ToString()))
                {
                    continue;
                }

                Debug.WriteLine("ServiceType: " + rateDetail.ServiceType);
                if (!serviceName.Equals("UNKNOWN"))
                {
                    shippingOption.Name = serviceName;

                    Debug.WriteLine("ServiceType: " + rateDetail.ServiceType);
                    foreach (RatedShipmentDetail shipmentDetail in rateDetail.RatedShipmentDetails)
                    {
                        Debug.WriteLine("RateType : " + shipmentDetail.ShipmentRateDetail.RateType);
                        Debug.WriteLine("Total Billing Weight : " + shipmentDetail.ShipmentRateDetail.TotalBillingWeight.Value);
                        Debug.WriteLine("Total Base Charge : " + shipmentDetail.ShipmentRateDetail.TotalBaseCharge.Amount);
                        Debug.WriteLine("Total Discount : " + shipmentDetail.ShipmentRateDetail.TotalFreightDiscounts.Amount);
                        Debug.WriteLine("Total Surcharges : " + shipmentDetail.ShipmentRateDetail.TotalSurcharges.Amount);
                        Debug.WriteLine("Net Charge : " + shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount);
                        Debug.WriteLine("*********");

                        // Get discounted rates if option is selected
                        if (applyDiscount == true & shipmentDetail.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_ACCOUNT)
                        {
                            shippingOption.Rate = shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount + additionalFee;
                            break;
                        }
                        else if (shipmentDetail.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_LIST) // Get List Rates (not discount rates)
                        {
                            shippingOption.Rate = shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount + additionalFee;
                            break;
                        }
                        else // Skip the rate (RATED_ACCOUNT, PAYOR_MULTIWEIGHT, or RATED_LIST)
                        {
                            continue;
                        }
                    }
                    result.Add(shippingOption);
                }
                Debug.WriteLine("**********************************************************");
            }

            return result;
        }

        private bool IsPackageTooLarge(int length, int height, int width)
        {
            int total = TotalPackageSize(length, height, width);
            if (total > 165)
                return true;
            else
                return false;
        }

        private int TotalPackageSize(int length, int height, int width)
        {
            int girth = height + height + width + width;
            int total = girth + length;
            return total;
        }

        private bool IsPackageTooHeavy(int weight)
        {
            if (weight > MAXPACKAGEWEIGHT)
                return true;
            else
                return false;
        }

        #endregion

        #region Methods
        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="shipmentPackage">Shipment package</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping options</returns>
        public List<ShippingOption> GetShippingOptions(ShipmentPackage shipmentPackage, ref string error)
        {
            var shippingOptions = new List<ShippingOption>();
            if (shipmentPackage == null)
                throw new ArgumentNullException("shipmentPackage");
            if (shipmentPackage.Items == null)
                throw new NopException("No shipment items");
            if (shipmentPackage.ShippingAddress == null)
            {
                error = "Shipping address is not set";
                return shippingOptions;
            }
            if (shipmentPackage.ShippingAddress.Country == null)
            {
                error = "Shipping country is not set";
                return shippingOptions;
            }

            RateRequest request = CreateRateRequest(shipmentPackage);
            RateService service = new RateService(); // Initialize the service
            service.Url = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.FedEx.URL");
            try
            {
                // This is the call to the web service passing in a RateRequest and returning a RateReply
                var reply = service.getRates(request); // Service call
                
                if (reply.HighestSeverity == NotificationSeverityType.SUCCESS || reply.HighestSeverity == NotificationSeverityType.NOTE || reply.HighestSeverity == NotificationSeverityType.WARNING) // check if the call was successful
                {
                    if (reply != null && reply.RateReplyDetails != null)
                    {
                        shippingOptions = ParseResponse(reply);
                    }
                    else
                    {
                        if (reply!=null && 
                            reply.Notifications != null && 
                            reply.Notifications.Length > 0 &&
                            !String.IsNullOrEmpty(reply.Notifications[0].Message))
                        {
                            error = string.Format("{0} (code: {1})", reply.Notifications[0].Message, reply.Notifications[0].Code);
                        }
                        else
                        {
                            error = "Could not get reply from shipping server";
                        }
                    }
                }
                else
                {
                    Debug.WriteLine(reply.Notifications[0].Message);
                    error = reply.Notifications[0].Message;
                }
            }
            catch (SoapException e)
            {
                Debug.WriteLine(e.Detail.InnerText);
                error = e.Detail.InnerText;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                error = e.Message;
            }

            return shippingOptions;
        }

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="shipmentPackage">Shipment package</param>
        /// <returns>Fixed shipping rate; or null if shipping rate could not be calculated before checkout</returns>
        public decimal? GetFixedRate(ShipmentPackage shipmentPackage)
        {
            return null;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        /// <returns>A shipping rate computation method type</returns>
        public ShippingRateComputationMethodTypeEnum ShippingRateComputationMethodType
        {
            get
            {
                return ShippingRateComputationMethodTypeEnum.Realtime;
            }
        }

        #endregion
    }
}
