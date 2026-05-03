using Newtonsoft.Json;

namespace Framework.Core
{
    public abstract class RemoteConfigData
    {
        [JsonIgnore]
        public abstract string Key { get; }

        [JsonIgnore]
        public virtual bool PrettyPrint { get; } = false;
    }
}
