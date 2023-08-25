using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.Core.Defaults;

public class NullBasicDataValueFiller : IBasicDataValueFiller, ITransientDependency
{
    public Task FillAsync<T>(T source) where T : class
    {
        return Task.CompletedTask;
    }

    public Task ParallelFillAsync<T>(IReadOnlyList<T> source) where T : class
    {
        return Task.CompletedTask;
    }
}