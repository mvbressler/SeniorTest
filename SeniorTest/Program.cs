using System.Runtime.InteropServices;
using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
//using SeniotTest.Data;
using Syncfusion.Blazor;
using Newtonsoft.Json.Serialization;
using SeniorTest.Api.Factories;
using SeniorTest.Api.Repositories;
using SeniorTest.Api.Utilities;
using SeniorTest.Core.Repositories;
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

//---
builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),  
        b => b.MigrationsAssembly("SeniorTest.DataModel")),
    ServiceLifetime.Transient,
    ServiceLifetime.Transient
);
builder.Services
    .AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtIssuer"],
            ValidAudience = builder.Configuration["JwtAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSecurityKey"]))
        };
    });
//--
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

//------
builder.Services.AddScoped<ICustomDbContextFactory<IApplicationDbContext>, CustomDbContextFactory<IApplicationDbContext>>();
builder.Services.AddScoped<IUserFileRepository, UserFileRepository>();
//------


builder.Services.AddScoped<IAuthentService, AuthentService>();

builder.Services.AddBlazoredLocalStorage();


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

//--
app.UseAuthentication();
app.UseAuthorization();
//--

app.MapDefaultControllerRoute();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
