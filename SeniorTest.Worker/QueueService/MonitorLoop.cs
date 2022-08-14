using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeniorTest.Core.Repositories;
using SeniorTest.DataModel.Models;
using SeniorTest.Worker.FilesUtilities;

namespace SeniorTest.Worker.QueueService;

public class MonitorLoop
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger<MonitorLoop> _logger;
    private readonly CancellationToken _cancellationToken;
    private readonly IUserFileRepository _userFileRepository;
    private IdentityUser _identityUser;

    public MonitorLoop(
        IBackgroundTaskQueue taskQueue,
        ILogger<MonitorLoop> logger,
        IHostApplicationLifetime applicationLifetime, 
        IUserFileRepository userFileRepository)
    {
        _taskQueue = taskQueue;
        _logger = logger;
        _userFileRepository = userFileRepository;
        _cancellationToken = applicationLifetime.ApplicationStopping;
    }

    public void StartMonitorLoop()
    {
        _logger.LogInformation($"{nameof(MonitorAsync)} loop is starting.");

        // Run a console user input loop in a background thread
        Task.Run(async () => await MonitorAsync());
    }

    private async ValueTask MonitorAsync()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            // var keyStroke = Console.ReadKey();
            // if (keyStroke.Key == ConsoleKey.W)
            // {
            //     // Enqueue a background work item
            //     await _taskQueue.QueueBackgroundWorkItemAsync(BuildWorkItemAsync);
            // }

            var userFiles = await _userFileRepository
                .GetAsQueryable()
                .Where(x => x.IsZipped == false)
                .ToListAsync(_cancellationToken);
            foreach (var userFile in userFiles)
            {
                await _taskQueue.QueueBackgroundWorkItemAsync((token) => BuildWorkItemAsync(userFile, _cancellationToken));
            }
            await Task.Delay(TimeSpan.FromSeconds(60), _cancellationToken);
        }
    }

    private async ValueTask BuildWorkItemAsync(UserFile userFile, CancellationToken token)
    {
        // 1 - Comprimir el FILE
        var originalFilename = userFile.Filename;
        var pathToFiles = "..\\SeniorTest\\wwwroot\\files" + userFile.Path.Replace("/", "\\");  
        var compressedFilename =  await FileUtils.CompressTo7ZipFileAsync(pathToFiles, originalFilename, token);
        _logger
            .LogInformation("File: {OriginalFilename} is already compressed. New file is: {CompressedFilename}",
                originalFilename, compressedFilename);

        
        // 2 - Actualizar la DB
        await _userFileRepository.UpdateZippedFile(userFile, compressedFilename);
        
        _logger
            .LogInformation("Modifying DB with the new file and marking it as IsZipped");
        
        // 3 - Eliminar el File original
        if (!originalFilename.EndsWith(".zip") || !originalFilename.EndsWith(".7z"))
        {
            await FileUtils.DeleteFile(pathToFiles , originalFilename);
            _logger
                .LogInformation("Deleting File: {Filename}", originalFilename); 
        }
        else
        {
            _logger
                .LogInformation("No need to delete File: {Filename}", originalFilename); 
        }
        

    }
}
