﻿using GalaxyCinemaBackEnd.Data;
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
                            IsPlaying = mov.ReleaseDate < DateTime.Now,
                            Casts = mov.Casts,
                            Writer = mov.Writer
                        }).FirstOrDefaultAsync();

            if (movie == null)
            {

                return NotFound("Movie not found!");
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
        public async Task<ActionResult<GetScheduleResponse>> GetMovieSchedule(int scheduleID)
        {

            var movieSchedule = await (from sch in _db.Schedule
                                       join mov in _db.Movie on sch.MovieID equals mov.MovieID
                                       join st in _db.Studio on sch.StudioID equals st.StudioID
                                       join stype in _db.StudioType on st.StudioTypeID equals stype.StudioTypeId
                                       join sp in _db.StudioPrice on st.StudioID equals sp.StudioID
                                       where sch.ScheduleID == scheduleID
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
    }
}
