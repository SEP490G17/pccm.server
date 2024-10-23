namespace Domain.Entity
{
    public class StaffDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string CCCD { get; set; }
        public string Position { get; set; }
        public string[] Roles { get; set; }
        public string Shift { get; set; } = "8:30 - 12:00";
        public string[]? CourtCluster { get; set; }
        public string PhoneNumber { get; set; }


    }
}