using System;
using System.Collections.Generic;
using System.Linq;

namespace BeniceSoft.Abp.Core.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// 循环遍历
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aim"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(this IEnumerable<T>? aim, Action<T>? action)
    {
        if ((aim == null || !aim.Any()) || action == null)
        {
            return;
        }

        foreach (var item in aim)
        {
            action.Invoke(item);
        }
    }

    public static IEnumerable<string> WhereNotNullOrWhiteSpace(this IEnumerable<string> source)
    {
        return source.Where(x => !string.IsNullOrWhiteSpace(x));
    }

    public static IEnumerable<TOut> Map<TIn, TOut>(this IEnumerable<TIn> source, Func<int, TIn, TOut> selector)
    {
        var index = 0;
        foreach (var item in source)
        {
            yield return selector(index++, item);
        }
    }
}