//------------------------------------------------------------------------------
// Contributor(s): mb. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Routing;
using System.Web.Services.Protocols;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Shipping.Fedex.Domain;
using Nop.Plugin.Shipping.Fedex.RateServiceWebReference;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Services.Shipping;

namespace Nop.Plugin.Shipping.Fedex
{
    /// <summary>
    /// Fedex computation method
    /// </summary>
    public class FedexComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Constants

        private const int MAXPACKAGEWEIGHT = 150;
        private const string MEASUREWEIGHTSYSTEMKEYWORD = "lb";
        private const string MEASUREDIMENSIONSYSTEMKEYWORD = "inches";

        #endregion

        #region Fields

        private readonly IMeasureService _measureService;
        private readonly IShippingService _shippingService;
        private readonly ISettingService _settingService;
        private readonly FedexSettings _fedexSettings;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;

        #endregion

        #region Ctor
        public FedexComputationMethod(IMeasureService measureService,
            IShippingService shippingService, ISettingService settingService,
            FedexSettings fedexSettings, IOrderTotalCalculationService orderTotalCalculationService,
            ICurrencyService currencyService, CurrencySettings currencySettings)
        {
            this._measureService = measureService;
            this._shippingService = shippingService;
            this._settingService = settingService;
            this._fedexSettings = fedexSettings;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
        }
        #endregion

        #region Utilities

