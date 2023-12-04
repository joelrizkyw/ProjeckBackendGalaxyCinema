using GalaxyCinemaBackEnd.Data;
using GalaxyCinemaBackEnd.Models.GalaxyCinemaDB;
using GalaxyCinemaBackEnd.Models.Request;
using GalaxyCinemaBackEnd.Models.Response;
using GalaxyCinemaBackEnd.Output;
using GalaxyCinemaBackEnd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("getPaymentDetail")]
        public async Task<ActionResult<object>> GetPaymentDetail(int bookingHeaderId)
        {
            try
            {
                var allData = await (
                    from payment in _db.Payment
                    join bh in _db.BookingHeader on payment.BookingHeaderID equals bh.BookingHeaderID
                    join bd in _db.BookingDetail on bh.BookingHeaderID equals bd.BookingHeaderID
                    join ss in _db.StudioSeat on bd.StudioSeatID equals ss.StudioSeatID
                    join schedule in _db.Schedule on bh.ScheduleID equals schedule.ScheduleID
                    join studio in _db.Studio on schedule.StudioID equals studio.StudioID
                    join movie in _db.Movie on schedule.MovieID equals movie.MovieID
                    where bh.BookingHeaderID == bookingHeaderId
                    select new
                    {
                        paymentID = payment.PaymentID,
                        timestart = schedule.TimeStart,
                        studioseats = ss.Name,
                        amount = payment.Amount,
                        studioName = studio.Name,
                        movieName = movie.Title
                    }
                ).ToListAsync();

                var studioSeats = allData.Select(item => item.studioseats).Distinct().ToArray();

                var response = new APIResponse<object>
                {
                    Status = 200,
                    Message = "Success",
                    Data = allData.Any() ? new
                    {
                        paymentID = allData[0].paymentID,
                        movieName = allData[0].movieName,
                        studio = allData[0].studioName,
                        amount = allData[0].amount,
                        timestart = allData[0].timestart,
                        studioSeats = studioSeats
                    } : null
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                var response = new APIResponse<object>
                {
                    Status = 500,
                    Message = "Internal Server Error",
                    Data = null
                };

                // Log the exception or handle it as needed
                return StatusCode(500, response);
            }
        }


        [HttpPut("addPayment")]
        public async Task<ActionResult<object>> AddPayment(int paymentID)
        {

            try
            {
                var paymentData = await (
                    from payment in _db.Payment
                    where payment.PaymentID == paymentID
                    select payment
                ).FirstOrDefaultAsync();

                if (paymentData != null)
                {
                    paymentData.IsPaid = true;
                    await _db.SaveChangesAsync();

                    var response = new APIResponse<object>
                    {
                        Status = 200,
                        Message = "Success",
                        Data = null
                    };

                    return Ok(response);
                }
                else
                {
                    var response = new APIResponse<object>
                    {
                        Status = 404,
                        Message = "Payment not found",
                        Data = null
                    };

                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                var response = new APIResponse<object>
                {
                    Status = 500,
                    Message = "Internal Server Error",
                    Data = null
                };

                // Log the exception or handle it as needed
                return StatusCode(500, response);
            }
        }

    }
}
