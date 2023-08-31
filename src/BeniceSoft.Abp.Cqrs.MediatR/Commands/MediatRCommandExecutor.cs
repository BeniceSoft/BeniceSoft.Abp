using BeniceSoft.Abp.Cqrs.Commands;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.Cqrs.MediatR.Commands;

public class MediatRCommandExecutor : ICommandExecutor, ITransientDependency
{
    
    
    public Task<TResult?> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));

        throw new NotImplementedException();
    }

    public Task ExecuteAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    
}