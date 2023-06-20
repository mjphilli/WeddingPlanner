#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models;

public class Wedding
{
    [Key]
    public int WeddingId { get; set; }

    [Required(ErrorMessage = "is required")]
    public string WedderOne { get; set; }

    [Required(ErrorMessage = "is required")]
    public string WedderTwo { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Date")]
    [Future]
    public DateTime WeddingDate { get; set; }

    [Required(ErrorMessage = "is required")]
    // [EmailAddress]
    // [UniqueEmail]
    public string Address { get; set; }

    // [Required(ErrorMessage = "is required")]
    // [MinLength(8, ErrorMessage = "must be at least 8 characters.")]
    // [DataType(DataType.Password)]
    // public string Password { get; set; }

    // [NotMapped]
    // [DataType(DataType.Password)]
    // [Compare("Password", ErrorMessage = "must match password.")]
    // public string ConfirmPassword { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public List<Association> AllAssociations { get; set; } = new List<Association>();
}

public class FutureAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Though we have Required as a validation, sometimes we make it here anyways
        // In which case we must first verify the value is not null before we proceed
        if (value == null)
        {
            // If it was, return the required error
            return new ValidationResult("Must provide a wedding date!");
        }

        // This will connect us to our database since we are not in our Controller

        // Check to see if there are any records of this email in our database
        if ((DateTime)value <= DateTime.Now.Date)
        {
            // If yes, throw an error
            return new ValidationResult("Wedding must be in the future!");
        }
        else
        {
            // If no, proceed
            return ValidationResult.Success;
        }
    }
}