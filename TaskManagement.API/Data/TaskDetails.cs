namespace TaskManagement.API.Data;

public class TaskDetails
{
    public int Id { get; set; }
    public string Description { get; set; }
    public DateOnly DueDate { get; set; }
    public TaskStatus Status { get; set; }
    public List<Comment> Comments { get; set; }
}
