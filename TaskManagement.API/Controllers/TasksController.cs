﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs;
using TaskManagement.API.Extensions;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TasksController(UserManager<AppUser> userManager, AppDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Gets pending tasks for the current user.
    /// </summary>
    /// <returns>A list of <see cref="TaskResponse"/></returns>
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
            AssignedUserEmail = x.AssignedUser.Email,
            TeamId = x.AssignedUser.TeamId
        });

        return Ok(response);
    }

    /// <summary>
    /// Gets a Task by id.
    /// </summary>
    /// <param name="taskId">The task id.</param>
    /// <returns><see cref="TaskResponse"/></returns>
    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetTask(int taskId)
    {
        var task = await dbContext.Tasks.Include(x => x.AssignedUser)
                                        .Include(x => x.Documents)
                                        .SingleOrDefaultAsync(t => t.Id == taskId);

        if (task is null)
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
            Title = task.Title,
            Description = task.Description,
            DueDate = new DateTime(task.DueDate.Year, task.DueDate.Month, task.DueDate.Day),
            AssignedUserEmail = task.AssignedUser.Email,
            TeamId = task.AssignedUser.TeamId,
            Comments = comments,
            DocumentIds = task.Documents.Select(x => x.Id)
        });
    }

    [HttpGet("/teams/{teamId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetTasksByTeam(int teamId, [Required] DateTime dueDateStart, [Required] DateTime dueEndDate)
    {
        if(dueDateStart < dueEndDate)
            return BadRequest(new ApiResponse(404, "start date should be smaller than end date"));

        var team = await dbContext.Teams.FindAsync(teamId);

        if (team is null)
            return BadRequest(new ApiResponse(404, "Team not found"));

        var tasks = await dbContext.Tasks
                                   .Include(x => x.AssignedUser)
                                   .Where(x => x.DueDate < new DateOnly(dueEndDate.Year, dueEndDate.Month, dueEndDate.Day) &&
                                               x.DueDate >= new DateOnly(dueDateStart.Year, dueDateStart.Month, dueDateStart.Day) &&
                                               x.AssignedUser.TeamId == teamId)
                                   .ToListAsync();

        var response = tasks.Select(x => new TaskResponse
        {
            Id = x.Id,
            Status = x.Status,
            Title = x.Title,
            Description = x.Description,
            DueDate = new DateTime(x.DueDate.Year, x.DueDate.Month, x.DueDate.Day),
            AssignedUserEmail = x.AssignedUser.Email,
            TeamId = x.AssignedUser.TeamId,
        });

        return Ok(response);
    }

    /// <summary>
    /// Adds a comment to a particular task.
    /// </summary>
    /// <param name="taskId">The task id.</param>
    /// <param name="commentRequest">The comment body.</param>
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
    /// Creates a new task.
    /// </summary>
    /// <param name="request">The create request.</param>
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

    /// <summary>
    /// Updates a task.
    /// </summary>
    /// <param name="request">The update request.</param>
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
