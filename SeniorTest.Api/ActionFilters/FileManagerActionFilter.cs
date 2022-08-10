using System.Net.Http.Headers;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
//using SeniorTest.Api.Data.Base;
using SeniorTest.Api.Repositories;
using SeniorTest.Core.Repositories;
using SeniorTest.Core.Utilities;
using SeniorTest.DataModel.Models;

namespace SeniorTest.Api.ActionFilters;

public class UpdateUserFileActionFilter: ActionFilterAttribute
{
    private IUserFileRepository _userFileRepository;
    private UserManager<IdentityUser> _userManager;
    private IdentityUser _user;

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Do something before the action executes.
       Console.WriteLine("FileManagerActionFilter On Before EXCECUTED");
       await next();
       // Do something after the action executes
        
       _userManager = context.HttpContext.RequestServices.GetService<UserManager<IdentityUser>>();

       if (!string.IsNullOrWhiteSpace(context.HttpContext.Request.Headers.Authorization))
       { 
           var token = AuthenticationHeaderValue.Parse(context.HttpContext.Request.Headers.Authorization) ;
           var _claims = JWTClaimHelper.ParseClaimsFromJwt(token.Parameter);  
           _user = await _userManager.FindByEmailAsync(_claims.FirstOrDefault(x=>x.Type == ClaimTypes.Name).Value);  
       }
       
       

       if (context.ActionDescriptor.AttributeRouteInfo.Template.Contains("FileOperations"))
       {
       //     var s = context.Controller.ToString();
       //     var obj = (FileManagerDirectoryContent) context.ActionArguments["args"]!;
       //     switch (obj.Action)
       //     {
       //         case "copy":
       //             //if (File.Exists())
       //             break;
       //         case "move":
       //             break;
       //         case "rename":
       //             //var path = 
       //             break;
       //         case "delete":
       //             break;
       //     }   
       }

       if (context.ActionDescriptor.AttributeRouteInfo.Template.Contains("Upload"))
       {
           // var path = (string)context.ActionArguments["path"];
           // var filenames = (List<IFormFile>)context.ActionArguments["uploadFiles"];
           // foreach (var file in filenames)
           // {
           //     _userFileRepository.CreateAsync(new UserFile()
           //         { Filename = file.FileName, Path = path, User = _user, IsZipped = false});
           // }
       }
       
       if (context.ActionDescriptor.AttributeRouteInfo.Template.Contains("Download"))
       {
        //do nothing    
       }

       Console.WriteLine($"FileManagerActionFilter After Action EXECUTED, Action: [{context.ActionDescriptor.AttributeRouteInfo.Template}]");
    }
}