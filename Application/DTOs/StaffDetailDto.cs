using Microsoft.AspNetCore.Identity;

namespace Domain.Entity
{
    public class StaffDetailDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public List<IdentityRole> RoleAdd { get; set; }
        public int[] CourtCluster { get; set; }
        public string PhoneNumber { get; set; }
    }
}