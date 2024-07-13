using System.ComponentModel.DataAnnotations;
using TaskManagement.DataAccess.Data;

namespace TaskManagement.API.DTOs;

public class CreateTaskRequest
{
    [Required]
    public string Title { get; set; }
    public string Description { get; set; }
    [Required]
    public DateTime DueDate { get; set; }
    public Status Status { get; set; }

    [EmailAddress]
    public string AssignedTo { get; set; }
}
