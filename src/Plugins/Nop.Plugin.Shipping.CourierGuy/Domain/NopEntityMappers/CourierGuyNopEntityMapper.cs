using Nop.Core.Domain.Shipping;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Shipping;

namespace Nop.Plugin.Shipping.CourierGuy.Domain.NopEntityMappers;

public class CourierGuyNopEntityMapper(
    IAddressService addressService,
    IStateProvinceService stateProvinceService,
    ICountryService countryService,
    IShippingService shippingService,
    IWarehouseService warehouseService) : ICourierGuyNopEntityMapper
{
    public async Task<List<CourierGuyRateRequest>> NopCourierGuyRateRequest(
        GetShippingOptionRequest getShippingOptionRequest)
    {
        var shipTo = getShippingOptionRequest.ShippingAddress;
        if (shipTo == null || string.IsNullOrWhiteSpace(shipTo.Address1))
        {
            return new List<CourierGuyRateRequest>();
        }

        var shipToStateProvince =
            await stateProvinceService.GetStateProvinceByIdAsync(shipTo.StateProvinceId.GetValueOrDefault());
        var shipToCountry = await countryService.GetCountryByIdAsync(shipTo.CountryId.GetValueOrDefault());
        var totalInsurableValue = getShippingOptionRequest.Items.Sum(x => x.Product.Price);

        var parcels = getShippingOptionRequest.Items.Select(x =>
        {
            var parcel = new CourierGuyRateRequest.Parcel()
            {
                SubmittedLengthCm = x.Product.Length,
                SubmittedWidthCm = x.Product.Width,
                SubmittedHeightCm = x.Product.Height,
                SubmittedWeightKg = x.Product.Weight
            };
            var parcelWarehouse =
                warehouseService.GetWarehouseByIdAsync(x.Product.WarehouseId).GetAwaiter().GetResult();
            var parcelShipFrom = addressService.GetAddressByIdAsync(parcelWarehouse.AddressId).GetAwaiter().GetResult();
            return (parcel, parcelShipFrom);
        }).ToList();

        var deliveryAddress = new CourierGuyRateRequest.Address()
        {
            Type =
                string.IsNullOrEmpty(shipTo.Company)
                    ? CourierGuyRateRequest.AddressType.business
                    : CourierGuyRateRequest.AddressType.residential,
            Company = shipTo.Company,
            StreetAddress = shipTo.Address1,
            LocalArea = shipTo.Address2,
            City = shipTo.City,
            Zone = shipToStateProvince?.Name,
            Country = shipToCountry?.TwoLetterIsoCode,
            Code = shipTo.ZipPostalCode,
        };

        var courierRequests = parcels.Select(x =>
        {
            var groupedGoods = parcels.Where(filter => filter.parcelShipFrom.Id == x.parcelShipFrom.Id).ToList();
            var parcelShipFrom = x.parcelShipFrom;
            var stateProvince = stateProvinceService
                .GetStateProvinceByIdAsync(parcelShipFrom.StateProvinceId.GetValueOrDefault()).GetAwaiter().GetResult();
            var country = countryService.GetCountryByIdAsync(parcelShipFrom.CountryId.GetValueOrDefault()).GetAwaiter()
                .GetResult();

            return new CourierGuyRateRequest()
            {
                DeliveryAddress = deliveryAddress,
                CollectionAddress = new CourierGuyRateRequest.Address()
                {
                    Type = CourierGuyRateRequest.AddressType.business,
                    Company = parcelShipFrom.Company,
                    StreetAddress = parcelShipFrom.Address1,
                    LocalArea = parcelShipFrom.Address2,
                    City = parcelShipFrom.City,
                    Zone = stateProvince?.Name,
                    Country = country?.TwoLetterIsoCode,
                    Code = parcelShipFrom.ZipPostalCode,
                },
                Parcels = groupedGoods.Select(good => good.parcel).ToList(),
                // Only add for insurance for goods under 10k
                DeclaredValue = 0,
                CollectionMinDate = DateTime.Today.AddDays(1),
                DeliveryMinDate = DateTime.Today.AddDays(1)
            };
        }).ToList();
        return courierRequests;
    }

    public async Task<GetShippingOptionResponse> MapToShippingOptionResponse(
        List<CourierGuyRateResponse> responseContent)
    {
        var courierGuyRates = responseContent
            .Select(x => x.Rates)
            .ToList();

        var economyOptions = courierGuyRates
            .SelectMany(rateList =>
            {
                var cheapest = rateList.OrderByDescending(rate => rate.ShippingOptionRate).LastOrDefault();
                return rateList.Where(rate => rate.ServiceLevel.Equals(cheapest?.ServiceLevel));
            }).ToList();

        if (!economyOptions.Any())
        {
            return new GetShippingOptionResponse()
            {
                ShippingOptions = new List<ShippingOption>()
                {
                    new ShippingOption()
                    {
                        Name = "📦Courier Guy",
                        Rate = 150,
                        Description =
                            "Rest assured your goods are in good hands, and fully insured in transit",
                    }
                },
                ShippingFromMultipleLocations = false,
            };
        }

        var shippingOption = new ShippingOption()
        {
            Name = "📦 Courier Guy",
            Rate =
                economyOptions.Sum(x => x.ShippingOptionRate - x.Extras?.Sum(extra => extra.InsuranceCharge) ?? 0),
            Description = "Rest assured your goods are in good hands, and fully insured in transit",
        };
        return new GetShippingOptionResponse()
        {
            ShippingOptions = new List<ShippingOption>() { shippingOption }, ShippingFromMultipleLocations = false,
        };
    }
}