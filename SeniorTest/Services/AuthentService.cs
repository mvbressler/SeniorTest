using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Refit;
using SeniorTest.Core.Utilities;
using SeniorTest.DataModel.Models;
using SeniorTest.Middleware;

namespace SeniorTest.Services;

class AuthentService : IAuthentService
{
    private readonly HttpClient _httpClient;
    private NavigationManager _navigationManager;
    private readonly IAuthentService _apiService;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorage;

    public AuthentService(IConfiguration configuration, NavigationManager navigationManager, 
        ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
    {
        _navigationManager = navigationManager;
        _localStorage = localStorage;
        _authenticationStateProvider = authenticationStateProvider;
        _httpClient = new HttpClient(new HttpClientDiagnosticsHandler(new HttpClientHandler()))
        {
            BaseAddress = new Uri(configuration["BaseUrlApi"])
        };
        _apiService = RestService.For<IAuthentService>(_httpClient,
            new RefitSettings()
            {
                // ExceptionFactory = DataServiceUtility.HandleExceptions,
                ContentSerializer = new SystemTextJsonContentSerializer()
            });
    }

    public async Task<LoginResult> Login(LoginModel loginModel )
    {
        try
        {
            try
            {
                var result = await _apiService.Login(loginModel);
                if (!result.Successful) return null;
                await _localStorage.SetItemAsync("authToken", result.Token);
                ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(result.Token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);
                return result;
            }
            catch (ApiException  e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        catch (Exception e)
        {
            throw;
            //return false;
        }
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<RegisterResult> Register(RegisterModel registerModel)
    {
        try
        {
            var result = await _apiService.Register(registerModel).ConfigureAwait(false);
            return result;
        }
        catch (ApiException  e)
        {
            throw;
            //return false;
        }
    }
}