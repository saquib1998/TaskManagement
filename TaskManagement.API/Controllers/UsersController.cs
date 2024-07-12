using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public UsersController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMe(AppDbContext context)
        {
            string userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await context.Users.FindAsync(userId);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            _signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, true, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized(TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized));
            }

            // The signInManager already produced the needed response in the form of a cookie or bearer token.
            return Ok(TypedResults.Empty);
        }
    }
}
