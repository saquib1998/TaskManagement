namespace TaskManagement.API.Data;

public class Document
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public byte[] Content { get; set; }
    public int TaskId { get; set; }
    public TaskDetails Task { get; set; }
}
