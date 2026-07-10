using Web.Public.Common.Abstraction;

namespace Web.Public.Providers
{
    public sealed class SystemClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
