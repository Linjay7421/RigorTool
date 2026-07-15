namespace Web.Library.Application.Abstractions
{
    public interface IClock
    {
        public DateTimeOffset UtcNow { get; }
    }
}
