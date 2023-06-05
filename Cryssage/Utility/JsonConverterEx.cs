using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Cryssage.Models;

namespace Cryssage.Utility
{
public class JsonConverterEx : JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType.IsSubclassOf(typeof(MessageModel));

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);

        var typeName = jsonObject["$type"]?.ToString();
        if (!string.IsNullOrEmpty(typeName))
        {
            var derivedType = Type.GetType(typeName);
            if (derivedType != null && derivedType.IsSubclassOf(objectType))
            {
                return jsonObject.ToObject(derivedType, serializer);
            }
        }

        return jsonObject.ToObject(objectType, serializer);
    }

    public override void WriteJson(JsonWriter writer, object value,
                                   JsonSerializer serializer) => serializer.Serialize(writer, value);
}
}
