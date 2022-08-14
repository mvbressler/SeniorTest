using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SeniorTest.Api.ActionFilters;
using SeniorTest.Api.Repositories;
using SeniorTest.DataModel.Models;
using SfFileService.FileManager.Base;
using SfFileService.FileManager.PhysicalFileProvider;
using Microsoft.EntityFrameworkCore;
using SeniorTest.Api.Utilities;
using SeniorTest.Core.Repositories;
using SeniorTest.Core.Utilities;

namespace SeniorTest.Controllers
{
    //[UpdateUserFileActionFilter]
    [Authorize(Roles = "User")]
    //[EnableCors("AllowAllOrigins")]
    //[EnableCors("_MyAllowSubdomainPolicy")]
    //[EnableCors(origins: "https://localhost*", headers: "*", methods: "*", SupportsCredentials = true)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    public class FileManagerController : Controller
    {
        public PhysicalFileProvider operation;
        public string basePath;
        string root = "wwwroot\\Files";
        private readonly IUserFileRepository _userFileRepository;
        private readonly SignInManager<IdentityUser> _signInManager;
        private IdentityUser _user;
        
        

        public FileManagerController(IHostEnvironment hostingEnvironment, IUserFileRepository userFileRepository, 
            SignInManager<IdentityUser> signInManager)
        {
            _userFileRepository = userFileRepository;
            _signInManager = signInManager;
            this.basePath = hostingEnvironment.ContentRootPath;
            this.operation = new PhysicalFileProvider();
			if(this.basePath.EndsWith("\\"))
             this.operation.RootFolder(this.basePath + this.root);
		    else
			 this.operation.RootFolder(this.basePath + "\\" + this.root);
            
        }
        
        [Microsoft.AspNetCore.Mvc.Route("FileOperations")]
        public async Task<object> FileOperations([Microsoft.AspNetCore.Mvc.FromBody] FileManagerDirectoryContent args)
        {
            _user = await _signInManager.UserManager.FindByEmailAsync(User.Identity?.Name);
            
            UpdateUserFiles.SetUserFilesRepository(_userFileRepository, _user);
            
            var response = new FileManagerResponse();
            
            if (args.Action == "delete" || args.Action == "rename")
            {
                if ((args.TargetPath == null) && (args.Path == ""))
                {
                    response = new FileManagerResponse();
                    response.Error = new ErrorDetails { Code = "401", Message = "Restricted to modify the root folder." };
                    return this.operation.ToCamelCase(response);
                }
            }
            switch (args.Action)
            {
                case "read":
                    try
                    {
                        response = this.operation.GetFiles(args.Path, args.ShowHiddenItems);
                        response.Files = await UpdateUserFiles.Read(args.Path, response.Files);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    return this.operation.ToCamelCase(response);
                case "delete":
                    // deletes the selected file(s) or folder(s) from the given path.
                    try
                    {
                        response = this.operation.Delete(args.Path, args.Names);
                        await UpdateUserFiles.Delete(args.Path, response.Files );
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    return this.operation.ToCamelCase(response);
                case "copy":
                    // copies the selected file(s) or folder(s) from a path and then pastes them into a given target path.
                    try
                    {
                        response = this.operation.Copy(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData);
                        await UpdateUserFiles.Copy(args.Path, args.TargetPath, response.Files);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    return this.operation.ToCamelCase(response);
                case "move":
                    // cuts the selected file(s) or folder(s) from a path and then pastes them into a given target path.
                    try
                    {
                        response = this.operation.Move(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData);
                        await UpdateUserFiles.Move(args.Path, args.TargetPath, response.Files);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    
                    return this.operation.ToCamelCase(response);
                case "details":
                    // gets the details of the selected file(s) or folder(s).
                    return this.operation.ToCamelCase(this.operation.Details(args.Path, args.Names, args.Data));
                case "create":
                    // creates a new folder in a given path.
                    return this.operation.ToCamelCase(this.operation.Create(args.Path, args.Name));
                case "search":
                    // gets the list of file(s) or folder(s) from a given path based on the searched key string.
                    return this.operation.ToCamelCase(this.operation.Search(args.Path, args.SearchString, args.ShowHiddenItems, args.CaseSensitive));
                case "rename":
                    // renames a file or folder.
                    response = this.operation.Rename(args.Path, args.Name, args.NewName);
                    UpdateUserFiles.Rename(args.Path, args.Name, args.NewName);
                    return this.operation.ToCamelCase(response);
            }
            return null;
        }

        // uploads the file(s) into a specified path
        [Microsoft.AspNetCore.Mvc.Route("Upload")]
        public async Task<IActionResult> Upload(string path, IList<IFormFile> uploadFiles, string action)
        {
            _user = await _signInManager.UserManager.FindByEmailAsync(User.Identity?.Name);
            try
            {
                UpdateUserFiles.SetUserFilesRepository(_userFileRepository, _user);
            
                FileManagerResponse uploadResponse;
                uploadResponse = operation.Upload(path, uploadFiles, action, null);
                if (uploadResponse.Error != null)
                {
                   Response.Clear();
                   Response.ContentType = "application/json; charset=utf-8";
                   Response.StatusCode = Convert.ToInt32(uploadResponse.Error.Code);
                   Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = uploadResponse.Error.Message;
                }

               // await Task.Run(async () =>
                //{
                    
                    await UpdateUserFiles.Upload(path, uploadFiles);
                    //_semaphoreSlim.Release();
                //});
                
                
                
                var a = "a";


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

            return Content("");
        }

        // downloads the selected file(s) and folder(s)
        [AllowAnonymous]
        [Route("Download")]
        public async Task<IActionResult> Download(string downloadInput)
        {
            _user = await _signInManager.UserManager.FindByEmailAsync(User.Identity?.Name);
            
            UpdateUserFiles.SetUserFilesRepository(_userFileRepository, _user);
            
            var args = JsonConvert.DeserializeObject<FileManagerDirectoryContent>(downloadInput);
            return operation.Download(args.Path, args.Names, args.Data);
        }

        // gets the image(s) from the given path
        [AllowAnonymous]
        [Route("GetImage")]
        public IActionResult GetImage(FileManagerDirectoryContent args)
        {
            return this.operation.GetImage(args.Path, args.Id,false,null, null);
        }       
    }
}
