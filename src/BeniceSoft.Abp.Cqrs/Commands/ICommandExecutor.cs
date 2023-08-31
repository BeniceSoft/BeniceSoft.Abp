using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.Cqrs.Commands;

public interface ICommandExecutor
{
    Task<TResult?> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);

    Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default);
}

public class NullCommandExecutor : ICommandExecutor, ISingletonDependency
{
    private readonly ILogger<NullCommandExecutor> _logger = NullLogger<NullCommandExecutor>.Instance;

    public Task<TResult?> ExecuteAsync<TResult>(ICommand<TResult> command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogWarning($"CommandsExecutor ExecuteAsync was not implemented! Using {nameof(NullCommandExecutor)}:");
        _logger.LogWarning("Command : " + command);

        return Task.FromResult((TResult?) default);
    }

    public Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning($"CommandsExecutor ExecuteAsync was not implemented! Using {nameof(NullCommandExecutor)}:");
        _logger.LogWarning("Command : " + command);

        return Task.CompletedTask;
    }
}