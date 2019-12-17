namespace Nop.Plugin.Api.JSON.Serializers
{
    using Nop.Plugin.Api.DTOs;

    public interface IJsonFieldsSerializer
    {
        string Serialize(ISerializableObject objectToSerialize, string fields);
    }
}
