namespace TaskManagement.API.Data;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int TaskDetailsId { get; set; }
    public TaskDetails Task { get; set; }
}
