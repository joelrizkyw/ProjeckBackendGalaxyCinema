using System.ComponentModel.DataAnnotations;

namespace GalaxyCinemaBackEnd.Models.GalaxyCinemaDB
{
    public class User
    {
        [Key]
        public string UserID { get; set; }

        [Required]
        [RegularExpression("^(User|Admin)$", ErrorMessage = "Invalid role.")]
        public string Role { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
