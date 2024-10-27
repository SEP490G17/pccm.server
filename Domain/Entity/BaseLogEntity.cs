using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class BaseLogEntity:BaseEntity
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public AppUser Creator { get; set; }
    }
}