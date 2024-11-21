using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class CourtCombo
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public int CourtId { get; set; }
        public decimal TotalPrice { get; set; }
        public int Duration { get; set; }

        [ForeignKey("CourtId")]
        public Court Court { get; set; }
    }

}