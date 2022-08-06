using Refit;
using SeniorTest.DataModel.Models;

namespace SeniorTest.Services;

public interface IAuthentService
{
    [Post("/api/login")]
    Task<LoginResult> Login([Body] LoginModel loginModel);

    Task Logout();
    
    [Post("/api/accounts")]
    Task<RegisterResult> Register([Body] RegisterModel registerModel);
}



