using BeniceSoft.Abp.Auth.Core;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace BeniceSoft.Abp.Auth.EntityFrameworkCore;

public abstract class AuthAbstractionDbContext<TDbContext> : AbpDbContext<TDbContext>
    where TDbContext : DbContext
{
    protected AuthAbstractionDbContext(DbContextOptions<TDbContext> options) : base(options)
    {
    }

    protected ICurrentUserPermissionAccessor UserPermissionAccessor
        => LazyServiceProvider.LazyGetService<ICurrentUserPermissionAccessor>();


    public override int SaveChanges()
    {
        PreSaveChanges();

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        PreSaveChanges();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void PreSaveChanges()
    {
        ChangeTracker.DetectChanges();
        var changes = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified);
        foreach (var objectStateEntry in changes)
        {
            if (objectStateEntry.State == EntityState.Modified)
            {
                var tableName = objectStateEntry.Metadata.GetTableName();
                var currentColumnAuth = UserPermissionAccessor
                    .UserPermission?
                    .ColumnPermissions?
                    .Where(c => c.TableName == tableName)
                    .ToList();

                if (currentColumnAuth is null) continue;

                foreach (var propertyEntry in objectStateEntry.Metadata.GetProperties())
                {
                    if (currentColumnAuth.Any(c => c.ColumnName == propertyEntry.Name && c.ColumnAuthLevel == 0))
                    {
                        var property = objectStateEntry.Property(propertyEntry.Name);
                        property.IsModified = false;
                    }
                }
            }
        }
    }
}