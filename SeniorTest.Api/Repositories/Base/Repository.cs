using Microsoft.EntityFrameworkCore;
using SeniorTest.Api.Factories;
using SeniorTest.Core.Repositories.Base;
using SeniorTest.DataModel.Data;

namespace SeniorTest.Api.Repositories.Base;
/*
 * Due to blazor nature, it is necessary to  create in every method the IApplicationDbContext and not 
 * in the constructor.
 */
public class Repository<T> : IRepository<T> where T : class
{
    private const bool V = true;
    private IApplicationDbContext  _applicationDbContext;
    private readonly ICustomDbContextFactory<IApplicationDbContext> _contextFactory;
    private bool disposed = false;
    private DbSet<T> _table = null;

    public Repository(ICustomDbContextFactory<IApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;            
    }

    public bool IsDisposed() 
    {
      return disposed;          
    }


    public T GetById(object id)
    {
        _applicationDbContext = _contextFactory.CreateDbContext();
        _table = _applicationDbContext.Set<T>();
        return _table.Find(id);
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        _applicationDbContext = await _contextFactory.CreateDbContextAsync();
        _table = _applicationDbContext.Set<T>();
        return await _table.FindAsync(id);
    }
    
    public IEnumerable<T> GetAll()
    {
        _applicationDbContext = _contextFactory.CreateDbContext();
        _table = _applicationDbContext.Set<T>();
        return _table.AsNoTracking().ToList();
    }

    public async Task<List<T>> GetAllAsync()
    {
        _applicationDbContext = await _contextFactory.CreateDbContextAsync();
        _table = _applicationDbContext.Set<T>();
        return await _table.AsNoTracking().ToListAsync();
    }

    public IQueryable<T> GetAllAsQueryable() {
        _applicationDbContext = _contextFactory.CreateDbContext();
        _table = _applicationDbContext.Set<T>();
        return _table.AsNoTracking().AsQueryable();
    }

    public T Create(T newValue)
    {
        _applicationDbContext = _contextFactory.CreateDbContext();
        _table = _applicationDbContext.Set<T>();
        _table.Add(newValue);
        _applicationDbContext.SaveChanges();
        _applicationDbContext.Entry(newValue).Reload();
        return newValue;
    }
    
    public async Task<T> CreateAsync(T newValue)
    {
        _applicationDbContext = await _contextFactory.CreateDbContextAsync();
        _table = _applicationDbContext.Set<T>();
        await _table.AddAsync(newValue);
        await _applicationDbContext.SaveChangesAsync();
        await _applicationDbContext.Entry(newValue).ReloadAsync();
        return newValue;
    }

    public async Task<IList<T>> BulkCreateAsync(IList<T> newValue)
    {
        _applicationDbContext = await _contextFactory.CreateDbContextAsync();
        var transaction = await _applicationDbContext.Database.BeginTransactionAsync();
        
        try
        {
            _table = _applicationDbContext.Set<T>();
            foreach (var item in newValue)
            {
                if (_applicationDbContext.Entry(item).State == EntityState.Detached)
                    _applicationDbContext.Attach(item);
                _applicationDbContext.Entry(item).State = EntityState.Added;
            }     
            await _applicationDbContext.SaveChangesAsync();
            await transaction.CommitAsync();            
            return await _table.AsNoTracking().ToListAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return new List<T>();
        }
    }

    public async Task<T> ModifyAsync(T newValue)
    {
        _applicationDbContext = await _contextFactory.CreateDbContextAsync();        
        _applicationDbContext.Attach(newValue);        
        _applicationDbContext.Entry(newValue).State = EntityState.Modified;
        await _applicationDbContext.SaveChangesAsync();
        return newValue;
    }

    public async Task<T> RemoveAsync(T newValue)
    {
        try
        {
            _applicationDbContext = await _contextFactory.CreateDbContextAsync();            
            _applicationDbContext.Attach(newValue);
            _applicationDbContext.Entry(newValue).State = EntityState.Deleted;
            await _applicationDbContext.SaveChangesAsync();
            return null ;
        }
        catch (Exception)
        {
            return null;
        }        
    }

    public async Task<bool> BulkRemoveAsync(IList<T> newValue)
    {
        try
        {
            _applicationDbContext = await _contextFactory.CreateDbContextAsync();   
            var transaction = await _applicationDbContext.Database.BeginTransactionAsync();
            //_applicationDbContext.AttachRange(newValue);
            foreach (var item in newValue)
            {
                if (_applicationDbContext.Entry(item).State == EntityState.Detached)
                    _applicationDbContext.Attach(item);
                _applicationDbContext.Entry(item).State = EntityState.Deleted;
            }            
            await _applicationDbContext.SaveChangesAsync(CancellationToken.None);
            await transaction.CommitAsync();  
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public T SaveOrInsert(T entity)
    {
        throw new NotImplementedException(); 
    }

    public async Task<T> SaveOrInsertAsync(T entity)
    {
        _applicationDbContext = await _contextFactory.CreateDbContextAsync();        
        _applicationDbContext.Attach(entity);
        await _applicationDbContext.SaveChangesAsync();
        await _applicationDbContext.Entry(entity).ReloadAsync();
        return entity;
    }


    public async Task<T?> DeleteAllAsync()
    {
        _applicationDbContext = await _contextFactory.CreateDbContextAsync();
        
        var transaction = await _applicationDbContext.Database.BeginTransactionAsync();
        try
        {           
            
            await _applicationDbContext.Database.ExecuteSqlRawAsync("delete from " + typeof(T).Name);
            await transaction.CommitAsync();
            return (T)Activator.CreateInstance<T>();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return null;
        }
        
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _applicationDbContext.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

internal class NewClass
{
    public NewClass()
    {
    }

    public override bool Equals(object? obj)
    {
        return obj is NewClass other;
    }

    public override int GetHashCode()
    {
        return 0;
    }
}