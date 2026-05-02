using System.Runtime.Serialization;

namespace Nop.Plugin.Tax.Avalara.ItemClassificationAPI;

public enum NameEnum
{
    [EnumMember(Value = "coo")]
    Coo,

    [EnumMember(Value = "description")]
    Description,

    [EnumMember(Value = "image_url")]
    ImageUrl,

    [EnumMember(Value = "url")]
    Url,

    [EnumMember(Value = "material")]
    Material,

    [EnumMember(Value = "price")]
    Price,

    [EnumMember(Value = "weight")]
    Weight,

    [EnumMember(Value = "height")]
    Height,

    [EnumMember(Value = "length")]
    Length,

    [EnumMember(Value = "width")]
    Width,
}