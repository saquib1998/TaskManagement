using Microsoft.AspNetCore.Identity;

namespace TaskManagement.API.Data
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public List<TaskDetails> Tasks { get; set; }
        public List<Comment> Comments { get; set; }
        public int? TeamId { get; set; }
        public Team Team { get; set; }
    }
}
