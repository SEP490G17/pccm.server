using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class StatisticInputDTO
    {
        public string? Year { get; set; }
        public string? Month { get; set; }
        public int? CourtClusterId { get; set; }
    }
}
