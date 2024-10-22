using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class BaseEntity:IEntity
    {
        [Key]
        public int Id { get; set; }
    }
}