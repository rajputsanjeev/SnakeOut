#pragma warning disable 0649
#pragma warning disable 0162

namespace Framework.Core
{
    public enum AuthorizationTrackingStatus
    {
        NOT_DETERMINED = 0,
        RESTRICTED,
        DENIED,
        AUTHORIZED
    }
}