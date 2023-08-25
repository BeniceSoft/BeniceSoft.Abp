using System.Threading.Tasks;
using BeniceSoft.Abp.Core.Models;

namespace BeniceSoft.Abp.Core.Extensions;

public static class ResponseResultExtensions
{
    /// <summary>
    /// 业务对象转换成ResponseResult
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ResponseResult<T> ToSucceed<T>(this T data)
    {
        return new ResponseResult<T>(data);
    }

    /// <summary>
    /// 异步业务对象转换成ResponseResult
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <returns></returns>
    public static async Task<ResponseResult<T>> ToSucceed<T>(this Task<T> task)
    {
        var result = await task;
        return new ResponseResult<T>(result);
    }
}