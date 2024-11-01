using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ActiveDto
    {
        public string Id { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
