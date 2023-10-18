using System.ComponentModel.DataAnnotations;

namespace FlightBooking.DTOs
{
    public class RegisterDto
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "The first name field should have a maximum of 255 characters")]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "The last name field should have a maximum of 255 characters")]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Emmail")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "ConfirmPassword")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "DateOfBirth")]
        public DateOnly DateOfBirth { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "Country")]
        public string Country { get; set; }
    }
}
