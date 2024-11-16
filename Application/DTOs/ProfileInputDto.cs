namespace Application.DTOs
{
    public class ProfileInputDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Address { get; set; }
    }
}
