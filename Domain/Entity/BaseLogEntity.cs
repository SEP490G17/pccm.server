namespace Domain.Entity
{
    public class BaseLogEntity:BaseEntity
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public AppUser Creator { get; set; }
    }
}