using GalaxyCinemaBackEnd.Data;
using GalaxyCinemaBackEnd.Models.GalaxyCinemaDB;
using GalaxyCinemaBackEnd.Models.Response;
using GalaxyCinemaBackEnd.Output;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GalaxyCinemaBackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public MovieController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("getMovies")]
        public async Task<ActionResult<IEnumerable<GetMovieResponse>>> Get()
        {

            var movieList = await (from mov in _db.Movie
                                select new GetMovieResponse
                                {
                                    Id = mov.MovieID,
                                    Title = mov.Title,
                                    Director = mov.Director,
                                    Poster = mov.Poster,
                                    Synopsis = mov.Synopsis,
                                    Duration = mov.Duration,
                                    IsPlaying = mov.ReleaseDate < DateTime.Now,
                                    Casts = mov.Casts,
                                    Writer = mov.Writer
                                }).ToListAsync();

            var response = new APIResponse<IEnumerable<GetMovieResponse>>
            {
                Status = 200,
                Message = "success",
                Data = movieList
            };

            return Ok(response);
        }
    }
}
