using System.Runtime.InteropServices;
using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
//using SeniotTest.Data;
using Syncfusion.Blazor;
using Newtonsoft.Json.Serialization;
using SeniorTest.Api.Factories;
using SeniorTest.Api.Repositories;
using SeniorTest.Api.Utilities;
using SeniorTest.Core.Utilities;
using SeniorTest.DataModel.Data;
using SeniorTest.Services;

var builder = WebApplication.CreateBuilder(args);
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(
    "OTk5NkAzMjMwMkUzMjJFMzBNWndBMi9jT0t0OVJ4Q2FFSGlhSGJ6aW8vTkhhS1FBSjd4dmw2eGZsTTFNPQ==");

// Add services to the container.
builder.Services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });
builder.Services.AddRazorPages();
builder.Services.AddControllers().AddNewtonsoftJson(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); });
builder.Services.AddServerSideBlazor();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

builder.Services.AddScoped<IAuthentService, AuthentService>();

builder.Services.AddBlazoredLocalStorage();

// builder.Services.AddAuthentication(config =>
// {
//     config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//     config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// });
//
// builder.Services.AddAuthorizationCore(config =>
// {
//     config.AddPolicy(Policies.IsAdmin, Policies.IsAdminPolicy());
//     config.AddPolicy(Policies.IsUser, Policies.IsUserPolicy());
//     
// });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseCors();
//app.UseAuthorization();
//app.UseAuthentication();
app.MapDefaultControllerRoute();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
