using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class FilterStatisticDTO
    {
        [Required]
        [StringLength(255)]
        public string Year { get; set; }
        [Required]
        [StringLength(255)]
        public string Month { get; set; }
        [Required]
        [StringLength(255)]
        public string CourtClusterId { get; set; }
    }
}