using System.ComponentModel.DataAnnotations;
using Domain;
using Domain.Entity;

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