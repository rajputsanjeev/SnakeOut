using System;

namespace Framework.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class HideAttribute : ConditionAttribute
    {
        public HideAttribute()
        {

        }
    }
}