namespace BeniceSoft.Abp.Ddd.Domain;

public interface ISqlExecuter<out TContext> where TContext : ISqlExecutionContext
{
    TContext SqlExecutionContext { get; }

    /// <summary>
    /// 查询单个
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<TResult> QueryFirstAsync<TResult>(string sql, object? param = null);

    /// <summary>
    /// 查询列表
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<List<TResult>> QueryAsync<TResult>(string sql, object? param = null);

    Task<List<Dictionary<string, object>>> QueryDictionaryAsync(string sql, object? param = null);

    /// <summary>
    /// 执行sql
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    Task<int> ExecuteAsync(string sql, object? param = null);
}