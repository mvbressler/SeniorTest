using Microsoft.EntityFrameworkCore;
using SeniorTest.Api.Factories;
using SeniorTest.Core.Repositories.Base;
using SeniorTest.DataModel.Data;

namespace SeniorTest.Api.Repositories.Base;
/*
 * Due to blazor nature, it is necessary to  create in every method the IApplicationDbContext and not 
 * in the constructor.
 */
public class Repository<T> : IDisposable, IRepository<T> where T : class
{
    protected readonly ICustomDbContextFactory<IApplicationDbContext> _contextFactory;
    private bool disposed = false;
    

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
        using var _applicationDbContext = _contextFactory.CreateDbContext();
        try
        {
            var _table = _applicationDbContext.Set<T>();
            return _table.Find(id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _applicationDbContext.Database.CloseConnection();
        }
        
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        await using var applicationDbContext = await _contextFactory.CreateDbContextAsync();
        try
        {
            var _table = applicationDbContext.Set<T>();
            return await _table.FindAsync(id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await applicationDbContext.Database.CloseConnectionAsync();
        }
        
    }
    
    public IEnumerable<T> GetAll()
    {
        using var _applicationDbContext = _contextFactory.CreateDbContext();
        try
        {
            var _table = _applicationDbContext.Set<T>();
            return _table.AsNoTracking().ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _applicationDbContext.Database.CloseConnection();
        }
        
    }

    public async Task<List<T>> GetAllAsync()
    {
        await using var  applicationDbContext = await _contextFactory.CreateDbContextAsync();
        try
        {
            var _table = applicationDbContext.Set<T>();
            return await _table.AsNoTracking().ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await applicationDbContext.Database.CloseConnectionAsync();
        }
    }

    public IQueryable<T> GetAsQueryable() {
        var applicationDbContext = _contextFactory.CreateDbContext();
        try
        {
            var table = applicationDbContext?.Set<T>();
            return table.AsNoTracking().AsQueryable();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    public virtual Task<bool> ExistsAsync(T obj)
    {
        throw new NotImplementedException();
    }

    public T? Find(object[] keyValues)
    {
        throw new NotImplementedException();
    }
    
    public Task<T?> FindAsync(object[] keyValues)
    {
        throw new NotImplementedException();
    }


    public T Create(T newValue)
    {
         using var applicationDbContext =  _contextFactory.CreateDbContext();
         try
         {
            var table = applicationDbContext?.Set<T>();
            table?.Add(newValue);
            applicationDbContext?.SaveChanges();
            applicationDbContext?.Entry(newValue).Reload();
        return newValue;
         }
         catch (Exception e)
         {
             Console.WriteLine(e);
             throw;
         }
         finally
         {
             applicationDbContext.Database.CloseConnection();
         }
    }
    
    public async Task<T> CreateAsync(T newValue)
    {
        await using var applicationDbContext = await _contextFactory.CreateDbContextAsync();
        try
        {
            if (applicationDbContext.Entry(newValue).State == EntityState.Detached)
                applicationDbContext.Attach(newValue);
            applicationDbContext.Entry(newValue).State = EntityState.Added;
            await applicationDbContext.SaveChangesAsync();
            await applicationDbContext.Entry(newValue).ReloadAsync();
            return newValue;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await applicationDbContext.Database.CloseConnectionAsync();
        }
        
    }

    public async Task<IList<T>> BulkCreateAsync(IList<T> newValue)
    {
        await using var applicationDbContext = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await applicationDbContext.Database.BeginTransactionAsync();
        try
        {
            var table = applicationDbContext.Set<T>();
            foreach (var item in newValue)
            {
                if (await ExistsAsync(item)) continue;
                if (applicationDbContext.Entry(item).State == EntityState.Detached)
                    applicationDbContext.Attach(item);
                applicationDbContext.Entry(item).State = EntityState.Added;
            }     
            await applicationDbContext.SaveChangesAsync();
            await transaction.CommitAsync();            
            return await table.AsNoTracking().ToListAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return new List<T>();
        }
        finally
        {
            await applicationDbContext.Database.CloseConnectionAsync();
        }
    }

    public async Task<T> ModifyAsync(T newValue)
    {
        await using var applicationDbContext = await _contextFactory.CreateDbContextAsync();
        try
        {
            applicationDbContext.Attach(newValue);        
            applicationDbContext.Entry(newValue).State = EntityState.Modified;
            await applicationDbContext.SaveChangesAsync();
            return newValue;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await applicationDbContext.Database.CloseConnectionAsync();
        }
        
    }

    public async Task<T> RemoveAsync(T newValue)
    {
        await using var applicationDbContext = await _contextFactory.CreateDbContextAsync();
        try
        {
            applicationDbContext.Attach(newValue);
            applicationDbContext.Entry(newValue).State = EntityState.Deleted;
            await applicationDbContext.SaveChangesAsync();
            return null ;
        }
        catch (Exception)
        {
            return null;
        } 
        finally
        {
            await applicationDbContext.Database.CloseConnectionAsync();
        }
    }

    public async Task<bool> BulkRemoveAsync(IList<T> newValue)
    {
        await using var _applicationDbContext = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await _applicationDbContext.Database.BeginTransactionAsync();
        try
        {

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
        finally
        {
            //transaction.Dispose();
            await _applicationDbContext.Database.CloseConnectionAsync();
        }
    }

    public T SaveOrInsert(T entity)
    {
        using var applicationDbContext = _contextFactory.CreateDbContext();
        try
        {
            applicationDbContext?.Attach(entity);
            applicationDbContext?.SaveChanges();
            applicationDbContext.Entry(entity).Reload();

            return entity;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            applicationDbContext?.Database.CloseConnection();
        }
    }

    public async Task<T> SaveOrInsertAsync(T entity)
    {
        await using var applicationDbContext = await _contextFactory.CreateDbContextAsync();
        try
        {
            applicationDbContext?.Attach(entity);
            await applicationDbContext?.SaveChangesAsync()!;
            await applicationDbContext.Entry(entity).ReloadAsync();

            return entity;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await applicationDbContext?.Database.CloseConnectionAsync()!;
        }
    }


    public async Task<T?> DeleteAllAsync()
    {
        await using var applicationDbContext = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await applicationDbContext.Database.BeginTransactionAsync();
        try
        {

            await applicationDbContext.Database.ExecuteSqlRawAsync("delete from " + typeof(T).Name);
            await transaction.CommitAsync();
            return (T)Activator.CreateInstance<T>();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return null;
        }
        finally
        {
            await applicationDbContext?.Database.CloseConnectionAsync()!;
        }
        
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                //_applicationDbContext.Dispose();
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