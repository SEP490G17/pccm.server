using System.ComponentModel.DataAnnotations;
using Domain.Entity;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class AppUser : IdentityUser,IEntity
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(255)]
        public string ImageUrl { get; set; }

        [StringLength(12)]
        public string CitizenIdentification { get; set; }

        public bool IsDisabled { get; set; } = false;
    }
}