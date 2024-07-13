using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class TeamsController(UserManager<AppUser> userManager, AppDbContext dbContext) : ControllerBase
    {
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

    }
}
