using System.ComponentModel.DataAnnotations;
using TaskManagement.DataAccess.Data;

namespace TaskManagement.API.DTOs;

public class UpdateTaskRequest
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public DateTime DueDate { get; set; }
    [Required]
    public Status Status { get; set; }
    [Required]
    [EmailAddress]
    public string AssignedTo { get; set; }
}
