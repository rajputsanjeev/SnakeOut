using System;

namespace Framework.Core
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class UnpackNestedAttribute : Attribute
    {
        public UnpackNestedAttribute() { }
    }
}
