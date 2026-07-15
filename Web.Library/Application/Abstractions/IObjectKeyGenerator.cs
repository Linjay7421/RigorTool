namespace Web.Library.Application.Abstractions
{
    public interface IObjectKeyGenerator
    {
        string Generate(string prefix, Guid fileId, string extension);
    }
}
