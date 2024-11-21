using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entity;

namespace Application.DTOs
{
    public class SaveRevenueDto
    {
        private DateTime DateSave { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date
        {
            get => DateSave;
            set => DateSave = new DateTime(value.Year, value.Month, 1);
        }
        public int courtClusterId { get; set; }
        [ForeignKey("CourtClusterId")]
        public virtual CourtCluster? CourtCluster { get; set; }
        public List<OrderProductDetailDto> OrderProductDetails { get; set; }
        public List<BookingDetailDto> BookingDetails { get; set; }
        public List<OrderServiceDetailDto> OrderServiceDetails { get; set; }
        public List<ExpenseDetailDto> ExpenseDetails { get; set; }
    }
}