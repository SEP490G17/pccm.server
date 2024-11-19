using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Revenue : BaseEntity
    {
        public int courtClusterId { get; set; }
        public DateTime revenueAt { get; set; }
        public string BookingDetail { get; set; }
        public string ProductDetail { get; set; }
        public string ServiceDetail { get; set; }
        public string ExpenseDetail { get; set; }
    }
}