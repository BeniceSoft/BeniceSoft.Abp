namespace BeniceSoft.Abp.Core.Exceptions;

public interface IKnownException
{
    string Message { get; }
    int ErrorCode { get; }
    object? ErrorData { get; }
}