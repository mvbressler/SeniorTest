namespace SeniorTest.Api.Repositories.Base;

public interface IReadRepository<T> where T : class
{
    public T GetById(object id);
    public Task<T?> GetByIdAsync(object id);
    public IEnumerable<T> GetAll();    
    public Task<List<T>> GetAllAsync();
    public IQueryable<T> GetAllAsQueryable();
}