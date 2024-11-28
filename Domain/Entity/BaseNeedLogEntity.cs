using System.Diagnostics.CodeAnalysis;

namespace Domain.Entity
{
    public class BaseNeedLogEntity:BaseEntity
    {

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        [AllowNull]
        public string CreatedBy { get; set; }
        [AllowNull]
        public string UpdatedBy { get; set; }
        [AllowNull]
        public string DeletedBy { get; set; }


        public AppUser Creator  { get; set; }
        public AppUser Updater { get; set; }
        public AppUser Deleter { get; set; }
    }
}