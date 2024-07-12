using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TasksController : ControllerBase
{

    [HttpGet]
    [Authorize]
    public Task<IActionResult> GetPendingTasks()
    {
        throw new NotImplementedException();
    }
}
