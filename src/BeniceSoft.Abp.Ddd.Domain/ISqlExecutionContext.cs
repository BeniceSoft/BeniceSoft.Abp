using System.Data;

namespace BeniceSoft.Abp.Ddd.Domain;

public interface ISqlExecutionContext
{
    Task<IDbConnection> GetDbConnectionAsync();

    Task<IDbTransaction?> GetCurrentDbTransactionAsync();
}