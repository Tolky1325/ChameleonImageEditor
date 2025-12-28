using System.ComponentModel.DataAnnotations;

namespace ChameleonPhotoredactor.Models.ViewModels.Account
{
    public class UpdateProfileViewModel
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Display Name must be between 3 and 50 characters.")]
        [Display(Name = "Display Name")]
        public string NewDisplayName { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email format.")]
        [Display(Name = "Email")]
        public string NewEmail { get; set; }
    }
}