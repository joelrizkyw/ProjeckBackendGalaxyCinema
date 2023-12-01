using System.ComponentModel.DataAnnotations;
namespace GalaxyCinemaBackEnd.Models.Request
{
    public class AuthRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public DateTime Birthdate { get; set; }
    }

    public class LoginRequest
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
