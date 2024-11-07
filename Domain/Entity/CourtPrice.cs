using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class CourtPrice : BaseEntity
    {
        [StringLength(150)] public string DisplayName { get; set; }
        public decimal Price { get; set; }
        public Court Court { get; set; }
        [Column(TypeName = "TIME")] public TimeOnly FromTime { get; set; }
        [Column(TypeName = "TIME")] public TimeOnly ToTime { get; set; }
    }
}