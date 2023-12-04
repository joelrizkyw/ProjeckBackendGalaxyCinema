using GalaxyCinemaBackEnd.Data;
using GalaxyCinemaBackEnd.Models.GalaxyCinemaDB;
using GalaxyCinemaBackEnd.Models.Request;
using GalaxyCinemaBackEnd.Models.Response;
using GalaxyCinemaBackEnd.Output;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
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
                                    ReleaseDate = mov.ReleaseDate,
                                    Casts = mov.Casts,
                                    Writer = mov.Writer,
                                    Rating = mov.Rating
                                }).ToListAsync();

            var response = new APIResponse<IEnumerable<GetMovieResponse>>
            {
                Status = 200,
                Message = "success",
                Data = movieList
            };

            return Ok(response);
        }

        [HttpGet("getMovieDetail")]
        public async Task<ActionResult<GetMovieResponse>> GetMovieDetail(int movieID)
        {

            var movie = await (from mov in _db.Movie
                        where mov.MovieID == movieID
                        select new GetMovieResponse
                        {
                            Id = mov.MovieID,
                            Title = mov.Title,
                            Director = mov.Director,
                            Poster = mov.Poster,
                            Synopsis = mov.Synopsis,
                            Duration = mov.Duration,
                            ReleaseDate = mov.ReleaseDate,
                            Casts = mov.Casts,
                            Writer = mov.Writer,
                            Rating = mov.Rating,
                        }).FirstOrDefaultAsync();

            if (movie == null)
            {
                var errorResponse = new APIResponse<object>
                {
                    Status = 404,
                    Message = "Movie Not Found",
                    Data = null
                };

                return NotFound(errorResponse);
            }

            var response = new APIResponse<GetMovieResponse>
            {
                Status = 200,
                Message = "Success",
                Data = movie
            };

            return Ok(response);
        }

        [HttpGet("getMovieSchedule")]
        public async Task<ActionResult<IEnumerable<GetScheduleResponse>>> GetMovieSchedule(int movieID)
        {

            var movieTimeStart = await (from sch in _db.Schedule
                                        where sch.MovieID == movieID
                                        select sch.TimeStart).ToListAsync();

            var movieDayTypes = movieTimeStart.Select(date =>
            {
                if (date.DayOfWeek == DayOfWeek.Friday)
                {
                    return "Friday";
                }

                else if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    return "Weekends";
                }

                else
                {
                    return "Weekdays(without friday)";
                }
            }).ToList();

            var uniqueMovieDayTypes = movieDayTypes.Distinct().ToList();

            // Debug.WriteLine("Isi list = " + string.Join(", ", uniqueMovieDayTypes));

            var movieSchedule = await (from sch in _db.Schedule
                                       join mov in _db.Movie on sch.MovieID equals mov.MovieID
                                       join st in _db.Studio on sch.StudioID equals st.StudioID
                                       join stype in _db.StudioType on st.StudioTypeID equals stype.StudioTypeId
                                       join sp in _db.StudioPrice on st.StudioID equals sp.StudioID
                                       where sch.MovieID == movieID && uniqueMovieDayTypes.Contains(sp.CategoryDays)
                                       select new GetScheduleResponse
                                       {
                                           ScheduleId = sch.ScheduleID,
                                           MovieId = sch.MovieID,
                                           StudioId = sch.StudioID,
                                           StudioTypeId = st.StudioTypeID,
                                           TimeStart = sch.TimeStart,
                                           StudioTypeName = stype.Name,
                                           Price = sp.Price,
                                           Rating = mov.Rating
                                       }).ToListAsync();

            var response = new APIResponse<IEnumerable<GetScheduleResponse>>
            {
                Status = 200,
                Message = "Success",
                Data = movieSchedule
            };

            return Ok(response);
        }

        [HttpPost("addMovie")]
        public async Task<IActionResult> AddMovie([FromBody] MovieBodyRequest req)
        {
            var movie = new Movie
            {
                Title = req.Title,
                Director = req.Director,
                Poster = req.Poster,
                Synopsis = req.Synopsis,
                Duration = req.Duration,
                ReleaseDate = req.ReleaseDate,
                Casts = req.Casts,
                Writer = req.Writer,
                Rating = req.Rating
            };

            _db.Movie.Add(movie);
            await _db.SaveChangesAsync();

            var response = new APIResponse<object>
            {
                Status = 200,
                Message = "Success",
                Data = null
            };

            return Ok(response);
        }

        [HttpPut("updateMovie")]
        public async Task<IActionResult> UpdateMovie(int movieID, [FromBody] MovieBodyRequest req)
        {
            var movie = await (from mov in _db.Movie
                               where mov.MovieID == movieID
                               select mov).FirstOrDefaultAsync();

            if (movie == null)
            {
                var errorResponse = new APIResponse<object>
                {
                    Status = 404,
                    Message = "Movie Not Found",
                    Data = null
                };

                return NotFound(errorResponse);
            }

            movie.Title = req.Title;
            movie.Director = req.Director;
            movie.Poster = req.Poster;
            movie.Synopsis = req.Synopsis;
            movie.Duration = req.Duration;
            movie.ReleaseDate = req.ReleaseDate;
            movie.Casts = req.Casts;
            movie.Writer = req.Writer;
            movie.Rating = req.Rating;

            await _db.SaveChangesAsync();

            var response = new APIResponse<object>
            {
                Status = 200,
                Message = "Success",
                Data = null
            };

            return Ok(response);
        }

        [HttpDelete("deleteMovie")]
        public async Task<IActionResult> DeleteMovie(int movieID)
        {

            var movie = await (from mov in _db.Movie
                               where mov.MovieID == movieID
                               select mov).FirstOrDefaultAsync();

            if (movie == null)
            {
                var errorResponse = new APIResponse<object>
                {
                    Status = 404,
                    Message = "Movie Not Found",
                    Data = null
                };

                return NotFound(errorResponse);
            }

            _db.Movie.Remove(movie);
            await _db.SaveChangesAsync();

            var response = new APIResponse<object>
            {
                Status = 200,
                Message = "Success",
                Data = null
            };

            return Ok(response);
        }
    }
}
