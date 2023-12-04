using GalaxyCinemaBackEnd.Data;
using GalaxyCinemaBackEnd.Models.GalaxyCinemaDB;
using GalaxyCinemaBackEnd.Models.Request;
using GalaxyCinemaBackEnd.Output;
using Microsoft.EntityFrameworkCore;

namespace GalaxyCinemaBackEnd.Services
{
    public interface IBookingPaymentService
    {
        Task<APIResponse<object>> AddBooking(AddBookingRequest req);
        Task<APIResponse<object>> AddPayment(AddPaymentRequest req);
    }
    public class BookingPaymentService: IBookingPaymentService
    {
        private readonly ApplicationDbContext _db;

        public BookingPaymentService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<APIResponse<object>> AddBooking(AddBookingRequest req)
        {
            if (req.userID == null || req.ScheduleID == null || req.seatID == null)
            {
                var errorResponse = new APIResponse<object>
                {
                    Status = 400,
                    Message = "Property should not be null!",
                    Data = null
                };
                return errorResponse;
            }
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var unavailSeatSelected = await (from bh in _db.BookingHeader
                                                     join bd in _db.BookingDetail on bh.BookingHeaderID equals bd.BookingHeaderID
                                                     where bh.ScheduleID == req.ScheduleID && req.seatID.Contains(bd.StudioSeatID)
                                                     select bd.StudioSeatID).ToListAsync();

                    if (unavailSeatSelected.Any())
                    {
                        var errorResponse = new APIResponse<object>
                        {
                            Status = 409,
                            Message = "One or more seats are already taken.",
                            Data = null
                        };

                        return errorResponse;
                    }

                    BookingHeader newBookingHeader = new BookingHeader
                    {
                        UserID = req.userID,
                        ScheduleID = req.ScheduleID,
                        BookingDate = DateTime.Now
                    };

                    _db.BookingHeader.Add(newBookingHeader);
                    await _db.SaveChangesAsync();



                    var bookingHeaderID = newBookingHeader.BookingHeaderID;
                    List<BookingDetail> bookingDetailList = new List<BookingDetail>();

                    foreach (int number in req.seatID)
                    {
                        BookingDetail newBookingDetail = new BookingDetail
                        {
                            BookingHeaderID = bookingHeaderID,
                            StudioSeatID = number
                        };
                        bookingDetailList.Add(newBookingDetail);
                    }

                    _db.BookingDetail.AddRange(bookingDetailList);
                    await _db.SaveChangesAsync();
                    transaction.Commit();

                    var response = new APIResponse<object>
                    {
                        Status = 200,
                        Message = "Success",
                        Data = bookingHeaderID
                    };

                    return response;
                }
                catch (Exception ex)
                {
                    var errorResponse = new APIResponse<object>
                    {
                        Status = 500,
                        Message = "Internal Server Error",
                        Data = null
                    };
                    return errorResponse;
                }
            }

        }
        
        public async Task<APIResponse<object>> AddPayment(AddPaymentRequest req)
        {
            try
            {
                if (req.BookingHeaderID == null || req.ScheduleID == null || req.SeatQty == null)
                {
                    var errorResponse = new APIResponse<object>
                    {
                        Status = 400,
                        Message = "Booking is not successfully created or passed",
                        Data = null
                    };
                    return errorResponse;
                }

                //get price
                var movieTimeStart = await (from schedule in _db.Schedule
                                        where schedule.ScheduleID == req.ScheduleID
                                        select schedule.TimeStart).FirstOrDefaultAsync();

                TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
                DayOfWeek dayOfWeek = (TimeZoneInfo.ConvertTimeFromUtc(movieTimeStart, localTimeZone)).DayOfWeek;

                var day = "";
                if(dayOfWeek == DayOfWeek.Friday)
                {
                    day = "Friday";
                } else if (dayOfWeek == DayOfWeek.Sunday || dayOfWeek == DayOfWeek.Saturday)
                {
                    day = "Weekends";
                } else
                {
                    day = "Weekdays(without friday)";
                }

                var studioPrice = await (from schedule in _db.Schedule
                                            join studio in _db.Studio on schedule.StudioID equals studio.StudioID
                                            join sp in _db.StudioPrice on studio.StudioID equals sp.StudioID
                                            where schedule.ScheduleID == req.ScheduleID && sp.CategoryDays == day
                                            select sp.Price).FirstOrDefaultAsync();

                Payment newPayment = new Payment
                {
                    BookingHeaderID = req.BookingHeaderID,
                    Amount = req.SeatQty * studioPrice,
                    IsPaid = false,
                };

                _db.Payment.Add(newPayment);
                await _db.SaveChangesAsync();



                var response = new APIResponse<object>
                {
                    Status = 200,
                    Message = "Success",
                    Data = null
                };

                return response;
            }
            catch (Exception ex)
            {
                var errorResponse = new APIResponse<object>
                {
                    Status = 500,
                    Message = "Internal Server Error",
                    Data = null
                };
                return errorResponse;
            }
        }
    }
}
