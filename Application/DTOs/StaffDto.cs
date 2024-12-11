namespace Domain.Entity
{
    public class StaffDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string[] Roles { get; set; }
        public string[] CourtCluster { get; set; }
        public string PhoneNumber { get; set; }
    }
}