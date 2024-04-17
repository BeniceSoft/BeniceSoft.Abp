using BeniceSoft.Abp.Core.Exceptions;

namespace BeniceSoft.Abp.Sample.Domain.Shared.Exceptions;

public class BizException : Exception, IKnownException
{
    public BizException(int errorCode, string message, object? errorData = null) : base(message)
    {
        ErrorCode = errorCode;
        Message = message;
        ErrorData = errorData;
    }

    public string Message { get; }
    public int ErrorCode { get; }
    public object? ErrorData { get; }
}