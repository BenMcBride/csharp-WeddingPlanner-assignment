#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
[NotMapped]
public class LoginUser
{
  [Required(ErrorMessage = "The Email field is Required")]
  public string LoginEmail { get; set; }
  [Required(ErrorMessage = "The Password field is Required")]
  public string LoginPassword { get; set; }
}