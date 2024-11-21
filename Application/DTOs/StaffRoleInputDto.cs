using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class StaffRoleInputDto
    {
        public string name { get; set; }
        public string[] defaultRoles { get; set; }
    }
}