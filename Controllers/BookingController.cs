using GalaxyCinemaBackEnd.Data;
using GalaxyCinemaBackEnd.Models.GalaxyCinemaDB;
using GalaxyCinemaBackEnd.Models.Request;
using GalaxyCinemaBackEnd.Models.Response;
using GalaxyCinemaBackEnd.Output;
using GalaxyCinemaBackEnd.Services;
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
        private readonly IBookingPaymentService _bookingPaymentService;


        public BookingController(ApplicationDbContext db, IBookingPaymentService bookingPaymentService)
        {
            _db = db;
            _bookingPaymentService = bookingPaymentService;
        }

        [HttpGet("getSeat")]
        public async Task<ActionResult<object>> GetSeat(int scheduleID)
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

        [HttpPost("add")]
        public async Task<ActionResult<object>> AddBooking([FromBody] AddBookingRequest req)
        {
            try
            {
                var responseAddBooking = await _bookingPaymentService.AddBooking(req);
 

                if (responseAddBooking.Status == 200)
                {
                    var newPaymentRequest = new AddPaymentRequest
                    {
                        BookingHeaderID = (int)responseAddBooking.Data,
                        ScheduleID = req.ScheduleID,
                        SeatQty = req.seatID.Count
                    };
                    var responseAddPayment = await _bookingPaymentService.AddPayment(newPaymentRequest);

                    return StatusCode(responseAddPayment.Status, responseAddPayment);
                }
                return StatusCode(responseAddBooking.Status, responseAddBooking);
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it appropriately
                    var errorResponse = new APIResponse<object>
                    {
                        Status = 500,
                        Message = "Internal Server Error",
                        Data = null
                    };
                    return StatusCode(errorResponse.Status, errorResponse);
                }

        }

        [HttpGet("getBookingHistory")]
        public async Task<ActionResult<IEnumerable<GetBookingHistoryResponse>>> GetBookingHistory(Guid userID)
        {
            var historyData = await (from bh in _db.BookingHeader
                              join s in _db.Schedule on bh.ScheduleID equals s.ScheduleID
                              join mov in _db.Movie on s.MovieID equals mov.MovieID
                              join st in _db.Studio on s.StudioID equals st.StudioID
                              where bh.UserID == userID
                              select new GetBookingHistoryResponse
                              {
                                  BookingHeaderId = bh.BookingHeaderID,
                                  StudioName = st.Name,
                                  MovieTitle = mov.Title,
                                  Poster = mov.Poster,
                                  BookingDate = bh.BookingDate,
                                  ScheduleTimeStart = s.TimeStart
                              }).ToListAsync();

            var response = new APIResponse<IEnumerable<GetBookingHistoryResponse>>
            {
                Status = 200,
                Message = "Success",
                Data = historyData
            };

            return Ok(response);
        }
    }
}
