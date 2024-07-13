using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class RolesController(UserManager<AppUser> userManager, AppDbContext dbContext) : ControllerBase
    {

        /// <summary>
        /// Add a role to a given user.
        /// </summary>
        /// <param name="request">The request body.</param>
        [HttpPost("/modify")]
        public async Task<IActionResult> ModifyRole(ModifyRoleRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return BadRequest(new ApiResponse(400, "User does not exist"));

                if(request.TeamId is null)
                    return BadRequest(new ApiResponse(400, "Team Id is required if role is not admin."));

                var team = await dbContext.Teams.FindAsync(request.TeamId);

                if (team is null) return BadRequest(new ApiResponse(400, "Team does not exist"));

                user.TeamId = team.Id;

                if (request.Role == Role.Manager)
                {
                    team.ManagerId = user.Id;
                }

                await dbContext.SaveChangesAsync();

            await userManager.AddToRoleAsync(user, request.Role.ToString());

            return NoContent();
        }
    }
}
