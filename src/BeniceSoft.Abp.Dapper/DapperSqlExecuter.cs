using BeniceSoft.Abp.Ddd.Domain;
using Dapper;
using Microsoft.Extensions.Logging;

namespace BeniceSoft.Abp.Dapper;

public abstract class DapperSqlExecuter<TContext> : ISqlExecuter<TContext> where TContext : ISqlExecutionContext
{
    private readonly ILogger<DapperSqlExecuter<TContext>> _logger;

    public DapperSqlExecuter(ILogger<DapperSqlExecuter<TContext>> logger)
    {
        _logger = logger;
    }

    public async Task<TResult> QueryFirstAsync<TResult>(string sql, object param = null)
    {
        _logger.LogInformation("Execute Query: {0}", sql);

        var connection = await SqlExecutionContext.GetDbConnectionAsync();
        return await connection.QueryFirstAsync<TResult>(sql, param, await SqlExecutionContext.GetCurrentDbTransactionAsync());
    }

    public async Task<List<TResult>> QueryAsync<TResult>(string sql, object param = null)
    {
        _logger.LogInformation("Execute Query: {0}", sql);

        var connection = await SqlExecutionContext.GetDbConnectionAsync();
        return (await connection.QueryAsync<TResult>(sql, param, await SqlExecutionContext.GetCurrentDbTransactionAsync())).ToList();
    }

    public async Task<List<Dictionary<string, object>>> QueryDictionaryAsync(string sql, object param = null)
    {
        var connection = await SqlExecutionContext.GetDbConnectionAsync();
        var rows = new List<Dictionary<string, object>>();
        using var reader = await connection.ExecuteReaderAsync(sql, param);
        while (reader.Read())
        {
            var row = new Dictionary<string, object>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.GetValue(i);
            }

            rows.Add(row);
        }

        return rows;
    }

    public async Task<int> ExecuteAsync(string sql, object param = null)
    {
        var connection = await SqlExecutionContext.GetDbConnectionAsync();
        return await connection.ExecuteAsync(sql, param, await SqlExecutionContext.GetCurrentDbTransactionAsync());
    }
    
    public abstract TContext SqlExecutionContext { get; }
}