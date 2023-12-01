using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GalaxyCinemaBackEnd.Models
{
    public class StudioPrice
    {
        [Key]
        public int StudioPriceID { get; set; }
        [Required]

        public int StudioID { get; set; }

        [Required]
        [RegularExpression("^(Weekdays\\(without friday\\)|Friday|Weekends)$", ErrorMessage = "Invalid Category Days")]
        public string CategoryDays { get; set; }
        [Required]

        public int Price { get; set; }

    }
}
