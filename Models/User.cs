#pragma warning disable CS8618
#pragma warning disable CS8600
#pragma warning disable CS8602
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models;
public class User
{
  [Key]
  public int UserId { get; set; }
  [Required]
  [MinLength(2, ErrorMessage = "must be at least 2 characters.")]
  [MaxLength(20, ErrorMessage = "must be at most 30 characters.")]
  public string FirstName { get; set; }
  [Required]
  [MinLength(2, ErrorMessage = "must be at least 2 characters.")]
  [MaxLength(20, ErrorMessage = "must be at most 30 characters.")]
  public string LastName { get; set; }
  [Required]
  [EmailAddress]
  [UniqueEmail]
  public string Email { get; set; }
  [Required]
  [DataType(DataType.Password)]
  [MinLength(8, ErrorMessage = "must be at least 8 characters.")]
  public string Password { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime UpdatedAt { get; set; } = DateTime.Now;
  [NotMapped]
  [Compare("Password")]
  public string ConfirmPassword { get; set; }
  public List<RSVP> AllRsvps { get; set; } = new List<RSVP>(); // not always necessary.
}
public class UniqueEmailAttribute : ValidationAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    // Though we have Required as a validation, sometimes we make it here anyways
    // In which case we must first verify the value is not null before we proceed
    if (value == null)
    {
      // If it was, return the required error
      return new ValidationResult("Email is required!");
    }

    // This will connect us to our database since we are not in our Controller
    MyContext db = (MyContext)validationContext.GetService(typeof(MyContext));
    // Check to see if there are any records of this email in our database
    if (db.Users.Any(e => e.Email == value.ToString()))
    {
      // If yes, throw an error
      return new ValidationResult("Email must be unique!");
    }
    else
    {
      // If no, proceed
      return ValidationResult.Success;
    }
  }
}