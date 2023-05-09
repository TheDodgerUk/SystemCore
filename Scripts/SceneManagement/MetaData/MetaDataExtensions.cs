using System;
using System.Collections.Generic;
using System.Linq;

public static class MetaDataExtensions
{
    private static readonly Dictionary<MetaDataType, Type> DataTypeToType = GetEnumToTypeMap();

    public static Type GetDataType(this MetaDataType enumType) => DataTypeToType.Get(enumType);

    public static MetaDataType? GetMetaDataType(this Type type) => type.GetAttribute<MetaDataAttribute>()?.MetaType;

    private static Dictionary<MetaDataType, Type> GetEnumToTypeMap()
    {
        var metaDataTypes = typeof(MetaData).GetChildTypes();
        var pairs = metaDataTypes.Select(t => new { Enum = t.GetMetaDataType(), Type = t });
        return pairs.Where(p => p.Enum != null).ToDictionary(p => p.Enum.Value, p => p.Type);
    }
}
