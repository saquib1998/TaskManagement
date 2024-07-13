using System.ComponentModel.DataAnnotations;
using TaskManagement.API.Data;

namespace TaskManagement.API.DTOs
{
    public class RegisterRequest
    {
        [Required]
        public string DisplayName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(4)]
        public string Password { get; set; }

        public Role Role { get; set; }
    }
}
