using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace BeniceSoft.Abp.Sample.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class SampleDbContext : AbpDbContext<SampleDbContext>
{
    public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
    {
    }
}