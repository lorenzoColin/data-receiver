using System.ComponentModel.DataAnnotations;

namespace data_receiver.Models.ViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "password and confirmPassword are not matching")]
        public string confirmPassword { get; set; }

    }
}
