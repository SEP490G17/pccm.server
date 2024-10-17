using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}