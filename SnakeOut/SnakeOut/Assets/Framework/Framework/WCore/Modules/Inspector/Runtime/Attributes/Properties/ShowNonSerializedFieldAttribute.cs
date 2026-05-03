using System;

namespace Framework.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShowNonSerializedAttribute : Attribute { }
}
