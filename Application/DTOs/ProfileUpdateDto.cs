namespace Application.DTOs
{
    public class ProfileUpdateDto
    {
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public bool? Gender { get; set; }
        public string? BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
    }
}
