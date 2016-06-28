//------------------------------------------------------------------------------
// Contributor(s): mb, New York. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

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
        private readonly ILogger _logger;

        #endregion

        #region Ctor
        public FedexComputationMethod(IMeasureService measureService,
            IShippingService shippingService, ISettingService settingService,
            FedexSettings fedexSettings, IOrderTotalCalculationService orderTotalCalculationService,
            ICurrencyService currencyService, CurrencySettings currencySettings,
            ILogger logger)
        {
            this._measureService = measureService;
            this._shippingService = shippingService;
            this._settingService = settingService;
            this._fedexSettings = fedexSettings;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._logger = logger;
        }
        #endregion

        #region Utilities

        private RateRequest CreateRateRequest(GetShippingOptionRequest getShippingOptionRequest, out Currency requestedShipmentCurrency)
        {
            // Build the RateRequest
            var request = new RateRequest();

            request.WebAuthenticationDetail = new RateServiceWebReference.WebAuthenticationDetail();
            request.WebAuthenticationDetail.UserCredential = new RateServiceWebReference.WebAuthenticationCredential();
            request.WebAuthenticationDetail.UserCredential.Key = _fedexSettings.Key;
            request.WebAuthenticationDetail.UserCredential.Password = _fedexSettings.Password;

            request.ClientDetail = new RateServiceWebReference.ClientDetail();
            request.ClientDetail.AccountNumber = _fedexSettings.AccountNumber;
            request.ClientDetail.MeterNumber = _fedexSettings.MeterNumber;

            request.TransactionDetail = new RateServiceWebReference.TransactionDetail();
            request.TransactionDetail.CustomerTransactionId = "***Rate Available Services v16 Request - nopCommerce***"; // This is a reference field for the customer.  Any value can be used and will be provided in the response.

            request.Version = new RateServiceWebReference.VersionId(); // WSDL version information, value is automatically set from wsdl            

            request.ReturnTransitAndCommit = true;
            request.ReturnTransitAndCommitSpecified = true;
            request.CarrierCodes = new RateServiceWebReference.CarrierCodeType[2];
            // Insert the Carriers you would like to see the rates for
            request.CarrierCodes[0] = RateServiceWebReference.CarrierCodeType.FDXE;
            request.CarrierCodes[1] = RateServiceWebReference.CarrierCodeType.FDXG;

            decimal orderSubTotalDiscountAmount;
            List<Discount> orderSubTotalAppliedDiscounts;
            decimal subTotalWithoutDiscountBase;
            decimal subTotalWithDiscountBase;
            //TODO we should use getShippingOptionRequest.Items.GetQuantity() method to get subtotal
            _orderTotalCalculationService.GetShoppingCartSubTotal(getShippingOptionRequest.Items.Select(x=>x.ShoppingCartItem).ToList(),
                false, out orderSubTotalDiscountAmount, out orderSubTotalAppliedDiscounts,
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
            decimal subTotalBase = subTotalWithDiscountBase;

            request.RequestedShipment = new RequestedShipment();

            SetOrigin(request, getShippingOptionRequest);
            SetDestination(request, getShippingOptionRequest);

            requestedShipmentCurrency = GetRequestedShipmentCurrency(
                request.RequestedShipment.Shipper.Address.CountryCode,    // origin
                request.RequestedShipment.Recipient.Address.CountryCode); // destination

            decimal subTotalShipmentCurrency;
            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (requestedShipmentCurrency.CurrencyCode == primaryStoreCurrency.CurrencyCode)
                subTotalShipmentCurrency = subTotalBase;
            else
                subTotalShipmentCurrency = _currencyService.ConvertFromPrimaryStoreCurrency(subTotalBase, requestedShipmentCurrency);

            Debug.WriteLine("SubTotal (Primary Currency) : {0} ({1})", subTotalBase, primaryStoreCurrency.CurrencyCode);
            Debug.WriteLine("SubTotal (Shipment Currency): {0} ({1})", subTotalShipmentCurrency, requestedShipmentCurrency.CurrencyCode);

            SetShipmentDetails(request, subTotalShipmentCurrency, requestedShipmentCurrency.CurrencyCode);
            SetPayment(request);

            switch (_fedexSettings.PackingType)
            {
                case PackingType.PackByOneItemPerPackage:
                    SetIndividualPackageLineItemsOneItemPerPackage(request, getShippingOptionRequest,  requestedShipmentCurrency.CurrencyCode);
                    break;
                case PackingType.PackByVolume:
                    SetIndividualPackageLineItemsCubicRootDimensions(request, getShippingOptionRequest, subTotalShipmentCurrency, requestedShipmentCurrency.CurrencyCode);
                    break;
                case PackingType.PackByDimensions:
                default:
                    SetIndividualPackageLineItems(request, getShippingOptionRequest, subTotalShipmentCurrency, requestedShipmentCurrency.CurrencyCode);
                    break;
            }
            return request;
        }

        private void SetShipmentDetails(RateRequest request, decimal orderSubTotal, string currencyCode)
        {
            //set drop off type
            switch (_fedexSettings.DropoffType)
            {
                case DropoffType.BusinessServiceCenter:
                    request.RequestedShipment.DropoffType = RateServiceWebReference.DropoffType.BUSINESS_SERVICE_CENTER;
                    break;
                case DropoffType.DropBox:
                    request.RequestedShipment.DropoffType = RateServiceWebReference.DropoffType.DROP_BOX;
                    break;
                case DropoffType.RegularPickup:
                    request.RequestedShipment.DropoffType = RateServiceWebReference.DropoffType.REGULAR_PICKUP;
                    break;
                case DropoffType.RequestCourier:
                    request.RequestedShipment.DropoffType = RateServiceWebReference.DropoffType.REQUEST_COURIER;
                    break;
                case DropoffType.Station:
                    request.RequestedShipment.DropoffType = RateServiceWebReference.DropoffType.STATION;
                    break;
                default:
                    request.RequestedShipment.DropoffType = RateServiceWebReference.DropoffType.BUSINESS_SERVICE_CENTER;
                    break;
            }
            request.RequestedShipment.TotalInsuredValue = new Money();
            request.RequestedShipment.TotalInsuredValue.Amount = orderSubTotal;
            request.RequestedShipment.TotalInsuredValue.Currency = currencyCode;


            //Saturday pickup is available for certain FedEx Express U.S. service types:
            //http://www.fedex.com/us/developer/product/WebServices/MyWebHelp/Services/Options/c_SaturdayShipAndDeliveryServiceDetails.html
            //If the customer orders on a Saturday, the rate calculation will use Saturday as the shipping date, and the rates will include a Saturday pickup surcharge
            //More info: http://www.nopcommerce.com/boards/t/27348/fedex-rate-can-be-excessive-for-express-methods-if-calculated-on-a-saturday.aspx
            var shipTimestamp = DateTime.Now;
            if (shipTimestamp.DayOfWeek == DayOfWeek.Saturday)
                shipTimestamp = shipTimestamp.AddDays(2);
            request.RequestedShipment.ShipTimestamp = shipTimestamp; // Shipping date and time
            request.RequestedShipment.ShipTimestampSpecified = true;
            request.RequestedShipment.RateRequestTypes = new RateRequestType[2];
            request.RequestedShipment.RateRequestTypes[0] = RateRequestType.PREFERRED;
            request.RequestedShipment.RateRequestTypes[1] = RateRequestType.LIST;
            //request.RequestedShipment.PackageDetail = RequestedPackageDetailType.INDIVIDUAL_PACKAGES;
            //request.RequestedShipment.PackageDetailSpecified = true;
        }

        private void SetPayment(RateRequest request)
        {
            request.RequestedShipment.ShippingChargesPayment = new Payment(); // Payment Information
            request.RequestedShipment.ShippingChargesPayment.PaymentType = PaymentType.SENDER; // Payment options are RECIPIENT, SENDER, THIRD_PARTY
            request.RequestedShipment.ShippingChargesPayment.PaymentTypeSpecified = true;
            request.RequestedShipment.ShippingChargesPayment.Payor = new Payor();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty = new Party();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.AccountNumber = _fedexSettings.AccountNumber;
        }

        private void SetDestination(RateRequest request, GetShippingOptionRequest getShippingOptionRequest)
        {
            request.RequestedShipment.Recipient = new Party();
            request.RequestedShipment.Recipient.Address = new RateServiceWebReference.Address();
            if (_fedexSettings.UseResidentialRates)
            {
                request.RequestedShipment.Recipient.Address.Residential = true;
                request.RequestedShipment.Recipient.Address.ResidentialSpecified = true;
            }
            request.RequestedShipment.Recipient.Address.StreetLines = new [] { getShippingOptionRequest.ShippingAddress.Address1 };
            request.RequestedShipment.Recipient.Address.City = getShippingOptionRequest.ShippingAddress.City;
            if (getShippingOptionRequest.ShippingAddress.StateProvince != null &&
                IncludeStateProvinceCode(getShippingOptionRequest.ShippingAddress.Country.TwoLetterIsoCode))
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

        private void SetOrigin(RateRequest request, GetShippingOptionRequest getShippingOptionRequest)
        {
            request.RequestedShipment.Shipper = new Party();
            request.RequestedShipment.Shipper.Address = new RateServiceWebReference.Address();

            if (getShippingOptionRequest.CountryFrom == null)
                throw new Exception("FROM country is not specified");

            request.RequestedShipment.Shipper.Address.StreetLines = new [] { getShippingOptionRequest.AddressFrom };
            request.RequestedShipment.Shipper.Address.City = getShippingOptionRequest.CityFrom;
            if (IncludeStateProvinceCode(getShippingOptionRequest.CountryFrom.TwoLetterIsoCode))
            {
                string stateProvinceAbbreviation = getShippingOptionRequest.StateProvinceFrom == null ? "" : getShippingOptionRequest.StateProvinceFrom.Abbreviation;
                request.RequestedShipment.Shipper.Address.StateOrProvinceCode = stateProvinceAbbreviation;
            }
            request.RequestedShipment.Shipper.Address.PostalCode = getShippingOptionRequest.ZipPostalCodeFrom;
            request.RequestedShipment.Shipper.Address.CountryCode = getShippingOptionRequest.CountryFrom.TwoLetterIsoCode;
        }

        private bool IncludeStateProvinceCode(string countryCode)
        {
            return (countryCode.Equals("US", StringComparison.InvariantCultureIgnoreCase) || 
                    countryCode.Equals("CA", StringComparison.InvariantCultureIgnoreCase));
        }

        private void SetIndividualPackageLineItems(RateRequest request, GetShippingOptionRequest getShippingOptionRequest, decimal orderSubTotal, string currencyCode)
        {
            // Rate request setup - Total Dimensions of Shopping Cart Items determines number of packages

            var usedMeasureWeight = GetUsedMeasureWeight();
            var usedMeasureDimension = GetUsedMeasureDimension();

            decimal lengthTmp, widthTmp, heightTmp;
            _shippingService.GetDimensions(getShippingOptionRequest.Items, out widthTmp, out lengthTmp, out heightTmp);

            int length = ConvertFromPrimaryMeasureDimension(lengthTmp, usedMeasureDimension);
            int height = ConvertFromPrimaryMeasureDimension(heightTmp, usedMeasureDimension);
            int width = ConvertFromPrimaryMeasureDimension(widthTmp, usedMeasureDimension);
            int weight = ConvertFromPrimaryMeasureWeight(_shippingService.GetTotalWeight(getShippingOptionRequest), usedMeasureWeight);
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
                request.RequestedShipment.RequestedPackageLineItems[0].GroupPackageCount = "1";
                request.RequestedShipment.RequestedPackageLineItems[0].Weight = new RateServiceWebReference.Weight(); // package weight
                request.RequestedShipment.RequestedPackageLineItems[0].Weight.Units = RateServiceWebReference.WeightUnits.LB;
                request.RequestedShipment.RequestedPackageLineItems[0].Weight.UnitsSpecified = true;
                request.RequestedShipment.RequestedPackageLineItems[0].Weight.Value = weight;
                request.RequestedShipment.RequestedPackageLineItems[0].Weight.ValueSpecified = true;

                request.RequestedShipment.RequestedPackageLineItems[0].Dimensions = new RateServiceWebReference.Dimensions(); // package dimensions
                request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Length = _fedexSettings.PassDimensions ? length.ToString() : "0";
                request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Width = _fedexSettings.PassDimensions ? width.ToString() : "0";
                request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Height = _fedexSettings.PassDimensions ? height.ToString() : "0";
                request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Units = RateServiceWebReference.LinearUnits.IN;
                request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.UnitsSpecified = true;
                request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue = new Money(); // insured value
                request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue.Amount = orderSubTotal;
                request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue.Currency = currencyCode;

            }
            else
            {
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
                var totalPackages = totalPackagesDims > totalPackagesWeights ? totalPackagesDims : totalPackagesWeights;
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
                    request.RequestedShipment.RequestedPackageLineItems[i].GroupPackageCount = "1";
                    request.RequestedShipment.RequestedPackageLineItems[i].Weight = new RateServiceWebReference.Weight(); // package weight
                    request.RequestedShipment.RequestedPackageLineItems[i].Weight.Units = RateServiceWebReference.WeightUnits.LB;
                    request.RequestedShipment.RequestedPackageLineItems[i].Weight.UnitsSpecified = true;
                    request.RequestedShipment.RequestedPackageLineItems[i].Weight.Value = weight2;
                    request.RequestedShipment.RequestedPackageLineItems[i].Weight.ValueSpecified = true;

                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions = new RateServiceWebReference.Dimensions(); // package dimensions
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Length = _fedexSettings.PassDimensions ? length2.ToString() : "0";
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Width = _fedexSettings.PassDimensions ? width2.ToString() : "0";
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Height = _fedexSettings.PassDimensions ? height2.ToString() : "0";
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Units = RateServiceWebReference.LinearUnits.IN;
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.UnitsSpecified = true;
                    request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue = new Money(); // insured value
                    request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue.Amount = orderSubTotal2;
                    request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue.Currency = currencyCode;
                }
            }
        }

        private void SetIndividualPackageLineItemsOneItemPerPackage(RateRequest request, GetShippingOptionRequest getShippingOptionRequest, string currencyCode)
        {
            // Rate request setup - each Shopping Cart Item is a separate package

            var usedMeasureWeight = GetUsedMeasureWeight();
            var usedMeasureDimension = GetUsedMeasureDimension();

            var items = getShippingOptionRequest.Items;
            var totalItems = items.Sum(x => x.GetQuantity());
            request.RequestedShipment.PackageCount = totalItems.ToString();
            request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[totalItems];

            int i = 0;
            foreach (var packageItem in items)
            {
                var sci = packageItem.ShoppingCartItem;
                var qty = packageItem.GetQuantity();

                //get dimensions for qty 1
                decimal lengthTmp, widthTmp, heightTmp;
                _shippingService.GetDimensions(new List<GetShippingOptionRequest.PackageItem>
                                               {
                                                   new GetShippingOptionRequest.PackageItem(sci, 1)
                                               }, out widthTmp, out lengthTmp, out heightTmp);

                int length = ConvertFromPrimaryMeasureDimension(lengthTmp, usedMeasureDimension);
                int height = ConvertFromPrimaryMeasureDimension(heightTmp, usedMeasureDimension);
                int width = ConvertFromPrimaryMeasureDimension(widthTmp, usedMeasureDimension);
                int weight = ConvertFromPrimaryMeasureWeight(sci.Product.Weight, usedMeasureWeight);
                if (length < 1)
                    length = 1;
                if (height < 1)
                    height = 1;
                if (width < 1)
                    width = 1;
                if (weight < 1)
                    weight = 1;

                for (int j = 0; j < qty; j++)
                {
                    request.RequestedShipment.RequestedPackageLineItems[i] = new RequestedPackageLineItem();
                    request.RequestedShipment.RequestedPackageLineItems[i].SequenceNumber = (i + 1).ToString(); // package sequence number                     
                    request.RequestedShipment.RequestedPackageLineItems[i].GroupPackageCount = "1";
                    request.RequestedShipment.RequestedPackageLineItems[i].Weight = new RateServiceWebReference.Weight(); // package weight
                    request.RequestedShipment.RequestedPackageLineItems[i].Weight.Units = RateServiceWebReference.WeightUnits.LB;
                    request.RequestedShipment.RequestedPackageLineItems[i].Weight.UnitsSpecified = true;
                    request.RequestedShipment.RequestedPackageLineItems[i].Weight.Value = weight;
                    request.RequestedShipment.RequestedPackageLineItems[i].Weight.ValueSpecified = true;

                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions = new RateServiceWebReference.Dimensions(); // package dimensions
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Length = length.ToString();
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Height = height.ToString();
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Width = width.ToString();
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Units = RateServiceWebReference.LinearUnits.IN;
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.UnitsSpecified = true;

                    request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue = new Money(); // insured value
                    request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue.Amount = sci.Product.Price;
                    request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue.Currency = currencyCode;

                    i++;
                }
            }

        }

        private void SetIndividualPackageLineItemsCubicRootDimensions(RateRequest request, GetShippingOptionRequest getShippingOptionRequest, decimal orderSubTotal, string currencyCode)
        {
            // Rate request setup - Total Volume of Shopping Cart Items determines number of packages

            //From FedEx Guide (Ground):
            //Dimensional weight is based on volume (the amount of space a package
            //occupies in relation to its actual weight). If the cubic size of your FedEx
            //Ground package measures three cubic feet (5,184 cubic inches or 84,951
            //cubic centimetres) or greater, you will be charged the greater of the
            //dimensional weight or the actual weight.
            //A package weighing 150 lbs. (68 kg) or less and measuring greater than
            //130 inches (330 cm) in combined length and girth will be classified by
            //FedEx Ground as an “Oversize” package. All packages must have a
            //combined length and girth of no more than 165 inches (419 cm). An
            //oversize charge of $30 per package will also apply to any package
            //measuring greater than 130 inches (330 cm) in combined length and
            //girth.
            //Shipping charges for packages smaller than three cubic feet are based
            //on actual weight

            // Dimensional Weight applies to packages with volume 5,184 cubic inches or more
            // cube root(5184) = 17.3

            // Packages that exceed 130 inches in length and girth (2xHeight + 2xWidth) 
            // are considered “oversize” packages.
            // Assume a cube (H=W=L) of that size: 130 = D + (2xD + 2xD) = 5xD :  D = 130/5 = 26
            // 26x26x26 = 17,576
            // Avoid oversize by using 25"
            // 25x25x25 = 15,625

            // Which is less $  - multiple small pakages, or one large package using dimensional weight
            //  15,625 / 5184 = 3.014 =  3 packages  
            // Ground for total weight:             60lbs     15lbs
            //  3 packages 17x17x17 (20 lbs each) = $66.21    39.39
            //  1 package  25x25x25 (60 lbs)      = $71.70    71.70


            var usedMeasureWeight = GetUsedMeasureWeight();
            var usedMeasureDimension = GetUsedMeasureDimension();

            int totalPackagesDims;
            int length;
            int height;
            int width;

            if (getShippingOptionRequest.Items.Count == 1 && getShippingOptionRequest.Items[0].GetQuantity() == 1)
            {
                var sci = getShippingOptionRequest.Items[0].ShoppingCartItem;

                //get dimensions for qty 1
                decimal lengthTmp, widthTmp, heightTmp;
                _shippingService.GetDimensions(new List<GetShippingOptionRequest.PackageItem>
                                               {
                                                   new GetShippingOptionRequest.PackageItem(sci, 1)
                                               }, out widthTmp, out lengthTmp, out heightTmp);

                totalPackagesDims = 1;
                length = ConvertFromPrimaryMeasureDimension(lengthTmp, usedMeasureDimension);
                height = ConvertFromPrimaryMeasureDimension(lengthTmp, usedMeasureDimension);
                width = ConvertFromPrimaryMeasureDimension(widthTmp, usedMeasureDimension);
            }
            else
            {
                decimal totalVolume = 0;
                foreach (var item in getShippingOptionRequest.Items)
                {
                    var sci = item.ShoppingCartItem;

                    //get dimensions for qty 1
                    decimal lengthTmp, widthTmp, heightTmp;
                    _shippingService.GetDimensions(new List<GetShippingOptionRequest.PackageItem>
                                               {
                                                   new GetShippingOptionRequest.PackageItem(sci, 1)
                                               }, out widthTmp, out lengthTmp, out heightTmp);

                    int productLength = ConvertFromPrimaryMeasureDimension(lengthTmp, usedMeasureDimension);
                    int productHeight = ConvertFromPrimaryMeasureDimension(heightTmp, usedMeasureDimension);
                    int productWidth = ConvertFromPrimaryMeasureDimension(widthTmp, usedMeasureDimension);
                    totalVolume += item.GetQuantity() * (productHeight * productWidth * productLength);
                }

                int dimension;
                if (totalVolume == 0)
                {
                    dimension = 0;
                    totalPackagesDims = 1;
                }
                else
                {
                    // cubic inches
                    int packageVolume = _fedexSettings.PackingPackageVolume;
                    if (packageVolume <= 0)
                        packageVolume = 5184;

                    // cube root (floor)
                    dimension = Convert.ToInt32(Math.Floor(Math.Pow(Convert.ToDouble(packageVolume), (double)(1.0 / 3.0))));
                    if (IsPackageTooLarge(dimension, dimension, dimension))
                        throw new NopException("fedexSettings.PackingPackageVolume exceeds max package size");

                    // adjust packageVolume for dimensions calculated
                    packageVolume = dimension * dimension * dimension;

                    totalPackagesDims = Convert.ToInt32(Math.Ceiling(totalVolume / packageVolume));
                }

                length = width = height = dimension;
            }
            if (length < 1)
                length = 1;
            if (height < 1)
                height = 1;
            if (width < 1)
                width = 1;

            int weight = ConvertFromPrimaryMeasureWeight(_shippingService.GetTotalWeight(getShippingOptionRequest), usedMeasureWeight);
            if (weight < 1)
                weight = 1;

            int totalPackagesWeights = 1;
            if (IsPackageTooHeavy(weight))
            {
                totalPackagesWeights = Convert.ToInt32(Math.Ceiling((decimal)weight / (decimal)MAXPACKAGEWEIGHT));
            }

            int totalPackages = totalPackagesDims > totalPackagesWeights ? totalPackagesDims : totalPackagesWeights;

            decimal orderSubTotalPerPackage = orderSubTotal / totalPackages;
            int weightPerPackage = weight / totalPackages;

            request.RequestedShipment.PackageCount = totalPackages.ToString();
            request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[totalPackages];

            for (int i = 0; i < totalPackages; i++)
            {
                request.RequestedShipment.RequestedPackageLineItems[i] = new RequestedPackageLineItem();
                request.RequestedShipment.RequestedPackageLineItems[i].SequenceNumber = (i + 1).ToString(); // package sequence number          
                request.RequestedShipment.RequestedPackageLineItems[i].GroupPackageCount = "1";           
                request.RequestedShipment.RequestedPackageLineItems[i].Weight = new RateServiceWebReference.Weight(); // package weight
                request.RequestedShipment.RequestedPackageLineItems[i].Weight.Units = RateServiceWebReference.WeightUnits.LB;
                request.RequestedShipment.RequestedPackageLineItems[i].Weight.UnitsSpecified = true;
                request.RequestedShipment.RequestedPackageLineItems[i].Weight.Value = weightPerPackage;
                request.RequestedShipment.RequestedPackageLineItems[i].Weight.ValueSpecified = true;

                request.RequestedShipment.RequestedPackageLineItems[i].Dimensions = new RateServiceWebReference.Dimensions(); // package dimensions
                request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Length = length.ToString();
                request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Height = height.ToString();
                request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Width = width.ToString();
                request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Units = RateServiceWebReference.LinearUnits.IN;
                request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.UnitsSpecified = true;
                request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue = new Money(); // insured value
                request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue.Amount = orderSubTotalPerPackage;
                request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue.Currency = currencyCode;
            }

        }

        private IEnumerable<ShippingOption> ParseResponse(RateReply reply, Currency requestedShipmentCurrency)
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

                    foreach (RatedShipmentDetail shipmentDetail in rateDetail.RatedShipmentDetails)
                    {
                        Debug.WriteLine("RateType : " + shipmentDetail.ShipmentRateDetail.RateType);
                        Debug.WriteLine("Total Billing Weight : " + shipmentDetail.ShipmentRateDetail.TotalBillingWeight.Value);
                        Debug.WriteLine("Total Base Charge : " + shipmentDetail.ShipmentRateDetail.TotalBaseCharge.Amount);
                        Debug.WriteLine("Total Discount : " + shipmentDetail.ShipmentRateDetail.TotalFreightDiscounts.Amount);
                        Debug.WriteLine("Total Surcharges : " + shipmentDetail.ShipmentRateDetail.TotalSurcharges.Amount);
                        Debug.WriteLine("Net Charge : " + shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount + "(" + shipmentDetail.ShipmentRateDetail.TotalNetCharge.Currency + ")");
                        Debug.WriteLine("*********");

                        // Get discounted rates if option is selected
                        if (_fedexSettings.ApplyDiscounts &
                            (shipmentDetail.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_ACCOUNT_PACKAGE ||
                            shipmentDetail.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_ACCOUNT_SHIPMENT))
                        {
                            decimal amount = ConvertChargeToPrimaryCurrency(shipmentDetail.ShipmentRateDetail.TotalNetCharge, requestedShipmentCurrency);
                            shippingOption.Rate = amount + _fedexSettings.AdditionalHandlingCharge;
                            break;
                        }
                        else if (shipmentDetail.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_LIST_PACKAGE ||
                            shipmentDetail.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_LIST_SHIPMENT) // Get List Rates (not discount rates)
                        {
                            decimal amount = ConvertChargeToPrimaryCurrency(shipmentDetail.ShipmentRateDetail.TotalNetCharge, requestedShipmentCurrency);
                            shippingOption.Rate = amount + _fedexSettings.AdditionalHandlingCharge;
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

        private Decimal ConvertChargeToPrimaryCurrency(Money charge, Currency requestedShipmentCurrency)
        {
            decimal amount;
            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (primaryStoreCurrency.CurrencyCode.Equals(charge.Currency, StringComparison.InvariantCultureIgnoreCase))
            {
                amount = charge.Amount;
            }
            else
            {
                Currency amountCurrency;
                if (charge.Currency == requestedShipmentCurrency.CurrencyCode)
                    amountCurrency = requestedShipmentCurrency;
                else
                    amountCurrency = _currencyService.GetCurrencyByCode(charge.Currency);

                //ensure the the currency exists; otherwise, presume that it was primary store currency
                if (amountCurrency == null)
                    amountCurrency = primaryStoreCurrency;

                amount = _currencyService.ConvertToPrimaryStoreCurrency(charge.Amount, amountCurrency);

                Debug.WriteLine("ConvertChargeToPrimaryCurrency - from {0} ({1}) to {2} ({3})",
                    charge.Amount, charge.Currency, amount, primaryStoreCurrency.CurrencyCode);
            }

            return amount;
        }
        
        private bool IsPackageTooLarge(int length, int height, int width)
        {
            int total = TotalPackageSize(length, height, width);
            if (total > 165)
                return true;
            
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
            return weight > MAXPACKAGEWEIGHT;
        }

        private MeasureWeight GetUsedMeasureWeight()
        {
            var usedMeasureWeight = _measureService.GetMeasureWeightBySystemKeyword(MEASUREWEIGHTSYSTEMKEYWORD);
            if (usedMeasureWeight == null)
                throw new NopException("FedEx shipping service. Could not load \"{0}\" measure weight", MEASUREWEIGHTSYSTEMKEYWORD);
            return usedMeasureWeight;
        }

        private MeasureDimension GetUsedMeasureDimension()
        {
            var usedMeasureDimension = _measureService.GetMeasureDimensionBySystemKeyword(MEASUREDIMENSIONSYSTEMKEYWORD);
            if (usedMeasureDimension == null)
                throw new NopException("FedEx shipping service. Could not load \"{0}\" measure dimension", MEASUREDIMENSIONSYSTEMKEYWORD);

            return usedMeasureDimension;
        }

        private int ConvertFromPrimaryMeasureDimension(decimal quantity, MeasureDimension usedMeasureDimension)
        {
            return Convert.ToInt32(Math.Ceiling(_measureService.ConvertFromPrimaryMeasureDimension(quantity, usedMeasureDimension)));
        }

        private int ConvertFromPrimaryMeasureWeight(decimal quantity, MeasureWeight usedMeasureWeight)
        {
            return Convert.ToInt32(Math.Ceiling(_measureService.ConvertFromPrimaryMeasureWeight(quantity, usedMeasureWeight)));
        }
        
        private Currency GetRequestedShipmentCurrency(string originCountryCode, string destinCountryCode)
        {
            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);

            //The solution coded here might be considered a bit of a hack
            //it only supports the scenario for US / Canada shipping
            //because nopCommerce does not have a concept of a designated currency for a Country.
            string originCurrencyCode;
            if (originCountryCode == "US")
                originCurrencyCode = "USD";
            else if (originCountryCode == "CA")
                originCurrencyCode = "CAD";
            else
                originCurrencyCode = primaryStoreCurrency.CurrencyCode;

            string destinCurrencyCode;
            if (destinCountryCode == "US")
                destinCurrencyCode = "USD";
            else if (destinCountryCode == "CA")
                destinCurrencyCode = "CAD";
            else
                destinCurrencyCode = primaryStoreCurrency.CurrencyCode;
            
            //when neither the shipping origin's currency or the destinations currency is the same as the store primary currency,
            //FedEx would complain that "There are no valid services available. (code: 556)".
            if (originCurrencyCode == primaryStoreCurrency.CurrencyCode || destinCurrencyCode == primaryStoreCurrency.CurrencyCode)
            {
                return primaryStoreCurrency;
            }
            
            //ensure that this currency exists
            return _currencyService.GetCurrencyByCode(originCurrencyCode) ?? primaryStoreCurrency;
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

            Currency requestedShipmentCurrency;
            var request = CreateRateRequest(getShippingOptionRequest, out requestedShipmentCurrency);
            var service = new RateService(); // Initialize the service
            service.Url = _fedexSettings.Url;
            try
            {
                // This is the call to the web service passing in a RateRequest and returning a RateReply
                var reply = service.getRates(request); // Service call

                if (reply.HighestSeverity == RateServiceWebReference.NotificationSeverityType.SUCCESS || 
                    reply.HighestSeverity == RateServiceWebReference.NotificationSeverityType.NOTE || 
                    reply.HighestSeverity == RateServiceWebReference.NotificationSeverityType.WARNING) // check if the call was successful
                {
                    if (reply.RateReplyDetails != null)
                    {
                        var shippingOptions = ParseResponse(reply, requestedShipmentCurrency);
                        foreach (var shippingOption in shippingOptions)
                            response.ShippingOptions.Add(shippingOption);
                    }
                    else
                    {
                        if (reply.Notifications != null &&
                            reply.Notifications.Length > 0 &&
                            !String.IsNullOrEmpty(reply.Notifications[0].Message))
                        {
                            response.AddError(string.Format("{0} (code: {1})", reply.Notifications[0].Message, reply.Notifications[0].Code));
                            return response;
                        }

                        response.AddError("Could not get reply from shipping server");
                        return response;
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
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Shipping.Fedex.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new FedexSettings
            {
                Url = "https://gatewaybeta.fedex.com:443/web-services/rate",
                DropoffType = DropoffType.BusinessServiceCenter,
                PackingPackageVolume = 5184
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.Url", "URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.Url.Hint", "Specify FedEx URL.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.Key", "Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.Key.Hint", "Specify FedEx key.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.Password", "Password");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.Password.Hint", "Specify FedEx password.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.AccountNumber", "Account number");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.AccountNumber.Hint", "Specify FedEx account number.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.MeterNumber", "Meter number");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.MeterNumber.Hint", "Specify FedEx meter number.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.UseResidentialRates", "Use residential rates");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.UseResidentialRates.Hint", "Check to use residential rates.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.ApplyDiscounts", "Use discounted rates");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.ApplyDiscounts.Hint", "Check to use discounted rates (instead of list rates).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.AdditionalHandlingCharge", "Additional handling charge");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.AdditionalHandlingCharge.Hint", "Enter additional handling fee to charge your customers.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.CarrierServices", "Carrier Services Offered");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.CarrierServices.Hint", "Select the services you want to offer to customers.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.PassDimensions", "Pass dimensions");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.PassDimensions.Hint", "Check if you want to pass package dimensions when requesting rates.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.PackingType", "Packing type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.PackingType.Hint", "Choose preferred packing type.");
            this.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Shipping.Fedex.PackingType.PackByDimensions", "Pack by dimensions");
            this.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Shipping.Fedex.PackingType.PackByOneItemPerPackage", "Pack by one item per package");
            this.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Shipping.Fedex.PackingType.PackByVolume", "Pack by volume");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.PackingPackageVolume", "Package volume");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.PackingPackageVolume.Hint", "Enter your package volume.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.DropoffType", "Dropoff Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.Fedex.Fields.DropoffType.Hint", "Choose preferred dropoff type.");
            this.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Shipping.Fedex.DropoffType.BusinessServiceCenter", "Business service center");
            this.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Shipping.Fedex.DropoffType.DropBox", "Drop box");
            this.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Shipping.Fedex.DropoffType.RegularPickup", "Regular pickup");
            this.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Shipping.Fedex.DropoffType.RequestCourier", "Request courier");
            this.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Shipping.Fedex.DropoffType.Station", "Station");
            
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<FedexSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.Url");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.Url.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.Key");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.Key.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.Password");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.Password.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.AccountNumber");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.AccountNumber.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.MeterNumber");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.MeterNumber.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.UseResidentialRates");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.UseResidentialRates.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.ApplyDiscounts");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.ApplyDiscounts.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.AdditionalHandlingCharge");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.AdditionalHandlingCharge.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.CarrierServices");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.CarrierServices.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.PassDimensions");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.PassDimensions.Hint");
            this.DeletePluginLocaleResource("Enums.Nop.Plugin.Shipping.Fedex.PackingType.PackByDimensions");
            this.DeletePluginLocaleResource("Enums.Nop.Plugin.Shipping.Fedex.PackingType.PackByOneItemPerPackage");
            this.DeletePluginLocaleResource("Enums.Nop.Plugin.Shipping.Fedex.PackingType.PackByVolume");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.PackingType");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.PackingType.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.PackingPackageVolume");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.PackingPackageVolume.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.DropoffType");
            this.DeletePluginLocaleResource("Plugins.Shipping.Fedex.Fields.DropoffType.Hint");
            this.DeletePluginLocaleResource("Enums.Nop.Plugin.Shipping.Fedex.DropoffType.BusinessServiceCenter");
            this.DeletePluginLocaleResource("Enums.Nop.Plugin.Shipping.Fedex.DropoffType.DropBox");
            this.DeletePluginLocaleResource("Enums.Nop.Plugin.Shipping.Fedex.DropoffType.RegularPickup");
            this.DeletePluginLocaleResource("Enums.Nop.Plugin.Shipping.Fedex.DropoffType.RequestCourier");
            this.DeletePluginLocaleResource("Enums.Nop.Plugin.Shipping.Fedex.DropoffType.Station");

            base.Uninstall();
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

        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        public IShipmentTracker ShipmentTracker
        {
            get { return new FedexShipmentTracker(_logger, _fedexSettings); }
        }

        #endregion
    }
}