using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class CourtPrice:BaseEntity
    {
        public decimal Price { get; set; }
        public Court Court { get; set; }
        [Column(TypeName ="TIME")]
        public TimeOnly MyProperty { get; set; }
    }
}