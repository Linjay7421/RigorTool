namespace Web.Public.Common.Abstraction
{
    public interface IObjectKeyGenerator
    {
        string Generate(string prefix, Guid fileId, string extension);
    }
}
