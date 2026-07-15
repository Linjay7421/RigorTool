using Web.Library.Application.Abstractions;

namespace Web.Infrastructure.Providers
{
    public sealed class SystemClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
