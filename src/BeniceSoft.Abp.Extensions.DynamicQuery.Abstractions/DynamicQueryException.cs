namespace BeniceSoft.Abp.Extensions.DynamicQuery.Abstractions;

public class DynamicQueryException : Exception
{
    public DynamicQueryException(string message) : base(message)
    {
    }
}