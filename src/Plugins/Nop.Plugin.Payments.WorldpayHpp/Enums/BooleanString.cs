using System.Runtime.Serialization;

namespace Nop.Plugin.Payments.WorldpayHpp.Enums;


public enum BooleanString {

    [EnumMember(Value = "true")]
    True,
    
    [EnumMember(Value = "false")]
    False 
}
