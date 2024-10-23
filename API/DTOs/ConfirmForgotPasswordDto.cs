namespace Application.DTOs
{
    public class ConfirmForgotPasswordDto 
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }

    }
}