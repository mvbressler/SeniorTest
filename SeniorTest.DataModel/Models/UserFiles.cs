using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SeniorTest.DataModel.Models;


[Table("UserFiles")]
public class UserFile 
{
    [ForeignKey("IdentityUser")] 
    public string UserId { get; set; }
    public string Path { get; set; }
    public string Filename { get; set; }
    [DefaultValue(false)]
    public bool IsZipped { get; set; }
    public virtual IdentityUser User { get; set; }

}