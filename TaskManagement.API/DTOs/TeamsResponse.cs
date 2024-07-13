namespace TaskManagement.API.DTOs
{
    public class TeamsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ManagerId { get; set; }
        public string Manager { get; set; }
        public IEnumerable<string> Members { get; set; }
    }
}
