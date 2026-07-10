namespace Web.Public.Common.Abstraction
{
    public interface IClock
    {
        public DateTimeOffset UtcNow { get; }
    }
}
