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
}