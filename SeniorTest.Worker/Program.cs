using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SeniorTest.Api.Factories;
using SeniorTest.Api.Repositories;
using SeniorTest.Core.Repositories;
using SeniorTest.DataModel.Data;
using SeniorTest.Worker;
using SeniorTest.Worker.QueueService;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")), 
            ServiceLifetime.Transient,
            ServiceLifetime.Transient
        );
        services.AddTransient<ICustomDbContextFactory<IApplicationDbContext>, CustomDbContextFactory<IApplicationDbContext>>();
        services.AddSingleton<IUserFileRepository, UserFileRepository>();
        
        services.AddSingleton<MonitorLoop>();
        services.AddHostedService<QueuedHostedService>();
        services.AddSingleton<IBackgroundTaskQueue>(_ => 
        {
            if (!int.TryParse(context.Configuration["QueueCapacity"], out var queueCapacity))
            {
                queueCapacity = 100;
            }

            return new DefaultBackgroundTaskQueue(queueCapacity);
        });
        
    })
    
    .Build();



MonitorLoop monitorLoop = host.Services.GetRequiredService<MonitorLoop>()!;
monitorLoop.StartMonitorLoop();

await host.RunAsync();
