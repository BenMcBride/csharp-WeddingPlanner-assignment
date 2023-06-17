#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace WeddingPlanner.Models;
public class Wedding
{
  [Key]
  public int WeddingId { get; set; }
  [Required]
  [MinLength(2, ErrorMessage = "must be at least 2 characters.")]
  [MaxLength(30, ErrorMessage = "must be at most 30 characters.")]
  public string WedderOne { get; set; }
  [Required]
  [MinLength(2, ErrorMessage = "must be at least 2 characters.")]
  [MaxLength(30, ErrorMessage = "must be at most 30 characters.")]
  public string WedderTwo { get; set; }
  [DisplayFormat(DataFormatString = "{0:MMMM d, yyyy}")]
  [FutureDate(ErrorMessage = "Date must be in the future.")]
  public DateTime? Date { get; set; }
  [Required]
  public string Address { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.Now;
  public DateTime UpdatedAt { get; set; } = DateTime.Now;
  public List<RSVP> AllRsvps { get; set; } = new List<RSVP>();
  public int UserId { get; set; }
  public User? Creator { get; set; }
}
public class FutureDateAttribute : ValidationAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (value is null)
    {
      return new ValidationResult("Date is required.");
    }
    DateTime date = (DateTime)value;

    if (date.Date < DateTime.Now.Date)
    {
      return new ValidationResult("Date must be in the future.");
    }

    return ValidationResult.Success;
  }
}