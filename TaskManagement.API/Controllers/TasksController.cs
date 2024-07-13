using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs;
using TaskManagement.API.Extensions;
using TaskManagement.DataAccess.Data;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TasksController(UserManager<AppUser> userManager, AppDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Gets pending tasks for the current user.
    /// </summary>
    /// <returns>Pending Tasks.</returns>
    [HttpGet]
    public async Task<IActionResult> GetPendingTasks()
    {
        var user = await dbContext
                            .Users
                            .Include(u => u.Tasks.Where(t => t.Status != Status.Closed))
                            .SingleOrDefaultAsync(u => u.Email == User.FindFirstValue(ClaimTypes.Email));

        var response = user.Tasks.Select(x => new TaskResponse
        {
            Id = x.Id,
            Status = x.Status,
            Title = x.Title,
            Description = x.Description,
            DueDate = new DateTime(x.DueDate.Year, x.DueDate.Month, x.DueDate.Day),
            AssignedTo = x.AssignedUser.Email,
        });

        return Ok(response);
    }

    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetTask(int taskId)
    {
        var task = await dbContext.Tasks.Include(x => x.AssignedUser)
                                        .Include(x => x.Documents)
                                        .SingleOrDefaultAsync(t => t.Id == taskId);

        if(task is null)
        {
            return NotFound(new ApiResponse(404, "The task doesn't exist."));
        }

        var comments = await dbContext
                                .Comments
                                .Include(x => x.Author)
                                .Where(x => x.TaskDetailsId == taskId)
                                .Select(x => new CommentResponse
                                {
                                    Id = x.Id,
                                    Content = x.Content,
                                    TaskDetailsId = taskId,
                                    AuthorEmail = x.Author.Email
                                }).ToListAsync();

        return Ok(new TaskResponse
        {
            Id = task.Id,
            Status = task.Status,
            Title= task.Title,
            Description = task.Description,
            DueDate = new DateTime(task.DueDate.Year, task.DueDate.Month, task.DueDate.Day),
            AssignedTo = task.AssignedUser.Email,
            Comments = comments,
            DocumentIds = task.Documents.Select(x => x.Id)
        });    
    }

    [HttpPost("{taskId}/comment")]
    public async Task<IActionResult> Comment(int taskId, CommentRequest commentRequest)
    {
        var task = await dbContext.Tasks.Include(x => x.AssignedUser)
                                        .SingleOrDefaultAsync(t => t.Id == taskId);

        if (task is null)
        {
            return NotFound(new ApiResponse(404, "The task doesn't exist."));
        }

        var author = await userManager.FindByEmailFromClaimsPrincipal(User);

        var comment = new Comment
        {
            Content = commentRequest.Content,
            TaskDetailsId = taskId,
            Task = task,
            Author = author
        };



        await dbContext.Comments.AddAsync(comment);
        await dbContext.SaveChangesAsync();

        return Created();
    }

    /// <summary>
    /// Gets pending tasks for all the users before a given due date.
    /// </summary>
    /// <returns>Pending Tasks.</returns>
    [HttpGet("/all")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetTasks(DateTime beforeDue, int top = 25, int skip = 0)
    {
        var tasks = await dbContext
                            .Tasks
                            .Where(x => x.Status != Status.Closed && 
                                        x.DueDate < new DateOnly(beforeDue.Year, beforeDue.Month, beforeDue.Day))
                            .Include(x => x.AssignedUser)
                            .Skip(skip)
                            .Take(top)
                            .ToListAsync();

        var response = tasks.Select(x => new TaskResponse
        {
            Id = x.Id,
            Status = x.Status,
            Title = x.Title,
            Description = x.Description,
            DueDate = new DateTime(x.DueDate.Year, x.DueDate.Month, x.DueDate.Day),
            AssignedTo = x.AssignedUser.Email,
        });

        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskRequest request)
    {
        AppUser appUser = null;
        if (request.AssignedTo is not null)
        {
            var user = await userManager.FindByEmailAsync(request.AssignedTo);
            if (user == null)
            {
                return BadRequest(new ApiResponse(400, "Cannot assign to a user that doesnt exist"));
            }
            appUser = user;
        }

        var newTask = new TaskDetails
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = new DateOnly(request.DueDate.Year, request.DueDate.Month, request.DueDate.Day),
            Status = request.Status,
            AssignedUser = appUser
        };

        await dbContext.Tasks.AddAsync(newTask);
        await dbContext.SaveChangesAsync();

        return Created();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTask(UpdateTaskRequest request)
    {
        var task = await dbContext.Tasks.SingleOrDefaultAsync(x => x.Id == request.Id);

        if (task is null)
        {
            return NotFound(new ApiResponse(404, "The task doesn't exist"));
        }

        AppUser appUser = null;
        if (request.AssignedTo is not null)
        {
            var user = await userManager.FindByEmailAsync(request.AssignedTo);
            if (user == null)
            {
                return BadRequest(new ApiResponse(400, "Cannot assign to a user that doesnt exist"));
            }
            appUser = user;
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.DueDate = new DateOnly(request.DueDate.Year, request.DueDate.Month, request.DueDate.Day);
        task.Status = request.Status;
        task.AssignedUser = appUser;

        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}
