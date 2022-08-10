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
    
    public static void SetUserFilesRepository(IUserFileRepository userFileRepository, IdentityUser user)
    {
        _userFileRepository = userFileRepository;
        _user = user;
    }
    
    private static List<UserFile> TransforFilesToUserFiles(string path, IEnumerable<FileManagerDirectoryContent> files)
    {
        var fileManagerDirectoryContents = files.ToList();

        return fileManagerDirectoryContents.Select(file => new UserFile
            {
                Filename = file.Name,
                IsZipped = false,
                Path = path,
                User = _user,
                UserId = _user.Id
            })
            .ToList();
    }

    public static async Task<IEnumerable<FileManagerDirectoryContent>> Read(string path, 
        IEnumerable<FileManagerDirectoryContent> files)
    {
        var userFiles = await _userFileRepository
            .GetAllAsQueryable()
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
        await _userFileRepository.BulkRemoveAsync(TransforFilesToUserFiles(path, files));
    }

    public static async void Rename(string path, string name, string newName)
    {
        var userFile = await _userFileRepository.GetAllAsQueryable()
            .FirstOrDefaultAsync(f => f.Path == path && f.Filename == name);
        if (userFile == null) return;
        userFile.Filename = newName;
        await _userFileRepository.ModifyAsync(userFile);
    }
    
    public static async Task Copy(string path,
        IEnumerable<FileManagerDirectoryContent> files)
    {
        await _userFileRepository.BulkCreateAsync(TransforFilesToUserFiles(path, files));
    }
    
    public static async Task Move(string path, string targetPath, 
        IEnumerable<FileManagerDirectoryContent> files)
    {
        await _userFileRepository.BulkRemoveAsync(TransforFilesToUserFiles(path, files));
        await _userFileRepository.BulkCreateAsync(TransforFilesToUserFiles(targetPath, files));
    }
    
    public static async Task Upload(string path,
        IList<IFormFile> uploadFiles)
    {
        var list = uploadFiles.Select(x => new UserFile()
        {
            Filename = x.FileName,
            IsZipped = false,
            Path = path,
            User = _user,
            UserId = _user.Id
        }).ToList();

        await _userFileRepository.BulkCreateAsync(list);
    }

}