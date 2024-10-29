using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ExpenseResult
    {
        public string Date { get; set; } 
        public decimal TotalImportFee { get; set; } // Tổng số tiền nhập
    }
}
