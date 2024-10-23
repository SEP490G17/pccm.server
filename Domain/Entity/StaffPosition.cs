namespace Domain.Entity
{
    public class StaffPosition : BaseEntity
    {
        public string Name { get; set; }
        public string[] DefaultRoles { get; set; }
    }
}