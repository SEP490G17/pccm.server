using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class IncomeResult
    {
        public string Date { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalBooking { get; set; }
    }
}
