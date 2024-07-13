using TaskManagement.DataAccess.Data;

namespace TaskManagement.API.Data;

public class TaskDetails
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateOnly DueDate { get; set; }
    public Status Status { get; set; }
    public List<Comment> Comments { get; set; }
    public AppUser AssignedUser { get; set; }
}
