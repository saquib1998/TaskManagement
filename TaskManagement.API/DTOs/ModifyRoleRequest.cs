using System.ComponentModel.DataAnnotations;
using TaskManagement.API.Data;

namespace TaskManagement.API.DTOs
{
    public class ModifyRoleRequest
    {
        public Role Role { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
