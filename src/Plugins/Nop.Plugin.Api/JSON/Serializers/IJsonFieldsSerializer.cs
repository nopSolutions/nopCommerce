using Nop.Plugin.Api.DTO;

namespace Nop.Plugin.Api.JSON.Serializers
{
    public interface IJsonFieldsSerializer
    {
        string Serialize(ISerializableObject objectToSerialize, string fields);
    }
}
