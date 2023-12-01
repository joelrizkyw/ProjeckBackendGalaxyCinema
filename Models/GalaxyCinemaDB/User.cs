using System.ComponentModel.DataAnnotations;

namespace GalaxyCinemaBackEnd.Models.GalaxyCinemaDB
{
    public class User
    {
        [Key]
        public Guid UserID { get; set; }

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
        [RegularExpression("^(Male|Female)$", ErrorMessage = "Invalid gender.")]
        public string Gender { get; set; }

        [Required]
        public DateTime Birthdate { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
