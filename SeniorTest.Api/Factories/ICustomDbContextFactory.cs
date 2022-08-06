namespace SeniorTest.Api.Factories;

public interface ICustomDbContextFactory<T> 
{
    T? CreateDbContext();
    Task<T?> CreateDbContextAsync(CancellationToken cancellationToken = default(CancellationToken));
}