using System.Net;

namespace BeniceSoft.Abp.Core.Models;

/// <summary>
/// 返回结果
/// </summary>
public class ResponseResult
{
    /// <summary>
    /// 错误编码。默认为200，表示没有错误。 定义在统一的地方。
    /// </summary>
    public int Code { get; set; } = 200;

    /// <summary>
    /// 错误文案。默认为空字符串。 直接由后端返回错误原因，一般直接是error对应的错误文案，将来可以由前端再次定义。
    /// </summary>
    public string Message { get; set; } = string.Empty;

    public object? ExceptionData { get; set; }

    /// <summary>
    /// 构造方法
    /// </summary>
    public ResponseResult()
    {
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <param name="exceptionData"></param>
    public ResponseResult(int code, string message, object? exceptionData = null)
    {
        Code = code;
        Message = message;
        ExceptionData = exceptionData;
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public ResponseResult(HttpStatusCode code, string message, string? exception = null) : this((int)code, message, exception)
    {
    }
}

/// <summary>
/// 返回结果
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResponseResult<T> : ResponseResult
{
    /// <summary>
    /// 构造方法
    /// </summary>
    public ResponseResult()
    {
        Data = default!;
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="td"></param>
    public ResponseResult(T td) : this(200, "Success") => Data = td;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    public ResponseResult(int code, string message) : base(code, message, null)
    {
        Code = code;
        Message = message;
        Data = default!;
    }

    /// <summary>
    /// 最重要的返回数据。 可以是number,bool,string,array,object
    /// </summary>
    public T Data { get; set; }
}