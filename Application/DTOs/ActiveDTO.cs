using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ActiveDTO
    {
        public string Id { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
