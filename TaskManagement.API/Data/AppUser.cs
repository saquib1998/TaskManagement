using Microsoft.AspNetCore.Identity;

namespace TaskManagement.API.Data
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
    }
}
