using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SeniorTest.DataModel.Models;

namespace SeniorTest.DataModel.Data;

public interface IApplicationDbContext
{
    public abstract DatabaseFacade  Database { get; }

    public abstract EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;
    public abstract void AttachRange(params object[] entities);
    public abstract EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    public abstract int SaveChanges();
    public abstract Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    public DbSet<TEntity> Set<TEntity>() where TEntity: class;
    public abstract void Dispose();
}