        private RateRequest CreateRateRequest(GetShippingOptionRequest getShippingOptionRequest)
        {
            // Build the RateRequest
            var request = new RateRequest();

            request.WebAuthenticationDetail = new WebAuthenticationDetail();
            request.WebAuthenticationDetail.UserCredential = new WebAuthenticationCredential();
            request.WebAuthenticationDetail.UserCredential.Key = _fedexSettings.Key;
            request.WebAuthenticationDetail.UserCredential.Password = _fedexSettings.Password;

            request.ClientDetail = new ClientDetail();
            request.ClientDetail.AccountNumber = _fedexSettings.AccountNumber;
            request.ClientDetail.MeterNumber = _fedexSettings.MeterNumber; 

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
            _orderTotalCalculationService.GetShoppingCartSubTotal(getShippingOptionRequest.Items,
                out orderSubTotalDiscountAmount, out orderSubTotalAppliedDiscount,
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
            subtotalBase = subTotalWithDiscountBase;
            SetShipmentDetails(request, getShippingOptionRequest, subtotalBase);
            SetOrigin(request);
            SetDestination(request, getShippingOptionRequest);
            SetPayment(request, getShippingOptionRequest);
            SetIndividualPackageLineItems(request, getShippingOptionRequest, subtotalBase);

            return request;
        }

        private void SetShipmentDetails(RateRequest request, GetShippingOptionRequest getShippingOptionRequest, decimal orderSubTotal)
        {
            request.RequestedShipment = new RequestedShipment();
            request.RequestedShipment.DropoffType = DropoffType.REGULAR_PICKUP; //Drop off types are BUSINESS_SERVICE_CENTER, DROP_BOX, REGULAR_PICKUP, REQUEST_COURIER, STATION
            request.RequestedShipment.TotalInsuredValue = new Money();
            request.RequestedShipment.TotalInsuredValue.Amount = orderSubTotal;
            request.RequestedShipment.TotalInsuredValue.Currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            request.RequestedShipment.ShipTimestamp = DateTime.Now; // Shipping date and time
            request.RequestedShipment.ShipTimestampSpecified = true;
            request.RequestedShipment.RateRequestTypes = new RateRequestType[2];
            request.RequestedShipment.RateRequestTypes[0] = RateRequestType.ACCOUNT;
            request.RequestedShipment.RateRequestTypes[1] = RateRequestType.LIST;
            request.RequestedShipment.PackageDetail = RequestedPackageDetailType.INDIVIDUAL_PACKAGES;
            request.RequestedShipment.PackageDetailSpecified = true;
        }

        private void SetPayment(RateRequest request, GetShippingOptionRequest getShippingOptionRequest)
        {
            request.RequestedShipment.ShippingChargesPayment = new Payment(); // Payment Information
            request.RequestedShipment.ShippingChargesPayment.PaymentType = PaymentType.SENDER; // Payment options are RECIPIENT, SENDER, THIRD_PARTY
            request.RequestedShipment.ShippingChargesPayment.PaymentTypeSpecified = true;
            request.RequestedShipment.ShippingChargesPayment.Payor = new Payor();
            request.RequestedShipment.ShippingChargesPayment.Payor.AccountNumber = _fedexSettings.AccountNumber;
        }

        private void SetDestination(RateRequest request, GetShippingOptionRequest getShippingOptionRequest)
        {
            request.RequestedShipment.Recipient = new Party();
            request.RequestedShipment.Recipient.Address = new Address();
            if (_fedexSettings.UseResidentialRates)
            {
                request.RequestedShipment.Recipient.Address.Residential = true;
                request.RequestedShipment.Recipient.Address.ResidentialSpecified = true;
            }
            request.RequestedShipment.Recipient.Address.StreetLines = new string[1] { "Recipient Address Line 1" };
            request.RequestedShipment.Recipient.Address.City = getShippingOptionRequest.ShippingAddress.City;
            if (getShippingOptionRequest.ShippingAddress.StateProvince != null && 
                IncludeStateProvidenceCode(getShippingOptionRequest.ShippingAddress.Country.TwoLetterIsoCode))
            {
                request.RequestedShipment.Recipient.Address.StateOrProvinceCode = getShippingOptionRequest.ShippingAddress.StateProvince.Abbreviation;
            }
            else
            {
                request.RequestedShipment.Recipient.Address.StateOrProvinceCode = string.Empty;
            }
            request.RequestedShipment.Recipient.Address.PostalCode = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
            request.RequestedShipment.Recipient.Address.CountryCode = getShippingOptionRequest.ShippingAddress.Country.TwoLetterIsoCode;
        }

        private void SetOrigin(RateRequest request)
        {
            request.RequestedShipment.Shipper = new Party();
            request.RequestedShipment.Shipper.Address = new Address();
            request.RequestedShipment.Shipper.Address.StreetLines = new string[1] { _fedexSettings.Street };
            request.RequestedShipment.Shipper.Address.City = _fedexSettings.City;            
            if (IncludeStateProvidenceCode(_fedexSettings.CountryCode))
            {
                request.RequestedShipment.Shipper.Address.StateOrProvinceCode = _fedexSettings.StateOrProvinceCode;
            }
            request.RequestedShipment.Shipper.Address.PostalCode = _fedexSettings.PostalCode;
            request.RequestedShipment.Shipper.Address.CountryCode = _fedexSettings.CountryCode;
        }

        private bool IncludeStateProvidenceCode(string countryCode)
        {
            return (countryCode.Equals("US", StringComparison.InvariantCultureIgnoreCase) || 
                    countryCode.Equals("CA", StringComparison.InvariantCultureIgnoreCase));
        }

        private void SetIndividualPackageLineItems(RateRequest request, GetShippingOptionRequest getShippingOptionRequest, decimal orderSubTotal)
        {
            // ------------------------------------------
            // Passing individual pieces rate request
            // ------------------------------------------

            var usedMeasureWeight = _measureService.GetMeasureWeightBySystemKeyword(MEASUREWEIGHTSYSTEMKEYWORD);
            if (usedMeasureWeight == null)
                throw new NopException(string.Format("FedEx shipping service. Could not load \"{0}\" measure weight", MEASUREWEIGHTSYSTEMKEYWORD));

            var usedMeasureDimension = _measureService.GetMeasureDimensionBySystemKeyword(MEASUREDIMENSIONSYSTEMKEYWORD);
            if (usedMeasureDimension == null)
                throw new NopException(string.Format("FedEx shipping service. Could not load \"{0}\" measure dimension", MEASUREDIMENSIONSYSTEMKEYWORD));

            int length = Convert.ToInt32(Math.Ceiling(_measureService.ConvertFromPrimaryMeasureDimension(getShippingOptionRequest.GetTotalLength(), usedMeasureDimension)));
            int height = Convert.ToInt32(Math.Ceiling(_measureService.ConvertFromPrimaryMeasureDimension(getShippingOptionRequest.GetTotalHeight(), usedMeasureDimension)));
            int width = Convert.ToInt32(Math.Ceiling(_measureService.ConvertFromPrimaryMeasureDimension(getShippingOptionRequest.GetTotalWidth(), usedMeasureDimension)));
            int weight = Convert.ToInt32(Math.Ceiling(_measureService.ConvertFromPrimaryMeasureWeight(_shippingService.GetShoppingCartTotalWeight(getShippingOptionRequest.Items), usedMeasureWeight)));
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
                request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue.Currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

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
                    request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue.Currency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
                }
            }
        }

        private List<ShippingOption> ParseResponse(RateReply reply)
        {
            var result = new List<ShippingOption>();

            Debug.WriteLine("RateReply details:");
            Debug.WriteLine("**********************************************************");
            foreach (var rateDetail in reply.RateReplyDetails)
            {
                var shippingOption = new ShippingOption();
                string serviceName = FedexServices.GetServiceName(rateDetail.ServiceType.ToString());

                // Skip the current service if services are selected and this service hasn't been selected
                if (!String.IsNullOrEmpty(_fedexSettings.CarrierServicesOffered) && !_fedexSettings.CarrierServicesOffered.Contains(rateDetail.ServiceType.ToString()))
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
                        if (_fedexSettings.ApplyDiscounts & shipmentDetail.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_ACCOUNT)
                        {
                            shippingOption.Rate = shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount + _fedexSettings.AdditionalHandlingCharge;
                            break;
                        }
                        else if (shipmentDetail.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_LIST) // Get List Rates (not discount rates)
                        {
                            shippingOption.Rate = shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount + _fedexSettings.AdditionalHandlingCharge;
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
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Represents a response of getting shipping rate options</returns>
        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException("getShippingOptionRequest");

            var response = new GetShippingOptionResponse();

            if (getShippingOptionRequest.Items == null)
            {
                response.AddError("No shipment items");
                return response;
            }

            if (getShippingOptionRequest.ShippingAddress == null)
            {
                response.AddError("Shipping address is not set");
                return response;
            }

            if (getShippingOptionRequest.ShippingAddress.Country == null)
            {
                response.AddError("Shipping country is not set");
                return response;
            }

            var request = CreateRateRequest(getShippingOptionRequest);
            var service = new RateService(); // Initialize the service
            service.Url = _fedexSettings.Url;
            try
            {
                // This is the call to the web service passing in a RateRequest and returning a RateReply
                var reply = service.getRates(request); // Service call

                if (reply.HighestSeverity == NotificationSeverityType.SUCCESS || reply.HighestSeverity == NotificationSeverityType.NOTE || reply.HighestSeverity == NotificationSeverityType.WARNING) // check if the call was successful
                {
                    if (reply != null && reply.RateReplyDetails != null)
                    {
                        var shippingOptions = ParseResponse(reply);
                        foreach (var shippingOption in shippingOptions)
                            response.ShippingOptions.Add(shippingOption);
                    }
                    else
                    {
                        if (reply != null &&
                            reply.Notifications != null &&
                            reply.Notifications.Length > 0 &&
                            !String.IsNullOrEmpty(reply.Notifications[0].Message))
                        {
                            response.AddError(string.Format("{0} (code: {1})", reply.Notifications[0].Message, reply.Notifications[0].Code));
                            return response;
                        }
                        else
                        {
                            response.AddError("Could not get reply from shipping server");
                            return response;
                        }
                    }
                }
                else
                {
                    Debug.WriteLine(reply.Notifications[0].Message);
                    response.AddError(reply.Notifications[0].Message);
                    return response;
                }
            }
            catch (SoapException e)
            {
                Debug.WriteLine(e.Detail.InnerText);
                response.AddError(e.Detail.InnerText);
                return response;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                response.AddError(e.Message);
                return response;
            }

            return response;
        }

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Fixed shipping rate; or null in case there's no fixed shipping rate</returns>
        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            return null;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ShippingFedex";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Shipping.Fedex.Controllers" }, { "area", null } };
        }
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            var settings = new FedexSettings()
            {
                Url = "https://gatewaybeta.fedex.com:443/web-services/rate",
                Street = "Sender Address Line 1",
                City = "Memphis",
                StateOrProvinceCode = "TN",
                PostalCode = "38115",
                CountryCode = "US"
            };
            _settingService.SaveSetting(settings);

            base.Install();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        public ShippingRateComputationMethodType ShippingRateComputationMethodType
        {
            get
            {
                return ShippingRateComputationMethodType.Realtime;
            }
        }

        #endregion
    }
}