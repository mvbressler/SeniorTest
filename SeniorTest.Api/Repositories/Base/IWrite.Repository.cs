namespace SeniorTest.Api.Repositories.Base;

public interface IWriteRepository<T> where T : class
{
    public T Create(T newValue);
    public Task<T> CreateAsync(T newValue);
    public Task<IList<T>> BulkCreateAsync(IList<T> newValue);
    public Task<T> ModifyAsync(T newValue);
    public Task<T> RemoveAsync(T newValue);
    public Task<bool> BulkRemoveAsync(IList<T> newValue);
    public Task<T> DeleteAllAsync();


    public T SaveOrInsert(T entity);
    public Task<T> SaveOrInsertAsync(T entity);
}