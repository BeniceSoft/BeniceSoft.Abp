using BeniceSoft.Abp.Ddd.Domain;
using Volo.Abp.Application.Services;

namespace BeniceSoft.Abp.Ddd.Application;

public abstract class BeniceSoftApplicationService : ApplicationService
{
    public IQueryableWrapperFactory QueryableWrapperFactory => LazyServiceProvider.LazyGetRequiredService<IQueryableWrapperFactory>();
}