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
    public class RolesController(UserManager<AppUser> userManager) : ControllerBase
    {

        /// <summary>
        /// Add a role to a given user.
        /// </summary>
        /// <param name="request">The request body.</param>
        [HttpPost("/add")]
        public async Task<IActionResult> ModifyRole(ModifyRoleRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return BadRequest(new ApiResponse(400, "User does not exist"));

            await userManager.AddToRoleAsync(user, request.Role.ToString());

            return NoContent();
        }
    }
}
