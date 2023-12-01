using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GalaxyCinemaBackEnd.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }
        [Required]

        public int BookingHeaderID { get; set; }

        [Required]
        public int Amount { get; set; }
        [Required]

        public bool IsPaid { get; set; } 

    }
}
