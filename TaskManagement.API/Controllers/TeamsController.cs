using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class TeamsController(UserManager<AppUser> userManager, AppDbContext dbContext) : ControllerBase
    {
        /// <summary>
        /// Gets teams for Admin.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            var teams = await dbContext.Teams
                           .Include(x => x.Members)
                           .Include(x => x.Manager)
                           .Select(x => new TeamsResponse
                           {
                               Id = x.Id,
                               Name = x.Name,
                               ManagerId = x.ManagerId,
                               Manager = x.Manager == null ? null : x.Manager.Email,
                               Members = x.Members.Select(x => x.Email)
                           })
                           .ToListAsync();

            return Ok(teams);
        }

        /// <summary>
        /// Create Teams.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTeam(TeamRequest request)
        {
            var team = new Team
            {
                Name = request.Name
            };

            await dbContext.Teams.AddAsync(team);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignToTeam(AssignRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return NotFound(new ApiResponse(404, "user doesnt exist");

            var team = await dbContext.Teams.FirstOrDefaultAsync(x => x.Id == request.Id);

            if(team == null)
                return NotFound(new ApiResponse(404, "Team not found"));

            user.TeamId = team.Id;
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        public class AssignRequest
        {
            [Required]
            public int Id { get; set; }
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
    }
}
