namespace GalaxyCinemaBackEnd.Models.Response
{
    public class LoginResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }

        public UserDetail User { get; set; }    
        

    }

    public class UserDetail
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }  
        public DateTime Birthdate { get; set; }
    }
}
