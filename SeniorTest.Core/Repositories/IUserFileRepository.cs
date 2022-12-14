using SeniorTest.Core.Repositories.Base;
using SeniorTest.DataModel.Models;

namespace SeniorTest.Core.Repositories;

public interface IUserFileRepository: IRepository<UserFile>
{
    public Task<UserFile> UpdateZippedFile(UserFile userFile, string newFilename);
}