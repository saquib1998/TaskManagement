using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs
{
    public class TeamRequest
    {
        [Required]
        public string Name { get; set; }
    }
}
