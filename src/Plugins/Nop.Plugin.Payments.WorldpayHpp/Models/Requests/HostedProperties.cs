using System.Text.Json.Serialization;
using Nop.Plugin.Payments.WorldpayHpp.Converters;
using Nop.Plugin.Payments.WorldpayHpp.Enums;

namespace Nop.Plugin.Payments.WorldpayHpp.Models.Requests;
public class HostedProperties { 
    
    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public AddressVisibility ShowBillingAddress { get; set; } =  AddressVisibility.HIDE;

    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public AddressVisibility ShowShippingAddress { get; set; } =  AddressVisibility.HIDE;

    [JsonConverter(typeof(BooleanStringJsonConverter))]
    public BooleanString ShowCountryList { get; set; } = BooleanString.False;

    //// [JsonConverter(typeof(JsonStringEnumConverter))]
    [JsonConverter(typeof(BooleanStringJsonConverter))]
    public BooleanString ShowLanguageList { get; set; }  = BooleanString.False;

    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public ContactDetailsVisibility ShowContactDetails { get; set; } =  ContactDetailsVisibility.HIDE;

    [JsonConverter(typeof(BooleanStringJsonConverter))]
    public BooleanString SendURLParameters { get; set; } = BooleanString.True;

    [JsonConverter(typeof(BooleanStringJsonConverter))]
    public BooleanString ShowPoweredByWorldPay { get; set; } = BooleanString.True;

    [JsonConverter(typeof(BooleanStringJsonConverter))]
    public BooleanString ShowCancelButton { get; set; } = BooleanString.True;

    [JsonConverter(typeof(BooleanStringJsonConverter))]
    public BooleanString ShowChangePaymentMethodButton { get; set; } = BooleanString.True;

    [JsonConverter(typeof(BooleanStringJsonConverter))]
    public BooleanString DisableStrictUrls { get; set; } = BooleanString.True;

    [JsonConverter(typeof(BooleanStringJsonConverter))]
    public BooleanString ShowHeader { get; set; } = BooleanString.True;

    [JsonConverter(typeof(BooleanStringJsonConverter))]
    public BooleanString ShowFooter { get; set; } = BooleanString.True;


    [JsonConverter(typeof(BooleanStringJsonConverter))]
    public BooleanString ShowCardIcons { get; set; }  = BooleanString.True;

    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public PaymentButtonLabel PaymentButtonLabel { get; set; } = PaymentButtonLabel.makePayment;

    [JsonConverter(typeof(BooleanStringJsonConverter))]
    public BooleanString ShowCardholderName { get; set; } = BooleanString.True;

    [JsonConverter(typeof(BooleanStringJsonConverter))]
    public BooleanString ShowPaymentDetailsHeader { get; set; } = BooleanString.True;

    [JsonConverter(typeof(BooleanStringJsonConverter))]
    public BooleanString PassBackErrorReasons { get; set; } = BooleanString.True;

    [JsonConverter(typeof(BooleanStringJsonConverter))]
    public BooleanString MaskCardDetails { get; set; }  = BooleanString.False;

    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public GooglePayButtonColour GooglePayButtonColour { get; set; }  = GooglePayButtonColour.black;

    [JsonConverter(typeof(GooglePayButtonLabelJsonConverter))] 
    public GooglePayButtonLabel GooglePayButtonLabel { get; set; }  = GooglePayButtonLabel.Short;

    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public ApplePayButtonType ApplePayButtonType { get; set; }  = ApplePayButtonType.plain;

    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public ApplePayButtonStyle ApplePayButtonStyle { get; set; } = ApplePayButtonStyle.black;

}