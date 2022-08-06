namespace SeniorTest.Api.Factories;

public class CustomDbContextFactory<T>: ICustomDbContextFactory<T>
{
    private IServiceProvider _serviceProvider;

    public CustomDbContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T? CreateDbContext()
    {
        return (T?) _serviceProvider.GetService(typeof(T));
    }

    public Task<T?> CreateDbContextAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return Task.FromResult((T?) _serviceProvider.GetService(typeof(T))) ;
    }
}