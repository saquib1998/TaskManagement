using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.API.Data;

namespace TaskManagement.API.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<AppUser> FindByEmailFromClaimsPrincipal(this UserManager<AppUser> userManager,
            ClaimsPrincipal user)
        {
            return await userManager.Users
                .SingleOrDefaultAsync(x => x.Email == user.FindFirstValue(ClaimTypes.Email));
        }
    }
}
