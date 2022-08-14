using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeniorTest.Core.Repositories;
using SeniorTest.Core.Utilities;
using SeniorTest.DataModel.Models;
using SfFileService.FileManager.Base;

namespace SeniorTest.Api.Utilities;

public static class UpdateUserFiles
{

    private static IUserFileRepository _userFileRepository;
    private static IdentityUser _user;
    private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(0,1);
    
    public static void SetUserFilesRepository(IUserFileRepository userFileRepository, IdentityUser user)
    {
        _userFileRepository = userFileRepository;
        _user = user;
    }
    
    private static async Task<IList<UserFile>> TransforFilesToUserFiles(IEnumerable<FileManagerDirectoryContent> files,
        string path, string? targetPath = null)
    {
        var fileManagerDirectoryContents = files.ToList();
        var existingUserFiles = new List<UserFile>(); 

        if (!string.IsNullOrWhiteSpace(targetPath))
        {
            existingUserFiles = await _userFileRepository
                .GetAsQueryable()
                .Where(x => x.UserId == _user.Id && x.Path == path)
                .ToListAsync();
        } 

        return fileManagerDirectoryContents.Select(file => new UserFile
            {
                Filename = file.Name,
                IsZipped = (existingUserFiles.Count > 0) && existingUserFiles.FirstOrDefault(x => x.UserId == _user.Id && x.Path == path && x.Filename == file.Name)!.IsZipped,
                Path = (string.IsNullOrWhiteSpace(targetPath) ? path : targetPath),
                User = _user,
                UserId = _user.Id
            })
            .ToList();
    }

    public static async Task<IEnumerable<FileManagerDirectoryContent>> Read(string path, 
        IEnumerable<FileManagerDirectoryContent> files)
    {
        var userFiles = await _userFileRepository
            .GetAsQueryable()
            .Where(x => (x.UserId == _user.Id && x.Path == path) ||
                        (x.Path == path && x.IsZipped == true))
            .ToListAsync();
        var newList = files
            .Where(r =>
                (userFiles.Any(u => r.Name == u.Filename && r.FilterPath == u.Path.Replace("/", "\\"))) ||
                (r.IsFile == false)).ToList();
        return newList;
    }

    public static async Task Delete(string path,
        IEnumerable<FileManagerDirectoryContent> files)
    {
        await _userFileRepository.BulkRemoveAsync(await TransforFilesToUserFiles(files, path));
    }

    public static async void Rename(string path, string name, string newName)
    {
        var userFile = await _userFileRepository.GetAsQueryable()
            .FirstOrDefaultAsync(f => f.Path == path && f.Filename == name);
        if (userFile == null) return;
        userFile.Filename = newName;
        await _userFileRepository.ModifyAsync(userFile);
    }
    
    public static async Task Copy(string path, string targetPath, 
        IEnumerable<FileManagerDirectoryContent> files)
    {
        await _userFileRepository.BulkCreateAsync(await TransforFilesToUserFiles(files, path, targetPath));
    }
    
    public static async Task Move(string path, string targetPath, 
        IEnumerable<FileManagerDirectoryContent> files)
    {
        await _userFileRepository.BulkCreateAsync(await TransforFilesToUserFiles(files, path, targetPath));
        await _userFileRepository.BulkRemoveAsync(await TransforFilesToUserFiles(files, path));
        
    }
    
    public static async Task Upload(string path,
        IEnumerable<IFormFile> uploadFiles)
    {
        var list = uploadFiles.Select(x => new UserFile()
        {
            Filename = x.FileName,
            IsZipped = false,
            Path = path,
            User = _user,
            UserId = _user.Id
        }).ToList();

        if (list.Count > 1)
        {
            await _userFileRepository.BulkCreateAsync(list);    
        }
        else
        {
            await _userFileRepository.CreateAsync(list.FirstOrDefault());
        }
    }

}