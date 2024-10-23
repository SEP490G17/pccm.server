namespace Application.DTOs
{
    public class ConfirmChangePasswordDto 
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}