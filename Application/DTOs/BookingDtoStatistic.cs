using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class BookingDtoStatistic
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string courtName { get; set; }
        public string courtClusterName { get; set; }
        public string ImageUrl { get; set; }
    }
}