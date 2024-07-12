using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TasksController : ControllerBase
{

    [HttpGet]
    public Task<IActionResult> GetTasks()
    {
        throw new NotImplementedException();
    }
}
