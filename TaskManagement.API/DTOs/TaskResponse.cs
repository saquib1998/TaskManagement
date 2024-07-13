using TaskManagement.API.Data;
using TaskManagement.DataAccess.Data;

namespace TaskManagement.API.DTOs;

public class TaskResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public Status Status { get; set; }
    public string AssignedTo { get; set; }
    public List<CommentResponse> Comments { get; set; } 
}
