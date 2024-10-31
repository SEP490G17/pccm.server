using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class TopStatisticDto
    {
        public List<StaffDetail> TopStaffs { get; set; }
        public List<Product> TopProducts { get; set; }
    }
}
