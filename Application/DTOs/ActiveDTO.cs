using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ActiveDTO
    {
        public string Id { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
