using BeniceSoft.Abp.Ddd.Domain;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Dapper;

[DependsOn(
    typeof(BeniceSoftAbpDddDomainModule)
)]
public class BeniceSoftAbpDapperModule : AbpModule
{
}