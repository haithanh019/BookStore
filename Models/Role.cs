using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Role
    {
        public int RoleID { get; set; }

        [Required, MaxLength(50)]
        public string? RoleName { get; set; }
    }
}
