using GalaxyCinemaBackEnd.Data;
using GalaxyCinemaBackEnd.Models.GalaxyCinemaDB;
using GalaxyCinemaBackEnd.Models.Request;
using GalaxyCinemaBackEnd.Models.Response;
using GalaxyCinemaBackEnd.Output;
using GalaxyCinemaBackEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyCinemaBackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _db;


        public PaymentController(ApplicationDbContext db)
        {
            _db = db;
        }

    }
}
