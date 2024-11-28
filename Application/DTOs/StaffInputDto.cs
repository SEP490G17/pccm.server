namespace Domain.Entity
{
    public class StaffInputDto
    {
        public int? StaffDetailId { get; set; }
        public string FirstName { get; set; }
        public string userName { get; set; }
        public string Email { get; set; }
        public int PositionId { get; set; }
        public int[] CourtCluster { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Password { get; set; }
    }
}