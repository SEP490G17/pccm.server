using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Entity
{
    public class Expense : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string ExpenseName { get; set; }  // Tên của chi tiêu

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Chi phí phải lớn hơn hoặc bằng 0.")]
        public decimal TotalPrice { get; set; }  // Số tiền chi tiêu

        private DateTime _expenseAt;
        public int? CourtClusterId { get; set; }
        [ForeignKey("CourtClusterId")]
        [AllowNull]
        public virtual CourtCluster CourtCluster { get; set; }

        [NotMapped]
        public int ExpenseYear
        {
            get => _expenseAt.Year;
            set => _expenseAt = new DateTime(value, _expenseAt.Month, 1);
        }

        [NotMapped]
        public int ExpenseMonth
        {
            get => _expenseAt.Month;
            set => _expenseAt = new DateTime(_expenseAt.Year, value, 1);
        }

        [Column(TypeName = "date")]
        public DateTime ExpenseAt
        {
            get => _expenseAt;
            set => _expenseAt = new DateTime(value.Year, value.Month, 1);
        }
    }
}
