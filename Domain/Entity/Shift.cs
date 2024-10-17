using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class Shift : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string ShiftName { get; set; }
        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }
    }
}