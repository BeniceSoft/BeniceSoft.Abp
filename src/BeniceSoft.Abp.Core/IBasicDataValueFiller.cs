using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeniceSoft.Abp.Core;

/// <summary>
/// 基础数据值填充
/// </summary>
public interface IBasicDataValueFiller
{
    /// <summary>
    /// 填充单个对象
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task FillAsync<T>(T source) where T : class;

    /// <summary>
    /// 并行填充多个对象
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task ParallelFillAsync<T>(IReadOnlyList<T> source) where T : class;
}