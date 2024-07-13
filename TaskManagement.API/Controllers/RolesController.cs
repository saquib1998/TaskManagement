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
    public class RolesController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        public RolesController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("/add")]
        public async Task<IActionResult> ModifyRole(ModifyRoleRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return BadRequest(new ApiResponse(400, "User does not exist"));

            await _userManager.AddToRoleAsync(user, request.Role.ToString());

            return NoContent();
        }
    }
}
