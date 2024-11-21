using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ActiveDto
    {
        public string username { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
