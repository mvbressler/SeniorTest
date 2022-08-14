using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.EntityFrameworkCore;
using SeniorTest.Api.Factories;
using SeniorTest.Api.Repositories.Base;
using SeniorTest.Core.Repositories;
using SeniorTest.DataModel.Data;
using SeniorTest.DataModel.Models;

namespace SeniorTest.Api.Repositories;

public class UserFileRepository: Repository<UserFile>, IUserFileRepository
{
    public UserFileRepository(ICustomDbContextFactory<IApplicationDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<UserFile> UpdateZippedFile(UserFile userFile, string newFilename)
    {
       await RemoveAsync(userFile);
       
        userFile.Filename = newFilename;
        userFile.IsZipped = true;

        var u = await GetAsQueryable().FirstOrDefaultAsync(x =>
            x.UserId == userFile.UserId && x.Path == userFile.Path && x.Filename == newFilename);
        
        return (u == null) ? await CreateAsync(userFile) : userFile;
    }
    
    public override async Task<bool> ExistsAsync(UserFile obj)
    {
        await using var _applicationDbContext = await _contextFactory.CreateDbContextAsync();
        var table = _applicationDbContext.Set<UserFile>();
        var o = await table.AsNoTracking().FirstOrDefaultAsync(x =>
            x.UserId == obj.UserId && x.Path == obj.Path && x.Filename == obj.Filename);
        return o != null;
    }
}