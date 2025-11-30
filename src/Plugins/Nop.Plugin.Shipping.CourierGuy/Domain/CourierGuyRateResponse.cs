using Newtonsoft.Json;
namespace Nop.Plugin.Shipping.CourierGuy.Domain;

public class CourierGuyRateResponse
{
    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("service_days")]
    public ServiceDays ServiceDays { get; set; }

    [JsonProperty("rates")]
    public List<Rate> Rates { get; set; }
}

public class BaseRate
{
    [JsonProperty("charge_per_parcel")]
    public List<double> ChargePerParcel { get; set; }

    [JsonProperty("charge")]
    public double Charge { get; set; }

    [JsonProperty("group_name")]
    public string GroupName { get; set; }

    [JsonProperty("vat")]
    public double Vat { get; set; }

    [JsonProperty("vat_type")]
    public string VatType { get; set; }

    [JsonProperty("rate_formula_type")]
    public string RateFormulaType { get; set; }

    [JsonProperty("total_calculated_weight")]
    public int TotalCalculatedWeight { get; set; }
}

public class Extra
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("insurance_charge")]
    public decimal InsuranceCharge { get; set; }

    [JsonProperty("vat")]
    public double Vat { get; set; }

    [JsonProperty("vat_type")]
    public string VatType { get; set; }
}

public partial class Rate
{
    [JsonProperty("rate")]
    public decimal ShippingOptionRate { get; set; }

    [JsonProperty("rate_excluding_vat")]
    public double RateExcludingVat { get; set; }

    [JsonProperty("base_rate")]
    public BaseRate BaseRate { get; set; }

    [JsonProperty("service_level")]
    public ServiceLevel ServiceLevel { get; set; }

    [JsonProperty("surcharges")]
    public List<dynamic> Surcharges { get; set; }

    [JsonProperty("rate_adjustments")]
    public List<dynamic> RateAdjustments { get; set; }

    [JsonProperty("time_based_rate_adjustments")]
    public List<dynamic> TimeBasedRateAdjustments { get; set; }

    [JsonProperty("extras")]
    public List<Extra> Extras { get; set; }

    [JsonProperty("charged_weight")]
    public int ChargedWeight { get; set; }

    [JsonProperty("actual_weight")]
    public int ActualWeight { get; set; }

    [JsonProperty("volumetric_weight")]
    public int VolumetricWeight { get; set; }
}

public class ServiceDays
{
    [JsonProperty("collection_service_days")]
    public string CollectionServiceDays { get; set; }

    [JsonProperty("delivery_service_days")]
    public string DeliveryServiceDays { get; set; }
}

public class ServiceLevel
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("delivery_date_from")]
    public string DeliveryDateFrom { get; set; }

    [JsonProperty("delivery_date_to")]
    public string DeliveryDateTo { get; set; }

    [JsonProperty("collection_date")]
    public string CollectionDate { get; set; }

    [JsonProperty("collection_cut_off_time")]
    public DateTime CollectionCutOffTime { get; set; }

    [JsonProperty("vat_type")]
    public string VatType { get; set; }

    [JsonProperty("insurance_disabled")]
    public bool InsuranceDisabled { get; set; }
}