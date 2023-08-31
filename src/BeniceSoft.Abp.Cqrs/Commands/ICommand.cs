namespace BeniceSoft.Abp.Cqrs.Commands;

public interface ICommand
{
}

public interface ICommand<out TResult> : ICommand
{
}