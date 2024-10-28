using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class BaseNeedLogEntity:BaseEntity
    {

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? DeletedBy { get; set; }


        public AppUser Creator  { get; set; }
        public AppUser? Updater { get; set; }
        public AppUser? Deleter { get; set; }
    }
}