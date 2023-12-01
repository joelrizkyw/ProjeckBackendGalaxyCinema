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

    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public BookingController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("getSeat")]
        public async Task<ActionResult<object>> Get(int scheduleID)
        {
            var unavailableSeat = await (from bh in _db.BookingHeader
                                         join bd in _db.BookingDetail on bh.BookingHeaderID equals bd.BookingHeaderID
                                         where bh.ScheduleID == scheduleID
                                         select bd.StudioSeatID).ToArrayAsync();

            var studioSeats = await (from st in _db.Studio
                                    join ss in _db.StudioSeat on st.StudioID equals ss.StudioID
                                    join sc in _db.Schedule on st.StudioID equals sc.StudioID
                                    where sc.ScheduleID == scheduleID
                                    select new
                                    {
                                        StudioSeatID = ss.StudioSeatID,
                                        StudioID = ss.StudioID,
                                        Name = ss.Name
                                    }).ToListAsync();

            var getSeatResponse = new GetSeatResponse
            {
                Data = studioSeats
                        .GroupBy(ss => ss.Name.Substring(0, 1))
                        .Select(rowSeatDetail => new RowSeatDetail
                        {
                            Row = rowSeatDetail.Key,
                            SeatDetail = rowSeatDetail.Select(seatDetail => new SeatDetail
                            {
                                Id = seatDetail.StudioSeatID,
                                SeatName = seatDetail.Name,
                                IsAvail = !unavailableSeat.Contains(seatDetail.StudioSeatID)
                            }).ToList()
                        }).ToList()

            };


        var response = new APIResponse<object>
            {
                Status = 200,
                Message = "Success",
                Data = getSeatResponse.Data
            };

            return Ok(response);
        }
    }
}
