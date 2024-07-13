namespace TaskManagement.API.DTOs;

public class CommentResponse
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int TaskDetailsId { get; set; }
    public string AuthorEmail { get; set; }
}
public class CommentRequest
{
    public string Content { get; set; }
}

