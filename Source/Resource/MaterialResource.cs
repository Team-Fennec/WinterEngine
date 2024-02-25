using Hjson;
using log4net;

namespace WinterEngine.Resource;

[AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
public class MatPropertyAttribute : Attribute {
    public Type PropType { get; set; }
    public string PropName { get; set; }

    public MatPropertyAttribute(Type propType, string propName) {
        PropType = propType;
        PropName = propName;
    }
}

public abstract class MaterialResource {
    public abstract string ShaderName { get; }
}
