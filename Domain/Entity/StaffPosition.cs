using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class StaffPosition : BaseEntity
    {
        public string Name { get; set; }
        public string[] DefaultRoles { get; set; }
    }
}