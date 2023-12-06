using GalaxyCinemaBackEnd.Data;
using GalaxyCinemaBackEnd.Models.GalaxyCinemaDB;
using GalaxyCinemaBackEnd.Models.Request;
using GalaxyCinemaBackEnd.Models.Response;
using GalaxyCinemaBackEnd.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GalaxyCinemaBackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment environment;
        public MovieController(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            this.environment = environment;
        }

        [HttpGet("getMovies")]
        public async Task<ActionResult<IEnumerable<GetMovieResponse>>> Get(string status)
        {
            List<GetMovieResponse> movieList = null;
            string imagePath = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/images/poster/";
            
            if (status == "now_playing")
            {

                movieList = await (from mov in _db.Movie
                                       where mov.ReleaseDate < DateTime.Now
                                       select new GetMovieResponse
                                       {
                                           Id = mov.MovieID,
                                           Title = mov.Title,
                                           Director = mov.Director,
                                           Poster = $"{imagePath}{mov.Poster}",
                                           Synopsis = mov.Synopsis,
                                           Duration = mov.Duration,
                                           ReleaseDate = mov.ReleaseDate,
                                           Casts = mov.Casts,
                                           Writer = mov.Writer,
                                           Rating = mov.Rating
                                       }).ToListAsync();
            }

            else if (status == "upcoming")
            {

                movieList = await (from mov in _db.Movie
                                       where mov.ReleaseDate > DateTime.Now
                                       select new GetMovieResponse
                                       {
                                           Id = mov.MovieID,
                                           Title = mov.Title,
                                           Director = mov.Director,
                                           Poster = $"{imagePath}{mov.Poster}",
                                           Synopsis = mov.Synopsis,
                                           Duration = mov.Duration,
                                           ReleaseDate = mov.ReleaseDate,
                                           Casts = mov.Casts,
                                           Writer = mov.Writer,
                                           Rating = mov.Rating
                                       }).ToListAsync();
            }

            else
            {

                var errorResponse = new APIResponse<object>
                {
                    Status = 404,
                    Message = "Movie list not found. Please enter valid movie status!",
                    Data = null
                };

                return NotFound(errorResponse);
            }

            var response = new APIResponse<IEnumerable<GetMovieResponse>>
            {
                Status = 200,
                Message = "Success",
                Data = movieList
            };

            return Ok(response);
        }

        [HttpGet("getMovieDetail")]
        public async Task<ActionResult<GetMovieResponse>> GetMovieDetail(int movieID)
        {
            string imagePath = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/images/poster/";
            
            var movie = await (from mov in _db.Movie
                        where mov.MovieID == movieID
                        select new GetMovieResponse
                        {
                            Id = mov.MovieID,
                            Title = mov.Title,
                            Director = mov.Director,
                            Poster = $"{imagePath}{mov.Poster}",
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
                    Message = "Movie not found. Please enter valid movie ID!",
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
        public async Task<IActionResult> AddMovie([FromForm] AddMovieRequest req)
        {
            try
            {
                var movieTitleCamelCase = ToCamelCase(req.Title);
                var Filepath = GetFilepath();
                if (!Directory.Exists(Filepath))
                {
                    Directory.CreateDirectory(Filepath);
                }

                var fileExtension = Path.GetExtension(req.PosterImage.FileName);

                var fullImageName = $"{movieTitleCamelCase}{fileExtension}";
                var imagepath = Path.Combine(Filepath, fullImageName);

                var existingFiles = Directory.GetFiles(Filepath)
                                    .Where(filePath => Path.GetFileNameWithoutExtension(filePath) == movieTitleCamelCase);

                if (existingFiles.Any())
                {
                    var errorResponse = new APIResponse<object>
                    {
                        Status = 409,
                        Message = "A file with the same name already exists.",
                        Data = null
                    };

                    return Conflict(errorResponse);
                }

                var movie = new Movie
                {
                    Title = req.Title,
                    Director = req.Director,
                    Poster = fullImageName,
                    Synopsis = req.Synopsis,
                    Duration = int.Parse(req.Duration),
                    ReleaseDate = req.ReleaseDate,
                    Casts = req.Casts,
                    Writer = req.Writer,
                    Rating = req.Rating
                };

                _db.Movie.Add(movie);
                await _db.SaveChangesAsync();

                using (FileStream stream = System.IO.File.Create(imagepath))
                {
                    await req.PosterImage.CopyToAsync(stream);
                }

                var response = new APIResponse<object>
                {
                    Status = 200,
                    Message = "Success",
                    Data = null
                };

                return Ok(response);

            }
            catch (Exception ex)
            {
                var errorResponse = new APIResponse<object>
                {
                    Status = 500,
                    Message = "Internal Server Error",
                    Data = null
                };
                return StatusCode(errorResponse.Status, errorResponse);
            }

        }

        [HttpPut("updateMovie")]
        public async Task<IActionResult> UpdateMovie([FromForm] UpdateMovieRequest req)
        {
            try
            {
                var movie = await (from mov in _db.Movie
                                   where mov.MovieID == int.Parse(req.MovieID)
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

                var fullImageName = req.OldPoster;

                if(req.NewPoster != null)
                {
                    var oldMovieTitleCamelCase = ToCamelCase(req.OldTitle);
                    string Filepath = GetFilepath();

                    var existingFiles = Directory.GetFiles(Filepath)
                        .Where(filePath => Path.GetFileNameWithoutExtension(filePath) == oldMovieTitleCamelCase);

                    if (existingFiles.Any())
                    {
                        foreach (var filePath in existingFiles)
                        {
                            System.IO.File.Delete(filePath);
                        }

                    }

                    //add yang baru

                    var newMovieTitleCamelCase = ToCamelCase(req.NewTitle);
                    if (!Directory.Exists(Filepath))
                    {
                        Directory.CreateDirectory(Filepath);
                    }

                    var fileExtension = Path.GetExtension(req.NewPoster.FileName);

                    fullImageName = $"{newMovieTitleCamelCase}{fileExtension}";
                    var imagepath = Path.Combine(Filepath, fullImageName);

                    using (FileStream stream = System.IO.File.Create(imagepath))
                    {
                        await req.NewPoster.CopyToAsync(stream);
                    }
                }

               


                movie.Title = req.NewTitle;
                movie.Director = req.Director;
                movie.Poster = fullImageName;
                movie.Synopsis = req.Synopsis;
                movie.Duration = int.Parse(req.Duration);
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
            catch (Exception ex)
            {
                var errorResponse = new APIResponse<object>
                {
                    Status = 500,
                    Message = "Internal Server Error",
                    Data = null
                };
                return StatusCode(errorResponse.Status, errorResponse);
            }
        }

        [HttpDelete("deleteMovie/{movieID}")]
        public async Task<IActionResult> DeleteMovie(int? movieID)
        {

            try
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

                var movieTitleCamelCase = ToCamelCase(movie.Title);
                string Filepath = GetFilepath();

                var existingFiles = Directory.GetFiles(Filepath)
                    .Where(filePath => Path.GetFileNameWithoutExtension(filePath) == movieTitleCamelCase);

                if (existingFiles.Any())
                {
                    foreach (var filePath in existingFiles)
                    {
                        System.IO.File.Delete(filePath);
                    }

                }
                else
                {
                    var errorResponse = new APIResponse<object>
                    {
                        Status = 404,
                        Message = "Movie Poster Not Found.",
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
            catch (Exception ex)
            {
                var errorResponse = new APIResponse<object>
                {
                    Status = 500,
                    Message = "Internal Server Error",
                    Data = null
                };
                return StatusCode(errorResponse.Status, errorResponse);
            }

        }



        private string GetFilepath()
        {
            return this.environment.WebRootPath + "\\images\\poster\\";
        }

        private string ToCamelCase(string input)
        {

            string[] words = input.ToLower().Split(' ');
            var result = words[0];

            for (int i = 1; i < words.Length; i++)
            {
                var firstLetter = char.ToUpper(words[i][0]);
                var remainingWord = words[i].Substring(1);
                result = result + firstLetter + remainingWord;
            }

            return result;
        }
    }
}
