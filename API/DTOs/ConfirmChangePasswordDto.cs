using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ConfirmChangePasswordDto
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}