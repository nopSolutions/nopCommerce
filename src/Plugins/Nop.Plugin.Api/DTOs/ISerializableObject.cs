using System;

namespace Nop.Plugin.Api.DTOs
{
    public interface ISerializableObject
    {
        string GetPrimaryPropertyName();
        Type GetPrimaryPropertyType();
    }
}