using Azure;
using GalaxyCinemaBackEnd.Data;
using GalaxyCinemaBackEnd.Models.GalaxyCinemaDB;
using GalaxyCinemaBackEnd.Models.Request;
using GalaxyCinemaBackEnd.Models.Response;
using GalaxyCinemaBackEnd.Output;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GalaxyCinemaBackEnd.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IConfiguration configuration;
        private readonly ApplicationDbContext _db;
        public AuthController(IConfiguration configuration, ApplicationDbContext db)
        {
            this.configuration = configuration;
            _db = db;
        }

        private string RegistrationValidation(AuthRequest newUser)
        {
            string emailRegexPattern = @"^\S+@\S+\.\S+$";
            string passwordRegexPattern = @"^(?=.*[0-9])(?=.*[A-Z])(?=.*[\W_]).{8,}$";  

            if (string.IsNullOrEmpty(newUser.Name))
            {
                return "Please input a valid name";
            }
            if (string.IsNullOrEmpty(newUser.PhoneNumber))
            {
                return "Please input a valid phone number";
            }
            if (string.IsNullOrEmpty(newUser.Email) || !System.Text.RegularExpressions.Regex.IsMatch(newUser.Email, emailRegexPattern))
            {
                return "Please input a valid email";
            }
            if (string.IsNullOrEmpty(newUser.Gender))
            {
                return "Please input a valid gender";
            }
            if (newUser.Birthdate == default(DateTime))
            {
                return "Please input a valid birth date";
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(newUser.Password, passwordRegexPattern))
            {
                return "Your password should consist of a minimum of 8 characters and include at least one uppercase letter, one special character, and one number.";
            }
            return "Success";
        }

        private string LoginValidation(LoginRequest req)
        {

            if (string.IsNullOrEmpty(req.PhoneNumber))
            {
                return "Please input a valid phone Number";
            }
            if (string.IsNullOrEmpty(req.Password))
            {
                return "Your password should consist of a minimum of 8 characters and include at least one uppercase letter, one special character, and one number.";
            }
            return "Success";
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody] AuthRequest req)
        {
            var registerValidation = RegistrationValidation(req);

            if (registerValidation != "Success")
            {
                var response = new APIResponse<object>
                {
                    Status = 400,
                    Message = registerValidation,
                    Data = null
                };
                return BadRequest(response);
            }
            var userExisted = _db.User.FirstOrDefault(x => x.Email == req.Email);
            if (userExisted != null)
            {
                var response = new APIResponse<object>
                {
                    Status = 400,
                    Message = "Email is already used!",
                    Data = null
                };
                return BadRequest(response);
            }

            User user = new()
            {
                UserID = Guid.NewGuid(),
                Role = "User",
                Name = req.Name,
                Email = req.Email,
                PhoneNumber = req.PhoneNumber,
                Gender = req.Gender,
                Birthdate = req.Birthdate,  
                Password = BCrypt.Net.BCrypt.HashPassword(req.Password)

            };

            _db.User.Add(user);
            _db.SaveChanges();
            var successResponse = new APIResponse<object>
            {
                Status=200, 
                Message = "Success",
                Data = null
            };
            return Ok(successResponse);

        }




        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            var loginValidation = LoginValidation(req);

            if (loginValidation != "Success")
            {
                var response = new APIResponse<object>
                {
                    Status = 400,
                    Message = loginValidation,
                    Data = null
                };
                return BadRequest(response);
            }
            var user = _db.User.FirstOrDefault(x => x.PhoneNumber == req.PhoneNumber);

            if (user == null)
            {
                var response = new APIResponse<object>
                {
                    Status = 404,
                    Message = "User not found!",
                    Data = null
                };
                return NotFound(response);
            }
            else if (!BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
            {
                var response = new APIResponse<object>
                {
                    Status = 400,
                    Message = "Password incorrect!",
                    Data = null
                };
                return BadRequest(response);
            }

            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
            var signingCredentials = new SigningCredentials(
                                    new SymmetricSecurityKey(key),
                                    SecurityAlgorithms.HmacSha512Signature
                                );

            var subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            });

            var expires = DateTime.UtcNow.AddMinutes(30);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                Expires = expires,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = signingCredentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            HttpContext.Response.Cookies.Append("token", jwtToken,
                new CookieOptions
                {
                    Expires = expires,
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None
                });

            var loginResponse = new LoginResponse
            {
                Status = 200,
                Message = "Success",
                Token = jwtToken,
                TokenExpiration = expires,
                User = new UserDetail
                {
                    Id = user.UserID,
                    Name = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    Birthdate = user.Birthdate
                }

            };


            return Ok(loginResponse);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

      
            Response.Cookies.Delete("token", new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddYears(-1),
                SameSite = SameSiteMode.None,
                Secure = true
            });

            return Ok(new { message = "Logout successful" });
        }
    }
}
