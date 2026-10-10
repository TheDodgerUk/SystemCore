using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public class MetaDataConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(Dictionary<MetaDataType, MetaData>).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var properties = jsonObject.Properties().ToList();
        int count = properties.Count;

        var dict = new Dictionary<MetaDataType, MetaData>(count);
        for (int i = 0; i < count; ++i)
        {
            var name = properties[i].Name;
            var metaType = name.ParseEnum<MetaDataType>();
            var dataType = metaType.GetDataType();

            string str = jsonObject[name].ToString(Formatting.None);
            var metaData = Json.JsonNet.ReadFromText(str, dataType);
            dict.Add(metaType, metaData as MetaData);
        }
        return dict;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
