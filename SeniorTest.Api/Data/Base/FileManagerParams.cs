using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace SfFileService.FileManager.Base
{
    public class FileManagerParams
    {
        public string Name { get; set; }

        public string[] Names { get; set; }

        public string Path { get; set; }

        public string TargetPath { get; set; }

        public string NewName { get; set; }

        public object Date { get; set; }
#if EJ2_DNX
        public IEnumerable<System.Web.HttpPostedFileBase> FileUpload { get; set; }
#else
        public IEnumerable<IFormFile> FileUpload { get; set; }

#endif
        public string[] RenameFiles { get; set; }
    }
}